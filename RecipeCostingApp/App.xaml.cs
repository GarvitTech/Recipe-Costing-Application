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
            
            // Initialize database
            DatabaseManager.InitializeDatabase();
        }
    }
}