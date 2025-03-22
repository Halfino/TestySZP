using System;
using System.Data.SQLite;
using System.IO;

namespace TestySZP.Data
{
    public class DatabaseHelper
    {
        private const string DbFile = "testy_szp.db";
        private const string ConnectionString = "Data Source=" + DbFile + ";Version=3;";

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            // Aktivuj podporu cizích klíčů (včetně ON DELETE CASCADE)
            using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection))
            {
                command.ExecuteNonQuery();
            }

            return connection;
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);

                using (var connection = GetConnection())
                {
                    connection.Open();

                    string createQuestionsTable = @"
CREATE TABLE IF NOT EXISTS Questions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    text TEXT NOT NULL,
    written BOOLEAN NOT NULL, -- true = Written, false = MultipleChoice
    knowledge_class INTEGER NOT NULL -- 1 = Nejvyšší, 3 = Nejnižší
);";

                    string createAnswersTable = @"
CREATE TABLE IF NOT EXISTS Answers (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    question_id INTEGER NOT NULL,
    text TEXT NOT NULL,
    is_correct BOOLEAN NOT NULL,
    FOREIGN KEY (question_id) REFERENCES Questions(id) ON DELETE CASCADE
);";

                    string createPersonsTable = @"
                    CREATE TABLE IF NOT EXISTS Persons (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        name TEXT NOT NULL,
                        knowledge_class INTEGER NOT NULL,
                        valid_until DATETIME NOT NULL
                    );";

                    string createQuestionCategoriesTable = @"
                    CREATE TABLE IF NOT EXISTS QuestionCategories (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        question_id INTEGER,
                        knowledge_class INTEGER NOT NULL,
                        FOREIGN KEY(question_id) REFERENCES Questions(id)
                    );"
                    ;


                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createQuestionsTable;
                        command.ExecuteNonQuery();
                        command.CommandText = createAnswersTable;
                        command.ExecuteNonQuery();
                        command.CommandText = createPersonsTable;
                        command.ExecuteNonQuery();
                        command.CommandText = createQuestionCategoriesTable;
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
        }
    }
}
