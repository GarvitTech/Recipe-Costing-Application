# Import Feature Documentation

## Overview
The Item Master now supports importing ingredients from multiple file formats:
- **Excel Files** (.xlsx, .xls)
- **PDF Files** (.pdf)
- **Image Files** (.jpg, .png, .bmp) - Basic support

## Excel Import Format

### Expected Column Headers (case-insensitive):
- **Name** or **Ingredient** or **Item** - Required
- **Category** or **Type** - Optional (defaults to "General")
- **Unit** - Optional (defaults to "g")
- **Purchase Unit** - Optional (defaults to 1000)
- **Price** or **Cost** - Optional (defaults to 0)
- **Waste** - Optional (defaults to 0)

### Example Excel Format:
```
Name          | Category   | Unit | Purchase Unit | Price | Waste
Tomatoes      | Vegetables | g    | 1000         | 50.00 | 5
Chicken Breast| Meat       | g    | 1000         | 180.00| 2
Olive Oil     | Oils       | ml   | 500          | 120.00| 0
```

## PDF Import Format

The system will attempt to parse ingredient data from PDF text using common patterns:
- `Name - Category - Price - Unit`
- `Name Price Unit`
- `Name, Category, Price`

### Example PDF Content:
```
Tomatoes - Vegetables - 50.00 - g
Chicken Breast 180.00 g
Olive Oil, Oils, 120.00
```

## Image Import

Currently provides basic image processing. For full OCR functionality, additional setup would be required.

## Import Process

1. Click the appropriate import button (Excel, PDF, or Image)
2. Select your file using the file dialog
3. Review the preview of found ingredients
4. Confirm to import (duplicates are automatically skipped)
5. View the import results

## Tips for Best Results

### Excel Files:
- Use the first row for headers
- Keep data in the first worksheet
- Ensure numeric values are properly formatted
- Use consistent units (g, ml, Kg, Liter, Pc)

### PDF Files:
- Ensure text is selectable (not scanned images)
- Use consistent formatting for ingredient lists
- Simple, structured layouts work best

### Error Handling:
- Invalid rows are skipped with console logging
- Duplicate ingredients (by name) are automatically skipped
- Import continues even if some items fail to process

## Supported File Extensions:
- Excel: .xlsx, .xls
- PDF: .pdf
- Images: .jpg, .jpeg, .png, .bmp