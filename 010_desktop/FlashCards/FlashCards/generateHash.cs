using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace FlashCards
{
    internal class generateHash
    {

        public static string GenerateSHA256Hash(string input, string baseCode, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                string fullInput = input + baseCode + password;

                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fullInput));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateSHA256Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
