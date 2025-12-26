using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using RecipeCostingApp.Data;
using RecipeCostingApp.Models;

namespace RecipeCostingApp.Services
{
    public class RecipeService
    {
        public async Task<List<Recipe>> GetAllRecipesAsync()
        {
            var recipes = new List<Recipe>();
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Recipes ORDER BY Name";
            using var command = new SqliteCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var recipe = MapRecipe(reader);
                recipe.Ingredients = new ObservableCollection<RecipeIngredient>(await GetRecipeIngredientsAsync(recipe.Id));
                recipes.Add(recipe);
            }
            return recipes;
        }

        public async Task<int> SaveRecipeAsync(Recipe recipe)
        {
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                if (recipe.Id == 0)
                {
                    var insertQuery = @"INSERT INTO Recipes (Name, Category, WastePercentage, GstPercentage, PackagingCharges, DeliveryCharges, SellingPrice, CreatedDate, ModifiedDate)
                        VALUES (@name, @category, @waste, @gst, @packaging, @delivery, @selling, @created, @modified);
                        SELECT last_insert_rowid();";
                    
                    using var command = new SqliteCommand(insertQuery, connection, transaction);
                    AddRecipeParameters(command, recipe);
                    recipe.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
                else
                {
                    var updateQuery = @"UPDATE Recipes SET Name=@name, Category=@category, WastePercentage=@waste, 
                        GstPercentage=@gst, PackagingCharges=@packaging, DeliveryCharges=@delivery, 
                        SellingPrice=@selling, ModifiedDate=@modified WHERE Id=@id";
                    
                    using var command = new SqliteCommand(updateQuery, connection, transaction);
                    AddRecipeParameters(command, recipe);
                    command.Parameters.AddWithValue("@id", recipe.Id);
                    await command.ExecuteNonQueryAsync();
                }

                await DeleteRecipeIngredientsAsync(recipe.Id, connection, transaction);
                await SaveRecipeIngredientsAsync(recipe, connection, transaction);
                
                transaction.Commit();
                return recipe.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<List<RecipeIngredient>> GetRecipeIngredientsAsync(int recipeId)
        {
            var ingredients = new List<RecipeIngredient>();
            using var connection = new SqliteConnection(DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            var query = @"SELECT ri.*, i.* FROM RecipeIngredients ri 
                         JOIN Ingredients i ON ri.IngredientId = i.Id 
                         WHERE ri.RecipeId = @recipeId";
            
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@recipeId", recipeId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                ingredients.Add(new RecipeIngredient
                {
                    Id = reader.GetInt32(0), // ri.Id
                    RecipeId = reader.GetInt32(1), // ri.RecipeId
                    IngredientId = reader.GetInt32(2), // ri.IngredientId
                    Quantity = reader.GetDecimal(3), // ri.Quantity
                    Ingredient = new Ingredient
                    {
                        Id = reader.GetInt32(4), // i.Id
                        Name = reader.GetString(5), // i.Name
                        Category = reader.GetString(6), // i.Category
                        Unit = reader.GetString(7), // i.Unit
                        PurchaseUnit = reader.GetDecimal(8), // i.PurchaseUnit
                        Price = reader.GetDecimal(9), // i.Price
                        WastePercentage = reader.GetDecimal(10) // i.WastePercentage
                    }
                });
            }
            return ingredients;
        }

        private async Task SaveRecipeIngredientsAsync(Recipe recipe, SqliteConnection connection, SqliteTransaction transaction)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                var query = "INSERT INTO RecipeIngredients (RecipeId, IngredientId, Quantity) VALUES (@recipeId, @ingredientId, @quantity)";
                using var command = new SqliteCommand(query, connection, transaction);
                command.Parameters.AddWithValue("@recipeId", recipe.Id);
                command.Parameters.AddWithValue("@ingredientId", ingredient.IngredientId);
                command.Parameters.AddWithValue("@quantity", ingredient.Quantity);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task DeleteRecipeIngredientsAsync(int recipeId, SqliteConnection connection, SqliteTransaction transaction)
        {
            var query = "DELETE FROM RecipeIngredients WHERE RecipeId = @recipeId";
            using var command = new SqliteCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@recipeId", recipeId);
            await command.ExecuteNonQueryAsync();
        }

        private static Recipe MapRecipe(SqliteDataReader reader)
        {
            return new Recipe
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                WastePercentage = reader.GetDecimal(3),
                GstPercentage = reader.GetDecimal(4),
                PackagingCharges = reader.GetDecimal(5),
                DeliveryCharges = reader.GetDecimal(6),
                SellingPrice = reader.GetDecimal(7),
                CreatedDate = DateTime.Parse(reader.GetString(8)),
                ModifiedDate = DateTime.Parse(reader.GetString(9))
            };
        }

        private static void AddRecipeParameters(SqliteCommand command, Recipe recipe)
        {
            recipe.ModifiedDate = DateTime.Now;
            if (recipe.Id == 0) recipe.CreatedDate = DateTime.Now;
            
            command.Parameters.AddWithValue("@name", recipe.Name);
            command.Parameters.AddWithValue("@category", recipe.Category);
            command.Parameters.AddWithValue("@waste", recipe.WastePercentage);
            command.Parameters.AddWithValue("@gst", recipe.GstPercentage);
            command.Parameters.AddWithValue("@packaging", recipe.PackagingCharges);
            command.Parameters.AddWithValue("@delivery", recipe.DeliveryCharges);
            command.Parameters.AddWithValue("@selling", recipe.SellingPrice);
            command.Parameters.AddWithValue("@created", recipe.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@modified", recipe.ModifiedDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}