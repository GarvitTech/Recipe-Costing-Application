using System;
using System.Windows;
using System.Windows.Media;
using RecipeCostingApp.Views;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class MainWindow : Window
    {
        private bool _isDarkMode = false;
        
        public MainWindow()
        {
            InitializeComponent();
            LoadDefaultPage();
        }

        private void LoadDefaultPage()
        {
            MainFrame.Navigate(new ItemMasterPage());
            StatusText.Text = "Item Master loaded";
        }

        private void BtnItemMaster_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ItemMasterPage());
            StatusText.Text = "Item Master loaded";
        }

        private void BtnRecipes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RecipePage());
            StatusText.Text = "Recipes loaded";
        }

        private void BtnCosting_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CostingAnalysisPage());
            StatusText.Text = "Costing Analysis loaded";
        }

        private void BtnBulkCosting_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BulkCostingPage());
            StatusText.Text = "Bulk Costing loaded";
        }

        private void BtnMenuEngineering_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MenuEngineeringPage());
            StatusText.Text = "Menu Engineering loaded";
        }

        private async void BtnBackup_Click(object sender, RoutedEventArgs e)
        {
            var backupService = new BackupService();
            var success = await backupService.CreateBackupAsync();
            
            if (success)
            {
                MessageBox.Show("Backup created successfully!", "Backup Complete", 
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Backup operation was cancelled or failed.", "Backup Failed", 
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }
        
        private void BtnThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            
            if (_isDarkMode)
            {
                ApplyDarkTheme();
                BtnThemeToggle.Content = "☀️ Light Mode";
            }
            else
            {
                ApplyLightTheme();
                BtnThemeToggle.Content = "🌙 Dark Mode";
            }
        }
        
        private void ApplyDarkTheme()
        {
            var darkResources = new ResourceDictionary();
            darkResources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(32, 32, 32));
            darkResources["SurfaceBrush"] = new SolidColorBrush(Color.FromRgb(48, 48, 48));
            darkResources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            darkResources["TextBrush"] = new SolidColorBrush(Colors.White);
            darkResources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0, 120, 215));
            darkResources["CalculatedBrush"] = new SolidColorBrush(Color.FromRgb(64, 64, 64));
            
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(darkResources);
        }
        
        private void ApplyLightTheme()
        {
            var lightResources = new ResourceDictionary();
            lightResources["BackgroundBrush"] = new SolidColorBrush(Color.FromRgb(248, 249, 250));
            lightResources["SurfaceBrush"] = new SolidColorBrush(Colors.White);
            lightResources["BorderBrush"] = new SolidColorBrush(Color.FromRgb(222, 226, 230));
            lightResources["TextBrush"] = new SolidColorBrush(Color.FromRgb(33, 37, 41));
            lightResources["PrimaryBrush"] = new SolidColorBrush(Color.FromRgb(0, 123, 255));
            lightResources["CalculatedBrush"] = new SolidColorBrush(Color.FromRgb(233, 236, 239));
            
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(lightResources);
        }
    }
}