using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebRadioFM.Models
{
    public class Track : INotifyPropertyChanged
    {
        private string _title;
        private string _artist;
        private string _album;
        private string _filePath;
        private TimeSpan _duration;
        private string _albumArt;

        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        public string Artist
        {
            get { return _artist; }
            set { _artist = value; OnPropertyChanged(); }
        }

        public string Album
        {
            get { return _album; }
            set { _album = value; OnPropertyChanged(); }
        }

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged(); }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; OnPropertyChanged(); }
        }

        public string AlbumArt
        {
            get { return _albumArt; }
            set { _albumArt = value; OnPropertyChanged(); }
        }

        public string DisplayText
        {
            get { return string.Format("{0} - {1}", Artist, Title); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
