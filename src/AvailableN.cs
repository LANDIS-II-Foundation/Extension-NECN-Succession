//  Author: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using System.Collections.Generic;
using System;
using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.UniversalCohorts;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// </summary>
    public class AvailableN
    {

        //---------------------------------------------------------------------
        // Method for setting the available resorbed N for each cohort.
        // Amount of resorbed N must be in units of g N m-2.
        public static void SetResorbedNallocation(ICohort cohort, double resorbedNallocation, ActiveSite site, double[] actualANPP)
        {
            double totalNdemand = CalculateCohortNDemand(site, cohort, actualANPP);

            // Use resorbed N first and only if it is spring time unless you are evergreen.  
            double leafLongevity = SpeciesData.LeafLongevity[cohort.Species];
            if ((leafLongevity <= 1.0 && Main.Month > 0 && Main.Month < 6) || leafLongevity > 1.0)
            {
                resorbedNallocation = Math.Max(0.0, cohort.Data.AdditionalParameters.Nresorption);

                cohort.Data.AdditionalParameters.Nresorption = Math.Max(0.0, resorbedNallocation - totalNdemand);
            }

            return;

        }

        //---------------------------------------------------------------------
        // Method for calculationg how much N should be resorbed, based the difference in N content between leaves and litterfall;
        // month is only included for logging purposes.
        public static double CalculateResorbedFoliarN(ActiveSite site, ISpecies species, double leafBiomass)
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

        public static void CalculateAnnualMineralNfraction(Site site)
        {
            double NAllocTotal = 0.0;
            
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    
                    //Nallocation is a measure of how much N a cohort can gather relative to other cohorts
                    double Nallocation = 1- Math.Exp((-Roots.CalculateCoarseRoot(cohort, cohort.Data.AdditionalParameters.WoodBiomass)*0.02));

                    if (Nallocation <= 0.0) 
                        Nallocation = Math.Max(Nallocation, cohort.Data.AdditionalParameters.WoodBiomass * 0.01);
                    
                    NAllocTotal += Nallocation;

                    cohort.Data.AdditionalParameters.MineralNfraction = Nallocation;

                }
            }
            
            // Next relativize
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                //PlugIn.ModelCore.UI.WriteLine(" SpeciesCohorts = {0}", speciesCohorts.Species.Name);
                foreach (ICohort cohort in speciesCohorts)
                {
                    double Nfraction = cohort.Data.AdditionalParameters.MineralNfraction;
                    double relativeNfraction = Nfraction / NAllocTotal;
                    cohort.Data.AdditionalParameters.MineralNfraction = relativeNfraction;

                    if (Double.IsNaN(relativeNfraction) || Double.IsNaN(Nfraction) || Double.IsNaN(NAllocTotal))
                    {
                        PlugIn.ModelCore.UI.WriteLine("  N ALLOCATION CALCULATION = NaN!  ");
                        PlugIn.ModelCore.UI.WriteLine("  Site_Row={0:0}, Site_Column={1:0}.", site.Location.Row, site.Location.Column);
                        PlugIn.ModelCore.UI.WriteLine("  Nallocation={0:0.00}, NAllocTotal={1:0.00}, relativeNallocation={2:0.00}.", Nfraction, NAllocTotal, relativeNfraction);
                        PlugIn.ModelCore.UI.WriteLine("  Wood={0:0.00}, Leaf={1:0.00}.", cohort.Data.AdditionalParameters.WoodBiomass, cohort.Data.AdditionalParameters.LeafBiomass);
                    }                    
                }
            }

        }

        //---------------------------------------------------------------------
        // Calculates how much mineral N a cohort can access, based on the amount of N available.
        public static void CalculateMonthlyMineralNallocation(Site site)
        {
           
           double availableN = SiteVars.MineralN[site];  // g/m2
           Math.Max(availableN, 0.01);
                                   
            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    
                    double Nfraction = Math.Max(0.05, cohort.Data.AdditionalParameters.MineralNfraction);  //even a new cohort gets a little love
                    
                    double Nallocation = Math.Max(0.05, Nfraction * availableN);
                   
                    if (Double.IsNaN(Nallocation) || Double.IsNaN(Nfraction) || Double.IsNaN(availableN))
                    {
                        PlugIn.ModelCore.UI.WriteLine("  LIMIT N CALCULATION = NaN!  ");
                        PlugIn.ModelCore.UI.WriteLine("  Site_Row={0:0}, Site_Column={1:0}.", site.Location.Row, site.Location.Column);
                        PlugIn.ModelCore.UI.WriteLine("  MineralNallocation={0:0.00}, Nfraction={1:0.00}, availableN={2:0.00}.", Nallocation, Nfraction, availableN);
                    }

                    cohort.Data.AdditionalParameters.MineralNallocation = Nallocation;

                }
            }                   
           
        }

        /// <summary>
        /// Calculates cohort N demand depending upon how much N would be removed through growth (ANPP) of leaves, wood, coarse roots and fine roots.  
        /// Demand is then used to determine the amount of N that a cohort "wants".
        /// </summary>
        public static double CalculateCohortNDemand(ActiveSite site, ICohort cohort, double[] ANPP)
        {            
            if(ANPP[1] <= 0.0)
                return 0.0;
             

            if (SpeciesData.NFixer[cohort.Species])  // We fix our own N!
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
                
                woodN       = ANPPwood * 0.47  / SpeciesData.WoodCN[cohort.Species];
                coarseRootN = ANPPcoarseRoot * 0.47  / SpeciesData.CoarseRootCN[cohort.Species];
            }

            if(ANPP[1] > 0.0)  // Leaf
            {
                ANPPleaf = ANPP[1];                                
                ANPPfineRoot = Roots.CalculateFineRoot(cohort, ANPPleaf);
                         
                leafN       = ANPPleaf * 0.47 / SpeciesData.LeafCN[cohort.Species];
                fineRootN   = ANPPfineRoot * 0.47/ SpeciesData.FineRootCN[cohort.Species];

            }

            double totalANPP_C = (ANPPleaf + ANPPwood + ANPPcoarseRoot + ANPPfineRoot) * 0.47;
            double Ndemand = leafN + woodN + coarseRootN + fineRootN;
            
            if(Ndemand < 0.0)
            {
                PlugIn.ModelCore.UI.WriteLine("   ERROR:  TotalANPP-C={0:0.00} Nreduction={1:0.00}.", totalANPP_C, Ndemand);
                throw new ApplicationException("Error: N Reduction is < 0.  See AvailableN.cs");
            }

            return Ndemand;
        }

        // <summary>
        // Because Growth used some Nitrogen, it must be subtracted from the appropriate pools, either resorbed or mineral.
        // </summary>
        public static void AdjustAvailableN(ICohort cohort, ActiveSite site, double[] actualANPP)
        {
            
            
            double totalNdemand = CalculateCohortNDemand(site, cohort, actualANPP);
            double adjNdemand = totalNdemand;
            double resorbedNused = 0.0;
            double resorbedNallocation = 0.0;

            // Use resorbed N first and only if it is spring time unless you are evergreen.  
            double leafLongevity = SpeciesData.LeafLongevity[cohort.Species];
            if ((leafLongevity <= 1.0 && Main.Month > 2 && Main.Month < 6) || leafLongevity > 1.0)
            {
                resorbedNallocation = Math.Max(0.0, cohort.Data.AdditionalParameters.Nresorption);
                resorbedNused = resorbedNallocation - Math.Max(0.0, resorbedNallocation - totalNdemand);
                adjNdemand = Math.Max(0.0, totalNdemand - resorbedNallocation);
                SetResorbedNallocation(cohort, Math.Max(0.0, resorbedNallocation - totalNdemand), site, actualANPP);
            }

            // Reduce available N after taking into account that some N may have been provided
            // via resorption (above).
            double Nuptake = 0.0;
            if (SiteVars.MineralN[site] >= adjNdemand)
            {
                SiteVars.MineralN[site] -= adjNdemand;
                Nuptake = adjNdemand;
            }

            else
            {
                adjNdemand = SiteVars.MineralN[site];
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


            return; 

        }


        //---------------------------------------------------------------------
        public static void AddResorbedN(ICohort cohort, double deadLeafRootsBiomass, ActiveSite site)
        {
           
            // Resorbed N:  We are assuming that any leaves dropped as a function of normal
            // growth and maintenance (e.g., fall senescence) will involve resorption of leaf N.
            double resorbedN = CalculateResorbedFoliarN(site, cohort.Species, deadLeafRootsBiomass); 

            cohort.Data.AdditionalParameters.Nresorption += resorbedN;
                        
            return;
        }
    }
}
