using System.ComponentModel;

namespace RecipeCostingApp.Models
{
    public class BulkIngredient : INotifyPropertyChanged
    {
        private RecipeIngredient _recipeIngredient;
        private int _multiplier;

        public RecipeIngredient RecipeIngredient
        {
            get => _recipeIngredient;
            set
            {
                _recipeIngredient = value;
                OnPropertyChanged(nameof(RecipeIngredient));
                OnPropertyChanged(nameof(Ingredient));
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(BulkQuantity));
                OnPropertyChanged(nameof(BulkCost));
            }
        }

        public Ingredient Ingredient => RecipeIngredient?.Ingredient;
        public decimal Quantity => RecipeIngredient?.Quantity ?? 0;

        public int Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                OnPropertyChanged(nameof(Multiplier));
                OnPropertyChanged(nameof(BulkQuantity));
                OnPropertyChanged(nameof(BulkCost));
            }
        }

        public decimal BulkQuantity => Quantity * Multiplier;
        public decimal BulkCost => (RecipeIngredient?.TotalCost ?? 0) * Multiplier;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}