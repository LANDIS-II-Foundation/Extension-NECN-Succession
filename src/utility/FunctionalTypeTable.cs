//  Authors:  Robert M. Scheller
using Landis.Utilities;


namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Definition of a Litter Type.
    /// </summary>
    public class FunctionalTypeTable

    {

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
            parameters = new FunctionalType[index];
        }

              

    }
}
