using System;
using System.Collections.Generic;

namespace TestySZP.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int PersonId { get; set; }

        public DateTime DateGenerated { get; set; } // Datum vytvoření testu
        public DateTime? DateCompleted { get; set; } // Vyplnění testu (volitelné)

        public int? Score { get; set; }             // Počet správných odpovědí
        public int MaxScore { get; set; }           // Celkový počet otázek

        public string Note { get; set; }            // Poznámka
        public string PdfPath { get; set; }         // Cesta k PDF

        public List<int> QuestionIds { get; set; } = new(); // Seznam ID otázek použitých v testu
    }
}
