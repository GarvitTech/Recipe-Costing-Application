using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using RecipeCostingApp.Models;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class RecipePage : Page
    {
        private readonly RecipeService _recipeService;
        private readonly IngredientService _ingredientService;
        private Recipe _currentRecipe;
        private ObservableCollection<RecipeIngredient> _recipeIngredients;

        public RecipePage()
        {
            InitializeComponent();
            _recipeService = new RecipeService();
            _ingredientService = new IngredientService();
            _recipeIngredients = new ObservableCollection<RecipeIngredient>();
            LoadData();
            SetupEventHandlers();
        }

        private async void LoadData()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            CmbIngredients.ItemsSource = ingredients;
            DgRecipeIngredients.ItemsSource = _recipeIngredients;
        }

        private void SetupEventHandlers()
        {
            TxtGst.TextChanged += UpdateCalculations;
            TxtPackaging.TextChanged += UpdateCalculations;
            TxtDelivery.TextChanged += UpdateCalculations;
            TxtSellingPrice.TextChanged += UpdateCalculations;
            TxtRecipeWaste.TextChanged += UpdateCalculations;
            _recipeIngredients.CollectionChanged += (s, e) => UpdateCalculations(null, null);
        }

        private void BtnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (CmbIngredients.SelectedItem is Ingredient ingredient && 
                decimal.TryParse(TxtQuantity.Text, out decimal quantity) && quantity > 0)
            {
                var recipeIngredient = new RecipeIngredient
                {
                    IngredientId = ingredient.Id,
                    Ingredient = ingredient,
                    Quantity = quantity
                };
                
                _recipeIngredients.Add(recipeIngredient);
                TxtQuantity.Text = "";
                CmbIngredients.SelectedIndex = -1;
            }
        }

        private void BtnRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is RecipeIngredient ingredient)
            {
                _recipeIngredients.Remove(ingredient);
            }
        }

        private async void BtnSaveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtRecipeName.Text))
            {
                MessageBox.Show("Please enter recipe name.", "Validation Error");
                return;
            }

            var recipe = _currentRecipe ?? new Recipe();
            recipe.Name = TxtRecipeName.Text;
            recipe.Category = CmbRecipeCategory.Text;
            recipe.WastePercentage = decimal.TryParse(TxtRecipeWaste.Text, out var waste) ? waste : 0;
            recipe.GstPercentage = decimal.TryParse(TxtGst.Text, out var gst) ? gst : 0;
            recipe.PackagingCharges = decimal.TryParse(TxtPackaging.Text, out var pkg) ? pkg : 0;
            recipe.DeliveryCharges = decimal.TryParse(TxtDelivery.Text, out var del) ? del : 0;
            recipe.SellingPrice = decimal.TryParse(TxtSellingPrice.Text, out var sell) ? sell : 0;
            
            recipe.Ingredients.Clear();
            foreach (var ingredient in _recipeIngredients)
            {
                recipe.Ingredients.Add(ingredient);
            }

            await _recipeService.SaveRecipeAsync(recipe);
            MessageBox.Show("Recipe saved successfully!", "Success");
        }

        private void BtnClearRecipe_Click(object sender, RoutedEventArgs e)
        {
            _currentRecipe = null;
            TxtRecipeName.Text = "";
            CmbRecipeCategory.SelectedIndex = -1;
            TxtRecipeWaste.Text = "0";
            TxtGst.Text = "18";
            TxtPackaging.Text = "0";
            TxtDelivery.Text = "0";
            TxtSellingPrice.Text = "";
            _recipeIngredients.Clear();
        }

        private void UpdateCalculations(object sender, TextChangedEventArgs e)
        {
            var totalWeight = _recipeIngredients.Sum(i => i.Quantity);
            var recipeCost = _recipeIngredients.Sum(i => i.TotalCost);
            
            var wastePercentage = decimal.TryParse(TxtRecipeWaste.Text, out var waste) ? waste : 0;
            var adjustedCost = wastePercentage > 0 ? recipeCost / (1 - wastePercentage / 100) : recipeCost;
            
            var gstPercentage = decimal.TryParse(TxtGst.Text, out var gst) ? gst : 0;
            var packaging = decimal.TryParse(TxtPackaging.Text, out var pkg) ? pkg : 0;
            var delivery = decimal.TryParse(TxtDelivery.Text, out var del) ? del : 0;
            var sellingPrice = decimal.TryParse(TxtSellingPrice.Text, out var sell) ? sell : 0;
            
            var gstAmount = adjustedCost * (gstPercentage / 100);
            var finalCost = adjustedCost + gstAmount + packaging + delivery;
            var profit = sellingPrice - finalCost;
            var costPercentage = sellingPrice > 0 ? (finalCost / sellingPrice) * 100 : 0;
            var grossMargin = sellingPrice > 0 ? (profit / sellingPrice) * 100 : 0;
            
            TxtTotalWeight.Text = $"Total Weight: {totalWeight:F1}g";
            TxtRecipeCost.Text = $"Recipe Cost: {recipeCost:C2}";
            TxtFinalCost.Text = $"Final Cost: {finalCost:C2}";
            TxtProfit.Text = $"Profit: {profit:C2}";
            TxtCostPercentage.Text = $"Cost %: {costPercentage:F1}%";
            TxtGrossMargin.Text = $"Margin %: {grossMargin:F1}%";
        }
    }
}