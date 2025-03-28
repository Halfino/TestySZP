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

        private void OpenTestHistory_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PersonViewModel viewModel && viewModel.SelectedPerson != null)
            {
                var historyWindow = new TestHistoryWindow(viewModel.SelectedPerson);
                historyWindow.ShowDialog();
            }
        }

    }
}