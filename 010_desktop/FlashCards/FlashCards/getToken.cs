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
    internal class getToken
    {
        private static string baseUrl = "https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php";

        public getToken()
        {

        }

        internal static async Task<dynamic> GetTokenAsync(HttpClient client)
        {
            var response = await client.GetStringAsync(
                $"{baseUrl}?action=getToken");

            return JsonConvert.DeserializeObject(response);
        }
    }
}
