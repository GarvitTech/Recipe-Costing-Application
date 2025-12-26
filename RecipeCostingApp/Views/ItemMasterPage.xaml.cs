using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipeCostingApp.Models;
using RecipeCostingApp.Services;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;

namespace RecipeCostingApp.Views
{
    public partial class ItemMasterPage : Page
    {
        private readonly IngredientService _ingredientService;
        private readonly ImportService _importService;
        private List<Ingredient> _allIngredients;
        private Ingredient _currentIngredient;

        public ItemMasterPage()
        {
            InitializeComponent();
            _ingredientService = new IngredientService();
            _importService = new ImportService();
            LoadData();
            SetupEventHandlers();
        }

        private async void LoadData()
        {
            try
            {
                _allIngredients = await _ingredientService.GetAllIngredientsAsync();
                DgIngredients.ItemsSource = _allIngredients;

                // Load categories for dropdown
                var categories = await _ingredientService.GetCategoriesAsync();
                categories.AddRange(new[] { "Vegetables", "Meat", "Dairy", "Dry Store", "Spices", "Oils" });
                CmbCategory.ItemsSource = categories.Distinct().OrderBy(c => c).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupEventHandlers()
        {
            TxtPrice.TextChanged += CalculateUnitCost;
            TxtPurchaseUnit.TextChanged += CalculateUnitCost;
            TxtWastePercentage.TextChanged += CalculateYield;
        }

        private void CalculateUnitCost(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(TxtPrice.Text, out decimal price) && 
                decimal.TryParse(TxtPurchaseUnit.Text, out decimal purchaseUnit) && 
                purchaseUnit > 0)
            {
                var unitCost = price / purchaseUnit;
                TxtUnitCost.Text = unitCost.ToString("F4");
            }
            else
            {
                TxtUnitCost.Text = "0.0000";
            }
        }

        private void CalculateYield(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(TxtWastePercentage.Text, out decimal waste))
            {
                var yield = 100 - waste;
                TxtYield.Text = yield.ToString("F1");
            }
            else
            {
                TxtYield.Text = "100.0";
            }
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                var ingredient = _currentIngredient ?? new Ingredient();
                
                ingredient.Name = TxtName.Text.Trim();
                ingredient.Category = CmbCategory.Text.Trim();
                ingredient.Unit = CmbUnit.Text;
                ingredient.PurchaseUnit = decimal.Parse(TxtPurchaseUnit.Text);
                ingredient.Price = decimal.Parse(TxtPrice.Text);
                ingredient.WastePercentage = decimal.Parse(TxtWastePercentage.Text);

                await _ingredientService.SaveIngredientAsync(ingredient);

                MessageBox.Show("Ingredient saved successfully!", "Success", 
                              MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving ingredient: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Please enter ingredient name.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(CmbCategory.Text))
            {
                MessageBox.Show("Please select or enter a category.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbCategory.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(CmbUnit.Text))
            {
                MessageBox.Show("Please select a unit.", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbUnit.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtPurchaseUnit.Text, out decimal purchaseUnit) || purchaseUnit <= 0)
            {
                MessageBox.Show("Please enter a valid purchase unit (greater than 0).", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPurchaseUnit.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price (0 or greater).", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrice.Focus();
                return false;
            }

            if (!decimal.TryParse(TxtWastePercentage.Text, out decimal waste) || waste < 0 || waste >= 100)
            {
                MessageBox.Show("Please enter a valid waste percentage (0-99).", "Validation Error", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtWastePercentage.Focus();
                return false;
            }

            return true;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _currentIngredient = null;
            TxtName.Text = "";
            CmbCategory.Text = "";
            CmbUnit.SelectedIndex = -1;
            TxtPurchaseUnit.Text = "1000";
            TxtPrice.Text = "";
            TxtWastePercentage.Text = "0";
            TxtUnitCost.Text = "";
            TxtYield.Text = "100.0";
            TxtName.Focus();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Ingredient ingredient)
            {
                EditIngredient(ingredient);
            }
        }

        private void EditIngredient(Ingredient ingredient)
        {
            _currentIngredient = ingredient;
            TxtName.Text = ingredient.Name;
            CmbCategory.Text = ingredient.Category;
            CmbUnit.Text = ingredient.Unit;
            TxtPurchaseUnit.Text = ingredient.PurchaseUnit.ToString();
            TxtPrice.Text = ingredient.Price.ToString();
            TxtWastePercentage.Text = ingredient.WastePercentage.ToString();
            
            // Show additional fields if any
            if (ingredient.AdditionalFields.Any())
            {
                ShowAdditionalFieldsDialog(ingredient);
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Ingredient ingredient)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{ingredient.Name}'?", 
                                           "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _ingredientService.DeleteIngredientAsync(ingredient.Id);
                        MessageBox.Show("Ingredient deleted successfully!", "Success", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting ingredient: {ex.Message}", "Error", 
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DgIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgIngredients.SelectedItem is Ingredient ingredient)
            {
                EditIngredient(ingredient);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterIngredients();
        }

        private void FilterIngredients()
        {
            if (_allIngredients == null) return;

            var searchText = TxtSearch.Text.ToLower();
            var filteredIngredients = _allIngredients.Where(i => 
                i.Name.ToLower().Contains(searchText) || 
                i.Category.ToLower().Contains(searchText)).ToList();

            DgIngredients.ItemsSource = filteredIngredients;
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            TxtSearch.Text = "";
        }

        private async void BtnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Excel File",
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                await ImportFromFileAsync(openFileDialog.FileName, "Excel");
            }
        }

        private async void BtnImportPdf_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select PDF File",
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                await ImportFromFileAsync(openFileDialog.FileName, "PDF");
            }
        }

        private async void BtnImportImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select Image File",
                Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                await ImportFromFileAsync(openFileDialog.FileName, "Image");
            }
        }

        private async Task ImportFromFileAsync(string filePath, string fileType)
        {
            try
            {
                // Show loading indicator
                var originalCursor = this.Cursor;
                this.Cursor = System.Windows.Input.Cursors.Wait;

                List<Ingredient> importedIngredients = new List<Ingredient>();

                switch (fileType.ToUpper())
                {
                    case "EXCEL":
                        importedIngredients = await _importService.ImportFromExcelAsync(filePath);
                        break;
                    case "PDF":
                        importedIngredients = await _importService.ImportFromPdfAsync(filePath);
                        break;
                    case "IMAGE":
                        var result = await _importService.ProcessImageAsync(filePath);
                        MessageBox.Show($"Image processing result: {result}\n\nNote: OCR functionality requires additional setup. For now, please manually enter ingredients from the image.", 
                                      "Image Import", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                }

                this.Cursor = originalCursor;

                if (importedIngredients.Count == 0)
                {
                    MessageBox.Show($"No ingredients found in the {fileType} file. Please check the file format and try again.", 
                                  "Import Result", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Show preview dialog
                var previewResult = ShowImportPreview(importedIngredients, fileType);
                if (previewResult == MessageBoxResult.Yes)
                {
                    var savedCount = await _importService.SaveImportedIngredientsAsync(importedIngredients, true);
                    MessageBox.Show($"Successfully imported {savedCount} out of {importedIngredients.Count} ingredients.\n\nDuplicates were skipped.", 
                                  "Import Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = System.Windows.Input.Cursors.Arrow;
                MessageBox.Show($"Error importing from {fileType}: {ex.Message}", "Import Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private MessageBoxResult ShowImportPreview(List<Ingredient> ingredients, string fileType)
        {
            var previewText = $"Found {ingredients.Count} ingredients in {fileType} file:\n\n";
            
            // Show first 10 ingredients as preview
            var previewItems = ingredients.Take(10);
            foreach (var ingredient in previewItems)
            {
                previewText += $"• {ingredient.Name} ({ingredient.Category}) - {ingredient.Price:C}";
                if (ingredient.AdditionalFields.Any())
                {
                    previewText += $" [+{ingredient.AdditionalFields.Count} extra fields]";
                }
                previewText += "\n";
            }

            if (ingredients.Count > 10)
            {
                previewText += $"... and {ingredients.Count - 10} more ingredients\n";
            }

            // Show additional fields found
            var allAdditionalFields = ingredients
                .SelectMany(i => i.AdditionalFields.Keys)
                .Distinct()
                .ToList();
            
            if (allAdditionalFields.Any())
            {
                previewText += $"\nAdditional fields found: {string.Join(", ", allAdditionalFields)}\n";
            }

            previewText += "\nDo you want to import these ingredients?\n(Duplicates will be skipped)";

            return MessageBox.Show(previewText, "Import Preview", 
                                 MessageBoxButton.YesNo, MessageBoxImage.Question);
        }
        
        private void ShowAdditionalFieldsDialog(Ingredient ingredient)
        {
            if (!ingredient.AdditionalFields.Any()) return;
            
            var fieldsText = $"Additional fields for '{ingredient.Name}':\n\n";
            foreach (var field in ingredient.AdditionalFields)
            {
                fieldsText += $"{field.Key}: {field.Value}\n";
            }
            fieldsText += "\nThese fields were imported from your document.\nYou can delete them by clearing the ingredient's additional data.";
            
            var result = MessageBox.Show(fieldsText, "Additional Fields", 
                                       MessageBoxButton.OKCancel, MessageBoxImage.Information);
            
            if (result == MessageBoxResult.Cancel)
            {
                var clearResult = MessageBox.Show("Do you want to clear all additional fields for this ingredient?", 
                                                 "Clear Additional Fields", 
                                                 MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (clearResult == MessageBoxResult.Yes)
                {
                    ingredient.AdditionalFields.Clear();
                    MessageBox.Show("Additional fields cleared.", "Success", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}