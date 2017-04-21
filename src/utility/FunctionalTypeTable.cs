//  Copyright 2008 Conservation Biology Institute
//  Authors:  Robert M. Scheller
//  License:  Available at  
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Definition of a Litter Type.
    /// </summary>
    public class FunctionalTypeTable
        //: IEditable<IFunctionalType[]>
    {

        //private IEditableFunctionalType[] parameters;
        private FunctionalType[] parameters;

       //---------------------------------------------------------------------
        public int Count
        {
            get {
                return parameters.Length;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The parameters for a functional type
        /// </summary>
        public FunctionalType this[int index]
        //public IEditableFunctionalType this[int index]
        {
            get {
                return parameters[index];
            }

            set {
                parameters[index] = value;
            }
        }
        
        
        //---------------------------------------------------------------------

        public FunctionalTypeTable(int index)
        {
            //parameters = new IEditableFunctionalType[index];
            parameters = new FunctionalType[index];
        }

              

    }
}
