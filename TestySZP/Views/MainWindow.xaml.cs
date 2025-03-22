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
        private void GenerateTest(object sender, RoutedEventArgs e)
        {
            Person testPerson = new Person { Id = 1, Name = "Jan Novák", KnowledgeClass = 2 };
            var testQuestions = _testService.GenerateTestForPerson(testPerson, 10);

            string message = $"Vygenerován test pro: {testPerson.Name}\n";
            message += $"Počet otázek: {testQuestions.Count}";
            MessageBox.Show(message, "Generování testu", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Ukončení aplikace
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
