using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Extension.Succession.NECN
{
    public class ReproductionLog
    {

        [DataFieldAttribute(Unit = FieldUnits.Year, Desc = "Simulation Year")]
        public int Time {set; get;}

        [DataFieldAttribute(Desc = "Species Name")]
        public string SpeciesName { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Planting Number of Cohorts")]
        public int NumCohortsPlanting { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Serotiny Number of Cohorts")]
        public int NumCohortsSerotiny { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Resprouting Number of Cohorts")]
        public int NumCohortsResprout { set; get; }

        [DataFieldAttribute(Unit = FieldUnits.Count, Desc = "Seeding Number of Cohorts")]
        public int NumCohortsSeed { set; get; }



    }
}
