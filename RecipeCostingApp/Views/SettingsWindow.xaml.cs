using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly BackupService _backupService;
        private readonly SettingsService _settingsService;

        public SettingsWindow()
        {
            InitializeComponent();
            _backupService = new BackupService();
            _settingsService = SettingsService.Instance;
            LoadSettings();
            LoadSystemInfo();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.Settings;
            TxtDefaultGst.Text = settings.DefaultGstPercentage.ToString();
            TxtCurrencySymbol.Text = settings.CurrencySymbol;
            TxtCompanyName.Text = settings.CompanyName;
            ChkAutoBackup.IsChecked = settings.AutoBackup;
            ChkShowTooltips.IsChecked = settings.ShowTooltips;

            // Show database path
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RecipeCostingApp"
            );
            TxtDatabasePath.Text = dbPath;

            LoadDatabaseStats();
        }

        private async void LoadDatabaseStats()
        {
            try
            {
                var ingredientService = new IngredientService();
                var recipeService = new RecipeService();

                var ingredients = await ingredientService.GetAllIngredientsAsync();
                var recipes = await recipeService.GetAllRecipesAsync();

                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RecipeCostingApp",
                    "RecipeCosting.db"
                );

                var dbSize = File.Exists(dbPath) ? new FileInfo(dbPath).Length / 1024 : 0; // KB

                TxtDbStats.Text = $"Ingredients: {ingredients.Count}\n" +
                                 $"Recipes: {recipes.Count}\n" +
                                 $"Database Size: {dbSize} KB\n" +
                                 $"Last Modified: {(File.Exists(dbPath) ? File.GetLastWriteTime(dbPath).ToString("yyyy-MM-dd HH:mm") : "N/A")}";
            }
            catch (Exception ex)
            {
                TxtDbStats.Text = $"Error loading stats: {ex.Message}";
            }
        }

        private void LoadSystemInfo()
        {
            try
            {
                var osVersion = Environment.OSVersion.ToString();
                var dotnetVersion = Environment.Version.ToString();
                var machineName = Environment.MachineName;
                var userName = Environment.UserName;

                TxtSystemInfo.Text = $"OS: {osVersion}\n" +
                                    $".NET Version: {dotnetVersion}\n" +
                                    $"Machine: {machineName}\n" +
                                    $"User: {userName}";
            }
            catch (Exception ex)
            {
                TxtSystemInfo.Text = $"Error loading system info: {ex.Message}";
            }
        }

        private void BtnOpenDbFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "RecipeCostingApp"
                );

                if (Directory.Exists(dbPath))
                {
                    Process.Start("explorer.exe", dbPath);
                }
                else
                {
                    MessageBox.Show("Database folder not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var success = await _backupService.CreateBackupAsync();
                if (success)
                {
                    MessageBox.Show("Backup created successfully!", "Backup Complete", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Backup operation was cancelled.", "Backup Cancelled", 
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating backup: {ex.Message}", "Backup Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Restoring a backup will replace all current data. Are you sure you want to continue?",
                    "Confirm Restore",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _backupService.RestoreBackupAsync();
                    if (success)
                    {
                        MessageBox.Show("Backup restored successfully! Please restart the application.", 
                                      "Restore Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDatabaseStats(); // Refresh stats
                    }
                    else
                    {
                        MessageBox.Show("Restore operation was cancelled.", "Restore Cancelled", 
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring backup: {ex.Message}", "Restore Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(TxtDefaultGst.Text, out var gst) || gst < 0 || gst > 100)
                {
                    MessageBox.Show("Please enter a valid GST percentage (0-100).", "Invalid Input", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxtCurrencySymbol.Text))
                {
                    MessageBox.Show("Please enter a currency symbol.", "Invalid Input", 
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var settings = new AppSettings
                {
                    DefaultGstPercentage = gst,
                    CurrencySymbol = TxtCurrencySymbol.Text.Trim(),
                    CompanyName = TxtCompanyName.Text.Trim(),
                    AutoBackup = ChkAutoBackup.IsChecked ?? false,
                    ShowTooltips = ChkShowTooltips.IsChecked ?? true
                };

                await _settingsService.SaveSettingsAsync(settings);
                
                MessageBox.Show("Settings saved successfully! Currency changes will be applied throughout the application.", 
                              "Settings Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Save Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "WARNING: This will permanently delete ALL data including ingredients, recipes, and settings.\n\n" +
                    "This action cannot be undone. Are you absolutely sure you want to continue?",
                    "Clear Database - Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    var confirmResult = MessageBox.Show(
                        "Last chance! This will delete everything. Click YES to proceed.",
                        "Final Confirmation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Stop
                    );

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        await ClearDatabaseAsync();
                        MessageBox.Show("Database cleared successfully! The application will restart.", 
                                      "Database Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        LoadDatabaseStats(); // Refresh stats
                        
                        // Restart application
                        System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing database: {ex.Message}", "Clear Database Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ClearDatabaseAsync()
        {
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(RecipeCostingApp.Data.DatabaseManager.ConnectionString);
            await connection.OpenAsync();

            // Delete all data from tables
            var clearScript = @"
                DELETE FROM RecipeIngredients;
                DELETE FROM Recipes;
                DELETE FROM Ingredients;
                VACUUM;
            ";

            using var command = new Microsoft.Data.Sqlite.SqliteCommand(clearScript, connection);
            await command.ExecuteNonQueryAsync();

            // Also clear settings file
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RecipeCostingApp",
                "settings.json"
            );
            
            if (File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}