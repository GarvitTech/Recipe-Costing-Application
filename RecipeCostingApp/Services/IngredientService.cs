using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RecipeCostingApp.Data;
using RecipeCostingApp.Models;
using System.Text.Json;

namespace RecipeCostingApp.Services
{
    public class IngredientService
    {
        public async Task<List<Ingredient>> GetAllIngredientsAsync()
        {
            var ingredients = new List<Ingredient>();

            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Ingredients ORDER BY Name";
            using var command = new SqliteCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ingredients.Add(MapIngredient(reader));
            }

            return ingredients;
        }

        public async Task<Ingredient> GetIngredientByIdAsync(int id)
        {
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Ingredients WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapIngredient(reader);
            }

            return null;
        }

        public async Task<int> SaveIngredientAsync(Ingredient ingredient)
        {
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            if (ingredient.Id == 0)
            {
                // Insert new ingredient
                var insertQuery = @"
                    INSERT INTO Ingredients (Name, Category, Unit, PurchaseUnit, Price, WastePercentage, Yield, AdditionalFields, CreatedDate, ModifiedDate)
                    VALUES (@name, @category, @unit, @purchaseUnit, @price, @wastePercentage, @yield, @additionalFields, @createdDate, @modifiedDate);
                    SELECT last_insert_rowid();";

                using var command = new SqliteCommand(insertQuery, connection);
                AddIngredientParameters(command, ingredient);
                ingredient.CreatedDate = DateTime.Now;
                ingredient.ModifiedDate = DateTime.Now;
                command.Parameters.AddWithValue("@createdDate", ingredient.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@modifiedDate", ingredient.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@additionalFields", JsonSerializer.Serialize(ingredient.AdditionalFields));

                var result = await command.ExecuteScalarAsync();
                ingredient.Id = Convert.ToInt32(result);
                return ingredient.Id;
            }
            else
            {
                // Update existing ingredient
                var updateQuery = @"
                    UPDATE Ingredients 
                    SET Name = @name, Category = @category, Unit = @unit, PurchaseUnit = @purchaseUnit, 
                        Price = @price, WastePercentage = @wastePercentage, Yield = @yield, AdditionalFields = @additionalFields, ModifiedDate = @modifiedDate
                    WHERE Id = @id";

                using var command = new SqliteCommand(updateQuery, connection);
                AddIngredientParameters(command, ingredient);
                ingredient.ModifiedDate = DateTime.Now;
                command.Parameters.AddWithValue("@modifiedDate", ingredient.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@additionalFields", JsonSerializer.Serialize(ingredient.AdditionalFields));
                command.Parameters.AddWithValue("@id", ingredient.Id);

                await command.ExecuteNonQueryAsync();
                return ingredient.Id;
            }
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Ingredients WHERE Id = @id";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var categories = new List<string>();

            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT DISTINCT Category FROM Ingredients ORDER BY Category";
            using var command = new SqliteCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(reader.GetString(0));
            }

            return categories;
        }

        private static Ingredient MapIngredient(SqliteDataReader reader)
        {
            var ingredient = new Ingredient
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                Unit = reader.GetString(3),
                PurchaseUnit = reader.GetDecimal(4),
                Price = reader.GetDecimal(5),
                WastePercentage = reader.GetDecimal(6),
                Yield = reader.GetDecimal(7),
                CreatedDate = DateTime.Parse(reader.GetString(9)),
                ModifiedDate = DateTime.Parse(reader.GetString(10))
            };
            
            // Deserialize AdditionalFields if present
            try
            {
                var additionalFieldsJson = reader.IsDBNull(8) ? "{}" : reader.GetString(8);
                ingredient.AdditionalFields = JsonSerializer.Deserialize<Dictionary<string, string>>(additionalFieldsJson) ?? new Dictionary<string, string>();
            }
            catch
            {
                ingredient.AdditionalFields = new Dictionary<string, string>();
            }
            
            return ingredient;
        }

        private static void AddIngredientParameters(SqliteCommand command, Ingredient ingredient)
        {
            command.Parameters.AddWithValue("@name", ingredient.Name);
            command.Parameters.AddWithValue("@category", ingredient.Category);
            command.Parameters.AddWithValue("@unit", ingredient.Unit);
            command.Parameters.AddWithValue("@purchaseUnit", ingredient.PurchaseUnit);
            command.Parameters.AddWithValue("@price", ingredient.Price);
            command.Parameters.AddWithValue("@wastePercentage", ingredient.WastePercentage);
            command.Parameters.AddWithValue("@yield", ingredient.Yield);
        }
    }
}