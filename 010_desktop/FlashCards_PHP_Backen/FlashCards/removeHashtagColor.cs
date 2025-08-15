/**
 * removeHashtagColor.cs
 *
 * Utility for extracting hexadecimal color codes from color strings for deck customization.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System;
using System.Text.RegularExpressions;

namespace FlashCards
{
    internal class removeHashtagColor
    {
        /// <summary>
        /// Extracts the hexadecimal color code from a color string.
        /// </summary>
        public static string RemoveHashtagColor(string color)
        {
            string pattern = @"#([0-9a-fA-F]{8})";

            Match match = Regex.Match(color, pattern);
            if (match.Success)
            {
                string hexColor = match.Groups[1].Value;
                Console.WriteLine(hexColor);
                return hexColor;
            }

            Console.WriteLine("Keine gültige Farbe gefunden.");
            return "";
        }
    }
}
