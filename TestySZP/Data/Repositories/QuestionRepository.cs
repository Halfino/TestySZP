using System.Collections.Generic;
using System.Data.SQLite;
using TestySZP.Models;
using TestySZP.Data;
using System.Linq;

namespace TestySZP.Data.Repositories
{
    public static class QuestionRepository
    {
        public static List<Question> GetAllQuestions()
        {
            var questions = new List<Question>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Questions", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var question = new Question
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.GetString(1),
                            IsWritten = reader.GetBoolean(2),
                            KnowledgeClass = reader.GetInt32(3),
                            Answers = new System.Collections.ObjectModel.ObservableCollection<Answer>(),
                            AnswerCount = 0
                        };

                        // načtení odpovědí
                        using (var ansConn = DatabaseHelper.GetConnection())
                        {
                            ansConn.Open();
                            using (var ansCmd = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @id", ansConn))
                            {
                                ansCmd.Parameters.AddWithValue("@id", question.Id);
                                using (var ansReader = ansCmd.ExecuteReader())
                                {
                                    while (ansReader.Read())
                                    {
                                        question.Answers.Add(new Answer
                                        {
                                            Id = ansReader.GetInt32(0),
                                            QuestionId = ansReader.GetInt32(1),
                                            Text = ansReader.GetString(2),
                                            IsCorrect = ansReader.GetBoolean(3)
                                        });
                                    }
                                }
                            }
                        }

                        question.AnswerCount = question.Answers.Count;
                        questions.Add(question);
                    }
                }
            }

            return questions;
        }


        public static void AddQuestion(Question question)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        INSERT INTO Questions (text, written, knowledge_class)
                        VALUES (@text, @written, @class)";
                    command.Parameters.AddWithValue("@text", question.Text);
                    command.Parameters.AddWithValue("@written", question.IsWritten);
                    command.Parameters.AddWithValue("@class", question.KnowledgeClass);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateQuestion(Question question)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"
                        UPDATE Questions
                        SET text = @text,
                            written = @written,
                            knowledge_class = @class
                        WHERE id = @id";
                    command.Parameters.AddWithValue("@text", question.Text);
                    command.Parameters.AddWithValue("@written", question.IsWritten);
                    command.Parameters.AddWithValue("@class", question.KnowledgeClass);
                    command.Parameters.AddWithValue("@id", question.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteQuestion(int questionId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand("DELETE FROM Questions WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", questionId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static List<Question> GetQuestionsForClass(int classLevel)
        {
            var result = new List<Question>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand("SELECT * FROM Questions WHERE knowledge_class = @class", connection))
                {
                    command.Parameters.AddWithValue("@class", classLevel);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                IsWritten = reader.GetBoolean(2),
                                KnowledgeClass = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            foreach (var question in result)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", question.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Answers.Add(new Answer
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
            }


                return result;
        }

        public static List<Question> GetRemainingQuestionsForTest(int classLevel, int count)
        {
            var result = new List<Question>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(@"
                    SELECT * FROM Questions 
                    WHERE knowledge_class < @class
                    ORDER BY RANDOM()
                    LIMIT @count", connection))
                {
                    command.Parameters.AddWithValue("@class", classLevel);
                    command.Parameters.AddWithValue("@count", count);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                IsWritten = reader.GetBoolean(2),
                                KnowledgeClass = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            foreach (var question in result)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", question.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Answers.Add(new Answer
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
            }

            return result;
        }

        public static List<Question> GetAdditionalQuestionsToFill(int count, int classLevel)
        {
            var result = new List<Question>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(@"
                    SELECT * FROM Questions
                    WHERE knowledge_class <= @class
                    ORDER BY RANDOM()
                    LIMIT @count", connection))
                {
                    command.Parameters.AddWithValue("@class", classLevel);
                    command.Parameters.AddWithValue("@count", count);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                IsWritten = reader.GetBoolean(2),
                                KnowledgeClass = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            foreach (var question in result)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", question.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Answers.Add(new Answer
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
            }

            return result;
        }

        public static Question GetQuestionById(int id)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand("SELECT * FROM Questions WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Question
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    IsWritten = reader.GetBoolean(2),
                    KnowledgeClass = reader.GetInt32(3),
                    AnswerCount = GetAnswerCount(reader.GetInt32(0))
                };
            }

            return null;
        }

        public static List<Question> GetLowerClassQuestions(int classLevel)
        {
            var result = new List<Question>();

            using (var connection = DatabaseHelper.GetConnection())
            {
                connection.Open();
                using (var command = new SQLiteCommand(@"
            SELECT * FROM Questions 
            WHERE knowledge_class < @class", connection))
                {
                    command.Parameters.AddWithValue("@class", classLevel);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Question
                            {
                                Id = reader.GetInt32(0),
                                Text = reader.GetString(1),
                                IsWritten = reader.GetBoolean(2),
                                KnowledgeClass = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }

            // Načtení odpovědí
            foreach (var question in result)
            {
                using (var connection = DatabaseHelper.GetConnection())
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT * FROM Answers WHERE question_id = @id", connection))
                    {
                        command.Parameters.AddWithValue("@id", question.Id);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                question.Answers.Add(new Answer
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
            }

            return result;
        }


        private static int GetAnswerCount(int questionId)
        {
            using var connection = DatabaseHelper.GetConnection();
            connection.Open();
            using var command = new SQLiteCommand("SELECT COUNT(*) FROM Answers WHERE question_id = @id", connection);
            command.Parameters.AddWithValue("@id", questionId);
            return Convert.ToInt32(command.ExecuteScalar());
        }


    }
}
