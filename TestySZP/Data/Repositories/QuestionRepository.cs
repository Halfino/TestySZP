using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using TestySZP.Models;

namespace TestySZP.Data.Repositories
{
    public static class QuestionRepository
    {
        public static void AddQuestion(Question question)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string insertQuestion = "INSERT INTO Questions (text, type, knowledge_class) VALUES (@text, @type, @knowledge_class); SELECT last_insert_rowid();";
                using (var cmd = new SQLiteCommand(insertQuestion, connection))
                {
                    cmd.Parameters.AddWithValue("@text", question.Text);
                    cmd.Parameters.AddWithValue("@type", (int)question.Type);
                    cmd.Parameters.AddWithValue("@knowledge_class", question.KnowledgeClass);
                    question.Id = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var answer in question.Answers)
                {
                    string insertAnswer = "INSERT INTO Answers (question_id, text, is_correct) VALUES (@question_id, @text, @is_correct)";
                    using (var cmd = new SQLiteCommand(insertAnswer, connection))
                    {
                        cmd.Parameters.AddWithValue("@question_id", question.Id);
                        cmd.Parameters.AddWithValue("@text", answer.Text);
                        cmd.Parameters.AddWithValue("@is_correct", answer.IsCorrect);
                        cmd.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        public static List<Question> GetAllQuestions()
        {
            List<Question> questions = new List<Question>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Questions";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Type = (QuestionType)reader.GetInt32(reader.GetOrdinal("type")),
                                KnowledgeClass = reader.GetInt32(reader.GetOrdinal("knowledge_class")),
                                Answers = GetAnswersForQuestion(reader.GetInt32(reader.GetOrdinal("id")))
                            };
                            questions.Add(question);
                        }
                    }
                }
                connection.Close();
            }
            return questions;
        }

        private static ObservableCollection<Answer> GetAnswersForQuestion(int questionId)
        {
            ObservableCollection<Answer> answers = new ObservableCollection<Answer>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Answers WHERE question_id = @question_id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@question_id", questionId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            answers.Add(new Answer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                QuestionId = questionId,
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                IsCorrect = reader.GetBoolean(reader.GetOrdinal("is_correct"))
                            });
                        }
                    }
                }
                connection.Close();
            }
            return answers;
        }

        public static void UpdateQuestion(Question question)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string updateQuery = "UPDATE Questions SET text = @text, type = @type, knowledge_class = @knowledge_class WHERE id = @id";
                using (var cmd = new SQLiteCommand(updateQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", question.Id);
                    cmd.Parameters.AddWithValue("@text", question.Text);
                    cmd.Parameters.AddWithValue("@type", (int)question.Type);
                    cmd.Parameters.AddWithValue("@knowledge_class", question.KnowledgeClass);
                    cmd.ExecuteNonQuery();
                }

                // Smazání starých odpovědí a přidání nových
                string deleteAnswersQuery = "DELETE FROM Answers WHERE question_id = @id";
                using (var cmd = new SQLiteCommand(deleteAnswersQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", question.Id);
                    cmd.ExecuteNonQuery();
                }

                foreach (var answer in question.Answers)
                {
                    string insertAnswerQuery = "INSERT INTO Answers (question_id, text, is_correct) VALUES (@question_id, @text, @is_correct)";
                    using (var cmd = new SQLiteCommand(insertAnswerQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@question_id", question.Id);
                        cmd.Parameters.AddWithValue("@text", answer.Text);
                        cmd.Parameters.AddWithValue("@is_correct", answer.IsCorrect);
                        cmd.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        public static void DeleteQuestion(int questionId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();

                // Smazání odpovědí spojených s otázkou
                string deleteAnswersQuery = "DELETE FROM Answers WHERE question_id = @question_id";
                using (var cmd = new SQLiteCommand(deleteAnswersQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@question_id", questionId);
                    cmd.ExecuteNonQuery();
                }

                // Smazání samotné otázky
                string deleteQuestionQuery = "DELETE FROM Questions WHERE id = @id";
                using (var cmd = new SQLiteCommand(deleteQuestionQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@id", questionId);
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public static List<Question> GetQuestionsForClass(int knowledgeClass)
        {
            List<Question> questions = new List<Question>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Questions WHERE knowledge_class = @knowledge_class";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@knowledge_class", knowledgeClass);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Type = (QuestionType)reader.GetInt32(reader.GetOrdinal("type")),
                                KnowledgeClass = reader.GetInt32(reader.GetOrdinal("knowledge_class")),
                                Answers = GetAnswersForQuestion(reader.GetInt32(reader.GetOrdinal("id")))
                            };
                            questions.Add(question);
                        }
                    }
                }
                connection.Close();
            }
            return questions;
        }

        public static List<Question> GetRemainingQuestionsForTest(int maxKnowledgeClass, int count)
        {
            List<Question> questions = new List<Question>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Questions WHERE knowledge_class <= @maxKnowledgeClass ORDER BY RANDOM() LIMIT @count";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@maxKnowledgeClass", maxKnowledgeClass);
                    cmd.Parameters.AddWithValue("@count", count);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Type = (QuestionType)reader.GetInt32(reader.GetOrdinal("type")),
                                KnowledgeClass = reader.GetInt32(reader.GetOrdinal("knowledge_class")),
                                Answers = GetAnswersForQuestion(reader.GetInt32(reader.GetOrdinal("id")))
                            };
                            questions.Add(question);
                        }
                    }
                }
                connection.Close();
            }
            return questions;
        }

        public static List<Question> GetAdditionalQuestionsToFill(int count, int maxKnowledgeClass)
        {
            List<Question> questions = new List<Question>();
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Questions WHERE knowledge_class <= @maxKnowledgeClass ORDER BY RANDOM() LIMIT @count";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@maxKnowledgeClass", maxKnowledgeClass);
                    cmd.Parameters.AddWithValue("@count", count);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var question = new Question
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Text = reader.GetString(reader.GetOrdinal("text")),
                                Type = (QuestionType)reader.GetInt32(reader.GetOrdinal("type")),
                                KnowledgeClass = reader.GetInt32(reader.GetOrdinal("knowledge_class")),
                                Answers = GetAnswersForQuestion(reader.GetInt32(reader.GetOrdinal("id")))
                            };
                            questions.Add(question);
                        }
                    }
                }
                connection.Close();
            }
            return questions;
        }




    }
}
