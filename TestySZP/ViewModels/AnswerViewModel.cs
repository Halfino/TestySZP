using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using TestySZP.Data.Repositories;
using TestySZP.Helpers;
using TestySZP.Models;
using TestySZP.Views;

namespace TestySZP.ViewModels
{
    public class AnswerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Answer> Answers { get; set; }
        public Question Question { get; }
        public ICommand AddAnswerCommand { get; }
        public ICommand DeleteAnswerCommand { get; }
        public ICommand SaveAnswerCommand { get; }

        private Answer _newAnswer;
        public Answer NewAnswer
        {
            get => _newAnswer;
            set
            {
                _newAnswer = value;
                OnPropertyChanged(nameof(NewAnswer));
            }
        }

        private Answer _selectedAnswer;
        public Answer SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != null)
                    _selectedAnswer.PropertyChanged -= SelectedAnswer_PropertyChanged;

                _selectedAnswer = value;
                OnPropertyChanged(nameof(SelectedAnswer));
                OnPropertyChanged(nameof(IsAnswerSelected));

                if (_selectedAnswer != null)
                    _selectedAnswer.PropertyChanged += SelectedAnswer_PropertyChanged;

                // >>> ZAJISTÍ ODEMČENÍ TLAČÍTKA
                if (DeleteAnswerCommand is RelayCommand deleteCmd)
                    deleteCmd.RaiseCanExecuteChanged();

                (SaveAnswerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool IsAnswerSelected => SelectedAnswer != null;

        public AnswerViewModel(Question question)
        {
            Question = question;
            Answers = new ObservableCollection<Answer>(AnswerRepository.GetAnswersForQuestion(question.Id));

            AddAnswerCommand = new RelayCommand(param => AddAnswer());
            DeleteAnswerCommand = new RelayCommand(param => DeleteAnswer(), param => SelectedAnswer != null);
            SaveAnswerCommand = new RelayCommand(param => SaveAnswer(), param => SelectedAnswer != null);

            ResetNewAnswer();
        }

        private void AddAnswer()
        {
            NewAnswer.QuestionId = Question.Id;
            AnswerRepository.AddAnswer(NewAnswer);
            Answers.Add(NewAnswer);
            ResetNewAnswer();
        }

        private void DeleteAnswer()
        {
            if (SelectedAnswer != null)
            {
                AnswerRepository.DeleteAnswer(SelectedAnswer.Id);
                Answers.Remove(SelectedAnswer);
                SelectedAnswer = null;
            }
        }

        private void SelectedAnswer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Answer answer)
            {
                Debug.WriteLine($"Ukládám do DB: {answer.Text}");
                AnswerRepository.UpdateAnswer(answer);
            }
        }

        private void ResetNewAnswer()
        {
            NewAnswer = new Answer
            {
                Text = "",
                IsCorrect = false
            };
        }


        private void SaveAnswer()
        {
            if (SelectedAnswer != null)
            {
                AnswerRepository.UpdateAnswer(SelectedAnswer);
            }
        }

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
