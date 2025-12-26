@echo off
echo Building Recipe Costing App Installer...
echo.

REM Check if NSIS is installed
where makensis >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: NSIS (Nullsoft Scriptable Install System) is not installed or not in PATH.
    echo.
    echo Please download and install NSIS from: https://nsis.sourceforge.io/Download
    echo After installation, add NSIS to your system PATH or run this script from NSIS directory.
    echo.
    pause
    exit /b 1
)

REM Ensure the application is built
echo Building application first...
cd ..\RecipeCostingApp
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to build application
    pause
    exit /b 1
)

REM Return to installer directory
cd ..\RecipeCostingApp.Installer

REM Copy icon file if it exists
if exist "..\RecipeCostingApp\app.ico" (
    copy "..\RecipeCostingApp\app.ico" "app.ico"
) else (
    echo Warning: app.ico not found, installer will use default icon
)

REM Build the installer
echo.
echo Building installer with NSIS...
makensis installer.nsi

if %ERRORLEVEL% EQU 0 (
    echo.
    echo SUCCESS: Installer created successfully!
    echo Location: RecipeCostingApp-Setup.exe
    echo.
    echo The installer includes:
    echo - Complete application with all dependencies
    echo - Start Menu shortcut
    echo - Desktop shortcut
    echo - Proper uninstaller
    echo - Add/Remove Programs entry
    echo.
) else (
    echo.
    echo ERROR: Failed to create installer
)

pause