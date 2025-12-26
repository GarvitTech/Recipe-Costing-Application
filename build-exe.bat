@echo off
echo Building Recipe Costing Application...
echo.

REM Clean previous builds
dotnet clean --configuration Release

REM Build the application
dotnet build --configuration Release

REM Publish as single-file executable
echo.
echo Publishing standalone executable...
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output ".\publish" /p:PublishSingleFile=true /p:PublishReadyToRun=true

echo.
echo Build complete! 
echo Executable location: .\publish\RecipeCostingApp.exe
echo.
pause