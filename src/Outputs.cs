//  Author: Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using System;
using System.Linq;
using Landis.Library.Metadata;
using Landis.Library.UniversalCohorts;


namespace Landis.Extension.Succession.NECN
{
    public class Outputs
    {
        public static MetadataTable<MonthlyLog> monthlyLog;
        public static MetadataTable<PrimaryLog> primaryLog;
        public static MetadataTable<PrimaryLogShort> primaryLogShort;
        public static MetadataTable<ReproductionLog> reproductionLog;
        public static MetadataTable<EstablishmentLog> establishmentLog;
        public static MetadataTable<CalibrateLog> calibrateLog;
        public static MetadataTable<DroughtLog> droughtLog;


        public static void WriteReproductionLog(int CurrentTime)
        {
            foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                reproductionLog.Clear();
                ReproductionLog rl = new ReproductionLog();

                rl.Time = CurrentTime;
                rl.SpeciesName = spp.Name;
                rl.NumCohortsPlanting = PlugIn.SpeciesByPlant[spp.Index];
                rl.NumCohortsSerotiny = PlugIn.SpeciesBySerotiny[spp.Index];
                rl.NumCohortsSeed = PlugIn.SpeciesBySeed[spp.Index];
                rl.NumCohortsResprout = PlugIn.SpeciesByResprout[spp.Index];

                reproductionLog.AddObject(rl);
                reproductionLog.WriteToFile();
            }

        }

        //---------------------------------------------------------------------
        public static void WriteShortPrimaryLogFile(int CurrentTime)
        {

            double avgNEEc = 0.0;
            double avgSOMtc = 0.0;
            double avgAGB = 0.0;
            double avgAGNPPtc = 0.0;
            double avgMineralN = 0.0;
            double avgDeadWoodC = 0.0;


            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                avgNEEc += SiteVars.AnnualNEE[site] / PlugIn.ModelCore.Landscape.ActiveSiteCount;
                avgSOMtc += GetOrganicCarbon(site) / PlugIn.ModelCore.Landscape.ActiveSiteCount;
                avgAGB += (double)Main.ComputeLivingBiomass(SiteVars.Cohorts[site]) / PlugIn.ModelCore.Landscape.ActiveSiteCount;
                avgAGNPPtc += SiteVars.AGNPPcarbon[site] / PlugIn.ModelCore.Landscape.ActiveSiteCount;
                avgMineralN += SiteVars.MineralN[site] / PlugIn.ModelCore.Landscape.ActiveSiteCount;
                avgDeadWoodC += SiteVars.SurfaceDeadWood[site].Carbon / PlugIn.ModelCore.Landscape.ActiveSiteCount;

            }

            primaryLogShort.Clear();
            PrimaryLogShort pl = new PrimaryLogShort();

            pl.Time = CurrentTime;
            pl.NEEC = avgNEEc;
            pl.SOMTC = avgSOMtc;
            pl.AGB = avgAGB;
            pl.AG_NPPC = avgAGNPPtc;
            pl.MineralN = avgMineralN;
            pl.C_DeadWood = avgDeadWoodC;

            primaryLogShort.AddObject(pl);
            primaryLogShort.WriteToFile();

        }

        //---------------------------------------------------------------------
        public static void WritePrimaryLogFile(int CurrentTime)
        {

            double[] avgAnnualPPT = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgJJAtemp = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgNEEc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOMtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgAGB = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgANPP = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgLeafB = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgWoodB = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgAGNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgBGNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgLittertc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgWoodMortality = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgMineralN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgGrossMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgTotalN = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgCohortLeafC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortFRootC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortWoodC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgWoodC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortCRootC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCRootC = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgCohortLeafN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortFRootN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortWoodN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCohortCRootN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgWoodN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCRootN = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSurfStrucC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSurfMetaC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilStrucC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilMetaC = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSurfStrucN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSurfMetaN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilStrucN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilMetaN = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSurfStrucNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSurfMetaNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilStrucNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilMetaNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSOM1surfC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM1soilC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM2C = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM3C = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSOM1surfN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM1soilN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM2N = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM3N = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgSOM1surfNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM1soilNetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM2NetMin = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOM3NetMin = new double[PlugIn.ModelCore.Ecoregions.Count];

            //doubl[] avgNDeposition = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgStreamC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgStreamN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgFireCEfflux = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgFireNEfflux = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgNvol = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgNresorbed = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgTotalSoilN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgNuptake = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgfrassC = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avglai = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] droughtMort = new double[PlugIn.ModelCore.Ecoregions.Count];

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                avgAnnualPPT[ecoregion.Index] = 0.0;
                avgJJAtemp[ecoregion.Index] = 0.0;

                avgNEEc[ecoregion.Index] = 0.0;
                avgSOMtc[ecoregion.Index] = 0.0;
                avgAGB[ecoregion.Index] = 0.0;
                avgANPP[ecoregion.Index] = 0.0;
                avgLeafB[ecoregion.Index] = 0.0;
                avgWoodB[ecoregion.Index] = 0.0;

                avgAGNPPtc[ecoregion.Index] = 0.0;
                avgBGNPPtc[ecoregion.Index] = 0.0;
                avgLittertc[ecoregion.Index] = 0.0;
                avgWoodMortality[ecoregion.Index] = 0.0;

                avgMineralN[ecoregion.Index] = 0.0;
                avgGrossMin[ecoregion.Index] = 0.0;
                avgTotalN[ecoregion.Index] = 0.0;

                avgCohortLeafC[ecoregion.Index] = 0.0;
                avgCohortFRootC[ecoregion.Index] = 0.0;
                avgCohortWoodC[ecoregion.Index] = 0.0;
                avgCohortCRootC[ecoregion.Index] = 0.0;
                avgWoodC[ecoregion.Index] = 0.0;
                avgCRootC[ecoregion.Index] = 0.0;

                avgSurfStrucC[ecoregion.Index] = 0.0;
                avgSurfMetaC[ecoregion.Index] = 0.0;
                avgSoilStrucC[ecoregion.Index] = 0.0;
                avgSoilMetaC[ecoregion.Index] = 0.0;

                avgCohortLeafN[ecoregion.Index] = 0.0;
                avgCohortFRootN[ecoregion.Index] = 0.0;
                avgCohortWoodN[ecoregion.Index] = 0.0;
                avgCohortCRootN[ecoregion.Index] = 0.0;
                avgWoodN[ecoregion.Index] = 0.0;
                avgCRootN[ecoregion.Index] = 0.0;

                avgSurfStrucN[ecoregion.Index] = 0.0;
                avgSurfMetaN[ecoregion.Index] = 0.0;
                avgSoilStrucN[ecoregion.Index] = 0.0;
                avgSoilMetaN[ecoregion.Index] = 0.0;

                avgSurfStrucNetMin[ecoregion.Index] = 0.0;
                avgSurfMetaNetMin[ecoregion.Index] = 0.0;
                avgSoilStrucNetMin[ecoregion.Index] = 0.0;
                avgSoilMetaNetMin[ecoregion.Index] = 0.0;

                avgSOM1surfC[ecoregion.Index] = 0.0;
                avgSOM1soilC[ecoregion.Index] = 0.0;
                avgSOM2C[ecoregion.Index] = 0.0;
                avgSOM3C[ecoregion.Index] = 0.0;

                avgSOM1surfN[ecoregion.Index] = 0.0;
                avgSOM1soilN[ecoregion.Index] = 0.0;
                avgSOM2N[ecoregion.Index] = 0.0;
                avgSOM3N[ecoregion.Index] = 0.0;

                avgSOM1surfNetMin[ecoregion.Index] = 0.0;
                avgSOM1soilNetMin[ecoregion.Index] = 0.0;
                avgSOM2NetMin[ecoregion.Index] = 0.0;
                avgSOM3NetMin[ecoregion.Index] = 0.0;

                //avgNDeposition[ecoregion.Index] = 0.0;
                avgStreamC[ecoregion.Index] = 0.0;
                avgStreamN[ecoregion.Index] = 0.0;
                avgFireCEfflux[ecoregion.Index] = 0.0;
                avgFireNEfflux[ecoregion.Index] = 0.0;
                avgNuptake[ecoregion.Index] = 0.0;
                avgNresorbed[ecoregion.Index] = 0.0;
                avgTotalSoilN[ecoregion.Index] = 0.0;
                avgNvol[ecoregion.Index] = 0.0;
                avgfrassC[ecoregion.Index] = 0.0;

                droughtMort[ecoregion.Index] = 0.0;
            }


            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {


                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                if (SiteVars.Cohorts[site] != null)
                {
                    foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                    {
                        foreach (ICohort cohort in speciesCohorts)
                        {
                            avgAGB[ecoregion.Index] += cohort.Data.Biomass;
                            avgANPP[ecoregion.Index] += cohort.Data.ANPP;
                            avgLeafB[ecoregion.Index] += cohort.Data.AdditionalParameters.LeafBiomass;
                            avgWoodB[ecoregion.Index] += cohort.Data.AdditionalParameters.WoodBiomass;
                        }
                    }
                }

                avgNEEc[ecoregion.Index] += SiteVars.AnnualNEE[site];
                avgSOMtc[ecoregion.Index] += GetOrganicCarbon(site);
                //avgAGB[ecoregion.Index] += Main.ComputeLivingBiomass();
                //avgANPP[ecoregion.Index] += Main.ComputeANPP(SiteVars.Cohorts[site]);

                avgAGNPPtc[ecoregion.Index] += SiteVars.AGNPPcarbon[site];
                avgBGNPPtc[ecoregion.Index] += SiteVars.BGNPPcarbon[site];
                avgLittertc[ecoregion.Index] += SiteVars.LitterfallC[site];
                avgWoodMortality[ecoregion.Index] += SiteVars.WoodMortality[site] * 0.47;

                avgMineralN[ecoregion.Index] += SiteVars.MineralN[site];
                avgTotalN[ecoregion.Index] += GetTotalNitrogen(site);
                avgGrossMin[ecoregion.Index] += SiteVars.GrossMineralization[site];

                avgCohortLeafC[ecoregion.Index] += SiteVars.CohortLeafC[site];
                avgCohortFRootC[ecoregion.Index] += SiteVars.CohortFRootC[site];
                avgCohortWoodC[ecoregion.Index] += SiteVars.CohortWoodC[site];
                avgCohortCRootC[ecoregion.Index] += SiteVars.CohortCRootC[site];
                avgWoodC[ecoregion.Index] += SiteVars.SurfaceDeadWood[site].Carbon;
                avgCRootC[ecoregion.Index] += SiteVars.SoilDeadWood[site].Carbon;

                avgSurfStrucC[ecoregion.Index] += SiteVars.SurfaceStructural[site].Carbon;
                avgSurfMetaC[ecoregion.Index] += SiteVars.SurfaceMetabolic[site].Carbon;
                avgSoilStrucC[ecoregion.Index] += SiteVars.SoilStructural[site].Carbon;
                avgSoilMetaC[ecoregion.Index] += SiteVars.SoilMetabolic[site].Carbon;

                avgSOM1surfC[ecoregion.Index] += SiteVars.SOM1surface[site].Carbon;
                avgSOM1soilC[ecoregion.Index] += SiteVars.SOM1soil[site].Carbon;
                avgSOM2C[ecoregion.Index] += SiteVars.SOM2[site].Carbon;
                avgSOM3C[ecoregion.Index] += SiteVars.SOM3[site].Carbon;

                avgCohortLeafN[ecoregion.Index] += SiteVars.CohortLeafN[site];
                avgCohortFRootN[ecoregion.Index] += SiteVars.CohortFRootN[site];
                avgCohortWoodN[ecoregion.Index] += SiteVars.CohortWoodN[site];
                avgCohortCRootN[ecoregion.Index] += SiteVars.CohortCRootN[site];
                avgWoodN[ecoregion.Index] += SiteVars.SurfaceDeadWood[site].Nitrogen;
                avgCRootN[ecoregion.Index] += SiteVars.SoilDeadWood[site].Nitrogen;

                avgSurfStrucN[ecoregion.Index] += SiteVars.SurfaceStructural[site].Nitrogen;
                avgSurfMetaN[ecoregion.Index] += SiteVars.SurfaceMetabolic[site].Nitrogen;
                avgSoilStrucN[ecoregion.Index] += SiteVars.SoilStructural[site].Nitrogen;
                avgSoilMetaN[ecoregion.Index] += SiteVars.SoilMetabolic[site].Nitrogen;

                avgSOM1surfN[ecoregion.Index] += SiteVars.SOM1surface[site].Nitrogen;
                avgSOM1soilN[ecoregion.Index] += SiteVars.SOM1soil[site].Nitrogen;
                avgSOM2N[ecoregion.Index] += SiteVars.SOM2[site].Nitrogen;
                avgSOM3N[ecoregion.Index] += SiteVars.SOM3[site].Nitrogen;
                avgTotalSoilN[ecoregion.Index] += GetTotalSoilNitrogen(site);

                avgSurfStrucNetMin[ecoregion.Index] += SiteVars.SurfaceStructural[site].NetMineralization;
                avgSurfMetaNetMin[ecoregion.Index] += SiteVars.SurfaceMetabolic[site].NetMineralization;
                avgSoilStrucNetMin[ecoregion.Index] += SiteVars.SoilStructural[site].NetMineralization;
                avgSoilMetaNetMin[ecoregion.Index] += SiteVars.SoilMetabolic[site].NetMineralization;

                avgSOM1surfNetMin[ecoregion.Index] += SiteVars.SOM1surface[site].NetMineralization;
                avgSOM1soilNetMin[ecoregion.Index] += SiteVars.SOM1soil[site].NetMineralization;
                avgSOM2NetMin[ecoregion.Index] += SiteVars.SOM2[site].NetMineralization;
                avgSOM3NetMin[ecoregion.Index] += SiteVars.SOM3[site].NetMineralization;

                //avgNDeposition[ecoregion.Index] = ClimateRegionData.AnnualNDeposition[ecoregion];
                avgStreamC[ecoregion.Index] += SiteVars.Stream[site].Carbon;
                avgStreamN[ecoregion.Index] += SiteVars.Stream[site].Nitrogen; //+ SiteVars.NLoss[site];
                avgFireCEfflux[ecoregion.Index] += SiteVars.FireCEfflux[site];
                avgFireNEfflux[ecoregion.Index] += SiteVars.FireNEfflux[site];
                avgNresorbed[ecoregion.Index] += SiteVars.ResorbedN[site];
                avgNuptake[ecoregion.Index] += GetSoilNuptake(site);
                avgNvol[ecoregion.Index] += SiteVars.Nvol[site];
                avgfrassC[ecoregion.Index] += SiteVars.FrassC[site];

                droughtMort[ecoregion.Index] += SiteVars.DroughtMort[site];

            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (!ecoregion.Active || ClimateRegionData.ActiveSiteCount[ecoregion] < 1)
                    continue;
                primaryLog.Clear();
                PrimaryLog pl = new PrimaryLog();

                pl.Time = CurrentTime;
                pl.ClimateRegionName = ecoregion.Name;
                pl.ClimateRegionIndex = ecoregion.Index;
                pl.NumSites = ClimateRegionData.ActiveSiteCount[ecoregion];

                pl.NEEC = (avgNEEc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SOMTC = (avgSOMtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.AGB = (avgAGB[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.ANPP = (avgANPP[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.LeafBiomass = (avgLeafB[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]); 
                pl.WoodBiomass = (avgWoodB[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]); 
                pl.AG_NPPC = (avgAGNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.BG_NPPC = (avgBGNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.Litterfall = (avgLittertc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.WoodMortalityBiomass = (avgWoodMortality[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.MineralN = (avgMineralN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.TotalN = (avgTotalN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.GrossMineralization = (avgGrossMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.TotalNdep = (ClimateRegionData.AnnualNDeposition[ecoregion] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_LiveLeaf = (avgCohortLeafC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_LiveFRoot = (avgCohortFRootC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_LiveWood = (avgCohortWoodC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_LiveCRoot = (avgCohortCRootC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadWood = (avgWoodC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadCRoot = (avgCRootC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadLeaf_Struc = (avgSurfStrucC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadLeaf_Meta = (avgSurfMetaC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadFRoot_Struc = (avgSoilStrucC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_DeadFRoot_Meta = (avgSoilMetaC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_SOM1surf = (avgSOM1surfC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_SOM1soil = (avgSOM1soilC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_SOM2 = (avgSOM2C[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.C_SOM3 = (avgSOM3C[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_Leaf = (avgCohortLeafN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_FRoot = (avgCohortFRootN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_Wood = (avgCohortWoodN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_CRoot = (avgCohortCRootN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadWood = (avgWoodN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadCRoot = (avgCRootN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadLeaf_Struc = (avgSurfStrucN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadLeaf_Meta = (avgSurfMetaN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadFRoot_Struc = (avgSoilStrucN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_DeadFRoot_Meta = (avgSoilMetaN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_SOM1surf = (avgSOM1surfN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_SOM1soil = (avgSOM1soilN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_SOM2 = (avgSOM2N[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.N_SOM3 = (avgSOM3N[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SurfStrucNetMin = (avgSurfStrucNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SurfMetaNetMin = (avgSurfMetaNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SoilStrucNetMin = (avgSoilStrucNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SoilMetaNetMin = (avgSoilMetaNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SOM1surfNetMin = (avgSOM1surfNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SOM1soilNetMin = (avgSOM1soilNetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SOM2NetMin = (avgSOM2NetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.SOM3NetMin = (avgSOM3NetMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.LeachedC = (avgStreamC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.LeachedN = (avgStreamN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.FireCEfflux = (avgFireCEfflux[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.FireNEfflux = (avgFireNEfflux[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.Nuptake = (avgNuptake[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.Nresorbed = (avgNresorbed[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.TotalSoilN = (avgTotalSoilN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.Nvol = (avgNvol[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                pl.FrassC = (avgfrassC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);

                pl.DroughtMort = (droughtMort[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);

                primaryLog.AddObject(pl);
                primaryLog.WriteToFile();
            }
            //Reset back to zero:
            //These are being reset here because fire effects are handled in the first year of the 
            //growth loop but the reporting doesn't happen until after all growth is finished.
            SiteVars.FireCEfflux.ActiveSiteValues = 0.0;
            SiteVars.FireNEfflux.ActiveSiteValues = 0.0;

        }

        public static void WriteMonthlyLogFile(int month)
        {
            double[] ppt = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] airtemp = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] pet = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgAET = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgCWD = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgTotalResp = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSoilResp = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgNEE = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] nitrogenDeposition = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] streamN = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] soilWaterContent = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] meanSoilWaterContent = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] anaerobicEffect = new double[PlugIn.ModelCore.Ecoregions.Count];


            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                ppt[ecoregion.Index] = 0.0;
                airtemp[ecoregion.Index] = 0.0;
                pet[ecoregion.Index] = 0.0;
                avgAET[ecoregion.Index] = 0.0;
                avgCWD[ecoregion.Index] = 0.0;
                avgNPPtc[ecoregion.Index] = 0.0;
                avgTotalResp[ecoregion.Index] = 0.0;
                avgSoilResp[ecoregion.Index] = 0.0;
                avgNEE[ecoregion.Index] = 0.0;
                nitrogenDeposition[ecoregion.Index] = 0.0;
                streamN[ecoregion.Index] = 0.0;
                soilWaterContent[ecoregion.Index] = 0.0;
                meanSoilWaterContent[ecoregion.Index] = 0.0;
                anaerobicEffect[ecoregion.Index] = 0.0;
            }

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                ppt[ecoregion.Index] = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPrecip[month];
                airtemp[ecoregion.Index] = ClimateRegionData.AnnualClimate[ecoregion].MonthlyTemp[month];
                pet[ecoregion.Index] = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPET[month]; //SF added 2023-6-27
                avgCWD[ecoregion.Index] += SiteVars.MonthlyClimaticWaterDeficit[site][month]; //SF added 2023-6-27
                avgAET[ecoregion.Index] += SiteVars.MonthlyActualEvapotranspiration[site][month]; //SF added 2023-6-27

                avgNPPtc[ecoregion.Index] += SiteVars.MonthlyAGNPPcarbon[site][month] + SiteVars.MonthlyBGNPPcarbon[site][month];
                avgTotalResp[ecoregion.Index] += SiteVars.MonthlyHeteroResp[site][month];
                avgSoilResp[ecoregion.Index] += SiteVars.MonthlySoilResp[site][month];
                avgNEE[ecoregion.Index] += SiteVars.MonthlyNEE[site][month];

                SiteVars.AnnualNEE[site] += SiteVars.MonthlyNEE[site][month];

                nitrogenDeposition[ecoregion.Index] = ClimateRegionData.MonthlyNDeposition[ecoregion][month];
                streamN[ecoregion.Index] += SiteVars.MonthlyStreamN[site][month];
                soilWaterContent[ecoregion.Index] += SiteVars.MonthlySoilWaterContent[site][month];
                meanSoilWaterContent[ecoregion.Index] += SiteVars.MonthlyMeanSoilWaterContent[site][month]; //SF added mean
                anaerobicEffect[ecoregion.Index] += SiteVars.MonthlyAnaerobicEffect[site][month]; //SF added 2023-4-11
            }


            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (ClimateRegionData.ActiveSiteCount[ecoregion] > 0)
                {
                    monthlyLog.Clear();
                    MonthlyLog ml = new MonthlyLog();

                    ml.Time = PlugIn.ModelCore.CurrentTime;
                    ml.Month = month + 1;
                    ml.ClimateRegionName = ecoregion.Name;
                    ml.ClimateRegionIndex = ecoregion.Index;

                    ml.NumSites = Convert.ToInt32(ClimateRegionData.ActiveSiteCount[ecoregion]);

                    ml.Precipitation = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPrecip[month];
                    ml.AirTemp = ClimateRegionData.AnnualClimate[ecoregion].MonthlyTemp[month];
                    ml.PET = ClimateRegionData.AnnualClimate[ecoregion].MonthlyPET[month];
                    ml.avgAET = (avgAET[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.avgCWD = (avgCWD[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.AvgTotalNPP_C = (avgNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.AvgHeteroRespiration = (avgTotalResp[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.AvgSoilRespiration = (avgSoilResp[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.avgNEE = (avgNEE[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.Ndep = nitrogenDeposition[ecoregion.Index];
                    ml.StreamN = (streamN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.SoilWaterContent = (soilWaterContent[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.meanSoilWaterContent = (meanSoilWaterContent[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.AnaerobicEffect = (anaerobicEffect[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);

                    monthlyLog.AddObject(ml);
                    monthlyLog.WriteToFile();
                }
            }

        }

        public static void WriteDroughtSpeciesFile(int CurrentTime)
        {
            
            double[][] droughtMort = new double[PlugIn.ModelCore.Ecoregions.Count][];
            double[][] droughtProb = new double[PlugIn.ModelCore.Ecoregions.Count][];
            int[][] numberCohorts = new int[PlugIn.ModelCore.Ecoregions.Count][];

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                droughtMort[ecoregion.Index] = new double[PlugIn.ModelCore.Species.Count];
                droughtProb[ecoregion.Index] = new double[PlugIn.ModelCore.Species.Count];
                numberCohorts[ecoregion.Index] = new int[PlugIn.ModelCore.Species.Count];

                for (int i = 0; i <= PlugIn.ModelCore.Species.Count - 1; i++)
                {
                    droughtMort[ecoregion.Index][i] = 0.0;
                    droughtProb[ecoregion.Index][i] = 0.0;
                    numberCohorts[ecoregion.Index][i] = 0;
                }
            }

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    droughtMort[ecoregion.Index][species.Index] += SiteVars.SpeciesDroughtMortality[site][species.Index];
                    droughtProb[ecoregion.Index][species.Index] += SiteVars.SpeciesDroughtProbability[site][species.Index];

                    //Get number of cohorts per species per site
                    ISpeciesCohorts cohortList = SiteVars.Cohorts[site][species];
                    if (cohortList != null)
                    {
                        foreach (ICohort cohort in cohortList)
                        {
                            numberCohorts[ecoregion.Index][cohort.Species.Index] += 1;
                        }
                    }
                }         
                 
            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (!ecoregion.Active || ClimateRegionData.ActiveSiteCount[ecoregion] < 1)
                    continue;
                
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    droughtLog.Clear();
                    DroughtLog dl = new DroughtLog();

                    dl.Time = CurrentTime;
                    dl.ClimateRegionName = ecoregion.Name;
                    dl.ClimateRegionIndex = ecoregion.Index;
                    dl.NumSites = ClimateRegionData.ActiveSiteCount[ecoregion];
                    dl.SpeciesName = species.Name;
                    dl.SpeciesIndex = species.Index;
                    dl.AverageBiomassKilled = droughtMort[ecoregion.Index][species.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion];
                    dl.AverageProbabilityMortality = droughtProb[ecoregion.Index][species.Index] / numberCohorts[ecoregion.Index][species.Index];
                    dl.NumberCohorts = numberCohorts[ecoregion.Index][species.Index];
                    //dl.TotalCohortsKilled(droughtMort[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    droughtLog.AddObject(dl);
                    droughtLog.WriteToFile();
                }
            }
            
        }


        public static void WriteMaps()
        {

            string pathH2O = MapNames.ReplaceTemplateVars(@"NECN\Annual-water-budget-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathH2O, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)((SiteVars.AnnualWaterBalance[site]));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string pathANPP = MapNames.ReplaceTemplateVars(@"NECN\AG_NPP-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathANPP, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)((SiteVars.AGNPPcarbon[site]));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }

            }


            string pathPET = MapNames.ReplaceTemplateVars(@"NECN\PET-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathPET, PlugIn.ModelCore.Landscape.Dimensions))
            {
                //string pathPET = MapNames.ReplaceTemplateVars(@"NECN\PET-{timestep}.img", PlugIn.ModelCore.CurrentTime);
                //using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathPET, PlugIn.ModelCore.Landscape.Dimensions))
                //{
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)((SiteVars.AnnualPotentialEvapotranspiration[site]));
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                //}
            }

            string path = MapNames.ReplaceTemplateVars(@"NECN\SOMTC-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)((SiteVars.SOM1surface[site].Carbon + SiteVars.SOM1soil[site].Carbon + SiteVars.SOM2[site].Carbon + SiteVars.SOM3[site].Carbon));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string path2 = MapNames.ReplaceTemplateVars(@"NECN\SoilN-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<ShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<ShortPixel>(path2, PlugIn.ModelCore.Landscape.Dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.MineralN[site] * 1000); //SF changed 2023-4-11
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string path4 = MapNames.ReplaceTemplateVars(@"NECN\ANEE-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<ShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<ShortPixel>(path4, PlugIn.ModelCore.Landscape.Dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.AnnualNEE[site] + 1000);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string path5 = MapNames.ReplaceTemplateVars(@"NECN\TotalC-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path5, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)(GetOrganicCarbon(site) +
                            SiteVars.CohortLeafC[site] +
                            SiteVars.CohortFRootC[site] +
                            SiteVars.CohortWoodC[site] +
                            SiteVars.CohortCRootC[site] +
                            SiteVars.SurfaceDeadWood[site].Carbon +
                            SiteVars.SoilDeadWood[site].Carbon);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string pathLAI = MapNames.ReplaceTemplateVars(@"NECN\LAI-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathLAI, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (short)(SiteVars.LAI[site]);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }

            }


            string pathavailablewater = MapNames.ReplaceTemplateVars(@"NECN\AvailableWater-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathavailablewater, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)((SiteVars.PlantAvailableWater[site]));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }

            }

            string pathsoilwater = MapNames.ReplaceTemplateVars(@"NECN\SoilWater-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathsoilwater, PlugIn.ModelCore.Landscape.Dimensions))
            {
                IntPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (int)((SiteVars.MeanSoilWaterContent[site])); //changed to mean
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }

            }

            if (PlugIn.Parameters.SmokeModelOutputs)
            {
                string pathNeedles = MapNames.ReplaceTemplateVars(@"NECN\ConiferNeedleBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathNeedles, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)(Main.ComputeNeedleBiomass(SiteVars.Cohorts[site]));
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }

                string pathDWD = MapNames.ReplaceTemplateVars(@"NECN\DeadWoodBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathDWD, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)(SiteVars.SurfaceDeadWood[site].Carbon * 2.0);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }

                string pathLitter = MapNames.ReplaceTemplateVars(@"NECN\SurfaceLitterBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathLitter, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)((SiteVars.SurfaceStructural[site].Carbon + SiteVars.SurfaceMetabolic[site].Carbon) * 2.0);
                            ;
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }




                string pathDeadWood = MapNames.ReplaceTemplateVars(@"NECN\SurfaceDeadWoodC-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathDeadWood, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)(SiteVars.SurfaceDeadWood[site].Carbon);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }

                }

                string pathDuff = MapNames.ReplaceTemplateVars(@"NECN\SurfaceDuffBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathDuff, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)(SiteVars.SOM1surface[site].Carbon * 2.0);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }
            }

            string pathAnerb= MapNames.ReplaceTemplateVars(@"NECN\AnaerobicEffect-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<ShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<ShortPixel>(pathAnerb, PlugIn.ModelCore.Landscape.Dimensions))
            {
                ShortPixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        //July anaerobic effect -- SF TODO make more flexible input, or mean for year
                        pixel.MapCode.Value = (short)(SiteVars.MonthlyAnaerobicEffect[site][7] * 1000);
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }

            }

            if (DroughtMortality.UseDrought)
            {
                string pathDrought = MapNames.ReplaceTemplateVars(@"NECN\DroughtMortality-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathDrought, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)(SiteVars.DroughtMort[site]);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }
            }
            if (DroughtMortality.OutputSoilWaterAvailable)
            {
                string pathSWA = MapNames.ReplaceTemplateVars(@"NECN\SWA-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(pathSWA, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    DoublePixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            if (SiteVars.SoilWater10[site].Count > 0)
                            {
                                pixel.MapCode.Value = (double)SiteVars.SoilWater10[site].Last();
                                //PlugIn.ModelCore.UI.WriteLine("Writing SoilWater value = {0}", pixel.MapCode.Value);
                            }
                            else
                            {
                                pixel.MapCode.Value = 0;
                            }
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }
            }
            if (DroughtMortality.OutputClimateWaterDeficit)
            {
                string pathCWD = MapNames.ReplaceTemplateVars(@"NECN\CWD-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(pathCWD, PlugIn.ModelCore.Landscape.Dimensions))

                {
                    DoublePixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)SiteVars.AnnualClimaticWaterDeficit[site];
                            //PlugIn.ModelCore.UI.WriteLine("Writing CWD value = {0}", pixel.MapCode.Value);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }
            }
            if (DroughtMortality.OutputTemperature)
            {
                string pathTemp = MapNames.ReplaceTemplateVars(@"NECN\SummerTemp-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(pathTemp, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    DoublePixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)SiteVars.Temp10[site].Last();
                            //PlugIn.ModelCore.UI.WriteLine("Writing Temp10 value = {0}", pixel.MapCode.Value);
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }
                }
            }
            if (DroughtMortality.WriteSpeciesDroughtMaps)
            {
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    string pathDroughtSpecies = DroughtMortality.SpeciesMapNames.ReplaceTemplateVars(@"NECN\DroughtMortality-{species}-{timestep}.tif", species.Name, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathDroughtSpecies, PlugIn.ModelCore.Landscape.Dimensions))
                    {
                        IntPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (int)SiteVars.SpeciesDroughtMortality[site][species.Index];
                            }
                            else
                            {
                                //  Inactive site
                                pixel.MapCode.Value = 0;
                            }
                            outputRaster.WriteBufferPixel();
                        }
                    }

                }

            }
        }
    


        // ---------------------------------------------------
        // This method created to create maps that could be used during a subsequent new model run.
        // These would be read as inputs for the next model run.
        public static void WriteCommunityMaps()
        {

            string input_map_1 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM1Nsurface-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_1, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM1surface[site].Nitrogen));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string input_map_2 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM1Nsoil-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_2, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM1soil[site].Nitrogen));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string input_map_3 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM2N-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_3, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM2[site].Nitrogen));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_4 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM3N-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_4, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM3[site].Nitrogen));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_5 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM1Csoil-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_5, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM1soil[site].Carbon));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }

            string input_map_6 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM2C-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_6, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM2[site].Carbon));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_7 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM3C-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_7, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM3[site].Carbon)); 
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_8 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\DeadWoodBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_8, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)(SiteVars.SurfaceDeadWood[site].Carbon / 0.47); //SF changed 2023-4-12
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_9 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\DeadRootBiomass-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_9, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)(SiteVars.SoilDeadWood[site].Carbon / 0.47); //SF changed 2023-4-12
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
            string input_map_10 = MapNames.ReplaceTemplateVars(@"NECN-Initial-Conditions\SOM1Csurface-{timestep}.tif", PlugIn.ModelCore.CurrentTime);
            using (IOutputRaster<DoublePixel> outputRaster = PlugIn.ModelCore.CreateRaster<DoublePixel>(input_map_10, PlugIn.ModelCore.Landscape.Dimensions))
            {
                DoublePixel pixel = outputRaster.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    if (site.IsActive)
                    {
                        pixel.MapCode.Value = (double)((SiteVars.SOM1surface[site].Carbon));
                    }
                    else
                    {
                        //  Inactive site
                        pixel.MapCode.Value = 0;
                    }
                    outputRaster.WriteBufferPixel();
                }
            }
        }
        //---------------------------------------------------------------------
        private static double GetTotalNitrogen(Site site)
        {
        
            
            double totalN =
                    + SiteVars.CohortLeafN[site]
                    + SiteVars.CohortFRootN[site] 
                    + SiteVars.CohortWoodN[site]
                    + SiteVars.CohortCRootN[site] 

                    + SiteVars.MineralN[site]
                    
                    + SiteVars.SurfaceDeadWood[site].Nitrogen
                    + SiteVars.SoilDeadWood[site].Nitrogen
                    
                    + SiteVars.SurfaceStructural[site].Nitrogen
                    + SiteVars.SoilStructural[site].Nitrogen
                    + SiteVars.SurfaceMetabolic[site].Nitrogen
                    + SiteVars.SoilMetabolic[site].Nitrogen

                    + SiteVars.SOM1surface[site].Nitrogen
                    + SiteVars.SOM1soil[site].Nitrogen
                    + SiteVars.SOM2[site].Nitrogen
                    + SiteVars.SOM3[site].Nitrogen
                    ;
        
            return totalN;
        }

        private static double GetTotalSoilNitrogen(Site site)
        {


            double totalsoilN =

                    +SiteVars.MineralN[site]

                    + SiteVars.SurfaceStructural[site].Nitrogen
                    + SiteVars.SoilStructural[site].Nitrogen
                    + SiteVars.SurfaceMetabolic[site].Nitrogen
            + SiteVars.SoilMetabolic[site].Nitrogen

            + SiteVars.SOM1surface[site].Nitrogen
            + SiteVars.SOM1soil[site].Nitrogen
            + SiteVars.SOM2[site].Nitrogen
            + SiteVars.SOM3[site].Nitrogen;
                    ;

            return totalsoilN;
        }
        //---------------------------------------------------------------------
        public static double GetOrganicCarbon(Site site)
        {
            double totalC = 
                    
                    SiteVars.SOM1surface[site].Carbon
                    + SiteVars.SOM1soil[site].Carbon
                    + SiteVars.SOM2[site].Carbon
                    + SiteVars.SOM3[site].Carbon
                    ;
        
            return totalC;
        }
        //---------------------------------------------------------------------
        private static double GetSoilNuptake(ActiveSite site)
        {
            double soilNuptake =

                    SiteVars.TotalNuptake[site]
                    
                    
                    ;

            return soilNuptake;
        }
        
    }
}
