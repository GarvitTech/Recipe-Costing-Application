using System;
using System.Windows;
using System.Windows.Controls;
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
            this.Background = new SolidColorBrush(Color.FromRgb(32, 32, 32));
            
            // Update all child elements
            UpdateElementColors(this, true);
        }
        
        private void ApplyLightTheme()
        {
            this.Background = new SolidColorBrush(Color.FromRgb(248, 249, 250));
            
            // Update all child elements
            UpdateElementColors(this, false);
        }
        
        private void UpdateElementColors(DependencyObject parent, bool isDark)
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    
                    if (child is Border border)
                    {
                        border.Background = new SolidColorBrush(isDark ? Color.FromRgb(48, 48, 48) : Colors.White);
                        border.BorderBrush = new SolidColorBrush(isDark ? Color.FromRgb(64, 64, 64) : Color.FromRgb(222, 226, 230));
                    }
                    else if (child is TextBlock textBlock)
                    {
                        textBlock.Foreground = new SolidColorBrush(isDark ? Colors.White : Color.FromRgb(33, 37, 41));
                    }
                    else if (child is Label label)
                    {
                        label.Foreground = new SolidColorBrush(isDark ? Colors.White : Color.FromRgb(33, 37, 41));
                    }
                    else if (child is Button button)
                    {
                        button.Background = new SolidColorBrush(isDark ? Color.FromRgb(0, 120, 215) : Color.FromRgb(0, 123, 255));
                        button.Foreground = new SolidColorBrush(Colors.White);
                    }
                    
                    UpdateElementColors(child, isDark);
                }
            }
            catch (Exception ex)
            {
                // Ignore theme update errors to prevent crashes
                System.Diagnostics.Debug.WriteLine($"Theme update error: {ex.Message}");
            }
        }
    }
}