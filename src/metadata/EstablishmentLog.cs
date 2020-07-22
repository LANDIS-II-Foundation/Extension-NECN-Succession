using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class EstablishmentLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { set; get; }

        [DataFieldAttribute(Desc = "Climate Region")]
        public string ClimateRegion { set; get; }

        [DataFieldAttribute(Desc = "Number of Establishment Attempts")]
        public int NumberAttempts { set; get; }

        [DataFieldAttribute(Desc = "Average Temperature Multiplier", Format = "0.00")]
        public double AvgTempMult { set; get; }

        [DataFieldAttribute(Desc = "Average January T Multiplier", Format = "0.00")]
        public double AvgMinJanTempMult { set; get; }

        [DataFieldAttribute(Desc = "Average Soil Moisture Multiplier", Format = "0.00")]
        public double AvgSoilMoistureMult { set; get; }

        [DataFieldAttribute(Desc = "Average Probability of Establishment", Format = "0.00")]
        public double AvgProbEst { set; get; }

        [DataFieldAttribute(Desc = "Number of Drought Days", Format = "0.00")]
        public double DryDays { set; get; }

        [DataFieldAttribute(Desc = "Beginning of Growing Season", Format = "0.00")]
        public double BeginGDD { set; get; }

        [DataFieldAttribute(Desc = "End of Growing Season", Format = "0.00")]
        public double EndGDD { set; get; }
    }
}
