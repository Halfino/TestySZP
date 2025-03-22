using System.Windows;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    public partial class GenerateTestWindow : Window
    {
        public GenerateTestWindow()
        {
            InitializeComponent();
            DataContext = new GenerateTestViewModel();
        }
    }
}
