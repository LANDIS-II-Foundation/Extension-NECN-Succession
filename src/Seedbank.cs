using Landis.Core;
using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;
using Landis.Utilities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Landis.Library.Succession;
using Landis.Library.Succession.DemographicSeeding;

namespace Landis.Extension.Succession.NECN
{
    class Seedbank
    {
        public static void UpdateSeedbank(ActiveSite site)
        {
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
                
                SiteVars.SeedbankAge[site][species] += PlugIn.Parameters.Timestep; ; //increment seedbank age by timestep length

                //if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Seedbank age for {0} at site {1}: {2}", species.Name, site.Location, SiteVars.SeedbankAge[site][species]);

                if (SiteVars.SeedbankAge[site][species] > seedbank_longevity)
                {
                    SiteVars.SeedbankViability[site][species] = false;
                    //if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Seedbank age for {0} at site {1} exceeds longevity ({2} > {3}), setting viability to false.", 
                    //    species.Name, site.Location, SiteVars.SeedbankAge[site][species], seedbank_longevity);
                    Seedbank.ClearSeedbankSpecies(site, species);
                }
            }

            //Next, update seedbank based on whether adults are present to disperse seeds to the seedbank
            //get all species in the site that can add to the seedbank
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
            foreach (ISpecies species in maturePresentList)
            {
                //if (OtherData.CalibrateMode) PlugIn.ModelCore.UI.WriteLine("Adding seeds for {0} at site {1}", species.Name, site.Location);
                SiteVars.SeedbankViability[site][species] = true;
                SiteVars.SeedbankAge[site][species] = 0;
            }

        }

        public static void PostfireGerminate(ActiveSite site)
        {
            foreach (var speciesEntry in SiteVars.SeedbankAge[site])
            {
                ISpecies species = speciesEntry.Key; // Extract the species from the dictionary entry
                if (SiteVars.SeedbankViability[site][species])
                {
                    // Only allow germination if time since fire exceeds species sexual maturity
                    int timeSincePreviousFire = PlugIn.ModelCore.CurrentTime - SiteVars.PreviousFireYear[site];
                    int sexualMaturity = species.Maturity;

                    double maturityScalar = SpeciesData.SeedbankMaturityMultiplier[species];

                    if (timeSincePreviousFire < sexualMaturity * maturityScalar)
                    {
                        //PlugIn.ModelCore.UI.WriteLine("   Seedbank germination blocked for {0} at site {1}: Time since fire ({2}) < Sexual maturity ({3})",
                        //    species.Name, site.Location, timeSincePreviousFire, sexualMaturity);
                        continue; // Skip germination for this species
                    }

                    bool sufficientResources = Reproduction.SufficientResources(species, site);
                    bool canEstablish = Reproduction.Establish(species, site);
                    //double siteLAI = SiteVars.LAI[site];

                    //PlugIn.ModelCore.UI.WriteLine($"Species: {species.Name}, Site LAI: {siteLAI:F2}, SufficientResources: {sufficientResources}, Establish: {canEstablish}");

                    if (sufficientResources && canEstablish)
                    {
                        PlugIn.AddNewCohort(species, site, "seedbank", 1.0);
                    }
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
    }
}
