using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace FlashCards
{


    public class CardResult
    {
        public string Question { get; set; }
        public string CorrectAnswer { get; set; }
        public bool WasCorrect { get; set; }
    }


    public partial class AuswertungLernen : Window
    {
        public int CorrectCount { get; }
        public int IncorrectCount { get; }
        public double Percentage { get; }

        public List<CardResult> CardResults { get; }

        public int _deckId;

        public AuswertungLernen(double left, double top, double width, double height, WindowState state, List<bool> results, List<Cards> cards, int deckId)
        {
            InitializeComponent();
            DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _deckId = deckId;

            CorrectCount = results.Count(r => r);
            IncorrectCount = results.Count(r => !r);
            Percentage = results.Count > 0 ? (CorrectCount / (double)results.Count) * 100 : 0;


            CardResults = new List<CardResult>();
            for (int i = 0; i < results.Count; i++)
            {
                CardResults.Add(new CardResult
                {
                    Question = cards[i].question,
                    CorrectAnswer = GetCorrectAnswer(cards[i]),
                    WasCorrect = results[i]
                });
            }

            DataContext = this;

        }


        private string GetCorrectAnswer(Cards card)
        {
            return card.type == "card"
                ? card.answer
                : GetQuizAnswer(card);
        }

        private string GetQuizAnswer(Cards card)
        {
            switch (card.correct_answer)
            {
                case 1:
                    return card.first_option;
                case 2:
                    return card.second_option;
                case 3:
                    return card.third_option;
                case 4:
                    return card.fourth_option;
                default:
                    return "Unbekannte Antwort";
            }
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new Deck(this.Left, this.Top, this.Width, this.Height, this.WindowState, _deckId).Show();
            this.Close();
        }
    }
}