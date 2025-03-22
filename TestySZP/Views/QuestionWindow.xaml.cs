using System.Windows;
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
    }
}
