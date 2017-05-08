//  Copyright 2007-2016 Portland State University
//  Author: Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;
using Edu.Wisc.Forest.Flel.Util;
using System.IO;

namespace Landis.Extension.Succession.NECN_Hydro
{
    /// <summary>
    /// Utility methods.
    /// </summary>
    public static class Util
    {

        /// <summary>
        /// Converts a table indexed by species and ecoregion into a
        /// 2-dimensional array.
        /// </summary>
        public static T[,] ToArray<T>(Species.AuxParm<Ecoregions.AuxParm<T>> table)
        {
            T[,] array = new T[PlugIn.ModelCore.Ecoregions.Count, PlugIn.ModelCore.Species.Count];
            foreach (ISpecies species in PlugIn.ModelCore.Species) {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions) {
                    array[ecoregion.Index, species.Index] = table[species][ecoregion];
                }
            }
            return array;
        }
        //---------------------------------------------------------------------

        public static void ReadSoilDepthMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    int mapValue = (int) pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                            if (mapValue < 1 || mapValue > 300)
                                throw new InputValueException(mapValue.ToString(),
                                                              "Soil depth value {0} is not between {1:0.0} and {2:0.0}",
                                                              mapValue, 1, 300);
                        SiteVars.SoilDepth[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilDrainMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "SOil drainage value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        SiteVars.SoilDrain[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilBaseFlowMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "Soil base flow value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        SiteVars.SoilBaseFlowFraction[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadSoilStormFlowMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "Soil storm flow value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        SiteVars.SoilStormFlowFraction[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadFieldCapacityMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map)
            {
                DoublePixel pixel = map.BufferPixel;
                foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    double mapValue = pixel.MapCode.Value;
                    if (site.IsActive)
                    {
                        if (mapValue < 0.0 || mapValue > 0.5)
                            throw new InputValueException(mapValue.ToString(),
                                                          "Soil fild capacity value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 0.5);
                        SiteVars.SoilFieldCapacity[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadWiltingPointMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "Soil field capacity value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        if (mapValue > SiteVars.SoilFieldCapacity[site])
                            throw new InputValueException(mapValue.ToString(),
                                                          "Wilting Point {0} is greater than field capacity {1:0.0} at this site",
                                                          mapValue, SiteVars.SoilFieldCapacity[site]);
                        SiteVars.SoilWiltingPoint[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadPercentSandMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "Soil percent sand value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        SiteVars.SoilPercentSand[site] = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public static void ReadPercentClayMap(string path)
        {
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
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

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
                                                          "Soil percent clay value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1.0);
                        SiteVars.SoilPercentClay[site] = mapValue;
                    }
                }
            }
            //foreach (Site site in PlugIn.ModelCore.Landscape.AllSites)
            //{
            //    SiteVars.SoilPercentClay[site] = 0.069;
            //}
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
                        if (mapValue < 0.0 || mapValue > 10000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1surf C value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 10000.0);
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
                        if (mapValue < 0.0 || mapValue > 500.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1surf N value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 500.0);
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
                        if (mapValue < 0.0 || mapValue > 10000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1C value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 10000.0);
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
                        if (mapValue < 0.0 || mapValue > 500.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM1N value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 500.0);
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
                        if (mapValue < 0.0 || mapValue > 20000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM2C value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 20000.0);
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
                        if (mapValue < 0.0 || mapValue > 1000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM2N value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1000.0);
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
                        if (mapValue < 0.0 || mapValue > 30000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOM3C value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 20000.0);
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
                        if (mapValue < 0.0 || mapValue > 1000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SOm3N value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 1000.0);
                        SiteVars.SOM3[site].Nitrogen = mapValue;
                    }
                }
            }
        }
        //---------------------------------------------------------------------
        private static IInputRaster<DoublePixel> MakeDoubleMap(string path)
        {
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
                        if (mapValue < 0.0 || mapValue > 50000.0)
                            throw new InputValueException(mapValue.ToString(),
                                                          "SurfDeadWood value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 50000.0);
                        SiteVars.SurfaceDeadWood[site].Carbon = mapValue * 0.47;
                        SiteVars.SurfaceDeadWood[site].Nitrogen = mapValue * 0.47 / 200.0;  // 200 is a generic wood CN ratio

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
                                                          "SoilDeadWood value {0} is not between {1:0.0} and {2:0.0}",
                                                          mapValue, 0.0, 50000.0);
                        SiteVars.SoilDeadWood[site].Carbon = mapValue * 0.47;
                        SiteVars.SoilDeadWood[site].Nitrogen = mapValue * 0.47 / 200.0;  // 200 is a generic wood CN ratio
                    }
                }
            }
        }
        //---------------------------------------------------------------------
    }
}
