using System.ComponentModel;

namespace RecipeCostingApp.Models
{
    public class RecipeIngredient : INotifyPropertyChanged
    {
        private int _id;
        private int _recipeId;
        private int _ingredientId;
        private decimal _quantity;
        private Ingredient _ingredient;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public int RecipeId
        {
            get => _recipeId;
            set { _recipeId = value; OnPropertyChanged(nameof(RecipeId)); }
        }

        public int IngredientId
        {
            get => _ingredientId;
            set { _ingredientId = value; OnPropertyChanged(nameof(IngredientId)); }
        }

        public decimal Quantity
        {
            get => _quantity;
            set 
            { 
                _quantity = value; 
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        public Ingredient Ingredient
        {
            get => _ingredient;
            set 
            { 
                _ingredient = value; 
                OnPropertyChanged(nameof(Ingredient));
                OnPropertyChanged(nameof(TotalCost));
            }
        }

        public decimal TotalCost
        {
            get
            {
                if (Ingredient == null) return 0;
                var baseCost = Ingredient.UnitCost * Quantity;
                return Ingredient.WastePercentage > 0 ? baseCost / (1 - Ingredient.WastePercentage / 100) : baseCost;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}