//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// The pool of dead biomass at a site.
    /// </summary>
    public class LitterLayer
    {

        /// <summary>
        /// Decomposes Structural Litter
        /// </summary>
        public static void Decompose(ActiveSite site)
        {
            SiteVars.SurfaceStructural[site].DecomposeStructural(site);
            SiteVars.SurfaceMetabolic[site].DecomposeMetabolic(site);

            SiteVars.SoilStructural[site].DecomposeStructural(site);
            SiteVars.SoilMetabolic[site].DecomposeMetabolic(site);

        }

        public static void PartitionResidue(
                            double inputMass,
                            double inputDecayValue,
                            double inputCNratio,
                            double fracLignin,
                            double ratioCNstructural,
                            //double CNratiofrass,
                            LayerName name,
                            LayerType type,
                            ActiveSite site)
        {
            double cAddToMetabolic, cAddToStructural, directAbsorb;
            double NAddToMetabolic, NAddToStructural, Npart;
            double fracStructuralLignin, fracMetabolic, fracN, ratioCNtotal, ratioLigninN;
            double totalNitrogen = 0.0;

            double totalC = inputMass * 0.47;

            if (totalC < 0.0000001)
            {
                //PlugIn.ModelCore.UI.WriteLine("C inputs to litter layer below threshold");
                return;
            }

            // ...For each mineral element..
            // ...Compute amount of element in residue.
            Npart = totalC / inputCNratio;

            // ...Direct absorption of mineral element by residue
            //      (mineral will be transferred to donor compartment
            //      and then partitioned into structural and metabolic
            //      using flow routines.)

            // ...If minerl(SRFC,iel) is negative then directAbsorb = zero.
            if (SiteVars.MineralN[site] <= 0.0)
                directAbsorb  = 0.0;
            else
                directAbsorb = SiteVars.MineralN[site]
                                * OtherData.FractionSurfNAbsorbed
                                * System.Math.Max(totalC / OtherData.ResidueMaxDirectAbsorb, 1.0);


            // ...If C/N ratio is too low, transfer just enough to make
            //       C/N of residue = damrmn
            if (Npart + directAbsorb  <= 0.0)
                ratioCNtotal = 0.0;
            else
                ratioCNtotal = totalC / (Npart + directAbsorb);

            if (ratioCNtotal < OtherData.MinResidueCN )
                directAbsorb  = (totalC / OtherData.MinResidueCN) - Npart;

            if (directAbsorb  < 0.0)
                directAbsorb  = 0.0;

            if(directAbsorb > SiteVars.MineralN[site])
                directAbsorb = SiteVars.MineralN[site];

            SiteVars.MineralN[site] -= directAbsorb;

            totalNitrogen = directAbsorb + Npart;

            // ...Partition carbon into structural and metabolic fraction of
            //      residue (including direct absorption) which is nitrogen
            fracN = totalNitrogen / inputMass; // (totalC * 2.0);

            // ...Lignin/nitrogen ratio of residue
            ratioLigninN = fracLignin / fracN;

            // METABOLIC calculations
            // ...Carbon added to metabolic
            //      Compute the fraction of carbon that goes to metabolic.

            fracMetabolic = OtherData.MetaStructSplitIntercept - OtherData.MetaStructSplitSlope * ratioLigninN;

            // ...Make sure the fraction of residue which is lignin isn't
            //      greater than the fraction which goes to structural.  -rm 12/91
            if (fracLignin >  (1.0 - fracMetabolic))
                fracMetabolic = (1.0 - fracLignin);

            // ...Make sure at least 1% goes to metabolic
            if (fracMetabolic < 0.20)
                fracMetabolic = 0.20;

            // ...Compute amounts to flow
            cAddToMetabolic = totalC * fracMetabolic;
            if (cAddToMetabolic < 0.0)
                cAddToMetabolic = 0.0;

            if((int) type == (int) LayerType.Surface)
            {
                SiteVars.SurfaceMetabolic[site].Carbon += cAddToMetabolic;
             }
            else
            {
                SiteVars.SoilMetabolic[site].Carbon += cAddToMetabolic;
            }

            // STRUCTURAL calculations

            cAddToStructural = totalC - cAddToMetabolic;

            // ...Adjust lignin content of structural.
            // ...fracStructuralLignin is the fraction of incoming structural residue
            //      which is lignin; restricting it to a maximum of .8
            fracStructuralLignin = fracLignin / (cAddToStructural / totalC);

            if((int) type == (int) LayerType.Surface && cAddToMetabolic <= 0.0)
                //PlugIn.ModelCore.UI.WriteLine("   SURFACE cAddToMetabolic={0}.", cAddToMetabolic);

            // ...Changed allowable maximum fraction from .6 to 1.0  -lh 1/93
            if (fracStructuralLignin > 1.0)
                fracStructuralLignin = 1.0;

            if((int) type == (int) LayerType.Surface)
            {
                SiteVars.SurfaceStructural[site].Carbon += cAddToStructural;
            }
            else
            {
                SiteVars.SoilStructural[site].Carbon += cAddToStructural;
            }

            // ...Adjust lignin in Structural Layers
            // adjlig(strucc(lyr), fracStructuralLignin, cAddToStructural, strlig(lyr));

            Layer structuralLayer;

            if((int) type == (int) LayerType.Surface)
                structuralLayer = SiteVars.SurfaceStructural[site];
            else
                structuralLayer = SiteVars.SoilStructural[site];

            structuralLayer.AdjustLignin(cAddToStructural, fracStructuralLignin);

            // Don't adjust litter decay rate: the base decay rate is
            // always 1.0
            // AdjustDecayRate();

            // ...Partition mineral elements into structural and metabolic
            // ...Flow into structural
            // ...Flow into metabolic
            NAddToStructural = cAddToStructural / ratioCNstructural;  //RATIO CN STRUCTURAL from species data
            NAddToMetabolic = totalNitrogen - NAddToStructural;

            if((int) type == (int) LayerType.Surface)
            {
                SiteVars.SurfaceStructural[site].Nitrogen += NAddToStructural;
                SiteVars.SurfaceMetabolic[site].Nitrogen += NAddToMetabolic;
            }
            else
            {
                SiteVars.SoilStructural[site].Nitrogen += NAddToStructural;
                SiteVars.SoilMetabolic[site].Nitrogen += NAddToMetabolic;
                //PlugIn.ModelCore.UI.WriteLine("  N added to Structural Soil: {0}.", NAddToStructural);
            }

            return;

        //}
        }

    }
}
