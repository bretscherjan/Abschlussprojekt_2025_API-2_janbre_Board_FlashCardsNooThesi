/**
 * sendRequest.cs
 *
 * Provides methods for sending various GET requests to the backend API for user and card operations.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FlashCards
{
    internal class sendRequest
    {
        private static string baseUrl = "https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php";

        /// <summary>
        /// Sends a GET request to the backend for various operations (overloaded for different parameters).
        /// </summary>
        internal static async Task<dynamic> SendRequest(HttpClient client, string request, string user, string token, string sessionID, string deckId)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&deckId={deckId}");

            return JsonConvert.DeserializeObject(response);
        }

        /// <summary>
        /// Sends a GET request to the backend for various operations (overloaded for different parameters).
        /// </summary>
        internal static async Task<dynamic> SendRequest(HttpClient client, string request, string user, string token, string sessionID, string deckId, string cardId, string is_fav, string type)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&deckId={deckId}&cardId={cardId}&isFav={is_fav}&type={type}");

            return JsonConvert.DeserializeObject(response);
        }

        /// <summary>
        /// Sends a request to add a follow relationship.
        /// </summary>
        internal static async Task<dynamic> AddFollow(HttpClient client, string request, string user, string token, string sessionID, string following)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&follow={following}");

            return JsonConvert.DeserializeObject(response);
        }

        /// <summary>
        /// Sends a request to delete a user.
        /// </summary>
        internal static async Task<dynamic> DeleteUser(HttpClient client, string request, string user, string token, string sessionID)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}");

            return JsonConvert.DeserializeObject(response);
        }
    }
}
