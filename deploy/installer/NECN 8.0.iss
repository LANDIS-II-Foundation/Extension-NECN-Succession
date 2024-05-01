; LANDIS-II Extension infomation
#define CoreRelease "LANDIS-II-V8"
#define ExtensionName "NECN Succession"
#define AppVersion "8.0"
#define AppPublisher "LANDIS-II Foundation"
#define AppURL "http://www.landis-ii.org/"

; Build directory
#define BuildDir "..\..\src\bin\Debug\netstandard2.0"
#define LibraryDir "..\..\src\lib"

; LANDIS-II installation directories
#define ExtDir "C:\Program Files\LANDIS-II-v8\extensions"
#define AppDir "C:\Program Files\LANDIS-II-v8"
#define LandisPlugInDir "C:\Program Files\LANDIS-II-v8\plug-ins-installer-files"
#define ExtensionsCmd AppDir + "\commands\landis-ii-extensions.cmd"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8994EAAA-1B13-40EC-A57B-5FFA7DAC5202}
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
Source: {#BuildDir}\Landis.Extension.Succession.NECN-v8.dll; DestDir: {#ExtDir}; Flags: ignoreversion
Source: {#BuildDir}\Landis.Extension.Succession.NECN-v8.pdb; DestDir: {#ExtDir}; Flags: ignoreversion

; Requisite auxiliary libraries
; NB. These libraries are used by other extensions and thus are never uninstalled.
Source: {#LibraryDir}\Landis.Library.UniversalCohorts-v1.dll; DestDir: {#ExtDir}; Flags: ignoreversion uninsneveruninstall 
Source: {#LibraryDir}\Landis.Library.Metadata-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#LibraryDir}\Landis.Library.Parameters-v2.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#LibraryDir}\Landis.Library.Climate-v4.3.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#LibraryDir}\Landis.Library.Succession-v9.dll; DestDir: {#ExtDir}; Flags: uninsneveruninstall 
Source: {#LibraryDir}\Landis.Library.InitialCommunity.Universal.dll; DestDir: {#ExtDir}; Flags: ignoreversion uninsneveruninstall 

; LANDIS-II identifies the extension with the info in this .txt file
; NB. New releases must modify the name of this file and the info in it
#define InfoTxt "NECN_Succession8.txt"
Source: {#InfoTxt}; DestDir: {#LandisPlugInDir}
; NOTE: Don't use "Flags: ignoreversion" on any shared system files


[Run]
Filename: {#ExtensionsCmd}; Parameters: "remove ""NECN Succession"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#ExtensionsCmd}; Parameters: "add ""{#InfoTxt}"" "; WorkingDir: {#LandisPlugInDir} 


[UninstallRun]
Filename: {#ExtensionsCmd}; Parameters: "remove ""NECN Succession"" "; WorkingDir: {#LandisPlugInDir}


