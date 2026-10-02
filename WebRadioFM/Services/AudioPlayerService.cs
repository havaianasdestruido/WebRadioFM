using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Xna.Framework.Media;
using WebRadioFM.Models;

namespace WebRadioFM.Services
{
    public class AudioPlayerService : IDisposable
    {
        private List<Track> _playlist;
        private List<Song> _songs;
        private int _currentIndex;
        private bool _isShuffled;
        private Random _random;
        private bool _isPlaying;
        private MediaLibrary _mediaLibrary;
        private bool _disposed;

        public event Action<Track> TrackChanged;
        public event Action<bool> PlayStateChanged;
        public event Action<TimeSpan> ProgressChanged;
        public event Action PlaylistEnded;

        public Track CurrentTrack
        {
            get
            {
                if (_playlist != null && _currentIndex >= 0 && _currentIndex < _playlist.Count)
                    return _playlist[_currentIndex];
                return null;
            }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        public bool IsShuffled
        {
            get { return _isShuffled; }
            set { _isShuffled = value; }
        }

        public List<Track> Playlist
        {
            get { return _playlist; }
        }

        public TimeSpan Position
        {
            get { return MediaPlayer.PlayPosition; }
        }

        public TimeSpan Duration
        {
            get
            {
                if (CurrentTrack != null)
                    return CurrentTrack.Duration;
                return TimeSpan.Zero;
            }
        }

        public AudioPlayerService()
        {
            _playlist = new List<Track>();
            _songs = new List<Song>();
            _currentIndex = -1;
            _isShuffled = false;
            _random = new Random();
            _isPlaying = false;
            _mediaLibrary = new MediaLibrary();

            MediaPlayer.MediaStateChanged += OnMediaStateChanged;
            MediaPlayer.ActiveSongChanged += OnActiveSongChanged;
        }

        public void LoadSongsFromMusicLibrary()
        {
            _playlist.Clear();
            _songs.Clear();
            _currentIndex = -1;

            var songs = _mediaLibrary.Songs;
            if (songs != null && songs.Count > 0)
            {
                foreach (var song in songs.OrderBy(s =>
                {
                    try { return s.Artist != null ? s.Artist.Name : ""; }
                    catch { return ""; }
                }).ThenBy(s =>
                {
                    try { return s.Name; }
                    catch { return ""; }
                }))
                {
                    var track = new Track();

                    try { track.Title = song.Name; }
                    catch { track.Title = "Unknown"; }

                    try { track.Artist = song.Artist != null ? song.Artist.Name : "Unknown Artist"; }
                    catch { track.Artist = "Unknown Artist"; }

                    try { track.Album = song.Album != null ? song.Album.Name : "Unknown Album"; }
                    catch { track.Album = "Unknown Album"; }

                    try { track.Duration = song.Duration; }
                    catch { track.Duration = TimeSpan.Zero; }

                    try
                    {
                        var artProp = song.Album?.GetType().GetProperty("Art");
                        if (artProp != null)
                        {
                            var albumArt = artProp.GetValue(song.Album, null);
                            var getImageMethod = albumArt?.GetType().GetMethod("GetImage");
                            if (getImageMethod != null)
                            {
                                using (var stream = getImageMethod.Invoke(albumArt, null) as System.IO.Stream)
                                {
                                    if (stream != null)
                                    {
                                        var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                                        bitmap.SetSource(stream);
                                    }
                                }
                            }
                        }
                    }
                    catch { }

                    _playlist.Add(track);
                    _songs.Add(song);
                }
            }

            if (_playlist.Count > 0)
            {
                _currentIndex = 0;
            }
        }

        public void Play(int index = -1)
        {
            if (index >= 0 && index < _playlist.Count)
            {
                _currentIndex = index;
            }

            if (_currentIndex < 0 || _currentIndex >= _playlist.Count || _currentIndex >= _songs.Count)
                return;

            if (_playlist[_currentIndex] == null || _songs[_currentIndex] == null)
                return;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    var song = _songs[_currentIndex];
                    MediaPlayer.Play(song);
                    _isPlaying = true;

                    if (TrackChanged != null)
                        TrackChanged(_playlist[_currentIndex]);
                    if (PlayStateChanged != null)
                        PlayStateChanged(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not play track: " + ex.Message);
                }
            });
        }

        public void Pause()
        {
            MediaPlayer.Pause();
            _isPlaying = false;
            if (PlayStateChanged != null)
                PlayStateChanged(false);
        }

        public void Resume()
        {
            MediaPlayer.Resume();
            _isPlaying = true;
            if (PlayStateChanged != null)
                PlayStateChanged(true);
        }

        public void Stop()
        {
            MediaPlayer.Stop();
            _isPlaying = false;
            if (PlayStateChanged != null)
                PlayStateChanged(false);
        }

        public void PlayNext()
        {
            if (_playlist.Count == 0)
                return;

            if (_isShuffled)
            {
                _currentIndex = _random.Next(0, _playlist.Count);
            }
            else
            {
                _currentIndex++;
                if (_currentIndex >= _playlist.Count)
                {
                    _currentIndex = 0;
                }
            }

            Play();
        }

        public void PlayPrevious()
        {
            if (_playlist.Count == 0)
                return;

            if (MediaPlayer.PlayPosition.TotalSeconds > 3)
            {
                Play(_currentIndex);
                return;
            }

            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = _playlist.Count - 1;
            }

            Play();
        }

        private void OnMediaStateChanged(object sender, EventArgs e)
        {
            _isPlaying = MediaPlayer.State == MediaState.Playing;

            if (PlayStateChanged != null)
                PlayStateChanged(_isPlaying);

            if (ProgressChanged != null)
                ProgressChanged(MediaPlayer.PlayPosition);
        }

        private void OnActiveSongChanged(object sender, EventArgs e)
        {
            try
            {
                int songIndex = -1;
                for (int i = 0; i < _songs.Count; i++)
                {
                    if (_songs[i] == MediaPlayer.Queue.ActiveSong)
                    {
                        songIndex = i;
                        break;
                    }
                }

                if (songIndex >= 0 && songIndex < _playlist.Count)
                {
                    _currentIndex = songIndex;
                    _isPlaying = true;

                    if (TrackChanged != null)
                        TrackChanged(_playlist[_currentIndex]);
                    if (PlayStateChanged != null)
                        PlayStateChanged(true);
                }
            }
            catch { }
        }

        public void UpdateProgress()
        {
            if (ProgressChanged != null && _isPlaying)
            {
                ProgressChanged(MediaPlayer.PlayPosition);
            }
        }

        public List<Track> GetTracksByArtist(string artist)
        {
            return _playlist.Where(t =>
            {
                try { return t.Artist.Equals(artist, StringComparison.OrdinalIgnoreCase); }
                catch { return false; }
            }).ToList();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                MediaPlayer.MediaStateChanged -= OnMediaStateChanged;
                MediaPlayer.ActiveSongChanged -= OnActiveSongChanged;
                _disposed = true;
            }
        }
    }
}
