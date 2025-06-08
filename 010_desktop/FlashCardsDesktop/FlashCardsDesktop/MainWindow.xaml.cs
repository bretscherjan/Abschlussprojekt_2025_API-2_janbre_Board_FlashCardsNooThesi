using MySqlConnector;
using System;
using System.Windows;

namespace FlashCardsDesktop
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /*
        private string host = "127.0.0.1";
        private string database = "FlashCards";
        private string username = "root";
        private string password = "1234";
        */
        private string host = "herkules.net.letsbuild.ch";
        private string database = "FlashCards";
        private string username = "jan";
        private string password = "VEezZ85d";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = $"Server={host};Port=3306;Database={database};Uid={username};Pwd={password};Connection Timeout=30;";
            Execute(connectionString, "SELECT * FROM users;");
        }

        public static void Execute(string connectionString, string sqlCommand, MySqlConnector.MySqlConnection connection = null)
        {
            try
            {
                if (connection == null)
                {
                    connection = new MySqlConnection(connectionString);
                    Console.WriteLine("New connection created.");
                }

                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                    Console.WriteLine("Connection opened.");
                }

                MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);

                Console.WriteLine(sqlCommand + "\n" + cmd);

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"MySQL Exception: {ex.Message}");
            }
            finally
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}