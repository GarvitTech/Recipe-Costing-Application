using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RecipeCostingApp.Models;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class CostingAnalysisPage : Page
    {
        private readonly RecipeService _recipeService;
        private readonly PdfService _pdfService;
        private Recipe _selectedRecipe;

        public CostingAnalysisPage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();
            _pdfService = new PdfService();
            LoadRecipes();
        }

        private async void LoadRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            CmbRecipes.ItemsSource = recipes;
        }

        private void CmbRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbRecipes.SelectedItem is Recipe recipe)
            {
                _selectedRecipe = recipe;
                DisplayRecipeCosting(recipe);
            }
        }

        private void DisplayRecipeCosting(Recipe recipe)
        {
            DgCostingIngredients.ItemsSource = recipe.Ingredients;
            
            var wasteAdjustment = recipe.AdjustedCost - recipe.RecipeCost;
            var gstAmount = recipe.GstAmount;
            
            TxtRecipeCostDisplay.Text = $"Recipe Cost: {recipe.RecipeCost:C2}";
            TxtWasteDisplay.Text = $"Waste Adjustment: {wasteAdjustment:C2}";
            TxtGstDisplay.Text = $"GST ({recipe.GstPercentage}%): {gstAmount:C2}";
            TxtPackagingDisplay.Text = $"Packaging: {recipe.PackagingCharges:C2}";
            TxtDeliveryDisplay.Text = $"Delivery: {recipe.DeliveryCharges:C2}";
            TxtFinalCostDisplay.Text = $"Final Cost: {recipe.FinalCost:C2}";
            TxtSellingPriceDisplay.Text = $"Selling Price: {recipe.SellingPrice:C2}";
            TxtProfitDisplay.Text = $"Profit: {recipe.Profit:C2}";
            TxtMarginDisplay.Text = $"Margin: {recipe.GrossMargin:F1}%";
        }

        private void BtnGeneratePdf_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe first.", "No Recipe Selected");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"{_selectedRecipe.Name}_CostingReport.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    _pdfService.GenerateRecipeCard(_selectedRecipe, saveDialog.FileName);
                    MessageBox.Show("PDF report generated successfully!", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating PDF: {ex.Message}", "Error");
                }
            }
        }
    }
}