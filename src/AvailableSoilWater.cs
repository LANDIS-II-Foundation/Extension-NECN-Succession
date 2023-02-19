// Author: Katie McQuillan
// Based on procedure in the AvailableN.cs (Scheller and Lucash) to distribute available soil water between cohorts based on biomass

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
        public static Dictionary<int, Dictionary<int, double>> CohortSWallocation;  // Dictionary of soil water allocation (cm) on a monthly basis 
        public static Dictionary<int, Dictionary<int, double>> CohortSWFraction;    // Dictionary of the fraction of total available water each cohort can access on a monthly basis 

        // Function allocates plant available water to each cohort on a site for a given month  
        public static void SetSWAllocation(Site site)
        {
            // Create a new dictionary for the sw allocation
            AvailableSoilWater.CohortSWallocation = new Dictionary<int, Dictionary<int, double>>();
            
            // This is the total water available to plants for growth/transpiration
            double availableSW = SiteVars.AvailableWaterTranspiration[site];

            //Loop through cohorts and set the allocation in the dictionary 
            foreach(ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach(ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;

                    double SWfraction = GetSWFraction(cohort);
                    double SWallocation = Math.Max(0.000001, SWfraction * availableSW); // Stop the soil water allocation from being 0. Even the smallest cohort gets some water. 

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

        // Function calculates the fraction of plant available water that each cohort receives on a site for a given month 
        public static void CalculateSWFraction(Site site)
        {   
            // Start a new dictionary for fraction of water for each cohort 
            AvailableSoilWater.CohortSWFraction = new Dictionary<int, Dictionary<int, double>>();
            
            // Initialize counter for total swallocation to be used to calculate fractions at the end 
            double SWAllocTotal = 0.0;
            
            // Loop thorugh each cohort to calculate fraction
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;
                    
                    // Fraction based on cohort biomass. Use an exponential function to control how evenly water is distributed among cohorts based on biomass. 
                    // Transpiration is senstive to this fraction. To minimize water limitation of larger cohorts, allocate water less evenly and more closely by biomass 
                    double SWallocation = 1-Math.Exp((-cohort.Biomass)*0.000002);  // 0.02 originally. Larger number produces more even allocation

                    // Need a minimum for each cohort so they no cohort ends up with nothing. 
                    if(SWallocation <= 0.0)
                        SWallocation = Math.Max(SWallocation, cohort.Biomass * 0.0000001); 
                    
                    // Allocations are summed so we can relativize in the next step to get the actual fractions
                    SWAllocTotal += SWallocation;
                    
                    // Add to dictionary 
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

            // Next relativize to get the actual fraction
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


        // Function to retrieve the allocated soil water of a given cohort 
        public static double GetSWAllocation(ICohort cohort)
        {
            int cohortAddYear = GetAddYear(cohort);
            double SWAllocation = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableSoilWater.CohortSWallocation.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out SWAllocation);
            return SWAllocation;
        }

        // Function to retrieve the fraction of soil water a given cohort can access 
        public static double GetSWFraction(ICohort cohort)
        {
            int cohortAddYear = GetAddYear(cohort);
            double SWFraction = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableSoilWater.CohortSWFraction.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out SWFraction);
            return SWFraction;
        }

        // Function to retrieve the year 
        // This is the SIMULATION year that a cohort was born (not its age).
        // The number can be negative if the cohort was added with the initial community.
        private static int GetAddYear(ICohort cohort)
        {
            int currentYear = PlugIn.ModelCore.CurrentTime;
            int cohortAddYear = currentYear - (cohort.Age - Main.Year);
            if (Main.MonthCnt == 11)
                cohortAddYear++; 
            return cohortAddYear;
        }





        // Stop transpiration from exceeding water in the cell  
        public static Dictionary<int, Dictionary<int, double>> CohortCapWater; 

        // Function allocates the swc-water empty as a cap on transpiration between cohorts each month
        public static void SetCapWater(Site site)
        {
            // Create a dictionary for the sw allocation
            AvailableSoilWater.CohortCapWater = new Dictionary<int, Dictionary<int, double>>();
            
            // This is the total water available to plants for growth/transpiration
            double availableSW = SiteVars.CapWater[site];
            
            // Loop through cohorts and set the allocation in the dictionary 
            foreach(ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach(ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort);
                    if (Main.MonthCnt == 11)
                        cohortAddYear--;
                
                    double SWfraction = GetSWFraction(cohort);
                    double CapAllocation = Math.Max(0.000001, SWfraction * availableSW); // Stop the SWallocation from being 0. Even the smallest cohort gets some water. 

                    Dictionary<int, double> newEntry = new Dictionary<int, double>();
                    newEntry.Add(cohortAddYear, CapAllocation);

                    if (CohortCapWater.ContainsKey(cohort.Species.Index))
                    {
                        if (!CohortCapWater[cohort.Species.Index].ContainsKey(cohortAddYear))
                            CohortCapWater[cohort.Species.Index][cohortAddYear] = CapAllocation;
                    }
                    else
                    {
                        CohortCapWater.Add(cohort.Species.Index, newEntry);
                    }
                }
            }

        }

        // Function to retrieve the allocated soil water of a given cohort 
        public static double GetCapWater(ICohort cohort)
        {
            int cohortAddYear = GetAddYear(cohort);
            double SWAllocation = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableSoilWater.CohortCapWater.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out SWAllocation);
            return SWAllocation;
        }
  

    }
}
