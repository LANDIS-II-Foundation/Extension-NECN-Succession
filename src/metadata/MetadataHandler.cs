using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Landis.Library.Metadata;
using Landis.Core;
using Landis.Utilities;

namespace Landis.Extension.Succession.NECN
{
    public class MetadataHandler
    {
        
        public static ExtensionMetadata Extension {get; set;}

        public static void InitializeMetadata(int timestep, ICore mCore, 
            string SoilCarbonMapNames, 
            string SoilNitrogenMapNames, 
            string ANPPMapNames, 
            string ANEEMapNames, 
            string TotalCMapNames)
            //string LAIMapNames,
            //string ShadeClassMapNames)
        {
            ScenarioReplicationMetadata scenRep = new ScenarioReplicationMetadata() {
                RasterOutCellArea = PlugIn.ModelCore.CellArea,
                TimeMin = PlugIn.ModelCore.StartTime,
                TimeMax = PlugIn.ModelCore.EndTime,
            };

            Extension = new ExtensionMetadata(mCore){
                Name = "NECN-Succession",
                TimeInterval = timestep, 
                ScenarioReplicationMetadata = scenRep
            };

            //---------------------------------------
            //          table outputs:   
            //---------------------------------------

            Outputs.primaryLog = new MetadataTable<PrimaryLog>("NECN-succession-log.csv");
            Outputs.primaryLogShort = new MetadataTable<PrimaryLogShort>("NECN-succession-log-short.csv");
            Outputs.monthlyLog = new MetadataTable<MonthlyLog>("NECN-succession-monthly-log.csv");
            Outputs.reproductionLog = new MetadataTable<ReproductionLog>("NECN-reproduction-log.csv");
            Outputs.establishmentLog = new MetadataTable<EstablishmentLog>("NECN-prob-establish-log.csv");
            Outputs.calibrateLog = new MetadataTable<CalibrateLog>("NECN-calibrate-log.csv");
            Outputs.droughtLog = new MetadataTable<DroughtLog>("NECN-succession-drought-log.csv");

            OutputMetadata tblOut_monthly = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "MonthlyLog",
                FilePath = Outputs.monthlyLog.FilePath,
                Visualize = true,
            };
            tblOut_monthly.RetriveFields(typeof(MonthlyLog));
            Extension.OutputMetadatas.Add(tblOut_monthly);

            OutputMetadata tblOut_primary = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "PrimaryLog",
                FilePath = Outputs.primaryLog.FilePath,
                Visualize = false,
            };
            tblOut_primary.RetriveFields(typeof(PrimaryLog));
            Extension.OutputMetadatas.Add(tblOut_primary);

            OutputMetadata tblOut_primaryShort = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "PrimaryLogShort",
                FilePath = Outputs.primaryLogShort.FilePath,
                Visualize = true,
            };

            tblOut_primaryShort.RetriveFields(typeof(PrimaryLogShort));
            Extension.OutputMetadatas.Add(tblOut_primaryShort);

            OutputMetadata tblOut_repro = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "ReproductionLog",
                FilePath = Outputs.reproductionLog.FilePath,
                Visualize = false,
            };
            tblOut_repro.RetriveFields(typeof(ReproductionLog));
            Extension.OutputMetadatas.Add(tblOut_repro);

            OutputMetadata tblOut_pest = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "EstablishmentLog",
                FilePath = Outputs.establishmentLog.FilePath,
                Visualize = false,
            };
            tblOut_pest.RetriveFields(typeof(EstablishmentLog));
            Extension.OutputMetadatas.Add(tblOut_pest);

            OutputMetadata tblOut_cali = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "CalibrateLog",
                FilePath = Outputs.calibrateLog.FilePath,
                Visualize = false,
            };
            tblOut_cali.RetriveFields(typeof(CalibrateLog));
            Extension.OutputMetadatas.Add(tblOut_cali);

            OutputMetadata tblOut_drought = new OutputMetadata()
            {
                Type = OutputType.Table,
                Name = "DroughtLog",
                FilePath = Outputs.droughtLog.FilePath,
                Visualize = true,
            };
            tblOut_drought.RetriveFields(typeof(DroughtLog));
            Extension.OutputMetadatas.Add(tblOut_drought);
            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------
            if (ANPPMapNames != null)
            {
                PlugIn.ModelCore.UI.WriteLine("  ANPP Map Names = \"{0}\" ...", ANPPMapNames);
                string[] paths = { @"NECN", "AG_NPP-{timestep}.tif" };
                OutputMetadata mapOut_ANPP = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Aboveground Net Primary Production",
                    //FilePath = @"NECN\AG_NPP-{timestep}.tif",  
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_ANPP);
            }

            if (ANEEMapNames != null)
            {
                string[] paths = { @"NECN", "NEE-{timestep}.tif" };
                OutputMetadata mapOut_Nee = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Net Ecosystem Exchange",
                    //FilePath = @"NECN\NEE-{timestep}.tif",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_Nee);
            }
            if (SoilCarbonMapNames != null)
            {
                string[] paths = { @"NECN", "SOC-{timestep}.tif" };
                OutputMetadata mapOut_SOC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Carbon",
                    //FilePath = @"NECN\SOC-{timestep}.tif",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SOC);
            }
            if (SoilNitrogenMapNames != null)
            {
                string[] paths = { @"NECN", "SON-{timestep}.tif" };
                OutputMetadata mapOut_SON = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Nitrogen",
                    //FilePath = @"NECN\SON-{timestep}.tif",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_N_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SON);
            }
            if (TotalCMapNames != null)
            {
                string[] paths = { @"NECN", "TotalC-{timestep}.tif" };
                OutputMetadata mapOut_TotalC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Total Carbon",
                    //FilePath = @"NECN\TotalC-{timestep}.tif",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_TotalC);
            }

            if (DroughtMortality.UseDrought)
            {
                string[] paths = { @"NECN", "DroughtMortality-{timestep}.tif" };
                OutputMetadata mapOut_DroughtMort = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Drought Mortality",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_DroughtMort);
            }

            if (DroughtMortality.OutputSoilWaterAvailable)
            {
                string[] paths = { @"NECN", "SWA-{timestep}.tif" };
                OutputMetadata mapOut_SWA = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Summer Soil Water Availability",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.cm, //SF check on units
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SWA);
            }

            if (DroughtMortality.OutputClimateWaterDeficit)
            {
                string[] paths = { @"NECN", "CWD-{timestep}.tif" };
                OutputMetadata mapOut_CWD = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Climatic Water Deficit",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.cm, //SF check on units
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_CWD);
            }

            if (DroughtMortality.OutputTemperature)
            {
                string[] paths = { @"NECN", "Temperature-{timestep}.tif" };
                OutputMetadata mapOut_Temp = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Temperature",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.DegreeC, //SF check on units
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_Temp);
            }



            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
