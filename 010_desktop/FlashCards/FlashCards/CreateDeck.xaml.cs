using Dsafa.WpfColorPicker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    /// <summary>
    /// Interaktionslogik für CreateDeck.xaml
    /// </summary>
    public partial class CreateDeck : Window
    {

        private string token;
        private string sessionID;
        private string hashedToken;
        private string _baseCode = "4gdrsh92z7";
        private string _user = "john_doe";
        private string _password = "password123";

        public CreateDeck(double left, double top, double width, double height, WindowState state)
        {
            InitializeComponent();

            this.DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;
        }

        public SolidColorBrush FirstColorBrush => new SolidColorBrush(FirstColor);
        public SolidColorBrush SecondColorBrush => new SolidColorBrush(SecondColor);


        private async void createDeck()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title.Text))
                {
                    MessageBox.Show("Bitte geben Sie einen Titel für das Deck ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
                    var responseData = await addDeck.AddDeck(requestClient, "addDeck", _user, hashedToken, sessionID, FirstColor.ToString(), SecondColor.ToString(), title.Text, alt.Text);

                    Console.WriteLine(responseData.toString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static readonly DependencyProperty FirstColorProperty =
            DependencyProperty.Register(nameof(FirstColor), typeof(Color), typeof(CreateDeck), new PropertyMetadata(Colors.BlueViolet));

        public static readonly DependencyProperty SecondColorProperty =
            DependencyProperty.Register(nameof(SecondColor), typeof(Color), typeof(CreateDeck), new PropertyMetadata(Colors.Red));

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
            goHome();
        }

        private void CreateDeckAddCardsButton_Click(object sender, RoutedEventArgs e)
        {

            createDeck();
            // var createCard = new CreateCard(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            // createCard.Show();
            // this.Close();
        }

        private void goHome()
        {
            var indexWindow = new Index(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            indexWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goHome();
        }
    }
}
