using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class GetUserData
    {

        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static async Task<string> GetUsersCredentials(string user)
        {

            string getQuery = @"SELECT TOP 1 username, email, password FROM users WHERE username = @username;";

            var data = new List<object>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                try
                {
                    using (SqlCommand getStmt = new SqlCommand(getQuery, connection))
                    {
                        getStmt.Parameters.AddWithValue("@username", user);

                        using (SqlDataReader reader = await getStmt.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var credentials = new
                                {
                                    username = reader["username"] as string,
                                    email = reader["email"] as string,
                                    password = reader["password"] as string
                                };

                                data.Add(credentials);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Abrufen der Decks: {ex.Message}");
                    return "[]";
                }
            }

            return JsonConvert.SerializeObject(data);
        }


    }
}
