/**
 * generateHash.cs
 *
 * Contains methods for generating SHA256 hashes for authentication and password security.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
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
        /// <summary>
        /// Generates a SHA256 hash from input, base code, and password.
        /// </summary>
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

        /// <summary>
        /// Generates a SHA256 hash from a password.
        /// </summary>
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
