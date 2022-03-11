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

        bool CalibrateMode { get; set; }
        WaterType WType {get;set;}
        double ProbEstablishAdjustment { get; set; }
        double[] MaximumShadeLAI { get; }
        bool SmokeModelOutputs { get; set; }
        bool SoilWater_Henne { get; set; }
        double GrassThresholdMultiplier { get; }

        //---------------------------------------------------------------------
        /// <summary>
        /// A suite of parameters for species functional groups
        /// </summary>
        FunctionalTypeTable FunctionalTypes
        {
            get;set;
        }
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

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        List<ISufficientLight> LightClassProbabilities
        {
            get;
        }

        //---------------------------------------------------------------------

        Landis.Library.Parameters.Species.AuxParm<int> SppFunctionalType{get;}
        Landis.Library.Parameters.Species.AuxParm<bool> NFixer{get;}
        Landis.Library.Parameters.Species.AuxParm<bool> Grass { get; }
        Landis.Library.Parameters.Species.AuxParm<bool> Nlog_depend { get; } // W.Hotta (2021.08.01)
        Landis.Library.Parameters.Species.AuxParm<int> GDDmin{get;}
        Landis.Library.Parameters.Species.AuxParm<int> GDDmax{get;}
        Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp{get;}
        Landis.Library.Parameters.Species.AuxParm<double> MaxDrought{get;}
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
