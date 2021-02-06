using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class PrimaryLogShort
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2_yr1, Desc = "Net Ecosystem Exchange C", Format = "0.0")]
        public double NEEC {get; set;}

        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Total Soil Organic Carbon", Format = "0.0")]
        public double SOMTC { get; set; }

        [DataFieldAttribute(Unit = FieldUnits.g_B_m2, Desc = "Aboveground Biomass", Format = "0.0")]
        public double AGB { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2_yr1, Desc = "Aboveground NPP C", Format = "0.0")]
        public double AG_NPPC { get; set; }
        
        
        [DataFieldAttribute(Unit = FieldUnits.g_N_m2, Desc = "Mineral N", Format = "0.00")]
        public double MineralN { get; set; }
        
        [DataFieldAttribute(Unit = FieldUnits.g_C_m2, Desc = "Dead Wood C", Format = "0.0")]
        public double C_DeadWood { get; set; }


    }
}
