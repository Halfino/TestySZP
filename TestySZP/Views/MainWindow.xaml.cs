using System.Windows;
using TestySZP.Views;
using TestySZP.Models;
using TestySZP.Services;
using TestySZP.ViewModels;

namespace TestySZP.Views
{
    public partial class MainWindow : Window
    {
        private TestGenerationService _testService = new TestGenerationService();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); // ✅ Nastavení DataContext
        }

        // Otevře správu osob jako nové okno
        private void OpenPersonWindow(object sender, RoutedEventArgs e)
        {
            PersonWindow personWindow = new PersonWindow();
            personWindow.ShowDialog(); // Blokující okno (modal)
        }

        // Generování testu
        private void OpenGenerateTestWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenerateTestWindow();
            window.ShowDialog();
        }


        // Ukončení aplikace
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
