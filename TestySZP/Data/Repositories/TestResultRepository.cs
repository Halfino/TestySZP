using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TestySZP.Data;
using TestySZP.Models;

namespace TestySZP.Data.Repositories
{
    public static class TestResultRepository
    {
        public static void AddTestResult(TestResult result)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var transaction = connection.BeginTransaction();

            using var command = new SQLiteCommand(connection);
            command.CommandText = @"
                INSERT INTO TestResults 
                (person_id, date_generated, date_completed, score, max_score, note, pdf_path)
                VALUES (@person_id, @date_generated, @date_completed, @score, @max_score, @note, @pdf_path);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@person_id", result.PersonId);
            command.Parameters.AddWithValue("@date_generated", result.DateGenerated);
            command.Parameters.AddWithValue("@date_completed", (object?)result.DateCompleted ?? DBNull.Value);
            command.Parameters.AddWithValue("@score", (object?)result.Score ?? DBNull.Value);
            command.Parameters.AddWithValue("@max_score", result.MaxScore);
            command.Parameters.AddWithValue("@note", result.Note ?? "");
            command.Parameters.AddWithValue("@pdf_path", result.PdfPath ?? "");

            long insertedId = (long)command.ExecuteScalar();
            result.Id = (int)insertedId;

            foreach (int questionId in result.QuestionIds)
            {
                using var qCmd = new SQLiteCommand("INSERT INTO TestQuestions (test_id, question_id) VALUES (@test_id, @question_id)", connection);
                qCmd.Parameters.AddWithValue("@test_id", result.Id);
                qCmd.Parameters.AddWithValue("@question_id", questionId);
                qCmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }

        public static List<TestResult> GetTestResultsForPerson(int personId)
        {
            var results = new List<TestResult>();

            using var connection = DatabaseHelper.GetConnection();
            using var command = new SQLiteCommand("SELECT * FROM TestResults WHERE person_id = @personId", connection);
            command.Parameters.AddWithValue("@personId", personId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var result = new TestResult
                {
                    Id = reader.GetInt32(0),
                    PersonId = reader.GetInt32(1),
                    DateGenerated = reader.GetDateTime(2),
                    DateCompleted = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                    Score = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                    MaxScore = reader.GetInt32(5),
                    Note = reader.IsDBNull(6) ? null : reader.GetString(6),
                    PdfPath = reader.IsDBNull(7) ? null : reader.GetString(7),
                    QuestionIds = new List<int>()
                };

                result.QuestionIds = GetQuestionIdsForTest(result.Id, connection);
                results.Add(result);
            }

            return results;
        }

        public static void UpdateTestResult(TestResult result)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SQLiteCommand(@"
                UPDATE TestResults 
                SET date_completed = @date_completed,
                    score = @score,
                    note = @note
                WHERE id = @id", connection);

            command.Parameters.AddWithValue("@date_completed", (object?)result.DateCompleted ?? DBNull.Value);
            command.Parameters.AddWithValue("@score", (object?)result.Score ?? DBNull.Value);
            command.Parameters.AddWithValue("@note", result.Note ?? "");
            command.Parameters.AddWithValue("@id", result.Id);

            command.ExecuteNonQuery();
        }

        public static void DeleteTestResult(int resultId)
        {
            using var connection = DatabaseHelper.GetConnection();
            using var command = new SQLiteCommand("DELETE FROM TestResults WHERE id = @id", connection);
            command.Parameters.AddWithValue("@id", resultId);
            command.ExecuteNonQuery();
        }

        private static List<int> GetQuestionIdsForTest(int testId, SQLiteConnection connection)
        {
            var ids = new List<int>();
            using var command = new SQLiteCommand("SELECT question_id FROM TestQuestions WHERE test_id = @test_id", connection);
            command.Parameters.AddWithValue("@test_id", testId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }

            return ids;
        }
    }
}
