//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Library.InitialCommunities;
using Landis.Library.Succession;
using Landis.Library.LeafBiomassCohorts;
using Landis.Library.Climate;
using Landis.Library.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Landis.Extension.Succession.NECN_Hydro
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "NECN_Hydro Succession";
        private static ICore modelCore;
        public static IInputParameters Parameters;
        public static double[] ShadeLAI;
        public static double AnnualWaterBalance;

        private List<ISufficientLight> sufficientLight;
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
        //public static int ShadeClassMapNames = null;
        //public static int ShadeClassMapFrequency;
        public static int SuccessionTimeStep;
        public static double ProbEstablishAdjust;

        public static int FutureClimateBaseYear;
        public static int B_MAX;

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
            SiteVars.Initialize();
            InputParametersParser parser = new InputParametersParser();
            Parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);

        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }


        //---------------------------------------------------------------------

        public override void Initialize()
        {
            PlugIn.ModelCore.UI.WriteLine("Initializing {0} ...", ExtensionName);
            Timestep              = Parameters.Timestep;
            SuccessionTimeStep    = Timestep;
            sufficientLight       = Parameters.LightClassProbabilities;
            ProbEstablishAdjust = Parameters.ProbEstablishAdjustment;
            MetadataHandler.InitializeMetadata(Timestep, modelCore, SoilCarbonMapNames, SoilNitrogenMapNames, ANPPMapNames, ANEEMapNames, TotalCMapNames); //,LAIMapNames, ShadeClassMapNames);
            //CohortBiomass.SpinupMortalityFraction = parameters.SpinupMortalityFraction;
            
            //Initialize climate.
            Climate.Initialize(Parameters.ClimateConfigFile, false, modelCore);
            FutureClimateBaseYear = Climate.Future_MonthlyData.Keys.Min();
            
            ClimateRegionData.Initialize(Parameters);
            SpeciesData.Initialize(Parameters);
            Util.ReadSoilDepthMap(Parameters.SoilDepthMapName);
            Util.ReadSoilDrainMap(Parameters.SoilDrainMapName);
            Util.ReadSoilBaseFlowMap(Parameters.SoilBaseFlowMapName);
            Util.ReadSoilStormFlowMap(Parameters.SoilStormFlowMapName);
            Util.ReadFieldCapacityMap(Parameters.SoilFieldCapacityMapName);
            Util.ReadWiltingPointMap(Parameters.SoilWiltingPointMapName);
            Util.ReadPercentSandMap(Parameters.SoilPercentSandMapName);
            Util.ReadPercentClayMap(Parameters.SoilPercentClayMapName);
            Util.ReadSoilCNMaps(Parameters.InitialSOM1CSurfaceMapName, 
                Parameters.InitialSOM1NSurfaceMapName,
                Parameters.InitialSOM1CSoilMapName,
                Parameters.InitialSOM1NSoilMapName,
                Parameters.InitialSOM2CMapName,
                Parameters.InitialSOM2NMapName,
                Parameters.InitialSOM3CMapName,
                Parameters.InitialSOM3NMapName);
            Util.ReadDeadWoodMaps(Parameters.InitialDeadSurfaceMapName, Parameters.InitialDeadSoilMapName);

            ShadeLAI = Parameters.MaximumShadeLAI; //.MinRelativeBiomass;
            OtherData.Initialize(Parameters);
            FunctionalType.Initialize(Parameters);
            FireEffects.Initialize(Parameters);
            HarvestEffects.Initialize(Parameters);

            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Library.LeafBiomassCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            base.Initialize(modelCore, Parameters.SeedAlgorithm);
            Landis.Library.LeafBiomassCohorts.Cohort.PartialDeathEvent += CohortPartialMortality;
            Landis.Library.LeafBiomassCohorts.Cohort.DeathEvent += CohortTotalMortality;
            //Landis.Library.LeafBiomassCohorts.Cohort.AgeOnlyDeathEvent += CohortTotalMortality;
            //Landis.Library.LeafBiomassCohorts.SiteCohorts.AgeOnlyDisturbanceEvent += SiteDisturbed;
            //AgeOnlyDisturbances.Module.Initialize(Parameters.AgeOnlyDisturbanceParms);


            InitializeSites(Parameters.InitialCommunities, Parameters.InitialCommunitiesMap, modelCore); 
            
            if (Parameters.CalibrateMode)
                Outputs.CreateCalibrateLogFile();
            Establishment.InitializeLogFile();

            B_MAX = 0;
            foreach(ISpecies species in ModelCore.Species)
            {
                if (SpeciesData.Max_Biomass[species] > B_MAX)
                    B_MAX = SpeciesData.Max_Biomass[species];
            }

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
                Main.ComputeTotalCohortCN(site, SiteVars.Cohorts[site]);

            Outputs.WritePrimaryLogFile(0);
            Outputs.WriteShortPrimaryLogFile(0);

            
        }

        //---------------------------------------------------------------------

        public override void Run()
        {
            
            if (PlugIn.ModelCore.CurrentTime > 0)
                    SiteVars.InitializeDisturbances();

            ClimateRegionData.AnnualNDeposition = new Ecoregions.AuxParm<double>(PlugIn.ModelCore.Ecoregions);

            //base.RunReproductionFirst();

            base.Run();

            if(Timestep > 0)
                ClimateRegionData.SetAllEcoregions_FutureAnnualClimate(ModelCore.CurrentTime);

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
                Outputs.WritePrimaryLogFile(PlugIn.ModelCore.CurrentTime);
                Outputs.WriteShortPrimaryLogFile(PlugIn.ModelCore.CurrentTime);
                Outputs.WriteMaps();
                Establishment.LogEstablishment();
            }

        }


        //---------------------------------------------------------------------

        public override byte ComputeShade(ActiveSite site)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            byte finalShade = 0;

            if (!ecoregion.Active)
                return 0;

            for (byte shade = 5; shade >= 1; shade--)
            {
                if(PlugIn.ShadeLAI[shade] <=0 ) 
                {
                    string mesg = string.Format("Maximum LAI has not been defined for shade class {0}", shade);
                    throw new System.ApplicationException(mesg);
                }
                if (SiteVars.LAI[site] >= PlugIn.ShadeLAI[shade])
                {
                    finalShade = shade;
                    break;
                }
            }

            //PlugIn.ModelCore.UI.WriteLine("Yr={0},      Shade Calculation:  B_MAX={1}, B_ACT={2}, Shade={3}.", PlugIn.ModelCore.CurrentTime, B_MAX, B_ACT, finalShade);

            return finalShade;
        }
        //---------------------------------------------------------------------

        protected override void InitializeSite(ActiveSite site,
                                               ICommunity initialCommunity)
        {

            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            SiteVars.MineralN[site] = Parameters.InitialMineralN;
        }


        //---------------------------------------------------------------------

        public void CohortPartialMortality(object sender, Landis.Library.BiomassCohorts.PartialDeathEventArgs eventArgs)
        {
            //PlugIn.ModelCore.UI.WriteLine("Cohort Partial Mortality");

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;
            Disturbed[site] = true;

            ICohort cohort = (Landis.Library.LeafBiomassCohorts.ICohort)eventArgs.Cohort;

            float fractionPartialMortality = (float)eventArgs.Reduction;
            //PlugIn.ModelCore.UI.WriteLine("Cohort experienced partial mortality: species={0}, age={1}, wood_biomass={2}, fraction_mortality={3:0.0}.", cohort.Species.Name, cohort.Age, cohort.WoodBiomass, fractionPartialMortality);

            float foliarInput = cohort.LeafBiomass * fractionPartialMortality;
            float woodInput = cohort.WoodBiomass * fractionPartialMortality;

            if (disturbanceType.IsMemberOf("disturbance:harvest"))
            {
                SiteVars.HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");

                woodInput -= woodInput * (float) HarvestEffects.GetCohortWoodRemoval(site);
                foliarInput -= foliarInput * (float) HarvestEffects.GetCohortLeafRemoval(site);
            }

            ForestFloor.AddWoodLitter(woodInput, cohort.Species, site);
            ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, site);

            Roots.AddCoarseRootLitter(Roots.CalculateCoarseRoot(cohort, woodInput), cohort, cohort.Species, site);  
            Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, foliarInput), cohort, cohort.Species, site);

            // Although there isn't currently a partial-mortality fire extension, there could be in the future.
            if (disturbanceType.IsMemberOf("disturbance:fire"))
            {
                SiteVars.FireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                    FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);
            }
            if (disturbanceType.IsMemberOf("disturbance:harvest"))
            {
                HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
            }

            //PlugIn.ModelCore.UI.WriteLine("EVENT: Cohort Partial Mortality: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, disturbanceType);
            //PlugIn.ModelCore.UI.WriteLine("       Cohort Reductions:  Foliar={0:0.00}.  Wood={1:0.00}.", cohortReductions.Foliar, cohortReductions.Wood);
            //PlugIn.ModelCore.UI.WriteLine("       InputB/TotalB:  Foliar={0:0.00}/{1:0.00}, Wood={2:0.0}/{3:0.0}.", foliarInput, foliar, woodInput, wood);

            return;
        }
        //---------------------------------------------------------------------
        // Not just sleeve mortality... true mortality

        public void CohortTotalMortality(object sender, Landis.Library.BiomassCohorts.DeathEventArgs eventArgs)
        {

            //PlugIn.ModelCore.UI.WriteLine("Cohort Total Mortality");

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;

            ICohort cohort = (Landis.Library.LeafBiomassCohorts.ICohort) eventArgs.Cohort;
            double foliar = (double) cohort.LeafBiomass;
            double wood = (double) cohort.WoodBiomass;

            if (disturbanceType != null)
            {
                //PlugIn.ModelCore.UI.WriteLine("DISTURBANCE EVENT: Cohort Died: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, eventArgs.DisturbanceType);

                Disturbed[site] = true;
                if (disturbanceType.IsMemberOf("disturbance:fire"))
                {
                    Landis.Library.Succession.Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);
                    SiteVars.FireSeverity = PlugIn.ModelCore.GetSiteVar<byte>("Fire.Severity");
                }
                else
                {
                    if (disturbanceType.IsMemberOf("disturbance:harvest"))
                    {
                        SiteVars.HarvestPrescriptionName = PlugIn.ModelCore.GetSiteVar<string>("Harvest.PrescriptionName");
                        wood -= wood * HarvestEffects.GetCohortWoodRemoval(site);
                        foliar -= foliar * HarvestEffects.GetCohortLeafRemoval(site);
                    }
                    Landis.Library.Succession.Reproduction.CheckForResprouting(eventArgs.Cohort, site);
                }
            }

            //PlugIn.ModelCore.UI.WriteLine("Cohort Died: species={0}, age={1}, wood={2:0.00}, foliage={3:0.00}.", cohort.Species.Name, cohort.Age, wood, foliar);
            ForestFloor.AddWoodLitter(wood, cohort.Species, eventArgs.Site);
            ForestFloor.AddFoliageLitter(foliar, cohort.Species, eventArgs.Site);

            // Assume that ALL dead root biomass stays on site.
            Roots.AddCoarseRootLitter(Roots.CalculateCoarseRoot(cohort, cohort.WoodBiomass), cohort, cohort.Species, eventArgs.Site);
            Roots.AddFineRootLitter(Roots.CalculateFineRoot(cohort, cohort.LeafBiomass), cohort, cohort.Species, eventArgs.Site);

            if (disturbanceType.IsMemberOf("disturbance:fire"))
            {
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                    FireEffects.ReduceLayers(SiteVars.FireSeverity[site], site);
            }
            if (disturbanceType.IsMemberOf("disturbance:harvest"))
            {
                HarvestEffects.ReduceLayers(SiteVars.HarvestPrescriptionName[site], site);
            }
            return;
        }

        //---------------------------------------------------------------------
        //Grows the cohorts for future climate
        protected override void AgeCohorts(ActiveSite site,
                                           ushort     years,
                                           int?       successionTimestep)
        {
            Main.Run(site, years, successionTimestep.HasValue);

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines if there is sufficient light at a site for a species to
        /// germinate/resprout.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool SufficientLight(ISpecies species, ActiveSite site)
        {

            //PlugIn.ModelCore.UI.WriteLine("  Calculating Sufficient Light from Succession.");
            byte siteShade = PlugIn.ModelCore.GetSiteVar<byte>("Shade")[site];

            double lightProbability = 0.0;
            bool found = false;

            foreach (ISufficientLight lights in sufficientLight)
            {

                //PlugIn.ModelCore.UI.WriteLine("Sufficient Light:  ShadeClass={0}, Prob0={1}.", lights.ShadeClass, lights.ProbabilityLight0);
                if (lights.ShadeClass == species.ShadeTolerance)
                {
                    if (siteShade == 0) lightProbability = lights.ProbabilityLight0;
                    if (siteShade == 1) lightProbability = lights.ProbabilityLight1;
                    if (siteShade == 2) lightProbability = lights.ProbabilityLight2;
                    if (siteShade == 3) lightProbability = lights.ProbabilityLight3;
                    if (siteShade == 4) lightProbability = lights.ProbabilityLight4;
                    if (siteShade == 5) lightProbability = lights.ProbabilityLight5;
                    found = true;
                }
            }

            if (!found)
                PlugIn.ModelCore.UI.WriteLine("A Sufficient Light value was not found for {0}.", species.Name);

            return modelCore.GenerateUniform() < lightProbability;

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Add a new cohort to a site following reproduction or planting.  Does not include initial communities.
        /// This is a Delegate method to base succession.
        /// </summary>

        public void AddNewCohort(ISpecies species, ActiveSite site)
        {
            float[] initialBiomass = CohortBiomass.InitialBiomass(species, SiteVars.Cohorts[site], site);
            SiteVars.Cohorts[site].AddNewCohort(species, 1, initialBiomass[0], initialBiomass[1]);
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Determines if a species can establish on a site.
        /// This is a Delegate method to base succession.
        /// </summary>
        public bool Establish(ISpecies species, ActiveSite site)
        {
            //IEcoregion ecoregion = modelCore.Ecoregion[site];
            //double establishProbability = SpeciesData.EstablishProbability[species][ecoregion];
            double establishProbability = Establishment.Calculate(species, site);// SpeciesData.EstablishProbability[species][ecoregion];

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
            double establishProbability = Establishment.Calculate(species, site); //, Timestep); // SpeciesData.EstablishProbability[species][ecoregion];

            return establishProbability > 0.0;
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
        //---------------------------------------------------------------------
        // Outmoded but required?

        public static void SiteDisturbed(object sender,
                                         Landis.Library.BiomassCohorts.DisturbanceEventArgs eventArgs)
        {

            //ExtensionType disturbanceType = eventArgs.DisturbanceType;

            //if (disturbanceType.ToString() == "disturbance:fire")
            //    return;

            ////PoolPercentages poolReductions = Module.Parameters.PoolReductions[disturbanceType];

            //ActiveSite site = eventArgs.Site;
            //SiteVars.SurfaceDeadWood[site].ReduceMass(poolReductions.Wood);
            //SiteVars.SurfaceStructural[site].ReduceMass(poolReductions.Foliar);
            //SiteVars.SurfaceMetabolic[site].ReduceMass(poolReductions.Foliar);
        }

        //---------------------------------------------------------------------

        public static float ReduceInput(float poolInput,
                                          Percentage reductionPercentage,
                                          ActiveSite site)
        {
            float reduction = (poolInput * (float)reductionPercentage);

            SiteVars.SourceSink[site].Carbon += (double)reduction * 0.47;

            return (poolInput - reduction);
        }
    }

}
