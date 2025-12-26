# Professional Recipe Costing & Menu Engineering Application

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey.svg)
![Framework](https://img.shields.io/badge/.NET-6.0-purple.svg)

A complete Windows desktop application for restaurant recipe costing, menu engineering, and food cost management. Built with WPF and SQLite for professional food service operations.

## 🚀 Quick Start

### Option 1: Run Executable (Recommended)
1. Download `RecipeCostingApp.exe` from the `publish` folder
2. Double-click to run (no installation required)
3. Application will create its database automatically

### Option 2: Build from Source
1. Install .NET 6 SDK
2. Run `build-exe.bat` to create standalone executable
3. Or use `dotnet run` for development

## ✨ Features

### 📊 Item Master Management
- Complete ingredient database with categories
- Unit conversions and purchase tracking
- Waste percentage and yield calculations
- Real-time cost per unit calculations

### 🍳 Recipe Management
- Intuitive recipe creation interface
- Automatic ingredient cost calculations
- Waste, GST, packaging, and delivery cost integration
- Profit margin analysis with selling price optimization

### 📈 Costing Analysis
- Detailed cost breakdowns by ingredient
- Visual cost distribution charts
- Profitability analysis with margin calculations
- Export capabilities for reporting

### 🎯 Bulk Costing
- Event and catering quantity calculations
- Bulk ingredient purchasing optimization
- Cost scaling for different serving sizes
- Professional PDF reports generation

### 💰 Menu Engineering
- Menu item profitability analysis
- Cost vs. popularity matrix
- Pricing optimization recommendations
- Strategic menu positioning insights

### ⚙️ Advanced Settings
- Database backup and restore functionality
- Clear database option with safety confirmations
- Application preferences and configuration
- System information and statistics

## 🛠️ Technical Specifications

- **Framework**: .NET 6 WPF (Windows Presentation Foundation)
- **Database**: SQLite with Entity Framework integration
- **Architecture**: MVVM pattern with services layer
- **PDF Generation**: iTextSharp for professional reports
- **Deployment**: Single-file executable with all dependencies

## 📋 System Requirements

- **OS**: Windows 10/11 (x64)
- **Memory**: 4GB RAM minimum
- **Storage**: 200MB free space
- **Display**: 1024x768 minimum resolution

## 🎨 User Interface

- **Modern Design**: Clean, professional interface
- **Color-Coded Fields**: White for input, blue for calculated values
- **Responsive Layout**: Resizable windows and adaptive controls
- **Intuitive Navigation**: Tab-based organization with clear workflows

## 📖 Usage Guide

### Getting Started
1. Launch the application
2. Start with **Item Master** to add your ingredients
3. Create recipes in **Recipe Management**
4. Analyze costs in **Costing Analysis**
5. Use **Bulk Costing** for events
6. Optimize pricing with **Menu Engineering**

### Data Management
- All data is stored locally in SQLite database
- Use Settings → Database to backup/restore data
- Clear database option available for fresh starts

## 🔧 Development

### Building the Application
```bash
# Clean build
dotnet clean --configuration Release

# Build executable
dotnet publish --configuration Release --runtime win-x64 --self-contained true --output "publish" /p:PublishSingleFile=true
```

### Project Structure
```
RecipeCostingApp/
├── Data/           # Database management
├── Models/         # Data models
├── Services/       # Business logic
├── Views/          # UI components
├── Styles/         # XAML styling
└── publish/        # Built executable
```

## 📄 Documentation

- `USER_GUIDE.md` - Detailed user instructions
- `ICON_SETUP.md` - Application icon configuration
- `DEPLOYMENT_COMPLETE.md` - Final deployment status

## 👨‍💻 Developer

**Made by Garvit**

Professional recipe costing solution designed for restaurants, catering businesses, and food service operations.

## 📝 License

This project is developed for professional food service management. All rights reserved.

---

*Transform your food cost management with professional-grade recipe costing and menu engineering tools.*
