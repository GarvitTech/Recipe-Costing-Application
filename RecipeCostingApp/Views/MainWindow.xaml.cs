using System;
using System.Windows;
using RecipeCostingApp.Views;
using RecipeCostingApp.Services;

namespace RecipeCostingApp.Views
{
    public partial class MainWindow : Window
    {
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
    }
}