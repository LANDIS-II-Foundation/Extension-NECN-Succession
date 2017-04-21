//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Library.Succession;
using Edu.Wisc.Forest.Flel.Util;
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


        //double SpinupMortalityFraction { get; set; }
        bool CalibrateMode { get; set; }
        WaterType WType {get;set;}
        double ProbEstablishAdjustment { get; set; }
        double[] MaximumShadeLAI { get; }

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
        /// The maximum relative biomass for each shade class.
        /// </summary>
        //Ecoregions.AuxParm<Percentage>[] MinRelativeBiomass
        //{
        //    get;
        //}

        //---------------------------------------------------------------------

        /// <summary>
        /// Definitions of sufficient light probabilities.
        /// </summary>
        List<ISufficientLight> LightClassProbabilities
        {
            get;
        }

        //---------------------------------------------------------------------

        Species.AuxParm<int> SppFunctionalType{get;}
        Species.AuxParm<bool> NFixer{get;}
        Species.AuxParm<int> GDDmin{get;}
        Species.AuxParm<int> GDDmax{get;}
        Species.AuxParm<int> MinJanTemp{get;}
        Species.AuxParm<double> MaxDrought{get;}
        Species.AuxParm<double> LeafLongevity {get;}
        Species.AuxParm<bool> Epicormic {get;}
        Species.AuxParm<double> LeafLignin {get;}
        Species.AuxParm<double> WoodLignin {get;}
        Species.AuxParm<double> CoarseRootLignin {get;}
        Species.AuxParm<double> FineRootLignin {get;}
        Species.AuxParm<double> LeafCN {get;}
        Species.AuxParm<double> WoodCN {get;}
        Species.AuxParm<double> CoarseRootCN {get;}
        Species.AuxParm<double> FoliageLitterCN {get;}
        Species.AuxParm<double> FineRootCN {get;}
        Species.AuxParm<int> MaxANPP { get; }
        Species.AuxParm<int> MaxBiomass { get; }

        double AtmosNslope {get;}
        double AtmosNintercept {get;}
        double Latitude {get;}
        double DecayRateSurf { get; }
        double DecayRateSOM1 { get; }
        double DecayRateSOM2 { get; }
        double DecayRateSOM3 { get; }
        double Denitrif { get; }
        double InitialMineralN { get; }
        

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with the biomass parameters for age-only
        /// disturbances.
        /// </summary>
        string AgeOnlyDisturbanceParms
        {
            get;set;
        }

        
    }
}
