//  Author: Robert Scheller

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;

using System;
using System.IO;
using Landis.Library.Climate;

namespace Landis.Extension.Succession.NECN_Hydro
{
    public class Establishment
    {

        private static StreamWriter log;
        private static double[,] avgSoilMoisturelimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgMATlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgJanuaryTlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 
        private static double[,] avgPest = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count]; 


        public static void InitializeLogFile()
        {
            string logFileName   = "NECN_Hydro-prob-establish-log.csv"; 
            PlugIn.ModelCore.UI.WriteLine("   Opening a NECN_Hydro log file \"{0}\" ...", logFileName);
            try {
                log = Landis.Data.CreateTextFile(logFileName);
            }
            catch (Exception err) {
                string mesg = string.Format("{0}", err.Message);
                throw new System.ApplicationException(mesg);
            }
            
            log.AutoFlush = true;
            log.WriteLine("Time, Species, ClimateRegion, AvgTempMult, AvgMinJanTempMult, AvgSoilMoistureMult, AvgProbEst");
        }

        public static double Calculate(ISpecies species, ActiveSite site)//, int years)
        {
            IEcoregion climateRegion = PlugIn.ModelCore.Ecoregion[site];

            double tempMultiplier = 0.0;
            double soilMultiplier = 0.0;
            double minJanTempMultiplier = 0.0;
            double establishProbability = 0.0;

            AnnualClimate_Monthly ecoClimate = ClimateRegionData.AnnualWeather[climateRegion];

            if (ecoClimate == null)
                throw new System.ApplicationException("Error in Establishment: CLIMATE NULL.");

            double ecoDryDays = SiteVars.DryDays[site]; // CalculateSoilMoisture(ecoClimate, climateRegion, ecoClimate.Year);
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

            return establishProbability;
        }

        public static void LogEstablishment()
        {
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
                {
                    if (!ecoregion.Active || ClimateRegionData.ActiveSiteCount[ecoregion] < 1)
                        continue;

                        log.Write("{0}, {1}, {2},", PlugIn.ModelCore.CurrentTime, species.Name, ecoregion.Name);
                        log.Write("{0:0.00},", (avgMATlimit[species.Index, ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]));
                        log.Write("{0:0.00},", (avgJanuaryTlimit[species.Index, ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]));
                        log.Write("{0:0.00},", (avgSoilMoisturelimit[species.Index, ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]));
                        log.WriteLine("{0:0.00}", (avgPest[species.Index, ecoregion.Index] / (double)ClimateRegionData.ActiveSiteCount[ecoregion]));
                }
            }

        avgSoilMoisturelimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgMATlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgJanuaryTlimit = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];
        avgPest = new double[PlugIn.ModelCore.Species.Count, PlugIn.ModelCore.Ecoregions.Count];


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
        
        //---------------------------------------------------------------------------
        private static double CalculateSoilMoisture(AnnualClimate_Monthly weather, IEcoregion ecoregion, int year, ActiveSite site)
        // Calculate fraction of growing season with unfavorable soil moisture
        // for growth (Dry_Days_in_Grow_Seas) used in SoilMoistureMultiplier to determine soil
        // moisture growth multipliers.
        //
        // Simulates method of Thorthwaite and Mather (1957) as modified by Pastor and Post (1984).
        //
        // This method necessary to estimate annual soil moisture at the ECOREGION scale, whereas
        // the SiteVar AvailableWater exists at the site level and is updated monthly.
        
        //field_cap = centimeters of water the soil can hold at field capacity
        //field_dry = centimeters of water below which tree growth stops
        //            (-15 bars)
        // NOTE:  Because the original LINKAGES calculations were based on a 100 cm rooting depth, 
        // 100 cm are used here, although soil depth may be given differently for Century 
        // calculations.
        
        //beg_grow_seas = year day on which the growing season begins
        //end_grow_seas = year day on which the growing season ends
        //latitude = latitude of region (degrees north)

        {

            double  xFieldCap,            //
            waterAvail,           //
            aExponentET,        //
            oldWaterAvail,      //
            monthlyRain,         //
            potWaterLoss,       //
            potentialET,
            tempFac,             //
            xAccPotWaterLoss, //
            changeSoilMoisture, //
            oldJulianDay,       //
            dryDayInterp;       //
            double fieldCapacity = SiteVars.SoilFieldCapacity[site] * (double)SiteVars.SoilDepth[site]; // ClimateRegionData.SoilDepth[ecoregion];
            double wiltingPoint = SiteVars.SoilWiltingPoint[site] * (double)SiteVars.SoilDepth[site]; // ClimateRegionData.SoilDepth[ecoregion];
            //double fieldCapacity = ClimateRegionData.FieldCapacity[ecoregion] * 100.0;
            //double wiltingPoint  = ClimateRegionData.WiltingPoint[ecoregion] * 100.0;
            
            //Initialize water content of soil in January to Field_Cap (mm)
            xFieldCap = 10.0 * fieldCapacity;
            waterAvail = fieldCapacity;


            //Initialize Thornwaithe parameters:
            //
            //TE = temperature efficiency
            //aExponentET = exponent of evapotranspiration function
            //pot_et = potential evapotranspiration
            //aet = actual evapotranspiration
            //acc_pot_water_loss = accumulated potential water loss
    
            double actualET = 0.0;
            double accPotWaterLoss = 0.0;
            double tempEfficiency = 0.0;
  
            for (int i = 0; i < 12; i++) 
            {
                tempFac = 0.2 * weather.MonthlyTemp[i];
      
                if (tempFac > 0.0)
                    tempEfficiency += System.Math.Pow(tempFac, 1.514);
            }
    
            aExponentET = 0.675 * System.Math.Pow(tempEfficiency, 3) - 
                            77.1 * (tempEfficiency * tempEfficiency) +
                            17920.0 * tempEfficiency + 492390.0;
            aExponentET *= (0.000001);
    
            //Initialize the number of dry days and current day of year
            int dryDays = 0;
            double julianDay = 15.0;
            double annualPotentialET = 0.0;
    
    
            for (int i = 0; i < 12; i++) 
            {
                double daysInMonth = AnnualClimate.DaysInMonth(i, year);
                oldWaterAvail = waterAvail;
                monthlyRain = weather.MonthlyPrecip[i];
                tempFac = 10.0 * weather.MonthlyTemp[i];
                
                //Calc potential evapotranspiriation (potentialET) Thornwaite and Mather,
                //1957.  Climatology 10:83 - 311.
                if (tempFac > 0.0) 
                {

                    potentialET = 1.6 * (System.Math.Pow((tempFac / tempEfficiency), aExponentET)) * 
                            AnnualClimate.LatitudeCorrection(i, PlugIn.Parameters.Latitude); //ClimateRegionData.Latitude[ecoregion]);
                } 
                else 
                {
                    potentialET = 0.0;
                }
                
                annualPotentialET += potentialET;
      
                //Calc potential water loss this month
                potWaterLoss = monthlyRain - potentialET;
      
                //If monthlyRain doesn't satisfy potentialET, add this month's potential
                //water loss to accumulated water loss from soil
                if (potWaterLoss < 0.0) 
                {
                    accPotWaterLoss += potWaterLoss;
                    xAccPotWaterLoss = accPotWaterLoss * 10;
      
                    //Calc water retained in soil given so much accumulated potential
                    //water loss Pastor and Post. 1984.  Can. J. For. Res. 14:466:467.
      
                    waterAvail = fieldCapacity * 
                                 System.Math.Exp((.000461 - 1.10559 / xFieldCap) * (-1.0 * xAccPotWaterLoss));
      
                    if (waterAvail < 0.0)
                        waterAvail = 0.0;
      
                    //changeSoilMoisture - during this month
                    changeSoilMoisture = waterAvail - oldWaterAvail;
      
                    //Calc actual evapotranspiration (AET) if soil water is drawn down
                    actualET += (monthlyRain - changeSoilMoisture);
                } 

                //If monthlyRain satisfies potentialET, don't draw down soil water
                else 
                {
                    waterAvail = oldWaterAvail + potWaterLoss;
                    if (waterAvail >= fieldCapacity)
                        waterAvail = fieldCapacity;
                    changeSoilMoisture = waterAvail - oldWaterAvail;
 
                    //If soil partially recharged, reduce accumulated potential
                    //water loss accordingly
                    accPotWaterLoss += changeSoilMoisture;
      
                    //If soil completely recharged, reset accumulated potential
                    //water loss to zero
                    if (waterAvail >= fieldCapacity)
                        accPotWaterLoss = 0.0;
      
                    //If soil water is not drawn upon, add potentialET to AET
                    actualET += potentialET;
                }
      
                oldJulianDay = julianDay;
                julianDay += daysInMonth;
                dryDayInterp = 0.0;

                //Increment number of dry days, truncate
                //at end of growing season
                if ((julianDay > weather.BeginGrowing) && (oldJulianDay < weather.EndGrowing)) 
                {
                    if ((oldWaterAvail >= wiltingPoint)  && (waterAvail >= wiltingPoint))
                    {
                        dryDayInterp += 0.0;  // NONE below wilting point
                    }
                    else if ((oldWaterAvail > wiltingPoint) && (waterAvail < wiltingPoint)) 
                    {
                        dryDayInterp = daysInMonth * (wiltingPoint - waterAvail) / 
                                        (oldWaterAvail - waterAvail);
                        if ((oldJulianDay < weather.BeginGrowing) && (julianDay > weather.BeginGrowing))
                            if ((julianDay - weather.BeginGrowing) < dryDayInterp)
                                dryDayInterp = julianDay - weather.BeginGrowing;
    
                        if ((oldJulianDay < weather.EndGrowing) && (julianDay > weather.EndGrowing))
                            dryDayInterp = weather.EndGrowing - julianDay + dryDayInterp;
    
                        if (dryDayInterp < 0.0)
                            dryDayInterp = 0.0;
    
                    } 
                    else if ((oldWaterAvail < wiltingPoint) && (waterAvail > wiltingPoint)) 
                    {
                        dryDayInterp = daysInMonth * (wiltingPoint - oldWaterAvail) / 
                                        (waterAvail - oldWaterAvail);
          
                        if ((oldJulianDay < weather.BeginGrowing) && (julianDay > weather.BeginGrowing))
                            dryDayInterp = oldJulianDay + dryDayInterp - weather.BeginGrowing;
    
                        if (dryDayInterp < 0.0)
                            dryDayInterp = 0.0;
    
                        if ((oldJulianDay < weather.EndGrowing) && (julianDay > weather.EndGrowing))
                            if ((weather.EndGrowing - oldJulianDay) < dryDayInterp)
                                dryDayInterp = weather.EndGrowing - oldJulianDay;
                    } 
                    else // ALL below wilting point
                    {
                        dryDayInterp = daysInMonth;
          
                        if ((oldJulianDay < weather.BeginGrowing) && (julianDay > weather.BeginGrowing))
                            dryDayInterp = julianDay - weather.BeginGrowing;
    
                        if ((oldJulianDay < weather.EndGrowing) && (julianDay > weather.EndGrowing))
                            dryDayInterp = weather.EndGrowing - oldJulianDay;
                    }
      
                    dryDays += (int) dryDayInterp;
                }
            }  //END MONTHLY CALCULATIONS
  
            //Convert AET from cm to mm
            //actualET *= 10.0;

            //Calculate AET multiplier
            //(used to be done in decomp)
            //float aetMf = min((double)AET,600.0);
            //AET_Mult = (-1. * aetMf) / (-1200. + aetMf);
            
            return dryDays;
        }
        //---------------------------------------------------------------------

        //private static Species.AuxParm<Ecoregions.AuxParm<double>> CreateSpeciesEcoregionParm<T>(ISpeciesDataset speciesDataset, IEcoregionDataset ecoregionDataset)
        //{
        //    Species.AuxParm<Ecoregions.AuxParm<double>> newParm;
        //    newParm = new Species.AuxParm<Ecoregions.AuxParm<double>>(speciesDataset);
        //    foreach (ISpecies species in speciesDataset) {
        //        newParm[species] = new Ecoregions.AuxParm<double>(ecoregionDataset);
        //    }
        //    return newParm;
        //}
        
        
    }
}
