# Application Icon Setup

## Current Status
The project is configured to use an application icon, but you need to replace the placeholder `app.ico` file with a proper Windows icon.

## How to Create a Proper Icon

### Option 1: Online Icon Converter
1. Create or find a 256x256 pixel image (PNG/JPG) representing your app
2. Use an online converter like:
   - https://convertio.co/png-ico/
   - https://www.icoconverter.com/
   - https://favicon.io/favicon-converter/

### Option 2: Icon Design Tools
- **Free**: GIMP with ICO plugin
- **Paid**: Adobe Photoshop, Figma

### Recommended Icon Design
For a Recipe Costing Application, consider:
- Chef hat with calculator elements
- Cooking utensils with chart/graph
- Food items with price tags
- Professional, clean design
- Colors: Blue (#2E86AB), Orange (#F18F01), White

### Icon Specifications
- **Format**: .ico (Windows Icon)
- **Sizes**: Multiple sizes in one file (16x16, 32x32, 48x48, 64x64, 128x128, 256x256)
- **File name**: `app.ico` (already configured in project)
- **Location**: `RecipeCostingApp/app.ico`

## Building the Executable
1. Replace `app.ico` with your proper icon file
2. Run `build-exe.bat` to create the standalone executable
3. The final .exe will be in the `publish` folder

## Note
The current placeholder file will not display an icon. You must replace it with a proper .ico file for the icon to appear in Windows Explorer and the taskbar.