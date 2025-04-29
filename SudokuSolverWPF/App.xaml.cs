using System.Windows;
using SudokuSolverWPF.Views;

namespace SudokuSolverWPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // ایجاد نمونه از پنجره اصلی
            var mainWindow = new MainWindow();

            // نمایش پنجره
            mainWindow.Show();
        }
    }
}