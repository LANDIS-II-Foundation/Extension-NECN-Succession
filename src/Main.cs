//  Authors: See User Guide

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using System.Collections.Generic;
using Landis.Library.Climate;
using System.Linq;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public class Main
    {
        public static int Year;
        public static int Month;
        public static int MonthCnt;

        /// <summary>
        /// Grows all cohorts at a site for a specified number of years.
        /// Litter is decomposed following the Century model.
        /// </summary>
        public static ISiteCohorts Run(ActiveSite site,
                                       int         years,
                                       bool        isSuccessionTimeStep)
        {
            
            SiteCohorts siteCohorts = SiteVars.Cohorts[site];
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            for (int y = 0; y < years; ++y) 
            {
                Year = y + 1;
                //PlugIn.ModelCore.UI.WriteLine("Timestep is = {0}.", PlugIn.ModelCore.CurrentTime);

                //if (Climate.Future_MonthlyData.ContainsKey(PlugIn.FutureClimateBaseYear + y + PlugIn.ModelCore.CurrentTime - years))
                //    ClimateRegionData.AnnualWeather[ecoregion] = Climate.Future_MonthlyData[PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime][ecoregion.Index];

                ClimateRegionData.AnnualClimate[ecoregion] = Climate.FutureEcoregionYearClimate[ecoregion.Index][Year - years + PlugIn.ModelCore.CurrentTime];

                //PlugIn.ModelCore.UI.WriteLine("PlugIn_FutureClimateBaseYear={0}, y={1}, ModelCore_CurrentTime={2}, CenturyTimeStep = {3}, SimulatedYear = {4}.", PlugIn.FutureClimateBaseYear, y, PlugIn.ModelCore.CurrentTime, years, (PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime));

                SiteVars.ResetAnnualValues(site);

                // Next, Grow and Decompose each month
                int[] months = new int[12]{6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5};

                int[] summer = new int[6] {6, 7, 8, 9, 4, 5}; //summer months for calculating summer T for drought mortality 
                //TODO have these be user defined so drought can be used in southern hemisphere

                //if (OtherData.CalibrateMode)//SF remove?
                //    months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                PlugIn.AnnualWaterBalance = 0;

                for (MonthCnt = 0; MonthCnt < 12; MonthCnt++)
                {
                    // Calculate mineral N fractions based on coarse root biomass.  Only need to do once per year.
                    if (MonthCnt == 0)
                    {
                        AvailableN.CalculateAnnualMineralNfraction(site);
                    }
                    //PlugIn.ModelCore.UI.WriteLine("SiteVars.MineralN = {0:0.00}, month = {1}.", SiteVars.MineralN[site], i);

                    Month = months[MonthCnt];

                    SiteVars.MonthlyAGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlyBGNPPcarbon[site][Month] = 0.0;
                    SiteVars.MonthlyNEE[site][Month] = 0.0;
                    SiteVars.MonthlyHeteroResp[site][Month] = 0.0;
                    SiteVars.MonthlySoilResp[site][Month] = 0.0;
                    SiteVars.MonthlyStreamN[site][Month] = 0.0;
                    SiteVars.MonthlyLAI[site][Month] = 0.0;
                    SiteVars.MonthlyLAI_Trees[site][Month] = 0.0;
                    SiteVars.MonthlyLAI_Grasses[site][Month] = 0.0; // Chihiro, 2020.03.30: tentative
                    SiteVars.MonthlySoilWaterContent[site][Month] = 0.0;
                    SiteVars.MonthlyMeanSoilWaterContent[site][Month] = 0.0;
                    SiteVars.MonthlyAnaerobicEffect[site][Month] = 0.0;
                    SiteVars.SourceSink[site].Carbon = 0.0;
                    SiteVars.TotalWoodBiomass[site] = ComputeWoodBiomass((ActiveSite) site);


                    var ppt = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPrecip[Month];

                    double monthlyNdeposition;
                    if  (PlugIn.Parameters.AtmosNintercept !=-1 && PlugIn.Parameters.AtmosNslope !=-1)
                        monthlyNdeposition = PlugIn.Parameters.AtmosNintercept + (PlugIn.Parameters.AtmosNslope * ppt);
                    else 
                    {
                        monthlyNdeposition = ClimateRegionData.AnnualClimate[ecoregion].MonthlyNDeposition[Month];
                    }

                    if (monthlyNdeposition < 0)
                    {
                        string mesg = string.Format("Error: Nitrogen deposition = {0}. PPT={1}. Ecoregion={2}, Month={3}", monthlyNdeposition, ppt, ecoregion.Name, Month);
                        throw new System.ApplicationException(mesg);
                    }

                    ClimateRegionData.MonthlyNDeposition[ecoregion][Month] = monthlyNdeposition;
                    ClimateRegionData.AnnualNDeposition[ecoregion] += monthlyNdeposition;
                    SiteVars.MineralN[site] += monthlyNdeposition;
                    //PlugIn.ModelCore.UI.WriteLine("Ndeposition={0},MineralN={1:0.00}.", monthlyNdeposition, SiteVars.MineralN[site]);

                    double liveBiomass = (double) ComputeLivingBiomass(siteCohorts);
                    double baseFlow, stormFlow, AET;

                    SoilWater.Run(y, Month, liveBiomass, site, out baseFlow, out stormFlow, out AET);

                    PlugIn.AnnualWaterBalance += ppt - AET;

                    // Calculate N allocation for each cohort
                    AvailableN.CalculateMonthlyMineralNallocation(site);

                    //Drought vars
                    //Update monthly temperature and soil water
                    //TODO SF add to DroughtMortality class. This is mostly already duplicated in the drought climate spinup
                    if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputTemperature | DroughtMortality.OutputClimateWaterDeficit)
                    {
                        //Initialize each year by adding a new list element
                        if (MonthCnt == 0)
                        {
                            
                            SiteVars.SoilWater10[site].Add(0);
                            SiteVars.Temp10[site].Add(0);

                        }

                        int list_index = SiteVars.SoilWater10[site].Count - 1; //track how many elements are in the climate lists, to avoid them growing too long

                       if (summer.Contains(Month))
                            {
                                //add monthly temperatures
                                SiteVars.Temp10[site][list_index] += ClimateRegionData.AnnualWeather[ecoregion].MonthlyTemp[Month];
                            }
                       //add monthly water content
                        SiteVars.SoilWater10[site][list_index] += SiteVars.MeanSoilWaterContent[site];

                        //Do this just once a year, after CWD is calculated above
                        if (MonthCnt == 11) //TODO fix this so we don't have to always calculate all of these vars when writing maps
                        {
                            SiteVars.CWD10[site].Add(SiteVars.AnnualClimaticWaterDeficit[site]);
                            SiteVars.Temp10[site][list_index] /= summer.Length; //get monthly average temp
                        }
                    }

                    if (DroughtMortality.UseDrought & MonthCnt == 11)
                    {
                        foreach (ISpecies species in PlugIn.ModelCore.Species)
                        {//TODO could speed up by only looping thorugh species with drought parameters (skip ones with zeroes)
                            DroughtMortality.ComputeDroughtLaggedVars(site, species);
                        }
                    }
                    //End drought calculations

                    if (MonthCnt==11)
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), true);
                    else
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), false);

                    // Track the grasses species LAI on the site
                    // Chihiro 2021.03.30: tentative
                    SiteVars.MonthlyLAI_GrassesLastMonth[site] = SiteVars.MonthlyLAI_Grasses[site][Month];

                    WoodLayer.Decompose(site);
                    LitterLayer.Decompose(site);
                    SoilLayer.Decompose(site);

                    // Volatilization loss as a function of the mineral N which remains after uptake by plants.  
                    // ML added a correction factor for wetlands since their denitrification rate is double that of wetlands
                    // based on a review paper by Seitziner 2006.

                    double volatilize = (SiteVars.MineralN[site] * PlugIn.Parameters.DenitrificationRate); 

                    //PlugIn.ModelCore.UI.WriteLine("BeforeVol.  MineralN={0:0.00}.", SiteVars.MineralN[site]);

                    SiteVars.MineralN[site] -= volatilize;
                    SiteVars.SourceSink[site].Nitrogen += volatilize;
                    SiteVars.Nvol[site] += volatilize;

                    SoilWater.Leach(site, baseFlow, stormFlow);

                    SiteVars.MonthlyNEE[site][Month] -= SiteVars.MonthlyAGNPPcarbon[site][Month];
                    SiteVars.MonthlyNEE[site][Month] -= SiteVars.MonthlyBGNPPcarbon[site][Month];
                    SiteVars.MonthlyNEE[site][Month] += SiteVars.SourceSink[site].Carbon;
                }

                //Do this just once a year, after CWD is calculated above
                if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputClimateWaterDeficit | DroughtMortality.OutputTemperature) //TODO fix this so we don't have to always calculate all of these vars when writing maps
                {
                    int year_index = PlugIn.ModelCore.CurrentTime - 1;

                    SiteVars.CWD10[site].Add(0);

                    if (year_index >= 10)
                    {
                        year_index = 9;
                    }

                    SiteVars.CWD10[site][year_index] = SiteVars.AnnualClimaticWaterDeficit[site];
                }

                if (DroughtMortality.UseDrought)
                {
                    foreach (ISpecies species in PlugIn.ModelCore.Species)
                    {//TODO could speed up by only looping thorugh species with drought parameters (skip ones with zeroes)
                        DroughtMortality.ComputeDroughtLaggedVars(site, species);
                    }
                }

                SiteVars.FineFuels[site] = (SiteVars.SurfaceStructural[site].Carbon + SiteVars.SurfaceMetabolic[site].Carbon) * 2.0;
            }


            ComputeTotalCohortCN(site, siteCohorts);

            return siteCohorts;
        }

        //---------------------------------------------------------------------

        public static int ComputeLivingBiomass(ISiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
            {
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        total += (int)(cohort.Data.AdditionalParameters.WoodBiomass + cohort.Data.AdditionalParameters.LeafBiomass);
                    }
                }
            }
            return total;
        }

        //---------------------------------------------------------------------

        public static int ComputeNeedleBiomass(ISiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
            {
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        total += (int)(cohort.Data.AdditionalParameters.LeafBiomass);
                    }
                }
            }
            return total;
        }
        //---------------------------------------------------------------------

        public static double ComputeWoodBiomass(ActiveSite site)
        {
            double woodBiomass = 0;
            if (SiteVars.Cohorts[site] != null)
            {
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        woodBiomass += cohort.Data.AdditionalParameters.WoodBiomass;
                    }
                }
            }
            return woodBiomass;
        }

        //---------------------------------------------------------------------
        public static void ComputeTotalCohortCN(ActiveSite site, ISiteCohorts cohorts)
        {
            SiteVars.CohortLeafC[site] = 0;
            SiteVars.CohortFRootC[site] = 0;
            SiteVars.CohortLeafN[site] = 0;
            SiteVars.CohortFRootN[site] = 0;
            SiteVars.CohortWoodC[site] = 0;
            SiteVars.CohortCRootC[site] = 0;
            SiteVars.CohortWoodN[site] = 0;
            SiteVars.CohortCRootN[site] = 0;

            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        CalculateCohortCN(site, cohort);
            return;
        }

        /// <summary>
        /// Summarize cohort C&N for output.
        /// </summary>
        private static void CalculateCohortCN(ActiveSite site, ICohort cohort)
        {
            ISpecies species = cohort.Species;

            double leafC = cohort.Data.AdditionalParameters.LeafBiomass * 0.47;
            double woodC = cohort.Data.AdditionalParameters.WoodBiomass * 0.47;

            double fRootC = Roots.CalculateFineRoot(cohort, leafC);
            double cRootC = Roots.CalculateCoarseRoot(cohort, woodC);

            double totalC = leafC + woodC + fRootC + cRootC;

            double leafN  = leafC /  (double) SpeciesData.LeafCN[species];
            double woodN = woodC / (double) SpeciesData.WoodCN[species];
            double cRootN = cRootC / (double) SpeciesData.CoarseRootCN[species];
            double fRootN = fRootC / (double) SpeciesData.FineRootCN[species];


            SiteVars.CohortLeafC[site] += leafC;
            SiteVars.CohortFRootC[site] += fRootC;
            SiteVars.CohortLeafN[site] += leafN;
            SiteVars.CohortFRootN[site] += fRootN;
            SiteVars.CohortWoodC[site] += woodC;
            SiteVars.CohortCRootC[site] += cRootC;
            SiteVars.CohortWoodN[site] += woodN ;
            SiteVars.CohortCRootN[site] += cRootN;

            return;

        }


    }
}
