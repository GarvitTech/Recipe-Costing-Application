# Windows Installer Setup Guide

## 🎯 Overview
This guide helps you create a professional Windows installer (.exe) for the Recipe Costing Application that users can install like any other Windows software.

## 📦 Installer Options

### Option 1: NSIS Installer (Recommended)
**Free, lightweight, and professional**

#### Setup Steps:
1. **Download NSIS**: https://nsis.sourceforge.io/Download
2. **Install NSIS** with default settings
3. **Add to PATH** (usually automatic)
4. **Run**: `build-installer-complete.bat`
5. **Choose option 1** for NSIS installer

#### Features:
- Professional Windows installer
- Start Menu shortcuts
- Desktop shortcut
- Proper uninstaller
- Add/Remove Programs entry
- Admin privileges handling

### Option 2: Inno Setup Installer
**Alternative free installer creator**

#### Setup Steps:
1. **Download Inno Setup**: https://jrsoftware.org/isdl.php
2. **Install Inno Setup** with default settings
3. **Run**: `build-installer-complete.bat`
4. **Choose option 2** for Inno Setup installer

## 🚀 Quick Start

### Automated Build (Easiest)
```batch
# Run the complete build script
build-installer-complete.bat

# Choose your preferred installer type
# The script will:
# 1. Build the application
# 2. Create the installer
# 3. Output: RecipeCostingApp-Setup.exe
```

### Manual NSIS Build
```batch
# 1. Build application
cd RecipeCostingApp
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true

# 2. Build installer
cd ..\RecipeCostingApp.Installer
makensis installer.nsi
```

## 📋 What the Installer Includes

### Application Files:
- `RecipeCostingApp.exe` - Main application
- All required DLL dependencies
- SQLite database engine
- WPF runtime components

### Installation Features:
- **Installation Directory**: `C:\Program Files\Recipe Costing App\`
- **Start Menu**: Recipe Costing App folder with shortcut
- **Desktop Shortcut**: Optional during installation
- **Uninstaller**: Complete removal capability
- **Registry Entries**: Proper Windows integration
- **Admin Rights**: Handles UAC properly

### User Experience:
- Professional installation wizard
- License agreement display
- Custom installation directory selection
- Progress indication
- Launch application after installation
- Clean uninstallation process

## 🎨 Customization

### Icon Setup:
1. Replace `RecipeCostingApp\app.ico` with your custom icon
2. Rebuild the installer
3. Icon will appear in shortcuts and Add/Remove Programs

### Installer Customization:
- **Company Name**: Edit `installer.nsi` or `installer.iss`
- **Application Name**: Modify in installer scripts
- **Version Number**: Update version in scripts
- **License Text**: Edit `LICENSE.txt`

## 📁 Output Files

After successful build:
- **Installer**: `RecipeCostingApp-Setup.exe` (~100MB)
- **Application**: `RecipeCostingApp\publish\RecipeCostingApp.exe`

## 🔧 Troubleshooting

### NSIS Not Found:
- Download and install NSIS
- Restart command prompt
- Ensure NSIS is in system PATH

### Build Errors:
- Ensure .NET 6 SDK is installed
- Check file paths in installer scripts
- Verify all dependencies are in publish folder

### Permission Issues:
- Run command prompt as Administrator
- Ensure antivirus isn't blocking the build

## 🎉 Distribution

Once created, `RecipeCostingApp-Setup.exe` can be:
- Shared via email or file sharing
- Uploaded to websites for download
- Distributed on USB drives
- Published to software repositories

Users simply double-click the installer and follow the wizard to install your application professionally on their Windows systems!

## ✅ Final Result

Your users will get:
- Professional installation experience
- Application in Program Files
- Start Menu and Desktop shortcuts
- Proper Windows integration
- Easy uninstallation option
- No technical knowledge required