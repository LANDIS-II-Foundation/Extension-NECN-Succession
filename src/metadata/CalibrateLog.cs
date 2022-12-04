using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class CalibrateLog
    {

        public static int year, month, climateRegionIndex, cohortAge;
        public static double cohortWoodB, cohortLeafB;
        public static string speciesName;
        public static double mortalityAGEwood, mortalityAGEleaf;
        public static double actual_LAI, actual_LAI_tree, base_lai, seasonal_adjustment, siteLAI;
        public static double mineralNalloc, resorbedNalloc;
        public static double limitLAI, limitH20, limitT, limitN, limitLAIcompetition;
        public static double maxNPP, maxB, siteB, cohortB, soilTemp;
        public static double actualWoodNPP, actualLeafNPP;
        public static double deltaWood, deltaLeaf;
        public static double mortalityBIOwood, mortalityBIOleaf;
        public static double resorbedNused, mineralNused, demand_N;

        // KM added variables for tracking water 
        public static double precipitation;
        public static double vpd;
        public static double availableSW;
        public static double availableSWFraction;
        public static double availableWaterTranspiration;  //  (availableWaterMax + priorAvailableWaterMin) / 2.0;
        public static double cimodifier;
        public static double jh2o;
        public static double jco2;
        public static double wuescalar;
        public static double wue; // JCO2 / JH2O
        public static double grosspsn;
        public static double transpiration;
        public static double co2;


        public static void WriteLogFile()
        {
            Outputs.calibrateLog.Clear();
            CalibrateLog clog = new CalibrateLog();

            clog.Year = year;
            clog.Month = month;
            clog.ClimateRegionIndex = climateRegionIndex;
            clog.CohortAge = cohortAge;
            clog.CohortWoodBiomass = cohortWoodB;
            clog.CohortLeafBiomass = cohortLeafB;
            clog.SpeciesName = speciesName;
            clog.MortalityAGEwoodBiomass = mortalityAGEwood;
            clog.MortalityAGEleafBiomass = mortalityAGEleaf;
            clog.MortalityTHINwoodBiomass = mortalityBIOwood;
            clog.MortalityTHINleafBiomass = mortalityBIOleaf;
            clog.ActualLAI = actual_LAI; // Chihiro, 2021.03.26: renamed
            clog.TreeLAI = actual_LAI_tree;
            clog.SiteLAI = siteLAI; // Chihiro, 2021.03.26: added
            clog.BaseLAI = base_lai;
            clog.SeaonalAdjustLAI = seasonal_adjustment;
            clog.MineralNalloc = mineralNalloc;
            clog.ResorbedNalloc = resorbedNalloc;
            clog.MineralNconsumed = mineralNused;
            clog.ResorbedNconsumed = resorbedNused;
            clog.GrowthLimitLAI = limitLAI;
            clog.GrowthLimitN = limitN;
            clog.GrowthLimitSoilWater = limitH20;
            clog.GrowthLimitT = limitT;
            clog.GrowthLimitLAIcompetition = limitLAIcompetition; // Chihiro, 2021.03.26: added
            clog.MaximumANPP = maxNPP;
            clog.CohortMaximumBiomass = maxB;
            clog.TotalSiteBiomass = siteB;
            clog.CohortBiomass = cohortB;
            clog.SoilTemperature = soilTemp;
            clog.ActualWoodANPP = actualWoodNPP;
            clog.ActualLeafANPP = actualLeafNPP;
            clog.DeltaWood = deltaWood;
            clog.DeltaLeaf = deltaLeaf;
            clog.TotalNDemand = demand_N;

            // KM added variables for tracking water 
            clog.Precipitation = precipitation;
            clog.VPD = vpd;
            clog.AvailableSW = availableSW;
            clog.AvailableSWFraction = availableSWFraction;
            clog.AvailableWaterTranspiration = availableWaterTranspiration;
            clog.CiModifier = cimodifier;
            clog.JH2O = jh2o;
            clog.JCO2 = jco2;
            clog.WUEScalar = wuescalar;
            clog.WUE = wue;
            clog.GrossPsn = grosspsn;
            clog.Transpiration = transpiration;
            clog.CO2 = co2;
            
            Outputs.calibrateLog.AddObject(clog);
            Outputs.calibrateLog.WriteToFile();

        }


        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Year {set; get;}

        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.Month, Desc = "Simulation Month")]
        public int Month { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Index", Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit= FieldUnits.Year, Desc = "CohortAge")]
        public int CohortAge { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Wood Biomass", Format = "0.0")]
        public double CohortWoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Leaf Biomass", Format = "0.0")]
        public double CohortLeafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Total Biomass", Format = "0.0")]
        public double CohortBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Age Wood Biomass", Format = "0.000")]
        public double MortalityAGEwoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Age Leaf Biomass", Format = "0.00000")]
        public double MortalityAGEleafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Thinning Wood Biomass", Format = "0.000")]
        public double MortalityTHINwoodBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Mortality Thinning Leaf Biomass", Format = "0.00000")]
        public double MortalityTHINleafBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "m2_m-2", Desc = "Base LAI", Format = "0.0")]
        public double BaseLAI { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Seaonal LAI Adjust", Format = "0.00")]
        public double SeaonalAdjustLAI { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: modified from LAI
        [DataFieldAttribute(Unit = "m2_m-2", Desc = "Actual LAI adjusted by seasonality", Format = "0.0")]
        public double ActualLAI { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: added
        [DataFieldAttribute(Unit = "m2_m-2", Desc = "Site Total LAI above this cohort", Format = "0.0")]
        public double SiteLAI { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: added
        [DataFieldAttribute(Unit = "m2_m-2", Desc = "Tree Leaf Area Index", Format = "0.0")]
        public double TreeLAI { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit LAI", Format = "0.00")]
        public double GrowthLimitLAI { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Soil Water", Format = "0.00")]
        public double GrowthLimitSoilWater { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Temperature", Format = "0.00")]
        public double GrowthLimitT { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit Nitrogen", Format = "0.00")]
        public double GrowthLimitN { set; get; }
        // ********************************************************************
        // Chihiro, 2021.03.26: added
        [DataFieldAttribute(Unit = "Fraction", Desc = "Growth Limit LAI competition", Format = "0.00")]
        public double GrowthLimitLAIcompetition { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Maximum ANPP")]
        public double MaximumANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Actual Wood ANPP", Format = "0.000")]
        public double ActualWoodANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Actual Leaf ANPP", Format = "0.0000")]
        public double ActualLeafANPP { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Change in Wood Biomass", Format = "0.000")]
        public double DeltaWood { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_B_m2_month1", Desc = "Change in Leaf Biomass", Format = "0.0000")]
        public double DeltaLeaf { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Cohort Maximum Biomass")]
        public double CohortMaximumBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Total Site Biomass")]
        public double TotalSiteBiomass { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Soil Temperature", Format = "0.0")]
        public double SoilTemperature { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Mineral N Allocation", Format = "0.000")]
        public double MineralNalloc { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Resorbed N Allocation", Format = "0.000")]
        public double ResorbedNalloc { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Mineral N Consumed", Format = "0.000")]
        public double MineralNconsumed { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Resorbed N Consumed", Format = "0.000")]
        public double ResorbedNconsumed { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "Total N Demand", Format = "0.000")]
        public double TotalNDemand { set; get; }


        // KM added variables for water tracking 
        // ********************************************************************
        [DataFieldAttribute(Unit = "cm", Desc = "precip", Format = "0.00")]
        public double Precipitation { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "kPa", Desc = "VPD", Format = "0.00")]
        public double VPD { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Available Water Transpiration cohort", Format = "0.0000")]
        public double AvailableSW { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Available Water Transpiration Fraction", Format = "0.0000")]
        public double AvailableSWFraction { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "total water available to transpiration", Format = "0.0000")]
        public double AvailableWaterTranspiration { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "unitless", Desc = "CiModifier water limitation", Format = "0.00")]
        public double CiModifier { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "unitless", Desc = "JH2O", Format = "0.000000000000")]
        public double JH2O { set; get; }
                // ********************************************************************
        [DataFieldAttribute(Unit = "unitless", Desc = "JCO2", Format = "0.000000000000")]
        public double JCO2 { set; get; }
        // ********************************************************************
        
        [DataFieldAttribute(Unit = "unitless", Desc = "wue scalar", Format = "0.00")]
        public double WUEScalar { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "unitless", Desc = "wue", Format = "0.00")]
        public double WUE { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "gC m2 mo-1", Desc = "Gross Photosynthesis", Format = "0.00")]
        public double GrossPsn { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "cm", Desc = "Transpiration", Format = "0.0000")]
        public double Transpiration { set; get; }
        // ********************************************************************
        [DataFieldAttribute(Unit = "ppm", Desc = "co2", Format = "0.0000")]
        public double CO2 { set; get; }


    }
}
