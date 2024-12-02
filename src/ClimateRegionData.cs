//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Climate;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Landis.Extension.Succession.NECN
{
    public class ClimateRegionData
    {

        public static Landis.Library.Parameters.Ecoregions.AuxParm<int> ActiveSiteCount;
        public static Landis.Library.Parameters.Ecoregions.AuxParm<double> AnnualNDeposition;    
        public static Landis.Library.Parameters.Ecoregions.AuxParm<double[]> MonthlyNDeposition; 
        //public static Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate_Monthly> AnnualWeather;

        public static Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate> AnnualClimate;
        public static Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate> AnnualClimateSpinup;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            ActiveSiteCount = new Landis.Library.Parameters.Ecoregions.AuxParm<int>(PlugIn.ModelCore.Ecoregions);
            AnnualClimate = new Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate>(PlugIn.ModelCore.Ecoregions);
            AnnualClimateSpinup = new Landis.Library.Parameters.Ecoregions.AuxParm<AnnualClimate>(PlugIn.ModelCore.Ecoregions);
            MonthlyNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double[]>(PlugIn.ModelCore.Ecoregions);
            AnnualNDeposition = new Landis.Library.Parameters.Ecoregions.AuxParm<double>(PlugIn.ModelCore.Ecoregions);
            
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];
                ActiveSiteCount[ecoregion]++;
            }

            foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            {
                MonthlyNDeposition[ecoregion] = new double[12];

                //if (ecoregion.Active)
                //{
                //    Climate.GenerateEcoregionClimateData(ecoregion, 0, PlugIn.Parameters.Latitude); 
                //    SetSingleAnnualClimate(ecoregion, 0, Climate.Phase.SpinUp_Climate);  // Some placeholder data to get things started.
                //}
            }

            // generate all climate data for all ecoregions 
            Climate.GenerateEcoregionClimateData(PlugIn.Parameters.Latitude);

            // grab the first year's spinup climate
            foreach (var ecoregion in PlugIn.ModelCore.Ecoregions.Where(x => x.Active))
            {
                AnnualClimateSpinup[ecoregion] = Climate.SpinupEcoregionYearClimate[ecoregion.Index][1];      // Climate data year index is 1-based
            }
        }

        ////---------------------------------------------------------------------
        //// Generates new climate parameters for a SINGLE ECOREGION at an annual time step.
        //public static void SetSingleAnnualClimate(IEcoregion ecoregion, int year, Climate.Phase spinupOrfuture)
        //{
        //    int actualYear = Climate.Future_MonthlyData.Keys.Min() + year;

        //    if (spinupOrfuture == Climate.Phase.Future_Climate)
        //    {
        //        //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
        //        if (Climate.Future_MonthlyData.ContainsKey(actualYear))
        //        {
        //            AnnualWeather[ecoregion] = Climate.Future_MonthlyData[actualYear][ecoregion.Index];
        //        }
        //        //else
        //        //    PlugIn.ModelCore.UI.WriteLine("Key is missing: Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
        //    }
        //    else
        //    {
        //        //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
        //        if (Climate.Spinup_MonthlyData.ContainsKey(actualYear))
        //        {
        //            AnnualWeather[ecoregion] = Climate.Spinup_MonthlyData[actualYear][ecoregion.Index];
        //        }
        //    }
           
        //}

        //---------------------------------------------------------------------
        // Generates new climate parameters for all ecoregions at an annual time step.
        
        public static void SetAllEcoregionsFutureAnnualClimate(int year)
        {
            // grab the year's future climate
            foreach (var ecoregion in PlugIn.ModelCore.Ecoregions.Where(x => x.Active))
            {
                AnnualClimate[ecoregion] = Climate.FutureEcoregionYearClimate[ecoregion.Index][year];      // Climate data year index is 1-based
            }

            //int actualYear = Climate.Future_MonthlyData.Keys.Min() + year - 1;
            //foreach (IEcoregion ecoregion in PlugIn.ModelCore.Ecoregions)
            //{
            //    if (ecoregion.Active)
            //    {
            //        //PlugIn.ModelCore.UI.WriteLine("Retrieving {0} for year {1}.", spinupOrfuture.ToString(), actualYear);
            //        if (Climate.Future_MonthlyData.ContainsKey(actualYear))
            //        {
            //            AnnualWeather[ecoregion] = Climate.Future_MonthlyData[actualYear][ecoregion.Index];
            //        }

            //        //PlugIn.ModelCore.UI.WriteLine("Utilizing Climate Data: Simulated Year = {0}, actualClimateYearUsed = {1}.", actualYear, AnnualWeather[ecoregion].Year);
            //    }

            //}
        }
        

        
    }
}
