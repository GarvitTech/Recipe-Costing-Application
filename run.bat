@echo off
echo Professional Recipe Costing ^& Menu Engineering
echo ================================================

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo .NET 6.0 is not installed. Please install .NET 6.0 SDK first.
    echo Download from: https://dotnet.microsoft.com/download/dotnet/6.0
    pause
    exit /b 1
)

echo .NET found
echo.

REM Build the application
echo Building application...
dotnet build RecipeCostingApp.sln --configuration Release

if %errorlevel% equ 0 (
    echo Build successful!
    echo.
    echo Starting Recipe Costing Application...
    dotnet run --project RecipeCostingApp --configuration Release
) else (
    echo Build failed. Please check the error messages above.
    pause
    exit /b 1
)

pause