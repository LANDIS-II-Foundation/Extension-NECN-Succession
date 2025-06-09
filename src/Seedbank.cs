using Landis.Utilities;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using System;
using System.Dynamic;


namespace Landis.Extension.Succession.NECN
{

    //TODO Steps:


    class Seedbank
    {
        public void Initialize()
        {
            //TODO set initial seedbank age

        }

        public double[] UpdateSeedbank(ActiveSite site, ISpecies species)
        {

            //TODO only do this for the oldest cohort of a given species for each site
            // get max age for the species in the site

            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int maxCohortAge = 0;
                    if (cohort.Data.Age > maxCohortAge) maxCohortAge = cohort.Data.Age;
                }
            }

            int maturity_age = species.Maturity;
            int seedbank_longevity = species.SeedbankLongevity; //TODO add species param var
            int seedbank_age = SiteVars.SeedBankAge[site][species]; //TODO add SiteVar

            if (oldestCohort.Data.Age > maturity_age)
            {
                seedbank_age = 0;
            }
            else
            {
                seedbank_age += seedbank_age;
            }

            SiteVars.SeedBankAge[site][species] = seedbank_age;

            if(seedbank_age > seedbank_longevity)
            {
                SiteVars.SeedBankViability[site][species] = false;
            }

        }

        public void ClearSeedbank()
        {
        }

        public void GetSeedbankDispersalList(ActiveSite site)
        {

        }



        //		For each NECN timestep: 
        //	For each site
        //		For each seedbanking species
        //			If seedbank_age > seedbank_longevity
        //				Set seedbank_viability to FALSE				
        //if cohort age > maturity_age
        //	set seedbank_viability to TRUE
        //	set seedbank_age to 0
        //				Disperse seeds into neighboring site seedbanks


    }
}
