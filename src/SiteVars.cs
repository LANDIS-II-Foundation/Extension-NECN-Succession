//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.LeafBiomassCohorts;  
using System.Collections.Generic;
using System;
using System.Data;

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
        private static ISiteVar<double> frassC;
        private static ISiteVar<double> lai;
        //private static ISiteVar<double> annualPPT_AET; //Annual water budget calculation. 
        private static ISiteVar<int> dryDays;

        //TODO why are some upper and some lower case?
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
        public static ISiteVar<Dictionary<int, Dictionary<int, double>>> CohortResorbedNallocation;
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

        //Drought params
        //drought_todo
        public static ISiteVar<double> droughtMort;
        public static ISiteVar<Dictionary<int, double>> speciesDroughtMortality;
        public static ISiteVar<List<double>> swa10;
        public static ISiteVar<List<double>> temp10;
        public static ISiteVar<List<double>> cwd10;
        public static ISiteVar<Dictionary<int, double>> swaLagged;
        public static ISiteVar<Dictionary<int, double>> tempLagged;
        public static ISiteVar<Dictionary<int, double>> cwdLagged;
        public static ISiteVar<double> normalSWA;
        public static ISiteVar<double> normalCWD;
        public static ISiteVar<double> normalTemp;
        public static ISiteVar<double> slope;
        public static ISiteVar<double> aspect;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            cohorts = PlugIn.ModelCore.Landscape.NewSiteVar<Library.LeafBiomassCohorts.SiteCohorts>();
            biomassCohortsSiteVar = Landis.Library.Succession.CohortSiteVar<Landis.Library.BiomassCohorts.ISiteCohorts>.Wrap(cohorts);
            baseCohortsSiteVar = Landis.Library.Succession.CohortSiteVar<Landis.Library.AgeOnlyCohorts.ISiteCohorts>.Wrap(cohorts);
            fineFuels = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            timeOfLast = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            HarvestDisturbedYear = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            FireDisturbedYear = PlugIn.ModelCore.Landscape.NewSiteVar<int>();

            // Dead biomass:
            surfaceDeadWood = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            soilDeadWood        = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            
            surfaceStructural   = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            surfaceMetabolic    = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            soilStructural      = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            soilMetabolic       = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            
            // Dead wood carbon pools; Chihiro 2020.01.14
            originalDeadWoodC   = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            currentDeadWoodC    = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            // ------------------------------------------

            // Soil Layers
            som1surface         = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            som1soil            = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            som2                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            som3                = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            soilDepth           = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            soilDrain           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilBaseFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilStormFlowFraction = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilFieldCapacity = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilWiltingPoint = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilPercentSand = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilPercentClay = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            
            // Other Layers
            stream              = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();
            sourceSink          = PlugIn.ModelCore.Landscape.NewSiteVar<Layer>();

            // Other variables
            MonthlyLAI = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyLAI_Trees = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyLAI_Grasses = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();  // Chihiro, 2021.03.30: tentative
            MonthlyLAI_GrassesLastMonth = PlugIn.ModelCore.Landscape.NewSiteVar<double>();  // Chihiro, 2021.03.30: tentative
            mineralN = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            resorbedN           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            waterMovement       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            availableWater      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            liquidSnowPack      = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilWaterContent    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            meanSoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            decayFactor         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            soilTemperature     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            anaerobicEffect     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            dryDays             = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            
            // Annual accumulators
            grossMineralization = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            ag_nppC             = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            bg_nppC             = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            litterfallC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            monthlyAGNPPC       = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            monthlyBGNPPC       = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            monthlyNEE          = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            monthlyStreamN      = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyHeteroResp         = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlySoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyMeanSoilWaterContent = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyAnaerobicEffect = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyClimaticWaterDeficit = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            MonthlyActualEvapotranspiration = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            AnnualNEE           = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireCEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FireNEfflux         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            cohortLeafN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortFRootN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortLeafC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortFRootC     = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortWoodN         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortCRootN   = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortWoodC         = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            cohortCRootC = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
                        
            TotalWoodBiomass    = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            WoodMortality        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            Nvol                = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            PrevYearMortality   = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            totalNuptake        = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            monthlymineralN     = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();
            frassC              = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            lai                 = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualWaterBalance       = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualClimaticWaterDeficit  = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            AnnualPotentialEvapotranspiration = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            SmolderConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            FlamingConsumption = PlugIn.ModelCore.Landscape.NewSiteVar<double>(); 
            HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
            HarvestTime = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            MonthlySoilResp = PlugIn.ModelCore.Landscape.NewSiteVar<double[]>();

            droughtMort = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
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

            slope = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            aspect = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            CohortResorbedNallocation = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, Dictionary<int, double>>>();

            PlugIn.ModelCore.RegisterSiteVar(cohorts, "Succession.LeafBiomassCohorts");
            PlugIn.ModelCore.RegisterSiteVar(baseCohortsSiteVar, "Succession.AgeCohorts");
            PlugIn.ModelCore.RegisterSiteVar(biomassCohortsSiteVar, "Succession.BiomassCohorts");
            PlugIn.ModelCore.RegisterSiteVar(fineFuels, "Succession.FineFuels");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.SmolderConsumption, "Succession.SmolderConsumption");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.FlamingConsumption, "Succession.FlamingConsumption");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.AnnualClimaticWaterDeficit, "Succession.CWD");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.AnnualPotentialEvapotranspiration, "Succession.PET");


            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                surfaceDeadWood[site]       = new Layer(LayerName.Wood, LayerType.Surface);
                soilDeadWood[site]          = new Layer(LayerName.CoarseRoot, LayerType.Soil);
                
                // Dead wood carbons; Chihiro 2020.01.14
                originalDeadWoodC[site]     = new double[PlugIn.ModelCore.EndTime];
                currentDeadWoodC[site]      = new double[PlugIn.ModelCore.EndTime];
                // -------------------------------------

                surfaceStructural[site]     = new Layer(LayerName.Structural, LayerType.Surface);
                surfaceMetabolic[site]      = new Layer(LayerName.Metabolic, LayerType.Surface);
                soilStructural[site]        = new Layer(LayerName.Structural, LayerType.Soil);
                soilMetabolic[site]         = new Layer(LayerName.Metabolic, LayerType.Soil);
                som1surface[site]           = new Layer(LayerName.SOM1, LayerType.Surface);
                som1soil[site]              = new Layer(LayerName.SOM1, LayerType.Soil);
                som2[site]                  = new Layer(LayerName.SOM2, LayerType.Soil);
                som3[site]                  = new Layer(LayerName.SOM3, LayerType.Soil);
                
                stream[site]                = new Layer(LayerName.Other, LayerType.Other);
                sourceSink[site]            = new Layer(LayerName.Other, LayerType.Other);
                
                monthlyAGNPPC[site]           = new double[12];
                monthlyBGNPPC[site]           = new double[12];
                monthlyNEE[site]            = new double[12];
                monthlyStreamN[site]         = new double[12];
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

                //Drought parameters
                swa10[site] = new List<double>(10);
                temp10[site] = new List<double>(10);
                cwd10[site] = new List<double>(10);
                speciesDroughtMortality[site] =new Dictionary<int, double>();
                swaLagged[site] = new Dictionary<int, double>();
                tempLagged[site] = new Dictionary<int, double>();
                cwdLagged[site] = new Dictionary<int, double>();


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
            //SiteVars.HarvestDisturbedYear.SiteValues = false;
            //SiteVars.FireDisturbedYear.SiteValues = false;
            
            //if(HarvestPrescriptionName == null)
            //    throw new System.ApplicationException("TEST Error: Harvest Prescription Names NOT Initialized.");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Biomass cohorts at each site.
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
            ISiteCohorts siteCohorts = SiteVars.Cohorts[site];

            if(siteCohorts == null)
                return 0.0;
            
            int youngBiomass;
            int totalBiomass = Library.LeafBiomassCohorts.Cohorts.ComputeBiomass(siteCohorts, out youngBiomass);
            double B_ACT = totalBiomass - youngBiomass;

            return B_ACT;
        }
        
        //---------------------------------------------------------------------
        public static void ResetAnnualValues(Site site)
        {

            // Reset these accumulators to zero:
            
            
            SiteVars.DryDays[site] = 0;

            SiteVars.CohortLeafN[site] = 0.0;
            SiteVars.CohortFRootN[site] = 0.0;
            SiteVars.CohortLeafC[site] = 0.0;
            SiteVars.CohortFRootC[site] = 0.0;
            SiteVars.CohortWoodN[site] = 0.0;
            SiteVars.CohortCRootN[site] = 0.0;
            SiteVars.CohortWoodC[site] = 0.0;
            SiteVars.CohortCRootC[site] = 0.0;
            SiteVars.GrossMineralization[site] = 0.0;
            SiteVars.AGNPPcarbon[site] = 0.0;
            SiteVars.BGNPPcarbon[site] = 0.0;
            SiteVars.LitterfallC[site] = 0.0;
            
            SiteVars.Stream[site]          = new Layer(LayerName.Other, LayerType.Other);
            SiteVars.SourceSink[site]      = new Layer(LayerName.Other, LayerType.Other);
            
            SiteVars.SurfaceDeadWood[site].NetMineralization = 0.0;
            SiteVars.SurfaceStructural[site].NetMineralization = 0.0;
            SiteVars.SurfaceMetabolic[site].NetMineralization = 0.0;
            
            SiteVars.SoilDeadWood[site].NetMineralization = 0.0;
            SiteVars.SoilStructural[site].NetMineralization = 0.0;
            SiteVars.SoilMetabolic[site].NetMineralization = 0.0;
            
            SiteVars.SOM1surface[site].NetMineralization = 0.0;
            SiteVars.SOM1soil[site].NetMineralization = 0.0;
            SiteVars.SOM2[site].NetMineralization = 0.0;
            SiteVars.SOM3[site].NetMineralization = 0.0;
            SiteVars.AnnualNEE[site] = 0.0;
            SiteVars.Nvol[site] = 0.0;
            SiteVars.AnnualNEE[site] = 0.0;
            SiteVars.TotalNuptake[site] = 0.0;
            SiteVars.ResorbedN[site] = 0.0;
            SiteVars.FrassC[site] = 0.0;
            SiteVars.LAI[site] = 0.0;
            SiteVars.AnnualWaterBalance[site] = 0.0;
            SiteVars.AnnualClimaticWaterDeficit[site] = 0.0;
            SiteVars.AnnualPotentialEvapotranspiration[site] = 0.0;
            SiteVars.WoodMortality[site] = 0.0;

            SiteVars.DroughtMort[site] = 0.0;
            foreach(ISpecies species in PlugIn.ModelCore.Species)
            {
                SiteVars.SpeciesDroughtMortality[site][species.Index] = 0.0;
                SiteVars.TempLagged[site][species.Index] = 0.0;
                SiteVars.CWDLagged[site][species.Index] = 0.0;
                SiteVars.SWALagged[site][species.Index] = 0.0;
            }
            

            if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputClimateWaterDeficit | DroughtMortality.OutputTemperature)
            {
                if (PlugIn.ModelCore.CurrentTime >= 11)
                {
                    SiteVars.SoilWater10[site].RemoveAt(0);
                    SiteVars.Temp10[site].RemoveAt(0);
                    SiteVars.CWD10[site].RemoveAt(0);
                }
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLast
        {
            get {
                return timeOfLast;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The intact dead woody pools for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceDeadWood
        {
            get {
                return surfaceDeadWood;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The DEAD coarse root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilDeadWood
        {
            get {
                return soilDeadWood;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceStructural
        {
            get {
                return surfaceStructural;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dead surface pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SurfaceMetabolic
        {
            get {
                return surfaceMetabolic;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilStructural
        {
            get {
                return soilStructural;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The fine root pool for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SoilMetabolic
        {
            get {
                return soilMetabolic;
            }
        }
        
        /// <summary>
        /// The original and current dead wood carbon for the landscape's sites.
        /// </summary>
        // Chihiro 2020.01.14
        public static ISiteVar<double[]> OriginalDeadWoodC { get { return originalDeadWoodC; } }
        public static ISiteVar<double[]> CurrentDeadWoodC { get { return currentDeadWoodC; } }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM1-Surface) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM1surface
        {
            get {
                return som1surface;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM1-Soil) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM1soil
        {
            get {
                return som1soil;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM2) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM2
        {
            get {
                return som2;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The soil organic matter (SOM3) for the landscape's sites.
        /// </summary>
        public static ISiteVar<Layer> SOM3
        {
            get {
                return som3;
            }
        }
        public static ISiteVar<int> SoilDepth {get{return soilDepth;}}
        public static ISiteVar<double> SoilDrain { get { return soilDrain; } }
        public static ISiteVar<double> SoilBaseFlowFraction { get { return soilBaseFlowFraction; } }
        public static ISiteVar<double> SoilStormFlowFraction { get { return soilStormFlowFraction; } }
        public static ISiteVar<double> SoilFieldCapacity { get { return soilFieldCapacity; } }
        public static ISiteVar<double> SoilWiltingPoint { get { return soilWiltingPoint; } }
        public static ISiteVar<double> SoilPercentSand { get { return soilPercentSand; } }
        public static ISiteVar<double> SoilPercentClay { get { return soilPercentClay; } }
        //---------------------------------------------------------------------

        /// <summary>
        /// Leaching to a stream - using the soil layer object is a cheat
        /// </summary>
        public static ISiteVar<Layer> Stream
        {
            get {
                return stream;
            }
        }
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
        //---------------------------------------------------------------------

        /// <summary>
        /// Water loss
        /// </summary>
        public static ISiteVar<double> WaterMovement
        {
            get {
                return waterMovement;
            }
            set {
                waterMovement = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Water loss
        /// </summary>
        public static ISiteVar<double> PlantAvailableWater
        {
            get {
                return availableWater;
            }
            set {
                availableWater = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// End-of-month soil water content (cm)
        /// </summary>
        public static ISiteVar<double> SoilWaterContent
        {
            get {
                return soilWaterContent;
            }
            set {
                soilWaterContent = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Mean volumetric water content (proportion)
        /// </summary>
        public static ISiteVar<double> MeanSoilWaterContent
        {
            get
            {
                return meanSoilWaterContent;
            }
            set
            {
                meanSoilWaterContent = value;
            }
        }


        /// <summary>
        /// Liquid Snowpack
        /// </summary>
        public static ISiteVar<double> LiquidSnowPack
        {
            get
            {
                return liquidSnowPack;
            }
            set
            {
                liquidSnowPack = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Available mineral Nitrogen
        /// </summary>
        public static ISiteVar<double> MineralN
        {
            get {
                return mineralN;
            }
            set {
                mineralN = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// The amount of N resorbed before leaf fall
        /// </summary>
        public static ISiteVar<double> ResorbedN
        {
            get
            {
                return resorbedN;
            }
            set
            {
                resorbedN = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> DecayFactor
        {
            get {
                return decayFactor;
            }
            set {
                decayFactor = value;
            }
        }
        //---------------------------------------------------------------------
        
        /// <summary>
        /// Soil temperature (C)
        /// </summary>
        public static ISiteVar<double> SoilTemperature
        {
            get {
                return soilTemperature;
            }
            set {
                soilTemperature = value;
            }
        }
        //---------------------------------------------------------------------
        
        /// <summary>
        /// A generic decay factor determined by soil water and soil temperature.
        /// </summary>
        public static ISiteVar<double> AnaerobicEffect
        {
            get {
                return anaerobicEffect;
            }
            set {
                anaerobicEffect = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Soil moisture at the time of reproduction
        /// </summary>
        public static ISiteVar<int> DryDays
        {
            get
            {
                return dryDays;
            }
            set
            {
                dryDays = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Leaf Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortLeafN
        {
            get {
                return cohortLeafN;
            }
            set {
                cohortLeafN = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Fine Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortFRootN
        {
            get
            {
                return cohortFRootN;
            }
            set
            {
                cohortFRootN = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Carbon in the Leaves
        /// </summary>
        public static ISiteVar<double> CohortLeafC
        {
            get {
                return cohortLeafC;
            }
            set {
                cohortLeafC = value;
            }
        }

        /// <summary>
        /// A summary of all Carbon in the Fine Roots
        /// </summary>
        public static ISiteVar<double> CohortFRootC
        {
            get
            {
                return cohortFRootC;
            }
            set
            {
                cohortFRootC = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Aboveground Wood Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodN
        {
            get {
                return cohortWoodN;
            }
            set {
                cohortWoodN = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A summary of all Coarse Root Nitrogen in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortCRootN
        {
            get
            {
                return cohortCRootN;
            }
            set
            {
                cohortCRootN = value;
            }
        }



        /// <summary>
        /// A summary of all Aboveground Wood Carbon in the Cohorts.
        /// </summary>
        public static ISiteVar<double> CohortWoodC
        {
            get {
                return cohortWoodC;
            }
            set {
                cohortWoodC = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of all Carbon in the Coarse Roots
        /// </summary>
        public static ISiteVar<double> CohortCRootC
        {
            get
            {
                return cohortCRootC;
            }
            set
            {
                cohortCRootC = value;
            }
        }

        //-------------------------

        /// <summary>
        /// A summary of Gross Mineraliztion.
        /// </summary>
        public static ISiteVar<double> GrossMineralization
        {
            get {
                return grossMineralization;
            }
            set {
                grossMineralization = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> AGNPPcarbon
        {
            get {
                return ag_nppC;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double> BGNPPcarbon
        {
            get {
                return bg_nppC;
            }
        }

        
        
        
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Litter fall (g C/m2).
        /// </summary>
        public static ISiteVar<double> LitterfallC
        {
            get {
                return litterfallC;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Aboveground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyAGNPPcarbon
        {
            get {
                return monthlyAGNPPC;
            }
            set {
                monthlyAGNPPC = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Belowground Net Primary Productivity (g C/m2)
        /// </summary>
        public static ISiteVar<double[]> MonthlyBGNPPcarbon
        {
            get {
                return monthlyBGNPPC;
            }
            set {
                monthlyBGNPPC = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of heterotrophic respiration, i.e. CO2 loss from decomposition (g C/m2)
        /// </summary>
        //public static ISiteVar<double[]> MonthlyHeterotrophicResp
        //{
        //    get {
        //        return monthlyHeteroResp;
        //    }
        //    set {
        //        monthlyHeteroResp = value;
        //    }
        //}
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of Net Ecosystem Exchange (g C/m2), from a flux tower's perspective,
        /// whereby positive values indicate terrestrial C loss, negative values indicate C gain.
        /// Replace SourceSink?
        /// </summary>
        public static ISiteVar<double[]> MonthlyNEE
        {
            get {
                return monthlyNEE;
            }
            set {
                monthlyNEE = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of N leaching
        /// </summary>
        public static ISiteVar<double[]> MonthlyStreamN
        {
            get
            {
                return monthlyStreamN;
            }
            set
            {
                monthlyStreamN = value;
            }
        }
        
        /// <summary>
        /// Water loss
        /// </summary>
        public static ISiteVar<Layer> SourceSink
        {
            get {
                return sourceSink;
            }
            set {
                sourceSink = value;
            }
        }
        //---------------------------------------------------------------------
               /// <summary>
        /// A summary of N uptake (g N/m2)
        /// </summary>
        public static ISiteVar<double> TotalNuptake
        {
            get
            {
                return totalNuptake;
            }
            set 
            {
                totalNuptake = value;
            }
                
            
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of frass deposition (g C/m2)
        /// </summary>
        public static ISiteVar<double> FrassC
        {
            get
            {
                return frassC;
            }
            set
            {
                frassC = value;
            }


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// A summary of LAI (m2/m2)
        /// </summary>
        public static ISiteVar<double> LAI
        {
            get
            {
                return lai;
            }
            set
            {
                lai = value;
            }


        }

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
        /// Site-level drought mortality for each species, to map drought impacts
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
 
