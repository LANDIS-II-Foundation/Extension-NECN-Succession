//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Utilities;
using Landis.Library.Succession;
using Landis.Library.Climate;
using Landis.Library.Parameters;

using System.Collections.Generic;
using System;
using System.IO;

namespace Landis.Extension.Succession.NECN
{


    public class SpeciesData 
    {
        //public static Landis.Library.Parameters.Species.AuxParm<int> FuncType;
        public static Landis.Library.Parameters.Species.AuxParm<bool> NFixer;
        public static Landis.Library.Parameters.Species.AuxParm<int> GDDmin;
        public static Landis.Library.Parameters.Species.AuxParm<int> GDDmax;
        public static Landis.Library.Parameters.Species.AuxParm<int> MinJanTemp;
        public static Landis.Library.Parameters.Species.AuxParm<double> MaxDrought;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLongevity;
        public static Landis.Library.Parameters.Species.AuxParm<bool> Epicormic;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> WoodLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> CoarseRootLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> FineRootLignin;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafCN;
        public static Landis.Library.Parameters.Species.AuxParm<double> WoodCN;
        public static Landis.Library.Parameters.Species.AuxParm<double> CoarseRootCN;
        public static Landis.Library.Parameters.Species.AuxParm<double> LeafLitterCN;
        public static Landis.Library.Parameters.Species.AuxParm<double> FineRootCN;
        public static Landis.Library.Parameters.Species.AuxParm<int> Max_ANPP;
        public static Landis.Library.Parameters.Species.AuxParm<int> Max_Biomass;
        public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIShape;
        public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIScale;
        public static Landis.Library.Parameters.Species.AuxParm<double> LightLAILocation;
        public static Landis.Library.Parameters.Species.AuxParm<double> LightLAIAdjust;

        // Optional parameters:
        public static Landis.Library.Parameters.Species.AuxParm<bool> Grass;
        public static Landis.Library.Parameters.Species.AuxParm<bool> NurseLog_depend; // W.Hotta (2021.08.01)
        public static Landis.Library.Parameters.Species.AuxParm<double> GrowthLAI;

        //Drought mortality variables
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold;
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDThreshold2;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortalityAboveThreshold2;
        public static Landis.Library.Parameters.Species.AuxParm<double> Intercept;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaAge;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaTempAnom;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaSWAAnom;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaBiomass;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaCWDAnom;
        public static Landis.Library.Parameters.Species.AuxParm<double> BetaNormCWD;
        public static Landis.Library.Parameters.Species.AuxParm<double> IntxnCWD_Biomass;

        public static Landis.Library.Parameters.Species.AuxParm<int> LagTemp;
        public static Landis.Library.Parameters.Species.AuxParm<int> LagCWD;
        public static Landis.Library.Parameters.Species.AuxParm<int> LagSWA;

        //CWD Establishment
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDBegin;
        public static Landis.Library.Parameters.Species.AuxParm<int> CWDMax;

        //Previously Functional Group Parameters
        public static Landis.Library.Parameters.Species.AuxParm<double> TempCurve1;
        public static Landis.Library.Parameters.Species.AuxParm<double> TempCurve2;
        public static Landis.Library.Parameters.Species.AuxParm<double> TempCurve3;
        public static Landis.Library.Parameters.Species.AuxParm<double> TempCurve4;
        public static Landis.Library.Parameters.Species.AuxParm<double> BiomassToLAI;
        public static Landis.Library.Parameters.Species.AuxParm<double> K_LAI;
        public static Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve1;
        public static Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve2;
        public static Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve3;
        public static Landis.Library.Parameters.Species.AuxParm<double> MoistureCurve4;
        public static Landis.Library.Parameters.Species.AuxParm<double> MinSoilDrain;
        public static Landis.Library.Parameters.Species.AuxParm<double> MonthlyWoodMortality;
        public static Landis.Library.Parameters.Species.AuxParm<double> WoodDecayRate;
        public static Landis.Library.Parameters.Species.AuxParm<double> MortalityCurveShape;
        public static Landis.Library.Parameters.Species.AuxParm<int> FoliageDropMonth;
        public static Landis.Library.Parameters.Species.AuxParm<double> CoarseRootFraction;
        public static Landis.Library.Parameters.Species.AuxParm<double> FineRootFraction;
        public static Landis.Library.Parameters.Species.AuxParm<double> MinLAI;
        public static Landis.Library.Parameters.Species.AuxParm<double> MaxLAI;
        public static Landis.Library.Parameters.Species.AuxParm<double> FractionANPPtoLeaf;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            //FuncType            = parameters.SppFunctionalType;
            NFixer              = parameters.NFixer;
            GDDmin              = parameters.GDDmin;
            GDDmax              = parameters.GDDmax;
            MinJanTemp          = parameters.MinJanTemp;
            MaxDrought          = parameters.MaxDrought;
            LeafLongevity       = parameters.LeafLongevity;
            Epicormic           = parameters.Epicormic;
            LeafLignin          = parameters.LeafLignin;
            WoodLignin          = parameters.WoodLignin ;
            CoarseRootLignin    = parameters.CoarseRootLignin ;
            FineRootLignin      = parameters.FineRootLignin ;
            LeafCN              = parameters.LeafCN;
            WoodCN              = parameters.WoodCN;
            CoarseRootCN        = parameters.CoarseRootCN;
            LeafLitterCN        = parameters.FoliageLitterCN;
            FineRootCN          = parameters.FineRootCN;
            Max_ANPP            = parameters.MaxANPP;
            Max_Biomass         = parameters.MaxBiomass;
            Grass               = parameters.Grass;
            NurseLog_depend         = parameters.Nlog_depend; // W.Hotta (2021.08.01)
            GrowthLAI           = parameters.GrowthLAI;
            //CWD Establishment
            CWDBegin            = parameters.CWDBegin;
            CWDMax              = parameters.CWDMax;

         
            // Previously functional group parameters
            TempCurve1 = parameters.Tempcurve1;
            TempCurve2 = parameters.Tempcurve2;
            TempCurve3 = parameters.Tempcurve3;
            TempCurve4 = parameters.Tempcurve4;
            BiomassToLAI = parameters.BiomassToLAI;
            K_LAI = parameters.K_LAI;
            MinLAI = parameters.MinLAI;
            MaxLAI = parameters.MaxLAI;
            MoistureCurve1 = parameters.Moisturecurve1;
            MoistureCurve2 = parameters.Moisturecurve2;
            MoistureCurve3 = parameters.Moisturecurve3;
            MoistureCurve4 = parameters.Moisturecurve4;
            MinSoilDrain = parameters.MinSoilDrain;
            MonthlyWoodMortality = parameters.MonthlyWoodMortality;
            WoodDecayRate = parameters.WoodDecayRate;
            MortalityCurveShape = parameters.MortalityCurveShape;
            FoliageDropMonth = parameters.LeafNeedleDrop;
            CoarseRootFraction = parameters.CoarseRootFraction;
            FineRootFraction = parameters.FineRootFraction;
            FractionANPPtoLeaf = parameters.FractionANPPtoLeaf;

            LightLAIShape        = parameters.LightLAIShape;
            LightLAIScale       = parameters.LightLAIScale;
            LightLAILocation = parameters.LightLAILocation;
            LightLAIAdjust = parameters.LightLAIAdjust;   

            foreach (ISpecies spp in PlugIn.ModelCore.Species)
            {
                try
                {
                    double maxLAI = MaxLAI[spp];
                    //PlugIn.ModelCore.UI.WriteLine("Spp={0}, FT={1}", spp.Name, SpeciesData.FuncType[spp]);

                }
                catch (Exception)
                {
                    string mesg = string.Format("Species or Functional Type Missing: {0}", spp.Name);
                    throw new System.ApplicationException(mesg);
                }
            }

        }
    }
}
