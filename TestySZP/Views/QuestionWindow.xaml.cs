using System.Collections.ObjectModel;
using System.Windows;
using TestySZP.Models;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    /// <summary>
    /// Interakční logika pro QuestionWindow.xaml
    /// </summary>
    public partial class QuestionWindow : Window
    {
        public QuestionWindow()
        {
            InitializeComponent();
            DataContext = new QuestionViewModel(); // ✅ Nastavení DataContext
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool _sortAscending = true;

        private void SortByClass_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as QuestionViewModel;
            viewModel?.SortQuestionsByClass();
        }
    }
}
