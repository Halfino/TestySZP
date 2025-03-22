using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TestySZP.Data.Repositories;
using TestySZP.Models;

namespace TestySZP.Services
{
    public class TestGenerationService
    {
        public List<Question> GenerateTestForPerson(Person person, int totalQuestions)
        {
            var allAvailable = QuestionRepository
                .GetAllQuestions()
                .Where(q => q.KnowledgeClass <= person.KnowledgeClass)
                .ToList();

            if (!allAvailable.Any())
                return new List<Question>(); // žádné dostupné otázky

            var random = new Random();

            // Rozdělení otázek podle třídy
            var mainClassQuestions = allAvailable
                .Where(q => q.KnowledgeClass == person.KnowledgeClass)
                .OrderBy(q => random.Next())
                .ToList();

            var lowerClassQuestions = allAvailable
                .Where(q => q.KnowledgeClass < person.KnowledgeClass)
                .OrderBy(q => random.Next())
                .ToList();

            // Výpočet 20 % z hlavní třídy (zaokrouhleno nahoru)
            int minMainCount = (int)Math.Ceiling(totalQuestions * 0.2);

            var selectedQuestions = new List<Question>();

            // Přidej otázky z hlavní třídy
            selectedQuestions.AddRange(mainClassQuestions.Take(minMainCount));

            // Pokud jich není dost, použij všechny a zapamatuj si, kolik chybí
            int missing = minMainCount - mainClassQuestions.Count;
            if (missing > 0)
                Debug.WriteLine($"Chybí {missing} otázek z hlavní třídy, použity všechny dostupné.");

            // Doplnění z nižších tříd
            foreach (var q in lowerClassQuestions)
            {
                if (selectedQuestions.Count >= totalQuestions)
                    break;

                if (!selectedQuestions.Any(x => x.Id == q.Id))
                    selectedQuestions.Add(q);
            }

            // Pokud stále chybí otázky, doplň mixem z celého rozsahu
            if (selectedQuestions.Count < totalQuestions)
            {
                var fillPool = allAvailable.OrderBy(q => random.Next());
                foreach (var q in fillPool)
                {
                    if (selectedQuestions.Count >= totalQuestions)
                        break;

                    if (!selectedQuestions.Any(x => x.Id == q.Id))
                        selectedQuestions.Add(q);
                }
            }

            return selectedQuestions;
        }
    }
}
