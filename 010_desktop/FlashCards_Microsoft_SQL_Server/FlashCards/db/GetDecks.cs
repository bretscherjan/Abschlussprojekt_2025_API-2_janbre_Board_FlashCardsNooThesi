using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class GetDecks
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static async Task<string> GetDecksFromDB(string user)
        {
            Console.WriteLine($"Get decks for user: {user}");

            string getQuery = @"
        SELECT d.id, d.title, d.alt, d.is_private, d.created_at, dc.start_color, dc.end_color 
        FROM decks d 
        LEFT JOIN deck_colors dc ON d.id = dc.deck_id 
        WHERE d.creator_id = (SELECT u.id FROM users u WHERE u.username = @firstUserName) 
        OR d.id IN (
            SELECT deck_id FROM collaborators 
            WHERE user_id = (SELECT u.id FROM users u WHERE u.username = @secondUserName)
        );";

            var decks = new List<object>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                try
                {
                    using (SqlCommand getStmt = new SqlCommand(getQuery, connection))
                    {
                        getStmt.Parameters.AddWithValue("@firstUserName", user);
                        getStmt.Parameters.AddWithValue("@secondUserName", user);

                        using (SqlDataReader reader = await getStmt.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var deck = new
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    title = reader["title"] as string,
                                    alt = reader["alt"] as string,
                                    is_private = (reader["is_private"] != DBNull.Value && (bool)reader["is_private"]) ? 1 : 0,
                                    created_at = Convert.ToDateTime(reader["created_at"]),
                                    start_color = reader["start_color"] as string,
                                    end_color = reader["end_color"] as string
                                };

                                decks.Add(deck);
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

            return JsonConvert.SerializeObject(decks);
        }

    }
}
