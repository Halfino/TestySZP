using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestySZP.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public QuestionType Type { get; set; }
        public int KnowledgeClass { get; set; }
        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>(); // ✅ Opraveno
    }

    public enum QuestionType
    {
        MultipleChoice = 1,
        Written = 2
    }
}
