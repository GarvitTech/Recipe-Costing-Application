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
            
            // Global exception handlers
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            
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
        
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show($"Unexpected error: {ex?.Message}\n\nThe application will close.", 
                          "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
        
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Application error: {e.Exception.Message}\n\nClick OK to continue.", 
                          "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}