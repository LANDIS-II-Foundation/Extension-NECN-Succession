//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.Succession.NECN_Hydro.AgeOnlyDisturbances
{
    /// <summary>
    /// A table of pool percentages for age-only disturbances.
    /// </summary>
    public interface IPercentageTable
    {
        /// <summary>
        /// Gets the pair of percentages for a particular disturbance type.
        /// </summary>
        PoolPercentages this[ExtensionType disturbanceType]
        {
            get;set;
        }
        
        PoolPercentages Default {get;}
    }

    /// <summary>
    /// A table of pool percentages for age-only disturbances.
    /// </summary>
    public class PercentageTable
        : IPercentageTable
    {
        private IDictionary<ExtensionType, PoolPercentages> percentages;
        private PoolPercentages defaultPercentages;

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the pair of percentages for a particular disturbance type.
        /// </summary>
        public PoolPercentages this[ExtensionType disturbanceType]
        {
            get {
                PoolPercentages poolPercentages;
                //PlugIn.ModelCore.Log.WriteLine("   Trying to acquire pool percentages for {0}.", disturbanceType);
                if (percentages.TryGetValue(disturbanceType, out poolPercentages))
                    return poolPercentages;

                return defaultPercentages;
            }
            set {
                //PoolPercentages poolPercentages;
                percentages[disturbanceType] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The default pair of percentages for disturbance types that do not
        /// have an entry in the table.
        /// </summary>
        public PoolPercentages Default
        {
            get {
                return defaultPercentages;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
/*        public PercentageTable(IDictionary<ExtensionType, PoolPercentages> percentages,
                               PoolPercentages                          defaultPercentages)
        {
            this.percentages = percentages;
            this.defaultPercentages = defaultPercentages;
        }*/
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public PercentageTable()
        {
            this.percentages = new Dictionary<ExtensionType, PoolPercentages>();
            this.defaultPercentages = new PoolPercentages();
        }

    }
}
