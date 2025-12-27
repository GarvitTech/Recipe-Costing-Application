using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecipeCostingApp.Services
{
    public class SettingsService
    {
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RecipeCostingApp",
            "settings.json"
        );

        private static SettingsService _instance;
        private AppSettings _settings;

        public static SettingsService Instance => _instance ??= new SettingsService();

        public AppSettings Settings => _settings ??= LoadSettings();

        public event EventHandler<string> CurrencyChanged;

        private AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch { }
            
            return new AppSettings();
        }

        public async Task SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                var directory = Path.GetDirectoryName(SettingsPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var oldCurrency = _settings?.CurrencySymbol;
                _settings = settings;

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(SettingsPath, json);

                if (oldCurrency != settings.CurrencySymbol)
                    CurrencyChanged?.Invoke(this, settings.CurrencySymbol);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save settings: {ex.Message}");
            }
        }
    }

    public class AppSettings
    {
        public decimal DefaultGstPercentage { get; set; } = 18;
        public string CurrencySymbol { get; set; } = "KWD";
        public string CompanyName { get; set; } = "Your Restaurant Name";
        public bool AutoBackup { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;
    }
}