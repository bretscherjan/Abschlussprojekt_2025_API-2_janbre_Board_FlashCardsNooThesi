/**
 * Account.xaml.cs
 *
 * Handles user account management, including profile data, followers, and account actions.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using FlashCards.db;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;


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

        private string _OldUser = Properties.Settings.Default.username;
        private string _OldPassword = Properties.Settings.Default.password;
        private string _password;
        private string _user;


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
        private List<string> _followersListe;    
        private List<string> _followingListe;    

        public List<string> BenutzerNamenListe
        {
            get => _benutzerNamenListe;
            set
            {
                _benutzerNamenListe = value;
                OnPropertyChanged(nameof(BenutzerNamenListe));
            }
        }

        public List<string> FollowersListe
        {
            get => _followersListe;
            set
            {
                _followersListe = value;
                OnPropertyChanged(nameof(FollowersListe));
            }
        }

        public List<string> FollowingListe
        {
            get => _followingListe;
            set
            {
                _followingListe = value;
                OnPropertyChanged(nameof(FollowingListe));
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

            _benutzerNamenListe = new List<string>();
            _followersListe = new List<string>();
            _followingListe = new List<string>();

            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.WindowState = state;

            getNotFollowing();
            getFollowers();
            getFollowing();

        }

        /// <summary>
        /// Loads the list of users that the current user does not follow yet.
        /// </summary>
        private async void getNotFollowing()
        {

            var responseData = await FlashCards.db.Follows.GetUsersNotFollowing(_OldUser);


            List<UserFollow> allUsers = JsonConvert.DeserializeObject<List<UserFollow>>(responseData.ToString());
            BenutzerNamenListe = allUsers.Select(u => u.username).ToList();

        }

        /// <summary>
        /// Loads the list of users who follow the current user.
        /// </summary>
        private async void getFollowers()
        {

            var responseData = await FlashCards.db.Follows.GetUsersFollowers(_OldUser);

            List<UserFollow> allUsers = JsonConvert.DeserializeObject<List<UserFollow>>(responseData.ToString());
            FollowersListe = allUsers.Select(u => u.username).ToList();

        }

        /// <summary>
        /// Loads the list of users the current user is following.
        /// </summary>
        private async void getFollowing()
        {

            var responseData = await FlashCards.db.Follows.GetUsersFollowing(_OldUser);

            List<UserFollow> allUsers = JsonConvert.DeserializeObject<List<UserFollow>>(responseData.ToString());
            FollowingListe = allUsers.Select(u => u.username).ToList();

        }

        
        /// <summary>
        /// Adds a new follow relationship to another user.
        /// </summary>
        private void addFollow()
        {

            Follows.Follow(_OldUser, SelectedBenutzer);

            getNotFollowing();
            getFollowers();
            getFollowing();

        }

        /// <summary>
        /// Delete a follow relationship to another user.
        /// </summary>
        private void unfollow(string benutzer)
        {

            Follows.Unfollow(_OldUser, benutzer);


            getNotFollowing();
            getFollowers();
            getFollowing();
        }


        /// <summary>
        /// Updates the user profile data (username, email, password) via API.
        /// </summary>
        private void updateUser()
        {

            bool success = UpdateUser.UpdateuserInDB(_user, _email, _password, _OldUser);


            if (success)
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
                string errorMessage = "Unknown error occurred";
                System.Windows.MessageBox.Show($"Update failed: {errorMessage}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads the current user's profile data from the backend.
        /// </summary>
        private async void getUserData()
        {
            var responseData = await FlashCards.db.GetUserData.GetUsersCredentials(_OldUser);

            if (!string.IsNullOrWhiteSpace(responseData))
            {
                var userArray = JsonConvert.DeserializeObject<List<UserData>>(responseData);

                if (userArray != null && userArray.Count > 0)
                {
                    var userData = userArray[0];
                    Username = userData.username;
                    Email = userData.email;
                }
            }


        }

        private void SavePassword_Click(object sender, RoutedEventArgs e)
        {

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            string hashedOldPW = GenerateHash.GenerateSHA256Hash(PasswordBoxOld.Password);

            if (hashedOldPW == _OldPassword && PasswordBoxNew1.Password == PasswordBoxNew2.Password)
            {
                _password = GenerateHash.GenerateSHA256Hash(PasswordBoxNew1.Password);
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


        private void SaveFollow_Click(object sender, RoutedEventArgs e)
        {
            addFollow();
        }

        private void UnfollowUser_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var benutzername = menuItem?.Tag?.ToString();

            if (!string.IsNullOrEmpty(benutzername))
            {
                unfollow(benutzername);

            }

        }



        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {

            DeleteUser.DeleteUserFromDB(_OldUser);
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


