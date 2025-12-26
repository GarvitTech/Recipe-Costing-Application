@echo off
echo ========================================
echo Recipe Costing App - Final Build
echo ========================================

cd RecipeCostingApp

echo Cleaning previous builds...
dotnet clean --configuration Release

echo Building application...
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "../publish-final" /p:PublishSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo BUILD SUCCESSFUL!
    echo ========================================
    echo.
    echo Executable location: publish-final\RecipeCostingApp.exe
    echo.
    echo Ready for delivery!
    echo ========================================
) else (
    echo.
    echo ========================================
    echo BUILD FAILED!
    echo ========================================
    echo Please check the error messages above.
)

pause