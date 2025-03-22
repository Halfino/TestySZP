using TestySZP.Data.Repositories;
using TestySZP.Models;


namespace TestySZP.Services
{
    public class TestGenerationService
    {
        public List<Question> GenerateTestForPerson(Person person, int totalQuestions)
        {
            List<Question> selectedQuestions = new List<Question>();
            Random random = new Random();

            // Počet otázek z dosažené třídy (20 %)
            int requiredQuestionsFromClass = (int)Math.Ceiling(totalQuestions * 0.2);

            // Získání otázek pro dosaženou třídu
            List<Question> questionsForUserClass = QuestionRepository.GetQuestionsForClass(person.KnowledgeClass);
            int availableQuestionsFromClass = questionsForUserClass.Count;

            int missingQuestions = 0;
            if (availableQuestionsFromClass < requiredQuestionsFromClass)
            {
                selectedQuestions.AddRange(questionsForUserClass);
                missingQuestions = requiredQuestionsFromClass - availableQuestionsFromClass;
            }
            else
            {
                selectedQuestions.AddRange(questionsForUserClass.OrderBy(q => random.Next()).Take(requiredQuestionsFromClass));
            }

            // Výběr zbývajících otázek z nižších tříd
            int remainingQuestionsToSelect = (totalQuestions - selectedQuestions.Count) + missingQuestions;
            List<Question> remainingQuestions = QuestionRepository.GetRemainingQuestionsForTest(person.KnowledgeClass, remainingQuestionsToSelect);

            selectedQuestions.AddRange(remainingQuestions);

            // Odstranění duplicitních otázek
            selectedQuestions = selectedQuestions.Distinct().ToList();

            // Doplnění chybějících otázek po odstranění duplicit
            if (selectedQuestions.Count < totalQuestions)
            {
                int finalMissingQuestionsCount = totalQuestions - selectedQuestions.Count;
                List<Question> additionalQuestions = QuestionRepository.GetAdditionalQuestionsToFill(finalMissingQuestionsCount, person.KnowledgeClass);
                selectedQuestions.AddRange(additionalQuestions);
            }

            return selectedQuestions;
        }
    }
}