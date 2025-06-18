using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FlashCards
{
    public partial class AuswertungLernen : Window
    {
        public int CorrectCount { get; }
        public int IncorrectCount { get; }
        public double Percentage { get; }

        public AuswertungLernen(double left, double top, double width, double height, WindowState state, List<bool> results)
        {
            InitializeComponent();
            DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            CorrectCount = results.Count(r => r);
            IncorrectCount = results.Count(r => !r);
            Percentage = results.Count > 0 ? (CorrectCount / (double)results.Count) * 100 : 0;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            new Deck(this.Left, this.Top, this.Width, this.Height, this.WindowState, 0).Show();
            this.Close();
        }
    }
}