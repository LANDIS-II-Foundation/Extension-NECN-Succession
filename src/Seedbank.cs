using Landis.Utilities;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;
using System;
using System.Dynamic;

namespace Landis.Extension.Succession.NECN
{
    class Seedbank
    {
        public static void Initialize()
        {
            //TODO set initial seedbank age
            //TODO do we need to initialize, or can we just add to the dictionar for each site as we go?
        }


        public static void UpdateSeedbank(ActiveSite site, ISpecies species)
        // TODO For each NECN timestep, this logic should be called from the main succession loop.

        //TODO make sure that we're adding keys to the dictionary for each species as needed
        {
            // Figure out the age of the oldest cohort for this species at this site
            int maxCohortAge = 0;
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                if (speciesCohorts.Species == species)
                {
                    foreach (ICohort cohort in speciesCohorts)
                    {
                        if (cohort.Data.Age > maxCohortAge) maxCohortAge = cohort.Data.Age;
                    }
                }
            }

            int maturity_age = species.Maturity;
            int seedbank_longevity = SpeciesData.SeedbankLongevity[species];
            int seedbank_age = SiteVars.SeedbankAge[site][species];

            if (maxCohortAge > maturity_age)
            {
                SiteVars.SeedbankViability[site][species] = true;
                SiteVars.SeedbankAge[site][species] = 0;

                // TODO Disperse seeds to neighbors
                //DisperseSeedsToNeighbors(site, species);
            }
            else
            {
                seedbank_age += seedbank_age;
            }

            SiteVars.SeedbankAge[site][species] = seedbank_age;

            if (seedbank_age > seedbank_longevity)
            {
                SiteVars.SeedbankViability[site][species] = false;
            }
        }

        public static void AddSeedbank(ActiveSite site, ISpecies species)
        {
            // Initialize the dictionary for the species if needed
            if (!SiteVars.SeedbankAge[site].ContainsKey(species))
                SiteVars.SeedbankAge[site][species] = 0;
            if (!SiteVars.SeedbankViability[site].ContainsKey(species))
                SiteVars.SeedbankViability[site][species] = false;
            // Set the seedbank age and viability
            SiteVars.SeedbankAge[site][species] = 0;
            SiteVars.SeedbankViability[site][species] = true;
        }

        public static void PostfireGerminate(ActiveSite site)
        {
            foreach (var speciesEntry in SiteVars.SeedbankAge[site])
            {
                ISpecies species = speciesEntry.Key; // Extract the species from the dictionary entry
                if (SiteVars.SeedbankViability[site][species])
                {
                    PlugIn.AddNewCohort(species, site, "Postfire", 1.0); // TODO SF I had to change this to static in PlugIn
                }
            }
        }


        public static void ClearSeedbank(ActiveSite site)
        {
            SiteVars.SeedbankAge[site].Clear();
            SiteVars.SeedbankViability[site].Clear();
        }

        public static void ClearSeedbankSpecies(ActiveSite site, ISpecies species)
        {
            SiteVars.SeedbankAge[site].Remove(species);
            SiteVars.SeedbankViability[site].Remove(species);
        }

        /* 
         * TODO: Get seedbank dispersal set up; not included in current code
        public List<ActiveSite> GetSeedbankDispersalList(ActiveSite site)
        {
            site.GetNeighbor();
        }

        
        public void DisperseSeedsToNeighbors(ActiveSite site, ISpecies species)
        {
            var neighborList = GetSeedbankDispersalList();

            // Get all neighboring sites (8-way, including diagonals)
            foreach (ActiveSite neighbor in site.GetNeighbors())
            {
                // Only disperse to active sites (not water, inactive, etc.)
                if (!neighbor.IsActive)
                    continue;

                // Initialize the dictionary for the neighbor if needed
                if (!SiteVars.SeedbankAge[neighbor].ContainsKey(species))
                    SiteVars.SeedbankAge[neighbor][species] = 0; //TODO check if we should initilize all sites to 0 or some other value
                if (!SiteVars.SeedbankViability[neighbor].ContainsKey(species))
                    SiteVars.SeedbankViability[neighbor][species] = false;

                // Disperse seeds: reset age and set viability to true
                SiteVars.SeedbankAge[neighbor][species] = 0;
                SiteVars.SeedbankViability[neighbor][species] = true;
            }
        }
        */
    }
}
