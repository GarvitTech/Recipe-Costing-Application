@echo off
echo ========================================
echo Recipe Costing App Installer Builder
echo ========================================
echo.

echo Choose installer type:
echo 1. NSIS Installer (Recommended - Free)
echo 2. Inno Setup Installer (Alternative - Free)
echo 3. Build Application Only
echo.
set /p choice="Enter your choice (1-3): "

if "%choice%"=="3" goto :build_app
if "%choice%"=="2" goto :inno_setup
if "%choice%"=="1" goto :nsis_installer

echo Invalid choice. Using NSIS installer...
goto :nsis_installer

:build_app
echo.
echo Building application...
cd ..\RecipeCostingApp
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true
if %ERRORLEVEL% EQU 0 (
    echo SUCCESS: Application built successfully!
    echo Location: RecipeCostingApp\publish\RecipeCostingApp.exe
) else (
    echo ERROR: Failed to build application
)
goto :end

:nsis_installer
echo.
echo Building NSIS Installer...
echo.

where makensis >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: NSIS not found!
    echo Download from: https://nsis.sourceforge.io/Download
    echo After installation, add NSIS to system PATH
    goto :end
)

echo Building application first...
cd ..\RecipeCostingApp
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to build application
    goto :end
)

cd ..\RecipeCostingApp.Installer
if exist "..\RecipeCostingApp\app.ico" copy "..\RecipeCostingApp\app.ico" "app.ico"

makensis installer.nsi
if %ERRORLEVEL% EQU 0 (
    echo.
    echo SUCCESS: NSIS Installer created!
    echo File: RecipeCostingApp-Setup.exe
) else (
    echo ERROR: Failed to create NSIS installer
)
goto :end

:inno_setup
echo.
echo Building Inno Setup Installer...
echo.

where iscc >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Inno Setup not found!
    echo Download from: https://jrsoftware.org/isdl.php
    echo After installation, add Inno Setup to system PATH
    goto :end
)

echo Building application first...
cd ..\RecipeCostingApp
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to build application
    goto :end
)

cd ..\RecipeCostingApp.Installer
iscc installer.iss
if %ERRORLEVEL% EQU 0 (
    echo.
    echo SUCCESS: Inno Setup Installer created!
    echo File: RecipeCostingApp-Setup.exe
) else (
    echo ERROR: Failed to create Inno Setup installer
)
goto :end

:end
echo.
echo ========================================
echo Build process completed!
echo ========================================
pause