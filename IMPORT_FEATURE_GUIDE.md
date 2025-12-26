# Import Feature Documentation

## Overview
The Item Master supports importing ingredients from CSV files with **case-insensitive** field matching:
- **CSV Files** (.csv) - Full support
- **Text Files** (.txt) - Basic support

## Key Features
- **Case-insensitive field matching** - "Name", "name", "NAME" all work
- **Duplicate detection** - Automatically skips existing ingredients
- **Preview before import** - Review what will be imported
- **Error handling** - Continues processing even if some rows fail

## CSV Import Format

### Expected Column Headers (case-insensitive):
- **Name** or **Ingredient** or **Item** - Required
- **Category** or **Type** - Optional (defaults to "General")
- **Unit** - Optional (defaults to "g")
- **Purchase Unit** - Optional (defaults to 1000)
- **Price** or **Cost** - Optional (defaults to 0)
- **Waste** - Optional (defaults to 0)

### Example CSV Format:
```csv
Name,Category,Unit,Purchase Unit,Price,Waste
Tomatoes,Vegetables,g,1000,50.00,5
Chicken Breast,Meat,g,1000,180.00,2
Olive Oil,Oils,ml,500,120.00,0
Onions,Vegetables,g,1000,30.00,10
Rice,Grains,g,1000,80.00,1
```

## Import Process

1. Click **"📊 Import CSV"** button
2. Select your CSV file using the file dialog
3. Review the preview of found ingredients
4. Confirm to import (duplicates are automatically skipped)
5. View the import results

## Tips for Best Results

### CSV Files:
- Use the first row for headers
- Ensure numeric values are properly formatted (use decimal point, not comma)
- Use consistent units (g, ml, Kg, Liter, Pc)
- Enclose text with commas in quotes: "Chicken, boneless"

### Error Handling:
- Invalid rows are skipped with console logging
- Duplicate ingredients (by name) are automatically skipped
- Import continues even if some items fail to process
- Detailed error messages help identify issues

## Supported File Extensions:
- CSV: .csv
- Text: .txt (comma-separated)

## Sample File
A sample CSV file (`sample_ingredients.csv`) is included in the project root for testing the import functionality.