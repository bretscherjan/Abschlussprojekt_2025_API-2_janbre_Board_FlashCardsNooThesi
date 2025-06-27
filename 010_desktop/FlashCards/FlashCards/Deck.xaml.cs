/**
 * Deck.xaml.cs
 *
 * Manages the display and interaction with cards in a specific deck, including learning and editing features.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace FlashCards
{
    public class CardList
    {
        public string title { get; set; }
        public string type { get; set; }
        public int? id { get; set; }
        public string question { get; set; }
        public string answer { get; set; }
        public int? correct_answer { get; set; }
        public string first_option { get; set; }
        public string second_option { get; set; }
        public string third_option { get; set; }
        public string fourth_option { get; set; }
        public int? is_fav { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
    }

    public partial class Deck : Window
    {
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _salt = Properties.Settings.Default.salt;
        private string _user = Properties.Settings.Default.username;
        private string _password = Properties.Settings.Default.password;
        private int _deckId;
        private List<CardList> allCards;
        private List<CardList> filteredCards;

        public Deck(
            double left,
            double top,
            double width,
            double height,
            WindowState state,
            int deckId
        )
        {
            InitializeComponent();
            getCards();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _deckId = deckId;
        }

        /// <summary>
        /// Toggles the favorite status of a card.
        /// </summary>
        private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                int cardId = (int)button.Tag;

                var card = allCards.FirstOrDefault(c => c.id == cardId);
                if (card != null)
                {
                    card.is_fav = card.is_fav == 1 ? 0 : 1;

                    this.DataContext = null;
                    this.DataContext = filteredCards;

                    await createTokenHash();

                    using (HttpClient requestClient = new HttpClient())
                    {
                        var responseData = await sendRequest.SendRequest(
                            requestClient,
                            "updateCardFavorite",
                            _user,
                            hashedToken,
                            sessionID,
                            _deckId.ToString(),
                            cardId.ToString(),
                            card.is_fav.ToString(),
                            card.type.ToString()
                        );

                        Console.WriteLine(responseData.message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating favorite: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a new authentication token hash.
        /// </summary>
        private async Task createTokenHash()
        {
            try
            {
                using (HttpClient tokenClient = new HttpClient())
                {
                    var responseToken = await getToken.GetTokenAsync(tokenClient);
                    token = responseToken.token;
                    sessionID = responseToken.sessionID;
                    Console.WriteLine($"Token: {token} \nSessionId: {sessionID}");
                }

                hashedToken = generateHash.GenerateSHA256Hash(token, _salt, _password);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads all cards for the current deck from the backend.
        /// </summary>
        private async void getCards()
        {
            try
            {
                await createTokenHash();

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(
                        requestClient,
                        "getCards",
                        _user,
                        hashedToken,
                        sessionID,
                        _deckId.ToString()
                    );

                    allCards = JsonConvert.DeserializeObject<List<CardList>>(
                        responseData.ToString()
                    );
                    filteredCards = new List<CardList>(allCards);
                    this.DataContext = filteredCards;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Opens the window to add a new card.
        /// </summary>
        private void AddCardButton_Click(object sender, RoutedEventArgs e)
        {
            var addCardWindow = new AddCards(
                this.Left,
                this.Top,
                this.Width,
                this.Height,
                this.WindowState,
                _deckId
            );
            addCardWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Opens the learning window for the current deck.
        /// </summary>
        private void LearnButton_Click(object sender, RoutedEventArgs e)
        {
            var learnWindow = new LearnDeck(
                this.Left,
                this.Top,
                this.Width,
                this.Height,
                this.WindowState,
                _deckId
            );
            learnWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Returns to the main index window.
        /// </summary>
        private void BackButton_Click(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Filters cards based on search input.
        /// </summary>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCards();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterCards();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            filteredCards = new List<CardList>(allCards);
            this.DataContext = filteredCards;
        }

        private void FilterCards()
        {
            if (allCards == null)
                return;

            string searchText = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                filteredCards = new List<CardList>(allCards);
            }
            else
            {
                filteredCards = allCards
                    .Where(card =>
                        card.question != null && card.question.ToLower().Contains(searchText)
                    )
                    .ToList();
            }
            this.DataContext = filteredCards;
        }
    }
}
