@echo off
echo Creating Recipe Costing App Installer...
echo.

powershell -ExecutionPolicy Bypass -File "Create-Installer.ps1"

if exist "RecipeCostingApp-Setup.ps1" (
    echo.
    echo SUCCESS: Installer created as RecipeCostingApp-Setup.ps1
    echo.
    echo To distribute:
    echo 1. Send RecipeCostingApp-Setup.ps1 to users
    echo 2. Users right-click and select "Run with PowerShell"
    echo 3. Users click "Yes" for admin rights
    echo.
) else (
    echo ERROR: Failed to create installer
)

pause