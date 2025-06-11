/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Index Page (all decks are list here)
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/////////////////////////////////////////////
/// token + baseCode + password -> SHA256 Hash (Authentication)
/////////////////////////////////////////////

namespace FlashCards
{


    public class DataItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string alt { get; set; }
        public int is_private { get; set; }
        public DateTime created_at { get; set; }
        public string start_color { get; set; }
        public string end_color { get; set; }
    }





    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _baseCode = "4gdrsh92z7";

        // test user
        private string _user = "user1";
        private string _password = "password1";
        private string _request = "getDecks";

        public MainWindow()
        {
            InitializeComponent();
            getDecks();
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
                    var responseData = await sendRequest.SendRequest(requestClient, _request, _user, hashedToken, sessionID, 0);

                    List<DataItem> decks = JsonConvert.DeserializeObject<List<DataItem>>(responseData.ToString());

                    this.DataContext = decks;
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

    }
}





