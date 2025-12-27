using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace RecipeCostingApp.Data
{
    public static class DatabaseManager
    {
        private static readonly string DatabasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RecipeCostingApp",
            "RecipeCosting.db"
        );

        public static string ConnectionString => $"Data Source={DatabasePath}";

        public static void InitializeDatabase()
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(DatabasePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create database and tables if they don't exist
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                CreateTables(connection);
                SeedDefaultData(connection);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        private static void CreateTables(SqliteConnection connection)
        {
            var createTablesScript = @"
                CREATE TABLE IF NOT EXISTS Ingredients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    Unit TEXT NOT NULL,
                    PurchaseUnit DECIMAL NOT NULL,
                    Price DECIMAL NOT NULL,
                    WastePercentage DECIMAL DEFAULT 0,
                    Yield DECIMAL DEFAULT 100,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Recipes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Category TEXT NOT NULL,
                    WastePercentage DECIMAL DEFAULT 0,
                    GstPercentage DECIMAL DEFAULT 0,
                    PackagingCharges DECIMAL DEFAULT 0,
                    DeliveryCharges DECIMAL DEFAULT 0,
                    SellingPrice DECIMAL DEFAULT 0,
                    Portions INTEGER DEFAULT 1,
                    ParentRecipeId INTEGER DEFAULT 0,
                    IsSubRecipe BOOLEAN DEFAULT 0,
                    CreatedDate TEXT NOT NULL,
                    ModifiedDate TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS RecipeIngredients (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    RecipeId INTEGER NOT NULL,
                    IngredientId INTEGER NOT NULL,
                    Quantity DECIMAL NOT NULL,
                    FOREIGN KEY (RecipeId) REFERENCES Recipes (Id) ON DELETE CASCADE,
                    FOREIGN KEY (IngredientId) REFERENCES Ingredients (Id) ON DELETE CASCADE
                );

                CREATE INDEX IF NOT EXISTS IX_RecipeIngredients_RecipeId ON RecipeIngredients (RecipeId);
                CREATE INDEX IF NOT EXISTS IX_RecipeIngredients_IngredientId ON RecipeIngredients (IngredientId);
            ";

            using var command = new SqliteCommand(createTablesScript, connection);
            command.ExecuteNonQuery();
        }

        private static void SeedDefaultData(SqliteConnection connection)
        {
            // Check if data already exists
            using var checkCommand = new SqliteCommand("SELECT COUNT(*) FROM Ingredients", connection);
            var count = Convert.ToInt32(checkCommand.ExecuteScalar());

            if (count > 0) return; // Data already exists

            // Insert sample ingredients
            var seedScript = @"
                INSERT INTO Ingredients (Name, Category, Unit, PurchaseUnit, Price, WastePercentage, Yield, CreatedDate, ModifiedDate) VALUES
                ('Tomato', 'Vegetables', 'g', 1000, 50.00, 10, 90, datetime('now'), datetime('now')),
                ('Onion', 'Vegetables', 'g', 1000, 30.00, 15, 85, datetime('now'), datetime('now')),
                ('Chicken Breast', 'Meat', 'g', 1000, 250.00, 5, 95, datetime('now'), datetime('now')),
                ('Rice', 'Dry Store', 'g', 1000, 80.00, 2, 98, datetime('now'), datetime('now')),
                ('Olive Oil', 'Oils', 'ml', 1000, 300.00, 0, 100, datetime('now'), datetime('now')),
                ('Salt', 'Spices', 'g', 1000, 20.00, 0, 100, datetime('now'), datetime('now')),
                ('Black Pepper', 'Spices', 'g', 100, 150.00, 0, 100, datetime('now'), datetime('now'));
            ";

            using var seedCommand = new SqliteCommand(seedScript, connection);
            seedCommand.ExecuteNonQuery();
        }
    }
}