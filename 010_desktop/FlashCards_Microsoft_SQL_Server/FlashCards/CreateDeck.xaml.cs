/**
 * CreateDeck.xaml.cs
 *
 * Provides functionality for creating new decks, including color selection and collaborator management.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Dsafa.WpfColorPicker;
using Newtonsoft.Json;
using FlashCards.db;
using Newtonsoft.Json.Linq;

namespace FlashCards
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
                return new SolidColorBrush(color);
            return Brushes.Transparent;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }

    public class UserCollaborator
    {
        public string username { get; set; }
    }

    /// <summary>
    /// Interaktionslogik für CreateDeck.xaml
    /// </summary>
    public partial class CreateDeck : Window, INotifyPropertyChanged
    {
        private string _user = Properties.Settings.Default.username;

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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public CreateDeck(double left, double top, double width, double height, WindowState state)
        {
            InitializeComponent();

            this.DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            getUsers();
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

        /// <summary>
        /// Creates a new deck with the specified properties and collaborators.
        /// </summary>
        private void createDeck()
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

            FlashCards.db.CreateDeck.AddDeck(_user, FirstColor.ToString(), SecondColor.ToString(), title.Text, alt.Text, SelectedBenutzer);

            goHome();

        }

        public static readonly DependencyProperty FirstColorProperty = DependencyProperty.Register(
            nameof(FirstColor),
            typeof(Color),
            typeof(CreateDeck),
            new PropertyMetadata(Colors.BlueViolet)
        );

        public static readonly DependencyProperty SecondColorProperty = DependencyProperty.Register(
            nameof(SecondColor),
            typeof(Color),
            typeof(CreateDeck),
            new PropertyMetadata(Colors.Red)
        );

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
            createDeck();
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
    }
}
