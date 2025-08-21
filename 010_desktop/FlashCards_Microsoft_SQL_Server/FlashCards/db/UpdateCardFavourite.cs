using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class UpdateCardFavourite
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        private static string updateCardFavQuery;

        public static bool UpdateCardFav(string user, string deckId, string cardId, string is_fav, string type)
        {
            Console.WriteLine($"Register new user: {user}");

            if (type == "card")
            {
                updateCardFavQuery = @"UPDATE cards SET is_fav = @isFav WHERE id = @cardId";
            }
            else
            {
                updateCardFavQuery = @"UPDATE quiz SET is_fav = @isFav WHERE id = @cardId";
            }


                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    try
                    {
                        using (SqlCommand updateStmt = new SqlCommand(updateCardFavQuery, connection))
                        {
                            updateStmt.Parameters.AddWithValue("@isFav", is_fav);
                            updateStmt.Parameters.AddWithValue("@cardId", cardId);

                            int rowsAffected = updateStmt.ExecuteNonQuery();

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
