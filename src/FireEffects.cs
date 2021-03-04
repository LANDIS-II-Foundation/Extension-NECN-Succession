//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.LeafBiomassCohorts;  

using System;
using System.Collections.Generic;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class FireReductions
    {
        private double coarseLitterReduction;
        private double fineLitterReduction;
        private double somReduction;
        private double cohortWoodReduction;
        private double cohortLeafReduction;
        
        public double CoarseLitterReduction
        {
            get {
                return coarseLitterReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Coarse litter reduction due to fire must be between 0 and 1.0");
                coarseLitterReduction = value;
            }
               
        }
        public double FineLitterReduction
        {
            get {
                return fineLitterReduction; 
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Fine litter reduction due to fire must be between 0 and 1.0");
                fineLitterReduction = value;
            }
               
        }
        public double CohortWoodReduction
        {
            get
            {
                return cohortWoodReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Cohort wood reduction due to fire must be between 0 and 1.0");
                cohortWoodReduction = value;
            }

        }
        public double CohortLeafReduction
        {
            get
            {
                return cohortLeafReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Cohort wood reduction due to fire must be between 0 and 1.0");
                cohortLeafReduction = value;
            }

        }
        public double SOMReduction
        {
            get
            {
                return somReduction;
            }
            set
            {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Soil Organic Matter (SOM) reduction due to fire must be between 0 and 1.0");
                somReduction = value;
            }

        }
        //---------------------------------------------------------------------
        public FireReductions()
        {
            this.CoarseLitterReduction = 0.0; 
            this.FineLitterReduction = 0.0;
            this.CohortLeafReduction = 0.0;
            this.CohortWoodReduction = 0.0;
            this.SOMReduction = 0.0;
        }
    }
    
    public class FireEffects
    {
        public static FireReductions[] ReductionsTable; 
        
        //public FireEffects(int numberOfSeverities)
        //{
        //    ReductionsTable = new FireReductions[numberOfSeverities];  
            
        //    for(int i=0; i <= numberOfSeverities; i++)
        //    {
        //        ReductionsTable[i] = new FireReductions();
        //    }
        //}
       

        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            ReductionsTable = parameters.FireReductionsTable; 
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes fire effects on litter, coarse woody debris, mineral soil, and charcoal.
        ///   No effects on soil organic matter (negligible according to Johnson et al. 2001).
        /// </summary>
        public static void ReduceLayers(byte severity, Site site)
        {
            // PlugIn.ModelCore.UI.WriteLine("   Calculating fire induced layer reductions. FineLitter={0}, Wood={1}, Duff={2}.", ReductionsTable[severity].FineLitterReduction, ReductionsTable[severity].CoarseLitterReduction, ReductionsTable[severity].SOMReduction);

            // Structural litter first
            double fineLitterReduction = 0.0;

            try
            {
                fineLitterReduction = ReductionsTable[severity].FineLitterReduction;
            }
            catch
            {
                PlugIn.ModelCore.UI.WriteLine("   NOTE:  The fire reductions table does not contain an entry for severity {0}.  DEFAULT REDUCTION = 0.", severity);
                return;
            }


            double carbonLoss = SiteVars.SurfaceStructural[site].Carbon * fineLitterReduction;
            double nitrogenLoss = SiteVars.SurfaceStructural[site].Nitrogen * fineLitterReduction;
            double summaryNLoss = nitrogenLoss;
            
            SiteVars.SurfaceStructural[site].Carbon -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceStructural[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;
            SiteVars.FlamingConsumption[site] += carbonLoss * 2.0;
            
            // Metabolic litter

            carbonLoss = SiteVars.SurfaceMetabolic[site].Carbon * fineLitterReduction;
            nitrogenLoss = SiteVars.SurfaceMetabolic[site].Nitrogen * fineLitterReduction;
            summaryNLoss += nitrogenLoss;
            
            SiteVars.SurfaceMetabolic[site].Carbon  -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceMetabolic[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;
            SiteVars.FlamingConsumption[site] += carbonLoss * 2.0;

            // Surface dead wood

            double woodLossMultiplier = ReductionsTable[severity].CoarseLitterReduction;
            
            carbonLoss   = SiteVars.SurfaceDeadWood[site].Carbon * woodLossMultiplier;
            nitrogenLoss = SiteVars.SurfaceDeadWood[site].Nitrogen * woodLossMultiplier;
            summaryNLoss += nitrogenLoss;
            
            SiteVars.SurfaceDeadWood[site].Carbon   -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;
            
            SiteVars.SurfaceDeadWood[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;
            SiteVars.FlamingConsumption[site] += carbonLoss * 2.0 * 0.35;  // 0.35 = Small wood FRACTION FROM ALEC
            SiteVars.SmolderConsumption[site] += carbonLoss * 2.0 * 0.65;  // 0.65 = Large wood FRACTION FROM ALEC


            // Surficial Soil Organic Matter (the 'Duff' layer)

            double SOM_Multiplier = ReductionsTable[severity].SOMReduction;

            carbonLoss = SiteVars.SOM1surface[site].Carbon * SOM_Multiplier;
            nitrogenLoss = SiteVars.SOM1surface[site].Nitrogen * SOM_Multiplier;
            summaryNLoss += nitrogenLoss;

            SiteVars.SOM1surface[site].Carbon -= carbonLoss;
            SiteVars.SourceSink[site].Carbon += carbonLoss;
            SiteVars.FireCEfflux[site] += carbonLoss;

            SiteVars.SOM1surface[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;
            SiteVars.SmolderConsumption[site] += carbonLoss * 2.0;  

            // Transfer 1% to mineral N.
            SiteVars.MineralN[site] += summaryNLoss * 0.01;
            

        }
        //---------------------------------------------------------------------

        // Crown scorching is when a cohort loses its foliage but is not killed.
        public static double CrownScorching(ICohort cohort, byte siteSeverity)
        {
        
            int difference = (int) siteSeverity - cohort.Species.FireTolerance;
            double ageFraction = 1.0 - ((double) cohort.Age / (double) cohort.Species.Longevity);
            
            if(SpeciesData.Epicormic[cohort.Species])
            {
                if(difference < 0)
                    return 0.5 * ageFraction;
                if(difference == 0)
                    return 0.75 * ageFraction;
                if(difference > 0)
                    return 1.0 * ageFraction;
            }
            
            return 0.0;
        }

    }
}
