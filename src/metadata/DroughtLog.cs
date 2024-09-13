using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class DroughtLog
    {
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Name")]
        public string ClimateRegionName { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites")]
        public int NumSites { set; get; }

        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { get; set; }

        [DataFieldAttribute(Desc = "Species Index")]
        public int SpeciesIndex { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Average Biomass Killed", Format = "0.0")]
        public double AverageBiomassKilled { get; set; }

        [DataFieldAttribute(Desc = "Average Probability of Mortality", Format = "0.0000")]
        public double AverageProbabilityMortality { get; set; }

        [DataFieldAttribute(Desc = "Number of cohorts", Format = "0.0000")]
        public double NumberCohorts { get; set; }

    }
}
