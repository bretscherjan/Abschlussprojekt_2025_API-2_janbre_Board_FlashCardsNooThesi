using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FlashCards.db
{
    internal class Follows
    {

        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static async Task<string> GetUsersFollowers(string user)
        {

            string getQuery = @"SELECT u.username FROM users u JOIN follows f ON u.id = f.follower_id WHERE f.followed_id = (SELECT id FROM users WHERE username = @username) AND username != @username";

            var followers = new List<object>();

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
                                var follower = new
                                {
                                    username = reader["username"] as string
                                };

                                followers.Add(follower);
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
            return JsonConvert.SerializeObject(followers);
        }

        public static async Task<string> GetUsersFollowing(string user)
        {

            string getQuery = @"SELECT u.username FROM users u JOIN follows f ON u.id = f.followed_id WHERE f.follower_id = (SELECT id FROM users WHERE username = @username) AND username != @username";

            var following = new List<object>();

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
                                var followedUser = new
                                {
                                    username = reader["username"] as string
                                };

                                following.Add(followedUser);
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
            return JsonConvert.SerializeObject(following);
        }

        public static async Task<string> GetUsersNotFollowing(string user)
        {

            string getQuery = @"SELECT u.username FROM users u LEFT JOIN follows f ON u.id = f.followed_id AND f.follower_id = (SELECT id FROM users WHERE username = @username) WHERE f.followed_id IS NULL AND u.username != @username";

            var notFollowing = new List<object>();

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
                                var notFollowingUser = new
                                {
                                    username = reader["username"] as string
                                };

                                notFollowing.Add(notFollowingUser);
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
            return JsonConvert.SerializeObject(notFollowing);
        }


        public static void Unfollow(string user, string selectedUser) 
        {
            string insertQuery = $"DELETE FROM follows WHERE follower_id = (SELECT id FROM users WHERE username = @username) AND followed_id = (SELECT id FROM users WHERE username = @selectedUsername)";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                try
                {

                    using (SqlCommand deleteStmt = new SqlCommand(insertQuery, connection))
                    {
                        deleteStmt.Parameters.AddWithValue("@username", user);
                        deleteStmt.Parameters.AddWithValue("@selectedUsername", selectedUser);

                        int rowsAffected = deleteStmt.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Registrierung: {ex.Message}");
                }
            }
        }


        public static void Follow(string user, string selectedUser) 
        {
            string insertQuery = $"INSERT INTO follows (follower_id, followed_id) VALUES ((SELECT id FROM users WHERE username = @username), (SELECT id FROM users WHERE username = @selectedUsername))";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SqlCommand insertStmt = new SqlCommand(insertQuery, connection))
                    {
                        insertStmt.Parameters.AddWithValue("@username", user);
                        insertStmt.Parameters.AddWithValue("@selectedUsername", selectedUser);

                        int rowsAffected = insertStmt.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler bei der Registrierung: {ex.Message}");
                }
            }
        }
    }
}
