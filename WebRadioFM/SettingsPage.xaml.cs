using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using WebRadioFM.Helpers;

namespace WebRadioFM
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private string _currentToken;

        public SettingsPage()
        {
            InitializeComponent();

            themeListBox.ItemsSource = ThemeManager.ThemeNames;

            LoadSettings();
        }

        private void LoadSettings()
        {
            apiKeyTextBox.Text = App.LastFm.ApiKey ?? "";
            apiSecretPasswordBox.Password = App.LastFm.ApiSecret ?? "";

            scrobbleEnabledCheck.IsChecked = SettingsManager.ScrobblerEnabled;
            nowPlayingCheck.IsChecked = SettingsManager.NowPlayingEnabled;
            loveOnStartupCheck.IsChecked = SettingsManager.LoveOnStartup;

            delaySecsSlider.Value = SettingsManager.ScrobbleDelaySecs;
            delaySecsValue.Text = string.Format("{0}s", SettingsManager.ScrobbleDelaySecs);

            delayPercentSlider.Value = SettingsManager.ScrobbleDelayPercent;
            delayPercentValue.Text = string.Format("{0}%", SettingsManager.ScrobbleDelayPercent);

            minDurationSlider.Value = SettingsManager.MinTrackDuration;
            minDurationValue.Text = string.Format("{0}s", SettingsManager.MinTrackDuration);

            fetchAlbumArtCheck.IsChecked = SettingsManager.FetchAlbumArt;

            var blocked = SettingsManager.GetBlockedTracks();
            blockedTracksBox.Text = string.Join("\n", blocked);

            var edits = SettingsManager.GetSimpleEdits();
            simpleEditsBox.Text = string.Join("\n", edits.Select(e => string.Format("{0}:{1}:{2}", e.Field, e.Pattern, e.Replacement)));

            var regex = SettingsManager.GetRegexRules();
            regexRulesBox.Text = string.Join("\n", regex.Select(r => string.Format("{0}:{1}:{2}", r.Field, r.Pattern, r.Replacement)));

            string currentTheme = SettingsManager.ThemeName;
            for (int i = 0; i < ThemeManager.ThemeNames.Count; i++)
            {
                if (ThemeManager.ThemeNames[i] == currentTheme)
                {
                    themeListBox.SelectedIndex = i;
                    break;
                }
            }

            darkModeCheck.IsChecked = SettingsManager.ThemeIsDark;

            UpdateAuthStatus();
        }

        private void SaveApiCredentials()
        {
            App.LastFm.Configure(apiKeyTextBox.Text.Trim(), apiSecretPasswordBox.Password.Trim());
        }

        private void GetToken_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(apiKeyTextBox.Text) || string.IsNullOrWhiteSpace(apiSecretPasswordBox.Password))
            {
                MessageBox.Show("Enter your Last.fm API key and secret first.", "Missing Info", MessageBoxButton.OK);
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

                    MessageBox.Show("Token obtained! Now tap 'authorize' to open the browser.\n\nAfter authorizing, paste the token and tap 'connect'.", "Token Obtained", MessageBoxButton.OK);
                },
                (error) =>
                {
                    getTokenButton.IsEnabled = true;
                    MessageBox.Show("Failed: " + error, "Error", MessageBoxButton.OK);
                });
        }

        private void Authorize_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentToken))
            {
                MessageBox.Show("Get a token first.", "No Token", MessageBoxButton.OK);
                return;
            }

            var webBrowserTask = new WebBrowserTask
            {
                Uri = new Uri(App.LastFm.GetAuthUrl(_currentToken), UriKind.Absolute)
            };
            webBrowserTask.Show();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tokenTextBox.Text))
            {
                MessageBox.Show("Paste your token into the field.", "Missing Token", MessageBoxButton.OK);
                return;
            }

            connectButton.IsEnabled = false;
            App.LastFm.GetSessionAsync(
                tokenTextBox.Text.Trim(),
                (session) =>
                {
                    connectButton.IsEnabled = true;
                    UpdateAuthStatus();
                    SettingsManager.UpgradeSettings();
                    MessageBox.Show(string.Format("Connected as {0}!", session.Username), "Connected", MessageBoxButton.OK);
                },
                (error) =>
                {
                    connectButton.IsEnabled = true;
                    MessageBox.Show("Failed: " + error, "Connection Failed", MessageBoxButton.OK);
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

        private void ScrobbleEnabledChanged(object sender, RoutedEventArgs e)
        {
            SettingsManager.ScrobblerEnabled = scrobbleEnabledCheck.IsChecked.HasValue && scrobbleEnabledCheck.IsChecked.Value;
        }

        private void NowPlayingChanged(object sender, RoutedEventArgs e)
        {
            SettingsManager.NowPlayingEnabled = nowPlayingCheck.IsChecked.HasValue && nowPlayingCheck.IsChecked.Value;
        }

        private void LoveOnStartupChanged(object sender, RoutedEventArgs e)
        {
            SettingsManager.LoveOnStartup = loveOnStartupCheck.IsChecked.HasValue && loveOnStartupCheck.IsChecked.Value;
        }

        private void DelaySecsChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = (int)delaySecsSlider.Value;
            SettingsManager.ScrobbleDelaySecs = val;
            delaySecsValue.Text = string.Format("{0}s", val);
        }

        private void DelayPercentChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = (int)delayPercentSlider.Value;
            SettingsManager.ScrobbleDelayPercent = val;
            delayPercentValue.Text = string.Format("{0}%", val);
        }

        private void MinDurationChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int val = (int)minDurationSlider.Value;
            SettingsManager.MinTrackDuration = val;
            minDurationValue.Text = string.Format("{0}s", val);
        }

        private void FetchAlbumArtChanged(object sender, RoutedEventArgs e)
        {
            SettingsManager.FetchAlbumArt = fetchAlbumArtCheck.IsChecked.HasValue && fetchAlbumArtCheck.IsChecked.Value;
        }

        private void SaveBlocked_Click(object sender, RoutedEventArgs e)
        {
            var blocked = blockedTracksBox.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Length > 0)
                .ToList();
            SettingsManager.SaveBlockedTracks(blocked);
            MessageBox.Show(string.Format("Saved {0} blocked entries.", blocked.Count), "Saved", MessageBoxButton.OK);
        }

        private void SaveEdits_Click(object sender, RoutedEventArgs e)
        {
            var edits = simpleEditsBox.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Contains(':'))
                .Select(l =>
                {
                    var parts = l.Split(new[] { ':' }, 3);
                    return new MetadataEditRule
                    {
                        Field = parts.Length > 0 ? parts[0].Trim() : "all",
                        Pattern = parts.Length > 1 ? parts[1].Trim() : "",
                        Replacement = parts.Length > 2 ? parts[2].Trim() : "",
                        Enabled = true
                    };
                })
                .Where(r => !string.IsNullOrEmpty(r.Pattern))
                .ToList();
            SettingsManager.SaveSimpleEdits(edits);
            MessageBox.Show(string.Format("Saved {0} edit rules.", edits.Count), "Saved", MessageBoxButton.OK);
        }

        private void SaveRegex_Click(object sender, RoutedEventArgs e)
        {
            var rules = regexRulesBox.Text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .Where(l => l.Contains(':'))
                .Select(l =>
                {
                    var parts = l.Split(new[] { ':' }, 3);
                    return new MetadataEditRule
                    {
                        Field = parts.Length > 0 ? parts[0].Trim() : "all",
                        Pattern = parts.Length > 1 ? parts[1].Trim() : "",
                        Replacement = parts.Length > 2 ? parts[2].Trim() : "",
                        Enabled = true
                    };
                })
                .Where(r => !string.IsNullOrEmpty(r.Pattern))
                .ToList();
            SettingsManager.SaveRegexRules(rules);
            MessageBox.Show(string.Format("Saved {0} regex rules.", rules.Count), "Saved", MessageBoxButton.OK);
        }

        private void ThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themeListBox.SelectedItem != null)
            {
                string theme = themeListBox.SelectedItem as string;
                SettingsManager.ThemeName = theme;
                ThemeManager.ApplyTheme();
            }
        }

        private void DarkModeChanged(object sender, RoutedEventArgs e)
        {
            SettingsManager.ThemeIsDark = darkModeCheck.IsChecked.HasValue && darkModeCheck.IsChecked.Value;
            ThemeManager.ApplyTheme();
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/StatsPage.xaml", UriKind.Relative));
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
