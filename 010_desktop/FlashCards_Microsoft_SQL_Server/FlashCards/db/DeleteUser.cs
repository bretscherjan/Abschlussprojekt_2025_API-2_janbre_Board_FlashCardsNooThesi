using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class DeleteUser
    {

        private static string connectionString = Properties.Settings.Default.connectionString;

        public static void DeleteUserFromDB(string user)
        {
            string deleteUserQuery = "DELETE FROM users WHERE username = @username";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();


                try
                {
                    using (SqlCommand deleteUserCmd = new SqlCommand(deleteUserQuery, connection))
                    {
                        deleteUserCmd.Parameters.AddWithValue("@username", user);

                        int rowsAffected = deleteUserCmd.ExecuteNonQuery();

                    }
                } catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting user: {ex.Message}");

                }
            }
        }

    }
}
