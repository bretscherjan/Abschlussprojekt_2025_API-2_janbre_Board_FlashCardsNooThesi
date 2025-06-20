using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace FlashCards
{
    public static class postData
    {



        public static async Task SendCardsAsync(HttpClient httpClient, string request, string jsonData, string user, string token, string sessionID, string deckId)
        {
            string baseUrl = $"https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&deckId={deckId}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseBody}");
        }

        public static async Task SendNewUserAsync(HttpClient httpClient, string jsonData, string token, string sessionID)
        {
            string baseUrl = $"https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php?action=createUser&token={token}&sessionID={sessionID}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseBody}");

            MessageBox.Show(responseBody);
        }
    }
}