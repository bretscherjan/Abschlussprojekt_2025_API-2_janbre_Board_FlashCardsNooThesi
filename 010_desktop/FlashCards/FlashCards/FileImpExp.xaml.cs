using Newtonsoft.Json;
using System;
using System.IO;
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

namespace FlashCards
{

    public class CardExportModel
    {
        public List<NormalCardExport> normalCards { get; set; }
        public List<QuizCardExport> quizCards { get; set; }
    }

    public class NormalCardExport
    {
        public string question { get; set; }
        public string answer { get; set; }
        public bool is_fav { get; set; }
        public string status { get; set; }
    }

    public class QuizCardExport
    {
        public string question { get; set; }
        public string option1 { get; set; }
        public string option2 { get; set; }
        public string option3 { get; set; }
        public string option4 { get; set; }
        public int correctIndex { get; set; }
        public bool is_fav { get; set; }
        public string status { get; set; }
    }



    public class CardsImpExp
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


    public partial class FileImpExp : Window
    {
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _salt = Properties.Settings.Default.salt;
        private string _user = Properties.Settings.Default.username;
        private string _password = Properties.Settings.Default.password;
        private string _deckId;
        private List<CardsImpExp> allCards;
        private List<CardsImpExp> filteredCards;

        private readonly HttpClient _httpClient;

        public FileImpExp(double left, double top, double width, double height, WindowState state, string deckId)
        {
            InitializeComponent();
            getCards();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _httpClient = new HttpClient();

            _deckId = deckId;
        }

        private async void getCards()
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

                    allCards = JsonConvert.DeserializeObject<List<CardsImpExp>>(responseData.ToString());
                    filteredCards = new List<CardsImpExp>(allCards);
                    this.DataContext = filteredCards;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private async Task SendImportedCards(string jsonContent)
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

                await postData.SendCardsAsync(_httpClient, "importCards", jsonContent, _user, hashedToken, sessionID, _deckId.ToString());

            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error sending data: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void importCards()
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "cards",
                DefaultExt = ".json",
                Filter = "All supported files (*.json;*.csv)|*.json;*.csv|JSON files (*.json)|*.json|CSV files (*.csv)|*.csv"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filePath = dialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath)?.ToLower();


                try
                {

                    string jsonContent = string.Empty;

                    if (extension == ".csv")
                    {
                        jsonContent = CardConverter.ConvertCsvToJson(filePath);
                    }

                    if (extension == ".json")
                    {

                        string filename = dialog.FileName;
                        jsonContent = File.ReadAllText(filename);
                    }

                    if (!string.IsNullOrEmpty(jsonContent))
                    {
                        await SendImportedCards(jsonContent);

                        getCards();

                    }
                    else
                    {
                        MessageBox.Show("No valid cards found in the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing cards: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string ConvertCardsToCsv()
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("type;question;correctAnswerIndex;answer;options;fav");

            foreach (var card in filteredCards)
            {
                string type = card.type;
                string question = EscapeCsvField(card.question ?? "");
                string correctAnswerIndex = card.correct_answer?.ToString() ?? "";
                string answer = EscapeCsvField(card.answer ?? "");
                string options = "";
                bool isFav = card.is_fav == 1;

                if (type == "quiz")
                {
                    options = EscapeCsvField($"{card.first_option ?? ""},{card.second_option ?? ""},{card.third_option ?? ""},{card.fourth_option ?? ""}");
                }

                csvBuilder.AppendLine($"{type};{question};{correctAnswerIndex};{answer};{options};{isFav.ToString().ToLower()}");
            }

            return csvBuilder.ToString();
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // Wenn das Feld ein Semikolon, Komma oder Anführungszeichen enthält, muss es in Anführungszeichen gesetzt werden
            if (field.Contains(";") || field.Contains(",") || field.Contains("\""))
            {
                // Anführungszeichen im Feld verdoppeln
                field = field.Replace("\"", "\"\"");
                return $"\"{field}\"";
            }

            return field;
        }


        private void exportCards()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"cards{_deckId}",
                DefaultExt = ".json",
                Filter = "All supported files (*.json;*.csv)|*.json;*.csv|JSON files (*.json)|*.json|CSV files (*.csv)|*.csv"
            };

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string filePath = dialog.FileName;
                string extension = System.IO.Path.GetExtension(filePath)?.ToLower();

                try
                {
                    if (extension == ".csv")
                    {
                        string csvContent = ConvertCardsToCsv();
                        File.WriteAllText(filePath, csvContent);
                    }
                    else if (extension == ".json")
                    {
                        // Filter normal cards (type != "quiz")
                        var normalCards = filteredCards
                            .Where(card => card.type != "quiz")
                            .Select(card => new NormalCardExport
                            {
                                question = card.question,
                                answer = card.answer,
                                is_fav = card.is_fav == 1,
                                status = card.status
                            })
                            .ToList();

                        // Filter quiz cards (type == "quiz")
                        var quizCards = filteredCards
                            .Where(card => card.type == "quiz")
                            .Select(card => new QuizCardExport
                            {
                                question = card.question,
                                option1 = card.first_option,
                                option2 = card.second_option,
                                option3 = card.third_option,
                                option4 = card.fourth_option,
                                correctIndex = card.correct_answer ?? 1,
                                is_fav = card.is_fav == 1,
                                status = card.status
                            })
                            .ToList();

                        // Create the export data structure
                        var exportData = new CardExportModel
                        {
                            normalCards = normalCards,
                            quizCards = quizCards
                        };

                        string jsonContent = JsonConvert.SerializeObject(exportData, Formatting.Indented);
                        File.WriteAllText(dialog.FileName, jsonContent);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting cards: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            importCards();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            exportCards();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var indexWindow = new Index(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            indexWindow.Show();
            this.Close();
        }

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
            filteredCards = new List<CardsImpExp>(allCards);
            this.DataContext = filteredCards;
        }

        private void FilterCards()
        {
            if (allCards == null) return;

            string searchText = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                filteredCards = new List<CardsImpExp>(allCards);
            }
            else
            {
                filteredCards = allCards
                    .Where(card => card.question != null && card.question.ToLower().Contains(searchText))
                    .ToList();
            }
            this.DataContext = filteredCards;
        }

    }
}