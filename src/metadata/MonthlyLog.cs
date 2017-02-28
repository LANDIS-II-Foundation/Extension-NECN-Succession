using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class MonthlyLog
    {
        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Unit = FieldUnits.Month, Desc = "Simulation Month")]
        public int Month { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Name")]
        public string EcoregionName { set; get; }

        [DataFieldAttribute(Desc = "Ecoregion Index")]
        public int EcoregionIndex { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites")]
        public int NumSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Precipitation", Format = "0.0")]
        public double ppt {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Air Temperature", Format = "0.0")]
        public double airtemp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Aboveground NPP C", Format = "0.00")]
        public double avgNPPtc { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Aboveground Heterotrophic Respiration", Format = "0.00")]
        public double avgResp { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Net Ecosystem Exchange", Format = "0.00")]
        public double avgNEE { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Deposition and Fixation", Format = "0.00")]
        public double Ndep { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "N Leaching", Format = "0.00")]
        public double StreamN { get; set; }
    }
}
