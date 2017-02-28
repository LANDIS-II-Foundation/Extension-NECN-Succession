//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.LeafBiomassCohorts;

using System;
using System.Collections.Generic;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// A helper class.
    /// </summary>
    public class HarvestReductions
    {
        private string prescription;
        private double woodReduction;
        private double litterReduction;

        public string PrescriptionName
        {
            get
            {
                return prescription;
            }
            set
            {
                if (value != null)
                    prescription = value;
            }
        }

        public double WoodReduction
        {
            get {
                return woodReduction;
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Wood reduction due to fire must be between 0 and 1.0");
                woodReduction = value;
            }

        }
        public double LitterReduction
        {
            get {
                return litterReduction;
            }
            set {
                if (value < 0.0 || value > 1.0)
                    throw new InputValueException(value.ToString(), "Litter reduction due to fire must be between 0 and 1.0");
                litterReduction = value;
            }

        }
        //---------------------------------------------------------------------
        public HarvestReductions()
        {
            this.prescription = "";
            this.WoodReduction = 0.0;
            this.LitterReduction = 0.0;
        }
    }

    public class HarvestEffects
    {
        public static List<HarvestReductions> ReductionsTable;

        public HarvestEffects()
        {
            ReductionsTable = new List<HarvestReductions>();  

            //for(int i=0; i <= numberOfSeverities; i++)
            //{
            //    ReductionsTable[i] = new HarvestReductions();
            //}
        }


        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            ReductionsTable = parameters.HarvestReductionsTable;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes fire effects on litter, coarse woody debris, mineral soil, and charcoal.
        ///   No effects on soil organic matter (negligible according to Johnson et al. 2001).
        /// </summary>
        public static void ReduceLayers(string prescriptionName, Site site)
        {
            PlugIn.ModelCore.UI.WriteLine("   Calculating harvest induced layer reductions...");

            double litterLossMultiplier = 0.0;
            double woodLossMultiplier = 0.0;

            bool found = false;
            foreach (HarvestReductions prescription in ReductionsTable)
            {
                if (SiteVars.HarvestPrescriptionName != null && SiteVars.HarvestPrescriptionName[site].Trim() == prescription.PrescriptionName.Trim())
                {
                    litterLossMultiplier = prescription.LitterReduction;
                    woodLossMultiplier = prescription.WoodReduction;
                    found = true;
                }
            }
            if (!found) return;


            // Structural litter first

            double carbonLoss = SiteVars.SurfaceStructural[site].Carbon * litterLossMultiplier;
            double nitrogenLoss = SiteVars.SurfaceStructural[site].Nitrogen * litterLossMultiplier;
            double summaryNLoss = nitrogenLoss;

            SiteVars.SurfaceStructural[site].Carbon -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;

            SiteVars.SurfaceStructural[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;

            // Metabolic litter

            carbonLoss = SiteVars.SurfaceMetabolic[site].Carbon * litterLossMultiplier;
            nitrogenLoss = SiteVars.SurfaceMetabolic[site].Nitrogen * litterLossMultiplier;
            summaryNLoss += nitrogenLoss;

            SiteVars.SurfaceMetabolic[site].Carbon  -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;

            SiteVars.SurfaceMetabolic[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;

            // Surface dead wood

            //double woodLossMultiplier = ReductionsTable[severity].WoodReduction;

            carbonLoss   = SiteVars.SurfaceDeadWood[site].Carbon * woodLossMultiplier;
            nitrogenLoss = SiteVars.SurfaceDeadWood[site].Nitrogen * woodLossMultiplier;
            summaryNLoss += nitrogenLoss;

            SiteVars.SurfaceDeadWood[site].Carbon   -= carbonLoss;
            SiteVars.SourceSink[site].Carbon        += carbonLoss;
            SiteVars.FireCEfflux[site]               += carbonLoss;

            SiteVars.SurfaceDeadWood[site].Nitrogen -= nitrogenLoss;
            SiteVars.SourceSink[site].Nitrogen        += nitrogenLoss;
            SiteVars.FireNEfflux[site] += nitrogenLoss;

            SiteVars.MineralN[site] += summaryNLoss * 0.01;

        }

    }
}
