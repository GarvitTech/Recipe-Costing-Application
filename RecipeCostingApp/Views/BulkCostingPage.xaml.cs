using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RecipeCostingApp.Models;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class BulkCostingPage : Page
    {
        private readonly RecipeService _recipeService;
        private readonly PdfService _pdfService;
        private Recipe _selectedRecipe;
        private ObservableCollection<BulkIngredient> _bulkIngredients;
        private int _currentMultiplier = 50;

        public BulkCostingPage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();
            _pdfService = new PdfService();
            _bulkIngredients = new ObservableCollection<BulkIngredient>();
            DgBulkIngredients.ItemsSource = _bulkIngredients;
            LoadRecipes();
        }

        private async void LoadRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            CmbBulkRecipes.ItemsSource = recipes;
        }

        private void CmbBulkRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbBulkRecipes.SelectedItem is Recipe recipe)
            {
                _selectedRecipe = recipe;
                CalculateBulkCosting();
            }
        }

        private void TxtMultiplier_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TxtMultiplier.Text, out int multiplier) && multiplier > 0)
            {
                _currentMultiplier = multiplier;
                CalculateBulkCosting();
            }
        }

        private void BtnCalculateBulk_Click(object sender, RoutedEventArgs e)
        {
            CalculateBulkCosting();
        }

        private void BtnQuickMultiplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && int.TryParse(button.Tag.ToString(), out int multiplier))
            {
                TxtMultiplier.Text = multiplier.ToString();
                _currentMultiplier = multiplier;
                CalculateBulkCosting();
            }
        }

        private void CalculateBulkCosting()
        {
            if (_selectedRecipe == null) return;

            _bulkIngredients.Clear();
            foreach (var ingredient in _selectedRecipe.Ingredients)
            {
                _bulkIngredients.Add(new BulkIngredient
                {
                    RecipeIngredient = ingredient,
                    Multiplier = _currentMultiplier
                });
            }

            var totalCost = _selectedRecipe.FinalCost * _currentMultiplier;
            
            TxtBulkMultiplier.Text = $"Multiplier: x{_currentMultiplier}";
            TxtBulkRecipeCost.Text = $"Unit Recipe Cost: {_selectedRecipe.RecipeCost:C2}";
            TxtBulkTotalCost.Text = $"Total Recipe Cost: {_selectedRecipe.RecipeCost * _currentMultiplier:C2}";
            TxtBulkFinalCost.Text = $"Final Cost: {totalCost:C2}";
        }

        private void BtnGenerateBulkPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.", "No Recipe Selected");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"{_selectedRecipe.Name}_BulkCosting_x{_currentMultiplier}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GenerateBulkCostingSheet(_selectedRecipe, _currentMultiplier, saveDialog.FileName);
                    MessageBox.Show("Bulk costing PDF generated successfully!", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating PDF: {ex.Message}", "Error");
                }
            }
        }
    }
}