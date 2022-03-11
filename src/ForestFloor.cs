//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using System;
using System.Collections.Generic;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Soil organic matter (SOM) pool.
    /// Mass = C Fraction + N Fraction + P Fraction + other(inexplicit).
    /// </summary>
    public class ForestFloor
    {

        /// <summary>
        /// Adds some biomass for a species to the WOOD litter pools at a site.
        /// </summary>
        public static void AddWoodLitter(double woodBiomass, 
                                    ISpecies   species,
                                    ActiveSite site)
        {
            
            if (woodBiomass > 0)
                WoodLayer.PartitionResidue(woodBiomass,
                                FunctionalType.Table[SpeciesData.FuncType[species]].WoodDecayRate,
                                SpeciesData.WoodCN[species],
                                SpeciesData.WoodLignin[species],
                                LayerName.Wood,
                                LayerType.Surface,
                                site);
            
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Adds some biomass for a species to the foliar LITTER pools at a site.
        /// </summary>
        public static void AddFoliageLitter(double foliarBiomass, 
                                      ISpecies   species,
                                      ActiveSite site)
        {

            double inputDecayValue = 1.0;   // Decay value is calculated for surface/soil layers (leaf/fine root), 
                                            // therefore, this is just a dummy value.
                                            
            if(foliarBiomass > 0)
            {
                SiteVars.LitterfallC[site] += foliarBiomass * 0.47;
                
                LitterLayer.PartitionResidue(
                            foliarBiomass,
                            inputDecayValue,
                            SpeciesData.LeafCN[species],
                            SpeciesData.LeafLignin[species],
                            OtherData.StructuralCN,
                            LayerName.Leaf,
                            LayerType.Surface,
                            site);
            }


        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Adds some biomass for a species to the foliar LITTER pools at a site.
        /// Assumes that some of the N has been resorbed.
        /// </summary>
        public static void AddResorbedFoliageLitter(double foliarBiomass, ISpecies species, ActiveSite site)
        {

            double inputDecayValue = 1.0;   // Decay value is calculated for surface/soil layers (leaf/fine root), 
            // therefore, this is just a dummy value.

            if (foliarBiomass > 0)
            {
                SiteVars.LitterfallC[site] += foliarBiomass * 0.47;

                LitterLayer.PartitionResidue(
                            foliarBiomass,
                            inputDecayValue,
                            SpeciesData.LeafLitterCN[species],
                            SpeciesData.LeafLignin[species],
                            OtherData.StructuralCN,
                            LayerName.Leaf,
                            LayerType.Surface,
                            site);
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Adds frass for a species to the foliar LITTER pools at a site.
        /// Assumes that some of the N has been resorbed.
        /// </summary>
        public static void AddFrassLitter(double inputFrassBiomass, ISpecies species, ActiveSite site)
        {

            double inputDecayValue = 1.0;   // Decay value is calculated for surface/soil layers (leaf/fine root), 
            // therefore, this is just a dummy value.

            
            if (inputFrassBiomass > 0)
            {
                //SiteVars.LitterfallC[site] += defoliatedLeafBiomass * 0.47;

                //double frassBiomass = Math.Max(0.0, OtherData.frassdepk * defoliatedLeafBiomass);
                // Frass C added is a function of defoliated leaf biomass, but adjusted for the CN of litter and frass
                // Any C lost is due to insect metabolism
                double inputFrassC = inputFrassBiomass * 0.47;
                double inputFrassN = inputFrassC / (double) SpeciesData.LeafLitterCN[species];
                double actualFrassC = inputFrassN * (double) OtherData.CNratiofrass;  // the difference between input and actual is C lost to insect metabolism
                double actualFrassBiomass = actualFrassC / 0.47;

                //PlugIn.ModelCore.UI.WriteLine("AddFrass.Month={0:0}, inputfrassN={1:0.000}, inputfrassbiomass={2:0.00}, actualfrassbiomass={3:0.00} ", Main.Month, inputFrassN, inputFrassBiomass, actualFrassBiomass);

                SiteVars.FrassC[site] += actualFrassC;
 
                LitterLayer.PartitionResidue(
                            actualFrassBiomass,
                            inputDecayValue,
                            OtherData.CNratiofrass,
                            0.1,
                            OtherData.StructuralCN,
                            LayerName.Leaf,
                            LayerType.Surface,
                            site);
            }
        } 
         

    }
}
