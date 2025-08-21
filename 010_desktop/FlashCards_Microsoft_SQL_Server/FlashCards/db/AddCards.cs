using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace FlashCards.db
{
    public class BasicCards
    {
        public List<NormalCard> normalCards { get; set; }
        public List<QuizCard> quizCards { get; set; }
    }

    public class NormalCard
    {
        public string question { get; set; }
        public string answer { get; set; }
        public string is_fav { get; set; }
        public string status { get; set; }
    }

    public class QuizCard
    {
        public string question { get; set; }
        public string is_fav { get; set; }
        public int correctIndex { get; set; }
        public string status { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }
    }
    

    internal class AddCards
    {
        private static string connectionString = Properties.Settings.Default.connectionString;

        public static void AddCardsToDB(string data, string user, string deckId, string source)
        {
            var payload = JsonConvert.DeserializeObject<BasicCards>(data);

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                            using (var deleteCmd = new SqlCommand(@"
                            DELETE FROM quiz WHERE deck_id = @deckId;
                            DELETE FROM cards WHERE deck_id = @deckId;", connection, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@deckId", deckId);
                                deleteCmd.ExecuteNonQuery();
                            }

                        if (payload.normalCards != null)
                        {
                            foreach (var card in payload.normalCards)
                            {
                                using (var cmd = new SqlCommand(
                                    @"INSERT INTO cards (deck_id, question, answer, is_fav, status)
                                      VALUES (@deckId, @question, @answer, @isFav, @status);",
                                    connection, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@deckId", deckId);
                                    cmd.Parameters.AddWithValue("@question", card.question);
                                    cmd.Parameters.AddWithValue("@answer", card.answer);
                                    cmd.Parameters.AddWithValue("@isFav", card.is_fav);
                                    cmd.Parameters.AddWithValue("@status", card.status);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        if (payload.quizCards != null)
                        {
                            foreach (var quiz in payload.quizCards)
                            {
                                int quizId;
                                using (var cmdQuiz = new SqlCommand(
                                    @"INSERT INTO quiz (deck_id, question, is_fav, correct_answer, status)
                                      VALUES (@deckId, @question, @isFav, @correctAnswer, @status);
                                      SELECT SCOPE_IDENTITY();",
                                    connection, transaction))
                                {
                                    cmdQuiz.Parameters.AddWithValue("@deckId", deckId);
                                    cmdQuiz.Parameters.AddWithValue("@question", quiz.question);
                                    cmdQuiz.Parameters.AddWithValue("@isFav", quiz.is_fav);
                                    cmdQuiz.Parameters.AddWithValue("@correctAnswer", quiz.correctIndex);
                                    cmdQuiz.Parameters.AddWithValue("@status", quiz.status);

                                    var insertedId = cmdQuiz.ExecuteScalar();
                                    quizId = Convert.ToInt32(insertedId);
                                }

                                using (var cmdOpts = new SqlCommand(
                                    @"INSERT INTO quiz_options (quiz_id, first_option, second_option, third_option, fourth_option)
                                      VALUES (@quizId, @o1, @o2, @o3, @o4);",
                                    connection, transaction))
                                {
                                    cmdOpts.Parameters.AddWithValue("@quizId", quizId);
                                    cmdOpts.Parameters.AddWithValue("@o1", quiz.option1);
                                    cmdOpts.Parameters.AddWithValue("@o2", quiz.option2);
                                    cmdOpts.Parameters.AddWithValue("@o3", quiz.option3);
                                    cmdOpts.Parameters.AddWithValue("@o4", quiz.option4);
                                    cmdOpts.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Fehler beim Hinzufügen des Decks: {ex.Message}");
                        throw;
                    }
                }
            }
        }
    }
}
