using System.ComponentModel;
using System.Diagnostics;

namespace TestySZP.Models
{
    public class Answer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private int _questionId;
        public int QuestionId
        {
            get => _questionId;
            set { _questionId = value; OnPropertyChanged(nameof(QuestionId)); }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; Debug.WriteLine($"Text changed: {_text}"); OnPropertyChanged(nameof(Text)); }
        }

        private bool _isCorrect;
        public bool IsCorrect
        {
            get => _isCorrect;
            set { _isCorrect = value; OnPropertyChanged(nameof(IsCorrect)); }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
