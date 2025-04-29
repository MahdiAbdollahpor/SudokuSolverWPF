using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SudokuSolverWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Allow only numbers 1-9
            if (!char.IsDigit(e.Text, 0) || e.Text == "0")
            {
                e.Handled = true;
            }
        }
    }
}