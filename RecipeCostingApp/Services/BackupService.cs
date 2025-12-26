using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Win32;
using RecipeCostingApp.Data;

namespace RecipeCostingApp.Services
{
    public class BackupService
    {
        public async Task<bool> CreateBackupAsync()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "SQLite Database (*.db)|*.db",
                    FileName = $"RecipeCosting_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var sourcePath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "RecipeCostingApp",
                        "RecipeCosting.db"
                    );

                    if (File.Exists(sourcePath))
                    {
                        await Task.Run(() => File.Copy(sourcePath, saveDialog.FileName, true));
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RestoreBackupAsync()
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "SQLite Database (*.db)|*.db",
                    Title = "Select Backup File to Restore"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var targetPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "RecipeCostingApp",
                        "RecipeCosting.db"
                    );

                    await Task.Run(() => File.Copy(openDialog.FileName, targetPath, true));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}