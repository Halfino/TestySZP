using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Models;
using TestySZP.Services;
using TestySZP.Helpers;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace TestySZP.ViewModels
{
    public class GenerateTestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TestGenerationService _testService = new TestGenerationService();

        public ObservableCollection<Person> People { get; set; }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
                UpdateCanGenerate();
            }
        }

        private string _testHeader = "SZP";
        public string TestHeader
        {
            get => _testHeader;
            set
            {
                _testHeader = value;
                OnPropertyChanged(nameof(TestHeader));
            }
        }

        private string _questionCount;
        public string QuestionCount
        {
            get => _questionCount;
            set
            {
                _questionCount = value;
                OnPropertyChanged(nameof(QuestionCount));
                UpdateCanGenerate();
            }
        }

        private bool _canGenerate;
        public bool CanGenerate
        {
            get => _canGenerate;
            set
            {
                _canGenerate = value;
                OnPropertyChanged(nameof(CanGenerate));
            }
        }

        public ICommand GenerateTestCommand { get; }
        public ICommand GenerateForAllCommand { get; }

        public GenerateTestViewModel()
        {
            People = new ObservableCollection<Person>(PersonRepository.GetAll());
            GenerateTestCommand = new RelayCommand(param => GenerateTest(), param => CanGenerate);
            GenerateForAllCommand = new RelayCommand(_ => GenerateForAll());
        }

        private void UpdateCanGenerate()
        {
            bool valid = SelectedPerson != null && int.TryParse(QuestionCount, out int n) && n > 0;
            CanGenerate = valid;

            ((RelayCommand)GenerateTestCommand).RaiseCanExecuteChanged();
        }

        private void GenerateTest()
        {
            if (!int.TryParse(QuestionCount, out int count) || SelectedPerson == null)
                return;

            var questions = _testService.GenerateTestForPerson(SelectedPerson, count);

            // 📁 Složka Tests/
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests");
            string monthDir = DateTime.Now.ToString("MM-yyyy");
            string testsDir = Path.Combine(baseDir, monthDir);
            if (!Directory.Exists(testsDir))
                Directory.CreateDirectory(testsDir);

            // 📄 Název souboru
            string safeName = SelectedPerson.Name.Replace(" ", "_");
            string fileName = $"Test_{safeName}_{DateTime.Now:dd.MM.yyyy}.pdf";
            string fullPath = Path.Combine(testsDir, fileName);

            // 📤 Generuj PDF
            PDFGenerator.GenerateTestPDF(SelectedPerson, questions, fullPath, TestHeader);
        }

        private void GenerateForAll()
        {
            if (!int.TryParse(QuestionCount, out int count) || People.Count == 0)
                return;

            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tests");
            string monthDir = DateTime.Now.ToString("MM-yyyy");
            string testsDir = Path.Combine(baseDir, monthDir);

            if (!Directory.Exists(testsDir))
                Directory.CreateDirectory(testsDir);

            foreach (var person in People)
            {
                var questions = _testService.GenerateTestForPerson(person, count);

                string safeName = person.Name.Replace(" ", "_");
                string fileName = $"Test_{safeName}_{DateTime.Now:dd.MM.yyyy}.pdf";
                string fullPath = Path.Combine(testsDir, fileName);

                PDFGenerator.GenerateTestPDF(person, questions, fullPath, TestHeader);
            }

            MessageBox.Show("Testy pro všechny osoby byly úspěšně vygenerovány.", "Hotovo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Zavři okno po dokončení
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is Views.GenerateTestWindow)
                ?.Close();
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
