//  Authors:  Srinivas S., Robert M. Scheller

using Landis.SpatialModeling;

namespace Landis.Extension.Succession.NECN
{
    public class DoublePixel : Pixel
    {
        public Band<double> MapCode  = "The numeric code for each raster cell";

        public DoublePixel()
        {
            SetBands(MapCode);
        }
    }
}
