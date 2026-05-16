using System;

namespace WebRadioFM.Models
{
    public class LastFmSession
    {
        public string SessionKey { get; set; }
        public string Username { get; set; }
        public bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(SessionKey); }
        }
    }
}
