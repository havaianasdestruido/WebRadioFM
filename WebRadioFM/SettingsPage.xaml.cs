using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using WebRadioFM.Services;

namespace WebRadioFM
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private string _currentToken;

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("LastFmApiKey"))
                apiKeyTextBox.Text = settings["LastFmApiKey"] as string ?? "";

            if (settings.Contains("LastFmApiSecret"))
                apiSecretPasswordBox.Password = settings["LastFmApiSecret"] as string ?? "";

            if (settings.Contains("LastFmScrobblingEnabled"))
                scrobblingCheckBox.IsChecked = (bool)settings["LastFmScrobblingEnabled"];

            UpdateAuthStatus();
        }

        private void SaveApiCredentials()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["LastFmApiKey"] = apiKeyTextBox.Text.Trim();
            settings["LastFmApiSecret"] = apiSecretPasswordBox.Password.Trim();
            settings.Save();

            App.LastFm.Configure(apiKeyTextBox.Text.Trim(), apiSecretPasswordBox.Password.Trim());
        }

        private void GetToken_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apiKeyTextBox.Text) || string.IsNullOrWhiteSpace(apiSecretPasswordBox.Password))
            {
                MessageBox.Show("Please enter your Last.fm API key and secret first.", "Missing Info", MessageBoxButton.OK);
                return;
            }

            SaveApiCredentials();

            getTokenButton.IsEnabled = false;
            App.LastFm.GetTokenAsync(
                (token) =>
                {
                    _currentToken = token;
                    tokenTextBox.Text = token;
                    authButton.IsEnabled = true;
                    connectButton.IsEnabled = true;
                    getTokenButton.IsEnabled = true;

                    MessageBox.Show("Token obtained! Now tap 'Authorize' to open the browser and authorize this app.\n\nAfter authorizing, tap 'Connect' to finish.", "Token Obtained", MessageBoxButton.OK);
                },
                (error) =>
                {
                    getTokenButton.IsEnabled = true;
                    MessageBox.Show("Failed to get token: " + error, "Error", MessageBoxButton.OK);
                });
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentToken))
            {
                MessageBox.Show("Please get a token first.", "No Token", MessageBoxButton.OK);
                return;
            }

            string authUrl = App.LastFm.GetAuthUrl(_currentToken);

            var webBrowserTask = new WebBrowserTask
            {
                Uri = new Uri(authUrl, UriKind.Absolute)
            };
            webBrowserTask.Show();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tokenTextBox.Text))
            {
                MessageBox.Show("Please paste your token into the Token field.", "Missing Token", MessageBoxButton.OK);
                return;
            }

            string token = tokenTextBox.Text.Trim();
            connectButton.IsEnabled = false;

            App.LastFm.GetSessionAsync(
                token,
                (session) =>
                {
                    connectButton.IsEnabled = true;
                    UpdateAuthStatus();
                    MessageBox.Show(string.Format("Connected to Last.fm as {0}!", session.Username), "Connected!", MessageBoxButton.OK);
                },
                (error) =>
                {
                    connectButton.IsEnabled = true;
                    MessageBox.Show("Failed to connect: " + error + "\n\nMake sure you authorized the app in the browser first and copied the correct token.", "Connection Failed", MessageBoxButton.OK);
                });
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.LastFm.ClearSession();
            _currentToken = null;
            tokenTextBox.Text = "";
            authButton.IsEnabled = false;
            connectButton.IsEnabled = false;
            UpdateAuthStatus();
        }

        private void ScrobblingCheckChanged(object sender, RoutedEventArgs e)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["LastFmScrobblingEnabled"] = scrobblingCheckBox.IsChecked.HasValue && scrobblingCheckBox.IsChecked.Value;
            settings.Save();
        }

        private void UpdateAuthStatus()
        {
            if (App.LastFm.IsAuthenticated)
            {
                authStatusText.Text = string.Format("Connected as {0}", App.LastFm.Session.Username);
                logoutButton.IsEnabled = true;
            }
            else
            {
                authStatusText.Text = "Not connected";
                logoutButton.IsEnabled = false;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadSettings();
            UpdateAuthStatus();

            if (!string.IsNullOrEmpty(tokenTextBox.Text))
            {
                authButton.IsEnabled = true;
                connectButton.IsEnabled = true;
            }
        }
    }
}
