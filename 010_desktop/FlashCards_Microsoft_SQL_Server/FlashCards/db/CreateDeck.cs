using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class CreateDeck
    {
        private static string connectionString = Properties.Settings.Default.connectionString;

        public static void AddDeck(string user, string firstColor, string secondColor, string title, string altText, string selectedUser)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int deckId;

                        using (var cmd = new SqlCommand(
                            "INSERT INTO decks (title, alt, is_private, creator_id) " +
                            "VALUES (@title, @altText, 0, (SELECT id FROM users WHERE username = @user)); " +
                            "SELECT CAST(SCOPE_IDENTITY() AS int);", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@title", title);
                            cmd.Parameters.AddWithValue("@altText", altText);
                            cmd.Parameters.AddWithValue("@user", user);

                            deckId = (int)cmd.ExecuteScalar();
                        }

                        using (var cmd = new SqlCommand(
                            "INSERT INTO deck_colors (deck_id, start_color, end_color) " +
                            "VALUES (@deckId, @firstColor, @secondColor)", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@deckId", deckId);
                            cmd.Parameters.AddWithValue("@firstColor", firstColor);
                            cmd.Parameters.AddWithValue("@secondColor", secondColor);

                            cmd.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrEmpty(selectedUser))
                        {
                            using (var cmd = new SqlCommand(
                                "INSERT INTO collaborators (deck_id, user_id) " +
                                "VALUES (@deckId, (SELECT id FROM users WHERE username = @selectedUser))", connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@deckId", deckId);
                                cmd.Parameters.AddWithValue("@selectedUser", selectedUser);

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Fehler beim Hinzufügen des Decks: {ex.Message}");
                    }
                }
            }

        }
    }
}
