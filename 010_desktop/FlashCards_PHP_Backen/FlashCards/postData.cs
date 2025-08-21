/**
 * postData.cs
 *
 * Handles HTTP POST requests for sending and updating user and card data to the backend API.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
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
        /// <summary>
        /// Sends card data to the backend via HTTP POST.
        /// </summary>
        public static async Task SendCardsAsync(
            HttpClient httpClient,
            string request,
            string jsonData,
            string user,
            string token,
            string sessionID,
            string deckId
        )
        {
            string baseUrl =
                $"https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&deckId={deckId}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseBody}");
        }

        /// <summary>
        /// Sends new user registration data to the backend.
        /// </summary>
        public static async Task SendNewUserAsync(
            HttpClient httpClient,
            string jsonData,
            string token,
            string sessionID
        )
        {
            string baseUrl =
                $"https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php?action=createUser&token={token}&sessionID={sessionID}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseBody}");
        }

        /// <summary>
        /// Sends updated user data to the backend and returns the response.
        /// </summary>
        public static async Task<string> UpdateUserAsync(
            HttpClient httpClient,
            string jsonData,
            string token,
            string sessionID
        )
        {
            string baseUrl =
                $"https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php?action=updateUser&token={token}&sessionID={sessionID}";

            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(baseUrl, content);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {responseBody}");

            return responseBody;
        }
    }
}
