# Recipe Costing App Installer Builder
# This script creates a self-extracting installer

Write-Host "========================================" -ForegroundColor Green
Write-Host "Recipe Costing App Installer Builder" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# Build the application first
Write-Host "Building application..." -ForegroundColor Yellow
Set-Location "..\RecipeCostingApp"
& dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Failed to build application" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Set-Location "..\RecipeCostingApp.Installer"

# Create installer directory structure
$installerDir = "installer-temp"
if (Test-Path $installerDir) {
    Remove-Item $installerDir -Recurse -Force
}
New-Item -ItemType Directory -Path $installerDir | Out-Null
New-Item -ItemType Directory -Path "$installerDir\app" | Out-Null

# Copy application files
Write-Host "Copying application files..." -ForegroundColor Yellow
Copy-Item "..\RecipeCostingApp\publish\*" "$installerDir\app\" -Recurse

# Create installer script
$installerScript = @'
@echo off
echo Installing Recipe Costing App...
echo.

REM Create installation directory
set INSTALL_DIR=%ProgramFiles%\Recipe Costing App
if not exist "%INSTALL_DIR%" mkdir "%INSTALL_DIR%"

REM Copy files
echo Copying application files...
xcopy /E /I /Y "%~dp0app\*" "%INSTALL_DIR%\"

REM Create Start Menu shortcut
echo Creating shortcuts...
set SHORTCUT_DIR=%ProgramData%\Microsoft\Windows\Start Menu\Programs
if not exist "%SHORTCUT_DIR%\Recipe Costing App" mkdir "%SHORTCUT_DIR%\Recipe Costing App"

REM Create shortcut using PowerShell
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%SHORTCUT_DIR%\Recipe Costing App\Recipe Costing App.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\RecipeCostingApp.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.Description = 'Professional Recipe Costing & Menu Engineering'; $Shortcut.Save()"

REM Create Desktop shortcut
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%PUBLIC%\Desktop\Recipe Costing App.lnk'); $Shortcut.TargetPath = '%INSTALL_DIR%\RecipeCostingApp.exe'; $Shortcut.WorkingDirectory = '%INSTALL_DIR%'; $Shortcut.Description = 'Professional Recipe Costing & Menu Engineering'; $Shortcut.Save()"

REM Add to registry for uninstall
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /v "DisplayName" /t REG_SZ /d "Recipe Costing App" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /v "UninstallString" /t REG_SZ /d "\"%INSTALL_DIR%\uninstall.bat\"" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /v "InstallLocation" /t REG_SZ /d "%INSTALL_DIR%" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /v "Publisher" /t REG_SZ /d "Garvit Tech" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /v "DisplayVersion" /t REG_SZ /d "1.0.0" /f

REM Create uninstaller
echo @echo off > "%INSTALL_DIR%\uninstall.bat"
echo echo Uninstalling Recipe Costing App... >> "%INSTALL_DIR%\uninstall.bat"
echo rmdir /S /Q "%INSTALL_DIR%" >> "%INSTALL_DIR%\uninstall.bat"
echo del "%PUBLIC%\Desktop\Recipe Costing App.lnk" >> "%INSTALL_DIR%\uninstall.bat"
echo rmdir /S /Q "%ProgramData%\Microsoft\Windows\Start Menu\Programs\Recipe Costing App" >> "%INSTALL_DIR%\uninstall.bat"
echo reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\RecipeCostingApp" /f >> "%INSTALL_DIR%\uninstall.bat"
echo echo Uninstallation complete! >> "%INSTALL_DIR%\uninstall.bat"
echo pause >> "%INSTALL_DIR%\uninstall.bat"

echo.
echo Installation completed successfully!
echo.
echo Recipe Costing App has been installed to:
echo %INSTALL_DIR%
echo.
echo Shortcuts created:
echo - Start Menu: Recipe Costing App
echo - Desktop: Recipe Costing App
echo.
set /p launch="Launch Recipe Costing App now? (Y/N): "
if /i "%launch%"=="Y" start "" "%INSTALL_DIR%\RecipeCostingApp.exe"

echo.
echo Installation finished!
pause
'@

# Save installer script
$installerScript | Out-File -FilePath "$installerDir\install.bat" -Encoding ASCII

# Create self-extracting archive using PowerShell
Write-Host "Creating self-extracting installer..." -ForegroundColor Yellow

$sfxScript = @'
# Self-Extracting Installer for Recipe Costing App
Add-Type -AssemblyName System.IO.Compression.FileSystem

$tempDir = [System.IO.Path]::GetTempPath() + "RecipeCostingApp_" + [System.Guid]::NewGuid().ToString()
New-Item -ItemType Directory -Path $tempDir | Out-Null

try {
    # Extract embedded zip to temp directory
    $zipBytes = [System.Convert]::FromBase64String($zipData)
    [System.IO.File]::WriteAllBytes("$tempDir\installer.zip", $zipBytes)
    [System.IO.Compression.ZipFile]::ExtractToDirectory("$tempDir\installer.zip", $tempDir)
    
    # Run installer
    Start-Process -FilePath "$tempDir\install.bat" -Wait -Verb RunAs
}
finally {
    # Cleanup
    if (Test-Path $tempDir) {
        Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}
'@

# Create zip of installer files
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipPath = "installer-package.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}
[System.IO.Compression.ZipFile]::CreateFromDirectory($installerDir, $zipPath)

# Convert zip to base64
$zipBytes = [System.IO.File]::ReadAllBytes($zipPath)
$zipBase64 = [System.Convert]::ToBase64String($zipBytes)

# Create final installer
$finalInstaller = '$zipData = @"' + "`n" + $zipBase64 + "`n" + '"@' + "`n`n" + $sfxScript

$finalInstaller | Out-File -FilePath "RecipeCostingApp-Setup.ps1" -Encoding UTF8

Write-Host ""
Write-Host "SUCCESS: Installer created!" -ForegroundColor Green
Write-Host "File: RecipeCostingApp-Setup.ps1" -ForegroundColor Green
Write-Host ""
Write-Host "To install, users should:" -ForegroundColor Yellow
Write-Host "1. Right-click RecipeCostingApp-Setup.ps1" -ForegroundColor Yellow
Write-Host "2. Select 'Run with PowerShell'" -ForegroundColor Yellow
Write-Host "3. Click 'Yes' when prompted for admin rights" -ForegroundColor Yellow
Write-Host ""

# Cleanup
Remove-Item $installerDir -Recurse -Force
Remove-Item $zipPath -Force

Read-Host "Press Enter to exit"