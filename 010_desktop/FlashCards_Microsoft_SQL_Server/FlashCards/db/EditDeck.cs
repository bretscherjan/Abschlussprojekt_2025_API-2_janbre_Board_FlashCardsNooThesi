using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace FlashCards.db
{
    internal class EditDeck
    {



        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static bool UpdateDeckInDB(string title, string alt, string firstColor, string secondColor, int deckId, string collaborator)
        {
            string collaboratorQuery = null;

            if (collaborator != "" && collaborator != "You are not Admin of this deck")
            {
                collaboratorQuery = @"
                    IF NOT EXISTS (SELECT 1 FROM collaborators WHERE deck_id = @deckId)
                    BEGIN
                        INSERT INTO collaborators (deck_id, user_id, can_edit) VALUES (@deckId, @collaboratorUser, @isPrivate);
                    END
                    ELSE
                    BEGIN
                        UPDATE collaborators SET user_id = (SELECT id FROM users WHERE username = @collaboratorUser) WHERE deck_id = @deckId;
                    END";
            }

            string updateUserQuery = @"
                UPDATE decks SET title = @title, alt = @alt, is_private = @isPrivate WHERE id = @deckId;
                UPDATE deck_colors SET start_color = @firstColor, end_color = @secondColor WHERE deck_id = @deckId;" + collaboratorQuery;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                try
                {
                    using (SqlCommand checkStmt = new SqlCommand(updateUserQuery, connection))
                    {
                        checkStmt.Parameters.AddWithValue("@title", title);
                        checkStmt.Parameters.AddWithValue("@alt", alt);
                        checkStmt.Parameters.AddWithValue("@isPrivate", 1);
                        checkStmt.Parameters.AddWithValue("@deckId", deckId);
                        checkStmt.Parameters.AddWithValue("@firstColor", firstColor);
                        checkStmt.Parameters.AddWithValue("@secondColor", secondColor);
                        checkStmt.Parameters.AddWithValue("@collaboratorUser", collaborator);

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

        public static bool DeleteCollaboratorsFromDeck(int deckId)
        {
            string deleteQuery = "DELETE FROM collaborators WHERE deck_id = @deckId;";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                try
                {
                    using (SqlCommand deleteStmt = new SqlCommand(deleteQuery, connection))
                    {
                        deleteStmt.Parameters.AddWithValue("@deckId", deckId);
                        int rowsAffected = deleteStmt.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Löschen der Collaborators: {ex.Message}");
                    return false;
                }
            }
        }




    }
}


////////////////////////////////////
//// More than one collaobrator ////
////////////////////////////////////