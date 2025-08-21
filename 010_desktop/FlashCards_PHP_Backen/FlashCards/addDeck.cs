/**
 * addDeck.cs
 *
 * Provides methods to add new decks to the backend via API requests.
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
using System.Windows.Controls;
using Newtonsoft.Json;

namespace FlashCards
{
    internal class addDeck
    {
        private static string baseUrl =
            "https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php";

        /// <summary>
        /// Sends a request to add a new deck to the backend.
        /// </summary>
        internal static async Task<dynamic> AddDeck(
            HttpClient client,
            string request,
            string user,
            string token,
            string sessionID,
            string startColor,
            string endColor,
            string title,
            string alt,
            string collaborator
        )
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&startColor={startColor}&endColor={endColor}&title={title}&alt={alt}&collaborator={collaborator}"
            );

            return JsonConvert.DeserializeObject(response);
        }
    }
}
