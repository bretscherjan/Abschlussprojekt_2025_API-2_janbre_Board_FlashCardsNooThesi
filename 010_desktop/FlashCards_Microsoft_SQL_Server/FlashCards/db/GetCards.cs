using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards.db
{
    internal class GetCards
    {
        private static string ConnectionString = Properties.Settings.Default.connectionString;

        public static async Task<string> GetCardsFromDB(string deckId)
        {

            string getQuery = @"
            SELECT 
                'card' AS type,
                c.id,
                c.question,
                c.answer,
                NULL AS correct_answer,
                NULL AS first_option,
                NULL AS second_option,
                NULL AS third_option,
                NULL AS fourth_option,
                c.is_fav,
                c.status,
                c.created_at,
                d.title
            FROM cards c
            JOIN decks d ON c.deck_id = d.id
            WHERE c.deck_id = @deckId

            UNION ALL

            SELECT 
                'quiz' AS type,
                q.id,
                q.question,
                NULL AS answer,
                q.correct_answer,
                qo.first_option,
                qo.second_option,
                qo.third_option,
                qo.fourth_option,
                q.is_fav,
                q.status,
                q.created_at,
                d.title
            FROM quiz q
            JOIN quiz_options qo ON q.id = qo.quiz_id
            JOIN decks d ON q.deck_id = d.id
            WHERE q.deck_id = @deckId;";

            var cards = new List<object>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                await connection.OpenAsync();

                try
                {
                    using (SqlCommand getStmt = new SqlCommand(getQuery, connection))
                    {
                        getStmt.Parameters.AddWithValue("@deckId", deckId);

                        using (SqlDataReader reader = await getStmt.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var card = new
                                {
                                    type = reader["type"] != DBNull.Value ? reader["type"].ToString() : null,
                                    id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : (int?)null,
                                    question = reader["question"] != DBNull.Value ? reader["question"].ToString() : null,
                                    answer = reader["answer"] != DBNull.Value ? reader["answer"].ToString() : null,
                                    correct_answer = reader["correct_answer"] != DBNull.Value ? Convert.ToInt32(reader["correct_answer"]) : (int?)null,
                                    first_option = reader["first_option"] != DBNull.Value ? reader["first_option"].ToString() : null,
                                    second_option = reader["second_option"] != DBNull.Value ? reader["second_option"].ToString() : null,
                                    third_option = reader["third_option"] != DBNull.Value ? reader["third_option"].ToString() : null,
                                    fourth_option = reader["fourth_option"] != DBNull.Value ? reader["fourth_option"].ToString() : null,
                                    is_fav = reader["is_fav"] != DBNull.Value ? Convert.ToInt32(reader["is_fav"]) : (int?)null,
                                    status = reader["status"] != DBNull.Value ? reader["status"].ToString() : null,
                                    created_at = reader["created_at"] != DBNull.Value ? Convert.ToDateTime(reader["created_at"]) : (DateTime?)null,
                                    title = reader["title"] != DBNull.Value ? reader["title"].ToString() : null

                                };

                                cards.Add(card);
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

            return JsonConvert.SerializeObject(cards);
        }



    }
}
