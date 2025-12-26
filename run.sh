#!/bin/bash

# Professional Recipe Costing App - Build and Run Script

echo "🍳 Professional Recipe Costing & Menu Engineering"
echo "================================================"

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 6.0 is not installed. Please install .NET 6.0 SDK first."
    echo "Download from: https://dotnet.microsoft.com/download/dotnet/6.0"
    exit 1
fi

echo "✅ .NET found: $(dotnet --version)"

# Build the application
echo "🔨 Building application..."
dotnet build RecipeCostingApp.sln --configuration Release

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    
    # Run the application
    echo "🚀 Starting Recipe Costing Application..."
    dotnet run --project RecipeCostingApp --configuration Release
else
    echo "❌ Build failed. Please check the error messages above."
    exit 1
fi