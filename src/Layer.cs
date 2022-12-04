//  Author: Robert Scheller, Melissa Lucash

using Landis.Utilities;
using System;
using System.Threading;
using Landis.Core;
using Landis.SpatialModeling;


namespace Landis.Extension.Succession.NECN
{

    // Chihiro; add grass layer to track dead wood in grass species
    public enum LayerName { Leaf, FineRoot, Wood, CoarseRoot, Metabolic, Structural, SOM1, SOM2, SOM3, Other };
    public enum LayerType {Surface, Soil, Other} 

    /// <summary>
    /// A Century soil model carbon and nitrogen pool.
    /// </summary>
    public class Layer
    {
        private LayerName name;
        private LayerType type;
        private double carbon;
        private double nitrogen;
        private double decayValue;
        private double fractionLignin;
        private double netMineralization;
        private double grossMineralization;


        //---------------------------------------------------------------------
        public Layer(LayerName name, LayerType type)
        {
            this.name = name;
            this.type = type;
            this.carbon = 0.0;
            this.nitrogen = 0.0;

            // Initial decay value of Wood is set to mean decay rate of all tree species.
            // 
            // 
            // if this.name is LayerName.Wood:
            //     double decayvalue = 0.0
            //     double N_of_species = len(sppnames)
            //     // Compute mean wood decay rate
            //     for sppname in tree-sppnames:
            //         decayrate += decayrate(sppname)
            //     this.decayVlaue = decayvalue / N_of_species
            //
            // elif this.name is LayerName.Grass:
            //     double decayvalue = 0.0
            //     double N_of_species = len(sppnames)
            //     // Compute mean wood decay rate
            //     for sppname in grass-sppnames:
            //         decayrate += decayrate(sppname)
            //     this.decayVlaue = decayvalue / N_of_species
            //
            // else:
            //     this.decayValue = 0.0
            //
            if (this.name == LayerName.Wood)
            {
                // Compute mean decay value
                double decayvalue = 0.0;
                double n_of_tree_species = 0.0;
                foreach (ISpecies species in PlugIn.ModelCore.Species)
                {
                    if (!SpeciesData.Grass[species])
                        decayvalue += FunctionalType.Table[SpeciesData.FuncType[species]].WoodDecayRate;
                        n_of_tree_species += 1;
                }
                this.decayValue = decayvalue / n_of_tree_species;
            }
            else
            {
                this.decayValue = 0.0;
            }
            //this.decayValue = 0.0;
            this.fractionLignin = 0.0;

            this.netMineralization = 0.0;
            this.grossMineralization = 0.0;

        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Layer Name
        /// </summary>
        public LayerName Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Provides an index to LitterTypeTable
        /// </summary>
        public LayerType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Carbon
        /// </summary>
        public double Carbon
        {
            get
            {
                return carbon;
            }
            set
            {
                carbon = Math.Max(0.0, value);
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Nitrogen
        /// </summary>
        public double Nitrogen
        {
            get
            {
                return nitrogen;
            }
            set
            {
                nitrogen = Math.Max(0.0, value);
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Pool decay rate.
        /// </summary>
        public  double DecayValue
        {
            get
            {
                return decayValue;
            }
            set
            {
                decayValue = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Pool Carbon:Nitrogen Ratio
        /// </summary>
        public  double FractionLignin
        {
            get
            {
                return fractionLignin;
            }
            set
            {
                fractionLignin = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Net Mineralization
        /// </summary>
        public double NetMineralization
        {
            get
            {
                return netMineralization;
            }
            set
            {
                netMineralization = value;
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        /// Gross Mineralization
        /// </summary>
        public double GrossMineralization
        {
            get
            {
                return grossMineralization;
            }
            set
            {
                grossMineralization = value;
            }
        }

        // --------------------------------------------------
        public Layer Clone()
        {
            Layer newLayer = new Layer(this.Name, this.Type);

            newLayer.carbon = this.carbon;
            newLayer.nitrogen = this.nitrogen ;

            newLayer.decayValue = this.decayValue ;
            newLayer.fractionLignin = this.fractionLignin ;

            newLayer.netMineralization = this.netMineralization ;
            newLayer.grossMineralization = this.grossMineralization ;

            return newLayer;
        }

        // --------------------------------------------------
        public void DecomposeStructural(ActiveSite site)
        {
            if (this.Carbon > 0.0000001)
            {

                double anerb = SiteVars.AnaerobicEffect[site];

                if (this.Type == LayerType.Surface) anerb = 1.0; // No anaerobic effect on surface material

                //Compute total C flow out of structural in layer
                double totalCFlow = //System.Math.Min(this.Carbon, OtherData.MaxStructuralC)
                                this.Carbon
                                * SiteVars.DecayFactor[site]
                                //* OtherData.LitterParameters[(int)this.Type].DecayRateStrucC
                                // v6:  Replace the fixed value (0.39) with the user input value for surficial C decay
                                * PlugIn.Parameters.DecayRateSurf
                                * anerb
                                * System.Math.Exp(-1.0 * OtherData.LigninDecayEffect * this.FractionLignin)
                                * OtherData.MonthAdjust;

                //Decompose structural into som1 and som2 with CO2 loss.

                if (totalCFlow > this.Carbon)
                {
                    string mesg = string.Format("Error: Decompose Structural totalCFlow > this.Carbon:  totalCFlow={0}, DecayFactor={1}, Anerb={2}", totalCFlow, SiteVars.DecayFactor[site], anerb);
                    throw new ApplicationException(mesg);
                }


                this.DecomposeLignin(totalCFlow, site);
            }
        }
        // --------------------------------------------------
        // Only wood contains lignin
        public void DecomposeLignin(double totalCFlow, ActiveSite site)
        {
            double carbonToSOM1;    //Net C flow to SOM1
            double carbonToSOM2;    //Net C flow to SOM2
            double litterC = this.Carbon; 
            double ratioCN = litterC / this.Nitrogen;


            //See if Layer can decompose to SOM1.
            //If it can decompose to SOM1, it will also go to SOM2.
            //If it can't decompose to SOM1, it can't decompose at all.

            //If Wood Object can decompose
            if (this.DecomposePossible(ratioCN, SiteVars.MineralN[site]))
            {
                // Decompose Wood Object to SOM2
                // -----------------------
                // Gross C flow to som2
                carbonToSOM2 = totalCFlow * this.FractionLignin;

                //MicrobialRespiration associated with decomposition to som2
                double SOM2co2loss = carbonToSOM2 * OtherData.LigninRespirationRate;

                if (this.Type == LayerType.Surface)
                    this.Respiration(SOM2co2loss, site, true);
                else
                    this.Respiration(SOM2co2loss, site, false);

                //Net C flow to SOM2
                double netCFlow = carbonToSOM2 - SOM2co2loss;

                // Partition and schedule C flows 
                this.TransferCarbon(SiteVars.SOM2[site], netCFlow);
                this.TransferNitrogen(SiteVars.SOM2[site], netCFlow, litterC, ratioCN, site);

                // ----------------------------------------------
                // Decompose Wood Object to SOM1
                // Gross C flow to som1

                carbonToSOM1 = totalCFlow - netCFlow;

                double SOM1co2loss;

                //MicrobialRespiration associated with decomposition to som1
                if (this.Type == LayerType.Surface)
                {
                    SOM1co2loss = carbonToSOM1 * OtherData.StructuralToCO2Surface;
                    this.Respiration(SOM1co2loss, site, true);
                }
                else
                {
                    SOM1co2loss = carbonToSOM1 * OtherData.StructuralToCO2Soil;
                    this.Respiration(SOM1co2loss, site, false);
                }


                //Net C flow to SOM1
                carbonToSOM1 -= SOM1co2loss;

                if(this.Type == LayerType.Surface)
                {
                    if (carbonToSOM1 > this.Carbon)
                    {
                        string mesg = string.Format("Error: Carbon transfer SOM1surface->SOM1 exceeds C flow. Source={0}, C-transfer={1}, C={2}", this.Name, carbonToSOM1, this.Carbon);
                        throw new ApplicationException(mesg);
                    }

                    this.TransferCarbon(SiteVars.SOM1surface[site], carbonToSOM1);
                    this.TransferNitrogen(SiteVars.SOM1surface[site], carbonToSOM1, litterC, ratioCN, site);
                }
                else
                {
                    if (carbonToSOM1 > this.Carbon)
                    {
                        string mesg = string.Format("Error: Carbon transfer SOM1soil->SOM1 exceeds C flow.  Source={0}, C-transfer={1}, C={2}", this.Name, carbonToSOM1, this.Carbon);
                        throw new ApplicationException(mesg);
                    }

                    this.TransferCarbon(SiteVars.SOM1soil[site], carbonToSOM1);
                    this.TransferNitrogen(SiteVars.SOM1soil[site], carbonToSOM1, litterC, ratioCN, site);
                }
            }
            //PlugIn.ModelCore.UI.WriteLine("Decompose2.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
            return;
        }

        //---------------------------------------------------------------------
        public void DecomposeMetabolic(ActiveSite site)
        {
            double litterC = this.Carbon;
            double anerb = SiteVars.AnaerobicEffect[site];

            if (litterC > 0.0000001)
            {
              // Determine C/N ratios for flows to SOM1
                double ratioCNtoSOM1 = 0.0;
                double co2loss = 0.0;

                // Compute ratios for surface  metabolic residue
                if (this.Type == LayerType.Surface)
                    ratioCNtoSOM1 = Layer.AbovegroundDecompositionRatio(this.Nitrogen, litterC);

                //Compute ratios for soil metabolic residue
                else
                    ratioCNtoSOM1 = Layer.BelowgroundDecompositionRatio(site,
                                        OtherData.MinCNenterSOM1,
                                        OtherData.MaxCNenterSOM1,
                                        OtherData.MinContentN_SOM1);

                //Compute total C flow out of metabolic layer
                double totalCFlow = litterC
                                * SiteVars.DecayFactor[site]
                                * OtherData.LitterParameters[(int) this.Type].DecayRateMetabolicC
                                * OtherData.MonthAdjust;

                //PlugIn.ModelCore.UI.WriteLine("DecomposeMeta1.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                //Added impact of soil anerobic conditions
                if (this.Type == LayerType.Soil) totalCFlow *= anerb;

                //Make sure metabolic C does not go negative.
                if (totalCFlow > litterC)
                    totalCFlow = litterC;

                //If decomposition can occur,
                if(this.DecomposePossible(ratioCNtoSOM1, SiteVars.MineralN[site]))
                {
                    //CO2 loss
                    if (this.Type == LayerType.Surface)
                    {
                        co2loss = totalCFlow * OtherData.MetabolicToCO2Surface;
                        //this.Respiration(co2loss, site, true);
                    }
                    else
                    {
                        co2loss = totalCFlow * OtherData.MetabolicToCO2Soil;
                        //this.Respiration(co2loss, site, false);
                    }

                    this.Respiration(co2loss, site, false);  //SURFACE DECAY ALSO COUNTED AS SOIL RESPIRATION (Shih-Chieh Chang)
                    //PlugIn.ModelCore.UI.WriteLine("BeforeResp.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                    //PlugIn.ModelCore.UI.WriteLine("AfterResp.  MineralN={0:0.00}.", SiteVars.MineralN[site]);

                    //Decompose metabolic into som1
                    double netCFlow = totalCFlow - co2loss;

                    if (netCFlow > litterC)
                        PlugIn.ModelCore.UI.WriteLine("   ERROR:  Decompose Metabolic:  netCFlow={0:0.000} > layer.Carbon={0:0.000}.", netCFlow, this.Carbon);

                    // -- CARBON AND NITROGEN ---------------------------
                    // Partition and schedule C flows
                    // Compute and schedule N flows and update mineralization accumulators.
                    if((int) this.Type == (int) LayerType.Surface)
                    {
                        this.TransferCarbon(SiteVars.SOM1surface[site], netCFlow);
                        this.TransferNitrogen(SiteVars.SOM1surface[site], netCFlow, litterC, ratioCNtoSOM1, site);
                        //PlugIn.ModelCore.UI.WriteLine("DecomposeMetabolic.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                    }
                    else
                    {
                        this.TransferCarbon(SiteVars.SOM1soil[site], netCFlow);
                        this.TransferNitrogen(SiteVars.SOM1soil[site], netCFlow, litterC, ratioCNtoSOM1, site);
                    }

                }
            }
        //}
        }
        //---------------------------------------------------------------------
        public void TransferCarbon(Layer destination, double netCFlow)
        {
            if (netCFlow < 0)
            {
                //PlugIn.ModelCore.UI.WriteLine("NEGATIVE C FLOW!  Source: {0},{1}; Destination: {2},{3}.", this.Name, this.Type, destination.Name, destination.Type);
            }

            if (netCFlow > this.Carbon)
                netCFlow = this.Carbon;

            //round these to avoid unexpected behavior
            this.Carbon = this.Carbon - netCFlow; // Math.Round((this.Carbon - netCFlow),2);
            destination.Carbon = destination.Carbon + netCFlow; // Math.Round((destination.Carbon + netCFlow),2);
        }

        public void TransferNitrogen(Layer destination, double CFlow, double totalC, double ratioCNtoDestination, ActiveSite site)
        {
            // this is the source.

            double mineralNFlow = 0.0;

            //...N flow is proportional to C flow.
            double NFlow = this.Nitrogen * CFlow / totalC;

            //...This was added to avoid a 0/0 error on the pc.
            if (CFlow <= 0.0 || NFlow <= 0.0)
            {
                return;
            }

            if ((NFlow - this.Nitrogen) > 0.01)
            {
                //PlugIn.ModelCore.UI.WriteLine("  Transfer N:  N flow > source N.");
                //PlugIn.ModelCore.UI.WriteLine("     NFlow={0:0.000}, SourceN={1:0.000}", NFlow, this.Nitrogen);
                //PlugIn.ModelCore.UI.WriteLine("     CFlow={0:0.000}, totalC={1:0.000}", CFlow, totalC);
                //PlugIn.ModelCore.UI.WriteLine("     this.Name={0}, this.Type={1}", this.Name, this.Type);
                //PlugIn.ModelCore.UI.WriteLine("     dest.Name  ={0}, dest.Type  ={1}", destination.Name, destination.Type);
                //PlugIn.ModelCore.UI.WriteLine("     ratio CN to dest={0}", ratioCNtoDestination);
           }

            //...If C/N of Box A > C/N of new material entering Box B
            if ((CFlow / NFlow) > ratioCNtoDestination)
            {
               //...IMMOBILIZATION occurs.
               //...Compute the amount of N immobilized.
               //     since  ratioCNtoDestination = netCFlow / (Nflow + immobileN),
               //     where immobileN is the extra N needed from the mineral pool
                double immobileN = (CFlow / ratioCNtoDestination) - NFlow;
                
                //PlugIn.ModelCore.UI.WriteLine("     CFlow={0:0.000}, totalC={1:0.000}", CFlow, totalC);
               
               // PlugIn.ModelCore.UI.WriteLine("     this.Name={0}, this.Type={1}", this.Name, this.Type);
                //PlugIn.ModelCore.UI.WriteLine("     NFlow={0:0.000}, SourceN={1:0.000},CNdestination={2:0}", NFlow, this.Nitrogen,ratioCNtoDestination);

                //PlugIn.ModelCore.UI.WriteLine("CalculatingImmobil.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                //...Schedule flow from Box A to Box B (outofa)
                //flow(anps,bnps,time,outofa);
                this.Nitrogen -= NFlow;
                destination.Nitrogen += NFlow;

                //PlugIn.ModelCore.UI.WriteLine("NFlow.  MineralN={0:0.00}, ImmobileN={1:0.000}.", SiteVars.MineralN[site],immobileN);

                // Schedule flow from mineral pool to Box B (immobileN)
                // flow(labile,bnps,time,immflo);
                //Don't allow mineral N to go to zero or negative.- ML
                
                if (immobileN > SiteVars.MineralN[site])
                    immobileN = SiteVars.MineralN[site] - 0.01; //leave some small amount of mineral N


                SiteVars.MineralN[site] -= immobileN;
                //PlugIn.ModelCore.UI.WriteLine("AfterImmobil.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                destination.Nitrogen += immobileN;

                //PlugIn.ModelCore.UI.WriteLine("AdjustImmobil.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
                //PlugIn.ModelCore.UI.WriteLine("   TransferN immobileN={0:0.000}, C={1:0.000}, N={2:0.000}, ratioCN={3:0.000}.", immobileN, CFlow, NFlow, ratioCNtoDestination);
                //PlugIn.ModelCore.UI.WriteLine("     source={0}-{1}, destination={2}-{3}.", this.Name, this.Type, destination.Name, destination.Type);

                //...Return mineralization value.
                mineralNFlow = -1 * immobileN;
                //PlugIn.ModelCore.UI.WriteLine("MineralNflow.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
            }
            else

                //...MINERALIZATION occurs
                //...Schedule flow from Box A to Box B
            {
                //PlugIn.ModelCore.UI.WriteLine("  Transfer Nitrogen Min.");
                double mineralizedN = (CFlow / ratioCNtoDestination);
                

                this.Nitrogen -= mineralizedN;
                destination.Nitrogen += mineralizedN;

                //...Schedule flow from Box A to mineral pool

                mineralNFlow = NFlow - mineralizedN;

                if ((mineralNFlow - this.Nitrogen) > 0.01) 
                {
                    //PlugIn.ModelCore.UI.WriteLine("  Transfer N mineralization:  mineralN > source N.");
                    //PlugIn.ModelCore.UI.WriteLine("     MineralNFlow={0:0.000}, SourceN={1:0.000}", mineralNFlow, this.Nitrogen);
                    //PlugIn.ModelCore.UI.WriteLine("     CFlow={0:0.000}, totalC={1:0.000}", CFlow, totalC);
                    //PlugIn.ModelCore.UI.WriteLine("     this.Name={0}, this.Type={1}", this.Name, this.Type);
                   // PlugIn.ModelCore.UI.WriteLine("     dest.Name  ={0}, dest.Type  ={1}", destination.Name, destination.Type);
                    //PlugIn.ModelCore.UI.WriteLine("     ratio CN to dest={0}", ratioCNtoDestination);
                }

                this.Nitrogen -= mineralNFlow;

                SiteVars.MineralN[site] += mineralNFlow;
                //PlugIn.ModelCore.UI.WriteLine("     this.Name={0}, this.Type={1}", this.Name, this.Type);
                //PlugIn.ModelCore.UI.WriteLine("IfMinOccurs.  MineralN={0:0.00}.", SiteVars.MineralN[site]);

                //PlugIn.ModelCore.UI.WriteLine("  TransferN NFlow={0:0.000}, mineralizedN = {1:0.000}, N mineralalization = {1:0.000}", NFlow, mineralizedN, mineralNFlow);
                //PlugIn.ModelCore.UI.WriteLine("     Source:  this.Name={0}, this.Type={1}", this.Name, this.Type);
            }

            if (mineralNFlow > 0)
                SiteVars.GrossMineralization[site] += mineralNFlow;

            //...Net mineralization
            this.NetMineralization += mineralNFlow;

            //PlugIn.ModelCore.UI.WriteLine("     this.Nitrogen={0:0.000}.", this.Nitrogen);

            //PlugIn.ModelCore.UI.WriteLine("AfterMinOccurs.  MineralN={0:0.00}.", SiteVars.MineralN[site]);
            return;
        }

        public void Respiration(double co2loss, ActiveSite site, bool surface)
        {
        // Compute flows associated with microbial respiration.

        // Input:
        //  co2loss = CO2 loss associated with decomposition
        //  Box A.  For components with only 1 layer, tcstva will be dimensioned (1).

        //  Transput:
        //c         carbonSourceSink = C source/sink
        //c         grossMineralization = gross mineralization
        //c         netMineralization = net mineralization for layer N

            //c...Mineralization associated with respiration is proportional to the N fraction.
            double mineralNFlow = co2loss * this.Nitrogen / this.Carbon; 

            if(mineralNFlow > this.Nitrogen)
            {
                //if((mineralNFlow - this.Nitrogen) > 0.01)
                //{
                //    PlugIn.ModelCore.UI.WriteLine("RESPIRATION for layer {0} {1}:  Mineral N flow exceeds layer Nitrogen.", this.Name, this.Type);
                //    PlugIn.ModelCore.UI.WriteLine("  MineralNFlow={0:0.000}, this.Nitrogen ={0:0.000}", mineralNFlow, this.Nitrogen);
                //    PlugIn.ModelCore.UI.WriteLine("  CO2 loss={0:0.000}, this.Carbon={0:0.000}", co2loss, this.Carbon);
                //    PlugIn.ModelCore.UI.WriteLine("  Site R/C: {0}/{1}.", site.Location.Row, site.Location.Column);
                //}
                mineralNFlow = this.Nitrogen;
                co2loss = this.Carbon;
            }

            this.TransferCarbon(SiteVars.SourceSink[site], co2loss);

            //Add lost CO2 to monthly heterotrophic respiration
            SiteVars.MonthlyHeteroResp[site][Main.Month] += co2loss;

            if(!surface)
                SiteVars.MonthlySoilResp[site][Main.Month] += co2loss;


            this.Nitrogen -= mineralNFlow;
            SiteVars.MineralN[site] += mineralNFlow;

            

            //PlugIn.ModelCore.UI.WriteLine("     Source:  this.Name={0}, this.Type={1}", this.Name, this.Type);
            //PlugIn.ModelCore.UI.WriteLine("  Respiration.mineralN= {0:0.000}, co2loss={1:00}", mineralNFlow, co2loss);
           


            //c...Update gross mineralization
            // this.GrossMineralization += mineralNFlow;
            if (mineralNFlow > 0)
                SiteVars.GrossMineralization[site] += mineralNFlow;

            //c...Update net mineralization
            this.NetMineralization += mineralNFlow;

            return;
        }

        public bool DecomposePossible(double ratioCNnew, double mineralN)
        {

            //c...Determine if decomposition can occur.

            bool canDecompose = true;

            //c...If there is no available mineral N
            if (mineralN < 0.0000001)
            {

                // Compare the C/N of new material to the C/N of the layer if C/N of
                // the layer > C/N of new material
                if (this.Carbon / this.Nitrogen > ratioCNnew)
                {

                    // Immobilization is necessary and the stuff in Box A can't
                    // decompose to Box B.
                    canDecompose = false;
                }
            }

            // If there is some available mineral N, decomposition can
            // proceed even if mineral N is driven negative in
            // the next time step.

            return canDecompose;

        }

        public void AdjustLignin(double inputC, double inputFracLignin)
        {
            //c...Adjust the fraction of lignin in structural C when new material
            //c...  is added.

            //c    oldc  = grams C in structural before new material is added
            //c    frnew = fraction of lignin in new structural material
            //c    addc  = grams structural C being added

            //c    fractl comes in as fraction of lignin in structural before new
            //c           material is added; goes out as fraction of lignin in
            //c           structural with old and new combined.

            //c...oldlig  = grams of lignin in existing residue
            double oldlig = this.FractionLignin * this.Carbon;//totalC;

            //c...newlig = grams of lignin in new residue
            double newlig = inputFracLignin * inputC;

            //c...Compute lignin fraction in combined residue
            double newFraction = (oldlig + newlig) / (this.Carbon + inputC);

            this.FractionLignin = newFraction;

            return;
        }

        public void AdjustDecayRate(double inputC, double inputDecayRate)
        {
            //c...oldlig  = grams of lignin in existing residue
            double oldDecayRate = this.DecayValue * this.Carbon;

            //c...newlig = grams of lignin in new residue
            double newDecayRate = inputDecayRate * inputC;

            //c...Compute decay rate in combined residue
            this.DecayValue = (oldDecayRate + newDecayRate) / (inputC + this.Carbon);

            return;
        }


        public static double BelowgroundDecompositionRatio(ActiveSite site, double minCNenter, double maxCNenter, double minContentN)
        {
            //BelowGround Decomposition RATio computation.
            double bgdrat = 0.0;

            //Determine ratio of C/N of new material entering 'Box B'.
            //Ratio depends on available N

            double mineralN = SiteVars.MineralN[site];

            if (mineralN <= 0.0)
                bgdrat = maxCNenter;  // Set ratio to maximum allowed (HIGHEST carbon, LOWEST nitrogen)
            else if (mineralN > minContentN)
                bgdrat = minCNenter;  //Set ratio to minimum allowed
            else
                bgdrat = (1.0 - (mineralN / minContentN)) * (maxCNenter - minCNenter)
                    + minCNenter;

            return bgdrat;
        }

        public static double AbovegroundDecompositionRatio(double abovegroundN, double abovegroundC)
        {       

            double Ncontent, agdrat;
            double biomassConversion = 2.0;
            
            // CNmicrobialB = slope of the regression line for C/N of som1
            double CNmicrobial_b = (OtherData.MinCNSurfMicrobes - OtherData.MaxCNSurfMicrobes) / OtherData.MinNContentCNSurfMicrobes;

            // The ratios for metabolic and som1 may vary and must be recomputed each time step

            if ((abovegroundC * biomassConversion) <= 0.00000000001)  
                Ncontent = 0.0;
            else  
                Ncontent = abovegroundN / (abovegroundC * biomassConversion);

            //tca is multiplied by biomassConversion to give biomass

            if (Ncontent > OtherData.MinNContentCNSurfMicrobes)
                agdrat = OtherData.MinCNSurfMicrobes;
            else
                agdrat = OtherData.MaxCNSurfMicrobes + Ncontent * CNmicrobial_b;

            return agdrat;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduces the pool's biomass by a specified percentage.
        /// </summary>
        public void ReduceMass(double percentageLost)
        {
            if (percentageLost < 0.0 || percentageLost > 1.0)
                throw new ArgumentException("Percentage must be between 0% and 100%");

            this.Carbon   = this.Carbon * (1.0 - percentageLost);
            this.Nitrogen   = this.Nitrogen * (1.0 - percentageLost);

            return;
        }

    }
}
