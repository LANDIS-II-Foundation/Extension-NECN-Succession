//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;  
using System.Collections.Generic;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// The pools of dead biomass for the landscape's sites.
    /// </summary>
    public static class SiteVars
    {

        // Time of last succession simulation:
        private static ISiteVar<int> timeOfLast;

        // Live biomass:        
        private static ISiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts> baseCohortsSiteVar;
        private static ISiteVar<Landis.Library.BiomassCohorts.ISiteCohorts> biomassCohortsSiteVar;

        // Dead biomass:
        private static ISiteVar<Layer> surfaceDeadWood;
        private static ISiteVar<Layer> soilDeadWood;

        private static ISiteVar<Layer> surfaceStructural;
        private static ISiteVar<Layer> surfaceMetabolic;
        private static ISiteVar<Layer> soilStructural;
        private static ISiteVar<Layer> soilMetabolic;

        // Dead wood carbon for each year; Chihiro 2020.1.14
        private static ISiteVar<double[]> originalDeadWoodC;
        private static ISiteVar<double[]> currentDeadWoodC; // Carefully check the consistensiy with surfaceDeadWood

        // Soil layers
        private static ISiteVar<Layer> som1surface;
        private static ISiteVar<Layer> som1soil;
        private static ISiteVar<Layer> som2;
        private static ISiteVar<Layer> som3;
        private static ISiteVar<int> soilDepth;
        private static ISiteVar<double> soilDrain;
        private static ISiteVar<double> soilBaseFlowFraction;
        private static ISiteVar<double> soilStormFlowFraction;
        private static ISiteVar<double> soilFieldCapacity;
        private static ISiteVar<double> soilWiltingPoint;
        private static ISiteVar<double> soilPercentSand;
        private static ISiteVar<double> soilPercentClay;


        // Similar to soil layers with respect to their pools:
        private static ISiteVar<Layer> stream;
        private static ISiteVar<Layer> sourceSink;

        // Other variables:
        private static ISiteVar<double> mineralN;
        private static ISiteVar<double> resorbedN;
        private static ISiteVar<double> waterMovement;
        private static ISiteVar<double> availableWater;
        private static ISiteVar<double> soilWaterContent;
        private static ISiteVar<double> meanSoilWaterContent;
        private static ISiteVar<double> liquidSnowPack;
        private static ISiteVar<double> decayFactor;
        private static ISiteVar<double> soilTemperature;
        private static ISiteVar<double> anaerobicEffect;

        // Annual accumulators for reporting purposes.
        private static ISiteVar<double> grossMineralization;
        private static ISiteVar<double> ag_nppC;
        private static ISiteVar<double> bg_nppC;
        private static ISiteVar<double> litterfallC;
        private static ISiteVar<double> cohortLeafN;
        private static ISiteVar<double> cohortFRootN;
        private static ISiteVar<double> cohortLeafC;
        private static ISiteVar<double> cohortFRootC;
        private static ISiteVar<double> cohortWoodN;
        private static ISiteVar<double> cohortCRootN;
        private static ISiteVar<double> cohortWoodC;
        private static ISiteVar<double> cohortCRootC;
        private static ISiteVar<double[]> monthlyAGNPPC;
        private static ISiteVar<double[]> monthlyBGNPPC;
        private static ISiteVar<double[]> monthlyNEE;
        private static ISiteVar<double[]> monthlyStreamN;
        private static ISiteVar<double> totalNuptake;

        private static ISiteVar<double[]> monthlymineralN;

        public static ISiteVar<double> AnnualNEE;
        public static ISiteVar<double> FireCEfflux;
        public static ISiteVar<double> FireNEfflux;
        public static ISiteVar<double> Nvol;
        public static ISiteVar<double> TotalWoodBiomass;
        public static ISiteVar<int> PrevYearMortality;
        public static ISiteVar<byte> FireSeverity;
        public static ISiteVar<double> WoodMortality;
        public static ISiteVar<string> HarvestPrescriptionName;
        public static ISiteVar<int> HarvestTime;
        public static ISiteVar<double> fineFuels;
        public static ISiteVar<double> SmolderConsumption;
        public static ISiteVar<double> FlamingConsumption;
        public static ISiteVar<double> AnnualClimaticWaterDeficit; //Annual soil moisture calculation, defined as pet - aet
        public static ISiteVar<double> AnnualPotentialEvapotranspiration; //PET
        public static ISiteVar<double> AnnualWaterBalance; //Annual soil moisture calculation, defined as pet - aet
        public static ISiteVar<double[]> MonthlySoilResp;
        public static ISiteVar<double[]> MonthlyLAI;
        public static ISiteVar<double[]> MonthlyLAI_Trees;
        public static ISiteVar<double[]> MonthlyLAI_Grasses; // Chihiro, 2021.03.30: tentative
        public static ISiteVar<double> MonthlyLAI_GrassesLastMonth; // Chihiro, 2021.03.30: tentative
        public static ISiteVar<double[]> MonthlyHeteroResp;
        public static ISiteVar<double[]> MonthlySoilWaterContent;
        public static ISiteVar<double[]> MonthlyMeanSoilWaterContent;//SF added
        public static ISiteVar<double[]> MonthlyAnaerobicEffect;//SF added 2023-4-11
        public static ISiteVar<double[]> MonthlyClimaticWaterDeficit;//SF added 2023-6-27
        public static ISiteVar<double[]> MonthlyActualEvapotranspiration;//SF added 2023-6-27
        public static ISiteVar<int> HarvestDisturbedYear;
        public static ISiteVar<int> FireDisturbedYear;
        public static ISiteVar<double> slope;
        public static ISiteVar<double> aspect;

        //Drought params

        public static ISiteVar<double> droughtMort;
        public static ISiteVar<Dictionary<int, double>> speciesDroughtMortality;
        public static ISiteVar<Dictionary<int, double>> speciesDroughtProbability;

        public static ISiteVar<List<double>> swa10;
        public static ISiteVar<List<double>> temp10;
        public static ISiteVar<List<double>> cwd10;
        public static ISiteVar<Dictionary<int, double>> swaLagged;
        public static ISiteVar<Dictionary<int, double>> tempLagged;
        public static ISiteVar<Dictionary<int, double>> cwdLagged;
        public static ISiteVar<double> normalSWA;
        public static ISiteVar<double> normalCWD;
        public static ISiteVar<double> normalTemp;
            

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<SiteCohorts>();
            //baseCohortsSiteVar = CohortSiteVar<ISiteCohorts>.Wrap(cohorts);

            fineFuels = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            TimeOfLast = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            HarvestDisturbedYear = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            FireDisturbedYear = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            // Dead biomass:
            SurfaceDeadWood = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilDeadWood        = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            
            SurfaceStructural   = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SurfaceMetabolic    = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilStructural      = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilMetabolic       = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            
            // Dead wood carbon pools; Chihiro 2020.01.14
            OriginalDeadWoodC   = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            CurrentDeadWoodC    = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            // ------------------------------------------

            // Soil Layers
            SOM1surface         = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SOM1soil            = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SOM2                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SOM3                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SoilDepth           = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            SoilDrain           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilBaseFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilStormFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilFieldCapacity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilWiltingPoint = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilPercentSand = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilPercentClay = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            
            // Other Layers
            Stream              = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            SourceSink          = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();

            // Other variables
            MonthlyLAI = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyLAI_Trees = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyLAI_Grasses = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();  // Chihiro, 2021.03.30: tentative
            MonthlyLAI_GrassesLastMonth = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // Chihiro, 2021.03.30: tentative
            MineralN = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            ResorbedN           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WaterMovement       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            PlantAvailableWater      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LiquidSnowPack      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilWaterContent    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MeanSoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            DecayFactor         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SoilTemperature     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnaerobicEffect     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            DryDays             = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            
            // Annual accumulators
            GrossMineralization = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AGNPPcarbon             = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            BGNPPcarbon             = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LitterfallC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            MonthlyAGNPPcarbon       = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyBGNPPcarbon       = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyNEE          = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyStreamN      = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyHeteroResp         = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlySoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyMeanSoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyAnaerobicEffect = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyClimaticWaterDeficit = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyActualEvapotranspiration = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            AnnualNEE           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireCEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireNEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            CohortLeafN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortFRootN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortLeafC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortFRootC     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortWoodN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortCRootN   = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortWoodC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            CohortCRootC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
                        
            TotalWoodBiomass    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WoodMortality        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Nvol                = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            PrevYearMortality   = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            TotalNuptake        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            monthlymineralN     = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            FrassC              = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            LAI                 = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualWaterBalance       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualClimaticWaterDeficit  = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualPotentialEvapotranspiration = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SmolderConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FlamingConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>(); 
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            HarvestTime = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            MonthlySoilResp = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();

            droughtMort = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputClimateWaterDeficit | DroughtMortality.OutputTemperature)
            {
                swa10 = PlugIn.ModelCore.Landscape.NewSiteVar<List<double>>();
                temp10 = PlugIn.ModelCore.Landscape.NewSiteVar<List<double>>();
                cwd10 = PlugIn.ModelCore.Landscape.NewSiteVar<List<double>>();

                normalSWA = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
                normalCWD = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
                normalTemp = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

                swaLagged = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double>>();
                tempLagged = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double>>();
                cwdLagged = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double>>();

                speciesDroughtMortality = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double>>();
                speciesDroughtProbability = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double>>();
            }

            slope = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            aspect = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            PlugIn.ModelCore.RegisterSiteVar(cohorts, "Succession.UniversalCohorts");
            PlugIn.ModelCore.RegisterSiteVar(fineFuels, "Succession.FineFuels");
            PlugIn.ModelCore.RegisterSiteVar(SmolderConsumption, "Succession.SmolderConsumption");
            PlugIn.ModelCore.RegisterSiteVar(FlamingConsumption, "Succession.FlamingConsumption");
            PlugIn.ModelCore.RegisterSiteVar(AnnualClimaticWaterDeficit, "Succession.CWD");
            PlugIn.ModelCore.RegisterSiteVar(AnnualPotentialEvapotranspiration, "Succession.PET");


            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {

                SurfaceDeadWood[site]       = new Layer(LayerName.Wood, LayerType.Surface);
                SoilDeadWood[site]          = new Layer(LayerName.CoarseRoot, LayerType.Soil);
                
                // Dead wood carbons; Chihiro 2020.01.14
                OriginalDeadWoodC[site]     = new double[PlugIn.ModelCore.EndTime];
                CurrentDeadWoodC[site]      = new double[PlugIn.ModelCore.EndTime];
                // -------------------------------------

                SurfaceStructural[site]     = new Layer(LayerName.Structural, LayerType.Surface);
                SurfaceMetabolic[site]      = new Layer(LayerName.Metabolic, LayerType.Surface);
                SoilStructural[site]        = new Layer(LayerName.Structural, LayerType.Soil);
                SoilMetabolic[site]         = new Layer(LayerName.Metabolic, LayerType.Soil);
                SOM1surface[site]           = new Layer(LayerName.SOM1, LayerType.Surface);
                SOM1soil[site]              = new Layer(LayerName.SOM1, LayerType.Soil);
                SOM2[site]                  = new Layer(LayerName.SOM2, LayerType.Soil);
                SOM3[site]                  = new Layer(LayerName.SOM3, LayerType.Soil);
                
                Stream[site]                = new Layer(LayerName.Other, LayerType.Other);
                SourceSink[site]            = new Layer(LayerName.Other, LayerType.Other);
                
                MonthlyAGNPPcarbon[site]           = new double[12];
                MonthlyBGNPPcarbon[site]           = new double[12];
                MonthlyNEE[site]            = new double[12];
                MonthlyStreamN[site]         = new double[12];
                MonthlyHeteroResp[site]           = new double[12];
                MonthlySoilResp[site] = new double[12];
                MonthlyLAI[site] = new double[12];
                MonthlyLAI_Trees[site] = new double[12];
                MonthlyLAI_Grasses[site] = new double[12];
                MonthlySoilWaterContent[site]       = new double[12];
                MonthlyMeanSoilWaterContent[site] = new double[12];
                MonthlyAnaerobicEffect[site] = new double[12];
                MonthlyClimaticWaterDeficit[site] = new double[12];
                MonthlyActualEvapotranspiration[site] = new double[12];


                CohortResorbedNallocation[site] = new Dictionary<int, Dictionary<int, double>>();

                if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputClimateWaterDeficit | DroughtMortality.OutputTemperature)
                {
                    //Drought parameters
                    swa10[site] = new List<double>(10);
                    temp10[site] = new List<double>(10);
                    cwd10[site] = new List<double>(10);
                    speciesDroughtMortality[site] = new Dictionary<int, double>();
                    speciesDroughtProbability[site] = new Dictionary<int, double>();
                    swaLagged[site] = new Dictionary<int, double>();
                    tempLagged[site] = new Dictionary<int, double>();
                    cwdLagged[site] = new Dictionary<int, double>();
                }

            }
            
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes for disturbances.
        /// </summary>
        public static void ResetDisturbances()
        {
            FireSeverity        = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Cohorts at each site.
        /// </summary>
        private static ISiteVar<SiteCohorts> cohorts;
        public static ISiteVar<SiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
            set
            {
                cohorts = value;
    }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the actual biomass at a site.  The biomass is the total
        /// of all the site's cohorts except young ones.  The total is limited
        /// to being no more than the site's maximum biomass less the previous
        /// year's mortality at the site.
        /// </summary>
        public static double ActualSiteBiomass(ActiveSite site)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
            ISiteCohorts siteCohorts = Cohorts[site];

            if(siteCohorts == null)
                return 0.0;
            
            int youngBiomass;
            int totalBiomass = Library.UniversalCohorts.Cohorts.ComputeBiomass(siteCohorts, out youngBiomass);
            double B_ACT = totalBiomass - youngBiomass;

            return B_ACT;
        }
        
        //---------------------------------------------------------------------
        public static void ResetAnnualValues(Site site)
        {

            // Reset these accumulators to zero:
<<<<<<< HEAD


            DryDays[site] = 0;

            CohortLeafN[site] = 0.0;
            CohortFRootN[site] = 0.0;
            CohortLeafC[site] = 0.0;
            CohortFRootC[site] = 0.0;
            CohortWoodN[site] = 0.0;
            CohortCRootN[site] = 0.0;
            CohortWoodC[site] = 0.0;
            CohortCRootC[site] = 0.0;
            GrossMineralization[site] = 0.0;
            AGNPPcarbon[site] = 0.0;
            BGNPPcarbon[site] = 0.0;
            LitterfallC[site] = 0.0;

            Stream[site]          = new Layer(LayerName.Other, LayerType.Other);
            SourceSink[site]      = new Layer(LayerName.Other, LayerType.Other);

            SurfaceDeadWood[site].NetMineralization = 0.0;
            SurfaceStructural[site].NetMineralization = 0.0;
            SurfaceMetabolic[site].NetMineralization = 0.0;

            SoilDeadWood[site].NetMineralization = 0.0;
            SoilStructural[site].NetMineralization = 0.0;
            SoilMetabolic[site].NetMineralization = 0.0;

            SOM1surface[site].NetMineralization = 0.0;
            SOM1soil[site].NetMineralization = 0.0;
            SOM2[site].NetMineralization = 0.0;
            SOM3[site].NetMineralization = 0.0;
            AnnualNEE[site] = 0.0;
            Nvol[site] = 0.0;
            AnnualNEE[site] = 0.0;
            TotalNuptake[site] = 0.0;
            ResorbedN[site] = 0.0;
            FrassC[site] = 0.0;
            LAI[site] = 0.0;
            AnnualWaterBalance[site] = 0.0;
            AnnualClimaticWaterDeficit[site] = 0.0;
            AnnualPotentialEvapotranspiration[site] = 0.0;
            WoodMortality[site] = 0.0;

            DroughtMort[site] = 0.0;
                        
            if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputClimateWaterDeficit | DroughtMortality.OutputTemperature)
            {
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    //Annual variables for each site/species
                    SpeciesDroughtMortality[site][species.Index] = 0.0;
                    SpeciesDroughtProbability[site][species.Index] = 0.0;
                    TempLagged[site][species.Index] = 0.0;
                    CWDLagged[site][species.Index] = 0.0;
                    SWALagged[site][species.Index] = 0.0;
                }

                if (SoilWater10[site].Count >= 10) 
                    //TODO SF check each list separately, make sure they all have the same number of elements
                    //TODO Also, the first time values are reset, make sure lists are <= 10 elements long
                {
                    SoilWater10[site].RemoveAt(0);
                    Temp10[site].RemoveAt(0);
                    CWD10[site].RemoveAt(0);
                }
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLast { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceDeadWood { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// The DEAD coarse root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilDeadWood { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceStructural { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceMetabolic { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilStructural { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilMetabolic { get; private set; }

        /// <summary>
        /// The original and current dead wood carbon for the landscape's sites.
        /// </summary>
        // Chihiro 2020.01.14
        public static ISiteVar<double[]> OriginalDeadWoodC { get; private set; }
        public static ISiteVar<double[]> CurrentDeadWoodC { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM1-Surface) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM1surface { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM1-Soil) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM1soil { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM2) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM2 { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM3) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM3 { get; private set; }
        public static ISiteVar<int> SoilDepth { get; private set; }
        public static ISiteVar<double> SoilDrain { get; private set; }
        public static ISiteVar<double> SoilBaseFlowFraction { get; private set; }
        public static ISiteVar<double> SoilStormFlowFraction { get; private set; }
        public static ISiteVar<double> SoilFieldCapacity { get; private set; }
        public static ISiteVar<double> SoilWiltingPoint { get; private set; }
        public static ISiteVar<double> SoilPercentSand { get; private set; }
        public static ISiteVar<double> SoilPercentClay { get; private set; }
        public static ISiteVar<Layer> Stream { get; private set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Fine Fuels biomass
        /// </summary>
        public static ISiteVar<double> FineFuels
        {
            get
            {
                return fineFuels;
            }
            set
            {
                fineFuels = value;
            }
        }
        public static ISiteVar<double> WaterMovement { get; set; }
        public static ISiteVar<double> PlantAvailableWater { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// End-of-month soil water content (cm)
        /// </summary>
        public static ISiteVar<double> SoilWaterContent { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Mean volumetric water content (proportion)
        /// </summary>
        public static ISiteVar<double> MeanSoilWaterContent { get; set; }


        /// <summary>
        /// Liquid Snowpack
        /// </summary>
        public static ISiteVar<double> LiquidSnowPack { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Available mineral Nitrogen
        /// </summary>
        public static ISiteVar<double> MineralN { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// The amount of N resorbed before leaf fall
        /// </summary>
        public static ISiteVar<double> ResorbedN { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> DecayFactor { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil temperature (C)
        /// </summary>
        public static ISiteVar<double> SoilTemperature { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> AnaerobicEffect { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil moisture at the time of reproduction
        /// </summary>
        public static ISiteVar<int> DryDays { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Leaf Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortLeafN { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Fine Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortFRootN { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Carbon in the Leaves
        /// </summary>
        public static ISiteVar<double> CohortLeafC { get; set; }

        /// <summary>
        /// A summary of all Carbon in the Fine Roots
        /// </summary>
        public static ISiteVar<double> CohortFRootC { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Aboveground Wood Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodN { get; set; }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Coarse Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortCRootN { get; set; }



        /// <summary>
        /// A summary of all Aboveground Wood Carbon in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodC { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of all Carbon in the Coarse Roots
        /// </summary>
        public static ISiteVar<double> CohortCRootC { get; set; }

        //-------------------------

        /// <summary>
        /// A summary of Gross Mineraliztion.
        /// </summary>
        public static ISiteVar<double> GrossMineralization { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> AGNPPcarbon { get; private set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> BGNPPcarbon { get; private set; }




        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Litter fall (g C/m2).
        /// </summary>
        public static ISiteVar<double> LitterfallC { get; private set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyAGNPPcarbon { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyBGNPPcarbon { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Net Ecosystem Exchange (g C/m2), from a flux tower's perspective,
        /// whereby positive values indicate terrestrial C loss, negative values indicate C gain.
        /// Replace SourceSink?
        /// </summary>
        public static ISiteVar<double[]> MonthlyNEE { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of N leaching
        /// </summary>
        public static ISiteVar<double[]> MonthlyStreamN { get; set; }

        /// <summary>
        /// Water loss
        /// </summary>
        public static ISiteVar<Layer> SourceSink { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of N uptake (g N/m2)
        /// </summary>
        public static ISiteVar<double> TotalNuptake { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of frass deposition (g C/m2)
        /// </summary>
        public static ISiteVar<double> FrassC { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of LAI (m2/m2)
        /// </summary>
        public static ISiteVar<double> LAI { get; set; }

        // --------------------------------------------------------------------
        /// <summary>
        /// Keep track of minimum soil water values
        /// //TODO sam
        /// </summary>
        public static ISiteVar<double> DroughtMort
        {
            get
            {
                return droughtMort;
            }
            set
            {
                droughtMort = value;
            }


        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Site-level biomass mortality due to drought for each species, to map drought impacts and for log file
        /// //drought_todo
        /// </summary>
        public static ISiteVar<Dictionary<int, double>> SpeciesDroughtMortality
        {
            get
            {
                return speciesDroughtMortality;
            }
            set
            {
                speciesDroughtMortality = value;
            }
        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Site-level probability of mortality due to drought for each species, for output log file
        /// </summary>
        public static ISiteVar<Dictionary<int, double>> SpeciesDroughtProbability
        {
            get
            {
                return speciesDroughtProbability;
            }
            set
            {
                speciesDroughtProbability = value;
            }


        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Keep track of minimum soil water values
        /// //TODO sam //drought_todo
        /// </summary>
        public static ISiteVar<List<double>> SoilWater10
        {
            get
            {
                return swa10;
            }
            set
            {
                swa10 = value;
            }

        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Keep track of maximum temperatures
        /// //TODO sam
        /// </summary>
        public static ISiteVar<List<double>> Temp10 //list of doubles
        {
            get
            {
                return temp10;
            }
            set
            {
                temp10 = value;
            }


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Keep track of cwd
        /// //TODO sam
        /// </summary>
        public static ISiteVar<List<double>> CWD10 //list of doubles
        {
            get
            {
                return cwd10;
            }
            set
            {
                cwd10 = value;
            }


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// SWA calculated for each site with appropriate time-lag
        /// //TODO sam
        /// </summary>
        public static ISiteVar<Dictionary<int, double>> SWALagged //one value per species per site
        {
            get
            {
                return swaLagged;
            }
            set
            {
                swaLagged = value;
            }


        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Temperature calculated for each site with appropriate time-lag
        /// //TODO sam
        /// </summary>
        public static ISiteVar<Dictionary<int, double>> TempLagged //list of doubles
        {
            get
            {
                return tempLagged;
            }
            set
            {
                tempLagged = value;
            }


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// CWD calculated for each site with appropriate time-lag
        /// //TODO sam
        /// </summary>
        public static ISiteVar<Dictionary<int, double>> CWDLagged //list of doubles
        {
            get
            {
                return cwdLagged;
            }
            set
            {
                cwdLagged = value;
            }


        }


        // --------------------------------------------------------------------
        /// <summary>
        /// Input value of Normal SWA 
        /// //TODO sam //drought_todo
        /// </summary>
        public static ISiteVar<double> NormalSWA
        {
            get
            {
                return normalSWA;
            }
            set
            {
                normalSWA = value;
            }

        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Input value of Normal CWD
        /// //TODO sam //drought_todo
        /// </summary>
        public static ISiteVar<double> NormalCWD
        {
            get
            {
                return normalCWD;
            }
            set
            {
                normalCWD = value;
            }

        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Input value of Normal CWD
        /// //TODO sam //drought_todo
        /// </summary>
        public static ISiteVar<double> NormalTemp
        {
            get
            {
                return normalTemp;
            }
            set
            {
                normalTemp = value;
            }

        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Input value of Slope
        /// //TODO sam
        /// </summary>
        public static ISiteVar<double> Slope
        { //drought_todo
            get
            {
                return slope;
            }
            set
            {
                slope = value;
            }

        }

        // --------------------------------------------------------------------
        /// <summary>
        /// Input value of Aspect
        /// //TODO sam
        /// </summary>
        public static ISiteVar<double> Aspect
        { //drought_todo
            get
            {
                return aspect;
            }
            set
            {
                aspect = value;
            }

        }

    }

}
 
