using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RecipeCostingApp.Models;

namespace RecipeCostingApp.Services
{
    public class ImportService
    {
        private readonly IngredientService _ingredientService;

        public ImportService()
        {
            _ingredientService = new IngredientService();
        }

        public async Task<List<Ingredient>> ImportFromCsvAsync(string filePath)
        {
            var ingredients = new List<Ingredient>();
            
            try
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    throw new FileNotFoundException("File not found or path is invalid.");
                }
                
                var lines = await File.ReadAllLinesAsync(filePath);
                
                if (lines.Length < 2) return ingredients;

                var headers = ParseCsvLine(lines[0]);
                if (headers == null || headers.Length == 0)
                {
                    throw new InvalidOperationException("No valid headers found in CSV file.");
                }
                
                var columnMap = MapColumns(headers);

                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(lines[i])) continue;
                        
                        var values = ParseCsvLine(lines[i]);
                        if (values == null || values.Length == 0) continue;
                        
                        var ingredient = ParseIngredientFromValues(values, columnMap);
                        if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                        {
                            ingredients.Add(ingredient);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error processing row {i + 1}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading CSV file: {ex.Message}", ex);
            }

            return ingredients;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string currentField = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.Trim());
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }
            result.Add(currentField.Trim());
            return result.ToArray();
        }

        private Dictionary<string, int> MapColumns(string[] headers)
        {
            var columnMap = new Dictionary<string, int>();
            
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i].ToLower().Trim();
                if (string.IsNullOrEmpty(header)) continue;

                if (header.Contains("name") || header.Contains("ingredient") || header.Contains("item"))
                    columnMap["name"] = i;
                else if (header.Contains("category") || header.Contains("type"))
                    columnMap["category"] = i;
                else if (header.Contains("unit") && !header.Contains("purchase"))
                    columnMap["unit"] = i;
                else if (header.Contains("purchase") && header.Contains("unit"))
                    columnMap["purchaseunit"] = i;
                else if (header.Contains("price") || header.Contains("cost"))
                    columnMap["price"] = i;
                else if (header.Contains("waste"))
                    columnMap["waste"] = i;
            }

            return columnMap;
        }

        private Ingredient ParseIngredientFromValues(string[] values, Dictionary<string, int> columnMap)
        {
            var ingredient = new Ingredient();

            if (columnMap.ContainsKey("name") && columnMap["name"] < values.Length)
                ingredient.Name = values[columnMap["name"]]?.Trim();

            if (columnMap.ContainsKey("category") && columnMap["category"] < values.Length)
                ingredient.Category = values[columnMap["category"]]?.Trim() ?? "General";
            else
                ingredient.Category = "General";

            if (columnMap.ContainsKey("unit") && columnMap["unit"] < values.Length)
                ingredient.Unit = values[columnMap["unit"]]?.Trim() ?? "g";
            else
                ingredient.Unit = "g";

            if (columnMap.ContainsKey("purchaseunit") && columnMap["purchaseunit"] < values.Length)
            {
                if (decimal.TryParse(values[columnMap["purchaseunit"]], out decimal purchaseUnit))
                    ingredient.PurchaseUnit = purchaseUnit;
                else
                    ingredient.PurchaseUnit = 1000;
            }
            else
            {
                ingredient.PurchaseUnit = 1000;
            }

            if (columnMap.ContainsKey("price") && columnMap["price"] < values.Length)
            {
                if (decimal.TryParse(values[columnMap["price"]], out decimal price))
                    ingredient.Price = price;
            }

            if (columnMap.ContainsKey("waste") && columnMap["waste"] < values.Length)
            {
                if (decimal.TryParse(values[columnMap["waste"]], out decimal waste))
                    ingredient.WastePercentage = waste;
            }

            return ingredient;
        }

        public async Task<int> SaveImportedIngredientsAsync(List<Ingredient> ingredients, bool skipDuplicates = true)
        {
            int savedCount = 0;
            var existingIngredients = await _ingredientService.GetAllIngredientsAsync();
            var existingNames = existingIngredients.Select(i => i.Name.ToLower()).ToHashSet();

            foreach (var ingredient in ingredients)
            {
                try
                {
                    if (skipDuplicates && existingNames.Contains(ingredient.Name.ToLower()))
                        continue;

                    await _ingredientService.SaveIngredientAsync(ingredient);
                    savedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving ingredient {ingredient.Name}: {ex.Message}");
                }
            }

            return savedCount;
        }
    }
}