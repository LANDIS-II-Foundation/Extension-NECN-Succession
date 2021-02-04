//  Author: Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;

using System;
using System.IO;
using Landis.Library.Climate;

namespace Landis.Extension.Succession.NECN
{
    public class Establishment
    {

        //private static StreamWriter log;
        private static double[,] avgSoilMoisturelimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgMATlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgJanuaryTlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgPest = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        private static int[,] numberCalculations = new int[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        private static double[,] avgDryDays = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        private static double[,] avgBeginGDD = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        private static double[,] avgEndGDD = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];


        public static double Calculate(ISpecies species, ActiveSite site)
        {
            IEcoregion climateRegion = PlugIn.ModelCore.Ecoregion[site];
            
            double tempMultiplier = 0.0;
            double soilMultiplier = 0.0;
            double minJanTempMultiplier = 0.0;
            double establishProbability = 0.0;

            AnnualClimate_Monthly ecoClimate = ClimateRegionData.AnnualWeather[climateRegion];

            if (ecoClimate == null)
                throw new System.ApplicationException("Error in Establishment: CLIMATE NULL.");

            double ecoDryDays = SiteVars.DryDays[site]; 
            soilMultiplier = SoilMoistureMultiplier(ecoClimate, species, ecoDryDays);
            tempMultiplier = BotkinDegreeDayMultiplier(ecoClimate, species);
            minJanTempMultiplier = MinJanuaryTempModifier(ecoClimate, species);

            // Liebig's Law of the Minimum is applied to the four multipliers for each year:
            double minMultiplier = System.Math.Min(tempMultiplier, soilMultiplier);
            minMultiplier = System.Math.Min(minJanTempMultiplier, minMultiplier);

            establishProbability += minMultiplier;
            establishProbability *= PlugIn.ProbEstablishAdjust;

            avgSoilMoisturelimit[species.Index, climateRegion.Index] += soilMultiplier;
            avgMATlimit[species.Index, climateRegion.Index] += tempMultiplier;
            avgJanuaryTlimit[species.Index, climateRegion.Index] += minJanTempMultiplier;
            avgPest[species.Index, climateRegion.Index] += establishProbability;

            avgDryDays[species.Index, climateRegion.Index] += ecoDryDays;
            avgBeginGDD[species.Index, climateRegion.Index] += ecoClimate.BeginGrowing;
            avgEndGDD[species.Index, climateRegion.Index] += ecoClimate.EndGrowing;

            numberCalculations[species.Index, climateRegion.Index]++;

            return establishProbability;
        }

        public static void LogEstablishment()
        {
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                {
                    if (!ecoregion.Active)
                        continue;

                        Outputs.establishmentLog.Clear();
                        EstablishmentLog elog = new EstablishmentLog();

                        elog.Time = PlugIn.ModelCore.CurrentTime;
                        elog.SpeciesName = species.Name;
                        elog.ClimateRegion = ecoregion.Name;
                        elog.NumberAttempts = numberCalculations[species.Index, ecoregion.Index];
                        elog.AvgTempMult = (avgMATlimit[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.AvgMinJanTempMult = (avgJanuaryTlimit[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.AvgSoilMoistureMult = (avgSoilMoisturelimit[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.AvgProbEst = (avgPest[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.DryDays = (avgDryDays[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.BeginGDD = (avgBeginGDD[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);
                        elog.EndGDD = (avgEndGDD[species.Index, ecoregion.Index] / (double)numberCalculations[species.Index, ecoregion.Index]);

                        Outputs.establishmentLog.AddObject(elog);
                        Outputs.establishmentLog.WriteToFile();
                }
            }

        avgSoilMoisturelimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgMATlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgJanuaryTlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgPest = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        numberCalculations = new int[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgDryDays = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgBeginGDD = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgEndGDD = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];


    }


    //---------------------------------------------------------------------------
    private static double SoilMoistureMultiplier(AnnualClimate weather, ISpecies species, double dryDays)

        {
            double sppAllowableDrought = SpeciesData.MaxDrought[species];
            double growDays = 0.0;
            double maxDrought;
            double Soil_Moist_GF = 0.0;

            growDays = weather.EndGrowing - weather.BeginGrowing + 1.0;
            if (growDays < 2.0)
            {
                PlugIn.ModelCore.UI.WriteLine("Begin Grow = {0}, End Grow = {1}", weather.BeginGrowing, weather.EndGrowing);
                throw new System.ApplicationException("Error: Too few growing days.");
            }
            //Calc species soil moisture multipliers
            maxDrought = sppAllowableDrought * growDays;
            
            if (maxDrought < dryDays) 
            {
                Soil_Moist_GF = 0.0;
            }
            else
            {
                Soil_Moist_GF = System.Math.Sqrt((double)(maxDrought - dryDays) / maxDrought);
            }

            //PlugIn.ModelCore.UI.WriteLine("BeginGrow={0}, EndGrow={1}, dryDays={2}, maxDrought={3}", weather.BeginGrowing, weather.EndGrowing, dryDays, maxDrought);

            return Soil_Moist_GF;
        }
        
        //---------------------------------------------------------------------------
        private static double BotkinDegreeDayMultiplier(AnnualClimate weather, ISpecies species)
        {

            //Calc species degree day multipliers  
            //Botkin et al. 1972. J. Ecol. 60:849 - 87
            
            double max_Grow_Deg_Days = SpeciesData.GDDmax[species]; 
            double min_Grow_Deg_Days = SpeciesData.GDDmin[species];
            
            double Deg_Day_GF = 0.0;
            double Deg_Days = (double) weather.GrowingDegreeDays; 
            double totalGDD = max_Grow_Deg_Days - min_Grow_Deg_Days;
            
            Deg_Day_GF = (4.0 * (Deg_Days - min_Grow_Deg_Days) * 
                  (max_Grow_Deg_Days - Deg_Days)) / (totalGDD * totalGDD);
            
           if (Deg_Day_GF < 0) Deg_Day_GF = 0.0;
           //PlugIn.ModelCore.UI.WriteLine("SppMaxDD={0:0.00}, sppMinGDD={1:0.0}, actualGDD={2:0}, gddM={3:0.00}.", max_Grow_Deg_Days, min_Grow_Deg_Days, Deg_Days, Deg_Day_GF);
           
           return Deg_Day_GF;
        }
        
        //---------------------------------------------------------------------------
        private static double MinJanuaryTempModifier(AnnualClimate_Monthly weather, ISpecies species)
        // Is the January mean temperature greater than the species specified minimum?
        {
        
            int speciesMinimum = SpeciesData.MinJanTemp[species];
            
            if (weather.MonthlyTemp[0] < speciesMinimum)
                return 0.0;
            else
                return 1.0;
        }
        
       
        
    }
}
