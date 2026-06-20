#define AppName "TemperatureMonitor"
#define AppExe "TemperatureMonitor.exe"
#define AppPublisher "0cin05"
; AppVersion is passed via CLI: iscc setup.iss /DAppVersion=v1.2.3
#ifndef AppVersion
  #define AppVersion "dev"
#endif

[Setup]
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
OutputDir=Output
OutputBaseFilename={#AppName}-{#AppVersion}-setup
Compression=lzma2/ultra64
SolidCompression=yes
; requires admin because OpenHardwareMonitor needs hardware access
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs
Source: "..\THIRD-PARTY-NOTICES.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppExe}"

[Run]
Filename: "{app}\{#AppExe}"; Description: "Launch {#AppName}"; Flags: nowait postinstall skipifsilent