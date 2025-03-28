using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Helpers;
using TestySZP.Models;
using TestySZP.Views;

namespace TestySZP.ViewModels
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<Question> _questions = new();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                OnPropertyChanged(nameof(Questions));
            }
        }

        public ObservableCollection<int> KnowledgeLevels { get; set; }

        public ObservableCollection<Question> AllQuestions { get; set; } = new();

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterQuestions();
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand OpenAnswerWindowCommand { get; }
        public ICommand SaveQuestionCommand { get; }

        private Question _newQuestion;
        public Question NewQuestion
        {
            get => _newQuestion;
            set
            {
                _newQuestion = value;
                OnPropertyChanged(nameof(NewQuestion));
                OnPropertyChanged(nameof(IsWritten));
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                OnPropertyChanged(nameof(SelectedQuestion));
                OnPropertyChanged(nameof(IsQuestionSelected));
                OnPropertyChanged(nameof(IsWritten));

                (SaveQuestionCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteQuestionCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (OpenAnswerWindowCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsQuestionSelected => SelectedQuestion != null;

        public bool IsWritten
        {
            get
            {
                if (SelectedQuestion != null) return SelectedQuestion.IsWritten;
                return NewQuestion?.IsWritten ?? false;
            }
            set
            {
                if (SelectedQuestion != null)
                    SelectedQuestion.IsWritten = value;
                else if (NewQuestion != null)
                    NewQuestion.IsWritten = value;

                OnPropertyChanged(nameof(IsWritten));
            }
        }

        public QuestionViewModel()
        {
            KnowledgeLevels = new ObservableCollection<int> { 1, 2, 3 };

            AllQuestions = new ObservableCollection<Question>(QuestionRepository.GetAllQuestions());
            Questions = new ObservableCollection<Question>(AllQuestions);

            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            DeleteQuestionCommand = new RelayCommand(param => DeleteQuestion(), param => SelectedQuestion != null);
            OpenAnswerWindowCommand = new RelayCommand(param => OpenAnswerWindow(), param => SelectedQuestion != null);
            SaveQuestionCommand = new RelayCommand(param => SaveSelectedQuestion(), param => SelectedQuestion != null);

            ResetNewQuestion();
        }

        private void AddQuestion()
        {
            QuestionRepository.AddQuestion(NewQuestion);
            AllQuestions.Add(NewQuestion);
            FilterQuestions();
            ResetNewQuestion();
        }

        private void DeleteQuestion()
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.DeleteQuestion(SelectedQuestion.Id);
                AllQuestions.Remove(SelectedQuestion);
                FilterQuestions();
                SelectedQuestion = null;
                OnPropertyChanged(nameof(IsWritten));
            }
        }

        private void OpenAnswerWindow()
        {
            if (SelectedQuestion == null)
                return;

            var window = new AnswerWindow(SelectedQuestion);
            window.ShowDialog();

            var updated = QuestionRepository.GetQuestionById(SelectedQuestion.Id);
            if (updated != null)
                ReplaceQuestionInList(updated);
        }

        private void ReplaceQuestionInList(Question updated)
        {
            int index = AllQuestions.IndexOf(AllQuestions.First(q => q.Id == updated.Id));
            if (index >= 0)
                AllQuestions[index] = updated;

            FilterQuestions();
            SelectedQuestion = Questions.FirstOrDefault(q => q.Id == updated.Id);
        }

        private void SaveSelectedQuestion()
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.UpdateQuestion(SelectedQuestion);
                ReplaceQuestionInList(SelectedQuestion);
            }
        }

        private void ResetNewQuestion()
        {
            NewQuestion = new Question
            {
                Text = "",
                IsWritten = true,
                KnowledgeClass = 3,
                Answers = new ObservableCollection<Answer>()
            };
        }

        private void FilterQuestions()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? AllQuestions
                : new ObservableCollection<Question>(
                    AllQuestions.Where(q => q.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            Questions = new ObservableCollection<Question>(filtered);
        }

        private bool _sortAscending = true;

        public void SortQuestionsByClass()
        {
            var sorted = _sortAscending
                ? Questions.OrderBy(q => q.KnowledgeClass)
                : Questions.OrderByDescending(q => q.KnowledgeClass);

            Questions = new ObservableCollection<Question>(sorted);
            _sortAscending = !_sortAscending;
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
