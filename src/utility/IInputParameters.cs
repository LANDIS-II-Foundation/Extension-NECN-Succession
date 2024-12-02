//  Author: Robert Scheller, Melissa Lucash

using Landis.Library.Succession;
using Landis.Utilities;
using Landis.Library.Parameters;
using System.Collections.Generic;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// The parameters for biomass succession.
    /// </summary>
    public interface IInputParameters
    {
        int Timestep{ get;set;}
        SeedingAlgorithms SeedAlgorithm{ get;set;}
        string InitialCommunities{ get;set;}
        string InitialCommunitiesMap{ get;set;}
        string ClimateConfigFile { get; set; }
        string SoilDepthMapName { get; set; }
        string SoilDrainMapName { get; set; }
        string SoilBaseFlowMapName { get; set; }
        string SoilStormFlowMapName { get; set; }
        string SoilFieldCapacityMapName { get; set; }
        string SoilWiltingPointMapName { get; set; }
        string SoilPercentSandMapName { get; set; }
        string SoilPercentClayMapName { get; set; }
        string InitialSOM1CSurfaceMapName { get; set; }
        string InitialSOM1NSurfaceMapName { get; set; }
        string InitialSOM1CSoilMapName { get; set; }
        string InitialSOM1NSoilMapName { get; set; }
        string InitialSOM2CMapName { get; set; }
        string InitialSOM2NMapName { get; set; }
        string InitialSOM3CMapName { get; set; }
        string InitialSOM3NMapName { get; set; }
        string InitialDeadSurfaceMapName { get; set; }
        string InitialDeadSoilMapName { get; set; }
        string NormalSWAMapName { get; set; }
        string NormalCWDMapName { get; set; }
        string NormalTempMapName { get; set; }
        string SlopeMapName { get; set; }
        string AspectMapName { get; set; }
        bool CalibrateMode { get; set; }
        WaterType WType {get;set;}
        double ProbEstablishAdjustment { get; set; }
        bool SmokeModelOutputs { get; set; }
        double GrassThresholdMultiplier { get; }
        string CommunityInputMapNames { get; set; }

        bool OutputSoilWaterAvailable { get; set; }
        bool OutputClimateWaterDeficit { get; set; }
        bool OutputTemp { get; set; }
        bool WriteMeanSoilWaterMap { get; set; }  
        bool WritePETMap { get; set; }

        bool WriteSpeciesDroughtMaps { get; set; }


        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters for fire effects on wood and leaf litter
        /// </summary>
        FireReductions[] FireReductionsTable
        {
            get;set;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Parameters for harvest or fuel treatment effects on wood and leaf litter
        /// </summary>
        List<HarvestReductions> HarvestReductionsTable
        {
            get;
            set;
        }

        //---------------------------------------------------------------------

        Landis.Library.Parameters.Species.AuxParm<bool> NFixer{get;}
        Landis.Library.Parameters.Species.AuxParm<bool> Grass { get; }
        Landis.Library.Parameters.Species.AuxParm<bool> Nlog_depend { get; } // W.Hotta (2021.08.01)
        Landis.Library.Parameters.Species.AuxParm<int> GDDmin{get;}
        Landis.Library.Parameters.Species.AuxParm<int> GDDmax{get;}
        Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp{get;}
        Landis.Library.Parameters.Species.AuxParm<double> MaxDrought{get;}
        //CWD Establishment
        Landis.Library.Parameters.Species.AuxParm<int> CWDBegin { get; }
        Landis.Library.Parameters.Species.AuxParm<int> CWDMax { get; }

        Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity {get;}
        Landis.Library.Parameters.Species.AuxParm<bool> Epicormic {get;}
        Landis.Library.Parameters.Species.AuxParm<double> LeafLignin {get;}
        Landis.Library.Parameters.Species.AuxParm<double> WoodLignin {get;}
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin {get;}
        Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin {get;}
        Landis.Library.Parameters.Species.AuxParm<double> LeafCN {get;}
        Landis.Library.Parameters.Species.AuxParm<double> WoodCN {get;}
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN {get;}
        Landis.Library.Parameters.Species.AuxParm<double> FoliageLitterCN {get;}
        Landis.Library.Parameters.Species.AuxParm<double> FineRootCN {get;}
        Landis.Library.Parameters.Species.AuxParm<int> MaxANPP { get; }
        Landis.Library.Parameters.Species.AuxParm<int> MaxBiomass { get; }
        Landis.Library.Parameters.Species.AuxParm<double> GrowthLAI { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation { get; }
        Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust { get; }

        //Drought threshold parameters
        Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold { get; }
        Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold2 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold2 { get; }
        //Multiple regression parameters
        Landis.Library.Parameters.Species.AuxParm<double> Intercept { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaAge { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaTempAnom { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaCWDAnom { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BetaNormTemp { get; }
        Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass { get; }
        Landis.Library.Parameters.Species.AuxParm<int> LagTemp { get; }
        Landis.Library.Parameters.Species.AuxParm<int> LagCWD { get; }
        Landis.Library.Parameters.Species.AuxParm<int> LagSWA { get; }

        Landis.Library.Parameters.Species.AuxParm<double> Tempcurve1 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Tempcurve2 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Tempcurve3 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Tempcurve4 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> FractionANPPtoLeaf { get; }
        Landis.Library.Parameters.Species.AuxParm<double> BiomassToLAI { get; }
        Landis.Library.Parameters.Species.AuxParm<double> K_LAI { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MinLAI { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MaxLAI { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve1 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve2 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve3 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> Moisturecurve4 { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MinSoilDrain { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MonthlyWoodMortality { get; }
        Landis.Library.Parameters.Species.AuxParm<double> WoodDecayRate { get; }
        Landis.Library.Parameters.Species.AuxParm<double> MortalityCurveShape { get; }
        Landis.Library.Parameters.Species.AuxParm<int> LeafNeedleDrop { get; }
        Landis.Library.Parameters.Species.AuxParm<double> CoarseRootFraction { get; }
        Landis.Library.Parameters.Species.AuxParm<double> FineRootFraction { get; }


        double AtmosNslope {get;}
        double AtmosNintercept {get;}
        double Latitude {get;}
        double DecayRateSurf { get; }
        double DecayRateSOM1 { get; }
        double DecayRateSOM2 { get; }
        double DecayRateSOM3 { get; }
        double DenitrificationRate { get; }
        double InitialMineralN { get; }
        double InitialFineFuels { get; }
   
        
    }
}
