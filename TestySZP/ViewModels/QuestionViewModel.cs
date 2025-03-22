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

        public ObservableCollection<Question> Questions { get; set; }
        public ObservableCollection<int> KnowledgeLevels { get; set; }

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
                if (_selectedQuestion != null)
                    _selectedQuestion.PropertyChanged -= SelectedQuestion_PropertyChanged;

                _selectedQuestion = value;
                OnPropertyChanged(nameof(SelectedQuestion));
                OnPropertyChanged(nameof(IsQuestionSelected));
                OnPropertyChanged(nameof(IsWritten));

                if (_selectedQuestion != null)
                    _selectedQuestion.PropertyChanged += SelectedQuestion_PropertyChanged;
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

        public ICommand AddQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand OpenAnswerWindowCommand { get; }

        public QuestionViewModel()
        {
            Questions = new ObservableCollection<Question>(QuestionRepository.GetAllQuestions());
            KnowledgeLevels = new ObservableCollection<int> { 1, 2, 3 };

            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            DeleteQuestionCommand = new RelayCommand(param => DeleteQuestion(), param => SelectedQuestion != null);
            OpenAnswerWindowCommand = new RelayCommand(param => OpenAnswerWindow(), param => SelectedQuestion != null);

            ResetNewQuestion();
        }

        private void AddQuestion()
        {
            QuestionRepository.AddQuestion(NewQuestion);
            Questions.Add(NewQuestion);
            ResetNewQuestion();
        }

        private void DeleteQuestion()
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.DeleteQuestion(SelectedQuestion.Id);
                Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                OnPropertyChanged(nameof(IsWritten));
            }
        }

        private void OpenAnswerWindow()
        {
            var window = new AnswerWindow(SelectedQuestion);
            window.ShowDialog();

            // Aktualizace otázky po zavření okna
            var updated = QuestionRepository.GetQuestionById(SelectedQuestion.Id);
            if (updated != null)
            {
                var index = Questions.IndexOf(SelectedQuestion);
                Questions[index] = updated;
                SelectedQuestion = updated;
            }
        }

        private void SelectedQuestion_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.UpdateQuestion(SelectedQuestion);
                RefreshQuestionList(SelectedQuestion.Id);
            }
        }

        private void RefreshQuestionList(int idToRestore)
        {
            var allQuestions = QuestionRepository.GetAllQuestions();
            Questions.Clear();
            foreach (var q in allQuestions)
                Questions.Add(q);

            SelectedQuestion = Questions.FirstOrDefault(q => q.Id == idToRestore);
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

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
