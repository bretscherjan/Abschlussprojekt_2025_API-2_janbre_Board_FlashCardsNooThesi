using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für Account.xaml
    /// </summary>
    /// 

    public class UserData
    {
        public string username {  get; set; }
        public string password { get; set; }
        public string email { get; set; }
    }

    public class UserFollow
    {
        public string username { get; set; }
    }

    public partial class Account : Window, INotifyPropertyChanged
    {
        private string token;
        private string sessionID;
        private string hashedToken;
        private string _salt = Properties.Settings.Default.salt;
        private string _OldUser = Properties.Settings.Default.username;
        private string _OldPassword = Properties.Settings.Default.password;
        private string _password;
        private string _user;

        private readonly HttpClient _httpClient;

        private string _username;
        private string _email;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private List<string> _benutzerNamenListe;
        public List<string> BenutzerNamenListe
        {
            get => _benutzerNamenListe;
            set
            {
                _benutzerNamenListe = value;
                OnPropertyChanged(nameof(BenutzerNamenListe));
            }
        }

        private string _selectedBenutzer;
        public string SelectedBenutzer
        {
            get => _selectedBenutzer;
            set
            {
                _selectedBenutzer = value;
                OnPropertyChanged(nameof(SelectedBenutzer));
            }
        }

        public Account(double left, double top, double width, double height, WindowState state)
        {
            InitializeComponent();
            DataContext = this;

            getUserData();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            _httpClient = new HttpClient();

            getUsers();

        }



        private async void getUsers()
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

                hashedToken = generateHash.GenerateSHA256Hash(token, _salt, _OldPassword);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, "getUsersFollow", _OldUser, hashedToken, sessionID, "0");

                    List<UserFollow> allUsers = JsonConvert.DeserializeObject<List<UserFollow>>(responseData.ToString());
                    BenutzerNamenListe = allUsers.Select(u => u.username).ToList();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private async void updateUser()
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

                hashedToken = generateHash.GenerateSHA256Hash(token, _salt, _OldPassword);
                Console.WriteLine($"Hashed Token + baseCode + password: {hashedToken}");

                string json = JsonConvert.SerializeObject(new
                {
                    username = _user,
                    email = _email,
                    password = _password,
                    OldPassword = _OldPassword,
                    OldUser = _OldUser
                });


                string response = await postData.UpdateUserAsync(_httpClient, json, hashedToken, sessionID);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response);

                if (result.success == true)
                {
                    System.Windows.MessageBox.Show("Updated successfully!", "Erfolg", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                    Properties.Settings.Default.Reset();
                    Properties.Settings.Default.Save();
                    var loginWindow = new Login();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    string errorMessage = result.error ?? "Unknown error occurred";
                    System.Windows.MessageBox.Show($"Update failed: {errorMessage}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private async void getUserData()
        {
            try
            {
                using (HttpClient tokenClient = new HttpClient())
                {
                    var responseToken = await getToken.GetTokenAsync(tokenClient);
                    token = responseToken.token;
                    sessionID = responseToken.sessionID;
                }

                hashedToken = generateHash.GenerateSHA256Hash(token, _salt, _OldPassword);

                using (HttpClient requestClient = new HttpClient())
                {
                    var responseData = await sendRequest.SendRequest(requestClient, "getUserCredentials", _OldUser, hashedToken, sessionID, "0");

                    if (responseData is JArray userArray && userArray.Count > 0)
                    {
                        var userData = userArray[0].ToObject<UserData>();
                        Username = userData.username;
                        Email = userData.email;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            string hashedOldPW = generateHash.GenerateSHA256Hash(PasswordBoxOld.Password);

            if (hashedOldPW == _OldPassword && PasswordBoxNew1.Password == PasswordBoxNew2.Password)
            {
                _password = generateHash.GenerateSHA256Hash(PasswordBoxNew1.Password);
            }
            else
            {
                MessageBox.Show("Die Passwörter stimmen nicht überein oder das alte Passwort wurde falsch eingegeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
            updateUser();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        private async void deleteAccout()
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
                    var responseData = await sendRequest.DeleteUser(requestClient, "deleteUser", _user, hashedToken, sessionID);

                    Console.WriteLine(responseData.toString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }



        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {

            deleteAccout();
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }


        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            var loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var indexWindow = new Index(this.Left, this.Top, this.Width, this.Height, this.WindowState);
            indexWindow.Show();
            this.Close();
        }

    }



}


