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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using MySql.Data;
using MySqlConnector;

namespace FlashCardsDesktop
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /*
        private string host = "herkules.letsbuild.ch";
        private string database = "FlashCards";
        private string username = "jan";
        private string password = "VEezZ85d";
        */

        private string host = "localhost";
        private string database = "FlashCards";
        private string username = "root";
        private string password = "1234";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            mysql_connection();
        }

        private void mysql_connection()
        {
            string connectionString = $"Server={host};Port=3306;Database={database};User ID={username};Password={password};SslMode=None";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
            }
        }
    }
}