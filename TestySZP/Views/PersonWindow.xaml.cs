using System.Windows;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    public partial class PersonWindow : Window
    {
        public PersonWindow()
        {
            InitializeComponent();
            DataContext = new PersonViewModel();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
