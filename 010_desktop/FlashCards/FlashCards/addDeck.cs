using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FlashCards
{
    internal class addDeck
    {

        private static string baseUrl = "https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php";

        internal static async Task<dynamic> AddDeck(HttpClient client, string request, string user, string token, string sessionID, string startColor, string endColor, string title, string alt)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getData&request={request}&user={user}&token={token}&sessionID={sessionID}&startColor={startColor}&endColor={endColor}&title={title}&alt={alt}");

            return JsonConvert.DeserializeObject(response);
        }

    }
}
