using System.Windows;
using TestySZP.Models;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    public partial class TestHistoryWindow : Window
    {
        public TestHistoryWindow(Person person)
        {
            InitializeComponent();
            DataContext = new TestHistoryViewModel(person);
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
