using Newtonsoft.Json;
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
using System.Windows.Threading;

namespace FlashCards
{
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
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _salt = Properties.Settings.Default.salt;
        private string _user = Properties.Settings.Default.username;
        private string _password = Properties.Settings.Default.password;
        private int _deckId;
        private List<Cards> allCards;
        private List<Cards> filteredCards;


        private int currentCardIndex = 0;
        private List<bool> results = new List<bool>();
        private Cards currentCard => filteredCards[currentCardIndex];

        public LearnDeck(double left, double top, double width, double height, WindowState state, int deckId)
        {
            InitializeComponent();

            InitializeBasicUI();

            LoadDataAndStartLearning();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;
            _deckId = deckId;

        }


        private void InitializeBasicUI()
        {
            LearningPanel.Visibility = Visibility.Visible;
            QuestionTextBlock.Text = "Lade Karten...";
        }

        private async void LoadDataAndStartLearning()
        {
            await getCards();

            if (filteredCards?.Count > 0)
            {
                LoadCurrentCard();
            }
            else
            {
                QuestionTextBlock.Text = "Keine Karten verfügbar";
            }
        }


        private async Task getCards()
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

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, "getCards", _user, hashedToken, sessionID, _deckId.ToString());

                    allCards = JsonConvert.DeserializeObject<List<Cards>>(responseData.ToString());
                    filteredCards = new List<Cards>(allCards);
                    this.DataContext = filteredCards;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void LoadCurrentCard()
        {
            if (filteredCards == null || filteredCards.Count == 0) return;

            QuestionTextBlock.Text = currentCard.question;

            if (currentCard.type == "card")
            {
                // Normale Karte
                QuizOptionsPanel.Visibility = Visibility.Collapsed;
                AnswerInputPanel.Visibility = Visibility.Visible;
                AnswerTextBox.Text = "";
            }
            else if (currentCard.type == "quiz")
            {
                // Quiz-Karte
                AnswerInputPanel.Visibility = Visibility.Collapsed;
                QuizOptionsPanel.Visibility = Visibility.Visible;

                // Setze die Quiz-Optionen
                Option1Button.Content = currentCard.first_option;
                Option2Button.Content = currentCard.second_option;
                Option3Button.Content = currentCard.third_option;
                Option4Button.Content = currentCard.fourth_option;

                // Zurücksetzen der Auswahl
                foreach (var child in QuizOptionsPanel.Children)
                {
                    if (child is Button button)
                    {
                        button.Background = new SolidColorBrush(Color.FromRgb(59, 59, 59));
                    }
                }
            }

            // Update Fortschrittsanzeige
            ProgressTextBlock.Text = $"{currentCardIndex + 1} / {filteredCards.Count}";
        }

        private void CheckAnswer(string userAnswer)
        {
            bool isCorrect = false;

            if (currentCard.type == "card")
            {
                isCorrect = userAnswer.Trim().Equals(currentCard.answer, StringComparison.OrdinalIgnoreCase);
            }
            else if (currentCard.type == "quiz")
            {
                int selectedOption = int.Parse(userAnswer);
                isCorrect = selectedOption == currentCard.correct_answer;
            }

            results.Add(isCorrect);

            if (currentCardIndex < filteredCards.Count - 1)
            {
                currentCardIndex++;
                LoadCurrentCard();
            }
            else
            {
                ShowEvaluation();
            }
        }

        private void ShowEvaluation()
        {
            // Übergibt die Ergebnisse UND die originalen Karten
            var auswertungWindow = new AuswertungLernen(this.Left, this.Top, this.Width, this.Height, this.WindowState, results, filteredCards, _deckId);
            auswertungWindow.Show();
            this.Close();
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentCard.type == "card")
            {
                CheckAnswer(AnswerTextBox.Text);
            }
        }

        private void QuizOptionButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var optionNumber = int.Parse((string)button.Tag);

            // Visuelles Feedback für die Auswahl
            foreach (var child in QuizOptionsPanel.Children)
            {
                if (child is Button btn)
                {
                    btn.Background = new SolidColorBrush(Color.FromRgb(59, 59, 59));
                }
            }
            button.Background = new SolidColorBrush(Color.FromRgb(223, 170, 48));

            // Nach kurzer Verzögerung die Antwort überprüfen
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                CheckAnswer(optionNumber.ToString());
            };
            timer.Start();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var deckWindow = new Deck(this.Left, this.Top, this.Width, this.Height, this.WindowState, _deckId);
            deckWindow.Show();
            this.Close();
        }


    }
}