 //  Authors: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;
using Landis.Library.LeafBiomassCohorts;
using System;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// Calculations for an individual cohort's biomass.
    /// </summary>
    public class CohortBiomass
        : Landis.Library.LeafBiomassCohorts.ICalculator
    {

        /// <summary>
        /// The single instance of the biomass calculations that is used by
        /// the plug-in.
        /// </summary>
        public static CohortBiomass Calculator;

        //  Ecoregion where the cohort's site is located
        private IEcoregion ecoregion;
        private double defoliation;
        private double defoliatedLeafBiomass;

        //---------------------------------------------------------------------

        public CohortBiomass()
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the change in a cohort's biomass due to Annual Net Primary
        /// Productivity (ANPP), age-related mortality (M_AGE), and development-
        /// related mortality (M_BIO).
        /// </summary>
        public float[] ComputeChange(ICohort cohort, ActiveSite site)
        {

            ecoregion = PlugIn.ModelCore.Ecoregion[site];

            // First call to the Calibrate Log:
            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.year = PlugIn.ModelCore.CurrentTime;
                CalibrateLog.month = Main.Month + 1;
                CalibrateLog.climateRegionIndex = ecoregion.Index;
                CalibrateLog.speciesName = cohort.Species.Name;
                CalibrateLog.cohortAge = cohort.Age;
                CalibrateLog.cohortWoodB = cohort.WoodBiomass;
                CalibrateLog.cohortLeafB = cohort.LeafBiomass;
            } 
           

            double siteBiomass = Main.ComputeLivingBiomass(SiteVars.Cohorts[site]);

            if(siteBiomass < 0)
                throw new ApplicationException("Error: Site biomass < 0");

            // ****** Mortality *******
            // Age-related mortality includes woody and standing leaf biomass.
            double[] mortalityAge = ComputeAgeMortality(cohort, site);

            // ****** Growth *******
            double[] actualANPP = ComputeActualANPP(cohort, site, siteBiomass, mortalityAge);
            
            //  Growth-related mortality
            double[] mortalityGrowth = ComputeGrowthMortality(cohort, site, siteBiomass, actualANPP);

            double[] totalMortality = new double[2]{Math.Min(cohort.WoodBiomass, mortalityAge[0] + mortalityGrowth[0]), Math.Min(cohort.LeafBiomass, mortalityAge[1] + mortalityGrowth[1])};
            double nonDisturbanceLeafFall = totalMortality[1];

            
            double scorch = 0.0;
            defoliatedLeafBiomass = 0.0;

            if (Main.Month == 6)  //July = 6
            {
                if (SiteVars.FireSeverity != null && SiteVars.FireSeverity[site] > 0)
                    scorch = FireEffects.CrownScorching(cohort, SiteVars.FireSeverity[site]);

                if (scorch > 0.0)  // NEED TO DOUBLE CHECK WHAT CROWN SCORCHING RETURNS
                    totalMortality[1] = Math.Min(cohort.LeafBiomass, scorch + totalMortality[1]);

                // Defoliation (index) ranges from 1.0 (total) to none (0.0).
                if (PlugIn.ModelCore.CurrentTime > 0) //Skip this during initialization
                {
                    //defoliation = Landis.Library.BiomassCohorts.CohortDefoliation.Compute(cohort, site, (int)siteBiomass); //this line lets defoliation work with Biomass Browse
                    int cohortBiomass = (int)(cohort.LeafBiomass + cohort.WoodBiomass);
                    defoliation = Landis.Library.Biomass.CohortDefoliation.Compute(site, cohort.Species, cohortBiomass, (int)siteBiomass);
                }

                if (defoliation > 1.0)
                    defoliation = 1.0;

                if (defoliation > 0.0)
                {
                    defoliatedLeafBiomass = (cohort.LeafBiomass) * defoliation;
                   if (totalMortality[1] + defoliatedLeafBiomass - cohort.LeafBiomass > 0.001)
                        defoliatedLeafBiomass = cohort.LeafBiomass - totalMortality[1];
                    //PlugIn.ModelCore.UI.WriteLine("Defoliation.Month={0:0.0}, LeafBiomass={1:0.00}, DefoliatedLeafBiomass={2:0.00}, TotalLeafMort={2:0.00}", Main.Month, cohort.LeafBiomass, defoliatedLeafBiomass , mortalityAge[1]);

                    ForestFloor.AddFrassLitter(defoliatedLeafBiomass, cohort.Species, site);

                }
            }
            else
            {
                defoliation = 0.0;
                defoliatedLeafBiomass = 0.0;
            }


            if (totalMortality[0] <= 0.0 || cohort.WoodBiomass <= 0.0)
                totalMortality[0] = 0.0;

            if (totalMortality[1] <= 0.0 || cohort.LeafBiomass <= 0.0)
                totalMortality[1] = 0.0;


            if ((totalMortality[0]) > cohort.WoodBiomass)
            {
                PlugIn.ModelCore.UI.WriteLine("Warning: WOOD Mortality exceeds cohort wood biomass. M={0:0.0}, B={1:0.0}", (totalMortality[0]), cohort.WoodBiomass);
                PlugIn.ModelCore.UI.WriteLine("Warning: If M>B, then list mortality. Mage={0:0.0}, Mgrow={1:0.0},", mortalityAge[0], mortalityGrowth[0]);
                throw new ApplicationException("Error: WOOD Mortality exceeds cohort biomass");

            }
            if ((totalMortality[1] + defoliatedLeafBiomass - cohort.LeafBiomass) > 0.01)
            {
                PlugIn.ModelCore.UI.WriteLine("Warning: LEAF Mortality exceeds cohort biomass. Mortality={0:0.000}, Leafbiomass={1:0.000}", (totalMortality[1] + defoliatedLeafBiomass), cohort.LeafBiomass);
                PlugIn.ModelCore.UI.WriteLine("Warning: If M>B, then list mortality. Mage={0:0.00}, Mgrow={1:0.00}, Mdefo={2:0.000},", mortalityAge[1], mortalityGrowth[1], defoliatedLeafBiomass);
                throw new ApplicationException("Error: LEAF Mortality exceeds cohort biomass");

            }
            float deltaWood = (float)(actualANPP[0] - totalMortality[0]);
            float deltaLeaf = (float)(actualANPP[1] - totalMortality[1] - defoliatedLeafBiomass);

            float[] deltas = new float[2] { deltaWood, deltaLeaf };

            //if((totalMortality[1] + defoliatedLeafBiomass) > cohort.LeafBiomass)
            //   PlugIn.ModelCore.UI.WriteLine("Warning: Leaf Mortality exceeds cohort leaf biomass. M={0:0.0}, B={1:0.0}, DefoLeafBiomass={2:0.0}, defoliationIndex={3:0.0}", totalMortality[1], cohort.LeafBiomass, defoliatedLeafBiomass, defoliation);
            
            UpdateDeadBiomass(cohort, site, totalMortality);

            CalculateNPPcarbon(site, cohort, actualANPP);

            AvailableN.AdjustAvailableN(cohort, site, actualANPP);

            if (OtherData.CalibrateMode && PlugIn.ModelCore.CurrentTime > 0)
            {
                CalibrateLog.deltaLeaf = deltaLeaf;
                CalibrateLog.deltaWood = deltaWood;
                CalibrateLog.WriteLogFile();
            }

            return deltas;
        }


        //---------------------------------------------------------------------

        private double[] ComputeActualANPP(ICohort    cohort,
                                         ActiveSite site,
                                         double    siteBiomass,
                                         double[]   mortalityAge)
        {

            double leafFractionNPP  = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FractionANPPtoLeaf;
            //double maxBiomass       = SpeciesData.Max_Biomass[cohort.Species];
            double sitelai          = SiteVars.LAI[site];
            double maxNPP           = SpeciesData.Max_ANPP[cohort.Species];

            double limitT   = calculateTemp_Limit(site, cohort.Species);

            double limitH20 = calculateWater_Limit(site, ecoregion, cohort.Species);

            double limitLAI = calculateLAI_Limit(cohort, site);

            // RMS 03/2016: Testing alternative more similar to how Biomass Succession operates: REMOVE FOR NEXT RELEASE
            //double limitCapacity = 1.0 - Math.Min(1.0, Math.Exp(siteBiomass / maxBiomass * 5.0) / Math.Exp(5.0));

            double competition_limit = calculate_LAI_Competition(cohort, site);

            double potentialNPP = maxNPP * limitLAI * limitH20 * limitT * competition_limit;

            double limitN = calculateN_Limit(site, cohort, potentialNPP, leafFractionNPP);

            potentialNPP *= limitN;

            //if (Double.IsNaN(limitT) || Double.IsNaN(limitH20) || Double.IsNaN(limitLAI) || Double.IsNaN(limitCapacity) || Double.IsNaN(limitN))
            //{
            //    PlugIn.ModelCore.UI.WriteLine("  A limit = NaN!  Will set to zero.");
            //    PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     GROWTH LIMITS: LAI={2:0.00}, H20={3:0.00}, N={4:0.00}, T={5:0.00}, Capacity={6:0.0}", PlugIn.ModelCore.CurrentTime, month + 1, limitLAI, limitH20, limitN, limitT, limitCapacity);
            //    PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     Other Information: MaxB={2}, Bsite={3}, Bcohort={4:0.0}, SoilT={5:0.0}.", PlugIn.ModelCore.CurrentTime, month + 1, maxBiomass, (int)siteBiomass, (cohort.WoodBiomass + cohort.LeafBiomass), SiteVars.SoilTemperature[site]);
            //}


            //  Age mortality is discounted from ANPP to prevent the over-
            //  estimation of growth.  ANPP cannot be negative.
            double actualANPP = Math.Max(0.0, potentialNPP - mortalityAge[0] - mortalityAge[1]);

            // Growth can be reduced by another extension via this method.
            // To date, this is only used by Biomass Browse and Biomass Insects
            double growthReduction = Landis.Library.BiomassCohorts.CohortGrowthReduction.Compute(cohort, site);

            if (growthReduction > 0.0)
            {
                actualANPP *= (1.0 - growthReduction);
            }

            double leafNPP  = actualANPP * leafFractionNPP;
            double woodNPP  = actualANPP * (1.0 - leafFractionNPP);
                        
            if (Double.IsNaN(leafNPP) || Double.IsNaN(woodNPP))
            {
                PlugIn.ModelCore.UI.WriteLine("  EITHER WOOD or LEAF NPP = NaN!  Will set to zero.");
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     Other Information: MaxB={2}, Bsite={3}, Bcohort={4:0.0}, SoilT={5:0.0}.", PlugIn.ModelCore.CurrentTime, Main.Month + 1, SpeciesData.Max_Biomass[cohort.Species], (int)siteBiomass, (cohort.WoodBiomass + cohort.LeafBiomass), SiteVars.SoilTemperature[site]);
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     WoodNPP={2:0.00}, LeafNPP={3:0.00}.", PlugIn.ModelCore.CurrentTime, Main.Month + 1, woodNPP, leafNPP);
                if (Double.IsNaN(leafNPP))
                    leafNPP = 0.0;
                if (Double.IsNaN(woodNPP))
                    woodNPP = 0.0;

            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.limitLAI = limitLAI;
                CalibrateLog.limitH20 = limitH20;
                CalibrateLog.limitT = limitT;
                CalibrateLog.limitN = limitN;
                CalibrateLog.limitLAIcompetition = competition_limit; // Chihiro, 2021.03.26: added
                CalibrateLog.maxNPP = maxNPP;
                CalibrateLog.maxB = SpeciesData.Max_Biomass[cohort.Species];
                CalibrateLog.siteB = siteBiomass;
                CalibrateLog.cohortB = (cohort.WoodBiomass + cohort.LeafBiomass);
                CalibrateLog.soilTemp = SiteVars.SoilTemperature[site];
                CalibrateLog.actualWoodNPP = woodNPP;
                CalibrateLog.actualLeafNPP = leafNPP;
               
            }
                        
            return new double[2]{woodNPP, leafNPP};

        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes M_AGE_ij: the mortality caused by the aging of the cohort.
        /// See equation 6 in Scheller and Mladenoff, 2004.
        /// </summary>
        private double[] ComputeAgeMortality(ICohort cohort, ActiveSite site)
        {

            double monthAdjust = 1.0 / 12.0;
            double totalBiomass = (double) (cohort.WoodBiomass + cohort.LeafBiomass);
            double max_age      = (double) cohort.Species.Longevity;
            double d            = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].LongevityMortalityShape;

            double M_AGE_wood =    cohort.WoodBiomass *  monthAdjust *
                                    Math.Exp((double) cohort.Age / max_age * d) / Math.Exp(d);

            double M_AGE_leaf =    cohort.LeafBiomass *  monthAdjust *
                                    Math.Exp((double) cohort.Age / max_age * d) / Math.Exp(d);


            M_AGE_wood = Math.Min(M_AGE_wood, cohort.WoodBiomass);
            M_AGE_leaf = Math.Min(M_AGE_leaf, cohort.LeafBiomass);

            double[] M_AGE = new double[2]{M_AGE_wood, M_AGE_leaf};

            SiteVars.WoodMortality[site] += (M_AGE_wood);

            if(M_AGE_wood < 0.0 || M_AGE_leaf < 0.0)
            {
                PlugIn.ModelCore.UI.WriteLine("Mwood={0}, Mleaf={1}.", M_AGE_wood, M_AGE_leaf);
                throw new ApplicationException("Error: Woody or Leaf Age Mortality is < 0");
            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mortalityAGEleaf = M_AGE_leaf;
                CalibrateLog.mortalityAGEwood = M_AGE_wood;
            }

            return M_AGE;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Monthly mortality as a function of standing leaf and wood biomass.
        /// </summary>
        private double[] ComputeGrowthMortality(ICohort cohort, ActiveSite site, double siteBiomass, double[] AGNPP)
        {

            double maxBiomass = SpeciesData.Max_Biomass[cohort.Species];
            double NPPwood = (double)AGNPP[0];
            
            double M_wood_fixed = cohort.WoodBiomass * FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].MonthlyWoodMortality;
            double M_leaf = 0.0;

            double relativeBiomass = siteBiomass / maxBiomass;
            double M_constant = 5.0;  //This constant controls the rate of change of mortality with NPP

            //Functon which calculates an adjustment factor for mortality that ranges from 0 to 1 and exponentially increases with relative biomass.
            double M_wood_NPP = Math.Max(0.0, (Math.Exp(M_constant * relativeBiomass) - 1.0) / (Math.Exp(M_constant) - 1.0));
            M_wood_NPP = Math.Min(M_wood_NPP, 1.0);
            //PlugIn.ModelCore.UI.WriteLine("relativeBiomass={0}, siteBiomass={1}.", relativeBiomass, siteBiomass);

            //This function calculates mortality as a function of NPP 
            double M_wood = (NPPwood * M_wood_NPP) + M_wood_fixed;

            //PlugIn.ModelCore.UI.WriteLine("Mwood={0}, M_wood_relative={1}, NPPwood={2}, Spp={3}, Age={4}.", M_wood, M_wood_NPP, NPPwood, cohort.Species.Name, cohort.Age);


            // Leaves and Needles dropped.
            if (SpeciesData.LeafLongevity[cohort.Species] > 1.0) 
            {
                M_leaf = cohort.LeafBiomass / (double) SpeciesData.LeafLongevity[cohort.Species] / 12.0;  //Needle deposit spread across the year.
               
            }
            else
            {
                if(Main.Month +1 == FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth)
                {
                    M_leaf = cohort.LeafBiomass / 2.0;  //spread across 2 months
                    
                }
                if (Main.Month +2 > FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth)
                {
                    M_leaf = cohort.LeafBiomass;  //drop the remainder
                }
            }

            double[] M_BIO = new double[2]{M_wood, M_leaf};

            if(M_wood < 0.0 || M_leaf < 0.0)
            {
                PlugIn.ModelCore.UI.WriteLine("Mwood={0}, Mleaf={1}.", M_wood, M_leaf);
                throw new ApplicationException("Error: Wood or Leaf Growth Mortality is < 0");
            }

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mortalityBIOwood = M_wood;
                CalibrateLog.mortalityBIOleaf = M_leaf;
            }

            SiteVars.WoodMortality[site] += (M_wood);

            return M_BIO;

        }


        //---------------------------------------------------------------------

        private void UpdateDeadBiomass(ICohort cohort, ActiveSite site, double[] totalMortality)
        {


            double mortality_wood    = (double) totalMortality[0];
            double mortality_nonwood = (double)totalMortality[1];


            //  Add mortality to dead biomass pools.
            //  Coarse root mortality is assumed proportional to aboveground woody mortality
            //    mass is assumed 25% of aboveground wood (White et al. 2000, Niklas & Enquist 2002)
            if (mortality_wood > 0.0 && !SpeciesData.Grass[cohort.Species])
            {
                ForestFloor.AddWoodLitter(mortality_wood, cohort.Species, site);
                Roots.AddCoarseRootLitter(mortality_wood, cohort, cohort.Species, site);
            }

            //  Wood biomass of grass species is transfered to non wood litter. (W.Hotta 2021.12.16)
            if (mortality_wood > 0.0 && SpeciesData.Grass[cohort.Species])
            {
                AvailableN.AddResorbedN(cohort, totalMortality[0], site); //ignoring input from scorching, which is rare, but not resorbed.             
                ForestFloor.AddResorbedFoliageLitter(mortality_wood, cohort.Species, site);
                Roots.AddFineRootLitter(mortality_wood, cohort, cohort.Species, site);
            }

            if (mortality_nonwood > 0.0)
            {
                AvailableN.AddResorbedN(cohort, totalMortality[1], site); //ignoring input from scorching, which is rare, but not resorbed.             
                ForestFloor.AddResorbedFoliageLitter(mortality_nonwood, cohort.Species, site);
                Roots.AddFineRootLitter(mortality_nonwood, cohort, cohort.Species, site);
            }

            return;

        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the initial biomass for a cohort at a site.
        /// </summary>
        public static float[] InitialBiomass(ISpecies species, ISiteCohorts siteCohorts,
                                            ActiveSite site)
        {
            IEcoregion ecoregion = PlugIn.ModelCore.Ecoregion[site];

            double leafFrac = FunctionalType.Table[SpeciesData.FuncType[species]].FractionANPPtoLeaf;

            double B_ACT = SiteVars.ActualSiteBiomass(site);
            double B_MAX = SpeciesData.Max_Biomass[species]; 

            //  Initial biomass exponentially declines in response to
            //  competition.
            double initialBiomass = 0.002 * B_MAX * Math.Exp(-1.6 * B_ACT / B_MAX);

            initialBiomass = Math.Max(initialBiomass, 5.0);

            double initialLeafB = initialBiomass * leafFrac;
            double initialWoodB = initialBiomass - initialLeafB;
            double[] initialB = new double[2] { initialWoodB, initialLeafB };
            float[] initialWoodLeafBiomass = new float[2] { (float)initialB[0], (float)initialB[1] };

            return initialWoodLeafBiomass;
        }


        //---------------------------------------------------------------------
        /// <summary>
        /// Summarize NPP
        /// </summary>
        private static void CalculateNPPcarbon(ActiveSite site, ICohort cohort, double[] AGNPP)
        {
            double NPPwood = (double) AGNPP[0] * 0.47;
            double NPPleaf = (double) AGNPP[1] * 0.47;

            double NPPcoarseRoot = Roots.CalculateCoarseRoot(cohort, NPPwood);
            double NPPfineRoot = Roots.CalculateFineRoot(cohort, NPPleaf);

            if (Double.IsNaN(NPPwood) || Double.IsNaN(NPPleaf) || Double.IsNaN(NPPcoarseRoot) || Double.IsNaN(NPPfineRoot))
            {
                PlugIn.ModelCore.UI.WriteLine("  EITHER WOOD or LEAF NPP or COARSE ROOT or FINE ROOT = NaN!  Will set to zero.");
                PlugIn.ModelCore.UI.WriteLine("  Yr={0},Mo={1}.     WoodNPP={0}, LeafNPP={1}, CRootNPP={2}, FRootNPP={3}.", NPPwood, NPPleaf, NPPcoarseRoot, NPPfineRoot);
                if (Double.IsNaN(NPPleaf))
                    NPPleaf = 0.0;
                if (Double.IsNaN(NPPwood))
                    NPPwood = 0.0;
                if (Double.IsNaN(NPPcoarseRoot))
                    NPPcoarseRoot = 0.0;
                if (Double.IsNaN(NPPfineRoot))
                    NPPfineRoot = 0.0;
            }


            SiteVars.AGNPPcarbon[site] += NPPwood + NPPleaf;
            SiteVars.BGNPPcarbon[site] += NPPcoarseRoot + NPPfineRoot;
            SiteVars.MonthlyAGNPPcarbon[site][Main.Month] += NPPwood + NPPleaf;
            SiteVars.MonthlyBGNPPcarbon[site][Main.Month] += NPPcoarseRoot + NPPfineRoot;
            SiteVars.MonthlySoilResp[site][Main.Month] += (NPPcoarseRoot + NPPfineRoot) * 0.53/0.47;
        }

        //--------------------------------------------------------------------------
        //N limit is actual demand divided by maximum uptake.
        private double calculateN_Limit(ActiveSite site, ICohort cohort, double NPP, double leafFractionNPP)
        {

            //Get Cohort Mineral and Resorbed N allocation.
            double mineralNallocation = AvailableN.GetMineralNallocation(cohort);
            double resorbedNallocation = AvailableN.GetResorbedNallocation(cohort, site);

            double LeafNPP = (NPP * leafFractionNPP);
            
            double WoodNPP = NPP * (1.0 - leafFractionNPP); 
         
            double limitN = 0.0;
            if (SpeciesData.NFixer[cohort.Species])
                limitN = 1.0;  // No limit for N-fixing shrubs
            else
            {
                // Divide allocation N by N demand here:
                //PlugIn.ModelCore.UI.WriteLine("  WoodNPP={0:0.00}, LeafNPP={1:0.00}, FineRootNPP={2:0.00}, CoarseRootNPP={3:0.00}.", WoodNPP, LeafNPP);
               double Ndemand = (AvailableN.CalculateCohortNDemand(cohort.Species, site, cohort, new double[] { WoodNPP, LeafNPP})); 

                if (Ndemand > 0.0)
                {
                    limitN = Math.Min(1.0, (mineralNallocation + resorbedNallocation) / Ndemand);
                    //PlugIn.ModelCore.UI.WriteLine("mineralN={0}, resorbedN={1}, Ndemand={2}", mineralNallocation, resorbedNallocation, Ndemand);

                }
                else
                    limitN = 1.0; // No demand means that it is a new or very small cohort.  Will allow it to grow anyways.                
            }


            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.mineralNalloc = mineralNallocation;
                CalibrateLog.resorbedNalloc = resorbedNallocation;
            }

            return Math.Max(limitN, 0.0);
        }
        //--------------------------------------------------------------------------
        // Originally from lacalc.f of CENTURY model

        private static double calculateLAI_Limit(ICohort cohort, ActiveSite site)
        {

            //...Calculate true LAI using leaf biomass and a biomass-to-LAI
            //     conversion parameter which is the slope of a regression
            //     line derived from LAI vs Foliar Mass for Slash Pine.

            //...Calculate theoretical LAI as a function of large wood mass.
            //     There is no strong consensus on the true nature of the relationship
            //     between LAI and stemwood mass.  Many sutdies have cited as "general"
            //      an increase of LAI up to a maximum, then a decrease to a plateau value
            //     (e.g. Switzer et al. 1968, Gholz and Fisher 1982).  However, this
            //     response is not general, and seems to mostly be a feature of young
            //     pine plantations.  Northern hardwoods have shown a monotonic increase
            //     to a plateau  (e.g. Switzer et al. 1968).  Pacific Northwest conifers
            //     have shown a steady increase in LAI with no plateau evident (e.g.
            //     Gholz 1982).  In this version, we use a simple saturation fucntion in
            //     which LAI increases linearly against large wood mass initially, then
            //     approaches a plateau value.  The plateau value can be set very large to
            //     give a response of steadily increasing LAI with stemwood.

            //     References:
            //             1)  Switzer, G.L., L.E. Nelson and W.H. Smith 1968.
            //                 The mineral cycle in forest stands.  'Forest
            //                 Fertilization:  Theory and Practice'.  pp 1-9
            //                 Tenn. Valley Auth., Muscle Shoals, AL.
            //
            //             2)  Gholz, H.L., and F.R. Fisher 1982.  Organic matter
            //                 production and distribution in slash pine (Pinus
            //                 elliotii) plantations.  Ecology 63(6):  1827-1839.
            //
            //             3)  Gholz, H.L.  1982.  Environmental limits on aboveground
            //                 net primary production and biomass in vegetation zones of
            //                 the Pacific Northwest.  Ecology 63:469-481.

            //...Local variables
            double leafC = (double) cohort.LeafBiomass * 0.47;
            double woodC = (double) cohort.WoodBiomass * 0.47;

            double lai = 0.0;
            double lai_to_growth = SpeciesData.GrowthLAI[cohort.Species] * -1.0;
            double btolai = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].BiomassToLAI;
            double klai   = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].KLAI;
            double maxlai = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].MaxLAI;
            double minlai = FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].MinLAI;
            double monthly_cumulative_LAI = SiteVars.MonthlyLAI[site][Main.Month];
            
            // adjust for leaf on/off
            double seasonal_adjustment = 1.0;
            if (SpeciesData.LeafLongevity[cohort.Species] <= 1.0)
            {
                seasonal_adjustment = (Math.Max(0.0, 1.0 - Math.Exp(btolai * leafC)));
            }

           
            // The cohort LAI given wood Carbon
            double base_lai = maxlai * woodC/(klai + woodC);

            //...Choose the LAI reducer on production. 
            lai = base_lai * seasonal_adjustment;

            // This will allow us to set MAXLAI to zero such that LAI is completely dependent upon
            // foliar carbon, which may be necessary for simulating defoliation events.
            if(base_lai <= 0.0) lai = seasonal_adjustment;

            // The minimum LAI to calculate effect is 0.1.
            if (lai < minlai) lai = minlai;

            double LAI_Growth_limit = Math.Max(0.0, 1.0 - Math.Exp(lai_to_growth * lai));

            //This allows LAI to go to zero for deciduous trees.

            if (SpeciesData.LeafLongevity[cohort.Species] <= 1.0 &&
                (Main.Month > FunctionalType.Table[SpeciesData.FuncType[cohort.Species]].FoliageDropMonth || Main.Month < 3))
            {
                lai = 0.0;
                LAI_Growth_limit = 0.0;
            }

            if (Main.Month == 6)
                SiteVars.LAI[site] += lai; //Tracking LAI.

            SiteVars.MonthlyLAI[site][Main.Month] += lai;

            // Chihiro 2021.02.23
            // Tracking Tree species LAI above grasses
            if (!SpeciesData.Grass[cohort.Species])
                SiteVars.MonthlyLAI_Trees[site][Main.Month] += lai;
            else
                SiteVars.MonthlyLAI_Grasses[site][Main.Month] += lai; // Chihiro, 2021.03.30: tentative
                



            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.actual_LAI = lai;
                if (!SpeciesData.Grass[cohort.Species])
                    CalibrateLog.actual_LAI_tree = lai;
                else
                    CalibrateLog.actual_LAI_tree = 0;
                CalibrateLog.base_lai = base_lai;
                CalibrateLog.seasonal_adjustment = seasonal_adjustment;
                CalibrateLog.siteLAI = SiteVars.MonthlyLAI[site][Main.Month]; // Chihiro, 2021.03.26: added
            }


            return LAI_Growth_limit;

        }

        private static double calculate_LAI_Competition(ICohort cohort, ActiveSite site)
        {
            double k = -0.14;  
            // This is the value given for all temperature ecosystems. 
            // The model is relatively insensitive to this parameter ZR 06/01/2021

            //   Chihiro 2020.01.22
            //   Competition between cohorts considering understory and overstory interactions
            //   If the biomass of tree cohort is larger than total grass biomass on the site, 
            //   monthly_cummulative_LAI should ignore grass LAI.
            //
            //   Added GrassThresholdMultiplier (W.Hotta 2020.07.07)
            //   grassThresholdMultiplier: User defined parameter to adjust relationships between AGB and Hight of the cohort
            //   default = 1.0
            //
            //   if (the cohort is tree species) and (biomass_of_tree_cohort > total_grass_biomass_on_the_site * threshold_multiplier):
            //     monthly_cummulative_LAI = Monthly_LAI_of_tree_species
            //   else:
            //     monthly_cummulative_LAI = Monthly_LAI_of_tree_& grass_species
            //  
            double monthly_cumulative_LAI = 0.0;
            double grassThresholdMultiplier = PlugIn.Parameters.GrassThresholdMultiplier;
            // PlugIn.ModelCore.UI.WriteLine("TreeLAI={0},TreeLAI={0}", SiteVars.MonthlyLAITree[site][Main.Month], SiteVars.MonthlyLAI[site][Main.Month]); // added (W.Hotta 2020.07.07)
            // PlugIn.ModelCore.UI.WriteLine("Spp={0},Time={1},Mo={2},cohortBiomass={3},grassBiomass={4},LAI={5}", cohort.Species.Name, PlugIn.ModelCore.CurrentTime, Main.Month + 1, cohort.Biomass, Main.ComputeGrassBiomass(site), monthly_cumulative_LAI); // added (W.Hotta 2020.07.07)

            if (!SpeciesData.Grass[cohort.Species] &&
                cohort.Biomass > ComputeGrassBiomass(site) * grassThresholdMultiplier)
            {
                monthly_cumulative_LAI = SiteVars.MonthlyLAI_Trees[site][Main.Month];
                // PlugIn.ModelCore.UI.WriteLine("Higher than Sasa");  // added (W.Hotta 2020.07.07)
            }
            else
            {
                // monthly_cumulative_LAI = SiteVars.MonthlyLAI[site][Main.Month];
                monthly_cumulative_LAI = SiteVars.MonthlyLAI_Trees[site][Main.Month] + SiteVars.MonthlyLAI_GrassesLastMonth[site]; // Chihiro, 2021.03.30: tentative. trees + grass layer
            }

            double competition_limit = Math.Max(0.0, Math.Exp(k * monthly_cumulative_LAI));

            return competition_limit;

        }

        //---------------------------------------------------------------------------
        //... Originally from CENTURY

        //...This funtion returns a value for potential plant production
        //     due to water content.  Basically you have an equation of a
        //     line with a moveable y-intercept depending on the soil type.

        //     pprpts(1):  The minimum ratio of available water to pet which
        //                 would completely limit production assuming wc=0.
        //     pprpts(2):  The effect of wc on the intercept, allows the
        //                 user to increase the value of the intercept and
        //                 thereby increase the slope of the line.
        //     pprpts(3):  The lowest ratio of available water to pet at which
        //                 there is no restriction on production.
        private static double calculateWater_Limit(ActiveSite site, IEcoregion ecoregion, ISpecies species)
        {

            // Ratio_AvailWaterToPET used to be pptprd and WaterLimit used to be pprdwc
            double Ratio_AvailWaterToPET = 0.0;
            double waterContent = SiteVars.SoilFieldCapacity[site] - SiteVars.SoilWiltingPoint[site];
            double tmin = ClimateRegionData.AnnualWeather[ecoregion].MonthlyMinTemp[Main.Month];
            double H2Oinputs = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPrecip[Main.Month]; //rain + irract;
            double pet = ClimateRegionData.AnnualWeather[ecoregion].MonthlyPET[Main.Month];
            
            if (pet >= 0.01)
            {   
                Ratio_AvailWaterToPET = (SiteVars.AvailableWater[site] / pet);  //Modified by ML so that we weren't double-counting precip as in above equation
            }
            else Ratio_AvailWaterToPET = 0.01;

            //...The equation for the y-intercept (intcpt) is A+B*WC.  A and B
            //     determine the effect of soil texture on plant production based
            //     on moisture.

            //PPRPTS naming convention is imported from orginal Century model. Now replaced with 'MoistureCurve' to be more intuitive
            //...New way (with updated naming convention):

            double moisturecurve1 = 0.0; // OtherData.MoistureCurve1;
            double moisturecurve2 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve2;
            double moisturecurve3 = FunctionalType.Table[SpeciesData.FuncType[species]].MoistureCurve3;

            double intcpt = moisturecurve1 + (moisturecurve2 * waterContent);
            double slope = 1.0 / (moisturecurve3 - intcpt);

            double WaterLimit = 1.0 + slope * (Ratio_AvailWaterToPET - moisturecurve3);
              
            if (WaterLimit > 1.0)  WaterLimit = 1.0;
            if (WaterLimit < 0.01) WaterLimit = 0.01;

            //PlugIn.ModelCore.UI.WriteLine("Intercept={0}, Slope={1}, WaterLimit={2}.", intcpt, slope, WaterLimit);     

            if (PlugIn.ModelCore.CurrentTime > 0 && OtherData.CalibrateMode)
            {
                CalibrateLog.availableWater = SiteVars.AvailableWater[site];
                //Outputs.CalibrateLog.Write("{0:0.00},", SiteVars.AvailableWater[site]);
            }

            return WaterLimit;
        }


        //-----------
        private double calculateTemp_Limit(ActiveSite site, ISpecies species)
        {
            //Originally from gpdf.f of CENTURY model
            //It calculates the limitation of soil temperature on aboveground forest potential production.
            //...This routine is functionally equivalent to the routine of the
            //     same name, described in the publication:

            //       Some Graphs and their Functional Forms
            //       Technical Report No. 153
            //       William Parton and George Innis (1972)
            //       Natural Resource Ecology Lab.
            //       Colorado State University
            //       Fort collins, Colorado  80523

            double A1 = SiteVars.SoilTemperature[site];
            double A2 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve1;
            double A3 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve2;
            double A4 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve3;
            double A5 = FunctionalType.Table[SpeciesData.FuncType[species]].TempCurve4;

            double frac = (A3-A1) / (A3-A2);
            double U1 = 0.0;
            if (frac > 0.0)
                U1 = Math.Exp(A4 / A5 * (1.0 - Math.Pow(frac, A5))) * Math.Pow(frac, A4);

            //PlugIn.ModelCore.UI.WriteLine("  TEMPERATURE Limits:  Month={0}, Soil Temp={1:0.00}, Temp Limit={2:0.00}. [PPDF1={3:0.0},PPDF2={4:0.0},PPDF3={5:0.0},PPDF4={6:0.0}]", month+1, A1, U1,A2,A3,A4,A5);

            return U1;
        }
        //---------------------------------------------------------------------
        // Chihiro 2020.01.22
        public static double ComputeGrassBiomass(ActiveSite site)
        {
            double grassTotal = 0;
            if (SiteVars.Cohorts[site] != null)
                foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
                    foreach (ICohort cohort in speciesCohorts)
                        if (SpeciesData.Grass[cohort.Species])
                            grassTotal += cohort.WoodBiomass;
            return grassTotal;
        }

    }
}
