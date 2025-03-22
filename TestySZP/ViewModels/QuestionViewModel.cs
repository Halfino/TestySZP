using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            }
        }

        public bool IsQuestionSelected => SelectedQuestion != null;

        public bool IsWritten
        {
            get
            {
                if (SelectedQuestion != null) return SelectedQuestion.Type == QuestionType.Written;
                return NewQuestion?.Type == QuestionType.Written;
            }
            set
            {
                if (SelectedQuestion != null)
                    SelectedQuestion.Type = value ? QuestionType.Written : QuestionType.MultipleChoice;
                else if (NewQuestion != null)
                    NewQuestion.Type = value ? QuestionType.Written : QuestionType.MultipleChoice;

                OnPropertyChanged(nameof(IsWritten));
            }
        }

        public ICommand AddQuestionCommand { get; }
        public ICommand UpdateQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }

        public QuestionViewModel()
        {
            Questions = new ObservableCollection<Question>(QuestionRepository.GetAllQuestions());
            KnowledgeLevels = new ObservableCollection<int> { 1, 2, 3 };

            AddQuestionCommand = new RelayCommand(param => AddQuestion());
            UpdateQuestionCommand = new RelayCommand(param => UpdateQuestion(), param => SelectedQuestion != null);
            DeleteQuestionCommand = new RelayCommand(param => DeleteQuestion(), param => SelectedQuestion != null);

            ResetNewQuestion();
        }

        private void AddQuestion()
        {
            QuestionRepository.AddQuestion(NewQuestion);
            Questions.Add(NewQuestion);
            ResetNewQuestion();
        }

        private void UpdateQuestion()
        {
            if (SelectedQuestion == null)
                return;

            // Uložíme změnu do DB
            QuestionRepository.UpdateQuestion(SelectedQuestion);

            // Najdeme otázku v kolekci a ručně přepíšeme její hodnoty
            var updatedFromDb = QuestionRepository.GetById(SelectedQuestion.Id);
            var questionInList = Questions.FirstOrDefault(q => q.Id == SelectedQuestion.Id);

            if (questionInList != null && updatedFromDb != null)
            {
                questionInList.Text = updatedFromDb.Text;
                questionInList.Type = updatedFromDb.Type;
                questionInList.KnowledgeClass = updatedFromDb.KnowledgeClass;

                // Oznámíme změnu UI
                OnPropertyChanged(nameof(Questions));
                SelectedQuestion = questionInList;
            }
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

        private void ResetNewQuestion()
        {
            NewQuestion = new Question
            {
                Text = "",
                Type = QuestionType.Written,
                KnowledgeClass = 3,
                Answers = new ObservableCollection<Answer>()
            };
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
