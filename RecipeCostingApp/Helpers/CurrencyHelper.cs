using System;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Helpers
{
    public static class CurrencyHelper
    {
        public static string FormatCurrency(decimal amount)
        {
            var symbol = SettingsService.Instance.Settings.CurrencySymbol;
            return $"{symbol}{amount:F2}";
        }

        public static string FormatCurrency(double amount)
        {
            return FormatCurrency((decimal)amount);
        }

        public static string GetCurrencySymbol()
        {
            return SettingsService.Instance.Settings.CurrencySymbol;
        }
    }
}