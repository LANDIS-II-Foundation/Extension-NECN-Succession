//  Authors: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;

using Landis.Library.InitialCommunities.Universal;
using Landis.Library.Succession;
using Landis.Library.UniversalCohorts;
using Landis.Library.Climate;

using System;
using System.Collections.Generic;
using System.Dynamic;


namespace Landis.Extension.Succession.NECN
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "NECN Succession";
        private static ICore modelCore;
        public static IInputParameters Parameters;
        //public static double[] ShadeLAI;
        public static double AnnualWaterBalance;

        public static string SoilCarbonMapNames = null;
        public static int SoilCarbonMapFrequency;
        public static string SoilNitrogenMapNames = null;
        public static int SoilNitrogenMapFrequency;
        public static string ANPPMapNames = null;
        public static int ANPPMapFrequency;
        public static string ANEEMapNames = null;
        public static int ANEEMapFrequency;
        public static string TotalCMapNames = null;
        public static int TotalCMapFrequency;
        public static string InputCommunityMapNames = null;
        public static int InputCommunityMapFrequency;
        public static int SuccessionTimeStep;
        public static double ProbEstablishAdjust;
        public static double StormFlowOverride = 0.0;

        //public static int FutureClimateBaseYear;
        private ICommunity initialCommunity;

        public static int[] SpeciesByPlant;
        public static int[] SpeciesBySerotiny;
        public static int[] SpeciesByResprout;
        public static int[] SpeciesBySeed;

        //---------------------------------------------------------------------

        public PlugIn()
            : base(ExtensionName)
        {
        }

        //---------------------------------------------------------------------

        public override void LoadParameters(string dataFile,
                                            ICore mCore)
        {
            modelCore = mCore;
            InputParametersParser parser = new InputParametersParser();
            Parameters = Data.Load<IInputParameters>(dataFile, parser);
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        public override void AddCohortData()
        {
            dynamic tempObject =  additionalCohortParameters;
            tempObject.WoodBiomass = 0.0f;
            tempObject.LeafBiomass = 0.0f;
            tempObject.MineralNallocation = 0.0f;  // monthly allocation
            tempObject.MineralNfraction = 0.0f; // annual fraction
            tempObject.Nresorption = 0.0f;
        }


        //---------------------------------------------------------------------

        public override void Initialize()
        {
            ModelCore.UI.WriteLine("Initializing {0} ...", ExtensionName);
            Timestep = Parameters.Timestep;
            SuccessionTimeStep = Timestep;
            ProbEstablishAdjust = Parameters.ProbEstablishAdjustment;
            MetadataHandler.InitializeMetadata(Timestep, modelCore, SoilCarbonMapNames, SoilNitrogenMapNames, ANPPMapNames, ANEEMapNames, TotalCMapNames); 

            //FunctionalType.Initialize(Parameters);
            SpeciesData.Initialize(Parameters);
            SiteVars.Initialize(); // chihiro; this method use functional type data for initializing decay value
            ReadMaps.ReadSoilDepthMap(Parameters.SoilDepthMapName);
            ReadMaps.ReadSoilDrainMap(Parameters.SoilDrainMapName);
            ReadMaps.ReadSoilBaseFlowMap(Parameters.SoilBaseFlowMapName);
            ReadMaps.ReadSoilStormFlowMap(Parameters.SoilStormFlowMapName);
            ReadMaps.ReadFieldCapacityMap(Parameters.SoilFieldCapacityMapName);
            ReadMaps.ReadWiltingPointMap(Parameters.SoilWiltingPointMapName);
            ReadMaps.ReadPercentSandMap(Parameters.SoilPercentSandMapName);
            ReadMaps.ReadPercentClayMap(Parameters.SoilPercentClayMapName);
            ReadMaps.ReadSoilCNMaps(Parameters.InitialSOM1CSurfaceMapName,
                Parameters.InitialSOM1NSurfaceMapName,
                Parameters.InitialSOM1CSoilMapName,
                Parameters.InitialSOM1NSoilMapName,
                Parameters.InitialSOM2CMapName,
                Parameters.InitialSOM2NMapName,
                Parameters.InitialSOM3CMapName,
                Parameters.InitialSOM3NMapName);
            ReadMaps.ReadDeadWoodMaps(Parameters.InitialDeadSurfaceMapName, Parameters.InitialDeadSoilMapName);

            //Optional drought mortality maps
            if (Parameters.NormalSWAMapName != null)
            {
                ReadMaps.ReadNormalSWAMap(Parameters.NormalSWAMapName);
            }
            if (Parameters.NormalCWDMapName != null)
            {
                ReadMaps.ReadNormalCWDMap(Parameters.NormalCWDMapName);
            }
            if (Parameters.NormalTempMapName != null)
            {
                ReadMaps.ReadNormalTempMap(Parameters.NormalTempMapName);
            }

            //Optional topographic maps for adjusting PET
            if (Parameters.SlopeMapName != null)
            {
                ReadMaps.ReadSlopeMap(Parameters.SlopeMapName);
            }
            if (Parameters.AspectMapName != null)
            {
                ReadMaps.ReadAspectMap(Parameters.AspectMapName);
            }

            //Initialize climate.
            Climate.Initialize(Parameters.ClimateConfigFile, false, modelCore);
            //FutureClimateBaseYear = Climate.Future_MonthlyData.Keys.Min();
            ClimateRegionData.Initialize(Parameters);

            OtherData.Initialize(Parameters);
            FireEffects.Initialize(Parameters);

            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Library.UniversalCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            Cohort.ComputeCohortData = ComputeCohortData;
            Initialize(modelCore, Parameters.SeedAlgorithm);

            // Delegate mortality routines:
            Cohort.MortalityEvent += CohortMortality;

            InitializeSites(Parameters.InitialCommunities, Parameters.InitialCommunitiesMap, modelCore);

            if (DroughtMortality.UseDrought | DroughtMortality.OutputSoilWaterAvailable | DroughtMortality.OutputTemperature | DroughtMortality.OutputClimateWaterDeficit)
            {
                DroughtMortality.Initialize(Parameters);
            }

            foreach (ActiveSite site in ModelCore.Landscape)
            {
                Main.ComputeTotalCohortCN(site, SiteVars.Cohorts[site]);
                SiteVars.FineFuels[site] = (SiteVars.SurfaceStructural[site].Carbon + SiteVars.SurfaceMetabolic[site].Carbon) * 2.0;
            }

            Outputs.WritePrimaryLogFile(0);
            Outputs.WriteShortPrimaryLogFile(0);


        }

        //---------------------------------------------------------------------

        public override void Run()
        {

            if (ModelCore.CurrentTime > 0)
            {
                Disturbed.ActiveSiteValues = false;
                SiteVars.ResetDisturbances();
            }

            ClimateRegionData.AnnualNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double>(ModelCore.Ecoregions);
            SpeciesByPlant = new int[ModelCore.Species.Count];
            SpeciesByResprout = new int[ModelCore.Species.Count];
            SpeciesBySerotiny = new int[ModelCore.Species.Count];
            SpeciesBySeed = new int[ModelCore.Species.Count];

            base.Run();

            if (Timestep > 0)
                ClimateRegionData.SetAllEcoregionsFutureAnnualClimate(ModelCore.CurrentTime);

            if (ModelCore.CurrentTime % Timestep == 0)
            {
                // Write monthly log file:
                // Output must reflect the order of operation:
                int[] months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                if (OtherData.CalibrateMode)
                    months = new int[12] { 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 };

                for (int i = 0; i < 12; i++)
                {
                    int month = months[i];
                    Outputs.WriteMonthlyLogFile(month);
                }
                Outputs.WritePrimaryLogFile(ModelCore.CurrentTime);
                Outputs.WriteShortPrimaryLogFile(ModelCore.CurrentTime);
                Outputs.WriteMaps();
                Outputs.WriteReproductionLog(ModelCore.CurrentTime);
                Establishment.LogEstablishment();
                if (InputCommunityMapNames != null && ModelCore.CurrentTime % InputCommunityMapFrequency == 0)
                    Outputs.WriteCommunityMaps();
                if(DroughtMortality.UseDrought)   Outputs.WriteDroughtSpeciesFile(PlugIn.ModelCore.CurrentTime);
            }

        }


        //---------------------------------------------------------------------
        // Although this function is no longer referenced, it is required through inheritance from the succession library
        public override byte ComputeShade(ActiveSite site)
        {
            return (byte) SiteVars.LAI[site]; // finalShade;
        }


        //---------------------------------------------------------------------
        protected override void InitializeSite(ActiveSite site)
        {
            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            //SiteVars.MineralN[site] = Parameters.InitialMineralN;
        }


        //---------------------------------------------------------------------
        // The delegated cohort mortality method.  Triggers regeneration.
        public void CohortMortality(object sender, MortalityEventArgs eventArgs)
        {
            if(OtherData.CalibrateMode) ModelCore.UI.WriteLine("Cohort Partial Mortality:  {0}", eventArgs.Site);

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;

            ICohort cohort = (Landis.Library.UniversalCohorts.ICohort) eventArgs.Cohort;

            double fractionPartialMortality = (double)eventArgs.Reduction;

            double foliarInput = cohort.Data.AdditionalParameters.LeafBiomass * fractionPartialMortality;
            double woodInput = cohort.Data.AdditionalParameters.WoodBiomass * fractionPartialMortality;

            if (disturbanceType != null)
            {

                if (eventArgs.DisturbanceType != null && disturbanceType.IsMemberOf("disturbance:harvest"))
                {
                    SiteVars.HarvestPrescriptionName = ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
                    if (ModelCore.CurrentTime > SiteVars.HarvestDisturbedYear[site]) // this is the first cohort killed/damaged
                    {
                        //PlugIn.ModelCore.UI.WriteLine("   Begin harvest layer reductions...");
                        HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
                        SiteVars.HarvestDisturbedYear[site] = ModelCore.CurrentTime;
                    }
                    woodInput -= woodInput * HarvestEffects.GetCohortWoodRemoval(site);
                    foliarInput -= foliarInput * HarvestEffects.GetCohortLeafRemoval(site);
                }
                if (eventArgs.DisturbanceType != null && disturbanceType.IsMemberOf("disturbance:fire"))
                {

                    SiteVars.FireSeverity = ModelCore.GetSiteVar<byte>("Fire.Severity");

                    if (ModelCore.CurrentTime > SiteVars.FireDisturbedYear[site]) // this is the first cohort killed/damaged
                    {
                        SiteVars.SmolderConsumption[site] = 0.0;
                        SiteVars.FlamingConsumption[site] = 0.0;
                        if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                            FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);

                        SiteVars.FireDisturbedYear[site] = ModelCore.CurrentTime;

                    }

                    double live_woodFireConsumption = woodInput * FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortWoodReduction;
                    double live_foliarFireConsumption = foliarInput * FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortLeafReduction;

                    SiteVars.SmolderConsumption[site] += live_woodFireConsumption;
                    SiteVars.FlamingConsumption[site] += live_foliarFireConsumption;
                    SiteVars.SourceSink[site].Carbon += live_woodFireConsumption * 0.47;
                    SiteVars.SourceSink[site].Carbon += live_foliarFireConsumption * 0.47;
                    woodInput -= live_woodFireConsumption;
                    foliarInput -= live_foliarFireConsumption;

                }
                if (eventArgs.DisturbanceType != null && disturbanceType.IsMemberOf("disturbance:browse"))
                {
                    //Sam Flake: account for browsed biomass nutrient cycling. All browser waste treated as leaves with high N content. 
                    // This overestimates moose waste if there is a lot of cohort mortality versus browse eaten. 

                    foliarInput += woodInput;
                    woodInput = 0;

                    double inputDecayValue = 1.0;   // Decay value is calculated for surface/soil layers (leaf/fine root), 
                                                    // therefore, this is just a dummy value.

                    if (foliarInput > 0)
                    {
                        SiteVars.LitterfallC[site] += foliarInput * 0.47;
                        foliarInput = foliarInput * 0.1;                     //most carbon is respired

                        //N content of feces is approximately 1.6% for deer(Asada and Ochiai 1999), between 1.45% and 2.26% for deer (Howery and Pfister, 1990), 2.5%  for deer (Euan et al. 2020),
                        //1.33% in winter, 2.44% for moose in summer (Persson et al. 2000), 2.4% for moose (Kuijper et al. 2016)
                        //Feces N = 5.7 kg per moose per year (Persson et al. 2000)
                        //N in urine is 0.5% in summer (Persson et al. 2000), 3675 L urine per moose per year (Persson et al. 2000)
                        //Urine is 0.5% N = 18.375 kg N per year per moose (assuming summer and winter N content is the same)
                        //Total N for moose waste = 24 kg per moose per year; Each moose eats 2738 kg biomass per year
                        //Foliar inputs are 2738 * 0.47 * 0.1 kg C  = 128.67 kg C per moose; CN ratio = 128/24 = 5.33

                        LitterLayer.PartitionResidue(
                                    foliarInput,
                                    inputDecayValue,
                                    5.33, //CN ratio for browse waste -- metabolic
                                    1, //"lignin" content of waste
                                    5.33, //CN ratio for browse waste -- structural
                                    LayerName.Leaf,
                                    LayerType.Surface,
                                    site);

                        Disturbed[site] = false;

                        return;
                    }
                }
                Disturbed[site] = true;
            }
            
            if (SpeciesData.Grass[cohort.Species])
            {
                ForestFloor.AddFoliageLitter(woodInput + foliarInput, cohort.Species, site);  //  Wood biomass of grass species is transfered to non wood litter. (W.Hotta 2021.12.16)

                Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, (cohort.Data.AdditionalParameters.WoodBiomass + cohort.Data.AdditionalParameters.LeafBiomass) * fractionPartialMortality), cohort, cohort.Species, site);
            }
            else
            {
                ForestFloor.AddWoodLitter(woodInput, cohort.Species, site);
                ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, site);

                Roots.AddCoarseRootLitter(Roots.CalculateCoarseRoot(cohort, cohort.Data.AdditionalParameters.WoodBiomass * fractionPartialMortality), cohort, cohort.Species, site);
                Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, cohort.Data.AdditionalParameters.LeafBiomass * fractionPartialMortality), cohort, cohort.Species, site);
                
            }
            
           return;
        }
        //---------------------------------------------------------------------
        // Total mortality, including from disturbance or senescence.

        //public void CohortTotalMortality(object sender, DeathEventArgs eventArgs)
        //{

        //    //PlugIn.ModelCore.UI.WriteLine("Cohort Total Mortality: {0}", eventArgs.Site);

        //    ExtensionType disturbanceType = eventArgs.DisturbanceType;

        //    ActiveSite site = eventArgs.Site;

        //    ICohort cohort = (Landis.Library.UniversalCohorts.ICohort)eventArgs.Cohort;
        //    double foliarInput = (double)cohort.Data.AdditionalParameters.LeafBiomass;
        //    double woodInput = (double)cohort.Data.AdditionalParameters.WoodBiomass;
        //    cohort.Data.AdditionalParameters.Test = 1;

        //        //PlugIn.ModelCore.UI.WriteLine("DISTURBANCE EVENT: Cohort Died: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, eventArgs.DisturbanceType);

        //        if (disturbanceType.IsMemberOf("disturbance:fire"))
        //        {
        //            SiteVars.FireSeverity = ModelCore.GetSiteVar<byte>("Fire.Severity");
        //            Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);

        //            if (ModelCore.CurrentTime > SiteVars.FireDisturbedYear[site])  // the first cohort killed/damaged
        //            {
        //                SiteVars.SmolderConsumption[site] = 0.0;
        //                SiteVars.FlamingConsumption[site] = 0.0;
        //                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
        //                    FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);

        //                SiteVars.FireDisturbedYear[site] = ModelCore.CurrentTime;

        //            }

        //            double woodFireConsumption = woodInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortWoodReduction;
        //            double foliarFireConsumption = foliarInput * (float)FireEffects.ReductionsTable[(int)SiteVars.FireSeverity[site]].CohortLeafReduction;

        //            SiteVars.SmolderConsumption[site] += woodFireConsumption;
        //            SiteVars.FlamingConsumption[site] += foliarFireConsumption;
        //            SiteVars.SourceSink[site].Carbon += woodFireConsumption * 0.47;
        //            SiteVars.SourceSink[site].Carbon += foliarFireConsumption * 0.47;
        //            woodInput -= woodFireConsumption;
        //            foliarInput -= foliarFireConsumption;

        //        }
        //        else
        //        {
        //            if (disturbanceType.IsMemberOf("disturbance:harvest"))
        //            {
        //                SiteVars.HarvestPrescriptionName = ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
        //                if (ModelCore.CurrentTime > SiteVars.HarvestDisturbedYear[site])  // the first cohort killed/damaged
        //                {
        //                    HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
        //                }
        //                double woodLoss = woodInput * (float)HarvestEffects.GetCohortWoodRemoval(site);
        //                double foliarLoss = foliarInput * (float)HarvestEffects.GetCohortLeafRemoval(site);
        //                SiteVars.SourceSink[site].Carbon += woodLoss * 0.47;
        //                SiteVars.SourceSink[site].Carbon += foliarLoss * 0.47;
        //                woodInput -= woodLoss;
        //                foliarInput -= foliarLoss;
        //                SiteVars.HarvestDisturbedYear[site] = ModelCore.CurrentTime;
        //            }

        //            // If not fire, check for resprouting:
        //            Reproduction.CheckForResprouting(eventArgs.Cohort, site);
        //        }
        //    }


        //    if (SpeciesData.Grass[cohort.Species])
        //    {
        //        //PlugIn.ModelCore.UI.WriteLine("Cohort Died: species={0}, age={1}, wood={2:0.00}, foliage={3:0.00}.", cohort.Species.Name, cohort.Age, wood, foliar);
        //        ForestFloor.AddFoliageLitter(woodInput + foliarInput, cohort.Species, eventArgs.Site);  //  Wood biomass of grass species is transfered to non wood litter. (W.Hotta 2021.12.16)

        //        // Assume that ALL dead root biomass stays on site.
        //        Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, cohort.Data.AdditionalParameters.WoodBiomass + cohort.Data.AdditionalParameters.LeafBiomass), cohort, cohort.Species, eventArgs.Site);
        //    }
        //    else
        //    {
        //        //PlugIn.ModelCore.UI.WriteLine("Cohort Died: species={0}, age={1}, wood={2:0.00}, foliage={3:0.00}.", cohort.Species.Name, cohort.Age, wood, foliar);
        //        ForestFloor.AddWoodLitter(woodInput, cohort.Species, eventArgs.Site);
        //        ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, eventArgs.Site);

        //        // Assume that ALL dead root biomass stays on site.
        //        Roots.AddCoarseRootLitter(Roots.CalculateCoarseRoot(cohort, cohort.Data.AdditionalParameters.WoodBiomass), cohort, cohort.Species, eventArgs.Site);
        //        Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, cohort.Data.AdditionalParameters.LeafBiomass), cohort, cohort.Species, eventArgs.Site);
        //    }
            

        //    if (disturbanceType != null)
        //        Disturbed[site] = true;

        //    return;
        //}

        //---------------------------------------------------------------------
        //Grows the cohorts for future climate
        protected override void AgeCohorts(ActiveSite site,
                                           ushort years,
                                           int? successionTimestep)
        {
            Main.Run(site, years, successionTimestep.HasValue);

        }
        //---------------------------------------------------------------------
        // <summary>
        // Determines if there is sufficient light at a site for a species to
        // germinate/resprout.
        // This is a Delegate method to base succession.
        // W.Hotta and Chihiro modified - Modify light probability based on the amount of nursery log on the site
        // </summary>

        public bool SufficientLight(ISpecies species, ActiveSite site)
        {

            bool isSufficientlight = false;
            double lightProbability = 0.0;

            string regenType = "failed"; // Identify where the cohort established; Chihiro
            //SF regenType is only used in CalibrateMode

            var random = new Troschuetz.Random.TRandom();
            double a = SpeciesData.LightLAIShape[species];
            double b = SpeciesData.LightLAIScale[species];
            double c = SpeciesData.LightLAILocation[species];
            double adjust = SpeciesData.LightLAIAdjust[species];
            double lai = SiteVars.LAI[site];

            lightProbability = adjust * (((a / b) * Math.Pow((lai / b), (a - 1)) * Math.Exp(-Math.Pow((lai / b), a))) + c); //3-parameter Weibull PDF equation
            lightProbability = Math.Min(lightProbability, 1.0);
            //if(OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Estimated Weibull light probability for species {0} = {1:0.000}, at LAI = {2:0.00}", species.Name, lightProbability, SiteVars.LAI[site]);
            

            if (modelCore.GenerateUniform() < lightProbability)
                isSufficientlight = true;

            // ------------------------------------------------------------------------
            // Modify light probability modified by the amount of nursery log on the site
            // W.Hotta 2020.01.22
            //
            // Compute the availability of nursery log on the site
            //   Option1: function type is linear
            //   Option2: function type is power

            if (!SpeciesData.NurseLog_depend[species])
                return isSufficientlight;

            double nurseryLogAvailabilityModifier = 2.0; // tuning parameter (only even)
            double nurseryLogAvailability = 1 - Math.Pow(ComputeNurseryLogAreaRatio(species, site) - 1, nurseryLogAvailabilityModifier);
            if (OtherData.CalibrateMode)
            {
                ModelCore.UI.WriteLine("original_lightProbability:{0},{1},{2}", ModelCore.CurrentTime, species.Name, lightProbability);
                ModelCore.UI.WriteLine("siteLAI:{0}", SiteVars.LAI[site]);
            }

            // Case 1. CWD-dependent species (species which can only be established on nursery log)
            if (SpeciesData.NurseLog_depend[species]) // W.Hotta (2021.08.01)
            {
                lightProbability *= nurseryLogAvailability;
                isSufficientlight = modelCore.GenerateUniform() < lightProbability;
                if (isSufficientlight) regenType = "nurse_log";
            }
            // Case 2. CWD-independent species (species which can be established on both forest floor & nursery log)
            else
            {
                // 1. Can the cohort establish on forest floor? (lightProbability is considering both Tree and Grass species)
                if (modelCore.GenerateUniform() < lightProbability)
                {
                    isSufficientlight = true;
                    regenType = "surface";
                }
                if (OtherData.CalibrateMode)
                {
                    ModelCore.UI.WriteLine("nurseryLogPenalty:{0},{1},{2}", ModelCore.CurrentTime, species.Name, nurseryLogAvailability);
                    ModelCore.UI.WriteLine("modified_lightProbability:{0},{1},{2}", ModelCore.CurrentTime, species.Name, lightProbability);
                    ModelCore.UI.WriteLine("regeneration_type:{0},{1},{2}", ModelCore.CurrentTime, species.Name, regenType);
                }
            }

            return isSufficientlight;
        }

        //---------------------------------------------------------------------
        // <summary>
        // Compute the ratio of projected area (= occupancy area) of nursery logs to the grid area.
        // W.Hotta & Chihiro;
        //
        // Description: 
        //     - Every SiteVars.CurrentDeadWoodC[site] is downed logs.
        //     - Only the downed logs (SiteVars.CurrentDeadWoodC[site]) which decay class is between 3 to 5 
        //       are suitable for establishment and treated as nursery logs.
        //     - The carbon stocks of the nursery logs are converted to volume 
        //       using a wood density of each decay class.
        //     - Then, the volume is converted to the projected area (occupation area) 
        //       using the mean height of downed logs derived from field data.
        //         - The shape of downed logs were assumed to be an elliptical cylinder
        // </summary>

        private static double ComputeNurseryLogAreaRatio(ISpecies species, ActiveSite site)
        {
            // Hight of downed logs
            double hight = 28.64; // Units: cm

            // Wood density (g cm^-3) of dead wood for each decay class.
            // Decay class 3-5 is suitable for establishment.
            // Reference: Unidentified spp category in Table 3 of Ugawa et al. (2012)
            //            https://www.ffpri.affrc.go.jp/pubs/bulletin/425/documents/425-2.pdf
            double densityDecayClass0 = 0.421;
            double densityDecayClass3 = 0.255;
            double densityDecayClass4 = 0.178;
            double densityDecayClass5 = 0.112;

            // Compute the amount of nursery log carbon (gC m^-2)
            double[] nurseryLogC = ComputeNurseryLogC(site, densityDecayClass0, densityDecayClass3, densityDecayClass4, densityDecayClass5);

            // Compute the area ratio in the site of the nursery log occupies.
            // The shape of downed logs were assumed to be an elliptical cylinder
            // Variables:
            //   decayClassXAreaRatio (-)
            //   nurseryLogC[X] (gC m^-2)
            //   height (cm)
            //   densityDecayClass[X] (gC cm^-3)
            double decayClass3AreaRatio = 4 * 2 * nurseryLogC[0] / (Math.PI * hight * densityDecayClass3) * Math.Pow(10, -4); // Decay class 3
            double decayClass4AreaRatio = 4 * 2 * nurseryLogC[1] / (Math.PI * hight * densityDecayClass4) * Math.Pow(10, -4); // Decay class 4
            double decayClass5AreaRatio = 4 * 2 * nurseryLogC[2] / (Math.PI * hight * densityDecayClass5) * Math.Pow(10, -4); // Decay class 5
            if (OtherData.CalibrateMode && species.Index == 0)
            {
                ModelCore.UI.WriteLine("nurseryLogC:{0},{1},{2},{3}", ModelCore.CurrentTime, nurseryLogC[0], nurseryLogC[1], nurseryLogC[2]);
                ModelCore.UI.WriteLine("decayClassAreaRatios:{0},{1},{2},{3}", ModelCore.CurrentTime, decayClass3AreaRatio, decayClass4AreaRatio, decayClass5AreaRatio);
            }
            return Math.Min(1.0, decayClass3AreaRatio + decayClass4AreaRatio + decayClass5AreaRatio);
        }


        //---------------------------------------------------------------------
        // <summary>
        /// Compute the amount of nursery log carbon based on its decay ratio
        // W.Hotta & Chihiro Description: 
        //     - In the process of decomposition of downed logs, 
        //       the volume remains the same, only the density changes.
        // </summary>

        private static double[] ComputeNurseryLogC(ActiveSite site, double densityDecayClass0, double densityDecayClass3, double densityDecayClass4, double densityDecayClass5)
        {
            // Define thresholds to identify decay class
            double retentionRatioThreshold3 = densityDecayClass3 / densityDecayClass0;
            double retentionRatioThreshold4 = densityDecayClass4 / densityDecayClass0;
            double retentionRatioThreshold5 = densityDecayClass5 / densityDecayClass0;

            // Initialize nursery log carbon for each decay class
            double decayClass3 = 0.0;
            double decayClass4 = 0.0;
            double decayClass5 = 0.0;

            // Update the amount of carbon for each decayClass
            for (int i = 0; i < SiteVars.CurrentDeadWoodC[site].Length; i++)
            {
                // Compute the ratio of the current dead wood C to the origindal dead wood C
                double retentionRatio = SiteVars.CurrentDeadWoodC[site][i] / SiteVars.OriginalDeadWoodC[site][i];
                // PlugIn.ModelCore.UI.WriteLine("decayRatio:{0},{1}", PlugIn.ModelCore.CurrentTime, decayRatio);

                // Identify the decay class of the current dead wood carbon & update the amount of C of each decay class (i.e. the amount of carbon just after the focused dead wood was generated.)
                if (retentionRatio >= retentionRatioThreshold4 & retentionRatio < retentionRatioThreshold3)
                {
                    decayClass3 += SiteVars.CurrentDeadWoodC[site][i];
                }
                else if (retentionRatio >= retentionRatioThreshold5 & retentionRatio < retentionRatioThreshold4)
                {
                    decayClass4 += SiteVars.CurrentDeadWoodC[site][i];
                }
                else if (retentionRatio < retentionRatioThreshold5)
                {
                    decayClass5 += SiteVars.CurrentDeadWoodC[site][i];
                }
            }
            // PlugIn.ModelCore.UI.WriteLine("decayClasses:{0},{1},{2},{3}", PlugIn.ModelCore.CurrentTime, decayClass3, decayClass4, decayClass5);
            return new double[3] { decayClass3, decayClass4, decayClass5 };
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Add a new cohort to a site following reproduction or planting.  Does not include initial communities.
        /// This is a Delegate method to base succession.
        /// </summary>

        public void AddNewCohort(ISpecies species, ActiveSite site, string reproductionType, double propBiomass = 1.0)
        {
            float[] initialBiomass = CohortBiomass.InitialBiomass(species, SiteVars.Cohorts[site], site);

            ExpandoObject woodLeafBiomasses = new ExpandoObject();
            dynamic tempObject = woodLeafBiomasses;
            tempObject.WoodBiomass = initialBiomass[0];
            tempObject.LeafBiomass = initialBiomass[1];

            SiteVars.Cohorts[site].AddNewCohort(species, 1, Convert.ToInt32(initialBiomass[0] + initialBiomass[1]), woodLeafBiomasses);

            if (reproductionType == "plant")
                SpeciesByPlant[species.Index]++;
            else if (reproductionType == "serotiny")
                SpeciesBySerotiny[species.Index]++;
            else if (reproductionType == "resprout")
                SpeciesByResprout[species.Index]++;
            else if (reproductionType == "seed")
                SpeciesBySeed[species.Index]++;

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool Establish(ISpecies species, ActiveSite site)
        {
            double establishProbability = Establishment.Calculate(species, site);

            return modelCore.GenerateUniform() < establishProbability;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool PlantingEstablish(ISpecies species, ActiveSite site)
        {
            IEcoregion ecoregion = modelCore.Ecoregion[site];
            double establishProbability = Establishment.Calculate(species, site);

            return establishProbability > 0.0;
        }

        //---------------------------------------------------------------------

        public CohortData ComputeCohortData(ushort age, int biomass, int anpp, ExpandoObject parametersToAdd)
        {
            IDictionary<string, object> tempObject = parametersToAdd;

            int newBiomass = Convert.ToInt32(tempObject["LeafBiomass"]) + Convert.ToInt32(tempObject["WoodBiomass"]);

            return new CohortData(age, newBiomass, anpp, parametersToAdd);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if there is a mature cohort at a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool MaturePresent(ISpecies species, ActiveSite site)
        {
            return SiteVars.Cohorts[site].IsMaturePresent(species);
        }

        public override void InitializeSites(string initialCommunitiesText, string initialCommunitiesMap, ICore modelCore)
        {
            ModelCore.UI.WriteLine("   Loading initial communities from file \"{0}\" ...", initialCommunitiesText);

            Landis.Library.InitialCommunities.Universal.DatasetParser parser = new Landis.Library.InitialCommunities.Universal.DatasetParser(Timestep, ModelCore.Species, additionalCohortParameters, initialCommunitiesText);
            Landis.Library.InitialCommunities.Universal.IDataset communities = Data.Load<Landis.Library.InitialCommunities.Universal.IDataset>(initialCommunitiesText, parser);

            ModelCore.UI.WriteLine("   Reading initial communities map \"{0}\" ...", initialCommunitiesMap);
            IInputRaster<UIntPixel> map;
            map = ModelCore.OpenRaster<UIntPixel>(initialCommunitiesMap);
            using (map)
            {
                UIntPixel pixel = map.BufferPixel;
                foreach (Site site in ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    uint mapCode = pixel.MapCode.Value;
                    if (!site.IsActive)
                        continue;

                    ActiveSite activeSite = (ActiveSite)site;
                    SiteVars.MineralN[site] = Parameters.InitialMineralN;

                    initialCommunity = communities.Find(mapCode);
                    if (initialCommunity == null)
                    {
                        //ModelCore.UI.WriteLine("   Map Code {0} does not have an initial community", mapCode);
                        SiteVars.Cohorts[site] = new SiteCohorts();
                        //throw new ApplicationException(string.Format("Unknown map code for initial community: {0}", mapCode));
                    }
                    else
                    {
                        InitializeSite(activeSite);
                    }

                }
            }
        }
    }
    }

