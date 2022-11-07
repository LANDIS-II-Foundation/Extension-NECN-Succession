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
    public class AvailableN
    {
        //Nested dictionary of species,cohort
        public static Dictionary<int, Dictionary<int,double>> CohortMineralNfraction;  //calculated once per year
        public static Dictionary<int, Dictionary<int, double>> CohortMineralNallocation;  //calculated monthly

        //---------------------------------------------------------------------
        // Method for retrieving the available resorbed N for each cohort.
        // Return amount of resorbed N in g N m-2.
        public static double GetResorbedNallocation(ICohort cohort, ActiveSite site)
        {
            //cohort = null;
            int cohortAddYear = GetAddYear(cohort); 
            //PlugIn.ModelCore.UI.WriteLine("GETResorbedNallocation: year={0}, mo={1}, species={2}, cohortAge={3}, cohortAddYear={4}.", PlugIn.ModelCore.CurrentTime, Main.Month, cohort.Species.Name, cohort.Age, cohortAddYear);
            double resorbedNallocation = 0.0;
            Dictionary<int, double> cohortDict;
            
            if (SiteVars.CohortResorbedNallocation[site].TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out resorbedNallocation);

            //PlugIn.ModelCore.UI.WriteLine("GETResorbedNallocation: year={0}, mo={1}, species={2}, cohortAge={3}, cohortAddYear={4}.", PlugIn.ModelCore.CurrentTime, Main.Month, cohort.Species.Name, cohort.Age, cohortAddYear);

            return resorbedNallocation;
        }

        //---------------------------------------------------------------------
        // Method for setting the available resorbed N for each cohort.
        // Amount of resorbed N must be in units of g N m-2.
        public static void SetResorbedNallocation(ICohort cohort, double resorbedNallocation, ActiveSite site)
        {
            int cohortAddYear = GetAddYear(cohort); 
            //PlugIn.ModelCore.UI.WriteLine("SETResorbedNallocation: year={0}, mo={1}, species={2}, cohortAge={3}, cohortAddYear={4}.", PlugIn.ModelCore.CurrentTime, Main.Month, cohort.Species.Name, cohort.Age, cohortAddYear);
            Dictionary<int, double> cohortDict;
            double oldResorbedNallocation;


            // If the dictionary entry exists for the cohort, overwrite it:
            if (SiteVars.CohortResorbedNallocation[site].TryGetValue(cohort.Species.Index, out cohortDict))
                if (cohortDict.TryGetValue(cohortAddYear, out oldResorbedNallocation))
                {
                    SiteVars.CohortResorbedNallocation[site][cohort.Species.Index][cohortAddYear] = resorbedNallocation;
                    return;
                }

            // If the dictionary does not exist for the cohort, create it:
            Dictionary<int, double> newEntry = new Dictionary<int, double>();
            newEntry.Add(cohortAddYear, resorbedNallocation);

            if (SiteVars.CohortResorbedNallocation[site].ContainsKey(cohort.Species.Index))
            {
                SiteVars.CohortResorbedNallocation[site][cohort.Species.Index].Add(cohortAddYear, resorbedNallocation);
            }
            else
            {
                SiteVars.CohortResorbedNallocation[site].Add(cohort.Species.Index, newEntry);
            }

            //PlugIn.ModelCore.UI.WriteLine("SET ResorbedNallocation: ResorbedNallocation={0:0.00000}.", resorbedNallocation);
            return;
        }

        //---------------------------------------------------------------------
        // Method for RESETTING the available resorbed N for each cohort.  Any remaining in the Dictionary is deposited as Mineral N.
        // Amount of resorbed N must be in units of g N m-2.
        // This is necessary to prevent infinite storage of N.
        //public static void ResetResorbedNallocation(ICohort cohort, double resorbedNallocation)
        //{
        //    CohortResorbedNallocation = new Dictionary<int, Dictionary<int, double>>();
        //}

        //---------------------------------------------------------------------
        // Method for calculationg how much N should be resorbed, based the difference in N content between leaves and litterfall;
        // month is only included for logging purposes.
        public static double CalculateResorbedN(ActiveSite site, ISpecies species, double leafBiomass)
        {
           
                double leafN = leafBiomass * 0.47 / SpeciesData.LeafCN[species];
                double litterN = leafBiomass * 0.47 / SpeciesData.LeafLitterCN[species];

                double resorbedN = leafN - litterN;
                
                //PlugIn.ModelCore.UI.WriteLine("How much N to resorb?  leafN={0:0.00}, litterN={1:0.00}, resorbedN={2:0.00}, leafbiomass={3:0.00}.", leafN, litterN, resorbedN, leafBiomass);
                
                SiteVars.ResorbedN[site] += resorbedN;

                return resorbedN;           
            
        }   
         

        //---------------------------------------------------------------------
        // Method for calculating Mineral N allocation, called from Century.cs Run method before calling Grow
        // Iterates through cohorts, assigning each a portion of mineral N based on coarse root biomass.  Uses an exponential function to "distribute" 
        // the N more evenly between spp. so that the ones with the most woody biomass don't get all the N (L122).

        public static void CalculateMineralNfraction(Site site)
        {
            AvailableN.CohortMineralNfraction = new Dictionary<int, Dictionary<int, double>>();
            double NAllocTotal = 0.0;
            
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort); 
                    //PlugIn.ModelCore.UI.WriteLine("CALCMineralNfraction: year={0}, mo={1}, species={2}, cohortAge={3}, cohortAddYear={4}.", PlugIn.ModelCore.CurrentTime, Main.Month, cohort.Species.Name, cohort.Age, cohortAddYear);
                    
                    //Nallocation is a measure of how much N a cohort can gather relative to other cohorts
                    //double Nallocation = Roots.CalculateFineRoot(cohort.LeafBiomass); 
                    double Nallocation = 1- Math.Exp((-Roots.CalculateCoarseRoot(cohort, cohort.WoodBiomass)*0.02));

                    if (Nallocation <= 0.0) 
                        Nallocation = Math.Max(Nallocation, cohort.WoodBiomass * 0.01);
                    
                    NAllocTotal += Nallocation;
                    Dictionary<int, double> newEntry = new Dictionary<int, double>();
                    newEntry.Add(cohortAddYear, Nallocation);

                    if (CohortMineralNfraction.ContainsKey(cohort.Species.Index))
                    {
                        if (!CohortMineralNfraction[cohort.Species.Index].ContainsKey(cohortAddYear))
                           CohortMineralNfraction[cohort.Species.Index][cohortAddYear] = Nallocation;
                    }
                    else
                    {
                        CohortMineralNfraction.Add(cohort.Species.Index, newEntry);
                    }
                    
                }

            }
            
            // Next relativize
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                //PlugIn.ModelCore.UI.WriteLine(" SpeciesCohorts = {0}", speciesCohorts.Species.Name);
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort); 
                    double Nallocation = CohortMineralNfraction[cohort.Species.Index][cohortAddYear];
                    double relativeNallocation = Nallocation / NAllocTotal;
                    CohortMineralNfraction[cohort.Species.Index][cohortAddYear] = relativeNallocation;

                    if (Double.IsNaN(relativeNallocation) || Double.IsNaN(Nallocation) || Double.IsNaN(NAllocTotal))
                    {
                        //PlugIn.ModelCore.UI.WriteLine("  N ALLOCATION CALCULATION = NaN!  ");
                        //PlugIn.ModelCore.UI.WriteLine("  Site_Row={0:0}, Site_Column={1:0}.", site.Location.Row, site.Location.Column);
                        //PlugIn.ModelCore.UI.WriteLine("  Nallocation={0:0.00}, NAllocTotal={1:0.00}, relativeNallocation={2:0.00}.", Nallocation, NAllocTotal, relativeNallocation);
                        //PlugIn.ModelCore.UI.WriteLine("  Wood={0:0.00}, Leaf={1:0.00}.", cohort.WoodBiomass, cohort.LeafBiomass);
                    }                    
                }
            }

        }

        // Calculates how much N a cohort gets, based on the amount of N available.

        public static void SetMineralNallocation(Site site)
        {
            AvailableN.CohortMineralNallocation = new Dictionary<int, Dictionary<int, double>>();
            
           double availableN = SiteVars.MineralN[site];  // g/m2
           Math.Max(availableN, 0.01);
                                   
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    int cohortAddYear = GetAddYear(cohort); 
                    if (Main.MonthCnt == 11) 
                        cohortAddYear--;
                    
                    double Nfraction = 0.05;  //even a new cohort gets a little love
                    Dictionary<int, double> cohortDict = new Dictionary<int,double>();

                    if (AvailableN.CohortMineralNfraction.TryGetValue(cohort.Species.Index, out cohortDict))
                        cohortDict.TryGetValue(cohortAddYear, out Nfraction);
                    
                    double Nallocation = Math.Max(0.05, Nfraction * availableN);
                   
                    if (Double.IsNaN(Nallocation) || Double.IsNaN(Nfraction) || Double.IsNaN(availableN))
                    {
                        //PlugIn.ModelCore.UI.WriteLine("  LIMIT N CALCULATION = NaN!  ");
                        //PlugIn.ModelCore.UI.WriteLine("  Site_Row={0:0}, Site_Column={1:0}.", site.Location.Row, site.Location.Column);
                        //PlugIn.ModelCore.UI.WriteLine("  Nallocation={0:0.00}, Nfraction={1:0.00}, availableN={2:0.00}.", Nallocation, Nfraction, availableN);
                    }

                    Dictionary<int, double> newEntry = new Dictionary<int, double>();
                    newEntry.Add(cohortAddYear, Nallocation);

                    if (CohortMineralNallocation.ContainsKey(cohort.Species.Index))
                    {
                        if (!CohortMineralNallocation[cohort.Species.Index].ContainsKey(cohortAddYear))
                            CohortMineralNallocation[cohort.Species.Index][cohortAddYear] = Nallocation;
                    }
                    else
                    {
                        CohortMineralNallocation.Add(cohort.Species.Index, newEntry);
                    }
                }
            }                   
           
        }

        //---------------------------------------------------------------------
        // Method for retrieving the available mineral N for each cohort.
        // Return amount of resorbed N in g N m-2.
        public static double GetMineralNallocation(ICohort cohort)
        {
           
            int cohortAddYear = GetAddYear(cohort);             
            double mineralNallocation = 0.0;
            Dictionary<int, double> cohortDict;

            if (AvailableN.CohortMineralNallocation.TryGetValue(cohort.Species.Index, out cohortDict))
                cohortDict.TryGetValue(cohortAddYear, out mineralNallocation);

            return mineralNallocation;
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Calculates cohort N demand depending upon how much N would be removed through growth (ANPP) of leaves, wood, coarse roots and fine roots.  
        /// Demand is then used to determine the amount of N that a cohort "wants".
        /// </summary>
        public static double CalculateCohortNDemand(ISpecies species, ActiveSite site, ICohort cohort, double[] ANPP)
        {            
            if(ANPP[1] <= 0.0)
                return 0.0;
             

            if (SpeciesData.NFixer[species])  // We fix our own N!
                return 0.0;

            double ANPPwood = 0.0;
            double ANPPleaf = 0.0;
            double ANPPcoarseRoot = 0.0;
            double ANPPfineRoot = 0.0;
            double woodN = 0.0;
            double coarseRootN = 0.0;
            double leafN = 0.0;
            double fineRootN = 0.0;

            if(ANPP[0] > 0.0)  // Wood
            {
                ANPPwood = ANPP[0];                                
                ANPPcoarseRoot = Roots.CalculateCoarseRoot(cohort, ANPPwood);               
                
                woodN       = ANPPwood * 0.47  / SpeciesData.WoodCN[species];
                coarseRootN = ANPPcoarseRoot * 0.47  / SpeciesData.CoarseRootCN[species];
            }

            if(ANPP[1] > 0.0)  // Leaf
            {
                ANPPleaf = ANPP[1];                                
                ANPPfineRoot = Roots.CalculateFineRoot(cohort, ANPPleaf);
                         
                leafN       = ANPPleaf * 0.47 / SpeciesData.LeafCN[species];
                fineRootN   = ANPPfineRoot * 0.47/ SpeciesData.FineRootCN[species];

            }

            double totalANPP_C = (ANPPleaf + ANPPwood + ANPPcoarseRoot + ANPPfineRoot) * 0.47;
            double Ndemand = leafN + woodN + coarseRootN + fineRootN;
            
            if(Ndemand < 0.0)
            {
               // PlugIn.ModelCore.UI.WriteLine("   ERROR:  TotalANPP-C={0:0.00} Nreduction={1:0.00}.", totalANPP_C, Ndemand);
                throw new ApplicationException("Error: N Reduction is < 0.  See AvailableN.cs");
            }
            

            return Ndemand;
        }

        public static void AdjustAvailableN(ICohort cohort, ActiveSite site, double[] actualANPP)
        {
            // Because Growth used some Nitrogen, it must be subtracted from the appropriate pools, either resorbed or mineral.
            
            double totalNdemand = AvailableN.CalculateCohortNDemand(cohort.Species, site, cohort, actualANPP);
            double adjNdemand = totalNdemand;
            double resorbedNused = 0.0;
            //double mineralNused = 0.0;

            // Use resorbed N first and only if it is spring time unless you are evergreen.  
            double leafLongevity = SpeciesData.LeafLongevity[cohort.Species];
            if ((leafLongevity <= 1.0 && Main.Month > 2 && Main.Month < 6) || leafLongevity > 1.0)
            {
            double resorbedNallocation = Math.Max(0.0, AvailableN.GetResorbedNallocation(cohort, site));            

            resorbedNused = resorbedNallocation - Math.Max(0.0, resorbedNallocation - totalNdemand);            

            AvailableN.SetResorbedNallocation(cohort, Math.Max(0.0, resorbedNallocation - totalNdemand), site);

            adjNdemand = Math.Max(0.0, totalNdemand - resorbedNallocation);                            
            }

            // Reduce available N after taking into account that some N may have been provided
            // via resorption (above).
            double Nuptake = 0.0;
            if (SiteVars.MineralN[site] >= adjNdemand)
            {
                SiteVars.MineralN[site] -= adjNdemand;
                //mineralNused = adjNdemand;
                Nuptake = adjNdemand;
            }

            else
            {
                adjNdemand = SiteVars.MineralN[site];
                //mineralNused = SiteVars.MineralN[site];
                SiteVars.MineralN[site] = 0.0;
                Nuptake = SiteVars.MineralN[site];                
            }
            
            SiteVars.TotalNuptake[site] += Nuptake;

            if (OtherData.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
            {
                CalibrateLog.resorbedNused = resorbedNused;
                CalibrateLog.mineralNused = Nuptake;
                CalibrateLog.demand_N = totalNdemand;
            }

        }

        //---------------------------------------------------------------------
        // This is the SIMULATION year that a cohort was born (not its age).
        // The number can be negative if the cohort was added with the initial community.
        //---------------------------------------------------------------------
        private static int GetAddYear(ICohort cohort)
        {
            int currentYear = PlugIn.ModelCore.CurrentTime;
            int cohortAddYear = currentYear - (cohort.Age - Main.Year);
            if (Main.MonthCnt == 11)
                cohortAddYear++; 
            return cohortAddYear;
        }

        //---------------------------------------------------------------------

        public static void AddResorbedN(ICohort cohort, double deadLeafRootsBiomass, ActiveSite site)//, int month)
        {
           
            // Resorbed N:  We are assuming that any leaves dropped as a function of normal
            // growth and maintenance (e.g., fall senescence) will involve resorption of leaf N.
            double resorbedN = AvailableN.CalculateResorbedN(site, cohort.Species, deadLeafRootsBiomass); //, month);
            //double resorbedN = AvailableN.CalculateResorbedN(site, cohort.Species, cohort.LeafBiomass); //, month);
            double previouslyResorbedN = GetResorbedNallocation(cohort, site);

            AvailableN.SetResorbedNallocation(cohort, resorbedN + previouslyResorbedN, site);
                        
            return;
        }
    }
}
