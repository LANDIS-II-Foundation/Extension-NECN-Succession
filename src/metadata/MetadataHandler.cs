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
            //---------------------------------------            
            //          map outputs:         
            //---------------------------------------
            if (ANPPMapNames != null)
            {
                PlugIn.ModelCore.UI.WriteLine("  ANPP Map Names = \"{0}\" ...", ANPPMapNames);
                string[] paths = { @"NECN", "AG_NPP-{timestep}.img" };
                OutputMetadata mapOut_ANPP = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Aboveground Net Primary Production",
                    //FilePath = @"NECN\AG_NPP-{timestep}.img",  
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_ANPP);
            }

            if (ANEEMapNames != null)
            {
                string[] paths = { @"NECN", "NEE-{timestep}.img" };
                OutputMetadata mapOut_Nee = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Net Ecosystem Exchange",
                    //FilePath = @"NECN\NEE-{timestep}.img",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_Nee);
            }
            if (SoilCarbonMapNames != null)
            {
                string[] paths = { @"NECN", "SOC-{timestep}.img" };
                OutputMetadata mapOut_SOC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Carbon",
                    //FilePath = @"NECN\SOC-{timestep}.img",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SOC);
            }
            if (SoilNitrogenMapNames != null)
            {
                string[] paths = { @"NECN", "SON-{timestep}.img" };
                OutputMetadata mapOut_SON = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Soil Organic Nitrogen",
                    //FilePath = @"NECN\SON-{timestep}.img",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_N_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_SON);
            }
            if (TotalCMapNames != null)
            {
                string[] paths = { @"NECN", "TotalC-{timestep}.img" };
                OutputMetadata mapOut_TotalC = new OutputMetadata()
                {
                    Type = OutputType.Map,
                    Name = "Total Carbon",
                    //FilePath = @"NECN\TotalC-{timestep}.img",
                    FilePath = Path.Combine(paths),
                    Map_DataType = MapDataType.Continuous,
                    Map_Unit = FieldUnits.g_C_m2,
                    Visualize = true,
                };
                Extension.OutputMetadatas.Add(mapOut_TotalC);
            }
//            if (LAImapnames != null) //These are new maps for testing and analysis purposes
//            {
//                OutputMetadata mapOut_LAI = new OutputMetadata()
//                {
//                    Type = OutputType.Map,
//                    Name = "LAI",
//                    FilePath = @"century\LAI-{timestep}.gis",  //century
//                    Map_DataType = MapDataType.Continuous,
//                   Map_Unit = FieldUnits.g_C_m2, //Not sure
//                    Visualize = true,
//                };
//                Extension.OutputMetadatas.Add(mapOut_LAI);
//            }
//            if (ShadeClassmapnames != null)
//            {
//                OutputMetadata mapOut_ShadeClass = new OutputMetadata()
//                {
//                    Type = OutputType.Map,
//                    Name = "ShadeClass",
//                    FilePath = @"century\ShadeClass-{timestep}.gis",  //century
//                    Map_DataType = MapDataType.Continuous,
//                   Map_Unit = FieldUnits.g_C_m2, //NOt sure
//                    Visualize = true,
//                };
//                Extension.OutputMetadatas.Add(mapOut_LAI);
//            }


            //---------------------------------------
            MetadataProvider mp = new MetadataProvider(Extension);
            mp.WriteMetadataToXMLFile("Metadata", Extension.Name, Extension.Name);




        }
    }
}
