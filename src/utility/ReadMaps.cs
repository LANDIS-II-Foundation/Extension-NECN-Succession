//  Author: Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using Landis.Utilities;
using System.IO;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class ReadMaps
    {

        //---------------------------------------------------------------------

        public static void ReadSoilDepthMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

 
            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    int mapValue = (int) pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                            if (mapValue <= 0 || mapValue > 300)
                                throw new InputValueException(mapValue.ToString(),
                                                              "Soil depth value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                              mapValue, 0, 300, site.Location.Row, site.Location.Column);
                        SiteVars.SoilDepth[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilDrainMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 1.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOil drainage value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilDrain[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilBaseFlowMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 1.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil base flow value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilBaseFlowFraction[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilStormFlowMap(string path)
        {
            if(PlugIn.StormFlowOverride > 0.0)
            {
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    SiteVars.SoilStormFlowFraction[site] = PlugIn.StormFlowOverride;
                }
                return;
            }
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 1.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil storm flow value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilStormFlowFraction[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadFieldCapacityMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path); 

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0001 || mapValue > 0.75)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil field capacity value {0} is not between {1:0.0} and {2:0.00}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.00001, 0.75, site.Location.Row, site.Location.Column);
                        SiteVars.SoilFieldCapacity[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadWiltingPointMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path); 

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 0.75)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil wilting point value {0} is not between {1:0.0} and {2:0.00}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 0.75, site.Location.Row, site.Location.Column);
                        if (mapValue > SiteVars.SoilFieldCapacity[site])
                            throw new InputValueException(mapValue.ToString(),
                                                          "Wilting Point {0} is greater than field capacity {1:0.0}.  Site_Row={2:0}, Site_Column={3:0}",
                                                          mapValue, SiteVars.SoilFieldCapacity[site], site.Location.Row, site.Location.Column);
                        SiteVars.SoilWiltingPoint[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadPercentSandMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path); 

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 1.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil percent sand value {0} is not between {1:0.0} and {2:0.0}.  Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilPercentSand[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadPercentClayMap(string path)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 1.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil percent clay value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilPercentClay[site] = mapValue;
                    }
                }
            }
        }
        public static void ReadSoilCNMaps(string path, string path2, string path3, string path4, string path5, string path6, string path7, string path8)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(path);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 0.0 || mapValue > 10000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1surf C value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 10000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM1surface[site].Carbon = mapValue;
                    }
                }
            }
            
            map = MakeDoubleMap(path2);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 0.0 || mapValue > 500.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1surf N value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 500.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM1surface[site].Nitrogen = mapValue;
                    }
                }
            }
            
            map = MakeDoubleMap(path3);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 1.0 || mapValue > 15000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1C value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 1.0, 15000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM1soil[site].Carbon = mapValue;
                    }
                }
            }

            map = MakeDoubleMap(path4);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 0.0 || mapValue > 500.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1N value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 500.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM1soil[site].Nitrogen = mapValue;
                    }
                }
            }
            map = MakeDoubleMap(path5);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 1.0 || mapValue > 25000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM2C value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 1.0, 25000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM2[site].Carbon = mapValue;
                    }
                }
            }

            map = MakeDoubleMap(path6);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 0.0 || mapValue > 1000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM2N value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM2[site].Nitrogen = mapValue;
                    }
                }
            }
            map = MakeDoubleMap(path7);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 1.0 || mapValue > 30000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM3C value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 1.0, 20000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM3[site].Carbon = mapValue;
                    }
                }
            }

            map = MakeDoubleMap(path8);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue <= 0.0 || mapValue > 1000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM3N value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 1000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SOM3[site].Nitrogen = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private static IInputRaster<DoublePixel> MakeDoubleMap(string path)
        {
            //PlugIn.ModelCore.UI.WriteLine("  Read in data from {0}", path);

            IInputRaster<DoublePixel> map;

            try
            {
                map = PlugIn.ModelCore.OpenRaster<DoublePixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != PlugIn.ModelCore.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the scenario ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            return map;
        }
        //---------------------------------------------------------------------

        public static void ReadDeadWoodMaps(string surfacePath, string soilPath)
        {
            IInputRaster<DoublePixel> map = MakeDoubleMap(surfacePath);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 75000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SurfDeadWood value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 75000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SurfaceDeadWood[site].Carbon = mapValue * 0.47;
                        SiteVars.SurfaceDeadWood[site].Nitrogen = mapValue * 0.47 / 200.0;  // 200 is a generic wood CN ratio
                        SiteVars.SurfaceStructural[site].Carbon = SiteVars.SurfaceDeadWood[site].Carbon * 0.85 * PlugIn.Parameters.InitialFineFuels;
                        SiteVars.SurfaceStructural[site].Nitrogen = SiteVars.SurfaceStructural[site].Carbon / OtherData.StructuralCN;
                        SiteVars.SurfaceMetabolic[site].Carbon = SiteVars.SurfaceDeadWood[site].Carbon * 0.15 * PlugIn.Parameters.InitialFineFuels;
                        SiteVars.SurfaceMetabolic[site].Nitrogen = SiteVars.SurfaceMetabolic[site].Carbon / 10;  // a generic metabolic CN ratio

                    }
                }
            }

            map = MakeDoubleMap(soilPath);

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 50000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SoilDeadWood value {0} is not between {1:0.0} and {2:0.0}. Site_Row={3:0}, Site_Column={4:0}",
                                                          mapValue, 0.0, 50000.0, site.Location.Row, site.Location.Column);
                        SiteVars.SoilDeadWood[site].Carbon = mapValue * 0.47;
                        SiteVars.SoilDeadWood[site].Nitrogen = mapValue * 0.47 / 200.0;  // 200 is a generic wood CN ratio
                    }
                }
            }
        }
        //---------------------------------------------------------------------
    }
}
