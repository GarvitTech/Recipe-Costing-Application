# 📖 HOW TO USE THE RECIPE COSTING APPLICATION

## 🚀 **STARTING THE APPLICATION**

### **Quick Start:**
1. Navigate to: `c:\Users\garvi\Downloads\Recipe-Costing-APP-main\Recipe-Costing-APP-main`
2. **Double-click `run.bat`** - This will automatically start the application

### **Manual Start:**
```bash
cd "c:\Users\garvi\Downloads\Recipe-Costing-APP-main\Recipe-Costing-APP-main"
dotnet run --project RecipeCostingApp --configuration Release
```

---

## 📦 **HOW TO ADD INGREDIENTS (Item Master)**

### **Step 1: Navigate to Item Master**
- When the application starts, you'll see the **Item Master** page (it loads by default)
- Or click **"📦 Item Master"** in the left sidebar

### **Step 2: Add New Ingredients**
Fill in the following fields in the top form:

1. **Name**: Enter ingredient name (e.g., "Tomato", "Chicken Breast")
2. **Category**: Select or type category (e.g., "Vegetables", "Meat", "Dairy")
3. **Unit**: Select unit from dropdown (g, ml, Pc, Kg, Liter)
4. **Purchase Unit**: Enter purchase quantity (e.g., 1000 for 1kg)
5. **Price**: Enter price for the purchase unit (e.g., 50.00)
6. **Waste %**: Enter waste percentage (e.g., 10 for 10% waste)

### **Step 3: Save the Ingredient**
- Click **"💾 Save"** button
- The **Unit Cost** and **Yield %** will be calculated automatically
- The ingredient will appear in the grid below

### **Example Ingredients to Add:**
```
Name: Tomato          Category: Vegetables    Unit: g     Purchase Unit: 1000    Price: 50.00    Waste %: 10
Name: Onion           Category: Vegetables    Unit: g     Purchase Unit: 1000    Price: 30.00    Waste %: 15
Name: Chicken Breast  Category: Meat          Unit: g     Purchase Unit: 1000    Price: 250.00   Waste %: 5
Name: Rice            Category: Dry Store     Unit: g     Purchase Unit: 1000    Price: 80.00    Waste %: 2
Name: Olive Oil       Category: Oils          Unit: ml    Purchase Unit: 1000    Price: 300.00   Waste %: 0
```

---

## 📋 **HOW TO CREATE RECIPES**

### **Step 1: Navigate to Recipes**
- Click **"📋 Recipes"** in the left sidebar

### **Step 2: Enter Recipe Details**
1. **Recipe Name**: Enter your recipe name (e.g., "Chicken Curry")
2. **Category**: Select category (Appetizers, Main Course, Desserts, Beverages)
3. **Waste %**: Enter recipe-level waste percentage (default: 0)

### **Step 3: Add Ingredients to Recipe**
1. **Select Ingredient**: Choose from the dropdown (shows all ingredients from Item Master)
2. **Enter Quantity**: Type the quantity needed (e.g., 500 for 500g)
3. **Click "➕ Add"**: The ingredient will be added to the recipe
4. **Repeat** for all ingredients in your recipe

### **Step 4: Set Costing Parameters**
In the right panel, enter:
- **GST %**: Tax percentage (default: 18)
- **Packaging**: Packaging cost (e.g., 5.00)
- **Delivery**: Delivery charges (e.g., 10.00)
- **Selling Price**: Your desired selling price (e.g., 150.00)

### **Step 5: Save Recipe**
- Click **"💾 Save Recipe"**
- All costs will be calculated automatically

---

## 💰 **HOW TO ANALYZE COSTS**

### **Step 1: Navigate to Costing Analysis**
- Click **"💰 Costing Analysis"** in the left sidebar

### **Step 2: Select Recipe**
- Choose a recipe from the dropdown
- View detailed cost breakdown:
  - Recipe Cost
  - Waste Adjustment
  - GST Amount
  - Packaging & Delivery
  - Final Cost
  - Profit & Margin

### **Step 3: Generate PDF Report**
- Click **"📄 Generate PDF"**
- Choose save location
- Professional recipe card will be generated

---

## 📊 **HOW TO DO BULK COSTING**

### **Step 1: Navigate to Bulk Costing**
- Click **"📊 Bulk Costing"** in the left sidebar

### **Step 2: Select Recipe and Multiplier**
1. **Choose Recipe**: Select from dropdown
2. **Set Multiplier**: Enter custom number or use quick buttons (25x, 50x, 100x, 200x)
3. **Click "🧮 Calculate"**

### **Step 3: View Bulk Requirements**
- See scaled ingredient quantities
- View total costs for bulk production
- Generate kitchen production sheets

---

## 🎯 **HOW TO USE MENU ENGINEERING**

### **Step 1: Navigate to Menu Engineering**
- Click **"🎯 Menu Engineering"** in the left sidebar

### **Step 2: Analyze Menu Performance**
- View all recipes with profitability metrics
- See profit margins and cost percentages
- Identify high/medium/low profit items

---

## 🔍 **FEATURES OVERVIEW**

### **Color Coding System:**
- **White Fields**: User input required
- **Light Blue Fields**: Auto-calculated (read-only)

### **Search & Filter:**
- Use search boxes to find ingredients quickly
- Filter by name or category

### **Data Management:**
- **Edit**: Click ✏️ button in any grid to edit items
- **Delete**: Click 🗑️ button to remove items
- **Backup**: Click "💾 Backup Data" in sidebar

### **Automatic Calculations:**
- Unit costs calculated from price and purchase unit
- Yield calculated from waste percentage
- Recipe costs update in real-time
- Profit margins calculated automatically

---

## 📝 **SAMPLE WORKFLOW**

### **1. Setup Ingredients (Item Master)**
```
Add: Chicken (Meat, 1000g, $250, 5% waste)
Add: Rice (Dry Store, 1000g, $80, 2% waste)
Add: Onion (Vegetables, 1000g, $30, 15% waste)
```

### **2. Create Recipe**
```
Recipe: Chicken Rice
- Chicken: 300g
- Rice: 200g  
- Onion: 100g
GST: 18%, Packaging: $5, Selling Price: $120
```

### **3. Analyze Costs**
- View cost breakdown
- Check profit margins
- Generate PDF reports

### **4. Scale for Events**
- Use bulk costing for large orders
- Generate production sheets

---

## ⚡ **QUICK TIPS**

1. **Start with Item Master**: Always add ingredients first
2. **Use Categories**: Organize ingredients for easy finding
3. **Set Realistic Waste %**: This affects final costs significantly
4. **Regular Backups**: Use the backup feature regularly
5. **PDF Reports**: Generate professional reports for stakeholders

---

## 🆘 **TROUBLESHOOTING**

### **Application Won't Start:**
- Ensure .NET 6.0 is installed
- Run `run.bat` as administrator

### **Database Issues:**
- Database is created automatically in `%AppData%\RecipeCostingApp\`
- Use backup/restore if needed

### **Calculations Wrong:**
- Check waste percentages
- Verify purchase units and prices
- Ensure all fields are filled correctly

---

**🎉 The application is now complete and ready to use! Start by adding your ingredients in the Item Master, then create recipes and analyze costs.**