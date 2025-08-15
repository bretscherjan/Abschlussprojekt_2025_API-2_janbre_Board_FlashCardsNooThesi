/**
 * Index.xaml.cs
 *
 * Displays the main dashboard with deck overview, search, and navigation to deck details.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
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
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Newtonsoft.Json;
using System.Data.SqlClient;
using FlashCards.db;

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
        private string _user = Properties.Settings.Default.username;

        // private string _request = "getDecks";
        private List<DeckList> allDecks;
        private List<DeckList> filteredDecks;

        public Index()
            : this(100, 100, 800, 450, WindowState.Normal)
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

        /// <summary>
        /// Loads all decks for the current user.
        /// </summary>
        private async void getDecks()
        {
            var responseData = await FlashCards.db.GetDecks.GetDecksFromDB(_user);

            allDecks = JsonConvert.DeserializeObject<List<DeckList>>(responseData);
            filteredDecks = new List<DeckList>(allDecks);
            this.DataContext = filteredDecks;
        }


        
        /// <summary>
        /// Deletes a deck by its ID.
        /// </summary>
        private void deleteDeck(string _deckId)
        {

            DeleteDeck.DeleteDeckFromDB(_deckId);
        }
        

        /// <summary>
        /// Opens the import/export window for a deck.
        /// </summary>
        private void ImpExpCardsDeck_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var deckId = menuItem?.Tag?.ToString();

            if (!string.IsNullOrEmpty(deckId))
            {
                var fileImpExpWindow = new FileImpExp(
                    this.Left,
                    this.Top,
                    this.Width,
                    this.Height,
                    this.WindowState,
                    deckId
                );
                fileImpExpWindow.Show();
                this.Close();
            }
        }

        /// <summary>
        /// Opens the deck details window.
        /// </summary>
        private void DeckButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int deckId)
            {
                var deckWindow = new Deck(
                    this.Left,
                    this.Top,
                    this.Width,
                    this.Height,
                    this.WindowState,
                    deckId
                );
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

        /// <summary>
        /// Filters the deck list based on search input.
        /// </summary>
        private void FilterDecks()
        {
            if (allDecks == null)
                return;

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
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    deleteDeck(deckId);
                    getDecks();
                }
            }
        }

        private void AddDeckButton_Click(object sender, RoutedEventArgs e)
        {
            var createDeckWindow = new CreateDeck(
                this.Left,
                this.Top,
                this.Width,
                this.Height,
                this.WindowState
            );
            createDeckWindow.Show();
            this.Close();
        }

        private void AcoountButton_Click(object sender, RoutedEventArgs e)
        {
            var accountWindow = new Account(
                this.Left,
                this.Top,
                this.Width,
                this.Height,
                this.WindowState
            );
            accountWindow.Show();
            this.Close();
        }

        /*private void EditDeck_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var deckId = menuItem?.Tag?.ToString();
            if (!string.IsNullOrEmpty(deckId))
            {
                var editDeckWindow = new EditDeck(this.Left, this.Top, this.Width, this.Height, this.WindowState, int.Parse(deckId));
                editDeckWindow.Show();
                this.Close();
            }
        }*/
    }
}
