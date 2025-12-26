using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using RecipeCostingApp.Helpers;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Models
{
    public class Ingredient : INotifyPropertyChanged
    {
        private int _id;
        private string _name;
        private string _category;
        private string _unit;
        private decimal _purchaseUnit;
        private decimal _price;
        private decimal _wastePercentage;
        private decimal _yield;

        public Ingredient()
        {
            SettingsService.Instance.CurrencyChanged += (s, e) => 
            {
                OnPropertyChanged(nameof(FormattedPrice));
                OnPropertyChanged(nameof(FormattedUnitCost));
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

        public string Unit
        {
            get => _unit;
            set { _unit = value; OnPropertyChanged(nameof(Unit)); }
        }

        public decimal PurchaseUnit
        {
            get => _purchaseUnit;
            set { _purchaseUnit = value; OnPropertyChanged(nameof(PurchaseUnit)); }
        }

        public decimal Price
        {
            get => _price;
            set 
            { 
                _price = value; 
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(UnitCost));
            }
        }

        public decimal WastePercentage
        {
            get => _wastePercentage;
            set 
            { 
                _wastePercentage = value; 
                Yield = 100 - value;
                OnPropertyChanged(nameof(WastePercentage));
            }
        }

        public decimal Yield
        {
            get => _yield;
            set { _yield = value; OnPropertyChanged(nameof(Yield)); }
        }

        public decimal UnitCost => PurchaseUnit > 0 ? Price / PurchaseUnit : 0;

        public string FormattedPrice => CurrencyHelper.FormatCurrency(Price);
        public string FormattedUnitCost => CurrencyHelper.FormatCurrency(UnitCost);

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        
        // Additional fields from import
        public Dictionary<string, string> AdditionalFields { get; set; } = new Dictionary<string, string>();
        
        public string AdditionalFieldsIndicator => AdditionalFields.Any() ? "📋" : "";

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}