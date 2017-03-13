//  Copyright 2007-2016 Portland State University
//  Author: Robert Scheller

//using Landis.Cohorts;
using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.IO;
using System;

using Landis.Library.Metadata;
using System.Data;
using System.Collections.Generic;
using System.Collections;



namespace Landis.Extension.Succession.NECN
{
    public class Outputs
    {
        public static StreamWriter CalibrateLog;
        public static MetadataTable<MonthlyLog> monthlyLog; 
        public static MetadataTable<PrimaryLog> primaryLog;
        public static MetadataTable<PrimaryLogShort> primaryLogShort;


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
                avgAGB += Century.ComputeLivingBiomass(SiteVars.Cohorts[site]) / PlugIn.ModelCore.Landscape.ActiveSiteCount; 
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

            PlugIn.SWHC_List.Sort();
            int soil_count = PlugIn.SWHC_List.Count;

            double[,] avgAnnualPPT = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgJJAtemp = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgNEEc = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOMtc = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgAGB = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgAGNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgBGNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgLittertc = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgWoodMortality = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgMineralN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgGrossMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgTotalN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgCohortLeafC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortFRootC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortWoodC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgWoodC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortCRootC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCRootC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgCohortLeafN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortFRootN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortWoodN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCohortCRootN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgWoodN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgCRootN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSurfStrucC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSurfMetaC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilStrucC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilMetaC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSurfStrucN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSurfMetaN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilStrucN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilMetaN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSurfStrucNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSurfMetaNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilStrucNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSoilMetaNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSOM1surfC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM1soilC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM2C = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM3C = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSOM1surfN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM1soilN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM2N = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM3N = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            double[,] avgSOM1surfNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM1soilNetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM2NetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgSOM3NetMin = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            //double[] avgNDeposition = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[,] avgStreamC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgStreamN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgFireCEfflux = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgFireNEfflux = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgNvol = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgNresorbed = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgTotalSoilN = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgNuptake = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avgfrassC = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];
            double[,] avglai = new double[PlugIn.ModelCore.Ecoregions.Count, soil_count];

            //int swhc_cnt = 0;

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                foreach (int swhc in PlugIn.SWHC_List)
                {
                    int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc);

                    // TO DO: ADD SWHC LOOP
                    avgAnnualPPT[ecoregion.Index, swhc_index] = 0.0;
                    avgJJAtemp[ecoregion.Index, swhc_index] = 0.0;

                    avgNEEc[ecoregion.Index, swhc_index] = 0.0;
                    avgSOMtc[ecoregion.Index, swhc_index] = 0.0;
                    avgAGB[ecoregion.Index, swhc_index] = 0.0;

                    avgAGNPPtc[ecoregion.Index, swhc_index] = 0.0;
                    avgBGNPPtc[ecoregion.Index, swhc_index] = 0.0;
                    avgLittertc[ecoregion.Index, swhc_index] = 0.0;
                    avgWoodMortality[ecoregion.Index, swhc_index] = 0.0;

                    avgMineralN[ecoregion.Index, swhc_index] = 0.0;
                    avgGrossMin[ecoregion.Index, swhc_index] = 0.0;
                    avgTotalN[ecoregion.Index, swhc_index] = 0.0;

                    avgCohortLeafC[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortFRootC[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortWoodC[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortCRootC[ecoregion.Index, swhc_index] = 0.0;
                    avgWoodC[ecoregion.Index, swhc_index] = 0.0;
                    avgCRootC[ecoregion.Index, swhc_index] = 0.0;

                    avgSurfStrucC[ecoregion.Index, swhc_index] = 0.0;
                    avgSurfMetaC[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilStrucC[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilMetaC[ecoregion.Index, swhc_index] = 0.0;

                    avgCohortLeafN[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortFRootN[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortWoodN[ecoregion.Index, swhc_index] = 0.0;
                    avgCohortCRootN[ecoregion.Index, swhc_index] = 0.0;
                    avgWoodN[ecoregion.Index, swhc_index] = 0.0;
                    avgCRootN[ecoregion.Index, swhc_index] = 0.0;

                    avgSurfStrucN[ecoregion.Index, swhc_index] = 0.0;
                    avgSurfMetaN[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilStrucN[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilMetaN[ecoregion.Index, swhc_index] = 0.0;

                    avgSurfStrucNetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSurfMetaNetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilStrucNetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSoilMetaNetMin[ecoregion.Index, swhc_index] = 0.0;

                    avgSOM1surfC[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM1soilC[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM2C[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM3C[ecoregion.Index, swhc_index] = 0.0;

                    avgSOM1surfN[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM1soilN[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM2N[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM3N[ecoregion.Index, swhc_index] = 0.0;

                    avgSOM1surfNetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM1soilNetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM2NetMin[ecoregion.Index, swhc_index] = 0.0;
                    avgSOM3NetMin[ecoregion.Index, swhc_index] = 0.0;

                    //avgNDeposition[ecoregion.Index, swhc_index] = 0.0;
                    avgStreamC[ecoregion.Index, swhc_index] = 0.0;
                    avgStreamN[ecoregion.Index, swhc_index] = 0.0;
                    avgFireCEfflux[ecoregion.Index, swhc_index] = 0.0;
                    avgFireNEfflux[ecoregion.Index, swhc_index] = 0.0;
                    avgNuptake[ecoregion.Index, swhc_index] = 0.0;
                    avgNresorbed[ecoregion.Index, swhc_index] = 0.0;
                    avgTotalSoilN[ecoregion.Index, swhc_index] = 0.0;
                    avgNvol[ecoregion.Index, swhc_index] = 0.0;
                    avgfrassC[ecoregion.Index, swhc_index] = 0.0;
                    //swhc_cnt++;
                }
            }


            int[,] Climate_SWHC_Count = new int[PlugIn.ModelCore.Ecoregions.Count, PlugIn.SWHC_List.Count];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                int swhc = (int)((SiteVars.SoilFieldCapacity[site] - SiteVars.SoilWiltingPoint[site]) * SiteVars.SoilDepth[site]);
                int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc);

                Climate_SWHC_Count[ecoregion.Index, swhc_index]++;

                avgNEEc[ecoregion.Index, swhc_index] += SiteVars.AnnualNEE[site];
                avgSOMtc[ecoregion.Index, swhc_index] += GetOrganicCarbon(site);
                avgAGB[ecoregion.Index, swhc_index] += Century.ComputeLivingBiomass(SiteVars.Cohorts[site]);

                avgAGNPPtc[ecoregion.Index, swhc_index] += SiteVars.AGNPPcarbon[site];
                avgBGNPPtc[ecoregion.Index, swhc_index] += SiteVars.BGNPPcarbon[site];
                avgLittertc[ecoregion.Index, swhc_index] += SiteVars.LitterfallC[site];
                avgWoodMortality[ecoregion.Index, swhc_index] += SiteVars.WoodMortality[site] * 0.47;

                avgMineralN[ecoregion.Index, swhc_index] += SiteVars.MineralN[site];
                avgTotalN[ecoregion.Index, swhc_index] += GetTotalNitrogen(site);
                avgGrossMin[ecoregion.Index, swhc_index] += SiteVars.GrossMineralization[site];

                avgCohortLeafC[ecoregion.Index, swhc_index] += SiteVars.CohortLeafC[site];
                avgCohortFRootC[ecoregion.Index, swhc_index] += SiteVars.CohortFRootC[site];
                avgCohortWoodC[ecoregion.Index, swhc_index] += SiteVars.CohortWoodC[site];
                avgCohortCRootC[ecoregion.Index, swhc_index] += SiteVars.CohortCRootC[site];
                avgWoodC[ecoregion.Index, swhc_index] += SiteVars.SurfaceDeadWood[site].Carbon;
                avgCRootC[ecoregion.Index, swhc_index] += SiteVars.SoilDeadWood[site].Carbon;

                avgSurfStrucC[ecoregion.Index, swhc_index] += SiteVars.SurfaceStructural[site].Carbon;
                avgSurfMetaC[ecoregion.Index, swhc_index] += SiteVars.SurfaceMetabolic[site].Carbon;
                avgSoilStrucC[ecoregion.Index, swhc_index] += SiteVars.SoilStructural[site].Carbon;
                avgSoilMetaC[ecoregion.Index, swhc_index] += SiteVars.SoilMetabolic[site].Carbon;

                avgSOM1surfC[ecoregion.Index, swhc_index] += SiteVars.SOM1surface[site].Carbon;
                avgSOM1soilC[ecoregion.Index, swhc_index] += SiteVars.SOM1soil[site].Carbon;
                avgSOM2C[ecoregion.Index, swhc_index] += SiteVars.SOM2[site].Carbon;
                avgSOM3C[ecoregion.Index, swhc_index] += SiteVars.SOM3[site].Carbon;

                avgCohortLeafN[ecoregion.Index, swhc_index] += SiteVars.CohortLeafN[site];
                avgCohortFRootN[ecoregion.Index, swhc_index] += SiteVars.CohortFRootN[site];
                avgCohortWoodN[ecoregion.Index, swhc_index] += SiteVars.CohortWoodN[site];
                avgCohortCRootN[ecoregion.Index, swhc_index] += SiteVars.CohortCRootN[site];
                avgWoodN[ecoregion.Index, swhc_index] += SiteVars.SurfaceDeadWood[site].Nitrogen;
                avgCRootN[ecoregion.Index, swhc_index] += SiteVars.SoilDeadWood[site].Nitrogen;

                avgSurfStrucN[ecoregion.Index, swhc_index] += SiteVars.SurfaceStructural[site].Nitrogen;
                avgSurfMetaN[ecoregion.Index, swhc_index] += SiteVars.SurfaceMetabolic[site].Nitrogen;
                avgSoilStrucN[ecoregion.Index, swhc_index] += SiteVars.SoilStructural[site].Nitrogen;
                avgSoilMetaN[ecoregion.Index, swhc_index] += SiteVars.SoilMetabolic[site].Nitrogen;

                avgSOM1surfN[ecoregion.Index, swhc_index] += SiteVars.SOM1surface[site].Nitrogen;
                avgSOM1soilN[ecoregion.Index, swhc_index] += SiteVars.SOM1soil[site].Nitrogen;
                avgSOM2N[ecoregion.Index, swhc_index] += SiteVars.SOM2[site].Nitrogen;
                avgSOM3N[ecoregion.Index, swhc_index] += SiteVars.SOM3[site].Nitrogen;
                avgTotalSoilN[ecoregion.Index, swhc_index] += GetTotalSoilNitrogen(site);

                avgSurfStrucNetMin[ecoregion.Index, swhc_index] += SiteVars.SurfaceStructural[site].NetMineralization;
                avgSurfMetaNetMin[ecoregion.Index, swhc_index] += SiteVars.SurfaceMetabolic[site].NetMineralization;
                avgSoilStrucNetMin[ecoregion.Index, swhc_index] += SiteVars.SoilStructural[site].NetMineralization;
                avgSoilMetaNetMin[ecoregion.Index, swhc_index] += SiteVars.SoilMetabolic[site].NetMineralization;

                avgSOM1surfNetMin[ecoregion.Index, swhc_index] += SiteVars.SOM1surface[site].NetMineralization;
                avgSOM1soilNetMin[ecoregion.Index, swhc_index] += SiteVars.SOM1soil[site].NetMineralization;
                avgSOM2NetMin[ecoregion.Index, swhc_index] += SiteVars.SOM2[site].NetMineralization;
                avgSOM3NetMin[ecoregion.Index, swhc_index] += SiteVars.SOM3[site].NetMineralization;

                //avgNDeposition[ecoregion.Index, swhc_index] = ClimateRegionData.AnnualNDeposition[ecoregion];
                avgStreamC[ecoregion.Index, swhc_index] += SiteVars.Stream[site].Carbon;
                avgStreamN[ecoregion.Index, swhc_index] += SiteVars.Stream[site].Nitrogen; //+ SiteVars.NLoss[site];
                avgFireCEfflux[ecoregion.Index, swhc_index] += SiteVars.FireCEfflux[site];
                avgFireNEfflux[ecoregion.Index, swhc_index] += SiteVars.FireNEfflux[site];
                avgNresorbed[ecoregion.Index, swhc_index] += SiteVars.ResorbedN[site];
                avgNuptake[ecoregion.Index, swhc_index] += GetSoilNuptake(site);
                avgNvol[ecoregion.Index, swhc_index] += SiteVars.Nvol[site];
                avgfrassC[ecoregion.Index, swhc_index] += SiteVars.FrassC[site];

            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (!ecoregion.Active || ClimateRegionData.ActiveSiteCount[ecoregion] < 1)
                    continue;
                primaryLog.Clear();
                PrimaryLog pl = new PrimaryLog();

                pl.Time =    CurrentTime;
                pl.ClimateRegionName =    ecoregion.Name;
                pl.ClimateRegionIndex = ecoregion.Index;

                foreach (int swhc in PlugIn.SWHC_List)
                {
                    int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc);

                    pl.SoilWaterHoldingCapacity = swhc;
                    pl.NumSites = Climate_SWHC_Count[ecoregion.Index, swhc_index]; // ClimateRegionData.ActiveSiteCount[ecoregion];

                    pl.NEEC = (avgNEEc[ecoregion.Index, swhc_index] / (double) Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SOMTC = (avgSOMtc[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.AGB = (avgAGB[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.AG_NPPC = (avgAGNPPtc[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.BG_NPPC = (avgBGNPPtc[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.Litterfall = (avgLittertc[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.AgeMortality = (avgWoodMortality[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.MineralN = (avgMineralN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.TotalN = (avgTotalN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.GrossMineralization = (avgGrossMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.TotalNdep = (ClimateRegionData.AnnualNDeposition[ecoregion] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_Leaf = (avgCohortLeafC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_FRoot = (avgCohortFRootC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_Wood = (avgCohortWoodC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_CRoot = (avgCohortCRootC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadWood = (avgWoodC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadCRoot = (avgCRootC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadLeaf_Struc = (avgSurfStrucC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadLeaf_Meta = (avgSurfMetaC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadFRoot_Struc = (avgSoilStrucC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_DeadFRoot_Meta = (avgSoilMetaC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_SOM1surf = (avgSOM1surfC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_SOM1soil = (avgSOM1soilC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_SOM2 = (avgSOM2C[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.C_SOM3 = (avgSOM3C[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_Leaf = (avgCohortLeafN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_FRoot = (avgCohortFRootN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_Wood = (avgCohortWoodN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_CRoot = (avgCohortCRootN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadWood = (avgWoodN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadCRoot = (avgCRootN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadLeaf_Struc = (avgSurfStrucN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadLeaf_Meta = (avgSurfMetaN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadFRoot_Struc = (avgSoilStrucN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_DeadFRoot_Meta = (avgSoilMetaN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_SOM1surf = (avgSOM1surfN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_SOM1soil = (avgSOM1soilN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_SOM2 = (avgSOM2N[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.N_SOM3 = (avgSOM3N[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SurfStrucNetMin = (avgSurfStrucNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SurfMetaNetMin = (avgSurfMetaNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SoilStrucNetMin = (avgSoilStrucNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SoilMetaNetMin = (avgSoilMetaNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SOM1surfNetMin = (avgSOM1surfNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SOM1soilNetMin = (avgSOM1soilNetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SOM2NetMin = (avgSOM2NetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.SOM3NetMin = (avgSOM3NetMin[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.StreamC = (avgStreamC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.StreamN = (avgStreamN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.FireCEfflux = (avgFireCEfflux[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.FireNEfflux = (avgFireNEfflux[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.Nuptake = (avgNuptake[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.Nresorbed = (avgNresorbed[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.TotalSoilN = (avgTotalSoilN[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.Nvol = (avgNvol[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    pl.FrassC = (avgfrassC[ecoregion.Index, swhc_index] / (double)Climate_SWHC_Count[ecoregion.Index, swhc_index]);
                    
                    primaryLog.AddObject(pl);
                    primaryLog.WriteToFile();
                }
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

            double[] avgNPPtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgResp = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgNEE = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] Ndep = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] StreamN = new double[PlugIn.ModelCore.Ecoregions.Count];
             
            
            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                ppt[ecoregion.Index] = 0.0;
                airtemp[ecoregion.Index] = 0.0;
                avgNPPtc[ecoregion.Index] = 0.0;
                avgResp[ecoregion.Index] = 0.0;
                avgNEE[ecoregion.Index] = 0.0;
                Ndep[ecoregion.Index] = 0.0;
                StreamN[ecoregion.Index] = 0.0;
            }

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

                ppt[ecoregion.Index] = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[month];
                airtemp[ecoregion.Index] = ClimateRegionData.AnnualWeather[ecoregion].MonthlyTemp[month];

                avgNPPtc[ecoregion.Index] += SiteVars.MonthlyAGNPPcarbon[site][month] + SiteVars.MonthlyBGNPPcarbon[site][month];
                avgResp[ecoregion.Index] += SiteVars.MonthlyResp[site][month];
                avgNEE[ecoregion.Index] += SiteVars.MonthlyNEE[site][month];

                SiteVars.AnnualNEE[site] += SiteVars.MonthlyNEE[site][month];

                Ndep[ecoregion.Index] = ClimateRegionData.MonthlyNDeposition[ecoregion][month];
                StreamN[ecoregion.Index] += SiteVars.MonthlyStreamN[site][month];

            }
            

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                if (ClimateRegionData.ActiveSiteCount[ecoregion] > 0) 
                {
                    monthlyLog.Clear();
                    MonthlyLog ml = new MonthlyLog();

                    ml.Time = PlugIn.ModelCore.CurrentTime;
                    ml.Month = month + 1;
                    ml.EcoregionName = ecoregion.Name;
                    ml.EcoregionIndex = ecoregion.Index;

                    ml.NumSites = Convert.ToInt32(ClimateRegionData.ActiveSiteCount[ecoregion]);

                    ml.ppt = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[month];
                    ml.airtemp = ClimateRegionData.AnnualWeather[ecoregion].MonthlyTemp[month];
                    ml.avgNPPtc = (avgNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.avgResp = (avgResp[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.avgNEE = (avgNEE[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    ml.Ndep = Ndep[ecoregion.Index];
                    ml.StreamN = (StreamN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);

                    monthlyLog.AddObject(ml);
                    monthlyLog.WriteToFile();
                }
            }

        }
        
        //Write log file for growth and limits
        public static void CreateCalibrateLogFile()
        {
            string logFileName = "NECN-H-calibrate-log.csv";
            PlugIn.ModelCore.UI.WriteLine("******************WARNING************************", logFileName);
            PlugIn.ModelCore.UI.WriteLine("******YOU ARE CURRENTLY IN CALIBRATE MODE********", logFileName);
            PlugIn.ModelCore.UI.WriteLine("*************************************************", logFileName);
            PlugIn.ModelCore.UI.WriteLine("   Opening NECN-H calibrate log file \"{0}\" ...", logFileName);
            try
            {
                CalibrateLog = new StreamWriter(logFileName);
            }
            catch (Exception err)
            {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }

            CalibrateLog.AutoFlush = true;

            CalibrateLog.Write("Year, Month, EcoregionIndex, SpeciesName, CohortAge, CohortWoodB, CohortLeafB, ");  // from ComputeChange
            CalibrateLog.Write("MortalityAGEwood, MortalityAGEleaf, ");  // from ComputeAgeMortality
            CalibrateLog.Write("MortalityBIOwood, MortalityBIOleaf, ");  // from ComputeGrowthMortality
            CalibrateLog.Write("availableWater,");  //from Water_limit
            CalibrateLog.Write("LAI,tlai,rlai,");  // from ComputeChange
            CalibrateLog.Write("mineralNalloc, resorbedNalloc, ");  // from calculateN_Limit
            //CalibrateLog.Write("limitLAI, limitH20, limitT, limitCapacity, limitN, ");  //from ComputeActualANPP
            CalibrateLog.Write("limitLAI, limitH20, limitT, limitN, ");  //from ComputeActualANPP
            CalibrateLog.Write("maxNPP, Bmax, Bsite, Bcohort, soilTemp, ");  //from ComputeActualANPP
            CalibrateLog.Write("actualWoodNPP, actualLeafNPP, ");  //from ComputeActualANPP
            CalibrateLog.Write("NPPwood, NPPleaf, ");  //from ComputeNPPcarbon
            CalibrateLog.Write("resorbedNused, mineralNused, Ndemand,");  // from AdjustAvailableN
            CalibrateLog.WriteLine("deltaWood, deltaLeaf, totalMortalityWood, totalMortalityLeaf, ");  // from ComputeChange
                        
            

        }
        
        public static void WriteMaps()
        {

            //if (PlugIn.SoilCarbonMapNames != null)// && (PlugIn.ModelCore.CurrentTime % SoilCarbonMapFrequency) == 0)
            //{
                string pathH2O = MapNames.ReplaceTemplateVars("Annual-water-budget-{timestep}.img", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathH2O, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            //This is incorrect right now. Should be ppt-AET, not SOMTC calc
                            //pixel.MapCode.Value = (int)((SiteVars.SOM1surface[site].Carbon + SiteVars.SOM1soil[site].Carbon + SiteVars.SOM2[site].Carbon + SiteVars.SOM3[site].Carbon));
                            pixel.MapCode.Value = (int)((SiteVars.AnnualPPT_AET[site]));
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
            
            if (PlugIn.SoilCarbonMapNames != null)// && (PlugIn.ModelCore.CurrentTime % SoilCarbonMapFrequency) == 0)
                {
                    string path = MapNames.ReplaceTemplateVars(PlugIn.SoilCarbonMapNames, PlugIn.ModelCore.CurrentTime);
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
                }

                if (PlugIn.SoilNitrogenMapNames != null)// && (PlugIn.ModelCore.CurrentTime % SoilNitrogenMapFrequency) == 0)
                {
                    string path2 = MapNames.ReplaceTemplateVars(PlugIn.SoilNitrogenMapNames, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<ShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<ShortPixel>(path2, PlugIn.ModelCore.Landscape.Dimensions))
                    {
                        ShortPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (short)(SiteVars.MineralN[site]);
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

                if (PlugIn.ANPPMapNames != null)// && (PlugIn.ModelCore.CurrentTime % ANPPMapFrequency) == 0)
                {
                    string path3 = MapNames.ReplaceTemplateVars(PlugIn.ANPPMapNames, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<ShortPixel> outputRaster = PlugIn.ModelCore.CreateRaster<ShortPixel>(path3, PlugIn.ModelCore.Landscape.Dimensions))
                    {
                        ShortPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (short)SiteVars.AGNPPcarbon[site];
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
                if (PlugIn.ANEEMapNames != null)// && (PlugIn.ModelCore.CurrentTime % ANEEMapFrequency) == 0)
                {

                    string path4 = MapNames.ReplaceTemplateVars(PlugIn.ANEEMapNames, PlugIn.ModelCore.CurrentTime);
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
                }
                if (PlugIn.TotalCMapNames != null)// && (PlugIn.ModelCore.CurrentTime % TotalCMapFrequency) == 0)
                {

                    string path5 = MapNames.ReplaceTemplateVars(PlugIn.TotalCMapNames, PlugIn.ModelCore.CurrentTime);
                    using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path5, PlugIn.ModelCore.Landscape.Dimensions))
                    {
                        IntPixel pixel = outputRaster.BufferPixel;
                        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                        {
                            if (site.IsActive)
                            {
                                pixel.MapCode.Value = (int)(Outputs.GetOrganicCarbon(site) +
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
                }

                //if (PlugIn.LAIMapNames != null)// && (PlugIn.ModelCore.CurrentTime % LAIMapFrequency) == 0)
                //{

                //    string path5 = MapNames.ReplaceTemplateVars(PlugIn.LAIMapNames, PlugIn.ModelCore.CurrentTime);
                //    using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path5, PlugIn.ModelCore.Landscape.Dimensions))
                //    {
                //        IntPixel pixel = outputRaster.BufferPixel;
                //        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                //        {
                //            if (site.IsActive)
                //            {
                //                pixel.MapCode.Value = (short)(SiteVars.LAI[site]);
                //            }
                //            else
                //            {
                //                //  Inactive site
                //                pixel.MapCode.Value = 0;
                //            }
                //            outputRaster.WriteBufferPixel();
                //        }
                //    }
                //}


                //if (PlugIn.ShadeClassMapNames != null)// && (PlugIn.ModelCore.CurrentTime % LAIMapFrequency) == 0)
                //{

                //    string path5 = MapNames.ReplaceTemplateVars(PlugIn.ShadeClassMapNames, PlugIn.ModelCore.CurrentTime);
                //    using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(path5, PlugIn.ModelCore.Landscape.Dimensions))
                //    {
                //        IntPixel pixel = outputRaster.BufferPixel;
                //        foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                //        {
                //            if (site.IsActive)
                //            {
                //                pixel.MapCode.Value = (short)(SiteVars.ShadeClass[site]); //Shade Class SiteCar doesn't exist. Just a placeholder
                //            }
                //            else
                //            {
                //                //  Inactive site
                //                pixel.MapCode.Value = 0;
                //            }
                //            outputRaster.WriteBufferPixel();
                //        }
                //    }
                //}

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

                   //+ SiteVars.SurfaceDeadWood[site].Nitrogen
                   //+ SiteVars.SoilDeadWood[site].Nitrogen

                    + SiteVars.SurfaceStructural[site].Nitrogen
                    + SiteVars.SoilStructural[site].Nitrogen
                    + SiteVars.SurfaceMetabolic[site].Nitrogen
            +SiteVars.SoilMetabolic[site].Nitrogen

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
                    
                    //SiteVars.SurfaceStructural[site].Carbon
                    //+ SiteVars.SoilStructural[site].Carbon
                    //+ SiteVars.SurfaceMetabolic[site].Carbon
                    //+ SiteVars.SoilMetabolic[site].Carbon

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
