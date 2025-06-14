using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace FlashCards
{
    public class DeckList
    {
        public int id { get; set; }
        public string title { get; set; }
        public string alt { get; set; }
        public int is_private { get; set; }
        public DateTime created_at { get; set; }
        public string start_color { get; set; }
        public string end_color { get; set; }
    }

    public partial class Index : Window
    {
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _baseCode = "4gdrsh92z7";
        private string _user = "john_doe";
        private string _password = "password123";
        // private string _request = "getDecks";
        private List<DeckList> allDecks;
        private List<DeckList> filteredDecks;

        public Index() : this(100, 100, 800, 450, WindowState.Normal)
        {
            InitializeComponent();
            getDecks();
        }

        public Index(double left, double top, double width, double height, WindowState state)
        {
            InitializeComponent();
            getDecks();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;
        }

        private async void getDecks()
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

                hashedToken = generateHash.GenerateSHA256Hash(token, _baseCode, _password);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, "getDecks", _user, hashedToken, sessionID, "0");

                    allDecks = JsonConvert.DeserializeObject<List<DeckList>>(responseData.ToString());
                    filteredDecks = new List<DeckList>(allDecks);
                    this.DataContext = filteredDecks;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async void deleteDeck(string _deckId)
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

                hashedToken = generateHash.GenerateSHA256Hash(token, _baseCode, _password);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, "deleteDeck", _user, hashedToken, sessionID, _deckId);

                    Console.WriteLine(responseData.toString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DeckButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int deckId)
            {
                var deckWindow = new Deck(this.Left, this.Top, this.Width, this.Height, this.WindowState, deckId);
                deckWindow.Show();
                this.Close();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterDecks();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            FilterDecks();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            filteredDecks = new List<DeckList>(allDecks);
            this.DataContext = filteredDecks;
        }

        private void FilterDecks()
        {
            if (allDecks == null) return;

            string searchText = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                filteredDecks = new List<DeckList>(allDecks);
            }
            else
            {
                filteredDecks = allDecks
                    .Where(deck => deck.title.ToLower().Contains(searchText))
                    .ToList();
            }
            this.DataContext = filteredDecks;
        }

        private void DeleteDeck_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var deckId = menuItem?.Tag?.ToString();

            if (!string.IsNullOrEmpty(deckId))
            {
                var result = MessageBox.Show(
                    "Sind Sie sicher, dass Sie dieses Deck löschen möchten?",
                    "Deck löschen",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    deleteDeck(deckId);
                    getDecks();
                }
            }
        }

        private void AddDeckButton_Click(object sender, RoutedEventArgs e)
        {
            var createDeckWindow = new CreateDeck(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            createDeckWindow.Show();
            this.Close();

        }
    }
}