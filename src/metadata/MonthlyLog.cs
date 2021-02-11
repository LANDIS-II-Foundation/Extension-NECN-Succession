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

        [DataFieldAttribute(Desc = "Climate Region Name")]
        public string ClimateRegionName { set; get; }

        [DataFieldAttribute(Desc = "Climate Region Index")]
        public int ClimateRegionIndex { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Number of Sites")]
        public int NumSites { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Precipitation", Format = "0.0")]
        public double Precipitation {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.DegreeC, Desc = "Air Temperature", Format = "0.0")]
        public double AirTemp { get; set; }

        [DataFieldAttribute(Unit = "g_C_m2_month1", Desc = "Total NPP C", Format = "0.00")]
        public double AvgTotalNPP_C { get; set; }

        [DataFieldAttribute(Unit = "g_C_m2_month1", Desc = "Heterotrophic Respiration", Format = "0.00")]
        public double AvgHeteroRespiration { get; set; }

        [DataFieldAttribute(Unit = "g_C_m2_month1", Desc = "Soil Respiration", Format = "0.00")]
        public double AvgSoilRespiration { get; set; }

        [DataFieldAttribute(Unit = "g_C_m2_month1", Desc = "Net Ecosystem Exchange", Format = "0.00")]
        public double avgNEE { get; set; }

        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "N Deposition and Fixation", Format = "0.00")]
        public double Ndep { get; set; }

        [DataFieldAttribute(Unit = "g_N_m2_month1", Desc = "N Leaching", Format = "0.00")]
        public double StreamN { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.cm, Desc = "Soil Water Content", Format = "0.00")]
        public double SoilWaterContent { get; set; }
    }
}
