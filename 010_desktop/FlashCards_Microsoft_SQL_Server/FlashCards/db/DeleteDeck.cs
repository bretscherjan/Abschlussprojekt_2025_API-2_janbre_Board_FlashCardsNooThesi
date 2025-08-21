using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FlashCards.db
{
    internal class DeleteDeck
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static void DeleteDeckFromDB(string deckId)
        {
            var queries = new List<(string Query, string Param)>
            {
                ("DELETE FROM collaborators WHERE deck_id = @deckId", "@deckId"),
                ("DELETE FROM deck_colors WHERE deck_id = @deckId", "@deckId"),
                ("DELETE FROM cards WHERE deck_id = @deckId", "@deckId"),
                ("DELETE FROM quiz_options WHERE quiz_id IN (SELECT id FROM quiz WHERE deck_id = @deckId)", "@deckId"),
                ("DELETE FROM quiz WHERE deck_id = @deckId", "@deckId"),
                ("DELETE FROM decks WHERE id = @deckId", "@deckId")
            };

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var (query, param) in queries)
                        {
                            using (var cmd = new SqlCommand(query, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue(param, deckId);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error deleting deck: {ex.Message}");
                    }
                }
            }
        }
    }
}
