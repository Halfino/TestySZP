using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Helpers;
using TestySZP.Models;

namespace TestySZP.ViewModels
{
    public class TestHistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string PersonName { get; }
        public ObservableCollection<TestResult> TestResults { get; set; }

        private TestResult _selectedTestResult;
        public TestResult SelectedTestResult
        {
            get => _selectedTestResult;
            set
            {
                _selectedTestResult = value;
                OnPropertyChanged(nameof(SelectedTestResult));

                (OpenPdfCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditResultCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteTestCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand DeleteTestCommand { get; }
        public ICommand EditResultCommand { get; }
        public ICommand OpenPdfCommand { get; }

        private readonly Person _person;

        public TestHistoryViewModel(Person person)
        {
            _person = person;
            PersonName = person.Name;

            var results = TestResultRepository.GetTestResultsForPerson(person.Id);
            TestResults = new ObservableCollection<TestResult>(results);

            DeleteTestCommand = new RelayCommand(_ => DeleteTest(), _ => SelectedTestResult != null);
            EditResultCommand = new RelayCommand(_ => EditResult(), _ => SelectedTestResult != null);
            OpenPdfCommand = new RelayCommand(_ => OpenPdf(), _ => SelectedTestResult != null);
        }

        private void DeleteTest()
        {
            if (SelectedTestResult == null) return;

            if (MessageBox.Show("Opravdu chcete smazat vybraný test?", "Potvrzení", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // 🗑️ Pokus o smazání souboru PDF
                try
                {
                    if (!string.IsNullOrWhiteSpace(SelectedTestResult.PdfPath) && File.Exists(SelectedTestResult.PdfPath))
                    {
                        File.Delete(SelectedTestResult.PdfPath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nepodařilo se smazat PDF soubor:\n{ex.Message}", "Chyba při mazání souboru");
                }

                // Smazání z databáze
                TestResultRepository.DeleteTestResult(SelectedTestResult.Id);
                TestResults.Remove(SelectedTestResult);
            }
        }

        private void EditResult()
        {
            if (SelectedTestResult == null) return;

            var dialog = new Views.EditTestResultWindow(SelectedTestResult);
            if (dialog.ShowDialog() == true)
            {
                TestResultRepository.UpdateTestResult(SelectedTestResult);

                // Refresh UI
                int index = TestResults.IndexOf(SelectedTestResult);
                if (index >= 0)
                {
                    TestResults[index] = SelectedTestResult;
                    OnPropertyChanged(nameof(TestResults));
                }
            }
        }

        private void OpenPdf()
        {
            if (SelectedTestResult == null || string.IsNullOrEmpty(SelectedTestResult.PdfPath))
            {
                MessageBox.Show("Cesta k PDF není dostupná.");
                return;
            }

            if (File.Exists(SelectedTestResult.PdfPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = SelectedTestResult.PdfPath,
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show("Soubor nebyl nalezen.");
            }
        }
    }
}
