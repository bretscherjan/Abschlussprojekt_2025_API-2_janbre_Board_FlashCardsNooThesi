using Newtonsoft.Json;
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
        private string _salt = Properties.Settings.Default.salt;
        private string token;
        private string sessionID;
        private string hashedToken;

        private readonly HttpClient _httpClient;
        public Register()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void registerUser()
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

                string json = JsonConvert.SerializeObject(new
                {
                    username = _user,
                    email = _email,
                    password = _password
                });


                await postData.SendNewUserAsync(_httpClient, json, hashedToken, sessionID);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (first_pw.Password == second_pw.Password)
            {
                _password = generateHash.GenerateSHA256Hash(first_pw.Password);
                _email = email.Text;
                _user = username.Text;
                registerUser();
            }
            else
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}
