//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Parameters;
using System.Collections.Generic;
using System.Diagnostics;

namespace Landis.Extension.Succession.NECN
{
    public class InputParameters : IInputParameters
    {
        private int timestep;
        private string initCommunities;
        private string communitiesMap;
        private string soilDepthMapName;
        private string soilDrainMapName;
        private string soilBaseFlowMapName;
        private string soilStormFlowMapName;
        private string soilFieldCapacityMapName;
        private string soilWiltingPointMapName;
        private string soilPercentSandMapName;
        private string soilPercentClayMapName;
        private string initialSOM1CSurfaceMapName;
        private string initialSOM1NSurfaceMapName;
        private string initialSOM1CSoilMapName;
        private string initialSOM1NSoilMapName;
        private string initialSOM2CMapName;
        private string initialSOM2NMapName;
        private string initialSOM3CMapName;
        private string initialSOM3NMapName;
        private string initialDeadSurfaceMapName;
        private string initialDeadSoilMapName;

        //Drought normals -- optional variables for drought mortality
        private string normalSWAMapName;
        private string normalCWDMapName;
        private string normalTempMapName;

        //Maps to adjust PET based on topography
        private string slopeMapName;
        private string aspectMapName;
<<<<<<< HEAD
=======

        private bool calibrateMode;
        private bool smokeModelOutputs;
        //private bool henne_watermode;
        private bool writeSWA; //write soil water maps, for calculating normal SWA
        private bool writeCWD; //write climatic water deficit maps, for calculating normal CWD
        private bool writeTemp; //write temperature maps, for calculating normal CWD
        private bool writeSpeciesDroughtMaps; //write a map of drought mortality for each species
        private bool writeMeanSoilWaterMap; //write a map of MeanSoilWater each year
        private bool writePETMap; //write a map of PET each year
        private WaterType wtype;       
        private string communityInputMapNames;
>>>>>>> master
        private double probEstablishAdjust;
        private ISpeciesDataset speciesDataset;
<<<<<<< HEAD
=======
        
        private FunctionalTypeTable functionalTypes;
        private FireReductions[] fireReductionsTable;
        private List<HarvestReductions> harvestReductionsTable;
        
        private Landis.Library.Parameters.Species.AuxParm<int> sppFunctionalType;
        private Landis.Library.Parameters.Species.AuxParm<bool> nFixer;
        private Landis.Library.Parameters.Species.AuxParm<int> gddMin;
        private Landis.Library.Parameters.Species.AuxParm<int> gddMax;
        private Landis.Library.Parameters.Species.AuxParm<int> minJanTemp;
        private Landis.Library.Parameters.Species.AuxParm<double> maxDrought;
        private Landis.Library.Parameters.Species.AuxParm<double> leafLongevity;
        private Landis.Library.Parameters.Species.AuxParm<bool> epicormic;
        private Landis.Library.Parameters.Species.AuxParm<double> leafLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> woodLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> coarseRootLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> fineRootLignin;
        private Landis.Library.Parameters.Species.AuxParm<double> leafCN;
        private Landis.Library.Parameters.Species.AuxParm<double> woodCN;
        private Landis.Library.Parameters.Species.AuxParm<double> coarseRootCN;
        private Landis.Library.Parameters.Species.AuxParm<double> foliageLitterCN;
        private Landis.Library.Parameters.Species.AuxParm<double> fineRootCN;
        private Landis.Library.Parameters.Species.AuxParm<int> maxANPP;
        private Landis.Library.Parameters.Species.AuxParm<int> maxBiomass;
        private Landis.Library.Parameters.Species.AuxParm<bool> grass;  // optional
        private Landis.Library.Parameters.Species.AuxParm<double> growthLAI; // optional
        private Landis.Library.Parameters.Species.AuxParm<bool> nlog_depend;
        private double grassThresholdMultiplier; // W.Hotta 2020.07.07
        private Landis.Library.Parameters.Species.AuxParm<double> lightLAIShape; 
        private Landis.Library.Parameters.Species.AuxParm<double> lightLAIScale;
        private Landis.Library.Parameters.Species.AuxParm<double> lightLAILocation;
        private Landis.Library.Parameters.Species.AuxParm<double> lightLAIAdjust; //optional

        //private List<ISufficientLight> sufficientLight;
>>>>>>> master

        //Drought variables
        private Landis.Library.Parameters.Species.AuxParm<double> intercept; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaAge; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaBiomass; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaTempAnom; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaSWAAnom; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaCWDAnom; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaNormCWD; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaNormTemp; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> intxnCWD_Biomass; // optional

        private Landis.Library.Parameters.Species.AuxParm<int> lagTemp; // optional
        private Landis.Library.Parameters.Species.AuxParm<int> lagCWD; // optional
        private Landis.Library.Parameters.Species.AuxParm<int> lagSWA; // optional

        private Landis.Library.Parameters.Species.AuxParm<int> cwdThreshold; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> mortalityAboveThreshold; // optional
        private Landis.Library.Parameters.Species.AuxParm<int> cwdThreshold2; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> mortalityAboveThreshold2; // optional

        //CWD Establishment
        private Landis.Library.Parameters.Species.AuxParm<int> cwdBegin;
        private Landis.Library.Parameters.Species.AuxParm<int> cwdMax;

        //Previously Functional Group Parameters
        private Landis.Library.Parameters.Species.AuxParm<double> tempcurve1;
        private Landis.Library.Parameters.Species.AuxParm<double> tempcurve2;
        private Landis.Library.Parameters.Species.AuxParm<double> tempcurve3;
        private Landis.Library.Parameters.Species.AuxParm<double> tempcurve4;
        private Landis.Library.Parameters.Species.AuxParm<double> btoLAI;
        private Landis.Library.Parameters.Species.AuxParm<double> kLAI;
        private Landis.Library.Parameters.Species.AuxParm<double> moisturecurve1;
        private Landis.Library.Parameters.Species.AuxParm<double> moisturecurve2;
        private Landis.Library.Parameters.Species.AuxParm<double> moisturecurve3;
        private Landis.Library.Parameters.Species.AuxParm<double> moisturecurve4;
        private Landis.Library.Parameters.Species.AuxParm<double> minSoilDrain;
        private Landis.Library.Parameters.Species.AuxParm<double> monthlyWoodMortality;
        private Landis.Library.Parameters.Species.AuxParm<double> woodDecayRate;
        private Landis.Library.Parameters.Species.AuxParm<double> mortCurveShape;
        private Landis.Library.Parameters.Species.AuxParm<int> leafNeedleDrop;
        private Landis.Library.Parameters.Species.AuxParm<double> coarseRootFraction;
        private Landis.Library.Parameters.Species.AuxParm<double> fineRootFraction;
        private Landis.Library.Parameters.Species.AuxParm<double> minLAI;
        private Landis.Library.Parameters.Species.AuxParm<double> maxLAI;
        private Landis.Library.Parameters.Species.AuxParm<double> fractionANPPtoLeaf;



        //---------------------------------------------------------------------
        /// <summary>
        /// Timestep (years)
        /// </summary>
        public int Timestep
        {
            get {
                return timestep;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Timestep must be > or = 0");
                timestep = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Seeding algorithm
        /// </summary>
        public SeedingAlgorithms SeedAlgorithm { get; set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with the initial communities' definitions.
        /// </summary>
        public string InitialCommunities
        {
            get
            {
                return initCommunities;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                initCommunities = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the initial communities are.
        /// </summary>
        public string InitialCommunitiesMap
        {
            get
            {
                return communitiesMap;
            }

            set
            {
                if (value != null)
                {
                    ValidatePath(value);
                }
                communitiesMap = value;
            }
        }

        //---------------------------------------------------------------------
        public string ClimateConfigFile { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether months are simulated 0 - 12 (calibration mode) or
        /// 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 (normal mode with disturbance at June 30).
        /// </summary>
        public bool CalibrateMode { get; set; }

        public string CommunityInputMapNames { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should yearly rasters of mean summer SWA be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputSoilWaterAvailable { get; set; }


        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of CWD be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputClimateWaterDeficit { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of Temperature be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputTemp { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of drought-associated mortality be written for each species?
        /// </summary>
<<<<<<< HEAD
        public bool WriteSpeciesDroughtMaps { get; set; }
=======
        public bool WriteSpeciesDroughtMaps
        {
            get
            {
                return writeSpeciesDroughtMaps;
            }
            set
            {
                writeSpeciesDroughtMaps = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of mean soil water be written? Primarily intended for 
        /// calibrating the water balance model and diagnosing problems
        /// </summary>
        public bool WriteMeanSoilWaterMap
        {
            get
            {
                return writeMeanSoilWaterMap;
            }
            set
            {
                writeMeanSoilWaterMap = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of PET be written? 
        /// This is useful for looking at topographic and climate region differences
        /// in PET, or for calculating rasters of AET from PET and CWD
        /// </summary>
        public bool WritePETMap
        {
            get
            {
                return writePETMap;
            }
            set
            {
                writePETMap = value;
            }
        }
>>>>>>> master


        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        public bool SmokeModelOutputs { get; set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether moisture effects on decomposition follow a linear or ratio calculation.
        /// </summary>
        public WaterType WType { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Adjust probability of establishment due to variable time step.  A multiplier.
        /// </summary>
        public double ProbEstablishAdjustment
        {
            get
            {
                return probEstablishAdjust;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Probability of adjustment factor must be > 0.0 and < 1");
                probEstablishAdjust = value;
            }
        }
        //---------------------------------------------------------------------
        public double AtmosNslope { get; private set; }
        //---------------------------------------------------------------------
        public double AtmosNintercept { get; private set; }

        //---------------------------------------------------------------------
        /// <summary>
        /// Fire reduction of leaf and wood litter parameters.
        /// </summary>
        public FireReductions[] FireReductionsTable { get; set; }
        //---------------------------------------------------------------------
        /// <summary>
        /// Harvest reduction of leaf and wood litter parameters.
        /// </summary>
        public List<HarvestReductions> HarvestReductionsTable { get; set; }

        //---------------------------------------------------------------------

        //public Landis.Library.Parameters.Species.AuxParm<int>     SppFunctionalType {get {return sppFunctionalType;}}
        public Landis.Library.Parameters.Species.AuxParm<bool> NFixer { get; }
        public Landis.Library.Parameters.Species.AuxParm<bool> Grass { get; }
        public Landis.Library.Parameters.Species.AuxParm<bool> Nlog_depend { get; }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmin { get; }
        public Landis.Library.Parameters.Species.AuxParm<int> GDDmax { get; }
        public Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp { get; }
        public Landis.Library.Parameters.Species.AuxParm<double> MaxDrought { get; }
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity { get; }
        public Landis.Library.Parameters.Species.AuxParm<double> GrowthLAI { get; }
        public double GrassThresholdMultiplier { get; private set; }

        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get { return lightLAIShape; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get { return lightLAIScale; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get { return lightLAILocation; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust { get { return lightLAIAdjust; } }

        //Drought variables

        public Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold { get { return cwdThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold { get { return mortalityAboveThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold2 { get { return cwdThreshold2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold2 { get { return mortalityAboveThreshold2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Intercept { get { return intercept; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaAge { get { return betaAge; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaTempAnom { get { return betaTempAnom; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom { get { return betaSWAAnom; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass { get { return betaBiomass; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaCWDAnom { get { return betaCWDAnom; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD { get { return betaNormCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaNormTemp { get { return betaNormTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass { get { return intxnCWD_Biomass; } }

        public Landis.Library.Parameters.Species.AuxParm<int> LagTemp { get { return lagTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<int> LagCWD { get { return lagCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<int> LagSWA { get { return lagSWA; } }

        //CWD Establishment
        public Landis.Library.Parameters.Species.AuxParm<int> CWDBegin { get { return cwdBegin; } }
        public Landis.Library.Parameters.Species.AuxParm<int> CWDMax { get { return cwdMax; } }

        public Landis.Library.Parameters.Species.AuxParm<double> Tempcurve1 { get { return tempcurve1; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Tempcurve2 { get { return tempcurve2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Tempcurve3 { get { return tempcurve3; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Tempcurve4 { get { return tempcurve4; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BiomassToLAI { get { return btoLAI; } }
        public Landis.Library.Parameters.Species.AuxParm<double> K_LAI { get { return kLAI; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MinLAI { get { return minLAI; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MaxLAI { get { return maxLAI; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve1 { get { return moisturecurve1; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve2 { get { return moisturecurve2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve3 { get { return moisturecurve3; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve4 { get { return moisturecurve4; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MinSoilDrain { get { return minSoilDrain; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MonthlyWoodMortality { get { return monthlyWoodMortality; } }
        public Landis.Library.Parameters.Species.AuxParm<double> WoodDecayRate { get { return woodDecayRate; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityCurveShape { get { return mortCurveShape; } }
        public Landis.Library.Parameters.Species.AuxParm<int> LeafNeedleDrop { get { return leafNeedleDrop; } }
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootFraction { get { return coarseRootFraction; } }
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootFraction { get { return fineRootFraction; } }
        public Landis.Library.Parameters.Species.AuxParm<double> FractionANPPtoLeaf { get { return fractionANPPtoLeaf; } }


        //---------------------------------------------------------------------
        /// <summary>
        /// Can the species resprout epicormically following a fire?
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool> Epicormic { get; set; }

        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLignin { get; }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> WoodLignin { get; }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin { get; }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> LeafCN { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> WoodCN { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FineRootCN { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxANPP { get; }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass { get; }
        //---------------------------------------------------------------------
        public double Latitude { get; private set; }
        //-----------------------------------------------
        public double DecayRateSurf { get; private set; }
        //-----------------------------------------------
        public double DecayRateSOM1 { get; private set; }
        //---------------------------------------------------------------------
        public double DecayRateSOM2 { get; private set; }
        //---------------------------------------------------------------------
        public double DecayRateSOM3 { get; private set; }
        //-----------------------------------------------
        public double DenitrificationRate { get; private set; }
        public double InitialMineralN { get; private set; }
        public double InitialFineFuels { get; private set; }


        //---------------------------------------------------------------------
        public string SoilDepthMapName
        {
            get
            {
                return soilDepthMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilDepthMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilDrainMapName
        {
            get
            {
                return soilDrainMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilDrainMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilBaseFlowMapName
        {
            get
            {
                return soilBaseFlowMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilBaseFlowMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilStormFlowMapName
        {
            get
            {
                return soilStormFlowMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilStormFlowMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilFieldCapacityMapName
        {
            get
            {
                return soilFieldCapacityMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilFieldCapacityMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilWiltingPointMapName
        {
            get
            {
                return soilWiltingPointMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilWiltingPointMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilPercentSandMapName
        {
            get
            {
                return soilPercentSandMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilPercentSandMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string SoilPercentClayMapName
        {
            get
            {
                return soilPercentClayMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                soilPercentClayMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialSOM1CSurfaceMapName
        {
            get
            {
                return initialSOM1CSurfaceMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM1CSurfaceMapName = value;
            }
        }

        //---------------------------------------------------------------------

        public string InitialSOM1NSurfaceMapName
        {
            get
            {
                return initialSOM1NSurfaceMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM1NSurfaceMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialSOM1CSoilMapName
        {
            get
            {
                return initialSOM1CSoilMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM1CSoilMapName = value;
            }
        }

        //---------------------------------------------------------------------

        public string InitialSOM1NSoilMapName
        {
            get
            {
                return initialSOM1NSoilMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM1NSoilMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialSOM2CMapName
        {
            get
            {
                return initialSOM2CMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM2CMapName = value;
            }
        }

        //---------------------------------------------------------------------

        public string InitialSOM2NMapName
        {
            get
            {
                return initialSOM2NMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM2NMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialSOM3CMapName
        {
            get
            {
                return initialSOM3CMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM3CMapName = value;
            }
        }

        //---------------------------------------------------------------------

        public string InitialSOM3NMapName
        {
            get
            {
                return initialSOM3NMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialSOM3NMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialDeadSurfaceMapName
        {
            get
            {
                return initialDeadSurfaceMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialDeadSurfaceMapName = value;
            }
        }
        //---------------------------------------------------------------------

        public string InitialDeadSoilMapName
        {
            get
            {
                return initialDeadSoilMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                initialDeadSoilMapName = value;
            }
        }
        //---------------------------------------------------------------------


        public string NormalSWAMapName
        {
            get
            {
                return normalSWAMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                normalSWAMapName = value;
            }
        }
        //---------------------------------------------------------------------


        public string NormalCWDMapName
        {
            get
            {
                return normalCWDMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                normalCWDMapName = value;
            }
        }

        //---------------------------------------------------------------------


        public string NormalTempMapName
        {
            get
            {
                return normalTempMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                normalTempMapName = value;
            }
        }
        //---------------------------------------------------------------------
        public string SlopeMapName
        {
            get
            {
                return slopeMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                slopeMapName = value;
            }
        }

        //---------------------------------------------------------------------
        public string AspectMapName
        {
            get
            {
                return aspectMapName;
            }
            set
            {
                string path = value;
                if (path.Trim(null).Length == 0)
                    throw new InputValueException(path, "\"{0}\" is not a valid path.", path);
                aspectMapName = value;
            }
        }

        //---------------------------------------------------------------------

        //public void SetMaximumShadeLAI(byte                   shadeClass,
        //                                  //IEcoregion             ecoregion,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(1 <= shadeClass && shadeClass <= 5);
        //    //Debug.Assert(ecoregion != null);
        //    if (newValue != null) {
        //        if (newValue.Actual < 0.0 || newValue.Actual > 20)
        //            throw new InputValueException(newValue.String,
        //                                          "{0} is not between 0 and 20", newValue.String);
        //    }
        //    maximumShadeLAI[shadeClass] = newValue;
        //    //minRelativeBiomass[shadeClass][ecoregion] = newValue;
        //}
        //---------------------------------------------------------------------

        //public void SetFunctionalType(ISpecies species, int newValue)
        //{
        //    Debug.Assert(species != null);
        //    sppFunctionalType[species] = VerifyRange(newValue, 0, 100, "FunctionalType");
        //}
        //---------------------------------------------------------------------

        //public void SetNFixer(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    nTolerance[species] = CheckBiomassParm(newValue, 1, 4);
        //}

        //---------------------------------------------------------------------

        //public void SetGDDmin(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    gddMin[species] = VerifyRange(newValue, 1, 4000);
        //}
        public void SetGDDmin(ISpecies species,int newValue)
        {
            Debug.Assert(species != null);
            GDDmin[species] = VerifyRange(newValue, 1, 4000, "GDDMin");
        }
        //---------------------------------------------------------------------

        //public void SetGDDmax(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    gddMax[species] = VerifyRange(newValue, 500, 7000);
        //}
        public void SetGDDmax(ISpecies species,int newValue)
        {
            Debug.Assert(species != null);
            GDDmax[species] = VerifyRange(newValue, 500, 7000, "GDDmax");
        }
        //---------------------------------------------------------------------

        //public void SetMinJanTemp(ISpecies           species,
        //                             InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    minJanTemp[species] = VerifyRange(newValue, -60, 20);
        //}
        public void SetMinJanTemp(ISpecies species,int newValue)
        {
            Debug.Assert(species != null);
            MinJanTemp[species] = VerifyRange(newValue, -60, 20, "MinJanTemp");
        }
        //---------------------------------------------------------------------

        //public void SetMaxDrought(ISpecies           species,
        //                             InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    maxDrought[species] = VerifyRange(newValue, 0.0, 1.0);
        //}
        public void SetMaxDrought(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            MaxDrought[species] = VerifyRange(newValue, 0.0, 1.0, "MaxDrought");
        }
        //---------------------------------------------------------------------
        //CWD Establishment
        public void SetCWDBeginLimit(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            cwdBegin[species] = VerifyRange(newValue, 0, 5000, "CWDBegin");
        }
         public void SetCWDMax(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            cwdMax[species] = VerifyRange(newValue, 0, 5000, "CWDMax");
        }

        //---------------------------------------------------------------------

        //public void SetLeafLongevity(ISpecies           species,
        //                             InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafLongevity[species] = VerifyRange(newValue, 1.0, 10.0, "Leaf Longevity");
        //}
        public void SetLeafLongevity(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            LeafLongevity[species] = VerifyRange(newValue, 1.0, 10.0, "LeafLongevity");
        }

        //---------------------------------------------------------------------

        //public void SetLeafLignin(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafLignin[species] = VerifyRange(newValue, 0.0, 0.4);
        //}
        public void SetLeafLignin(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            LeafLignin[species] = VerifyRange(newValue, 0.0, 0.4, "LeafLignin");
        }
        //---------------------------------------------------------------------

        //public void SetWoodLignin(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    woodLignin[species] = VerifyRange(newValue, 0.0, 0.4);
        //}
        public void SetWoodLignin(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            WoodLignin[species] = VerifyRange(newValue, 0.0, 0.4, "WoodLignin");
        }
        //---------------------------------------------------------------------

        //public void SetCoarseRootLignin(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    coarseRootLignin[species] = VerifyRange(newValue, 0.0, 0.4);
        //}
        public void SetCoarseRootLignin(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            CoarseRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "CourseRootLignin");
        }
        //---------------------------------------------------------------------

        //public void SetFineRootLignin(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    fineRootLignin[species] = VerifyRange(newValue, 0.0, 0.4);
        //}
        public void SetFineRootLignin(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            FineRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "FineRootLignin");
        }
        //---------------------------------------------------------------------

        //public void SetLeafCN(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    leafCN[species] = VerifyRange(newValue, 5.0, 100.0);
        //}
        public void SetLeafCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            LeafCN[species] = VerifyRange(newValue, 5.0, 100.0, "LeafCN");
        }
        //---------------------------------------------------------------------

        //public void SetWoodCN(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    woodCN[species] = VerifyRange(newValue, 5.0, 900.0);
        //}
        public void SetWoodCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            WoodCN[species] = VerifyRange(newValue, 5.0, 900.0, "WoodCN");
        }
        //---------------------------------------------------------------------

        public void SetCoarseRootCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            CoarseRootCN[species] = VerifyRange(newValue, 5.0, 500.0, "CourseRootCN");
        }
        //---------------------------------------------------------------------

        public void SetFoliageLitterCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            FoliageLitterCN[species] = VerifyRange(newValue, 5.0, 100.0, "FoliarLitterCN");
        }
        //---------------------------------------------------------------------

        public void SetFineRootCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            FineRootCN[species] = VerifyRange(newValue, 5.0, 100.0, "FineRootCN");
        }
        //---------------------------------------------------------------------
        public void SetMaxANPP(ISpecies species,int newValue)
        {
            Debug.Assert(species != null);
            MaxANPP[species] = VerifyRange(newValue, 2, 1000, "MaxANPP");
        }
        //---------------------------------------------------------------------

        public void SetMaxBiomass(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            MaxBiomass[species] = VerifyRange(newValue, 2, 300000, "MaxBiomass");
        }

        public void SetGrowthLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            GrowthLAI[species] = VerifyRange(newValue, 0.0, 1.0, "GrowthLAI");
        }

        public void SetLightLAIShape(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAIShape[species] = VerifyRange(newValue, 0.0, 10.0, "LightLAIShape");
        }

        public void SetLightLAIScale(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAIScale[species] = VerifyRange(newValue, 0.0, 1000.0, "LightLAIScale"); //Scale parameter can be high for some shade-tolerant spp
        }

        public void SetLightLAILocation(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            LightLAILocation[species] = VerifyRange(newValue, 0.0, 1, "LightLAILocation");
        }

        public void SetLightLAIAdjust(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            lightLAIAdjust[species] = VerifyRange(newValue, 0.0, 100.0, "LightLAIAdjust");
        }



        //---------------------------------------------------------------------

        public void SetAtmosNslope(InputValue<double> newValue)
        {
            AtmosNslope = VerifyRange(newValue, -1.0, 2.0, "AtmosNslope");
        }
        //---------------------------------------------------------------------
        public void SetAtmosNintercept(InputValue<double> newValue)
        {
            AtmosNintercept = VerifyRange(newValue, -1.0, 2.0, "AtmostNintercept");
        }
        //---------------------------------------------------------------------
        public void SetLatitude(InputValue<double> newValue)
        {
            Latitude = VerifyRange(newValue, 0.0, 70.0, "Latitude");
        }
        //---------------------------------------------------------------------
       
        public void SetDecayRateSurf(InputValue<double> newValue)
        {
            DecayRateSurf = VerifyRange(newValue, 0.0, 10.0, "DecayRateSurf");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM1(InputValue<double> newValue)
        {
            DecayRateSOM1 = VerifyRange(newValue, 0.0, 10.0, "DecayRateSOM1");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM2(InputValue<double> newValue)
        {
            DecayRateSOM2 = VerifyRange(newValue, 0.0, 1.0, "DecayRateSOM2");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM3(InputValue<double> newValue)
        {
            DecayRateSOM3 = VerifyRange(newValue, 0.0, 1.0, "DecayRateSOM3");
        }
        // --------------------------------------------------------------------
        // Multiplier to adjust judgement whether a tree-cohort is larger than grass layer
        // W.Hotta 2020.07.07
        public void SetGrassThresholdMultiplier(InputValue<double> newValue)
        {
            GrassThresholdMultiplier = VerifyRange(newValue, 0.0, 10.0, "GrassThresholdMultiplier");
        }
        //---------------------------------------------------------------------
        public void SetDenitrif(InputValue<double> newValue)
        {
            DenitrificationRate = VerifyRange(newValue, 0.0, 1.0, "Denitrification");
        }

        //---------------------------------------------------------------------
        public void SetInitMineralN(InputValue<double> newValue)
        {
            InitialMineralN = VerifyRange(newValue, 0.0, 5000.0, "InitialMineralN");
        }
        //---------------------------------------------------------------------
        public void SetInitFineFuels(InputValue<double> newValue)
        {
            InitialFineFuels = VerifyRange(newValue, 0.0, 1.0, "InitialFineFuel");
        }
        //---------------------------------------------------------------------

        public void SetNlog_depend(ISpecies species, bool newValue)
        {
            Debug.Assert(species != null);
            Nlog_depend[species] = newValue;
        }


        public void SetCWDThreshold(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            cwdThreshold[species] = VerifyRange(newValue, 0, 100000, "CWDThreshold");
        }

        public void SetMortalityAboveThreshold(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            mortalityAboveThreshold[species] = VerifyRange(newValue, 0, 1, "MortalityAboveGThreshold");
        }

        public void SetCWDThreshold2(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            cwdThreshold2[species] = VerifyRange(newValue, 0, 100000, "CWDThreshold2");
        }

        public void SetMortalityAboveThreshold2(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            mortalityAboveThreshold2[species] = VerifyRange(newValue, 0, 1, "MortalityAboveThreshold2");
        }

        public void SetIntercept(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            intercept[species] = VerifyRange(newValue, -10, 10, "DroughtIntercept");

        }
        public void SetBetaAge(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaAge[species] = VerifyRange(newValue, -10, 10, "DroughtBetaAge");
        }
        public void SetBetaTempAnom(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaTempAnom[species] = VerifyRange(newValue, -10, 10, "DroughtBetaTempAnom");
        }
        public void SetBetaSWAAnom(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaSWAAnom[species] = VerifyRange(newValue, -10, 10, "DroughtBetaSWAAnom");
        }
        public void SetBetaBiomass(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaBiomass[species] = VerifyRange(newValue, -10, 10, "DroughtBetaBiomass");
        }

        public void SetBetaCWDAnom(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaCWDAnom[species] = VerifyRange(newValue, -10, 10, "DroughtBetaCWDAnom");
        }

        public void SetBetaNormCWD(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaNormCWD[species] = VerifyRange(newValue, -10, 10, "DroughtBetaNormCWD");
        }
        public void SetBetaNormTemp(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaNormTemp[species] = VerifyRange(newValue, -10, 10, "DroughtBetaNormTemp");
        }

        public void SetIntxnCWD_Biomass(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            intxnCWD_Biomass[species] = VerifyRange(newValue, -10, 10, "DroughtBetaIntxn");
        }

        public void SetLagTemp(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            lagTemp[species] = VerifyRange(newValue,  0, 10, "DroughtLagTemp");
        }
        public void SetLagCWD(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            lagCWD[species] = VerifyRange(newValue, 0, 10, "DroughtLagCWD");
        }
        public void SetLagSWA(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            lagSWA[species] = VerifyRange(newValue, 0, 10, "DroughtLagSWA");
        }
        public void SetTempCurve1(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            tempcurve1[species] = VerifyRange(newValue, 10.0, 40.0, "Tempcurve1");
        }
        public void SetTempCurve2(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            tempcurve2[species] = VerifyRange(newValue, 20.0, 100.0, "Tempcurve2");
        }
        
        public void SetTempCurve3(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            tempcurve3[species] = newValue;
        }
        public void SetTempCurve4(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            tempcurve4[species] = newValue;
        }
        public void SetFractionANPPtoLeaf(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            fractionANPPtoLeaf[species] = VerifyRange(newValue, 0.0, 1.0, "FractionANPPtoLeaf");
        }
        public void SetBiomassToLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            btoLAI[species] = VerifyRange(newValue, -3.0, 1000.0, "BiomassToLAI");
        }
        public void SetKLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            kLAI[species] = VerifyRange(newValue, 1.0, 50000.0, "K_LAI");
        }
        public void SetMaxLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            maxLAI[species] = VerifyRange(newValue, 0.0, 20.0, "MaxLAI");
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Suppression is sensitive to the minimum LAI.  Below 0.3 and a cohort can be permanently suppressed without any growth.
        /// </summary>
        public void SetMinLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            minLAI[species] = VerifyRange(newValue, 0.0, 5.0, "MinLAI");
        }

        public void SetMoistureCurve1(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            moisturecurve1[species] = newValue;
        }
        public void SetMoistureCurve2(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            moisturecurve2[species] = newValue;
        }
        public void SetMoistureCurve3(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            moisturecurve3[species] = newValue;
        }
        // 
        public void SetMoistureCurve4(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            moisturecurve4[species] = newValue;
        }
        public void SetMinSoilDrain(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            minSoilDrain[species] = newValue;
        }

        //---------------------------------------------------------------------
        public void SetMonthlyWoodMortality(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            monthlyWoodMortality[species] = VerifyRange(newValue, 0.0, 1.0, "MonthlyWoodMortality");
        }

        //---------------------------------------------------------------------
        public void SetWoodDecayRate(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            woodDecayRate[species] = VerifyRange(newValue, 0.0, 2.0, "WoodDecayRate");
        }

        public void SetMortalityShapeCurve(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            mortCurveShape[species] = VerifyRange(newValue, 2.0, 25.0, "MortCurveShape");
        }
        public void SetFoliageDropMonth(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            leafNeedleDrop[species] = VerifyRange(newValue, 1, 12, "FoliageDropMonth");
        }
        public void SetCoarseRootFraction(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            coarseRootFraction[species] = VerifyRange(newValue, 0.0, 1.0, "CoarseRootFraction");
        }
        public void SetFineRootFraction(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            fineRootFraction[species] = VerifyRange(newValue, 0.0, 1.0, "FineRootFraction");
        }


        public InputParameters(ISpeciesDataset speciesDataset, int litterCnt, int functionalCnt)
        {
            this.speciesDataset = speciesDataset;

            //functionalTypes = new FunctionalTypeTable(functionalCnt);
            FireReductionsTable = new FireReductions[11];
            HarvestReductionsTable = new List<HarvestReductions>();

            //sppFunctionalType       = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            NFixer                  = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            Grass                   = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            Nlog_depend             = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            GDDmin                  = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            GDDmax                  = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MinJanTemp              = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MaxDrought              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LeafLongevity           = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            Epicormic               = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            LeafLignin              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            WoodLignin              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CoarseRootLignin        = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FineRootLignin          = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            LeafCN                  = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            WoodCN                  = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            CoarseRootCN            = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FoliageLitterCN         = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            FineRootCN              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            MaxANPP                 = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            MaxBiomass              = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            GrowthLAI               = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            //CWD Establishment
            cwdBegin = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            cwdMax = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            //Drought variables
            cwdThreshold = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            mortalityAboveThreshold = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            cwdThreshold2 = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            mortalityAboveThreshold2 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            intercept = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaAge = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaTempAnom = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaSWAAnom = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaBiomass = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaCWDAnom = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaNormCWD = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaNormTemp = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            intxnCWD_Biomass = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lagTemp = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            lagCWD = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            lagSWA = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);

            tempcurve1 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            tempcurve2 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            tempcurve3 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            tempcurve4 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            btoLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            kLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            minLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            maxLAI = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            moisturecurve1 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            moisturecurve2 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            moisturecurve3 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            moisturecurve4 = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            minSoilDrain = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            monthlyWoodMortality = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            woodDecayRate = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            mortCurveShape = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            leafNeedleDrop = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            coarseRootFraction = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fineRootFraction = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fractionANPPtoLeaf = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);

            lightLAIShape = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lightLAIScale = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lightLAILocation = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lightLAIAdjust = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);


        }

        //---------------------------------------------------------------------

        public static double VerifyRange(InputValue<double> newValue, double minValue, double maxValue, string parameterName)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} {1} is not between {2:0.0} and {3:0.0}",
                                                  parameterName, newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        public static double VerifyRange(double newValue, double minValue, double maxValue, string parameterName)
        {
                if (newValue < minValue || newValue > maxValue)
                    throw new InputValueException(newValue.ToString(),
                                                  "{0} {1} is not between {2:0.0} and {3:0.0}",
                                                  parameterName, newValue.ToString(), minValue, maxValue);
            return newValue;
        }
        //---------------------------------------------------------------------

        public static int VerifyRange(InputValue<int> newValue, int minValue, int maxValue, string parameterName)
        {
            if (newValue != null) {
                if (newValue.Actual < minValue || newValue.Actual > maxValue)
                    throw new InputValueException(newValue.String,
                                                  "{0} {1} is not between {2:0.0} and {3:0.0}",
                                                  parameterName, newValue.String, minValue, maxValue);
            }
            return newValue.Actual;
        }
        public static int VerifyRange(int newValue, int minValue, int maxValue, string parameterName)
        {
                if (newValue < minValue || newValue > maxValue)
                    throw new InputValueException(newValue.ToString(),
                                                  "{0} {1} is not between {2:0.0} and {3:0.0}",
                                                  parameterName, newValue.ToString(), minValue, maxValue);
            return newValue;
        }

        //---------------------------------------------------------------------

        private void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new InputValueException();
            if (path.Trim(null).Length == 0)
                throw new InputValueException(path,
                                              "\"{0}\" is not a valid path.",
                                              path);
        }

    }
}
