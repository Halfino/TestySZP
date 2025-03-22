using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestySZP.Models
{
    public class Question : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(nameof(Text)); }
        }

        private bool _isWritten;
        public bool IsWritten
        {
            get => _isWritten;
            set { _isWritten = value; OnPropertyChanged(nameof(IsWritten)); }
        }

        private int _knowledgeClass;
        public int KnowledgeClass
        {
            get => _knowledgeClass;
            set { _knowledgeClass = value; OnPropertyChanged(nameof(KnowledgeClass)); }
        }

        private int _answerCount;
        public int AnswerCount
        {
            get => _answerCount;
            set
            {
                _answerCount = value;
                OnPropertyChanged(nameof(AnswerCount));
            }
        }


        private ObservableCollection<Answer> _answers = new();
        public ObservableCollection<Answer> Answers
        {
            get => _answers;
            set { _answers = value; OnPropertyChanged(nameof(Answers)); }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
