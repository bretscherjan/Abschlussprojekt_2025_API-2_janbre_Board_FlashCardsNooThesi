using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class UpdateUser
    {

        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static bool UpdateuserInDB(string user, string email, string password, string olduser)
        {

            string updateUserQuery = $"UPDATE users SET username = @username, email = @email, password = @password WHERE username = @olduser";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SqlCommand checkStmt = new SqlCommand(updateUserQuery, connection))
                    {
                        checkStmt.Parameters.AddWithValue("@username", user);
                        checkStmt.Parameters.AddWithValue("@email", email);
                        checkStmt.Parameters.AddWithValue("@password", password);
                        checkStmt.Parameters.AddWithValue("@olduser", olduser);

                        int rowsAffected = checkStmt.ExecuteNonQuery();

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
