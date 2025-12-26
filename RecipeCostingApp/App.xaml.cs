using System.Threading.Tasks;
using System.Windows;
using RecipeCostingApp.Data;

namespace RecipeCostingApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Initialize database
                DatabaseManager.InitializeDatabase();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}", "Startup Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}