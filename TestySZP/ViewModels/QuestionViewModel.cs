using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Models;
using TestySZP.Helpers;

namespace TestySZP.ViewModels
{
    public class QuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Question> Questions { get; set; }
        public ObservableCollection<int> KnowledgeLevels { get; set; }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                EnsureAnswersInitialized(_selectedQuestion);
                OnPropertyChanged(nameof(SelectedQuestion));
                OnPropertyChanged(nameof(IsMultipleChoice));
                OnPropertyChanged(nameof(CanAddAnswer));
            }
        }

        private Answer _selectedAnswer;
        public Answer SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                _selectedAnswer = value;
                OnPropertyChanged(nameof(SelectedAnswer));
            }
        }


        public bool IsMultipleChoice
        {
            get => SelectedQuestion?.Type == QuestionType.MultipleChoice;
            set
            {
                if (SelectedQuestion != null)
                {
                    SelectedQuestion.Type = value ? QuestionType.MultipleChoice : QuestionType.Written;
                    if (value && SelectedQuestion.Answers == null)
                    {
                        SelectedQuestion.Answers = new ObservableCollection<Answer>(); // ✅ Při přepnutí na Multiple Choice zajistíme inicializaci
                    }
                    else if (!value)
                    {
                        SelectedQuestion.Answers?.Clear(); // ✅ Při přepnutí na Written smažeme odpovědi
                    }
                    OnPropertyChanged(nameof(IsMultipleChoice));
                    OnPropertyChanged(nameof(SelectedQuestion));
                }
            }
        }

        public bool CanAddAnswer => SelectedQuestion != null && IsMultipleChoice;

        public ICommand AddQuestionCommand { get; }
        public ICommand UpdateQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }
        public ICommand AddAnswerCommand { get; }
        public ICommand DeleteAnswerCommand { get; }

        public QuestionViewModel()
        {
            Questions = new ObservableCollection<Question>(QuestionRepository.GetAllQuestions());
            KnowledgeLevels = new ObservableCollection<int> { 1, 2, 3 };

            if (Questions.Count > 0)
            {
                SelectedQuestion = Questions[0];
                EnsureAnswersInitialized(SelectedQuestion);
            }
            else
            {
                AddQuestion(); // ✅ Pokud nejsou otázky, automaticky vytvoříme novou
            }

            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            UpdateQuestionCommand = new RelayCommand(param => UpdateQuestion(), param => SelectedQuestion != null);
            DeleteQuestionCommand = new RelayCommand(param => DeleteQuestion(), param => SelectedQuestion != null);
            AddAnswerCommand = new RelayCommand(param => AddAnswer(), param => CanAddAnswer);
            DeleteAnswerCommand = new RelayCommand(param => DeleteAnswer(), param => SelectedQuestion != null && SelectedQuestion.Answers.Count > 0);
        }

        private void EnsureAnswersInitialized(Question question)
        {
            if (question != null && question.Answers == null)
            {
                question.Answers = new ObservableCollection<Answer>(); // ✅ Oprava inicializace odpovědí
            }
        }

        private void AddQuestion()
        {
            var newQuestion = new Question
            {
                Text = "Nová otázka",
                Type = QuestionType.MultipleChoice,
                KnowledgeClass = 3,
                Answers = new ObservableCollection<Answer>()
            };

            QuestionRepository.AddQuestion(newQuestion);
            Questions.Add(newQuestion);
            SelectedQuestion = newQuestion;
            OnPropertyChanged(nameof(SelectedQuestion));
        }

        private void UpdateQuestion()
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.UpdateQuestion(SelectedQuestion);
                OnPropertyChanged(nameof(Questions));
            }
        }

        private void DeleteQuestion()
        {
            if (SelectedQuestion != null)
            {
                QuestionRepository.DeleteQuestion(SelectedQuestion.Id);
                Questions.Remove(SelectedQuestion);
                SelectedQuestion = null;
                OnPropertyChanged(nameof(SelectedQuestion));
            }
        }

        private void AddAnswer()
        {
            if (SelectedQuestion != null && IsMultipleChoice)
            {
                if (SelectedQuestion.Answers == null)
                {
                    SelectedQuestion.Answers = new ObservableCollection<Answer>();
                }

                var newAnswer = new Answer { Text = "Nová odpověď", IsCorrect = false };
                SelectedQuestion.Answers.Add(newAnswer);
                SelectedAnswer = newAnswer; // ✅ Automaticky vybereme nově přidanou odpověď
                OnPropertyChanged(nameof(SelectedQuestion.Answers));
            }
        }

        private void DeleteAnswer()
        {
            if (SelectedQuestion != null && SelectedQuestion.Answers.Count > 0)
            {
                SelectedQuestion.Answers.RemoveAt(SelectedQuestion.Answers.Count - 1);
                OnPropertyChanged(nameof(SelectedQuestion));
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
