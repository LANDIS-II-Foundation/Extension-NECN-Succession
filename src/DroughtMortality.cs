//  Authors:  Robert M. Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;
using System;

namespace Landis.Extension.Succession.NECN
{
        /// <summary>
    /// Functions (optional) related to drought mortality, using either linear regression or threshold for CWD
    /// </summary>
    public class DroughtMortality
    {
        //Drought species params
        //these are from InputParameters.cs Lines 413-424
        //We want one value per species to be stored here. I guess we can reuse the AuxParm structure?
        //I admit I don't really understand what the get and return are doing or how to use them here
            
            // ROB TESTNG GITHUB DIRECT EDITS
        public Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold { get { return cwdThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold { get { return mortalityAboveThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Intercept { get { return intercept; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaAge { get { return betaAge; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaTemp { get { return betaTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom { get { return betaSWAAnom; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass { get { return betaBiomass; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaCWD { get { return betaCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD { get { return betaNormCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass { get { return intxnCWD_Biomass; } }

        public double[] ComputeDroughtMortality(ICohort cohort, ActiveSite site)
        {

            //Predictor variables
            double normalSWA = SiteVars.NormalSWA[site];
            //double normalSWA = 16.8; //placeholder -- mean of ponderosa meanSWA. Replace with normal SWA from map

            double normalCWD = SiteVars.NormalCWD[site];
            //PlugIn.ModelCore.UI.WriteLine("normalCWD is {0}", normalCWD);

            //TODO ned to divide SoilWater10 by swayear
            double swaAnom = SiteVars.SoilWater10[site] - normalSWA;
            //PlugIn.ModelCore.UI.WriteLine("swaAnom is {0}", swaAnom);

            double waterDeficit = SiteVars.AnnualClimaticWaterDeficit[site];

            double cohortAge = cohort.Age;
            double siteBiomass = SiteVars.ActualSiteBiomass(site);


            //Equation parameters
            //Rob: am I using "this" corectly here?
            int cwdThreshold = this.CWDThreshold[cohort.Species];
            //PlugIn.ModelCore.UI.WriteLine("cwdThreshold is {0}", cwdThreshold);

            double mortalityAboveThreshold = this.MortalityAboveThreshold[cohort.Species];

            double intercept = this.Intercept[cohort.Species];
            double betaAge = this.BetaAge[cohort.Species];
            double betaTemp = this.BetaTemp[cohort.Species];
            double betaSWAAnom = this.BetaSWAAnom[cohort.Species];
            double betaBiomass = this.BetaBiomass[cohort.Species];
            double betaCWD = this.BetaCWD[cohort.Species];
            double betaNormCWD = this.BetaNormCWD[cohort.Species];
            double intxnCWD_Biomass = this.IntxnCWD_Biomass[cohort.Species];
            //PlugIn.ModelCore.UI.WriteLine("Regression parameters are: intercept {0}, age {1}, temp {2}, SWAAnom {3}, biomass {4}", 
            //    intercept, betaAge, betaTemp, betaSWAAnom, betaBiomass);

            // double mortalitySlope = 0.005; //TODO make this a species-level param

            double p_mort = 0;
            double p_surv = 0;
            double M_leaf = 0;
            double M_wood = 0;

            if (waterDeficit > cwdThreshold & cwdThreshold != 0)
            {
                //p_mort = mortalityAboveThreshold + mortalitySlope * (waterDeficit - cwdThreshold); TODO implement
                p_mort = mortalityAboveThreshold;
                //PlugIn.ModelCore.UI.WriteLine("p_mort from CWD is", p_mort);
            }


            if (intercept != 0)
            {
                //calculate decadal log odds of survival
                //TODO we need to get the climate vars from a SiteVar, calculated in ComputeDroughtSiteVars
                double logOdds = intercept + betaAge * cohortAge + betaTemp * Temp7years + betaSWAAnom * swaAnom + betaBiomass * siteBiomass +
                    betaCWD * CWD10years + betaNormCWD * normalCWD + intxnCWD_Biomass * CWD10years * siteBiomass;
                p_surv = Math.Exp(logOdds) / (Math.Exp(logOdds) + 1);
                p_mort = (1 - Math.Pow(p_surv, 0.1));

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

            }

            //SiteVars.Cohorts[site].ReduceOrKillBiomassCohorts(this);
            //PlugIn.ModelCore.UI.WriteLine("CWD is {0}", waterDeficit);
            //PlugIn.ModelCore.UI.WriteLine("Drought wood mortality = {0}. Leaf mortality = {0}.", M_leaf, M_wood);

            double[] M_DROUGHT = new double[2] { M_wood, M_leaf };

            return M_DROUGHT;

        }

        private void ComputeDroughtSiteVars(ActiveSite site)
        {
            //TODO this only needs to be computed once per site per year (in month 5)

            int timestep = PlugIn.ModelCore.CurrentTime;

            double waterDeficit = SiteVars.AnnualClimaticWaterDeficit[site];
            //PlugIn.ModelCore.UI.WriteLine("curernt year CWD is {0}", waterDeficit);


            double[] soilWater = new double[0];
            double[] tempValue = new double[0];
            double[] cwdValue = new double[0];

            //For the first few years, use all of the temp and soilwater data, until lags can start to be used
            //Really not ideal! TODO fix this

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

            //TODO make these SiteVars so that ComputeDroughtMortality can use them
            SWA8years /= swayear;
            Temp7years /= tempyear;
            CWD10years /= cwdyear;

            //PlugIn.ModelCore.UI.WriteLine("temp is {0}", Temp7years);
        }

    }

}
