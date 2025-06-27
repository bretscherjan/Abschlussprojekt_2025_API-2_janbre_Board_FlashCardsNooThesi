/**
 * App.xaml.cs
 *
 * Application entry point and startup logic for the FlashCards WPF application.
 *
 * Author: Jan Bretscher
 * Created: June 27, 2025
 * Version: 3.3
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using FlashCards.Properties;

namespace FlashCards
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string username = FlashCards.Properties.Settings.Default.username;
            string password = FlashCards.Properties.Settings.Default.password;
            DateTime lastLoginDate = FlashCards.Properties.Settings.Default.LastLoginDate;

            Window startWindow;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                if (
                    lastLoginDate != DateTime.MinValue
                    && (DateTime.Now - lastLoginDate).TotalDays > 30
                )
                {
                    FlashCards.Properties.Settings.Default.username = string.Empty;
                    FlashCards.Properties.Settings.Default.password = string.Empty;
                    FlashCards.Properties.Settings.Default.LastLoginDate = DateTime.MinValue;
                    FlashCards.Properties.Settings.Default.Save();

                    startWindow = new Login();
                    MessageBox.Show(
                        "Ihre Anmeldedaten sind abgelaufen. Bitte melden Sie sich erneut an.",
                        "Anmeldung abgelaufen",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
                else
                {
                    // Benutzer ist bekannt und Anmeldezeitraum ist noch gültig → Login aufrufen
                    startWindow = new Index();
                }
            }
            else
            {
                // Noch kein Benutzer vorhanden (oder Daten wurden gerade gelöscht) → Registrierung
                startWindow = new Login();
            }

            startWindow.Show();
        }
    }
}
