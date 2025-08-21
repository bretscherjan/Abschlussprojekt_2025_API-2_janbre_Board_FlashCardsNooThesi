using Dsafa.WpfColorPicker;
using FlashCards.db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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

    public class  DeckData
    {
        public int id { get; set; }
        public string title { get; set; }
        public string alt { get; set; }
        public int is_private { get; set; }
        public string creator_id { get; set; }
        public DateTime created_at { get; set; }
        public string start_color { get; set; }
        public string end_color { get; set; }
        public string username { get; set; }
        public bool isHost { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für EditDeck.xaml
    /// </summary>
    public partial class EditDeck : Window, INotifyPropertyChanged
    {
        private string _user = Properties.Settings.Default.username;
        private int _deckId;
    
        private List<string> _benutzerNamenListe;

        public List<string> BenutzerNamenListe
        {
            get => _benutzerNamenListe;
            set
            {
                _benutzerNamenListe = value;
                OnPropertyChanged(nameof(BenutzerNamenListe));
            }
        }
    
        private string _selectedBenutzer;
        public string SelectedBenutzer
        {
            get => _selectedBenutzer;
            set
            {
                _selectedBenutzer = value;
                OnPropertyChanged(nameof(SelectedBenutzer));
            }
        }



        private string _title;
        public string DeckTitle
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(DeckTitle));
            }
        }

        private string _alt;

        public string Alt
        {
            get => _alt;
            set
            {
                _alt = value;
                OnPropertyChanged(nameof(Alt));
            }
        }

        private bool isHost;
        public bool IsHost
        {
            get => isHost;
            set
            {
                isHost = value;
                OnPropertyChanged(nameof(IsHost));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
        public EditDeck(double left, double top, double width, double height, WindowState state, int deckId)
        {
            InitializeComponent();
    
            this.DataContext = this;
    
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _deckId = deckId;

            getDeck();

        }
    
        public SolidColorBrush FirstColorBrush => new SolidColorBrush(FirstColor);
        public SolidColorBrush SecondColorBrush => new SolidColorBrush(SecondColor);
    
        /// <summary>
        /// Loads the list of possible collaborators for a new deck.
        /// </summary>
        private async void getUsers()
        {
            var responseData = await FlashCards.db.Follows.GetUsersFollowers(_user);
    
            List<UserCollaborator> allUsers = JsonConvert.DeserializeObject<List<UserCollaborator>>(responseData.ToString());
            BenutzerNamenListe = allUsers.Select(u => u.username).ToList();
    
        }
    
        private async void getDeck()
        {
            var responseData = await FlashCards.db.GetDeck.GetDeckById(_user, _deckId.ToString());

            if (!string.IsNullOrWhiteSpace(responseData))
            {
                var dataArray = JsonConvert.DeserializeObject<List<DeckData>>(responseData);

                if (dataArray != null && dataArray.Count > 0)       
                {
                    var deckData = dataArray[0];
                    DeckTitle = deckData.title.ToString();
                    Alt = deckData.alt.ToString();
                    FirstColor = (Color)ColorConverter.ConvertFromString(deckData.start_color);
                    SecondColor = (Color)ColorConverter.ConvertFromString(deckData.end_color);
                    SelectedBenutzer = deckData.username;
                    isHost = dataArray[1].isHost;
                }
            }


            if (isHost)
            {
                getUsers();
            } else
            {
                BenutzerNamenListe = new List<string>();
                BenutzerNamenListe.Add("You are not Admin of this deck");

            }
        }

        /// <summary>
        /// Creates a new deck with the specified properties and collaborators.
        /// </summary>
        private void SaveChanges()
        {
    
            if (string.IsNullOrWhiteSpace(title.Text))
            {
                MessageBox.Show(
                    "Bitte geben Sie einen Titel für das Deck ein.",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }
    
            FlashCards.db.EditDeck.UpdateDeckInDB(title.Text, alt.Text, FirstColor.ToString(), SecondColor.ToString(), _deckId, SelectedBenutzer);
    
            goHome();
    
        }
    
        public static readonly DependencyProperty FirstColorProperty =
            CreateDeck.FirstColorProperty.AddOwner(typeof(EditDeck));

        public static readonly DependencyProperty SecondColorProperty =
            CreateDeck.SecondColorProperty.AddOwner(typeof(EditDeck));
    
        public Color FirstColor
        {
            get => (Color)GetValue(FirstColorProperty);
            set => SetValue(FirstColorProperty, value);
        }
    
        public Color SecondColor
        {
            get => (Color)GetValue(SecondColorProperty);
            set => SetValue(SecondColorProperty, value);
        }
    
        private void PickFirstColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog(FirstColor);
            dialog.Owner = this;
            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                FirstColor = dialog.Color;
            }
        }
    
        private void PickSecondColorButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ColorPickerDialog(SecondColor);
            dialog.Owner = this;
            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                SecondColor = dialog.Color;
            }
        }
    
        private void CreateDeckHomeButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges();
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
    
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goHome();
        }

        private void DeleteCollaboratorButton_Click(object sender, RoutedEventArgs e)
        {

            if (isHost)
            {
                if (!FlashCards.db.EditDeck.DeleteCollaboratorsFromDeck(_deckId))
                {
                    MessageBox.Show(
                        "Keine Mitarbeiter ausgewählt.",
                        "Fehler",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }

                SelectedBenutzer = "";
            }
            else
            {
                MessageBox.Show(
                    "Sie sind nicht der Admin dieses Decks.",
                    "Fehler",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

            }
        }
    }
}
