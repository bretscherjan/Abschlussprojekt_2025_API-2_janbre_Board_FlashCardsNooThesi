/**
 * Login.xaml.cs
 *
 * Handles user authentication, login logic, and navigation to registration or main index.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System.Windows;
using FlashCards.db;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string _password;
        private string _user;
        // private string _salt = Properties.Settings.Default.salt;
        private bool _isLoggedIn = Properties.Settings.Default.isLogedIn;


        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Verifies the user's credentials and logs in if successful.
        /// </summary>
        private void verifyAccount()
        {

            HandleLogin.Login(_user, _password);

            _isLoggedIn = Properties.Settings.Default.isLogedIn;

            if (_isLoggedIn)
            {
                var indexPage = new Index();
                indexPage.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed! Please check your credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        /// <summary>
        /// Opens the registration window.
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new Register();
            registerWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Initiates the login process.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _password = GenerateHash.GenerateSHA256Hash(password.Password);
            _user = username.Text;
            verifyAccount();
        }
    }
}
