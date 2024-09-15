//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.InitialCommunities.Universal;
using System.Collections.Generic;
using Landis.Library.UniversalCohorts;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// The initial live and dead biomass at a site.
    /// </summary>
    public class InitialBiomass
    {
        private ISiteCohorts cohorts;


        //---------------------------------------------------------------------

        /// <summary>
        /// The site's initial cohorts.
        /// </summary>
        public ISiteCohorts Cohorts
        {
            get
            {
                return cohorts;
            }
        }


        //---------------------------------------------------------------------

        private InitialBiomass(ISiteCohorts cohorts)
        {
            this.cohorts = cohorts;

        }


        private static IDictionary<uint, List<ICohort>> mapCodeCohorts;

        //---------------------------------------------------------------------

        static InitialBiomass()
        {
            mapCodeCohorts = new Dictionary<uint, List<ICohort>>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the initial biomass at a site.
        /// </summary>
        /// <param name="site">
        /// The selected site.
        /// </param>
        /// <param name="initialCommunity">
        /// The initial community of age cohorts at the site.
        /// </param>
        public static InitialBiomass Compute(ActiveSite site, ICommunity initialCommunity)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            if (!ecoregion.Active)
            {
                string mesg = string.Format("Initial community {0} is located on a non-active ecoregion {1}", initialCommunity.MapCode, ecoregion.Name);
                throw new System.ApplicationException(mesg);
            }

            InitialBiomass initialBiomass;

            List<ICohort> sortedAgeCohorts = SortCohorts(initialCommunity.Cohorts);

            ISiteCohorts cohorts = MakeBiomassCohorts(sortedAgeCohorts, site);
            initialBiomass = new InitialBiomass(cohorts);

            return initialBiomass;
        }

        //---------------------------------------------------------------------
        public static ISiteCohorts MakeBiomassCohorts(List<ICohort> sortedCohorts, ActiveSite site)
        {

            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            SiteVars.Cohorts[site] = new SiteCohorts();

            foreach (ICohort cohort in sortedCohorts)
            {
                SiteVars.Cohorts[site].AddNewCohort(cohort.Species, cohort.Data.Age, cohort.Data.Biomass, 0, cohort.Data.AdditionalParameters);
            }
            return SiteVars.Cohorts[site];
        }


        public static List<ICohort> SortCohorts(List<ISpeciesCohorts> sppCohorts)
        {
            List<ICohort> cohorts = new List<ICohort>();
            foreach (ISpeciesCohorts speciesCohorts in sppCohorts)
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    cohorts.Add(cohort);
                    //PlugIn.ModelCore.UI.WriteLine("ADDED:  {0} {1}.", cohort.Species.Name, cohort.Age);
                }
            }
            cohorts.Sort(WhichIsOlderCohort);
            return cohorts;
        }

        private static int WhichIsOlderCohort(ICohort x, ICohort y)
        {
            return WhichIsOlder(x.Data.Age, y.Data.Age);
        }

        private static int WhichIsOlder(ushort x, ushort y)
        {
            return y - x;
        }
    }
}
    
        //---------------------------------------------------------------------

    
