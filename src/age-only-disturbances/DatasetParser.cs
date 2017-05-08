//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Melissa Lucash

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.Extension.Succession.NECN_Hydro.AgeOnlyDisturbances
{
    /// <summary>
    /// A parser that reads a dataset of biomass parameters for age-only
    /// disturbances from text input.
    /// </summary>
    public class DatasetParser
        : Landis.TextParser<IParameterDataset>
    {
        public override string LandisDataValue
        {
            get {
                return "Age-only Disturbances - Biomass Parameters";
            }
        }


        private Dictionary<string, int> lineNums;
        //private string LandisDataValue = "Age-only Disturbances - Biomass Parameters";

        //---------------------------------------------------------------------


        public DatasetParser()
        {
            lineNums = new Dictionary<string, int>();
        }

        //---------------------------------------------------------------------

        protected override IParameterDataset Parse()
        {
            //InputVar<string> landisData = new InputVar<string>("LandisData");
            //ReadVar(landisData);
            //if (landisData.Value.Actual != LandisDataValue)
            //    throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", LandisDataValue);

            ReadLandisDataVar();

            ParameterDataset dataset = new ParameterDataset();
            const string DeadBiomassReductions = "DeadBiomassReductions";
            
            ParseTable(dataset.CohortReductions,
                       "CohortBiomassReductions",
                       DeadBiomassReductions);
            
            ParseTable(dataset.PoolReductions,
                       DeadBiomassReductions,
                       null);

            return dataset; //.GetComplete();
        }

        //---------------------------------------------------------------------

        private void ParseTable(IPercentageTable table,
                                string                  tableName,
                                string                  nextTableName)
        {
            ReadName(tableName);
            
            PlugIn.ModelCore.UI.WriteLine("      Reading {0}.", tableName);

            InputVar<string> disturbance = new InputVar<string>("Disturbance");
            InputVar<Percentage> woodPercentage = new InputVar<Percentage>("Woody");
            InputVar<Percentage> foliarPercentage = new InputVar<Percentage>("Non-Woody");
            string lastColumn = "the " + foliarPercentage.Name + " column";

            const string defaultName = "(default)";
            bool defaultFound = false;
            lineNums.Clear();
            while (! AtEndOfInput && CurrentName != nextTableName) {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(disturbance, currentLine);
                int lineNum;
                if (lineNums.TryGetValue(disturbance.Value.Actual, out lineNum))
                    throw new InputValueException(disturbance.Value.Actual,
                                                  "The value \"{0}\" was previously used on line {1}",
                                                  disturbance.Value.Actual,
                                                  lineNum);
                lineNums[disturbance.Value.String] = LineNumber;

                PoolPercentages percentages;
                if (disturbance.Value.Actual == defaultName) {
                    defaultFound = true;
                    percentages = table.Default;
                }
                else {
                    ExtensionType disturbanceType = new ExtensionType("disturbance:" + disturbance.Value.Actual);
                    
                    //if(disturbance.Value.Actual == "Fire" || disturbance.Value.Actual == "fire")
                    //    throw new InputValueException(disturbance.Value.Actual,
                    //                              "\"{0}\" is not an allowable disturbance type, line {1}",
                    //                              disturbance.Value.Actual,
                    //                              lineNum);
                    
                    //percentages = table[disturbanceType];
                    table[disturbanceType] = new PoolPercentages();
                    percentages = table[disturbanceType];
                    PlugIn.ModelCore.UI.WriteLine("         Adding {0}...", disturbanceType);
                }

                ReadValue(woodPercentage, currentLine);
                percentages.Wood = woodPercentage.Value;

                ReadValue(foliarPercentage, currentLine);
                percentages.Foliar = foliarPercentage.Value;

                CheckNoDataAfter(lastColumn, currentLine);
                GetNextLine();
            }

            if (! defaultFound)
                throw NewParseException("Missing the \"{0}\" row in the percentage table",
                                        defaultName);
        }
    }
}
