using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestySZP.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsWritten { get; set; }
        public int KnowledgeClass { get; set; }
        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>(); // ✅ Opraveno
    }
}
