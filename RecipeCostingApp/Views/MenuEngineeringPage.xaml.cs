using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipeCostingApp.Models;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class MenuEngineeringPage : Page
    {
        private readonly RecipeService _recipeService;

        public MenuEngineeringPage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();
            LoadMenuEngineering();
        }

        private async void LoadMenuEngineering()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            DgMenuEngineering.ItemsSource = recipes;
            UpdateStatistics(recipes);
        }

        private void UpdateStatistics(List<Recipe> recipes)
        {
            var highProfit = recipes.Count(r => r.GrossMargin >= 60);
            var mediumProfit = recipes.Count(r => r.GrossMargin >= 30 && r.GrossMargin < 60);
            var lowProfit = recipes.Count(r => r.GrossMargin < 30);
            
            TxtHighProfitCount.Text = $"{highProfit} items (≥60% margin)";
            TxtMediumProfitCount.Text = $"{mediumProfit} items (30-59% margin)";
            TxtLowProfitCount.Text = $"{lowProfit} items (<30% margin)";
            TxtTotalCount.Text = $"{recipes.Count} total items";
        }

        private void BtnRefreshMenu_Click(object sender, RoutedEventArgs e)
        {
            LoadMenuEngineering();
        }

        private async void BtnExportMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx",
                    FileName = $"MenuEngineering_{DateTime.Now:yyyyMMdd}.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var recipes = await _recipeService.GetAllRecipesAsync();
                    var csv = "Recipe,Category,Recipe Cost,Final Cost,Selling Price,Profit,Cost %,Margin %\n";
                    
                    foreach (var recipe in recipes)
                    {
                        csv += $"{recipe.Name},{recipe.Category},{recipe.RecipeCost:F2},{recipe.FinalCost:F2},{recipe.SellingPrice:F2},{recipe.Profit:F2},{recipe.CostPercentage:F1},{recipe.GrossMargin:F1}\n";
                    }
                    
                    System.IO.File.WriteAllText(saveDialog.FileName, csv);
                    MessageBox.Show("Menu engineering data exported successfully!", "Export Complete");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Export Error");
            }
        }
    }
}