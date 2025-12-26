using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using RecipeCostingApp.Models;
using System.Text.RegularExpressions;

namespace RecipeCostingApp.Services
{
    public class ImportService
    {
        private readonly IngredientService _ingredientService;

        public ImportService()
        {
            _ingredientService = new IngredientService();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public async Task<List<Ingredient>> ImportFromExcelAsync(string filePath)
        {
            var ingredients = new List<Ingredient>();

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            
            if (worksheet == null)
                throw new InvalidOperationException("No worksheet found in Excel file");

            // Find header row
            int headerRow = FindHeaderRow(worksheet);
            if (headerRow == -1)
                throw new InvalidOperationException("Could not find valid headers in Excel file");

            var columnMap = MapColumns(worksheet, headerRow);

            for (int row = headerRow + 1; row <= worksheet.Dimension.End.Row; row++)
            {
                try
                {
                    var ingredient = ParseIngredientFromRow(worksheet, row, columnMap);
                    if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                    {
                        ingredients.Add(ingredient);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other rows
                    Console.WriteLine($"Error processing row {row}: {ex.Message}");
                }
            }

            return ingredients;
        }

        public async Task<List<Ingredient>> ImportFromPdfAsync(string filePath)
        {
            var ingredients = new List<Ingredient>();
            var text = ExtractTextFromPdf(filePath);
            
            // Parse ingredients from text using patterns
            var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines)
            {
                var ingredient = ParseIngredientFromText(line.Trim());
                if (ingredient != null && !string.IsNullOrWhiteSpace(ingredient.Name))
                {
                    ingredients.Add(ingredient);
                }
            }

            return ingredients;
        }

        public async Task<string> ProcessImageAsync(string imagePath)
        {
            // For now, return the image path as a reference
            // In a full implementation, you could use OCR libraries like Tesseract
            return $"Image processed: {Path.GetFileName(imagePath)}";
        }

        private int FindHeaderRow(ExcelWorksheet worksheet)
        {
            for (int row = 1; row <= Math.Min(5, worksheet.Dimension.End.Row); row++)
            {
                var firstCell = worksheet.Cells[row, 1].Text?.ToLower();
                if (firstCell != null && (firstCell.Contains("name") || firstCell.Contains("ingredient") || firstCell.Contains("item")))
                {
                    return row;
                }
            }
            return 1; // Default to first row
        }

        private Dictionary<string, int> MapColumns(ExcelWorksheet worksheet, int headerRow)
        {
            var columnMap = new Dictionary<string, int>();
            var additionalColumns = new Dictionary<string, int>();
            
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                var header = worksheet.Cells[headerRow, col].Text?.ToLower().Trim();
                if (string.IsNullOrEmpty(header)) continue;

                // Map known fields (case-insensitive)
                if (header.Contains("name") || header.Contains("ingredient") || header.Contains("item"))
                    columnMap["name"] = col;
                else if (header.Contains("category") || header.Contains("type"))
                    columnMap["category"] = col;
                else if (header.Contains("unit") && !header.Contains("purchase"))
                    columnMap["unit"] = col;
                else if (header.Contains("purchase") && header.Contains("unit"))
                    columnMap["purchaseunit"] = col;
                else if (header.Contains("price") || header.Contains("cost"))
                    columnMap["price"] = col;
                else if (header.Contains("waste"))
                    columnMap["waste"] = col;
                else
                {
                    // Store additional fields
                    additionalColumns[header] = col;
                }
            }
            
            // Add additional columns to the map with "additional_" prefix
            foreach (var kvp in additionalColumns)
            {
                columnMap[$"additional_{kvp.Key}"] = kvp.Value;
            }

            return columnMap;
        }

        private Ingredient ParseIngredientFromRow(ExcelWorksheet worksheet, int row, Dictionary<string, int> columnMap)
        {
            var ingredient = new Ingredient();

            // Parse standard fields
            if (columnMap.ContainsKey("name"))
                ingredient.Name = worksheet.Cells[row, columnMap["name"]].Text?.Trim();

            if (columnMap.ContainsKey("category"))
                ingredient.Category = worksheet.Cells[row, columnMap["category"]].Text?.Trim() ?? "General";
            else
                ingredient.Category = "General";

            if (columnMap.ContainsKey("unit"))
                ingredient.Unit = worksheet.Cells[row, columnMap["unit"]].Text?.Trim() ?? "g";
            else
                ingredient.Unit = "g";

            if (columnMap.ContainsKey("purchaseunit"))
            {
                if (decimal.TryParse(worksheet.Cells[row, columnMap["purchaseunit"]].Text, out decimal purchaseUnit))
                    ingredient.PurchaseUnit = purchaseUnit;
                else
                    ingredient.PurchaseUnit = 1000;
            }
            else
            {
                ingredient.PurchaseUnit = 1000;
            }

            if (columnMap.ContainsKey("price"))
            {
                if (decimal.TryParse(worksheet.Cells[row, columnMap["price"]].Text, out decimal price))
                    ingredient.Price = price;
            }

            if (columnMap.ContainsKey("waste"))
            {
                if (decimal.TryParse(worksheet.Cells[row, columnMap["waste"]].Text, out decimal waste))
                    ingredient.WastePercentage = waste;
            }

            // Parse additional fields
            foreach (var kvp in columnMap.Where(c => c.Key.StartsWith("additional_")))
            {
                var fieldName = kvp.Key.Substring(11); // Remove "additional_" prefix
                var cellValue = worksheet.Cells[row, kvp.Value].Text?.Trim();
                if (!string.IsNullOrEmpty(cellValue))
                {
                    ingredient.AdditionalFields[fieldName] = cellValue;
                }
            }

            return ingredient;
        }

        private string ExtractTextFromPdf(string filePath)
        {
            var text = "";
            
            using var reader = new PdfReader(filePath);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            
            return text;
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