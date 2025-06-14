using System;
using System.Text.RegularExpressions;

namespace FlashCards
{
    internal class removeHashtagColor
    {
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
