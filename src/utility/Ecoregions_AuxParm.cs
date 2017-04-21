//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;

namespace Landis.Ecoregions
{
    /// <summary>
    /// An auxiliary parameter for ecoregions.
    /// </summary>
    public class AuxParm<T>
    {
        private T[] values;

        //---------------------------------------------------------------------

        public T this[IEcoregion ecoregion]
        {
            get {
                return values[ecoregion.Index];
            }

            set {
                values[ecoregion.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        public AuxParm(IEcoregionDataset ecoregions)
        {
            values = new T[ecoregions.Count];
        }
    }
}
