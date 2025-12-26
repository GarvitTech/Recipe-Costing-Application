[Setup]
AppName=Recipe Costing App
AppVersion=1.0.0
AppPublisher=Garvit Tech
AppPublisherURL=https://github.com/GarvitTech/Recipe-Costing-APP
AppSupportURL=https://github.com/GarvitTech/Recipe-Costing-APP
AppUpdatesURL=https://github.com/GarvitTech/Recipe-Costing-APP
DefaultDirName={autopf}\Recipe Costing App
DefaultGroupName=Recipe Costing App
AllowNoIcons=yes
LicenseFile=LICENSE.txt
OutputDir=.
OutputBaseFilename=RecipeCostingApp-Setup
SetupIconFile=..\RecipeCostingApp\app.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\RecipeCostingApp\publish\RecipeCostingApp.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\RecipeCostingApp\publish\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\RecipeCostingApp\publish\RecipeCostingApp.pdb"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Recipe Costing App"; Filename: "{app}\RecipeCostingApp.exe"
Name: "{group}\{cm:UninstallProgram,Recipe Costing App}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Recipe Costing App"; Filename: "{app}\RecipeCostingApp.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\RecipeCostingApp.exe"; Description: "{cm:LaunchProgram,Recipe Costing App}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCU; Subkey: "Software\GarvitTech\RecipeCostingApp"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"

[UninstallDelete]
Type: filesandordirs; Name: "{userappdata}\RecipeCostingApp"