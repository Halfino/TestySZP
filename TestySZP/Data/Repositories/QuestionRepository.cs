﻿using System.Collections.Generic;
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
                        questions.Add(new Question
                        {
                            Id = reader.GetInt32(0),
                            Text = reader.GetString(1),
                            IsWritten = reader.GetBoolean(2),
                            KnowledgeClass = reader.GetInt32(3),
                            AnswerCount = GetAnswerCount(reader.GetInt32(0)) // doplníme funkci
                        });
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
