using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using RecipeCostingApp.Helpers;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Models
{
    public class Recipe : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _category;
        private decimal _wastePercentage;
        private decimal _gstPercentage;
        private decimal _packagingCharges;
        private decimal _deliveryCharges;
        private decimal _sellingPrice;
        private int _portions = 1;
        private int _parentRecipeId;
        private bool _isSubRecipe;

        public Recipe()
        {
            SettingsService.Instance.CurrencyChanged += (s, e) => 
            {
                OnPropertyChanged(nameof(FormattedRecipeCost));
                OnPropertyChanged(nameof(FormattedFinalCost));
                OnPropertyChanged(nameof(FormattedSellingPrice));
                OnPropertyChanged(nameof(FormattedProfit));
            };
        }

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public decimal WastePercentage
        {
            get => _wastePercentage;
            set 
            { 
                _wastePercentage = value; 
                OnPropertyChanged(nameof(WastePercentage));
                RecalculateCosts();
            }
        }

        public decimal GstPercentage
        {
            get => _gstPercentage;
            set 
            { 
                _gstPercentage = value; 
                OnPropertyChanged(nameof(GstPercentage));
                RecalculateCosts();
            }
        }

        public decimal PackagingCharges
        {
            get => _packagingCharges;
            set 
            { 
                _packagingCharges = value; 
                OnPropertyChanged(nameof(PackagingCharges));
                RecalculateCosts();
            }
        }

        public decimal DeliveryCharges
        {
            get => _deliveryCharges;
            set 
            { 
                _deliveryCharges = value; 
                OnPropertyChanged(nameof(DeliveryCharges));
                RecalculateCosts();
            }
        }

        public decimal SellingPrice
        {
            get => _sellingPrice;
            set 
            { 
                _sellingPrice = value; 
                OnPropertyChanged(nameof(SellingPrice));
                OnPropertyChanged(nameof(Profit));
                OnPropertyChanged(nameof(CostPercentage));
                OnPropertyChanged(nameof(GrossMargin));
            }
        }

        public int Portions
        {
            get => _portions;
            set 
            { 
                _portions = value > 0 ? value : 1; 
                OnPropertyChanged(nameof(Portions));
                OnPropertyChanged(nameof(CostPerPortion));
                RecalculateCosts();
            }
        }

        public int ParentRecipeId
        {
            get => _parentRecipeId;
            set { _parentRecipeId = value; OnPropertyChanged(nameof(ParentRecipeId)); }
        }

        public bool IsSubRecipe
        {
            get => _isSubRecipe;
            set { _isSubRecipe = value; OnPropertyChanged(nameof(IsSubRecipe)); }
        }

        public ObservableCollection<RecipeIngredient> Ingredients { get; set; } = new ObservableCollection<RecipeIngredient>();

        // Calculated Properties
        public decimal TotalWeight => Ingredients.Sum(i => i.Quantity);
        public decimal RecipeCost => Ingredients.Sum(i => i.TotalCost);
        public decimal AdjustedCost => WastePercentage > 0 ? RecipeCost / (1 - WastePercentage / 100) : RecipeCost;
        public decimal GstAmount => AdjustedCost * (GstPercentage / 100);
        public decimal FinalCost => AdjustedCost + GstAmount + PackagingCharges + DeliveryCharges;
        public decimal Profit => SellingPrice - FinalCost;
        public decimal CostPercentage => SellingPrice > 0 ? (FinalCost / SellingPrice) * 100 : 0;
        public decimal GrossMargin => SellingPrice > 0 ? (Profit / SellingPrice) * 100 : 0;
        public decimal CostPerPortion => Portions > 0 ? FinalCost / Portions : FinalCost;

        // Formatted Currency Properties
        public string FormattedRecipeCost => CurrencyHelper.FormatCurrency(RecipeCost);
        public string FormattedFinalCost => CurrencyHelper.FormatCurrency(FinalCost);
        public string FormattedSellingPrice => CurrencyHelper.FormatCurrency(SellingPrice);
        public string FormattedProfit => CurrencyHelper.FormatCurrency(Profit);

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        private void RecalculateCosts()
        {
            OnPropertyChanged(nameof(AdjustedCost));
            OnPropertyChanged(nameof(GstAmount));
            OnPropertyChanged(nameof(FinalCost));
            OnPropertyChanged(nameof(Profit));
            OnPropertyChanged(nameof(CostPercentage));
            OnPropertyChanged(nameof(GrossMargin));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}