using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class GetDeck
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static async Task<string> GetDeckById(string user, string deckId)
        {
            string sql = @"
                IF NOT EXISTS (SELECT 1 FROM decks WHERE id = @deckId)
BEGIN
                    SELECT
                    d.id,
                    d.title,
                    d.alt,
                    d.is_private,
                    d.creator_id,
                    d.created_at,
                    dc.start_color,
                    dc.end_color
                FROM decks AS d
                JOIN deck_colors AS dc
                    ON d.id = dc.deck_id
                WHERE d.id = @deckId;
END
ELSE
BEGIN
                    SELECT
                    d.id,
                    d.title,
                    d.alt,
                    d.is_private,
                    d.creator_id,
                    d.created_at,
                    dc.start_color,
                    dc.end_color,
                    MIN(u.username) AS username
                FROM decks AS d
                JOIN deck_colors AS dc
                    ON d.id = dc.deck_id
                LEFT JOIN collaborators AS c
                    ON d.id = c.deck_id
                LEFT JOIN users AS u
                    ON u.id = c.user_id
                WHERE d.id = @deckId
                GROUP BY
                    d.id, d.title, d.alt, d.is_private, d.creator_id, d.created_at,
                    dc.start_color, dc.end_color;
END";
            string isAdmin = @"
                SELECT collaborators.* 
                FROM collaborators
                JOIN decks ON collaborators.deck_id = decks.id
                WHERE collaborators.deck_id = @deckId AND decks.creator_id = (SELECT id FROM users WHERE username = @username);";

            var decks = new List<object>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@deckId", deckId);
                        cmd.Parameters.AddWithValue("@username", user);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var deck = new
                                {
                                    id = reader["id"] == DBNull.Value ? (Int32?)null : Convert.ToInt32(reader["id"]),
                                    title = reader["title"] == DBNull.Value ? null : Convert.ToString(reader["title"]),
                                    alt = reader["alt"] == DBNull.Value ? null : Convert.ToString(reader["alt"]),
                                    is_private = reader["is_private"] == DBNull.Value ? (Int32?)null : Convert.ToInt32(reader["is_private"]),
                                    creator_id = reader["creator_id"] == DBNull.Value ? null : Convert.ToString(reader["creator_id"]),
                                    created_at = reader["created_at"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["created_at"]),
                                    start_color = reader["start_color"] == DBNull.Value ? null : Convert.ToString(reader["start_color"]),
                                    end_color = reader["end_color"] == DBNull.Value ? null : Convert.ToString(reader["end_color"]),
                                    username = reader["username"] == DBNull.Value ? null : Convert.ToString(reader["username"])
                                };

                                decks.Add(deck);
                            }
                        }
                    }


                    using (SqlCommand command = new SqlCommand(isAdmin, connection))
                    {
                        command.Parameters.AddWithValue("@deckId", deckId);
                        command.Parameters.AddWithValue("@username", user);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {

                            var isHost = new
                            {
                                isHost = reader.HasRows ? true : false,
                            };

                            decks.Add(isHost);
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
