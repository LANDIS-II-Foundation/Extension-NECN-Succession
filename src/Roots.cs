//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.LeafBiomassCohorts;
using System;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Fine and coarse roots.
    /// </summary>
    public class Roots
    {

        //---------------------------------------------------------------------

        /// <summary>
        /// Kills coarse roots and add the biomass to the Wood Debris pool.
        /// </summary>
        public static void AddCoarseRootLitter(double abovegroundWoodBiomass,ICohort cohort,
                                    ISpecies   species,
                                    ActiveSite site)
        {

            double coarseRootBiomass = CalculateCoarseRoot(cohort, abovegroundWoodBiomass); // Ratio above to below

            if(coarseRootBiomass > 0)
            WoodLayer.PartitionResidue(coarseRootBiomass,  
                            FunctionalType.Table[SpeciesData.FuncType[species]].WoodDecayRate,
                            SpeciesData.CoarseRootCN[species], 
                            SpeciesData.CoarseRootLignin[species], 
                            LayerName.CoarseRoot,
                            LayerType.Soil,
                            site);
            
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Kills fine roots and add the biomass to the Dead Fine Roots pool.
        /// </summary>
        public static void AddFineRootLitter(double abovegroundFoliarBiomass, ICohort cohort,
                                      ISpecies   species,
                                      ActiveSite site)
        {
            double fineRootBiomass = CalculateFineRoot(cohort, abovegroundFoliarBiomass); 
            
            double inputDecayValue = 1.0;   // Decay value is calculated for surface/soil (leaf/fine root), 
                                            // therefore, this is just a dummy value.
            if(fineRootBiomass > 0)
            LitterLayer.PartitionResidue(
                            fineRootBiomass,
                            inputDecayValue,
                            SpeciesData.FineRootCN[species],
                            SpeciesData.FineRootLignin[species],
                            OtherData.StructuralCN,
                            LayerName.FineRoot,
                            LayerType.Soil,
                            site);
            
        }
        
        /// <summary>
        /// Calculate coarse and fine roots biomass or NPP based on aboveground wood and leaf biomass.
        /// </summary>
        public static double CalculateCoarseRoot(ICohort cohort, double wood)
        {
            return (wood * FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].CoarseRootFraction);
        }
        public static double CalculateFineRoot(ICohort cohort, double foliarBiomass)
        {
            return (foliarBiomass * FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FineRootFraction);
        }
    }
}
