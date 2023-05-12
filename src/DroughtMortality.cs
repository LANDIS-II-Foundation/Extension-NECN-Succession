//  Authors:  Samuel W. Flake, Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Landis.Extension.Succession.NECN
{
        /// <summary>
    /// Functions (optional) related to drought mortality, using either linear regression or threshold for CWD
    /// </summary>
    public class DroughtMortality
    {
        //Drought species params        
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold;
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold2;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold2;
        public static Landis.Library.Parameters.Species.AuxParm<double> Intercept;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaAge;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaTemp;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaCWD;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaNormTemp;
        public static Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass;  // needs better variable name

        public static bool UseDrought = false;
        public static bool WriteSWA;
        public static bool WriteCWD;
        public static bool WriteTemp;
        public static bool WriteSpeciesDroughtMaps;


        //---------------------------------------------------------------------
        // This initialize will be called from PlugIn, dependent on whether drought=true.
        public static void Initialize(IInputParameters parameters)
        {
            CWDThreshold            = parameters.CWDThreshold;
            MortalityAboveThreshold              = parameters.MortalityAboveThreshold;
            CWDThreshold2 = parameters.CWDThreshold2;
            MortalityAboveThreshold2 = parameters.MortalityAboveThreshold2;
            Intercept           = parameters.Intercept;
            BetaAge             = parameters.BetaAge;
            BetaTemp            = parameters.BetaTemp;
            BetaSWAAnom         = parameters.BetaSWAAnom;
            BetaBiomass         = parameters.BetaBiomass;
            BetaCWD             = parameters.BetaCWD;
            BetaNormCWD         = parameters.BetaNormCWD;
            BetaNormTemp           = parameters.BetaNormTemp;
            IntxnCWD_Biomass    = parameters.IntxnCWD_Biomass ;

            WriteSWA = parameters.WriteSWA;
            WriteCWD = parameters.WriteCWD;
            WriteTemp = parameters.WriteTemp;
            WriteSpeciesDroughtMaps = parameters.WriteSpeciesDroughtMaps;
            PlugIn.ModelCore.UI.WriteLine("UseDrought on initialization = {0}", UseDrought); //debug
        }

        public static class SpeciesMapNames
        {
            public const string SpeciesVar = "species";
            public const string TimestepVar = "timestep";

            private static IDictionary<string, bool> knownVars;
            private static IDictionary<string, string> varValues;

            //---------------------------------------------------------------------

            static SpeciesMapNames()
            {
                knownVars = new Dictionary<string, bool>();
                knownVars[SpeciesVar] = true;
                knownVars[TimestepVar] = true;

                varValues = new Dictionary<string, string>();
            }

            //---------------------------------------------------------------------

            public static void CheckTemplateVars(string template)
            {
                OutputPath.CheckTemplateVars(template, knownVars);
            }

            //---------------------------------------------------------------------

            public static string ReplaceTemplateVars(string template,
                                                     string species,
                                                     int timestep)
            {
                varValues[SpeciesVar] = species;
                varValues[TimestepVar] = timestep.ToString();
                return OutputPath.ReplaceTemplateVars(template, varValues);
            }
        }

        public static double[] ComputeDroughtMortality(ICohort cohort, ActiveSite site)
        {

            //Predictor variables
            double normalSWA = SiteVars.NormalSWA[site];
            //double normalSWA = 16.8; //placeholder -- mean of ponderosa meanSWA. Replace with normal SWA from map

            double normalCWD = SiteVars.NormalCWD[site];
            //PlugIn.ModelCore.UI.WriteLine("normalCWD is {0}", normalCWD);

            //TODO ned to divide SoilWater10 by swayear
            double swaAnom = SiteVars.SWALagged[site] - normalSWA;
            //PlugIn.ModelCore.UI.WriteLine("swaAnom is {0}", swaAnom);

            double tempLagged = SiteVars.TempLagged[site];

            double cwdLagged = SiteVars.CWDLagged[site];

            double waterDeficit = SiteVars.AnnualClimaticWaterDeficit[site];

            double cohortAge = cohort.Age;
            double siteBiomass = SiteVars.ActualSiteBiomass(site);


            //Equation parameters
            int cwdThreshold = CWDThreshold[cohort.Species];
            PlugIn.ModelCore.UI.WriteLine("cwdThreshold is {0}", cwdThreshold);

            double mortalityAboveThreshold = MortalityAboveThreshold[cohort.Species];
            int cwdThreshold2 = CWDThreshold2[cohort.Species];
            PlugIn.ModelCore.UI.WriteLine("cwdThreshold2 is {0}", cwdThreshold2);

            double mortalityAboveThreshold2 = MortalityAboveThreshold2[cohort.Species];

            double intercept = Intercept[cohort.Species];
            double betaAge = BetaAge[cohort.Species];
            double betaTemp = BetaTemp[cohort.Species];
            double betaSWAAnom = BetaSWAAnom[cohort.Species];
            double betaBiomass = BetaBiomass[cohort.Species];
            double betaCWD = BetaCWD[cohort.Species];
            double betaNormCWD = BetaNormCWD[cohort.Species];
            double betaNormTemp = BetaNormTemp[cohort.Species];
            double intxnCWD_Biomass = IntxnCWD_Biomass[cohort.Species];
            //PlugIn.ModelCore.UI.WriteLine("Regression parameters are: intercept {0}, age {1}, temp {2}, SWAAnom {3}, biomass {4}", 
            //    intercept, betaAge, betaTemp, betaSWAAnom, betaBiomass);

            // double mortalitySlope = 0.005; //TODO make this a species-level param

            double p_mort = 0;
            double p_surv = 0;
            double M_leaf = 0;
            double M_wood = 0;

            if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("lagged CWD is {0}", cwdLagged);

            if (cwdLagged > cwdThreshold & cwdThreshold != 0)
            {
                //p_mort = mortalityAboveThreshold + mortalitySlope * (waterDeficit - cwdThreshold); TODO implement
                p_mort = mortalityAboveThreshold;
                if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("p_mort from CWD is {0}", p_mort);
            }

            if (cwdLagged > cwdThreshold2 & cwdThreshold2 != 0)
            {
                //p_mort = mortalityAboveThreshold + mortalitySlope * (waterDeficit - cwdThreshold); TODO implement
                p_mort = mortalityAboveThreshold2;
                if(OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("p_mort from CWD2 is {0}", p_mort);
            }


            if (intercept != 0)
            {
                //calculate decadal log odds of survival
                //TODO we need to get the climate vars from a SiteVar, calculated in ComputeDroughtSiteVars
                double logOdds = intercept + betaAge * cohortAge + betaTemp * tempLagged + betaSWAAnom * swaAnom + betaBiomass * siteBiomass +
                    betaCWD * cwdLagged + betaNormCWD * normalCWD + betaNormTemp * normalTemp + intxnCWD_Biomass * cwdLagged * siteBiomass;
                p_surv = Math.Exp(logOdds) / (Math.Exp(logOdds) + 1);
                p_mort = (1 - Math.Pow(p_surv, 0.1));
                if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("p_mort from regression is {0}", p_mort);

            }


            double random = PlugIn.ModelCore.GenerateUniform();

            //PlugIn.ModelCore.UI.WriteLine("p_mort is {0}", p_mort);
            //PlugIn.ModelCore.UI.WriteLine("random is {0}", random);


            if (p_mort > random)
            {

                M_leaf = cohort.LeafBiomass;
                M_wood = cohort.WoodBiomass;
                double aboveground_Biomass_Died = M_leaf + M_wood;

                SiteVars.DroughtMort[site] += aboveground_Biomass_Died;
                SiteVars.SpeciesDroughtMortality[site][cohort.Species.Index] += aboveground_Biomass_Died;

            }

            //SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(this);
            //PlugIn.ModelCore.UI.WriteLine("CWD is {0}", waterDeficit);
            //PlugIn.ModelCore.UI.WriteLine("Drought wood mortality = {0}. Leaf mortality = {0}.", M_leaf, M_wood);

            double[] M_DROUGHT = new double[2] { M_wood, M_leaf };

            return M_DROUGHT;

        }

        public static void ComputeDroughtLaggedVars(ActiveSite site)
        {
            //TODO this only needs to be computed once per site per year (in month 5)

            int timestep = PlugIn.ModelCore.CurrentTime;

            double waterDeficit = SiteVars.AnnualClimaticWaterDeficit[site];
            //PlugIn.ModelCore.UI.WriteLine("curernt year CWD is {0}", waterDeficit);


            double[] soilWater = new double[0];
            double[] tempValue = new double[0];
            double[] cwdValue = new double[0];

            //For the first few years, use all of the temp and soilwater data, until lags can start to be used
            //TODO use spinup climate?

            //TODO have adjustable timelags as input variable?

            int currentyear = timestep + 1;
            //PlugIn.ModelCore.UI.WriteLine("current year is ", currentyear);
            int swayear = currentyear;
            int tempyear = currentyear;
            int cwdyear = currentyear;
            if (currentyear >= 10)
            {
                currentyear = 10;
                swayear = 8;
                tempyear = 7;
                cwdyear = 10;
            }
            else if (currentyear == 9 | currentyear == 8)
            {
                swayear = 8;
                tempyear = 7;
                cwdyear = currentyear;
            }

            //PlugIn.ModelCore.UI.WriteLine("SoilWater10 is {0}", SiteVars.SoilWater10[site]);

            //soilWater should already have just 10 elements
            soilWater = SiteVars.SoilWater10[site].OrderByDescending(s => s).Reverse().Take(swayear).ToArray();
            tempValue = SiteVars.Temp10[site].OrderByDescending(s => s).Take(tempyear).ToArray();
            cwdValue = SiteVars.cwd10[site].OrderByDescending(s => s).Take(cwdyear).ToArray();

            //PlugIn.ModelCore.UI.WriteLine("Time-lagged CWD is {0}", cwdValue);

            //get SWA8yrs and Temp7yrs           

            //initialize variables for lowest SWA of highest 8 years of 10 and highest temperature for 7 years out of 10
            double SWA8years = 0;
            double Temp7years = 0;
            double CWD10years = 0;

            //Sum values selected above 
            Array.ForEach(soilWater, i => SWA8years += i);
            Array.ForEach(tempValue, i => Temp7years += i);
            Array.ForEach(cwdValue, i => CWD10years += i);


            SWA8years /= swayear;
            Temp7years /= tempyear;
            CWD10years /= cwdyear;

            SiteVars.SWALagged[site] = SWA8years; //TODO can we store these in DroughtMortality instead of SiteVars?
            SiteVars.TempLagged[site] = Temp7years;
            SiteVars.CWDLagged[site] = CWD10years;
            //PlugIn.ModelCore.UI.WriteLine("temp is {0}", Temp7years);
        }

    }
}
