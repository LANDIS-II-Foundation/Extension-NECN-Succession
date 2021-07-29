//  Author: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;


namespace Landis.Extension.Succession.NECN
{
    public interface IFunctionalType
    {
        double TempCurve1{get;set;}
        double TempCurve2 { get; set; }
        double TempCurve3 { get; set; }
        double TempCurve4 { get; set; }
        double FractionANPPtoLeaf{get;set;}
        double BiomassToLAI{get;set;}
        double KLAI{get;set;}
        double MaxLAI{get;set;}
        double MinLAI { get; set; }
        //double GrowthLAI { get; set; }
        double MoistureCurve2 {get;set;}
        double MoistureCurve3 { get; set; }
        double MonthlyWoodMortality{get;set;}
        double WoodDecayRate{get;set;}
        double LongevityMortalityShape{get;set;}
        int FoliageDropMonth{get;set;}
        double CoarseRootFraction { get; set; }
        double FineRootFraction { get; set; }

    }
    
    public class FunctionalType
    : IFunctionalType
    {
        private double tempcurve1;
        private double tempcurve2;
        private double tempcurve3;
        private double tempcurve4;
        private double fcfracLeaf;
        private double btoLAI;
        private double kLAI;
        private double maxLAI;
        private double moisturecurve2;
        private double moisturecurve3;
        private double monthlyWoodMortality;
        private double woodDecayRate;
        private double mortCurveShape;
        private int leafNeedleDrop;
        private double coarseRootFraction;
        private double fineRootFraction;
        private double minLAI;
        //private double growthLAI;

        public static FunctionalTypeTable Table;
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Optimum temperature for production for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double TempCurve1
        {
            get {
                return tempcurve1;
            }
            set {
                    if (value  < 10.0 || value  > 40.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 10 and 40.0");
                    tempcurve1 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Maximum temperature for production for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double TempCurve2
        {
            get {
                return tempcurve2;
            }
            set {
                    if (value  < 20.0 || value  > 100.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 20 and 100.0");
                    tempcurve2 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Left curve shape for parameterization of a Poisson Density Function curve to 
        /// simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double TempCurve3
        {
            get {
                return tempcurve3;
            }
            set {
                    if (value  < 0.0 || value  > 5.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0 and 5.0");
                    tempcurve3 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Right curve shape for parameterization of a Poisson Density Function 
        /// curve to simulate temperature effect on growth.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double TempCurve4
        {
            get {
                return tempcurve4;
            }
            set {
                    if (value  < 0.0 || value  > 10.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0 and 10.0");
                    tempcurve4 = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// C allocation fraction of old leaves for mature forest.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double FractionANPPtoLeaf
        {
            get {
                return fcfracLeaf;
            }
            set {
                    if (value  < 0.1 || value  > 1.0)
                        throw new InputValueException(value.ToString(),
                            "The fraction of NPP allocated to leaves must be between 0.1 and 1.0");
                fcfracLeaf = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Biomass to leaf area index (LAI) conversion factor for trees.  This is a biome-specific parameters.  
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double BiomassToLAI
        {
            get {
                return btoLAI;
            }
            set {
                    if (value  < -3.0 || value  > 1000.0)
                        throw new InputValueException(value.ToString(),
                            "BTOLAI must be between -3 and 1000");
                btoLAI = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Large wood mass in grams per square meter (g C /m2) at which half of the 
        /// theoretical maximum leaf area (MAXLAI) is achieved.
        /// Century Model Interface Help - Colorado State University, Fort Collins, CO  80523
        /// </summary>
        public double KLAI
        {
            get {
                return kLAI;
            }
            set {
                    if (value  < 1.0 || value  > 50000.0)
                        throw new InputValueException(value.ToString(),
                            "K LAI must be between 1 and 50000");
                kLAI = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// The Century manual recommends a maximum of 20
        /// </summary>
        public double MaxLAI
        {
            get {
                return maxLAI;
            }
            set {
                    if (value  < 0 || value  > 20.0)
                        throw new InputValueException(value.ToString(),
                            "Max LAI must be between 0 and 50");
                maxLAI = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Suppression is sensitive to the minimum LAI.  Below 0.3 and a cohort can be permanently suppressed without any growth.
        /// </summary>
        public double MinLAI
        {
            get
            {
                return minLAI;
            }
            set
            {
                if (value < 0 || value > 5.0)
                    throw new InputValueException(value.ToString(),
                        "Max LAI must be between 0 and 5");
                minLAI = value;
            }
        }

      

        //---------------------------------------------------------------------
        // 'PPRPTS(2)': The effect of water content on the intercept, allows the user to 
        //              increase the value of the intercept and thereby increase the slope of the line. MoistureCurve has replaced PPRPTS naming convention in NECN
        public double MoistureCurve2
        {
            get {
                return moisturecurve2;
            }
            set {
                moisturecurve2 = value;
            }
        }
        //---------------------------------------------------------------------
        // 'PPRPTS(3)': The lowest ratio of available water to PET at which there is no restriction on production.
        public double MoistureCurve3
        {
            get {
                return moisturecurve3;
            }
            set {
                moisturecurve3 = value;
            }
        }
        //---------------------------------------------------------------------
        public double MonthlyWoodMortality
        {
            get {
                return monthlyWoodMortality;
            }
            set {
                    if (value  < 0.0 || value  > 1.0)
                        throw new InputValueException(value.ToString(),
                            "Monthly Wood Mortality is a fraction and must be between 0.0 and 1.0");
                monthlyWoodMortality = value;
            }
        }
        //---------------------------------------------------------------------
        public double WoodDecayRate 
        { 
            get { 
                return woodDecayRate;
            }
            set {
                    if (value  <= 0.0 || value  > 2.0)
                        throw new InputValueException(value.ToString(),
                            "Decay rate must be between 0.0 and 2.0");
                woodDecayRate = value;
            }
        }
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines the shape of the age-related mortality curve.  Ranges from a gradual senescence (5)
        /// to a steep senescence (15).
        /// </summary>
        public double LongevityMortalityShape 
        { 
            get { 
                return mortCurveShape;
            }
            set {
                    if (value  <= 2 || value  > 25)
                        throw new InputValueException(value.ToString(),
                            "Mortality shape curve parameters must be between 5 and 15");
                mortCurveShape = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Determines at what month of the year needles or leaves are dropped.
        /// </summary>
        public int FoliageDropMonth
        {
            get { 
                return leafNeedleDrop;
            }
            set {
                    if (value  < 1 || value  > 12)
                        throw new InputValueException(value.ToString(),
                            "Leaf/Needle Drop must be a month of the year, 1-12");
                leafNeedleDrop = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines the fraction of woody biomass that is coarse roots
        /// </summary>
        public double CoarseRootFraction
        {
            get
            {
                return coarseRootFraction;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new InputValueException(value.ToString(),
                        "Coarse Roots must be expressed as a fraction, 0-1");
                coarseRootFraction = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Determines the fraction of leaf biomass that is fine roots
        /// </summary>
        public double FineRootFraction
        {
            get
            {
                return fineRootFraction;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new InputValueException(value.ToString(),
                        "Fine Roots must be expressed as a fraction, 0-1");
                fineRootFraction = value;
            }
        }

        //---------------------------------------------------------------------

       public FunctionalType()
        {
        }
        
        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            Table = parameters.FunctionalTypes;
        }

        //---------------------------------------------------------------------
    }
}
