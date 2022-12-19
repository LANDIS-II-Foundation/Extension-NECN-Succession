//  Authors: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using System.Collections.Generic;
using Landis.Library.Climate;

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
            
            ISiteCohorts siteCohorts = SiteVars.Cohorts[site];
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            for (int y = 0; y < years; ++y) {

                Year = y + 1;
                
                if (Climate.Future_MonthlyData.ContainsKey(PlugIn.FutureClimateBaseYear + y + PlugIn.ModelCore.CurrentTime - years))
                    ClimateRegionData.AnnualWeather[ecoregion] = Climate.Future_MonthlyData[PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime][ecoregion.Index];

                //PlugIn.ModelCore.UI.WriteLine("PlugIn_FutureClimateBaseYear={0}, y={1}, ModelCore_CurrentTime={2}, CenturyTimeStep = {3}, SimulatedYear = {4}.", PlugIn.FutureClimateBaseYear, y, PlugIn.ModelCore.CurrentTime, years, (PlugIn.FutureClimateBaseYear + y - years + PlugIn.ModelCore.CurrentTime));

                SiteVars.ResetAnnualValues(site);

                // Next, Grow and Decompose each month
                //int[] months = new int[12]{6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5};
                // KM: switched to running the months from 0 - 11 so that it would be in the right order for comparison with flux tower/ streamflow data 
                int[] months = new int[12]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};

                if(OtherData.CalibrateMode)
                    // KM: switched to running the months from 0 - 11 so that it would be in the right order for comparison with flux tower/ streamflow data 
                    months = new int[12]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}; ///This output will not match normal mode due to differences in initialization
                    //months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                PlugIn.AnnualWaterBalance = 0;
                double accumulated_precipitation = 0.0;
                for (MonthCnt = 0; MonthCnt < 12; MonthCnt++)
                {
                    // Calculate mineral N fractions based on coarse root biomass.  Only need to do once per year.
                    if (MonthCnt == 0)
                    {
                        AvailableN.CalculateMineralNfraction(site);
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
                    SiteVars.SourceSink[site].Carbon = 0.0;
                    SiteVars.TotalWoodBiomass[site] = Main.ComputeWoodBiomass((ActiveSite) site);
                    SiteVars.monthlyTranspiration[site][Month] = 0.0;
                                   
                    double ppt = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[Main.Month];
                    accumulated_precipitation += ppt;
                    SiteVars.MonthlyAccumPrecip[site][Month] += accumulated_precipitation;

                    double monthlyNdeposition;
                    if  (PlugIn.Parameters.AtmosNintercept !=-1 && PlugIn.Parameters.AtmosNslope !=-1)
                        monthlyNdeposition = PlugIn.Parameters.AtmosNintercept + (PlugIn.Parameters.AtmosNslope * ppt);
                    else 
                    {
                        monthlyNdeposition = ClimateRegionData.AnnualWeather[ecoregion].MonthlyNDeposition[Main.Month];
                    }

                    if (monthlyNdeposition < 0)
                        throw new System.ApplicationException("Error: Nitrogen deposition less than zero.");

                    ClimateRegionData.MonthlyNDeposition[ecoregion][Month] = monthlyNdeposition;
                    ClimateRegionData.AnnualNDeposition[ecoregion] += monthlyNdeposition;
                    SiteVars.MineralN[site] += monthlyNdeposition;
                    //PlugIn.ModelCore.UI.WriteLine("Ndeposition={0},MineralN={1:0.00}.", monthlyNdeposition, SiteVars.MineralN[site]);

                    // KM: move variables up 
                    double liveBiomass = (double) ComputeLivingBiomass(siteCohorts);
                    double baseFlow, stormFlow, AET;
                    double availableWaterMax, soilWaterContent;

                    // KM: Run the first half of the soil water routine necessary for growth/transpiration calcs 
                    SoilWater.Run_Henne_One(y, Month, liveBiomass, site, out availableWaterMax, out soilWaterContent);
                    
                    // KM: Calculate N allocation for each cohort
                    AvailableN.SetMineralNallocation(site);
                    
                    // KM: Calculate soil water allocation for each cohort
                    AvailableSoilWater.CalculateSWFraction(site);
                    AvailableSoilWater.SetSWAllocation(site);
                    AvailableSoilWater.SetCapWater(site);

                    if (MonthCnt==11)
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), true);
                    else
                        siteCohorts.Grow(site, (y == years && isSuccessionTimeStep), false);

                    // KM: After the growth/transpiration, run the second half of the soil water routine where water moves out of the cell 
                    SoilWater.Run_Henne_Two(y, Month, site, liveBiomass, availableWaterMax, soilWaterContent, out baseFlow, out stormFlow, out AET);
                    
                    // KM: Moved this down bc uses outputs from second half of soil water routine
                    PlugIn.AnnualWaterBalance += ppt - AET;

                    // Track the grasses species LAI on the site
                    // Chihiro 2021.03.30: tentative
                    SiteVars.MonthlyLAI_GrassesLastMonth[site] = SiteVars.MonthlyLAI_Grasses[site][Month];

                    WoodLayer.Decompose(site);
                    if (OtherData.CalibrateMode)
                    {
                        //PlugIn.ModelCore.UI.WriteLine("currentDeadWoodC:{0},{1},{2}", PlugIn.ModelCore.CurrentTime, Month, string.Join(", ", SiteVars.CurrentDeadWoodC[site]));
                       // PlugIn.ModelCore.UI.WriteLine("SurfaceDeadWoodC: {0},{1},{2}", PlugIn.ModelCore.CurrentTime, Month, SiteVars.SurfaceDeadWood[site].Carbon);
                    }
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
                    //SiteVars.FineFuels[site] = (System.Math.Min(1.0, (double) (PlugIn.ModelCore.CurrentTime - SiteVars.HarvestTime[site]) * 0.1));
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
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        total += (int) (cohort.WoodBiomass + cohort.LeafBiomass);
                    //total += ComputeBiomass(speciesCohorts);
            return total;
        }

        //---------------------------------------------------------------------

        public static int ComputeNeedleBiomass(ISiteCohorts cohorts)
        {
            int total = 0;
            if (cohorts != null)
                foreach (ISpeciesCohorts speciesCohorts in cohorts)
                    foreach (ICohort cohort in speciesCohorts)
                        total += (int)(cohort.LeafBiomass);
            //total += ComputeBiomass(speciesCohorts);
            return total;
        }
        //---------------------------------------------------------------------

        public static double ComputeWoodBiomass(ActiveSite site)
        {
            double woodBiomass = 0;
            if (SiteVars.Cohorts[site] != null)
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                    foreach (ICohort cohort in speciesCohorts)
                        woodBiomass += cohort.WoodBiomass;
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

            double leafC = cohort.LeafBiomass * 0.47;
            double woodC = cohort.WoodBiomass * 0.47;

            double fRootC = Roots.CalculateFineRoot(cohort, leafC);
            double cRootC = Roots.CalculateCoarseRoot(cohort, woodC);

            double totalC = leafC + woodC + fRootC + cRootC;

            double leafN  = leafC /  (double) SpeciesData.LeafCN[species];
            double woodN = woodC / (double) SpeciesData.WoodCN[species];
            double cRootN = cRootC / (double) SpeciesData.CoarseRootCN[species];
            double fRootN = fRootC / (double) SpeciesData.FineRootCN[species];

            //double totalN = woodN + cRootN + leafN + fRootN;

            //PlugIn.ModelCore.UI.WriteLine("month={0}, species={1}, leafB={2:0.0}, leafC={3:0.00}, leafN={4:0.0}, woodB={5:0.0}, woodC={6:0.000}, woodN={7:0.0}", Month, cohort.Species.Name, cohort.LeafBiomass, leafC, leafN, cohort.WoodBiomass, woodC, woodN);

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
