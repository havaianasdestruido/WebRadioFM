using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using WebRadioFM.Models;

namespace WebRadioFM
{
    public partial class StatsPage : PhoneApplicationPage
    {
        private string _artistsPeriod = "7day";
        private string _tracksPeriod = "7day";

        public StatsPage()
        {
            InitializeComponent();
            LoadRecentTracks();
            LoadTopArtists(_artistsPeriod);
            LoadTopTracks(_tracksPeriod);
        }

        private void LoadRecentTracks()
        {
            if (!App.LastFm.IsAuthenticated)
            {
                recentTracksList.ItemsSource = null;
                return;
            }

            App.LastFm.GetRecentTracksAsync(App.LastFm.Session.Username, 20,
                (tracks) =>
                {
                    recentTracksList.ItemsSource = tracks;
                },
                (error) => { });
        }

        private void LoadTopArtists(string period)
        {
            if (!App.LastFm.IsAuthenticated)
            {
                topArtistsList.ItemsSource = null;
                return;
            }

            _artistsPeriod = period;
            App.LastFm.GetTopArtistsAsync(App.LastFm.Session.Username, 20, period,
                (artists) =>
                {
                    topArtistsList.ItemsSource = artists;
                },
                (error) => { });
        }

        private void LoadTopTracks(string period)
        {
            if (!App.LastFm.IsAuthenticated)
            {
                topTracksList.ItemsSource = null;
                return;
            }

            _tracksPeriod = period;
            App.LastFm.GetTopTracksAsync(App.LastFm.Session.Username, 20, period,
                (tracks) =>
                {
                    topTracksList.ItemsSource = tracks;
                },
                (error) => { });
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadRecentTracks();
            LoadTopArtists(_artistsPeriod);
            LoadTopTracks(_tracksPeriod);
        }

        private void PeriodArtists_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                LoadTopArtists(btn.Tag as string);
            }
        }

        private void PeriodTracks_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                LoadTopTracks(btn.Tag as string);
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadRecentTracks();
        }
    }
}
