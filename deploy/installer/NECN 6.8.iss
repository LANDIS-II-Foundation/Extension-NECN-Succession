; LANDIS-II Extension infomation
#define CoreRelease "LANDIS-II-V7"
#define ExtensionName "NECN Succession"
#define AppVersion "6.8"
#define AppPublisher "LANDIS-II Foundation"
#define AppURL "http://www.landis-ii.org/"

; Build directory
#define BuildDir "..\..\src\bin\Debug\netstandard2.0"

; LANDIS-II installation directories
#define ExtDir "C:\Program Files\LANDIS-II-v7\extensions"
#define AppDir "C:\Program Files\LANDIS-II-v7"
#define LandisPlugInDir "C:\Program Files\LANDIS-II-v7\plug-ins-installer-files"
#define ExtensionsCmd AppDir + "\commands\landis-ii-extensions.cmd"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{1DD4C700-1693-4C93-B485-F17147D1CB11}
AppName={#CoreRelease} {#ExtensionName}
AppVersion={#AppVersion}
; Name in "Programs and Features"
AppVerName={#CoreRelease} {#ExtensionName} v{#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={commonpf}\{#ExtensionName}
DisableDirPage=yes
DefaultGroupName={#ExtensionName}
DisableProgramGroupPage=yes
LicenseFile=LANDIS-II_Binary_license.rtf
OutputDir={#SourcePath}
OutputBaseFilename={#CoreRelease} {#ExtensionName} {#AppVersion}-setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"


[Files]
; This .dll IS the extension (ie, the extension's assembly)
; NB: Do not put an additional version number in the file name of this .dll
; (The name of this .dll is defined in the extension's \src\*.csproj file)
Source: {#BuildDir}\Landis.Extension.Succession.NECN-v6.dll; DestDir: {#ExtDir}; Flags: ignoreversion
Source: {#BuildDir}\Landis.Extension.Succession.NECN-v6.pdb; DestDir: {#ExtDir}; Flags: ignoreversion

; Requisite auxiliary libraries
; NB. These libraries are used by other extensions and thus are never uninstalled.
Source: {#BuildDir}\Landis.Library.AgeOnlyCohorts-v3.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Cohorts-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.BiomassCohorts-v3.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.LeafBiomassCohorts-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Metadata-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Parameters-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Biomass-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Climate-v4.3.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.Succession-v8.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#BuildDir}\Landis.Library.InitialCommunity.Biomass.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
; Source: {#BuildDir}\Landis.Library.InitialCommunity-vInputBiomass.pdb; DestDir: {#ExtDir}; Flags: uninsneveruninstall 

; LANDIS-II identifies the extension with the info in this .txt file
; NB. New releases must modify the name of this file and the info in it
#define InfoTxt "NECN_Succession 6.txt"
Source: {#InfoTxt}; DestDir: {#LandisPlugInDir}
; NOTE: Don't use "Flags: ignoreversion" on any shared system files


[Run]
Filename: {#ExtensionsCmd}; Parameters: "remove ""NECN Succession"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#ExtensionsCmd}; Parameters: "add ""{#InfoTxt}"" "; WorkingDir: {#LandisPlugInDir} 


[UninstallRun]
Filename: {#ExtensionsCmd}; Parameters: "remove ""NECN Succession"" "; WorkingDir: {#LandisPlugInDir}


