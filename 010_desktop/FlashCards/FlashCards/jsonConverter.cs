using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace FlashCards
{


    public class CardConverter
    {
        public static string ConvertCsvToJson(string csvPath)
        {
            var lines = File.ReadAllLines(csvPath);
            var normalCards = new List<NormalCard>();
            var quizCards = new List<QuizCard>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';');
                if (parts.Length < 6) continue;

                string type = parts[0].Trim();
                string question = parts[1].Trim();
                string correctIndexStr = parts[2].Trim();
                string answer = parts[3].Trim();
                string options = parts[4].Trim();
                bool isFav = parts[5].Trim().ToLower() == "true";

                if (type == "normalCards")
                {
                    normalCards.Add(new NormalCard
                    {
                        question = question,
                        answer = answer,
                        is_fav = isFav,
                        status = "needs_practice"
                    });
                }
                else if (type == "quizCards")
                {
                    var opts = options.Split(',').Select(o => o.Trim()).ToArray();
                    if (opts.Length < 4) continue; // ensure 4 options
                    int correctIndex = int.TryParse(correctIndexStr, out var i) ? i : 0;

                    quizCards.Add(new QuizCard
                    {
                        question = question,
                        option1 = opts[0],
                        option2 = opts[1],
                        option3 = opts[2],
                        option4 = opts[3],
                        correctIndex = correctIndex,
                        is_fav = isFav,
                        status = "needs_practice"
                    });
                }
            }

            var result = new
            {
                normalCards,
                quizCards
            };

            return JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        }


        public class NormalCard
        {
            public string question { get; set; }
            public string answer { get; set; }
            public bool is_fav { get; set; }
            public string status { get; set; }
        }

        public class QuizCard
        {
            public string question { get; set; }
            public string option1 { get; set; }
            public string option2 { get; set; }
            public string option3 { get; set; }
            public string option4 { get; set; }
            public int correctIndex { get; set; }
            public bool is_fav { get; set; }
            public string status { get; set; }
        }
    }

}
