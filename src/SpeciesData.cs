//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.Succession;
using Landis.Library.Climate;

using System.Collections.Generic;
using System;
using System.IO;

namespace Landis.Extension.Succession.NECN_Hydro
{


    public class SpeciesData // : ISpeciesData
    {
        public static Species.AuxParm<int> FuncType;
        public static Species.AuxParm<bool> NFixer;
        public static Species.AuxParm<int> GDDmin;
        public static Species.AuxParm<int> GDDmax;
        public static Species.AuxParm<int> MinJanTemp;
        public static Species.AuxParm<double> MaxDrought;
        public static Species.AuxParm<double> LeafLongevity;
        public static Species.AuxParm<bool> Epicormic;
        public static Species.AuxParm<double> LeafLignin;
        public static Species.AuxParm<double> WoodLignin;
        public static Species.AuxParm<double> CoarseRootLignin;
        public static Species.AuxParm<double> FineRootLignin;
        public static Species.AuxParm<double> LeafCN;
        public static Species.AuxParm<double> WoodCN;
        public static Species.AuxParm<double> CoarseRootCN;
        public static Species.AuxParm<double> LeafLitterCN;
        public static Species.AuxParm<double> FineRootCN;
        public static Species.AuxParm<int> Max_ANPP;
        public static Species.AuxParm<int> Max_Biomass;

        //---------------------------------------------------------------------
        public static void Initialize(IInputParameters parameters)
        {
            FuncType            = parameters.SppFunctionalType;
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

        }
    }
}
