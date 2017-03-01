//  Copyright 2007-2016 Portland State University
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

namespace Landis.Extension.Succession.NECN
{
    public class PlugIn
        : Landis.Library.Succession.ExtensionBase
    {
        public static readonly string ExtensionName = "NECN-H Succession";
        private static ICore modelCore;
        private IInputParameters parameters;
        public static double AtmosNslope;
        public static double AtmosNintercept;
        public static double Latitude;
        public static double DenitrificationRate;
        public static double DecayRateSurf;
        public static double DecayRateSOM1;
        public static double DecayRateSOM2;
        public static double DecayRateSOM3;
        public static List<int> SWHC_List = new List<int>(0);
        //public static int[] SWHC_Count;
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
            parameters = Landis.Data.Load<IInputParameters>(dataFile, parser);

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
            Timestep              = parameters.Timestep;
            SuccessionTimeStep    = Timestep;
            sufficientLight       = parameters.LightClassProbabilities;
            ProbEstablishAdjust = parameters.ProbEstablishAdjustment;
            MetadataHandler.InitializeMetadata(Timestep, modelCore, SoilCarbonMapNames, SoilNitrogenMapNames, ANPPMapNames, ANEEMapNames, TotalCMapNames);
            //CohortBiomass.SpinupMortalityFraction = parameters.SpinupMortalityFraction;
            
            //Initialize climate.
            Climate.Initialize(parameters.ClimateConfigFile, false, modelCore);
            FutureClimateBaseYear = Climate.Future_MonthlyData.Keys.Min();
            
            ClimateRegionData.Initialize(parameters);
            SpeciesData.Initialize(parameters);
            Util.ReadSoilDepthMap(parameters.SoilDepthMapName);
            Util.ReadSoilDrainMap(parameters.SoilDrainMapName);
            Util.ReadSoilBaseFlowMap(parameters.SoilBaseFlowMapName);
            Util.ReadSoilStormFlowMap(parameters.SoilStormFlowMapName);
            Util.ReadFieldCapacityMap(parameters.SoilFieldCapacityMapName);
            Util.ReadWiltingPointMap(parameters.SoilWiltingPointMapName);
            Util.ReadPercentSandMap(parameters.SoilPercentSandMapName);
            Util.ReadPercentClayMap(parameters.SoilPercentClayMapName);
            Util.ReadSoilCNMaps(parameters.InitialSOM1CSurfaceMapName, 
                parameters.InitialSOM1NSurfaceMapName,
                parameters.InitialSOM1CSoilMapName,
                parameters.InitialSOM1NSoilMapName,
                parameters.InitialSOM2CMapName,
                parameters.InitialSOM2NMapName,
                parameters.InitialSOM3CMapName,
                parameters.InitialSOM3NMapName);
            Util.ReadDeadWoodMaps(parameters.InitialDeadSurfaceMapName, parameters.InitialDeadSoilMapName);

            AtmosNslope = parameters.AtmosNslope;
            AtmosNintercept = parameters.AtmosNintercept;
            Latitude = parameters.Latitude;
            DenitrificationRate = parameters.Denitrif;
            DecayRateSurf = parameters.DecayRateSurf;
            DecayRateSOM1 = parameters.DecayRateSOM1;
            DecayRateSOM2 = parameters.DecayRateSOM2;
            DecayRateSOM3 = parameters.DecayRateSOM3;

            ShadeLAI = parameters.MaximumShadeLAI; //.MinRelativeBiomass;
            OtherData.Initialize(parameters);
            FunctionalType.Initialize(parameters);
            FireEffects.Initialize(parameters);

            //  Cohorts must be created before the base class is initialized
            //  because the base class' reproduction module uses the core's
            //  SuccessionCohorts property in its Initialization method.
            Library.LeafBiomassCohorts.Cohorts.Initialize(Timestep, new CohortBiomass());

            // Initialize Reproduction routines:
            Reproduction.SufficientResources = SufficientLight;
            Reproduction.Establish = Establish;
            Reproduction.AddNewCohort = AddNewCohort;
            Reproduction.MaturePresent = MaturePresent;
            base.Initialize(modelCore, parameters.SeedAlgorithm);
            Landis.Library.LeafBiomassCohorts.Cohort.PartialDeathEvent += CohortPartialMortality;
            Landis.Library.BiomassCohorts.Cohort.DeathEvent += CohortDied;
            AgeOnlyDisturbances.Module.Initialize(parameters.AgeOnlyDisturbanceParms);

            //InitialBiomass.Initialize(Timestep);
            InitializeSites(parameters.InitialCommunities, parameters.InitialCommunitiesMap, modelCore); 
            
            if (parameters.CalibrateMode)
                Outputs.CreateCalibrateLogFile();
            Outputs.WritePrimaryLogFile(0);
            Outputs.WriteShortPrimaryLogFile(0);
            Establishment.InitializeLogFile();

            B_MAX = 0;
            foreach(ISpecies species in ModelCore.Species)
            {
                if (SpeciesData.Max_Biomass[species] > B_MAX)
                    B_MAX = SpeciesData.Max_Biomass[species];
            }

            
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

            //double B_MAX = (double) ClimateRegionData.B_MAX[ecoregion];

            //double oldBiomass = (double) Library.LeafBiomassCohorts.Cohorts.ComputeNonYoungBiomass(SiteVars.Cohorts[site]);

            //int lastMortality = SiteVars.PrevYearMortality[site];
            //double B_ACT = Math.Min(B_MAX - lastMortality, oldBiomass);

            //  Relative living biomass (ratio of actual to maximum site
            //  biomass).
            //double B_AM = B_ACT / B_MAX;

            for (byte shade = 5; shade >= 1; shade--)
            {
                if(PlugIn.ShadeLAI[shade] <=0 ) //ClimateRegionData.ShadeBiomass[shade][ecoregion] <= 0)
                {
                    string mesg = string.Format("Maximum LAI has not been defined for shade class {0}", shade);
                    throw new System.ApplicationException(mesg);
                }
                //PlugIn.ModelCore.UI.WriteLine("Shade Calculation:  lastMort={0:0.0}, B_MAX={1}, oldB={2}, B_ACT={3}, shade={4}.", lastMortality, B_MAX,oldBiomass,B_ACT,shade);
                if (SiteVars.LAI[site] >= PlugIn.ShadeLAI[shade])
                    //B_AM >= ClimateRegionData.ShadeBiomass[shade][ecoregion])
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
            int swhc = (int)((SiteVars.SoilFieldCapacity[site] - SiteVars.SoilWiltingPoint[site]) * SiteVars.SoilDepth[site]);
            if (!PlugIn.SWHC_List.Contains(swhc))
                PlugIn.SWHC_List.Add(swhc);
            //PlugIn.ModelCore.UI.WriteLine("SWHC = {0}", swhc);

            InitialBiomass initialBiomass = InitialBiomass.Compute(site, initialCommunity);
            SiteVars.MineralN[site] = parameters.InitialMineralN;
        }


        //---------------------------------------------------------------------

        public void CohortPartialMortality(object sender, Landis.Library.BiomassCohorts.PartialDeathEventArgs eventArgs)
        {
            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;
            double reduction = eventArgs.Reduction;

            ICohort cohort = (Landis.Library.LeafBiomassCohorts.ICohort)eventArgs.Cohort;

            float fractionPartialMortality = (float)eventArgs.Reduction;
            //PlugIn.ModelCore.UI.WriteLine("Cohort experienced partial mortality: species={0}, age={1}, wood_biomass={2}, fraction_mortality={3:0.0}.", cohort.Species.Name, cohort.Age, cohort.WoodBiomass, fractionPartialMortality);

            AgeOnlyDisturbances.PoolPercentages cohortReductions = AgeOnlyDisturbances.Module.Parameters.CohortReductions[disturbanceType];

            float foliar = cohort.LeafBiomass * fractionPartialMortality;
            float wood = cohort.WoodBiomass * fractionPartialMortality;

            float foliarInput = AgeOnlyDisturbances.Events.ReduceInput(foliar, cohortReductions.Foliar, site);
            float woodInput = AgeOnlyDisturbances.Events.ReduceInput(wood, cohortReductions.Wood, site);

            ForestFloor.AddWoodLitter(woodInput, cohort.Species, site);
            ForestFloor.AddFoliageLitter(foliarInput, cohort.Species, site);

            Roots.AddCoarseRootLitter(woodInput, cohort, cohort.Species, site);  // All of cohorts roots are killed.
            Roots.AddFineRootLitter(foliarInput, cohort, cohort.Species, site);

            //PlugIn.ModelCore.UI.WriteLine("EVENT: Cohort Partial Mortality: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, disturbanceType);
            //PlugIn.ModelCore.UI.WriteLine("       Cohort Reductions:  Foliar={0:0.00}.  Wood={1:0.00}.", cohortReductions.Foliar, cohortReductions.Wood);
            //PlugIn.ModelCore.UI.WriteLine("       InputB/TotalB:  Foliar={0:0.00}/{1:0.00}, Wood={2:0.0}/{3:0.0}.", foliarInput, foliar, woodInput, wood);

            return;
        }
        //---------------------------------------------------------------------

        public void CohortDied(object         sender,
                               Landis.Library.BiomassCohorts.DeathEventArgs eventArgs)
        {

            //PlugIn.ModelCore.UI.WriteLine("Cohort Died! :-(");

            ExtensionType disturbanceType = eventArgs.DisturbanceType;
            ActiveSite site = eventArgs.Site;

            ICohort cohort = (Landis.Library.LeafBiomassCohorts.ICohort) eventArgs.Cohort;
            double foliar = (double) cohort.LeafBiomass;

            double wood = (double) cohort.WoodBiomass;

            //PlugIn.ModelCore.UI.WriteLine("Cohort Died: species={0}, age={1}, biomass={2}, foliage={3}.", cohort.Species.Name, cohort.Age, cohort.Biomass, foliar);

            if (disturbanceType == null) {
                //PlugIn.ModelCore.UI.WriteLine("NO EVENT: Cohort Died: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, eventArgs.DisturbanceType);

                ForestFloor.AddWoodLitter(wood, cohort.Species, eventArgs.Site);
                ForestFloor.AddFoliageLitter(foliar, cohort.Species, eventArgs.Site);

                Roots.AddCoarseRootLitter(wood, cohort, cohort.Species, eventArgs.Site);
                Roots.AddFineRootLitter(foliar, cohort,  cohort.Species, eventArgs.Site);
            }

            if (disturbanceType != null) {
                //PlugIn.ModelCore.UI.WriteLine("DISTURBANCE EVENT: Cohort Died: species={0}, age={1}, disturbance={2}.", cohort.Species.Name, cohort.Age, eventArgs.DisturbanceType);

                Disturbed[site] = true;
                if (disturbanceType.IsMemberOf("disturbance:fire"))
                    Landis.Library.Succession.Reproduction.CheckForPostFireRegen(eventArgs.Cohort, site);
                else
                    Landis.Library.Succession.Reproduction.CheckForResprouting(eventArgs.Cohort, site);
            }
        }

        //---------------------------------------------------------------------
        //Grows the cohorts for future climate
        protected override void AgeCohorts(ActiveSite site,
                                           ushort     years,
                                           int?       successionTimestep)
        {
            Century.Run(site, years, successionTimestep.HasValue);

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
        /// Add a new cohort to a site.
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

    }

}
