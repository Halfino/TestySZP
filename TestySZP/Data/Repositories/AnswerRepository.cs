using System.Collections.Generic;
using System.Data.SQLite;
using TestySZP.Models;
using TestySZP.Data;

namespace TestySZP.Data.Repositories
{
    public static class AnswerRepository
    {
        public static List<Answer> GetAnswersForQuestion(int questionId)
        {
            var answers = new List<Answer>();

            using (var connection = DatabaseHelper.GetConnection())
            {

                using (var command = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @qid", connection))
                {
                    command.Parameters.AddWithValue("@qid", questionId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            answers.Add(new Answer
                            {
                                Id = reader.GetInt32(0),
                                QuestionId = reader.GetInt32(1),
                                Text = reader.GetString(2),
                                IsCorrect = reader.GetBoolean(3)
                            });
                        }
                    }
                }
            }

            return answers;
        }

        public static void AddAnswer(Answer answer)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                using (var command = new SQLiteCommand("INSERT INTO Answers (question_id, text, is_correct) VALUES (@qid, @text, @correct)", connection))
                {
                    command.Parameters.AddWithValue("@qid", answer.QuestionId);
                    command.Parameters.AddWithValue("@text", answer.Text);
                    command.Parameters.AddWithValue("@correct", answer.IsCorrect);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateAnswer(Answer answer)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                using (var command = new SQLiteCommand("UPDATE Answers SET text = @text, is_correct = @correct WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@text", answer.Text);
                    command.Parameters.AddWithValue("@correct", answer.IsCorrect);
                    command.Parameters.AddWithValue("@id", answer.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteAnswer(int id)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                using (var command = new SQLiteCommand("DELETE FROM Answers WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
