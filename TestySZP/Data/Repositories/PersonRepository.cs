using System.Data.SQLite;
using TestySZP.Models;

namespace TestySZP.Data.Repositories
{
    public static class PersonRepository
    {

        public static List<Person> GetAll()
        {
            var people = new List<Person>();

            using var connection = DatabaseHelper.GetConnection();


            using var command = new SQLiteCommand("SELECT * FROM Persons", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                people.Add(new Person
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    KnowledgeClass = reader.GetInt32(2),
                    ValidUntil = reader.GetDateTime(3)
                });
            }

            return people;
        }

        public static void AddPerson(Person person)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                string query = "INSERT INTO Persons (name, knowledge_class, valid_until) VALUES (@name, @knowledge_class, @valid_until)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@knowledge_class", person.KnowledgeClass);
                    cmd.Parameters.AddWithValue("@valid_until", person.ValidUntil.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static List<Person> GetAllPersons()
        {
            List<Person> persons = new List<Person>();
            using (var connection = DatabaseHelper.GetConnection())
            {

                string query = "SELECT * FROM Persons";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            persons.Add(new Person
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                KnowledgeClass = reader.GetInt32(reader.GetOrdinal("knowledge_class")),
                                ValidUntil = DateTime.Parse(reader.GetString(reader.GetOrdinal("valid_until")))
                            });
                        }
                    }
                }
                connection.Close();
            }
            return persons;
        }

        public static void UpdatePerson(Person person)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                string query = "UPDATE Persons SET name = @name, knowledge_class = @knowledge_class, valid_until = @valid_until WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", person.Id);
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@knowledge_class", person.KnowledgeClass);
                    cmd.Parameters.AddWithValue("@valid_until", person.ValidUntil.ToString("yyyy-MM-dd"));
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void DeletePerson(int personId)
        {
            using (var connection = DatabaseHelper.GetConnection())
            {

                string query = "DELETE FROM Persons WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", personId);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
