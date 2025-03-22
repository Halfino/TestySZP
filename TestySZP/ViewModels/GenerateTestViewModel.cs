using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Models;
using TestySZP.Services;
using TestySZP.Helpers;
using Microsoft.Win32;

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

        public GenerateTestViewModel()
        {
            People = new ObservableCollection<Person>(PersonRepository.GetAll());
            GenerateTestCommand = new RelayCommand(param => GenerateTest(), param => CanGenerate);
        }

        private void UpdateCanGenerate()
        {
            bool valid = SelectedPerson != null && int.TryParse(QuestionCount, out int n) && n > 0;
            CanGenerate = valid;
        }

        private void GenerateTest()
        {
            if (!int.TryParse(QuestionCount, out int count) || SelectedPerson == null)
                return;

            var questions = _testService.GenerateTestForPerson(SelectedPerson, count);

            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "PDF dokument|*.pdf",
                FileName = $"Test_{SelectedPerson.Name.Replace(" ", "_")}.pdf"
            };

            if (dlg.ShowDialog() == true)
            {
                PDFGenerator.GenerateTestPDF(SelectedPerson, questions, dlg.FileName);
            }
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
