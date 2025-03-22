using System;
using System.Collections.Generic;
using System.Linq;
using TestySZP.Data.Repositories;
using TestySZP.Models;

namespace TestySZP.Services
{
    public class TestGenerationService
    {
        public List<Question> GenerateTestForPerson(Person person, int totalQuestions)
        {
            var selectedQuestions = new List<Question>();
            var random = new Random();

            int requiredFromMainClass = (int)Math.Ceiling(totalQuestions * 0.2);

            // 1) Vyber otázky z hlavní třídy
            var mainClassQuestions = QuestionRepository.GetQuestionsForClass(person.KnowledgeClass)
                                                       .OrderBy(q => random.Next()).ToList();

            foreach (var q in mainClassQuestions)
            {
                if (selectedQuestions.Count >= requiredFromMainClass)
                    break;

                TryAddUnique(selectedQuestions, q);
            }

            // 2) Vyber otázky z nižších tříd
            var lowerQuestions = QuestionRepository.GetLowerClassQuestions(person.KnowledgeClass)
                                                   .OrderBy(q => random.Next()).ToList();

            foreach (var q in lowerQuestions)
            {
                if (selectedQuestions.Count >= totalQuestions)
                    break;

                TryAddUnique(selectedQuestions, q);
            }

            // 3) Znovu zkus použít z hlavní třídy, pokud nebyly dříve použity všechny
            foreach (var q in mainClassQuestions)
            {
                if (selectedQuestions.Count >= totalQuestions)
                    break;

                TryAddUnique(selectedQuestions, q);
            }

            // 4) Pokud stále nemáme dost, použij mix ze všech dostupných tříd ≤ dosažená
            if (selectedQuestions.Count < totalQuestions)
            {
                var additional = QuestionRepository
                    .GetAdditionalQuestionsToFill(1000, person.KnowledgeClass)
                    .OrderBy(q => random.Next());

                foreach (var q in additional)
                {
                    if (selectedQuestions.Count >= totalQuestions)
                        break;

                    TryAddUnique(selectedQuestions, q);
                }
            }

            return selectedQuestions;
        }

        private bool TryAddUnique(List<Question> list, Question question)
        {
            if (!list.Any(q => q.Id == question.Id))
            {
                list.Add(question);
                return true;
            }
            return false;
        }
    }
}
