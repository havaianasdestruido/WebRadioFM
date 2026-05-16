using System.Runtime.Serialization;

namespace WebRadioFM.Models
{
    [DataContract]
    public class TokenResponse
    {
        [DataMember(Name = "token")]
        public string token { get; set; }
    }

    [DataContract]
    public class SessionResponse
    {
        [DataMember(Name = "session")]
        public SessionData session { get; set; }
    }

    [DataContract]
    public class SessionData
    {
        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "key")]
        public string key { get; set; }

        [DataMember(Name = "subscriber")]
        public int subscriber { get; set; }
    }

    [DataContract]
    public class ErrorResponse
    {
        [DataMember(Name = "error")]
        public int error { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
    }

    [DataContract]
    public class ScrobbleResponse
    {
        [DataMember(Name = "scrobbles")]
        public ScrobblesData scrobbles { get; set; }
    }

    [DataContract]
    public class ScrobblesData
    {
        [DataMember(Name = "@attr")]
        public ScrobbleAttr attr { get; set; }

        [DataMember(Name = "scrobble")]
        public ScrobbleItem[] scrobble { get; set; }
    }

    [DataContract]
    public class ScrobbleAttr
    {
        [DataMember(Name = "accepted")]
        public int accepted { get; set; }

        [DataMember(Name = "ignored")]
        public int ignored { get; set; }
    }

    [DataContract]
    public class ScrobbleItem
    {
        [DataMember(Name = "artist")]
        public ScrobbleElement artist { get; set; }

        [DataMember(Name = "track")]
        public ScrobbleElement track { get; set; }

        [DataMember(Name = "album")]
        public ScrobbleElement album { get; set; }

        [DataMember(Name = "timestamp")]
        public ScrobbleElement timestamp { get; set; }

        [DataMember(Name = "ignoredMessage")]
        public IgnoredMessage ignoredMessage { get; set; }
    }

    [DataContract]
    public class ScrobbleElement
    {
        [DataMember(Name = "#text")]
        public string text { get; set; }

        [DataMember(Name = "corrected")]
        public string corrected { get; set; }
    }

    [DataContract]
    public class IgnoredMessage
    {
        [DataMember(Name = "#text")]
        public string text { get; set; }

        [DataMember(Name = "code")]
        public string code { get; set; }
    }
}
