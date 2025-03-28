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

        public static SQLiteConnection GetConnectionNoPragma()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            return connection;
        }

        public static void InitializeDatabase()
        {
            EnsureDatabaseExists();
            EnsureTablesExist();
        }

        private static void EnsureDatabaseExists()
        {
            if (!File.Exists(DbFile))
            {
                SQLiteConnection.CreateFile(DbFile);
            }
        }

        private static void EnsureTablesExist()
        {
            using (var connection = GetConnection())
            using (var command = new SQLiteCommand(connection))
            {
                string createQuestionsTable = @"
            CREATE TABLE IF NOT EXISTS Questions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                text TEXT NOT NULL,
                written BOOLEAN NOT NULL,
                knowledge_class INTEGER NOT NULL
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
            );";

                string createTestResultsTable = @"
            CREATE TABLE IF NOT EXISTS TestResults (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                person_id INTEGER NOT NULL,
                date_generated DATETIME NOT NULL,
                date_completed DATETIME,
                score INTEGER,
                max_score INTEGER,
                note TEXT,
                pdf_path TEXT,
                FOREIGN KEY (person_id) REFERENCES Persons(id) ON DELETE CASCADE
            );";

                string createTestQuestionsTable = @"
            CREATE TABLE IF NOT EXISTS TestQuestions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                test_id INTEGER NOT NULL,
                question_id INTEGER NOT NULL,
                FOREIGN KEY (test_id) REFERENCES TestResults(id) ON DELETE CASCADE,
                FOREIGN KEY (question_id) REFERENCES Questions(id) ON DELETE CASCADE
            );";

                command.CommandText = createQuestionsTable;
                command.ExecuteNonQuery();

                command.CommandText = createAnswersTable;
                command.ExecuteNonQuery();

                command.CommandText = createPersonsTable;
                command.ExecuteNonQuery();

                command.CommandText = createQuestionCategoriesTable;
                command.ExecuteNonQuery();

                command.CommandText = createTestResultsTable;
                command.ExecuteNonQuery();

                command.CommandText = createTestQuestionsTable;
                command.ExecuteNonQuery();
            }
        }
    }
}
