/**
 * getToken.cs
 *
 * Provides methods to retrieve authentication tokens from the backend API.
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlashCards
{
    internal class getToken
    {
        private static string baseUrl =
            "https://jan-bretscher.ch/01_zli/FlashCards/databaseRequest.php";

        public getToken() { }

        /// <summary>
        /// Retrieves a new authentication token from the backend.
        /// </summary>
        internal static async Task<dynamic> GetTokenAsync(HttpClient client)
        {
            var response = await client.GetStringAsync($"{baseUrl}?action=getToken");

            return JsonConvert.DeserializeObject(response);
        }
    }
}
