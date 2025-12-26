using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RecipeCostingApp.Models;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace RecipeCostingApp.Services
{
    public class ImportService
    {
        private readonly IngredientService _ingredientService;

        public ImportService()
        {
            _ingredientService = new IngredientService();
        }

        public async Task<List<Ingredient>> ImportFromExcelAsync(string filePath)
        {
            // For now, treat Excel files as CSV
            return await ImportFromCsvAsync(filePath);
        }

        public async Task<List<Ingredient>> ImportFromCsvAsync(string filePath)
        {
            var ingredients = new List<Ingredient>();
            var lines = await File.ReadAllLinesAsync(filePath);
            
            if (lines.Length < 2) return ingredients;

            var headers = ParseCsvLine(lines[0]);
            var columnMap = MapColumns(headers);

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var values = ParseCsvLine(lines[i]);
                    var ingredient = ParseIngredientFromValues(values, columnMap, headers);
                    if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                    {
                        ingredients.Add(ingredient);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row {i + 1}: {ex.Message}");
                }
            }

            return ingredients;
        }

        public async Task<List<Ingredient>> ImportFromPdfAsync(string filePath)
        {
            var ingredients = new List<Ingredient>();
            
            // Simple text file reading for now
            try
            {
                var text = await File.ReadAllTextAsync(filePath);
                var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in lines)
                {
                    var ingredient = ParseIngredientFromText(line.Trim());
                    if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                    {
                        ingredients.Add(ingredient);
                    }
                }
            }
            catch
            {
                throw new InvalidOperationException("Could not read PDF file. Please convert to text format first.");
            }

            return ingredients;
        }

        public async Task<string> ProcessImageAsync(string imagePath)
        {
            return $"Image processed: {Path.GetFileName(imagePath)}";
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
            var additionalColumns = new Dictionary<string, int>();
            
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
                else
                {
                    additionalColumns[header] = i;
                }
            }
            
            foreach (var kvp in additionalColumns)
            {
                columnMap[$"additional_{kvp.Key}"] = kvp.Value;
            }

            return columnMap;
        }

        private Ingredient ParseIngredientFromValues(string[] values, Dictionary<string, int> columnMap, string[] headers)
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

            // Parse additional fields - skip for now
            // foreach (var kvp in columnMap.Where(c => c.Key.StartsWith("additional_")))
            // {
            //     var fieldName = kvp.Key.Substring(11);
            //     if (kvp.Value < values.Length)
            //     {
            //         var cellValue = values[kvp.Value]?.Trim();
            //         if (!string.IsNullOrEmpty(cellValue))
            //         {
            //             ingredient.AdditionalFields[fieldName] = cellValue;
            //         }
            //     }
            // }

            return ingredient;
        }

private Ingredient ParseIngredientFromText(string line)
        {
            // Simple pattern matching for ingredient data
            // Format: "Name - Category - Price - Unit"
            var patterns = new[]
            {
                @"^(.+?)\s*-\s*(.+?)\s*-\s*(\d+\.?\d*)\s*-\s*(.+?)$",
                @"^(.+?)\s+(\d+\.?\d*)\s+(.+?)$",
                @"^(.+?)\s*,\s*(.+?)\s*,\s*(\d+\.?\d*).*$"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(line, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var ingredient = new Ingredient
                    {
                        Name = match.Groups[1].Value.Trim(),
                        Category = match.Groups.Count > 4 ? match.Groups[2].Value.Trim() : "General",
                        Unit = "g",
                        PurchaseUnit = 1000,
                        WastePercentage = 0
                    };

                    // Try to parse price
                    var priceGroup = match.Groups.Count > 4 ? match.Groups[3].Value : match.Groups[2].Value;
                    if (decimal.TryParse(priceGroup, out decimal price))
                    {
                        ingredient.Price = price;
                    }

                    return ingredient;
                }
            }

            // If no pattern matches, create basic ingredient with just name
            if (!string.IsNullOrWhiteSpace(line) && line.Length > 2)
            {
                return new Ingredient
                {
                    Name = line,
                    Category = "General",
                    Unit = "g",
                    PurchaseUnit = 1000,
                    Price = 0,
                    WastePercentage = 0
                };
            }

            return null;
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