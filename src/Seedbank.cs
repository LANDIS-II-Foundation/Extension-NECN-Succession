using Landis.Core;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;
using Landis.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Landis.Library.Succession;

namespace Landis.Extension.Succession.NECN
{
    class Seedbank
    {
        public static void Initialize()
        {
            //TODO set initial seedbank age
            //TODO do we need to initialize, or can we just add to the dictionary for each site as we go?
        }


        public static void UpdateSeedbank(ActiveSite site)
        {
            //TODO make sure that we're adding keys to the dictionary for each species as needed

            //First, age up all the existing seedbanks and set viability to false if age exceeds longevity
            //Get the list of species present in the seedbank at this site
            var seedbankSpeciesList = new List<ISpecies>();
            // Loop through all species in the SiteVars.SeedbankAge dictionary
            foreach (ISpecies species in SiteVars.SeedbankAge[site].Keys)
            {
                if (!seedbankSpeciesList.Contains(species))
                {
                    seedbankSpeciesList.Add(species);
                }
            }


            foreach (ISpecies species in seedbankSpeciesList)
            {
                int seedbank_longevity = SpeciesData.SeedbankLongevity[species];

                //Initialize the seedbank age and viability if not already done
                // SF needed? we should only be working with species already in the seedbank
                if (!SiteVars.SeedbankAge[site].ContainsKey(species))
                {
                    SiteVars.SeedbankAge[site][species] = 0;
                    SiteVars.SeedbankViability[site][species] = false;
                }

                SiteVars.SeedbankAge[site][species] += 1; //TODO fix this for different NECN timestep lengths

                if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Seedbank age for {0} at site {1}: {2}", species.Name, site.Location, SiteVars.SeedbankAge[site][species]);

                if (SiteVars.SeedbankAge[site][species] > seedbank_longevity)
                {
                    SiteVars.SeedbankViability[site][species] = false;
                    if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Seedbank age for {0} at site {1} exceeds longevity ({2} > {3}), setting viability to false.", 
                        species.Name, site.Location, SiteVars.SeedbankAge[site][species], seedbank_longevity);
                    Seedbank.ClearSeedbankSpecies(site, species);
                }
            }

            //Next, update seedbank based on whether adults are present to disperse seeds to the seedbank
            //get all species in the site that can add to the seedbank
            //TODO check for a faster way to do this
            var maturePresentList = new List<ISpecies>();
            foreach (ISpeciesCohorts cohort in SiteVars.Cohorts[site])
            {
                if (SpeciesData.SeedbankLongevity[cohort.Species] > 0 & cohort.IsMaturePresent) //only for seedbanking species with mature cohorts
                {
                    if (!maturePresentList.Contains(cohort.Species))
                    {
                        maturePresentList.Add(cohort.Species);
                    }
                }
            }
            if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Seedbanking species mature present at site {0}: {1}",
                site.Location, string.Join(", ", maturePresentList.ConvertAll(s => s.Name)));

            //loop through all mature seedbanking species at the site 
            //TODO combine with loop above to save looping through cohorts twice?
            foreach (ISpecies species in maturePresentList)
            {
                if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Adding seeds for {0} at site {1}", species.Name, site.Location);
                SiteVars.SeedbankViability[site][species] = true;
                SiteVars.SeedbankAge[site][species] = 0;
            }

            

            // TODO Disperse seeds to neighbors
            //DisperseSeedsToNeighboringSeedbanks(site, species);
            
        }

        //SF maybe use this function in UpdateSeedbank?
        public static void AddSeedbank(ActiveSite site, ISpecies species)
        {
            /*
            // Initialize the dictionary for the species if needed
            if (!SiteVars.SeedbankAge[site].ContainsKey(species))
                SiteVars.SeedbankAge[site][species] = 0;
            if (!SiteVars.SeedbankViability[site].ContainsKey(species))
                SiteVars.SeedbankViability[site][species] = false;
            */
            
            // Set the seedbank age and viability
            SiteVars.SeedbankAge[site][species] = 0;
            SiteVars.SeedbankViability[site][species] = true;
        }

        public static void PostfireGerminate(ActiveSite site)
        {
            foreach (var speciesEntry in SiteVars.SeedbankAge[site])
            {
                ISpecies species = speciesEntry.Key; // Extract the species from the dictionary entry
                if (SiteVars.SeedbankViability[site][species]) //&
                    //Reproduction.SufficientResources(species, site) &
                    //Reproduction.Establish(species, site))
                {
                    PlugIn.ModelCore.UI.WriteLine("Doing a postfire germinate");
                    PlugIn.AddNewCohort(species, site, "seedbank", 1.0); // TODO SF I had to change this to static in PlugIn
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


        public static class SpeciesMapNames
        {
            public const string SpeciesVar = "species";
            public const string TimestepVar = "timestep";

            private static IDictionary<string, bool> knownVars;
            private static IDictionary<string, string> varValues;

            //---------------------------------------------------------------------

            static SpeciesMapNames()
            {
                knownVars = new Dictionary<string, bool>();
                knownVars[SpeciesVar] = true;
                knownVars[TimestepVar] = true;

                varValues = new Dictionary<string, string>();
            }

            //---------------------------------------------------------------------

            public static void CheckTemplateVars(string template)
            {
                OutputPath.CheckTemplateVars(template, knownVars);
            }

            //---------------------------------------------------------------------

            public static string ReplaceTemplateVars(string template,
                                                     string species,
                                                     int timestep)
            {
                varValues[SpeciesVar] = species;
                varValues[TimestepVar] = timestep.ToString();
                return OutputPath.ReplaceTemplateVars(template, varValues);
            }
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
