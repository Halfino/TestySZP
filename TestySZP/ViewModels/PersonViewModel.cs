using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Models;
using TestySZP.Helpers;

namespace TestySZP.ViewModels
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<int> KnowledgeLevels { get; set; }

        private Person _selectedPerson;
        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged(nameof(SelectedPerson));
            }
        }

        public ICommand AddPersonCommand { get; }
        public ICommand UpdatePersonCommand { get; }
        public ICommand DeletePersonCommand { get; }

        public PersonViewModel()
        {
            Persons = new ObservableCollection<Person>(PersonRepository.GetAllPersons());
            KnowledgeLevels = new ObservableCollection<int> { 1, 2, 3 };

            // Inicializace SelectedPerson s prázdnými hodnotami
            SelectedPerson = new Person
            {
                Name = "",
                KnowledgeClass = 3,
                ValidUntil = DateTime.Today
            };

            AddPersonCommand = new RelayCommand(param => AddPerson());
            UpdatePersonCommand = new RelayCommand(param => UpdatePerson(), param => SelectedPerson != null);
            DeletePersonCommand = new RelayCommand(param => DeletePerson(), param => SelectedPerson != null);
        }

        private void AddPerson()
        {
            if (SelectedPerson == null || string.IsNullOrWhiteSpace(SelectedPerson.Name))
            {
                MessageBox.Show("Zadejte platné jméno!", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PersonRepository.AddPerson(SelectedPerson);
            Persons.Add(new Person
            {
                Id = SelectedPerson.Id,  // Přidáme nově vytvořené ID
                Name = SelectedPerson.Name,
                KnowledgeClass = SelectedPerson.KnowledgeClass,
                ValidUntil = SelectedPerson.ValidUntil
            });

            // ✅ Reset SelectedPerson pro další zadání a aktualizujeme UI
            SelectedPerson = new Person
            {
                Name = "",
                KnowledgeClass = 3,
                ValidUntil = DateTime.Today
            };

            OnPropertyChanged(nameof(SelectedPerson)); // Aktualizace UI
        }

        private void UpdatePerson()
        {
            if (SelectedPerson != null)
            {
                PersonRepository.UpdatePerson(SelectedPerson);
                OnPropertyChanged(nameof(Persons)); // Aktualizace UI
            }
        }

        private void DeletePerson()
        {
            if (SelectedPerson != null)
            {
                var result = MessageBox.Show($"Opravdu chcete smazat osobu {SelectedPerson.Name}?", "Potvrzení", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    PersonRepository.DeletePerson(SelectedPerson.Id);
                    Persons.Remove(SelectedPerson);

                    // ✅ Automatický výběr první osoby nebo vytvoření nového prázdného záznamu
                    if (Persons.Count > 0)
                    {
                        SelectedPerson = Persons[0];
                    }
                    else
                    {
                        SelectedPerson = new Person
                        {
                            Name = "",
                            KnowledgeClass = 3,
                            ValidUntil = DateTime.Today
                        };
                    }

                    OnPropertyChanged(nameof(SelectedPerson)); // Aktualizace UI
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
