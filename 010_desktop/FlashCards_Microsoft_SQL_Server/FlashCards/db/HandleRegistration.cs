using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class HandleRegistration
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static bool Registration(string user, string email, string password)
        {
            Console.WriteLine($"Register new user: {user}");

            string checkQuery = $"SELECT id FROM users WHERE username = @username OR email = @email";
            string insertQuery = $"INSERT INTO users (username, email, password) VALUES (@username, @email, @password)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SqlCommand checkStmt = new SqlCommand(checkQuery, connection))
                    {
                        checkStmt.Parameters.AddWithValue("@username", user);
                        checkStmt.Parameters.AddWithValue("@email", email);

                        using (SqlDataReader reader = checkStmt.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Benutzername oder E-Mail existieren bereits.");
                                return false;
                            }
                        }
                    }

                    using (SqlCommand insertStmt = new SqlCommand(insertQuery, connection))
                    {
                        insertStmt.Parameters.AddWithValue("@username", user);
                        insertStmt.Parameters.AddWithValue("@email", email);
                        insertStmt.Parameters.AddWithValue("@password", password);

                        int rowsAffected = insertStmt.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Registrierung: {ex.Message}");
                    return false;
                }
            }
        }



    }
}
