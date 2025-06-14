using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für AddCards.xaml
    /// </summary>
    public partial class AddCards : Window
    {
        public AddCards(double left, double top, double width, double height, WindowState state)
        {
            InitializeComponent();

            this.DataContext = this;

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;
        }

        private void CreateCardButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateQuizButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {


            goHome();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            goHome();
        }

        private void goHome()
        {
            var indexWindow = new Index(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            indexWindow.Show();
            this.Close();
        }
    }
}
