//  Author: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using System;
using Landis.Core;

namespace Landis.Extension.Succession.NECN
{
    public class OtherData
    {

        // USER INPUTS ------------------------------------------------------------
        public static LitterType[] LitterParameters;
        public static bool CalibrateMode;
        public static double ProbEstablishAdjust;
        public static WaterType WaterDecayFunction;
        public static bool SoilWaterVersion_Henne = false;


        // NOTE: *****************************************************************
        // ALL input data comments derived from the Century Model Interface Help -
        // Colorado State University, Fort Collins, CO  80523
        // NOTE: *****************************************************************

        // Maximum amount of structural material that will decompose in grams of carbon
        // per square meter (g C /m2).
        // public const double MaxStructuralC = 5000; //STRMAX(1) and (2)

        // P2CO2 & P3CO2 - Controls flow from soil organic matter with intermediate turnover
        // to CO2 (fraction of C lost as CO2 during decomposition).
        // Values from ffix.100 file.
        public const double FractionSOM2toCO2 = 0.55;
        public const double FractionSOM3toCO2 = 0.55;

        public const double StructuralCN = 200;  // (RCESTR)

        // ANIMPT - Slope term used to vary the impact of soil anaerobic conditions on
        // decomposition flows to the passive soil organic matter pool.
        public const double AnaerobicImpactSlope = 5.0;    // from the ffix.100 file

        // DAMR(1,1) - Fraction of surface N absorbed by residue. Valid Range: 0.0 to 1.0
        // DAMRMN - Minimum C / N ratio allowed in residue after direct absorption.
        // PABRES - Amount of residue which will give maximum direct absorption of N (g C /m2).
        public const double FractionSurfNAbsorbed    = 0.0;
        public const double MinResidueCN       = 15.0;
        public const double ResidueMaxDirectAbsorb  = 100.0;

        // Lignin Respiration Rate (RSPLIG)
        // Lignin Decay Effect (PLIGST)
        public const double LigninRespirationRate  = 0.3;
        public const double LigninDecayEffect   = 3.0;


        // 'PPRPTS(1)': The minimum ratio of available water to PET which would completely
        //              limit production assuming water content is equal to 0.
        // 'PPRPTS(2)': The effect of water content on the intercept, allows the user to
        //              increase the value of the intercept and thereby increase the slope of the line.
        // 'PPRPTS(3)': The lowest ratio of available water to PET at which there is no restriction on production.
        //public const double MoistureCurve1 = 0.0;

        // RAD1P:  C / N ratio of slow SOM formed from surface active pool.
        // Minimum allowable C / N used to calculate addition term for C / N
        // ratio of slow SOM formed from surface active pool.
        public const double SurfaceActivePoolCNIntercept = 12.0;   // RAD1P(1,1)
        public const double SurfaceActivePoolCNSlope = 3.0;        //RAD1P(2,1)
        public const double SurfaceActivePoolCNMinimum = 5.0;      //RAD1P(3,1)

        // ELITST - Effect of litter on soil temperature relative to live and standing dead biomass.
        // PMXTMP - Effect of biomass on maximum surface temperature.
        // PMNTMP - Effect of biomass on minimum surface temperature.
        public const double EffectLitterSoilT = 0.4;  //ELITST
        public const double EffectBiomassMaxSurfT = -0.00350;//         'PMXTMP'
        public const double EffectBiomassMinSurfT = 0.00400; //          'PMNTMP'

        public const double TextureEffectSlope         = 0.75;            // Century:  PEFTXB
        public const double TextureEffectIntercept     = 0.25;           // Century:  PEFTXA

        // PS1S3(1) - Intercept for flow from soil organic matter with fast turnover to
        // soil organic matter with slow turnover (fraction of C from SOM1C to SOM3C).
        // PS1S3(2) - Slope for the effect of clay on the control of the flow from
        // soil organic matter with fast turnover to soil organic matter with slow turnover (fraction of C from SOM1C to SOM3C).
        public const double PS1S3_Intercept = 0.003;
        public const double PS1S3_Slope = 0.032;

        public const double PS2S3_Intercept = 0.003;
        public const double PS2S3_Slope = 0.009;

        // OMLECH(1 & 2) Parameters for the effect of sand on leaching of organic compounds. Values from ffix.100 file and
        // OMLECH(3) - The amount of water in centimeters (cm) that needs to flow
        // out of water layer 2 to produce leaching of organics.
        //public const double OMLeachWater   = 1.9;                      // Century:  OMLECH(3)  No longer used.
        //public const double OMLeachSlope       = 0.12;                  // Century:  OMLECH(2)
        //public const double OMLeachIntercept   = 0.03;                  // Century:  OMLECH(1)
        public const double OMLeachSlope = 0.25;                  // Adjusted after SoilWater was revamped in 11/2014
        public const double OMLeachIntercept = 0.0;                  // No leaching when there's no sand.  Adjusted after SoilWater was revamped in 11/2014

        // FLEACH(1 & 2) Parameters for the effect of sand on leaching of mineral N compounds. 
        // FLEACH(3) - The amount of water in centimeters (cm) that needs to flow
        // out of water layer 2 to produce leaching of organics.
        
        //public const double MineralLeachIntercept = 0.6;                  // Century:  FLEACH(1). 
        //public const double NfracLeachWater = 0.95;                      // Century:  FLEACH(3) Value used before ML made changes in SoilWater.cs 
        public const double MineralLeachSlope = 0.05;                  // Modified parameter so DIN leaching was the correct order of magnitude
        public const double MineralLeachIntercept = 0.0;                  // Reduced intercept so no leaching now when percent sand = 0
        //public const double NfracLeachWater = 0.25;                      // Reduced the fraction of NH4 leaching
        public const double NO3frac = 0.15;                      // Ratio of export to available N based on HBEF N budget
                       
        // SPL:  Parameter for metabolic (vs. structural) split.
        public const double MetaStructSplitIntercept   = 0.85;         // Century:  SPL(1)
        public const double MetaStructSplitSlope   = 0.013;            // Century:  SPL(2)

        // P1CO2 - determines CO2 loss from the surface and soil
        public const double P1CO2_Surface = 0.6; //P1CO2A(1)
        public const double P1CO2_Soil_Intercept = 0.17; // P1CO2A(2)
        public const double P1CO2_Soil_Slope = 0.68; //P1CO2B(2)

        // PMCO2(1 and 2) - controls flow from surface/soil metabolic to CO2 (fraction of C lost as CO2 during decomposition).
        public const double MetabolicToCO2Surface = 0.55;
        public const double MetabolicToCO2Soil = 0.55;

        // PS1CO2(1 and 2) Controls amount of CO2 loss when structural decomposes to SOM1.
        public const double StructuralToCO2Surface = 0.45;
        public const double StructuralToCO2Soil = 0.55;

        // VARAT1(1,1) - Maximum C / N ratio for material entering SOM1.
        // VARAT1(2,1) - Minimum C / N ratio for material entering SOM1.
        // VARAT1(3,1) - Amount N present when minimum ratio applies.
        public const double MaxCNenterSOM1         = 18.0;
        public const double MinCNenterSOM1         = 8.0;
        public const double MinContentN_SOM1  = 2.0;

        // VARAT2(1,1) - Maximum C / N ratio for material entering SOM2.
        // VARAT2(2,1) - Minimum C / N ratio for material entering SOM2.
        // VARAT2(3,1) - Amount N present when minimum ratio applies.
        public const double MaxCNenterSOM2         = 40.0;
        public const double MinCNenterSOM2         = 12.0;
        public const double MinContentN_SOM2  = 2.0;

        // VARAT3(1,1) - Maximum C / N ratio for material entering SOM3.
        // VARAT3(2,1) - Minimum C / N ratio for material entering SOM3.
        // VARAT3(3,1) - Amount N present when minimum ratio applies.
        public const double MaxCNenterSOM3         = 20.0;
        public const double MinCNenterSOM3         = 6.0;
        public const double MinContentN_SOM3  = 2.0;

        // PCEMIC(1,1) - Maximum C / N ratio for surface microbial pool.
        // PCEMIC(2,1) - Minimum C / N ratio for surface microbial pool.
        // PCEMIC(3,1) - Minimum N content of decomposing aboveground material,
        // above which the C / N ratio of the surface microbes equals PCEMIC(2,*).
        
        public const double MaxCNSurfMicrobes          = 16.0;
        public const double MinCNSurfMicrobes          = 10.0;
        public const double MinNContentCNSurfMicrobes  = 0.02;

        //Constants needed to calculate frass deposition and C/N ratio of frass.
        //Frass amount calculated as a function of %defoliation using correspondence with Jane Foster citing Phil Townsends in press paper
        public const double frassdepk=0.86;
                
        //CNratiofrass- C/N ratio of frass as calculated by Lovett and Ruesink 1995 (Oecologia 104:133).
        public const double CNratiofrass = 23.0;
        
        // TMELT(1) - Minimum temperature above which at least some snow will melt.
        // TMELT(2) - Ratio between degrees above the minimum and centimeters of snow that will melt.
        public const double TMelt1 = -8.0;
        public const double TMelt2 = 4.0;

        // FWLOSS(1) - Scaling factor for interception and evaporation of precipitation by live and standing dead biomass.
        // Valid Range: 0.0 to 1.0
        public static double WaterLossFactor1 = 0.8;
        // FWLOSS(2) - Scaling factor for bare soil evaporation of precipitation (h2olos).
        // Valid Range: 0.0 to 1.0
        public static double WaterLossFactor2 = 0.8;
        // FWLOSS(4) - Scaling factor for potential evapotranspiration (pevap).
        public const double WaterLossFactor4 = 0.9;

        // AWTL -  Weighting factor for transpiration loss; indicates which fraction of the available water can be extracted by the roots.
        public const double TranspirationLossFactor = 0.8;  // Value from layer 2 of ffix.100 file.

        // TEFF(1) - Intercept value for determining the temperature component of DEFAC, the decomposition factor.
        // TEFF(2) - Slope value for determining the temperature component of DEFAC, the decomposition factor.
        // TEFF(3) - Exponent value for determining the temperature component of DEFAC, the decomposition factor.
        public const double TemperatureEffectIntercept = 0.0;
        public const double TemperatureEffectSlope = 0.125;
        public const double TemperatureEffectExponent = 0.06;

        // ANEREF(1) - Ratio of rain/potential evapotranspiration below which there is no negative impact of soil anaerobic conditions on decomposition.
        // ANEREF(2) - Ratio of rain/potential evapotranspiration above which there is maximum negative impact of soil anaerobic conditions on decomposition.
        // ANEREF(3) - Minimum value of the impact of soil anaerobic conditions on decomposition; functions as a multiplier for the maximum decomposition rate.
        public const double RatioPrecipPETMaximum = 1.5;
        public const double RatioPrecipPETMinimum = 3.0;
        public const double AnerobicEffectMinimum = 0.3;

        public const double MonthAdjust = 1.0;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            LitterParameters = new LitterType[2];

            LitterType litterParmsSurface = new LitterType();
            LitterType litterParmsSoil = new LitterType();

            CalibrateMode = parameters.CalibrateMode;
            SoilWaterVersion_Henne = parameters.SoilWater_Henne;
            WaterDecayFunction = parameters.WType;
            
            // Structural decomposition rate, the fraction of the pool that turns over each year.
            litterParmsSurface.DecayRateStrucC = 3.9 / 100.0;//DEC1(1)  
            litterParmsSoil.DecayRateStrucC = 4.9 / 100.0; //DEC1(2)  

            // Metabolic decomposition rate, the fraction of the pool that turns over each year.
            litterParmsSurface.DecayRateMetabolicC = 14.8 / 100.0; //DEC2(1)  //modified assuming they meant, 'percentage'
            litterParmsSoil.DecayRateMetabolicC = 18.5 / 100; //DEC2(2)  //modified assuming they meant, 'percentage'

            // Decomposition rate of organic matter with active turnover, the fraction of the pool
            // that turns over each year (SOM1)
            litterParmsSurface.DecayRateMicrobes = 6.0 / 100.0; //DEC3(1)  set to 1.0 until meaning of 'fraction' is determined.
            litterParmsSoil.DecayRateMicrobes = 7.3 / 100.0; //DEC3(2)  

            LitterParameters[0] = litterParmsSurface;
            LitterParameters[1] = litterParmsSoil;


        }

    }
}
