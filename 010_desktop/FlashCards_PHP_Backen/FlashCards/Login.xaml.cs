/**
 * Login.xaml.cs
 *
 * Handles user authentication, login logic, and navigation to registration or main index.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private string _password;
        private string _user;
        private string _salt = Properties.Settings.Default.salt;
        private string token;
        private string sessionID;
        private string hashedToken;

        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Verifies the user's credentials and logs in if successful.
        /// </summary>
        private async void verifyAccount()
        {
            try
            {
                using (HttpClient tokenClient = new HttpClient())
                {
                    var responseToken = await getToken.GetTokenAsync(tokenClient);
                    token = responseToken.token;
                    sessionID = responseToken.sessionID;
                    Console.WriteLine($"Token: {token} \nSessionId: {sessionID}");
                }

                hashedToken = generateHash.GenerateSHA256Hash(token, _salt, _password);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(
                        requestClient,
                        "verifyAccount",
                        _user,
                        hashedToken,
                        sessionID,
                        "0"
                    );

                    if (responseData is bool && (bool)responseData == true)
                    {
                        Properties.Settings.Default.username = _user;
                        Properties.Settings.Default.password = _password;
                        Properties.Settings.Default.LastLoginDate = DateTime.Now;
                        Properties.Settings.Default.Save();
                        var indexWindow = new Index();
                        indexWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                            "Login fehlgeschlagen. Bitte überprüfen Sie Ihre Anmeldedaten.",
                            "Fehler",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
            _password = generateHash.GenerateSHA256Hash(password.Password);
            _user = username.Text;
            verifyAccount();
        }
    }
}
