//  Author: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using System.Collections.Generic;
using System;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.LeafBiomassCohorts;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// </summary>
    public class AvailableSoilWater
    {
        public static Dictionary<int, Dictionary<int, double>> CohortSWallocation;  
        public static Dictionary<int, Dictionary<int, double>> CohortSWFraction;
        public static void SetSWAllocation(Site site)
        {
            AvailableSoilWater.CohortSWallocation = new Dictionary<int, Dictionary<int, double>>();
            double availableSW = SiteVars.AvailableWaterTranspiration[site];

            foreach(ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach(ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;
                    
                    
                    double SWfraction = GetSWFraction(cohort);
                    double SWallocation = Math.Max(0.01, SWfraction * availableSW); // basically 0.01 cm is the minimum soil water a plant would have access to even if it was a really small cohort or really droughty or something

                    Dictionary<int, double> newEntry = new Dictionary<int, double>();
                    newEntry.Add(cohortAddYear, SWallocation);

                    if (CohortSWallocation.ContainsKey(cohort.Species.Index))
                    {
                        if (!CohortSWallocation[cohort.Species.Index].ContainsKey(cohortAddYear))
                            CohortSWallocation[cohort.Species.Index][cohortAddYear] = SWallocation;
                    }
                    else
                    {
                        CohortSWallocation.Add(cohort.Species.Index, newEntry);
                    }
                }
            }

        }


        public static void CalculateSWFraction(Site site)
        {
            AvailableSoilWater.CohortSWFraction = new Dictionary<int, Dictionary<int, double>>();
            double SWAllocTotal = 0.0;
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;
                    double SWallocation = 1-Math.Exp((-cohort.Biomass)*0.02);

                    if(SWallocation <= 0.0)
                        SWallocation = Math.Max(SWallocation, cohort.Biomass * 0.001); // basically ensuring no cohort is left with nothing allocated 
                    
                    SWAllocTotal += SWallocation;
                    
                    Dictionary<int, double> newEntry = new Dictionary<int, double>();
                    newEntry.Add(cohortAddYear, SWallocation);

                    if (CohortSWFraction.ContainsKey(cohort.Species.Index))
                    {
                        if (!CohortSWFraction[cohort.Species.Index].ContainsKey(cohortAddYear))
                            CohortSWFraction[cohort.Species.Index][cohortAddYear] = SWallocation;
                    }
                    else
                    {
                        CohortSWFraction.Add(cohort.Species.Index, newEntry);
                    }
                }
            }

            // Next relativize 
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;
                    double SWallocation = CohortSWFraction[cohort.Species.Index][cohortAddYear];
                    double relativeSWallocation = SWallocation/SWAllocTotal;
                    CohortSWFraction[cohort.Species.Index][cohortAddYear] = relativeSWallocation;
                }
            }

        }




        public static double GetSWAllocation(ICohort cohort)
        {
            int cohortAddYear = GetAddYear(cohort);
            double SWAllocation = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableSoilWater.CohortSWallocation.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out SWAllocation);
            return SWAllocation;
        }

        public static double GetSWFraction(ICohort cohort)
        {
            int cohortAddYear = GetAddYear(cohort);
            double SWFraction = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableSoilWater.CohortSWFraction.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out SWFraction);
            return SWFraction;
        }

        private static int GetAddYear(ICohort cohort)
        {
            int currentYear = PlugIn.ModelCore.CurrentTime;
            int cohortAddYear = currentYear - (cohort.Age - Main.Year);
            if (Main.MonthCnt == 11)
                cohortAddYear++; 
            return cohortAddYear;
        }
  

    }
}
