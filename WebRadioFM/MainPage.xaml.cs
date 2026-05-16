using System;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WebRadioFM.Helpers;
using WebRadioFM.Models;
using WebRadioFM.Services;

namespace WebRadioFM
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AudioPlayerService _player;
        private DispatcherTimer _progressTimer;
        private Track _currentScrobbledTrack;
        private DateTime _playStartTime;
        private bool _scrobblePending;
        private bool _isLoved;

        public MainPage()
        {
            InitializeComponent();

            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromSeconds(1);
            _progressTimer.Tick += ProgressTimerTick;

            _player = new AudioPlayerService();
            _player.TrackChanged += OnTrackChanged;
            _player.PlayStateChanged += OnPlayStateChanged;

            LoadPlaylist();

            UpdateScrobbleStatus();
        }

        private void LoadPlaylist()
        {
            _player.LoadSongsFromMusicLibrary();

            if (_player.Playlist.Count > 0)
            {
                playlistListBox.ItemsSource = _player.Playlist;
                trackCountText.Text = string.Format("{0} tracks", _player.Playlist.Count);
            }
            else
            {
                trackCountText.Text = "No music found";
                MessageBox.Show("No music found in your library. Add some music to your phone first.");
            }
        }

        private void PlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playlistListBox.SelectedIndex >= 0)
            {
                int index = playlistListBox.SelectedIndex;
                _player.Play(index);
                playlistListBox.SelectedIndex = -1;
            }
        }

        private void PlayPause_Click(object sender, EventArgs e)
        {
            if (_player.CurrentTrack == null)
            {
                if (_player.Playlist.Count > 0)
                {
                    _player.Play(0);
                }
                return;
            }

            if (_player.IsPlaying)
            {
                _player.Pause();
                ((ApplicationBarIconButton)appBarPlayPause).IconUri = new Uri("/Assets/appbar.transport.play.png", UriKind.Relative);
            }
            else
            {
                _player.Resume();
                ((ApplicationBarIconButton)appBarPlayPause).IconUri = new Uri("/Assets/appbar.transport.pause.png", UriKind.Relative);
            }
        }

        private void Next_Click(object sender, EventArgs e)
        {
            _player.PlayNext();
        }

        private void Previous_Click(object sender, EventArgs e)
        {
            _player.PlayPrevious();
        }

        private void Shuffle_Click(object sender, EventArgs e)
        {
            _player.IsShuffled = !_player.IsShuffled;
            ((ApplicationBarIconButton)appBarShuffle).IconUri = _player.IsShuffled
                ? new Uri("/Assets/appbar.shuffle.active.png", UriKind.Relative)
                : new Uri("/Assets/appbar.shuffle.png", UriKind.Relative);
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void Stats_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/StatsPage.xaml", UriKind.Relative));
        }

        private void TrackInfo_Click(object sender, EventArgs e)
        {
            if (_player.CurrentTrack != null)
            {
                MessageBox.Show(
                    string.Format("Artist: {0}\nTrack: {1}\nAlbum: {2}\nDuration: {3:mm\\:ss}",
                        _player.CurrentTrack.Artist,
                        _player.CurrentTrack.Title,
                        _player.CurrentTrack.Album,
                        _player.CurrentTrack.Duration),
                    "Track Info", MessageBoxButton.OK);
            }
        }

        private void LoveButton_Click(object sender, EventArgs e)
        {
            if (_player.CurrentTrack == null || !App.LastFm.IsAuthenticated)
                return;

            _isLoved = !_isLoved;
            UpdateLoveButton();

            if (_isLoved)
            {
                App.LastFm.LoveTrackAsync(
                    _player.CurrentTrack.Artist,
                    _player.CurrentTrack.Title,
                    () => { },
                    (error) => { });
            }
            else
            {
                App.LastFm.UnloveTrackAsync(
                    _player.CurrentTrack.Artist,
                    _player.CurrentTrack.Title,
                    () => { },
                    (error) => { });
            }
        }

        private void UpdateLoveButton()
        {
            loveButton.Foreground = _isLoved
                ? Application.Current.Resources["PhoneAccentBrush"] as System.Windows.Media.Brush
                : Application.Current.Resources["PhoneSubtleBrush"] as System.Windows.Media.Brush;
        }

        private void ProgressTimerTick(object sender, EventArgs e)
        {
            _player.UpdateProgress();

            if (_player.CurrentTrack != null && _player.IsPlaying)
            {
                if (_player.Duration.TotalSeconds > 0)
                {
                    progressSlider.Value = _player.Position.TotalSeconds / _player.Duration.TotalSeconds;
                }
                currentPositionText.Text = string.Format("{0:mm\\:ss}", _player.Position);

                CheckScrobble();
            }
        }

        private void OnTrackChanged(Track track)
        {
            string title = SettingsManager.ApplyEdits(track.Title, "title");
            string artist = SettingsManager.ApplyEdits(track.Artist, "artist");
            string album = SettingsManager.ApplyEdits(track.Album, "album");

            nowPlayingTitle.Text = title;
            nowPlayingArtist.Text = artist;
            nowPlayingAlbum.Text = album;

            totalDurationText.Text = string.Format("{0:mm\\:ss}", track.Duration);

            progressSlider.Value = 0;
            currentPositionText.Text = "0:00";

            _currentScrobbledTrack = null;
            _scrobblePending = false;
            _playStartTime = DateTime.Now;

            _isLoved = false;
            UpdateLoveButton();

            if (SettingsManager.LoveOnStartup && App.LastFm.IsAuthenticated)
            {
                _isLoved = true;
                UpdateLoveButton();
                App.LastFm.LoveTrackAsync(artist, title, () => { }, (error) => { });
            }

            if (!_progressTimer.IsEnabled)
            {
                _progressTimer.Start();
            }

            SendNowPlaying(track);

            UpdateScrobbleStatus();

            if (SettingsManager.FetchAlbumArt && App.LastFm.IsAuthenticated)
            {
                App.LastFm.GetTrackInfoAsync(artist, title,
                    (info) => { },
                    (error) => { });
            }
        }

        private void OnPlayStateChanged(bool isPlaying)
        {
            if (isPlaying)
            {
                ((ApplicationBarIconButton)appBarPlayPause).IconUri = new Uri("/Assets/appbar.transport.pause.png", UriKind.Relative);
                _progressTimer.Start();
            }
            else
            {
                ((ApplicationBarIconButton)appBarPlayPause).IconUri = new Uri("/Assets/appbar.transport.play.png", UriKind.Relative);
            }
        }

        private void SendNowPlaying(Track track)
        {
            if (!App.LastFm.IsAuthenticated || !SettingsManager.NowPlayingEnabled || !SettingsManager.ScrobblerEnabled)
                return;

            string artist = SettingsManager.ApplyEdits(track.Artist, "artist");
            string title = SettingsManager.ApplyEdits(track.Title, "title");
            string album = SettingsManager.ApplyEdits(track.Album, "album");

            App.LastFm.UpdateNowPlayingAsync(
                artist, title, album,
                (int)track.Duration.TotalSeconds,
                () =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        scrobbleStatusText.Text = "Now Playing on Last.fm";
                    });
                },
                (error) =>
                {
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        scrobbleStatusText.Text = "";
                    });
                });
        }

        private void CheckScrobble()
        {
            if (!App.LastFm.IsAuthenticated || _player.CurrentTrack == null)
                return;

            if (!SettingsManager.ScrobblerEnabled)
                return;

            if (_scrobblePending)
                return;

            Track currentTrack = _player.CurrentTrack;

            if (_currentScrobbledTrack == currentTrack)
                return;

            if (_player.Duration.TotalSeconds <= 0)
                return;

            if (_player.Duration.TotalSeconds < SettingsManager.MinTrackDuration)
                return;

            double progressPercent = _player.Position.TotalSeconds / _player.Duration.TotalSeconds;

            int delaySecs = Math.Min(SettingsManager.ScrobbleDelaySecs, (int)(_player.Duration.TotalSeconds * SettingsManager.ScrobbleDelayPercent / 100));

            if (progressPercent >= 0.5 || _player.Position.TotalSeconds >= delaySecs)
            {
                string artist = SettingsManager.ApplyEdits(currentTrack.Artist, "artist");
                string title = SettingsManager.ApplyEdits(currentTrack.Title, "title");
                string album = SettingsManager.ApplyEdits(currentTrack.Album, "album");

                if (SettingsManager.IsTrackBlocked(artist, title))
                {
                    _scrobblePending = true;
                    _currentScrobbledTrack = currentTrack;
                    scrobbleStatusText.Text = "Blocked";
                    return;
                }

                _scrobblePending = true;
                _currentScrobbledTrack = currentTrack;

                App.LastFm.ScrobbleAsync(
                    artist, title, album,
                    (int)currentTrack.Duration.TotalSeconds,
                    _playStartTime,
                    () =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            scrobbleStatusText.Text = "Scrobbled!";
                        });
                    },
                    (error) =>
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            scrobbleStatusText.Text = "Scrobble failed";
                            _scrobblePending = false;
                        });
                    });
            }
        }

        private void UpdateScrobbleStatus()
        {
            if (App.LastFm.IsAuthenticated && SettingsManager.ScrobblerEnabled)
            {
                scrobbleStatusText.Text = SettingsManager.NowPlayingEnabled ? "Last.fm scrobbling on" : "Scrobbling on (no NP)";
            }
            else if (App.LastFm.IsAuthenticated)
            {
                scrobbleStatusText.Text = "Last.fm scrobbling off";
            }
            else
            {
                scrobbleStatusText.Text = "";
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateScrobbleStatus();
        }
    }
}
