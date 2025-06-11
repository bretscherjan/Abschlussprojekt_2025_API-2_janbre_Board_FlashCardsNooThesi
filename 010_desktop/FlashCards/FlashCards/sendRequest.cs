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

        internal static async Task<dynamic> SendRequest(HttpClient client, string request, string user, string token, string sessionID, int deckId)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&deckId={deckId}");

            return JsonConvert.DeserializeObject(response);
        }
    }
}
