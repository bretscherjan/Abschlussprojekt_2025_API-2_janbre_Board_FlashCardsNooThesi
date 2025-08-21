/**
 * AddCards.xaml.cs
 *
 * Manages the addition and editing of cards within a deck, including UI and API communication.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using FlashCards.db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlashCards
{
    public abstract class BaseCard
    {
        [JsonPropertyName("question")]
        public string Question { get; set; } = "";
    }

    public class Card : BaseCard
    {
        [JsonPropertyName("answer")]
        public string Answer { get; set; } = "";
    }

    public class QuizCard : BaseCard
    {
        [JsonPropertyName("option1")]
        public string Option1 { get; set; } = "";

        [JsonPropertyName("option2")]
        public string Option2 { get; set; } = "";

        [JsonPropertyName("option3")]
        public string Option3 { get; set; } = "";

        [JsonPropertyName("option4")]
        public string Option4 { get; set; } = "";

        [JsonPropertyName("correctIndex")]
        public string CorrectIndex { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für AddCards.xaml
    /// </summary>
    public partial class AddCards : Window
    {
        private string _user = Properties.Settings.Default.username;
        private int _deckId;

        private readonly HttpClient _httpClient;
        public ObservableCollection<BaseCard> Cards { get; set; } =
            new ObservableCollection<BaseCard>();

        public AddCards(
            double left,
            double top,
            double width,
            double height,
            WindowState state,
            int deckId
        )
        {
            InitializeComponent();

            this.DataContext = this;

            _httpClient = new HttpClient();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _deckId = deckId;

            getCards();
        }

        /// <summary>
        /// Loads all cards for the current deck for editing.
        /// </summary>
        private async void getCards()
        {

            var responseData = await FlashCards.db.GetCards.GetCardsFromDB(_deckId.ToString());


            var cardsData = System.Text.Json.JsonSerializer.Deserialize<
                List<Dictionary<string, object>>
            >(responseData.ToString());

            foreach (var item in cardsData)
            {
                if (item["type"].ToString() == "card")
                {
                    Cards.Add(
                        new Card
                        {
                            Question = item["question"].ToString(),
                            Answer = item["answer"].ToString(),
                        }
                    );
                }
                else if (item["type"].ToString() == "quiz")
                {
                    Cards.Add(
                        new QuizCard
                        {
                            Question = item["question"].ToString(),
                            Option1 = item["first_option"].ToString(),
                            Option2 = item["second_option"].ToString(),
                            Option3 = item["third_option"].ToString(),
                            Option4 = item["fourth_option"].ToString(),
                            CorrectIndex = item["correct_answer"].ToString(),
                        }
                    );
                }
            }

        }

        /// <summary>
        /// Creates a JSON string from the current card list for API submission.
        /// </summary>
        private string CreateJsonFromCards()
        {
            var cardsData = new
            {
                normalCards = Cards
                    .OfType<Card>()
                    .Select(c => new
                    {
                        question = c.Question,
                        answer = c.Answer,
                        is_fav = false,
                        status = "needs_practice",
                    })
                    .ToList(),
                quizCards = Cards
                    .OfType<QuizCard>()
                    .Select(q => new
                    {
                        question = q.Question,
                        option1 = q.Option1,
                        option2 = q.Option2,
                        option3 = q.Option3,
                        option4 = q.Option4,
                        correctIndex = q.CorrectIndex,
                        is_fav = false,
                        status = "needs_practice",
                    })
                    .ToList(),
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            return System.Text.Json.JsonSerializer.Serialize(cardsData, options);
        }

        /// <summary>
        /// Sends the current card list to the backend API.
        /// </summary>
        private void SendCards()
        {
            string json = CreateJsonFromCards();

            FlashCards.db.AddCards.AddCardsToDB(json, _user, _deckId.ToString(), "addCards");


        }

        private void CreateCardButton_Click(object sender, RoutedEventArgs e)
        {
            Cards.Add(new Card());
        }

        private void CreateQuizButton_Click(object sender, RoutedEventArgs e)
        {
            Cards.Add(new QuizCard());
        }

        private void RemoveCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BaseCard card)
                Cards.Remove(card);
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            SendCards();
            goHome();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goHome();
        }

        private void goHome()
        {
            var indexWindow = new Index(
                this.Left,
                this.Top,
                this.Width,
                this.Height,
                this.WindowState
            );
            indexWindow.Show();
            this.Close();
        }
    }
}
