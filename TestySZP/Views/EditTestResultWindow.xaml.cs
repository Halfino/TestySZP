using System.Windows;
using TestySZP.Models;

namespace TestySZP.Views
{
    public partial class EditTestResultWindow : Window
    {
        public EditTestResultWindow(TestResult result)
        {
            InitializeComponent();
            DataContext = result;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
