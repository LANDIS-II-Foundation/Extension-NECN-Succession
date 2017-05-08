//  Copyright 2009 Conservation Biology Institute
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

namespace Landis.Extension.Succession.NECN_Hydro.AgeOnlyDisturbances
{
    /// <summary>
    /// A collection of biomass parameters for age-only disturbances.
    /// </summary>
    public class ParameterDataset
        : IParameterDataset
    {
        private IPercentageTable cohortReductions;
        private IPercentageTable poolReductions;

        //---------------------------------------------------------------------

        /// <summary>
        /// A table of percentages that a disturbance reduce a cohort's
        /// inputs to the dead pools.
        /// </summary>
        public IPercentageTable CohortReductions
        {
            get {
                return cohortReductions;
            }
            set {
                cohortReductions = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// A table of percentages that a disturbances reduces a site's dead
        /// pools.
        /// </summary>
        public IPercentageTable PoolReductions
        {
            get {
                return poolReductions;
            }
            set {
                poolReductions = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
/*        public ParameterDataset(IPercentageTable cohortReductions,
                                IPercentageTable poolReductions)
        {
            this.cohortReductions = cohortReductions;
            this.poolReductions = poolReductions;
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ParameterDataset()
        {
            cohortReductions = new PercentageTable();
            poolReductions = new PercentageTable();
        }
    }
}
