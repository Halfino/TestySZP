using System.Windows;
using TestySZP.Models;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    public partial class AnswerWindow : Window
    {
        public AnswerWindow(Question question)
        {
            InitializeComponent();
            DataContext = new AnswerViewModel(question);
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
