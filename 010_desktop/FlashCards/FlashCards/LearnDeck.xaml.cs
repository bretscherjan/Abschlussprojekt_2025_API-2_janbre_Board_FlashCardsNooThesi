using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FlashCards
{


    public class TypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;
            string cardType = value.ToString();
            string expectedType = parameter.ToString();
            return cardType == expectedType ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class Cards
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

    public partial class LearnDeck : Window
    {
        private readonly List<Cards> allCards;
        private int currentCardIndex;
        private readonly List<bool> results = new List<bool>();
        private readonly int deckId;


        private string token;
        private string sessionID;
        private string hashedToken;
        private string _baseCode = "4gdrsh92z7";
        private string _user = "john_doe";
        private string _password = "password123";
        private int _deckId;


        public Cards CurrentCard { get; private set; }
        public int CurrentCardDisplayIndex => currentCardIndex + 1;
        public int TotalCards => allCards?.Count ?? 0;

        public LearnDeck(double left, double top, double width, double height, WindowState state, int deckId)
        {
            InitializeComponent();
            this.deckId = deckId;
            DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _deckId = deckId;

            allCards = LoadCards().Result;
            if (allCards?.Count > 0)
            {
                CurrentCard = allCards[0];
            }
        }

        private async Task<List<Cards>> LoadCards()
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
                    var responseData = await sendRequest.SendRequest(requestClient, "getCards", _user, hashedToken, sessionID, _deckId.ToString());

                    return JsonConvert.DeserializeObject<List<Cards>>(responseData.ToString());
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cards: {ex.Message}");
                return new List<Cards>();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrect = CurrentCard.type == "card"
                ? string.Equals(AnswerInput.Text?.Trim(), CurrentCard.answer?.Trim(), StringComparison.OrdinalIgnoreCase)
                : GetSelectedOption() == CurrentCard.correct_answer;

            results.Add(isCorrect);
            ShowFeedback(isCorrect);

            if (currentCardIndex + 1 < allCards.Count)
            {
                ShowNextCard();
            }
            else
            {
                ShowResults();
            }
        }

        private int GetSelectedOption()
        {
            if (Option1.IsChecked == true) return 1;
            if (Option2.IsChecked == true) return 2;
            if (Option3.IsChecked == true) return 3;
            return Option4.IsChecked == true ? 4 : 0;
        }

        private void ShowFeedback(bool isCorrect)
        {
            FeedbackText.Text = isCorrect ? "Correct!" : "Incorrect!";
            FeedbackText.Foreground = isCorrect ? Brushes.Green : Brushes.Red;
            FeedbackText.Visibility = Visibility.Visible;
        }

        private void ShowNextCard()
        {
            currentCardIndex++;
            CurrentCard = allCards[currentCardIndex];
            AnswerInput.Text = string.Empty;
            FeedbackText.Visibility = Visibility.Collapsed;
            ResetRadioButtons();
        }

        private void ResetRadioButtons()
        {
            Option1.IsChecked = false;
            Option2.IsChecked = false;
            Option3.IsChecked = false;
            Option4.IsChecked = false;
        }

        private void ShowResults()
        {
            var auswertungWindow = new AuswertungLernen(this.Left, this.Top, this.Width, this.Height, this.WindowState, results);
            auswertungWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var deckWindow = new Deck(this.Left, this.Top, this.Width, this.Height, this.WindowState, deckId);
            deckWindow.Show();
            this.Close();
        }
    }
}