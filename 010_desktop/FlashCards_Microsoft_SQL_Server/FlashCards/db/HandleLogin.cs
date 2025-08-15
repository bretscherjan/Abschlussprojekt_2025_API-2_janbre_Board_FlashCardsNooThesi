using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace FlashCards.db
{
    internal class HandleLogin
    {

        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static void Login(string user, string password)
        {
            Console.WriteLine($"Logging in user: {user} with password: {password}");

            string Query = $"SELECT password FROM users WHERE username = @username";


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                connection.Open();

                try
                {
                    using (SqlCommand command = new SqlCommand(Query, connection))
                    {
                        command.Parameters.AddWithValue("@username", user);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["password"].ToString();
                                if (storedPassword == password)
                                {
                                    Console.WriteLine("Login successful.");
                                    Properties.Settings.Default.username = user;
                                    Properties.Settings.Default.password = password;
                                    Properties.Settings.Default.LastLoginDate = DateTime.Now;
                                    Properties.Settings.Default.isLogedIn = true;
                                    Properties.Settings.Default.Save();
                                }
                                else
                                {
                                    Console.WriteLine("Login failed: Incorrect password.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Login failed: User not found.");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during login: {ex.Message}");
                }
            }

        }

    }
}
