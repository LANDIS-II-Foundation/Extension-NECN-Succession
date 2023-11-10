//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.InitialCommunities.Universal;
using System.Collections.Generic;
using Landis.Library.UniversalCohorts;
using Landis.Library.Climate;
using System;
//using Landis.Cohorts;


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


        private static IDictionary<uint, List<Landis.Library.UniversalCohorts.ICohort>> mapCodeCohorts;

        //---------------------------------------------------------------------

        static InitialBiomass()
        {
            mapCodeCohorts = new Dictionary<uint, List<Landis.Library.UniversalCohorts.ICohort>>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes this class.
        /// </summary>
        /// <param name="timestep">
        /// The plug-in's timestep.  It is used for growing biomass cohorts.
        /// </param>
        //public static void Initialize(int timestep)
        //{
        //    //successionTimestep = (ushort) timestep;
        //}

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
        public static InitialBiomass Compute(ActiveSite site,
                                             ICommunity initialCommunity)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            if (!ecoregion.Active)
            {
                string mesg = string.Format("Initial community {0} is located on a non-active ecoregion {1}", initialCommunity.MapCode, ecoregion.Name);
                throw new System.ApplicationException(mesg);
            }

            InitialBiomass initialBiomass;

            List<Landis.Library.UniversalCohorts.ICohort> sortedAgeCohorts = SortCohorts(initialCommunity.Cohorts);

            ISiteCohorts cohorts = MakeBiomassCohorts(sortedAgeCohorts, site);
            initialBiomass = new InitialBiomass(cohorts);

            return initialBiomass;
        }

        //---------------------------------------------------------------------
        public static ISiteCohorts MakeBiomassCohorts(List<Landis.Library.UniversalCohorts.ICohort> sortedCohorts, ActiveSite site)
        {

            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            SiteVars.Cohorts[site] = new SiteCohorts();

            foreach (ICohort cohort in sortedCohorts)
            {
                dynamic additionalParameters = cohort.Data;
                //foreach(ICohort cohort in cohorts)
                SiteVars.Cohorts[site].AddNewCohort(cohort.Species, cohort.Data.Age, additionalParameters.WoodBiomass, additionalParameters.LeafBiomass);
            }
            return SiteVars.Cohorts[site];
        }


        public static List<Landis.Library.UniversalCohorts.ICohort> SortCohorts(List<Landis.Library.UniversalCohorts.ISpeciesCohorts> sppCohorts)
        {
            List<Landis.Library.UniversalCohorts.ICohort> cohorts = new List<Landis.Library.UniversalCohorts.ICohort>();
            foreach (Landis.Library.UniversalCohorts.ISpeciesCohorts speciesCohorts in sppCohorts)
            {
                foreach (Landis.Library.UniversalCohorts.ICohort cohort in speciesCohorts)
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

    
