//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.Utilities;
using Landis.Library.Succession;
using System.Collections.Generic;
using System.Collections;
using System.Data;

namespace Landis.Extension.Succession.NECN
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }


        public static class Names
        {
            public const string SpeciesParameters = "SpeciesParameters";
            public const string FunctionalGroupParameters = "FunctionalGroupParameters";
            public const string FireReductionParameters = "FireReductionParameters";
            public const string HarvestReductionParameters = "HarvestReductionParameters";
        }

        //---------------------------------------------------------------------

        private IEcoregionDataset ecoregionDataset;
        private ISpeciesDataset speciesDataset;
        private Dictionary<string, int> speciesLineNums;
        private InputVar<string> speciesName;

        //---------------------------------------------------------------------

        static InputParametersParser()
        {
            SeedingAlgorithmsUtil.RegisterForInputValues();
            RegisterForInputValues();
            Percentage dummy = new Percentage();

        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
            this.ecoregionDataset = PlugIn.ModelCore.Ecoregions;
            this.speciesDataset = PlugIn.ModelCore.Species;
            this.speciesLineNums = new Dictionary<string, int>();
            this.speciesName = new InputVar<string>("Species");

        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {

            ReadLandisDataVar();

            int numLitterTypes = 4;
            int numFunctionalTypes = 25;

            InputParameters parameters = new InputParameters(speciesDataset, numLitterTypes, numFunctionalTypes);

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<SeedingAlgorithms> seedAlg = new InputVar<SeedingAlgorithms>("SeedingAlgorithm");
            ReadVar(seedAlg);
            parameters.SeedAlgorithm = seedAlg.Value;

            //---------------------------------------------------------------------------------

            InputVar<string> initCommunities = new InputVar<string>("InitialCommunities");
            ReadVar(initCommunities);
            parameters.InitialCommunities = initCommunities.Value;

            InputVar<string> communitiesMap = new InputVar<string>("InitialCommunitiesMap");
            ReadVar(communitiesMap);
            parameters.InitialCommunitiesMap = communitiesMap.Value;

            InputVar<string> climateConfigFile = new InputVar<string>("ClimateConfigFile");
            ReadVar(climateConfigFile);
            parameters.ClimateConfigFile = climateConfigFile.Value;

            InputVar<string> soilDepthMapName = new InputVar<string>("SoilDepthMapName");
            ReadVar(soilDepthMapName);
            parameters.SoilDepthMapName = soilDepthMapName.Value;

            InputVar<string> soilDrainMapName = new InputVar<string>("SoilDrainMapName");
            ReadVar(soilDrainMapName);
            parameters.SoilDrainMapName = soilDrainMapName.Value;

            InputVar<string> soilBaseFlowMapName = new InputVar<string>("SoilBaseFlowMapName");
            ReadVar(soilBaseFlowMapName);
            parameters.SoilBaseFlowMapName = soilBaseFlowMapName.Value;

            InputVar<string> soilStormFlowMapName = new InputVar<string>("SoilStormFlowMapName");
            ReadVar(soilStormFlowMapName);
            parameters.SoilStormFlowMapName = soilStormFlowMapName.Value;

            InputVar<string> soilFCMapName = new InputVar<string>("SoilFieldCapacityMapName");
            ReadVar(soilFCMapName);
            parameters.SoilFieldCapacityMapName = soilFCMapName.Value;

            InputVar<string> soilWPMapName = new InputVar<string>("SoilWiltingPointMapName");
            ReadVar(soilWPMapName);
            parameters.SoilWiltingPointMapName = soilWPMapName.Value;

            InputVar<string> soilSandMapName = new InputVar<string>("SoilPercentSandMapName");
            ReadVar(soilSandMapName);
            parameters.SoilPercentSandMapName = soilSandMapName.Value;

            InputVar<string> soilClayMapName = new InputVar<string>("SoilPercentClayMapName");
            ReadVar(soilClayMapName);
            parameters.SoilPercentClayMapName = soilClayMapName.Value;

            InputVar<string> som1CsurfMapName = new InputVar<string>("InitialSOM1CsurfMapName");
            ReadVar(som1CsurfMapName);
            parameters.InitialSOM1CSurfaceMapName = som1CsurfMapName.Value;

            InputVar<string> som1NsurfMapName = new InputVar<string>("InitialSOM1NsurfMapName");
            ReadVar(som1NsurfMapName);
            parameters.InitialSOM1NSurfaceMapName = som1NsurfMapName.Value;

            InputVar<string> som1CsoilMapName = new InputVar<string>("InitialSOM1CsoilMapName");
            ReadVar(som1CsoilMapName);
            parameters.InitialSOM1CSoilMapName = som1CsoilMapName.Value;

            InputVar<string> som1NsoilMapName = new InputVar<string>("InitialSOM1NsoilMapName");
            ReadVar(som1NsoilMapName);
            parameters.InitialSOM1NSoilMapName = som1NsoilMapName.Value;

            InputVar<string> som2CMapName = new InputVar<string>("InitialSOM2CMapName");
            ReadVar(som2CMapName);
            parameters.InitialSOM2CMapName = som2CMapName.Value;

            InputVar<string> som2NMapName = new InputVar<string>("InitialSOM2NMapName");
            ReadVar(som2NMapName);
            parameters.InitialSOM2NMapName = som2NMapName.Value;

            InputVar<string> som3CMapName = new InputVar<string>("InitialSOM3CMapName");
            ReadVar(som3CMapName);
            parameters.InitialSOM3CMapName = som3CMapName.Value;

            InputVar<string> som3NMapName = new InputVar<string>("InitialSOM3NMapName");
            ReadVar(som3NMapName);
            parameters.InitialSOM3NMapName = som3NMapName.Value;

            InputVar<string> deadSurfMapName = new InputVar<string>("InitialDeadWoodSurfaceMapName");
            ReadVar(deadSurfMapName);
            parameters.InitialDeadSurfaceMapName = deadSurfMapName.Value;

            InputVar<string> deadSoilMapName = new InputVar<string>("InitialDeadCoarseRootsMapName");
            ReadVar(deadSoilMapName);
            parameters.InitialDeadSoilMapName = deadSoilMapName.Value;

            InputVar<string> normalSWAMapName = new InputVar<string>("NormalSWAMapName");
            if (ReadOptionalVar(normalSWAMapName))
            {
                parameters.NormalSWAMapName = normalSWAMapName.Value;
            }

            InputVar<string> normalCWDMapName = new InputVar<string>("NormalCWDMapName");
            if (ReadOptionalVar(normalCWDMapName))
            {
                parameters.NormalCWDMapName = normalCWDMapName.Value;
            }

            InputVar<string> normalTempMapName = new InputVar<string>("NormalTempMapName");
            if (ReadOptionalVar(normalTempMapName))
            {
                parameters.NormalTempMapName = normalTempMapName.Value;
            }


            InputVar<string> slopeMapName = new InputVar<string>("SlopeMapName");
            if (ReadOptionalVar(slopeMapName))
            {
                parameters.SlopeMapName = slopeMapName.Value;
            }

            InputVar<string> aspectMapName = new InputVar<string>("AspectMapName");
            if (ReadOptionalVar(aspectMapName))
            {
                parameters.AspectMapName = aspectMapName.Value;
            }

            InputVar<bool> calimode = new InputVar<bool>("CalibrateMode");
            if (ReadOptionalVar(calimode))
                parameters.CalibrateMode = calimode.Value;
            else
                parameters.CalibrateMode = false;

            InputVar<bool> smokemode = new InputVar<bool>("SmokeModelOutputs");
            if (ReadOptionalVar(smokemode))
                parameters.SmokeModelOutputs = smokemode.Value;
            else
                parameters.SmokeModelOutputs = false;

           /* InputVar<bool> version_Henne = new InputVar<bool>("Version_Henne_SoilWater");
            if (ReadOptionalVar(version_Henne))
            {
                parameters.SoilWater_Henne = version_Henne.Value;
            }
            else
                parameters.SoilWater_Henne = false;
           */

            InputVar<bool> write_SWA = new InputVar<bool>("Write_SWA_Maps");
            if (ReadOptionalVar(write_SWA))
            {
                parameters.OutputSoilWaterAvailable= write_SWA.Value;
            }
            else
                parameters.OutputSoilWaterAvailable = false;

            InputVar<bool> write_CWD = new InputVar<bool>("Write_CWD_Maps");
            if (ReadOptionalVar(write_CWD))
            {
                parameters.OutputClimateWaterDeficit = write_CWD.Value;
            }
            else
                parameters.OutputClimateWaterDeficit = false;

            InputVar<bool> write_Temp = new InputVar<bool>("Write_Temperature_Maps");
            if (ReadOptionalVar(write_Temp))
            {
                parameters.OutputTemp = write_Temp.Value;
            }
            else
                parameters.OutputTemp = false;

            InputVar<bool> write_SpeciesDroughtMaps = new InputVar<bool>("Write_Species_Drought_Maps");
            if (ReadOptionalVar(write_SpeciesDroughtMaps))
            {
                parameters.WriteSpeciesDroughtMaps = write_SpeciesDroughtMaps.Value;
            }
            else
                parameters.WriteSpeciesDroughtMaps = false;

            InputVar<string> wt = new InputVar<string>("WaterDecayFunction");
            ReadVar(wt);
            parameters.WType = WParse(wt.Value);

            InputVar<double> pea = new InputVar<double>("ProbabilityEstablishAdjust");
            ReadVar(pea);
            parameters.ProbEstablishAdjustment = pea.Value;

            InputVar<double> iMN = new InputVar<double>("InitialMineralN");
            ReadVar(iMN);
            parameters.SetInitMineralN(iMN.Value);

            InputVar<double> iFF = new InputVar<double>("InitialFineFuels");
            ReadVar(iFF);
            parameters.SetInitFineFuels(iFF.Value);


            InputVar<double> ans = new InputVar<double>("AtmosphericNSlope");
            ReadVar(ans);
            parameters.SetAtmosNslope(ans.Value);

            InputVar<double> ani = new InputVar<double>("AtmosphericNIntercept");
            ReadVar(ani);
            parameters.SetAtmosNintercept(ani.Value);

            InputVar<double> lat = new InputVar<double>("Latitude");
            ReadVar(lat);
            parameters.SetLatitude(lat.Value);

            InputVar<double> denits = new InputVar<double>("DenitrificationRate");
            ReadVar(denits);
            parameters.SetDenitrif(denits.Value);

            InputVar<double> drsoms = new InputVar<double>("DecayRateSurf");
            ReadVar(drsoms);
            parameters.SetDecayRateSurf(drsoms.Value);

            InputVar<double> drsom1 = new InputVar<double>("DecayRateSOM1");
            ReadVar(drsom1);
            parameters.SetDecayRateSOM1(drsom1.Value);

            InputVar<double> drsom2 = new InputVar<double>("DecayRateSOM2");
            ReadVar(drsom2);
            parameters.SetDecayRateSOM2(drsom2.Value);

            InputVar<double> drsom3 = new InputVar<double>("DecayRateSOM3");
            ReadVar(drsom3);
            parameters.SetDecayRateSOM3(drsom3.Value);

            // Multiplier to adjust judgement whether a tree-cohort is larger than grass layer
            // W.Hotta 2020.07.07
            InputVar<double> grassTMult = new InputVar<double>("GrassThresholdMultiplier");
            if (ReadOptionalVar(grassTMult))
            {
                parameters.SetGrassThresholdMultiplier(grassTMult.Value);
                //PlugIn.Grasses = true;
            }

            InputVar<string> inputCommunityMaps = new InputVar<string>("CreateInputCommunityMaps");
            if (ReadOptionalVar(inputCommunityMaps))
            {
                PlugIn.InputCommunityMapNames = inputCommunityMaps.Value;

                InputVar<int> inputMapFreq = new InputVar<int>("InputCommunityMapFrequency");
                ReadVar(inputMapFreq);
                PlugIn.InputCommunityMapFrequency = inputMapFreq.Value;

            }

            InputVar<double> stormFlowOverride = new InputVar<double>("StormFlowOverride");
            if (ReadOptionalVar(stormFlowOverride))
            {
                PlugIn.StormFlowOverride = stormFlowOverride.Value;
            }

            InputVar<double> wf1Override = new InputVar<double>("WaterLossFactor1Override");
            if (ReadOptionalVar(wf1Override))
            {
                OtherData.WaterLossFactor1 = wf1Override.Value;
            }

            InputVar<double> wf2Override = new InputVar<double>("WaterLossFactor2Override");
            if (ReadOptionalVar(wf2Override))
            {
                OtherData.WaterLossFactor2 = wf2Override.Value;
            }

            InputVar<double> anerb1Override = new InputVar<double>("AnaerobicFactor1Override");
            if (ReadOptionalVar(anerb1Override))
            {
                OtherData.ratioPlantAvailableWaterPETMaximum = anerb1Override.Value;
            }

            InputVar<double> anerb2Override = new InputVar<double>("AnaerobicFactor2Override");
            if (ReadOptionalVar(anerb2Override))
            {
                OtherData.ratioPlantAvailableWaterPETMinimum = anerb2Override.Value;
            }

            InputVar<double> anerb3Override = new InputVar<double>("AnaerobicFactor3Override");
            if (ReadOptionalVar(anerb3Override))
            {
                OtherData.AnerobicEffectMinimum = anerb3Override.Value;
            }

            InputVar<string> anppMaps = new InputVar<string>("ANPPMapName");
            if (ReadOptionalVar(anppMaps))
            {
                PlugIn.ANPPMapNames = anppMaps.Value;

                InputVar<int> anppMapFreq = new InputVar<int>("ANPPMapFrequency");
                ReadVar(anppMapFreq);
                PlugIn.ANPPMapFrequency = anppMapFreq.Value;

            }

            InputVar<string> aneeMaps = new InputVar<string>("ANEEMapName");
            if (ReadOptionalVar(aneeMaps))
            {
                PlugIn.ANEEMapNames = aneeMaps.Value;

                InputVar<int> aneeMapFreq = new InputVar<int>("ANEEMapFrequency");
                ReadVar(aneeMapFreq);
                PlugIn.ANEEMapFrequency = aneeMapFreq.Value;

            }

            InputVar<string> soilCarbonMaps = new InputVar<string>("SoilCarbonMapName");
            if (ReadOptionalVar(soilCarbonMaps))
            {
                PlugIn.SoilCarbonMapNames = soilCarbonMaps.Value;

                InputVar<int> soilCarbonMapFreq = new InputVar<int>("SoilCarbonMapFrequency");
                ReadVar(soilCarbonMapFreq);
                PlugIn.SoilCarbonMapFrequency = soilCarbonMapFreq.Value;

            }

            InputVar<string> soilNitrogenMaps = new InputVar<string>("SoilNitrogenMapName");
            if (ReadOptionalVar(soilNitrogenMaps))
            {
                PlugIn.SoilNitrogenMapNames = soilNitrogenMaps.Value;

                InputVar<int> soilNitrogenMapFreq = new InputVar<int>("SoilNitrogenMapFrequency");
                ReadVar(soilNitrogenMapFreq);
                PlugIn.SoilNitrogenMapFrequency = soilNitrogenMapFreq.Value;

            }

            InputVar<string> totalCMaps = new InputVar<string>("TotalCMapName");
            if (ReadOptionalVar(totalCMaps))
            {
                PlugIn.TotalCMapNames = totalCMaps.Value;

                InputVar<int> totalCMapFreq = new InputVar<int>("TotalCMapFrequency");
                ReadVar(totalCMapFreq);
                PlugIn.TotalCMapFrequency = totalCMapFreq.Value;

            }

            //-------------------------
            //  Read Species Parameters table
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing NECN SPECIES table.");

            InputVar<string> csv = new InputVar<string>("SpeciesParameters");
            ReadVar(csv);
            CSVParser speciesParser = new CSVParser();
            DataTable speciesTable = speciesParser.ParseToDataTable(csv.Value);
            foreach (DataRow row in speciesTable.Rows)
            {
                ISpecies species = ReadSpecies(System.Convert.ToString(row["SpeciesCode"]));
                parameters.SetFunctionalType(species, System.Convert.ToInt32(row["FunctionalGroupIndex"]));
                parameters.NFixer[species] = System.Convert.ToBoolean(row["NitrogenFixer"]);
                parameters.SetGDDmin(species, System.Convert.ToInt32(row["GDDMinimum"]));
                parameters.SetGDDmax(species, System.Convert.ToInt32(row["GDDMaximum"]));
                parameters.SetMinJanTemp(species, System.Convert.ToInt32(row["MinJanuaryT"]));
                parameters.SetMaxDrought(species, System.Convert.ToDouble(row["MaxDrought"]));
                parameters.SetLeafLongevity(species, System.Convert.ToDouble(row["LeafLongevity"]));
                parameters.Epicormic[species] = System.Convert.ToBoolean(row["Epicormic"]);
                parameters.SetLeafLignin(species, System.Convert.ToDouble(row["LeafLignin"]));
                parameters.SetFineRootLignin(species, System.Convert.ToDouble(row["FineRootLignin"]));
                parameters.SetWoodLignin(species, System.Convert.ToDouble(row["WoodLignin"]));
                parameters.SetCoarseRootLignin(species, System.Convert.ToDouble(row["CoarseRootLignin"]));
                parameters.SetLeafCN(species, System.Convert.ToDouble(row["LeafCN"]));
                parameters.SetFineRootCN(species, System.Convert.ToDouble(row["FineRootCN"]));
                parameters.SetWoodCN(species, System.Convert.ToDouble(row["WoodCN"]));
                parameters.SetCoarseRootCN(species, System.Convert.ToDouble(row["CoarseRootCN"]));
                parameters.SetFoliageLitterCN(species, System.Convert.ToDouble(row["FoliageLitterCN"]));
                parameters.SetMaxANPP(species, System.Convert.ToInt32(row["MaximumANPP"]));
                parameters.SetMaxBiomass(species, System.Convert.ToInt32(row["MaximumBiomass"]));
                
                parameters.Grass[species] = ReadGrass(row);
                parameters.SetNlog_depend(species, ReadNlog(row)); // W.Hotta (2023.05.06)
                parameters.SetLightLAIShape(species, System.Convert.ToDouble(row["LightLAIShape"]));
                parameters.SetLightLAIScale(species, System.Convert.ToDouble(row["LightLAIScale"]));
                parameters.SetLightLAILocation(species, System.Convert.ToDouble(row["LightLAILocation"]));

                parameters.SetGrowthLAI(species, ReadGrowthLAI(row));

                //Optional parameters for CWD-limited establishment
                parameters.SetCWDBeginLimit(species, ReadCWDBeginLimit(row));
                parameters.SetCWDMax(species, ReadCWDMax(row));
            }

            //--------- Read In Functional Group Table -------------------------------
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing FUNCTIONAL GROUP table.");

            InputVar<string> func_csv = new InputVar<string>("FunctionalGroupParameters");
            ReadVar(func_csv);
                CSVParser functionalParser = new CSVParser();
                DataTable functionalTable = functionalParser.ParseToDataTable(func_csv.Value);
                foreach (DataRow row in functionalTable.Rows)
                {
                    string FunctionalTypeName = System.Convert.ToString(row["FunctionalGroupName"]);
                    int funcIndex = System.Convert.ToInt32(row["FunctionalGroupIndex"]); 

                    if (funcIndex >= numFunctionalTypes)
                        throw new InputValueException(funcIndex.ToString(),
                                                  "The index:  {0} exceeds the allowable number of functional groups, {1}",
                                                  funcIndex.ToString(), numFunctionalTypes);

                    FunctionalType funcTParms = new FunctionalType();
                    parameters.FunctionalTypes[funcIndex] = funcTParms;

                    funcTParms.TempCurve1 = System.Convert.ToDouble(row["TemperatureCurve1"]);
                    funcTParms.TempCurve2 = System.Convert.ToDouble(row["TemperatureCurve2"]); 
                    funcTParms.TempCurve3 = System.Convert.ToDouble(row["TemperatureCurve3"]); 
                    funcTParms.TempCurve4 = System.Convert.ToDouble(row["TemperatureCurve4"]); 
                    funcTParms.FractionANPPtoLeaf = System.Convert.ToDouble(row["FractionANPPtoLeaf"]); 
                    funcTParms.BiomassToLAI = System.Convert.ToDouble(row["LeafBiomassToLAI"]);
                    funcTParms.KLAI = System.Convert.ToDouble(row["KLAI"]);
                    funcTParms.MaxLAI = System.Convert.ToDouble(row["MaximumLAI"]);
                    funcTParms.MoistureCurve2 = System.Convert.ToDouble(row["MoistureCurve2"]); 
                    funcTParms.MoistureCurve3 = System.Convert.ToDouble(row["MoistureCurve3"]);
                    funcTParms.WoodDecayRate = System.Convert.ToDouble(row["WoodDecayRate"]);
                    funcTParms.MonthlyWoodMortality = System.Convert.ToDouble(row["MonthlyWoodMortality"]);
                    funcTParms.LongevityMortalityShape = System.Convert.ToDouble(row["LongevityMortalityShape"]);
                    funcTParms.FoliageDropMonth = System.Convert.ToInt32(row["FoliageDropMonth"]);
                    funcTParms.CoarseRootFraction = System.Convert.ToDouble(row["CoarseRootFraction"]);
                    funcTParms.FineRootFraction = System.Convert.ToDouble(row["FineRootFraction"]);
                    funcTParms.MinLAI = ReadMinLAI(row);
                    funcTParms.MoistureCurve1 = ReadMC1(row);
                    funcTParms.MoistureCurve4 = ReadMC4(row);
                    funcTParms.MinSoilDrain = ReadMinSoilDrain(row);

            }


            //-------------------------
            //  Read Drought Mortality Parameters table
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing Drought table.");
                        
            InputVar<string> drought_csv = new InputVar<string>("DroughtMortalityParameters");

            if (ReadOptionalVar(drought_csv))
            {
                DroughtMortality.UseDrought = true; //set flag to use drought mortality algorithms
                //PlugIn.ModelCore.UI.WriteLine("    Setting UseDrought flag to true."); //debug

                InputVar<int> inputMapFreq = new InputVar<int>("InputCommunityMapFrequency");
               
                CSVParser droughtParser = new CSVParser();
                DataTable droughtTable = speciesParser.ParseToDataTable(drought_csv.Value);
                foreach (DataRow row in droughtTable.Rows)
                {
                    //TODO Currently, does not check for duplicate or missing species. It should be okay to have missing
                    // species, but we ought to check for duplicates. It does correctly reject species that aren't present
                    // in the species table already.
                    ISpecies species = ReadDroughtSpecies(System.Convert.ToString(row["SpeciesCode"]));
                    PlugIn.ModelCore.UI.WriteLine("Reading drought parameters for species {0}", species.Name);

                    parameters.SetCWDThreshold(species, System.Convert.ToInt32(row["CWDThreshold"]));
                    parameters.SetMortalityAboveThreshold(species, System.Convert.ToDouble(row["MortalityAboveThreshold"]));
                    parameters.SetCWDThreshold2(species, System.Convert.ToInt32(row["CWDThreshold2"]));
                    parameters.SetMortalityAboveThreshold2(species, System.Convert.ToDouble(row["MortalityAboveThreshold2"]));

                    parameters.SetIntercept(species, System.Convert.ToDouble(row["Intercept"]));
                    parameters.SetBetaAge(species, System.Convert.ToDouble(row["BetaAge"]));
                    parameters.SetBetaTemp(species, System.Convert.ToDouble(row["BetaTemp"]));
                    parameters.SetBetaSWAAnom(species, System.Convert.ToDouble(row["BetaSWAAnom"]));
                    parameters.SetBetaBiomass(species, System.Convert.ToDouble(row["BetaBiomass"]));
                    parameters.SetBetaCWD(species, System.Convert.ToDouble(row["BetaCWD"]));
                    parameters.SetBetaNormCWD(species, System.Convert.ToDouble(row["BetaNormCWD"]));
                    parameters.SetBetaNormTemp(species, System.Convert.ToDouble(row["BetaNormTemp"]));
                    parameters.SetIntxnCWD_Biomass(species, System.Convert.ToDouble(row["IntxnCWD_Biomass"]));

                    parameters.SetLagTemp(species, System.Convert.ToInt32(row["LagTemp"]));
                    parameters.SetLagCWD(species, System.Convert.ToInt32(row["LagCWD"]));
                    parameters.SetLagSWA(species, System.Convert.ToInt32(row["LagSWA"]));
                }
               
            }

            //--------- Read In Fire Reductions Table ---------------------------

            PlugIn.ModelCore.UI.WriteLine("   Begin reading FIRE REDUCTION parameters.");
            ReadName(Names.FireReductionParameters);

            InputVar<int> frindex = new InputVar<int>("Fire Severity Index MUST = 1-5");
            InputVar<double> wred = new InputVar<double>("Coarse Litter Reduction");
            InputVar<double> lred = new InputVar<double>("Fine Litter Reduction");
            InputVar<double> live_wood_red = new InputVar<double>("Cohort Wood Reduction");
            InputVar<double> live_leaf_red = new InputVar<double>("Cohort Litter Reduction");
            InputVar<double> som_red = new InputVar<double>("SOM Reduction");

            while (! AtEndOfInput && CurrentName != Names.HarvestReductionParameters)
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(frindex , currentLine);
                int ln = (int) frindex.Value.Actual;

                if(ln < 1 || ln > 10)
                    throw new InputValueException(frindex.Value.String,
                                              "The fire severity index:  {0} must be 1-10,",
                                              frindex.Value.String);


                FireReductions inputFireReduction = new FireReductions();  // ignoring severity = zero
                parameters.FireReductionsTable[ln] = inputFireReduction;

                ReadValue(wred, currentLine);
                inputFireReduction.CoarseLitterReduction = wred.Value;

                ReadValue(lred, currentLine);
                inputFireReduction.FineLitterReduction = lred.Value;

                ReadValue(live_wood_red, currentLine);
                inputFireReduction.CohortWoodReduction = live_wood_red.Value;

                ReadValue(live_leaf_red, currentLine);
                inputFireReduction.CohortLeafReduction = live_leaf_red.Value;

                ReadValue(som_red, currentLine);
                inputFireReduction.SOMReduction = som_red.Value;

                CheckNoDataAfter("the " + som_red.Name + " column", currentLine);

                GetNextLine();
            }

            //--------- Read In Harvest Reductions Table ---------------------------
            InputVar<string> hreds = new InputVar<string>("HarvestReductions");
            ReadName(Names.HarvestReductionParameters);
            PlugIn.ModelCore.UI.WriteLine("   Begin reading HARVEST REDUCTION parameters.");

            InputVar<string> prescriptionName = new InputVar<string>("Prescription");
            InputVar<double> wred_pr = new InputVar<double>("Coarse Litter Reduction");
            InputVar<double> lred_pr = new InputVar<double>("Fine Litter Reduction");
            InputVar<double> som_red_pr = new InputVar<double>("SOM Reduction");
            InputVar<double> cohortw_red_pr = new InputVar<double>("Cohort Wood Removal");
            InputVar<double> cohortl_red_pr = new InputVar<double>("Cohort Leaf Removal");


            while (!AtEndOfInput)
            {

                StringReader currentLine = new StringReader(CurrentLine);
                HarvestReductions harvReduction = new HarvestReductions();
                parameters.HarvestReductionsTable.Add(harvReduction);

                ReadValue(prescriptionName, currentLine);
                harvReduction.PrescriptionName = prescriptionName.Value;

                ReadValue(wred_pr, currentLine);
                harvReduction.CoarseLitterReduction = wred_pr.Value;

                ReadValue(lred_pr, currentLine);
                harvReduction.FineLitterReduction = lred_pr.Value;

                ReadValue(som_red_pr, currentLine);
                harvReduction.SOMReduction = som_red_pr.Value;

                ReadValue(cohortw_red_pr, currentLine);
                harvReduction.CohortWoodReduction = cohortw_red_pr.Value;

                ReadValue(cohortl_red_pr, currentLine);
                harvReduction.CohortLeafReduction = cohortl_red_pr.Value;

                GetNextLine();
            }


            return parameters; 
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<WaterType>("Water Effect on Decomposition");
            InputValues.Register<WaterType>(WParse);

        }
        //---------------------------------------------------------------------
        public static WaterType WParse(string word)
        {
            if (word == "Linear")
                return WaterType.Linear;
            else if (word == "Ratio")
                return WaterType.Ratio;
            throw new System.FormatException("Valid names:  Linear, Ratio");
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a species name from the current line, and verifies the name.
        /// </summary>
        private ISpecies ReadSpecies(StringReader currentLine)
        {
            ReadValue(speciesName, currentLine);
            ISpecies species = speciesDataset[speciesName.Value.Actual];
            if (species == null)
                throw new InputValueException(speciesName.Value.String,
                                              "{0} is not a species name.",
                                              speciesName.Value.String);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName.Value.String,
                                              "The species {0} was previously used on line {1}",
                                              speciesName.Value.String, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }
        private ISpecies ReadSpecies(string speciesName)
        {
            ISpecies species = speciesDataset[speciesName];
            if (species == null)
                throw new InputValueException(speciesName,
                                              "{0} is not a species name.",
                                              speciesName);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
                throw new InputValueException(speciesName,
                                              "The species {0} was previously used on line {1}",
                                              speciesName, lineNumber);
            else
                speciesLineNums[species.Name] = LineNumber;
            return species;
        }
        private ISpecies ReadDroughtSpecies(string speciesName)
        {
            ISpecies species = speciesDataset[speciesName];
            if (species == null)
                throw new InputValueException(speciesName,
                                              "{0} is not a species name.",
                                              speciesName);
            int lineNumber;
            if (speciesLineNums.TryGetValue(species.Name, out lineNumber))
            {
                return species;
            }            
            else throw new InputValueException(speciesName,
                                              "The species {0}, found on the drought table, does not exist in the species table",
                                              speciesName);
            
        }
        private bool ReadGrass(DataRow row)
        {
            try
            {
                bool grass = System.Convert.ToBoolean(row["Grass"]);
                return grass;
            }
            catch
            {
                return false;
            }
        }
        private bool ReadNlog(DataRow row)
        {
            try
            {
                bool Nlog_depend = System.Convert.ToBoolean(row["Nlog_depend"]);
                return Nlog_depend;
            }
            catch
            {
                return false;
            }
        }
        private double ReadMinLAI(DataRow row)
        {
            try
            {
                double minLAI = System.Convert.ToDouble(row["MinLAI"]);
                return minLAI;
            }
            catch
            {
                return 0.1;
            }
        }
        //Optional MoistureCurve1
        private double ReadMC1(DataRow row)
        {
            try
            {
                double mc1 = System.Convert.ToDouble(row["MoistureCurve1"]);
                return mc1;
            }
            catch
            {
                NewParseException("Error in moisture curve 1");
                return 0.0;
            }
        }
        //Optional MoistureCurve4 -- NECN uses 4-parameter water limit if MoistureCurve4 is present
        private double ReadMC4(DataRow row)
        {
            try
            {
                double mc4 = System.Convert.ToDouble(row["MoistureCurve4"]);
                OtherData.DGS_waterlimit = true; //SF set flag to turn on 4-parameter water limit mode
                return mc4;
            }
            catch
            {
                NewParseException("Error in moisture curve 4");
                return 0.0;
            }
        }
        //Optional minimum soil drainage -- prevents functional types from establishing on poorly drained soils
        private double ReadMinSoilDrain(DataRow row)
        {
            try
            {
                double minSoilDrain = System.Convert.ToDouble(row["MinSoilDrain"]);
                return minSoilDrain;
            }
            catch
            {
                return 0.0; //SF by default, all species can establish in all soil drainage classes
            }
        }

        //Optional CWD-based limit to establishment. If present for one species, this should be provided for all species.
        //This parameter sets the CWD at which establishment begins to become limited
        private int ReadCWDBeginLimit(DataRow row)
        {
            try
            {
                int cwdBeginLimit = System.Convert.ToInt32(row["CWDBeginLimit"]);
                return cwdBeginLimit;
            }
            catch
            {
                return 0; //SF set to 0 -- later handled to avoid CWD-based establishment if CWDBeginLimit == 0
            }
        }
        //Optional CWD-based limit to establishment. If present for one species, this should be provided for all species
        //This parameter sets the maximum CWD under which a species can establish; past this threshold, pest = 0.
        private int ReadCWDMax(DataRow row)
        {
            try
            {
                int cwdMax = System.Convert.ToInt32(row["CWDMax"]);
                return cwdMax;
            }
            catch
            {
                return 0; //SF set to 0 -- later handled to avoid CWD-based establishment if CWDBeginLimit == 0
            }
        }
        private double ReadGrowthLAI(DataRow row)
        {
            try
            {
                double gLAI = System.Convert.ToDouble(row["GrowthLAI"]);
                return gLAI;
            }
            catch
            {
                return 0.47;  // This is the value given for all biomes in the tree.100 file.
                // This was the default value for all previous versions of NECN.
            }
        }
    }
}
