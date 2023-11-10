using Landis.Library.UniversalCohorts;
using Landis.SpatialModeling;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Wraps a biomass-cohorts site variable and provides access to it as a
    /// site variable of base cohorts.
    /// </summary>
    public class BaseCohortsSiteVar
        : ISiteVar<ISiteCohorts>
    {
        private ISiteVar<ISiteCohorts> biomassCohortSiteVar;

        public BaseCohortsSiteVar(ISiteVar<ISiteCohorts> siteVar)
        {
            biomassCohortSiteVar = siteVar;
        }

        #region ISiteVariable members
        System.Type ISiteVariable.DataType
        {
            get
            {
                return typeof(SiteCohorts);
            }
        }

        InactiveSiteMode ISiteVariable.Mode
        {
            get
            {
                return biomassCohortSiteVar.Mode;
            }
        }

        ILandscape ISiteVariable.Landscape
        {
            get
            {
                return biomassCohortSiteVar.Landscape;
            }
        }
        #endregion

        #region ISiteVar<BaseCohorts.ISiteCohorts> members
        // Extensions other than succession have no need to assign the whole
        // site-cohorts object at any site.


        ISiteCohorts ISiteVar<ISiteCohorts>.this[Site site]
        {
            get
            {
                return (ISiteCohorts) biomassCohortSiteVar[site]; 
            }
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        ISiteCohorts ISiteVar<ISiteCohorts>.ActiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        ISiteCohorts ISiteVar<ISiteCohorts>.InactiveSiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }

        ISiteCohorts ISiteVar<ISiteCohorts>.SiteValues
        {
            set
            {
                throw new System.InvalidOperationException("Operation restricted to succession extension");
            }
        }
        #endregion
    }
}
