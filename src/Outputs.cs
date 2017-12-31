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



namespace Landis.Extension.Succession.NECN_Hydro
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
                avgAGB += (double) Century.ComputeLivingBiomass(SiteVars.Cohorts[site]) / PlugIn.ModelCore.Landscape.ActiveSiteCount; 
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

            //PlugIn.SWHC_List.Sort();
            //int soil_count = PlugIn.SWHC_List.Count;
            //PlugIn.ModelCore.UI.WriteLine("soil count = {0}", soil_count);

            double[] avgAnnualPPT = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgJJAtemp = new double[PlugIn.ModelCore.Ecoregions.Count];

            double[] avgNEEc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgSOMtc = new double[PlugIn.ModelCore.Ecoregions.Count];
            double[] avgAGB = new double[PlugIn.ModelCore.Ecoregions.Count];

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

            //int swhc_cnt = 0;

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                //foreach (int swhc in PlugIn.SWHC_List)
                //{
                    //int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc); 
                    //PlugIn.ModelCore.UI.WriteLine("SHWC Index = {0}", swhc_index);
                    //PlugIn.ModelCore.UI.WriteLine("SHWC = {0}", swhc);

                    // TO DO: ADD SWHC LOOP  
                    avgAnnualPPT[ecoregion.Index] = 0.0;
                    avgJJAtemp[ecoregion.Index] = 0.0;

                    avgNEEc[ecoregion.Index] = 0.0;
                    avgSOMtc[ecoregion.Index] = 0.0;
                    avgAGB[ecoregion.Index] = 0.0;

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
                    //swhc_cnt++;
                //}
            }


            //int[,] Climate_SWHC_Count = new int[PlugIn.ModelCore.Ecoregions.Count, PlugIn.SWHC_List.Count];

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                //int swhc = (int)((SiteVars.SoilFieldCapacity[site] - SiteVars.SoilWiltingPoint[site]) * SiteVars.SoilDepth[site]);
                //int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc);

                //Climate_SWHC_Count[ecoregion.Index]++;

                avgNEEc[ecoregion.Index] += SiteVars.AnnualNEE[site];
                avgSOMtc[ecoregion.Index] += GetOrganicCarbon(site);
                avgAGB[ecoregion.Index] += Century.ComputeLivingBiomass(SiteVars.Cohorts[site]);

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
                
 

                //foreach (int swhc in PlugIn.SWHC_List)
                //{
                //    int swhc_index = PlugIn.SWHC_List.BinarySearch(swhc);


                    //pl.SoilWaterHoldingCapacity = swhc;
                pl.NumSites = ClimateRegionData.ActiveSiteCount[ecoregion]; //Climate_SWHC_Count[ecoregion.Index]; // ClimateRegionData.ActiveSiteCount[ecoregion];

                    pl.NEEC = (avgNEEc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.SOMTC = (avgSOMtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.AGB = (avgAGB[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.AG_NPPC = (avgAGNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.BG_NPPC = (avgBGNPPtc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.Litterfall = (avgLittertc[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.AgeMortality = (avgWoodMortality[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.MineralN = (avgMineralN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.TotalN = (avgTotalN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.GrossMineralization = (avgGrossMin[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.TotalNdep = (ClimateRegionData.AnnualNDeposition[ecoregion] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.C_Leaf = (avgCohortLeafC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.C_FRoot = (avgCohortFRootC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.C_Wood = (avgCohortWoodC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.C_CRoot = (avgCohortCRootC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
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
                    pl.StreamC = (avgStreamC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.StreamN = (avgStreamN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.FireCEfflux = (avgFireCEfflux[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.FireNEfflux = (avgFireNEfflux[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.Nuptake = (avgNuptake[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.Nresorbed = (avgNresorbed[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.TotalSoilN = (avgTotalSoilN[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.Nvol = (avgNvol[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    pl.FrassC = (avgfrassC[ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]);
                    
                    primaryLog.AddObject(pl);
                    primaryLog.WriteToFile();
                //}
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
            string logFileName = "NECN_Hydro-calibrate-log.csv";
            PlugIn.ModelCore.UI.WriteLine("******************WARNING************************", logFileName);
            PlugIn.ModelCore.UI.WriteLine("******YOU ARE CURRENTLY IN CALIBRATE MODE********", logFileName);
            PlugIn.ModelCore.UI.WriteLine("*************************************************", logFileName);
            PlugIn.ModelCore.UI.WriteLine("   Opening NECN_Hydro calibrate log file \"{0}\" ...", logFileName);
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
                string pathH2O = MapNames.ReplaceTemplateVars(@"NECN_Hydro\Annual-water-budget-{timestep}.img", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathH2O, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
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
            //AMK: Trying out directly writing maps
                string pathANPP = MapNames.ReplaceTemplateVars(@"NECN_Hydro\AG_NPP-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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
            
            //if (PlugIn.SoilCarbonMapNames != null)
            //    {
                    string path = MapNames.ReplaceTemplateVars(@"NECN_Hydro\SOMTC-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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
                //}

                //if (PlugIn.SoilNitrogenMapNames != null)
                //{
                    string path2 = MapNames.ReplaceTemplateVars(@"NECN_Hydro\SoilN-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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
                //}

                    string path4 = MapNames.ReplaceTemplateVars(@"NECN_Hydro\ANEE-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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
                //}
                //if (PlugIn.TotalCMapNames != null) 
                //{

                    string path5 = MapNames.ReplaceTemplateVars(@"NECN_Hydro\TotalC-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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
                //}

                    string pathLAI = MapNames.ReplaceTemplateVars(@"NECN_Hydro\LAI-{timestep}.img", PlugIn.ModelCore.CurrentTime);
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

                string pathavailablewater = MapNames.ReplaceTemplateVars(@"NECN_Hydro\AvailableWater-{timestep}.img", PlugIn.ModelCore.CurrentTime);
                using (IOutputRaster<IntPixel> outputRaster = PlugIn.ModelCore.CreateRaster<IntPixel>(pathavailablewater, PlugIn.ModelCore.Landscape.Dimensions))
                {
                    IntPixel pixel = outputRaster.BufferPixel;
                    foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                    {
                        if (site.IsActive)
                        {
                            pixel.MapCode.Value = (int)((SiteVars.AvailableWater[site]));
                        }
                        else
                        {
                            //  Inactive site
                            pixel.MapCode.Value = 0;
                        }
                        outputRaster.WriteBufferPixel();
                    }

                }


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
