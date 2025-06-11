using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für Deck.xaml
    /// </summary>
    public partial class Deck : Window
    {

        private string token;
        private string sessionID;
        private string hashedToken;
        private string _baseCode = "4gdrsh92z7";

        // test user
        private string _user = "user1";
        private string _password = "password1";
        private string _request = "getCards";

        private int _deckId;

        public Deck(double left, double top, double width, double height, WindowState state, int deckId)
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

                hashedToken = generateHash.GenerateSHA256Hash(token, _baseCode, _password);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, _request, _user, hashedToken, sessionID, _deckId);

                    List<DataItem> cards = JsonConvert.DeserializeObject<List<DataItem>>(responseData.ToString());

                    this.DataContext = cards;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
