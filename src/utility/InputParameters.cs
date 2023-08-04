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
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public class InputParameters : IInputParameters
    {
        private int timestep;
        private SeedingAlgorithms seedAlg;

        private string climateConfigFile;
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

        private bool calibrateMode;
        private bool smokeModelOutputs;
        //private bool henne_watermode;
        private bool writeSWA; //write soil water maps, for calculating normal SWA
        private bool writeCWD; //write climatic water deficit maps, for calculating normal CWD
        private bool writeTemp; //write temperature maps, for calculating normal CWD
        private bool writeSpeciesDroughtMaps; //write a map of drought mortality for each species
        private WaterType wtype;       
        private string communityInputMapNames;
        private double probEstablishAdjust;
        private double atmosNslope;
        private double atmosNintercept;
        private double latitude;
        private double denitrif;
        private double decayRateSurf;
        private double decayRateSOM1;
        private double decayRateSOM2;
        private double decayRateSOM3;
        //private double[] maximumShadeLAI;
        private double initMineralN;
        private double initFineFuels;

        private ISpeciesDataset speciesDataset;
        
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

        //private List<ISufficientLight> sufficientLight;

        //Drought variables
        private Landis.Library.Parameters.Species.AuxParm<double> intercept; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaAge; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaBiomass; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaTemp; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaSWAAnom; // optional
        private Landis.Library.Parameters.Species.AuxParm<double> betaCWD; // optional
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
        public SeedingAlgorithms SeedAlgorithm
        {
            get {
                return seedAlg;
            }
            set {
                seedAlg = value;
            }
        }

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
        public string ClimateConfigFile
        {
            get
            {
                return climateConfigFile;
            }
            set
            {

                climateConfigFile = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether months are simulated 0 - 12 (calibration mode) or
        /// 6, 7, 8, 9, 10, 11, 0, 1, 2, 3, 4, 5 (normal mode with disturbance at June 30).
        /// </summary>
        public bool CalibrateMode
        {
            get {
                return calibrateMode;
            }
            set {
                calibrateMode = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// </summary>
        /*public bool SoilWater_Henne
        {
            get
            {
                return henne_watermode;
            }
            set
            {
                henne_watermode = value;
            }
        }
*/
        public string CommunityInputMapNames
        {
            get
            {
                return communityInputMapNames;
            }
            set
            {
                communityInputMapNames = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should yearly rasters of mean summer SWA be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputSoilWaterAvailable
        {
            get
            {
                return writeSWA;
            }
            set
            {
                writeSWA = value;
            }
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of CWD be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputClimateWaterDeficit
        {
            get
            {
                return writeCWD;
            }
            set
            {
                writeCWD = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of Temperature be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
        public bool OutputTemp
        {
            get
            {
                return writeTemp;
            }
            set
            {
                writeTemp = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Should annual rasters of CWD be written? Used to generate input
        /// variables for drought mortality
        /// </summary>
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
        /// </summary>
        public bool SmokeModelOutputs
        {
            get
            {
                return smokeModelOutputs;
            }
            set
            {
                smokeModelOutputs = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines whether moisture effects on decomposition follow a linear or ratio calculation.
        /// </summary>
        public WaterType WType
        {
            get {
                return wtype;
            }
            set {
                wtype = value;
            }
        }
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
        public double AtmosNslope
        {
            get
            {
                return atmosNslope;
            }
        }
        //---------------------------------------------------------------------
        public double AtmosNintercept
        {
            get
            {
                return atmosNintercept;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Functional type parameters.
        /// </summary>
        public FunctionalTypeTable FunctionalTypes
        {
            get {
                return functionalTypes;
            }
            set {
                functionalTypes = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Fire reduction of leaf and wood litter parameters.
        /// </summary>
        public FireReductions[] FireReductionsTable
        {
            get {
                return fireReductionsTable;
            }
            set {
                fireReductionsTable = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Harvest reduction of leaf and wood litter parameters.
        /// </summary>
        public List<HarvestReductions> HarvestReductionsTable
        {
            get
            {
                return harvestReductionsTable;
            }
            set
            {
                harvestReductionsTable = value;
            }
        }
        //---------------------------------------------------------------------
        //public double[] MaximumShadeLAI
        //{
        //    get
        //    {
        //        return maximumShadeLAI;
        //    }
        //}

 
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int>     SppFunctionalType {get {return sppFunctionalType;}}
        public Landis.Library.Parameters.Species.AuxParm<bool>     NFixer { get {return nFixer;}}
        public Landis.Library.Parameters.Species.AuxParm<bool> Grass { get { return grass; } }
        public Landis.Library.Parameters.Species.AuxParm<bool> Nlog_depend { get { return nlog_depend; } }
        public Landis.Library.Parameters.Species.AuxParm<int>     GDDmin     { get { return gddMin; }}
        public Landis.Library.Parameters.Species.AuxParm<int>     GDDmax     { get { return gddMax; }}
        public Landis.Library.Parameters.Species.AuxParm<int>     MinJanTemp { get { return minJanTemp; }}
        public Landis.Library.Parameters.Species.AuxParm<double>  MaxDrought { get { return maxDrought; }}
        public Landis.Library.Parameters.Species.AuxParm<double>  LeafLongevity {get {return leafLongevity;}}
        public Landis.Library.Parameters.Species.AuxParm<double> GrowthLAI { get { return growthLAI; } }
        public double GrassThresholdMultiplier { get { return grassThresholdMultiplier; } }

        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get { return lightLAIShape; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get { return lightLAIScale; } }
        public Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get { return lightLAILocation; } }
        //Drought variables

        public Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold { get { return cwdThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold { get { return mortalityAboveThreshold; } }
        public Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold2 { get { return cwdThreshold2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold2 { get { return mortalityAboveThreshold2; } }
        public Landis.Library.Parameters.Species.AuxParm<double> Intercept { get { return intercept; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaAge { get { return betaAge; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaTemp { get { return betaTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom { get { return betaSWAAnom; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass { get { return betaBiomass; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaCWD { get { return betaCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD { get { return betaNormCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<double> BetaNormTemp { get { return betaNormTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass { get { return intxnCWD_Biomass; } }

        public Landis.Library.Parameters.Species.AuxParm<int> LagTemp { get { return lagTemp; } }
        public Landis.Library.Parameters.Species.AuxParm<int> LagCWD { get { return lagCWD; } }
        public Landis.Library.Parameters.Species.AuxParm<int> LagSWA { get { return lagSWA; } }

        //CWD Establishment
        public Landis.Library.Parameters.Species.AuxParm<int> CWDBegin { get { return cwdBegin; } }
        public Landis.Library.Parameters.Species.AuxParm<int> CWDMax { get { return cwdMax; } }
        //---------------------------------------------------------------------
        /// <summary>
        /// Can the species resprout epicormically following a fire?
        /// </summary>
        public Landis.Library.Parameters.Species.AuxParm<bool>    Epicormic 
        {
            get {
                return epicormic;
            }
            set {
                epicormic = value;
            }
        }

        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> LeafLignin
        {
            get {
                return leafLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> WoodLignin
        {
            get {
                return woodLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin
        {
            get {
                return coarseRootLignin;
            }
        }
        //---------------------------------------------------------------------
        public Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin
        {
            get {
                return fineRootLignin;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> LeafCN
        {
            get {
                return leafCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> WoodCN
        {
            get {
                return woodCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN
        {
            get {
                return coarseRootCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN
        {
            get {
                return foliageLitterCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<double> FineRootCN
        {
            get {
                return fineRootCN;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxANPP
        {
            get
            {
                return maxANPP;
            }
        }
        //---------------------------------------------------------------------

        public Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass
        {
            get
            {
                return maxBiomass;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Definitions of sufficient light probabilities.
        ///// </summary>
        //public List<ISufficientLight> LightClassProbabilities
        //{
        //    get {
        //        return sufficientLight;
        //    }
        //    set 
        //    {
        //        Debug.Assert(sufficientLight.Count != 0);
        //        sufficientLight = value;
        //    }
        //}
        //---------------------------------------------------------------------
        public double Latitude
        {
            get {
                return latitude;
            }
        }
        //-----------------------------------------------
        public double DecayRateSurf
        {
            get
            {
                return decayRateSurf;
            }
        }
        //-----------------------------------------------
        public double DecayRateSOM1
        {
            get
            {
                return decayRateSOM1;
            }
        }//---------------------------------------------------------------------
        public double DecayRateSOM2
        {
            get
            {
                return decayRateSOM2;
            }
        }
        //---------------------------------------------------------------------
        public double DecayRateSOM3
        {
            get
            {
                return decayRateSOM3;
            }
        }
        //-----------------------------------------------
        public double DenitrificationRate
        {
            get
            {
                return denitrif;
            }
        }
        public double InitialMineralN { get { return initMineralN; } }
        public double InitialFineFuels { get { return initFineFuels; } }

        //---------------------------------------------------------------------

        //public string AgeOnlyDisturbanceParms
        //{
        //    get {
        //        return ageOnlyDisturbanceParms;
        //    }
        //    set {
        //        string path = value;
        //        if (path.Trim(null).Length == 0)
        //            throw new InputValueException(path,"\"{0}\" is not a valid path.",path);
        //        ageOnlyDisturbanceParms = value;
        //    }
        //}

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

        //public void SetFunctionalType(ISpecies species, InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    sppFunctionalType[species] = VerifyRange(newValue, 0, 100, "FunctionalType");
        //}
        public void SetFunctionalType(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            sppFunctionalType[species] = VerifyRange(newValue, 0, 100, "FunctionalType");
        }
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
            gddMin[species] = VerifyRange(newValue, 1, 4000, "GDDMin");
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
            gddMax[species] = VerifyRange(newValue, 500, 7000, "GDDmax");
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
            minJanTemp[species] = VerifyRange(newValue, -60, 20, "MinJanTemp");
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
            maxDrought[species] = VerifyRange(newValue, 0.0, 1.0, "MaxDrought");
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
            leafLongevity[species] = VerifyRange(newValue, 1.0, 10.0, "LeafLongevity");
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
            leafLignin[species] = VerifyRange(newValue, 0.0, 0.4, "LeafLignin");
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
            woodLignin[species] = VerifyRange(newValue, 0.0, 0.4, "WoodLignin");
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
            coarseRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "CourseRootLignin");
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
            fineRootLignin[species] = VerifyRange(newValue, 0.0, 0.4, "FineRootLignin");
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
            leafCN[species] = VerifyRange(newValue, 5.0, 100.0, "LeafCN");
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
            woodCN[species] = VerifyRange(newValue, 5.0, 900.0, "WoodCN");
        }
        //---------------------------------------------------------------------

        //public void SetCoarseRootCN(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    coarseRootCN[species] = VerifyRange(newValue, 5.0, 500.0);
        //}
        public void SetCoarseRootCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            coarseRootCN[species] = VerifyRange(newValue, 5.0, 500.0, "CourseRootCN");
        }
        //---------------------------------------------------------------------

        //public void SetFoliageLitterCN(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    foliageLitterCN[species] = VerifyRange(newValue, 5.0, 100.0);
        //}
        public void SetFoliageLitterCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            foliageLitterCN[species] = VerifyRange(newValue, 5.0, 100.0, "FoliarLitterCN");
        }
        //---------------------------------------------------------------------

        //public void SetFineRootCN(ISpecies           species,
        //                                  InputValue<double> newValue)
        //{
        //    Debug.Assert(species != null);
        //    fineRootCN[species] = VerifyRange(newValue, 5.0, 100.0);
        //}
        public void SetFineRootCN(ISpecies species,double newValue)
        {
            Debug.Assert(species != null);
            fineRootCN[species] = VerifyRange(newValue, 5.0, 100.0, "FineRootCN");
        }
        //---------------------------------------------------------------------

        //public void SetMaxANPP(ISpecies species,
        //                                  InputValue<int> newValue)
        //{
        //    Debug.Assert(species != null);
        //    maxANPP[species] = VerifyRange(newValue, 2, 1000);
        //}
        public void SetMaxANPP(ISpecies species,int newValue)
        {
            Debug.Assert(species != null);
            maxANPP[species] = VerifyRange(newValue, 2, 1000, "MaxANPP");
        }
        //---------------------------------------------------------------------

        public void SetMaxBiomass(ISpecies species, int newValue)
        {
            Debug.Assert(species != null);
            maxBiomass[species] = VerifyRange(newValue, 2, 300000, "MaxBiomass");
        }

        public void SetGrowthLAI(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            growthLAI[species] = VerifyRange(newValue, 0.0, 1.0, "GrowthLAI");
        }

        public void SetLightLAIShape(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            lightLAIShape[species] = VerifyRange(newValue, 0.0, 10.0, "LightLAIShape");
        }

        public void SetLightLAIScale(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            lightLAIScale[species] = VerifyRange(newValue, 0.0, 1000.0, "LightLAIScale"); //Scale parameter can be high for some shade-tolerant spp
        }

        public void SetLightLAILocation(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            lightLAILocation[species] = VerifyRange(newValue, 0.0, 1, "LightLAILocation");
        }


        //---------------------------------------------------------------------

        public void SetAtmosNslope(InputValue<double> newValue)
        {
            atmosNslope = VerifyRange(newValue, -1.0, 2.0, "AtmosNslope");
        }
        //---------------------------------------------------------------------
        public void SetAtmosNintercept(InputValue<double> newValue)
        {
            atmosNintercept = VerifyRange(newValue, -1.0, 2.0, "AtmostNintercept");
        }
        //---------------------------------------------------------------------
        public void SetLatitude(InputValue<double> newValue)
        {
            latitude = VerifyRange(newValue, 0.0, 70.0, "Latitude");
        }
        //---------------------------------------------------------------------
       
        public void SetDecayRateSurf(InputValue<double> newValue)
        {
            decayRateSurf = VerifyRange(newValue, 0.0, 10.0, "DecayRateSurf");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM1(InputValue<double> newValue)
        {
            decayRateSOM1 = VerifyRange(newValue, 0.0, 10.0, "DecayRateSOM1");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM2(InputValue<double> newValue)
        {
            decayRateSOM2 = VerifyRange(newValue, 0.0, 1.0, "DecayRateSOM2");
        }
        //---------------------------------------------------------------------
        public void SetDecayRateSOM3(InputValue<double> newValue)
        {
            decayRateSOM3 = VerifyRange(newValue, 0.0, 1.0, "DecayRateSOM3");
        }
        // --------------------------------------------------------------------
        // Multiplier to adjust judgement whether a tree-cohort is larger than grass layer
        // W.Hotta 2020.07.07
        public void SetGrassThresholdMultiplier(InputValue<double> newValue)
        {
            grassThresholdMultiplier = VerifyRange(newValue, 0.0, 10.0, "GrassThresholdMultiplier");
        }
        //---------------------------------------------------------------------
        public void SetDenitrif(InputValue<double> newValue)
        {
            denitrif = VerifyRange(newValue, 0.0, 1.0, "Denitrification");
        }

        //---------------------------------------------------------------------
        public void SetInitMineralN(InputValue<double> newValue)
        {
            initMineralN = VerifyRange(newValue, 0.0, 5000.0, "InitialMineralN");
        }
        //---------------------------------------------------------------------
        public void SetInitFineFuels(InputValue<double> newValue)
        {
            initFineFuels = VerifyRange(newValue, 0.0, 1.0, "InitialFineFuel");
        }
        //---------------------------------------------------------------------

        public void SetNlog_depend(ISpecies species, bool newValue)
        {
            Debug.Assert(species != null);
            nlog_depend[species] = newValue;
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
        public void SetBetaTemp(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaTemp[species] = VerifyRange(newValue, -10, 10, "DroughtBetaTemp");
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

        public void SetBetaCWD(ISpecies species, double newValue)
        {
            Debug.Assert(species != null);
            betaCWD[species] = VerifyRange(newValue, -10, 10, "DroghtBetaCWD");
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


        public InputParameters(ISpeciesDataset speciesDataset, int litterCnt, int functionalCnt)
        {
            this.speciesDataset = speciesDataset;

            functionalTypes = new FunctionalTypeTable(functionalCnt);
            fireReductionsTable = new FireReductions[11];
            harvestReductionsTable = new List<HarvestReductions>();

            sppFunctionalType       = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            nFixer                  = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            grass                   = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            nlog_depend             = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            gddMin                  = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            gddMax                  = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            minJanTemp              = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            maxDrought              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            leafLongevity           = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            epicormic               = new Landis.Library.Parameters.Species.AuxParm<bool>(speciesDataset);
            leafLignin              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            woodLignin              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            coarseRootLignin        = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fineRootLignin          = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            leafCN                  = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            woodCN                  = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            coarseRootCN            = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            foliageLitterCN         = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            fineRootCN              = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            maxANPP                 = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            maxBiomass              = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            growthLAI               = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
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
            betaTemp = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaSWAAnom = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaBiomass = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaCWD = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaNormCWD = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            betaNormTemp = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            intxnCWD_Biomass = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lagTemp = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            lagCWD = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);
            lagSWA = new Landis.Library.Parameters.Species.AuxParm<int>(speciesDataset);

            lightLAIShape = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lightLAIScale = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);
            lightLAILocation = new Landis.Library.Parameters.Species.AuxParm<double>(speciesDataset);

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
