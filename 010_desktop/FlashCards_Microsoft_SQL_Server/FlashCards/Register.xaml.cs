using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using FlashCards.db;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für register.xaml
    /// </summary>
    public partial class Register : Window
    {

        private string _password;
        private string _email;
        private string _user;
        // private string _salt = Properties.Settings.Default.salt;


        private readonly HttpClient _httpClient;
        public Register()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Registers a new user with the provided credentials.
        /// </summary>
        private void registerUser()
        {

            bool success = HandleRegistration.Registration(_user, _email, _password);

            if (success)
            {
                MessageBox.Show("registered successfully!", "Erfolg", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                var loginWindow = new Login();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed! Please check your credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        /// <summary>
        /// Handles the registration button click and validates input.
        /// </summary>
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (first_pw.Password == second_pw.Password)
            {
                // _password = generateHash.GenerateSHA256Hash(first_pw.Password);
                _password = first_pw.Password;
            }
            else
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            if (Regex.IsMatch(email.Text, emailPattern))
            {
                _email = email.Text;
            }
            else
            {
                MessageBox.Show("Die E-Mail ist ungültig", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _user = username.Text;
            registerUser();
        }

        /// <summary>
        /// Switches to the login window.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
