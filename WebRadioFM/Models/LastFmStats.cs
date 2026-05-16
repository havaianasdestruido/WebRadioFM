using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebRadioFM.Models
{
    [DataContract]
    public class RecentTracksResponse
    {
        [DataMember(Name = "recenttracks")]
        public RecentTracksData recenttracks { get; set; }
    }

    [DataContract]
    public class RecentTracksData
    {
        [DataMember(Name = "track")]
        public List<RecentTrackItem> track { get; set; }

        [DataMember(Name = "@attr")]
        public PageAttr attr { get; set; }
    }

    [DataContract]
    public class RecentTrackItem
    {
        [DataMember(Name = "artist")]
        public StatsArtist artist { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "album")]
        public StatsAlbum album { get; set; }

        [DataMember(Name = "image")]
        public List<StatsImage> image { get; set; }

        [DataMember(Name = "date")]
        public StatsDate date { get; set; }

        [DataMember(Name = "@attr")]
        public NowPlayingAttr attr { get; set; }

        [DataMember(Name = "loved")]
        public string loved { get; set; }

        public bool IsNowPlaying
        {
            get { return attr != null && attr.nowplaying == "true"; }
        }
    }

    [DataContract]
    public class StatsArtist
    {
        [DataMember(Name = "#text")]
        public string text { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }
    }

    [DataContract]
    public class StatsAlbum
    {
        [DataMember(Name = "#text")]
        public string text { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }
    }

    [DataContract]
    public class StatsDate
    {
        [DataMember(Name = "uts")]
        public string uts { get; set; }

        [DataMember(Name = "#text")]
        public string text { get; set; }
    }

    [DataContract]
    public class NowPlayingAttr
    {
        [DataMember(Name = "nowplaying")]
        public string nowplaying { get; set; }
    }

    [DataContract]
    public class PageAttr
    {
        [DataMember(Name = "page")]
        public string page { get; set; }

        [DataMember(Name = "perPage")]
        public string perPage { get; set; }

        [DataMember(Name = "totalPages")]
        public string totalPages { get; set; }

        [DataMember(Name = "total")]
        public string total { get; set; }
    }

    [DataContract]
    public class StatsImage
    {
        [DataMember(Name = "#text")]
        public string text { get; set; }

        [DataMember(Name = "size")]
        public string size { get; set; }
    }

    [DataContract]
    public class TopArtistsResponse
    {
        [DataMember(Name = "topartists")]
        public TopArtistsData topartists { get; set; }
    }

    [DataContract]
    public class TopArtistsData
    {
        [DataMember(Name = "artist")]
        public List<TopArtistItem> artist { get; set; }

        [DataMember(Name = "@attr")]
        public PageAttr attr { get; set; }
    }

    [DataContract]
    public class TopArtistItem
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "playcount")]
        public string playcount { get; set; }

        [DataMember(Name = "listeners")]
        public string listeners { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "image")]
        public List<StatsImage> image { get; set; }
    }

    [DataContract]
    public class TopTracksResponse
    {
        [DataMember(Name = "toptracks")]
        public TopTracksData toptracks { get; set; }
    }

    [DataContract]
    public class TopTracksData
    {
        [DataMember(Name = "track")]
        public List<TopTrackItem> track { get; set; }

        [DataMember(Name = "@attr")]
        public PageAttr attr { get; set; }
    }

    [DataContract]
    public class TrackInfoResponse
    {
        [DataMember(Name = "track")]
        public TrackInfoData track { get; set; }
    }

    [DataContract]
    public class TrackInfoData
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "duration")]
        public string duration { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "artist")]
        public StatsArtist artist { get; set; }

        [DataMember(Name = "album")]
        public TrackAlbum album { get; set; }
    }

    [DataContract]
    public class TrackAlbum
    {
        [DataMember(Name = "artist")]
        public string artist { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "image")]
        public List<StatsImage> image { get; set; }
    }

    [DataContract]
    public class TopTrackItem
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "playcount")]
        public string playcount { get; set; }

        [DataMember(Name = "listeners")]
        public string listeners { get; set; }

        [DataMember(Name = "mbid")]
        public string mbid { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "artist")]
        public StatsArtist artist { get; set; }

        [DataMember(Name = "image")]
        public List<StatsImage> image { get; set; }
    }
}
