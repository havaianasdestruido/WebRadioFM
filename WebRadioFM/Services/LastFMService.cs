using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using WebRadioFM.Helpers;
using WebRadioFM.Models;

namespace WebRadioFM.Services
{
    public class LastFMService
    {
        private const string ApiBaseUrl = "http://ws.audioscrobbler.com/2.0/";
        private const string AuthBaseUrl = "http://www.last.fm/api/auth/";

        private string _apiKey;
        private string _apiSecret;
        private LastFmSession _session;
        private int _activeRequests;
        private readonly object _requestLock = new object();

        public LastFmSession Session
        {
            get { return _session; }
        }

        public bool IsAuthenticated
        {
            get { return _session != null && _session.IsAuthenticated; }
        }

        public string ApiKey
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }

        public string ApiSecret
        {
            get { return _apiSecret; }
            set { _apiSecret = value; }
        }

        public LastFMService()
        {
            _session = new LastFmSession();
            _activeRequests = 0;
        }

        public void Configure(string apiKey, string apiSecret)
        {
            _apiKey = apiKey;
            _apiSecret = apiSecret;
        }

        public string GetAuthUrl(string token)
        {
            return string.Format("{0}?api_key={1}&token={2}", AuthBaseUrl, _apiKey, token);
        }

        public void GetTokenAsync(Action<string> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "auth.gettoken" },
                { "api_key", _apiKey }
            };

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                string token = ParseToken(json);
                if (!string.IsNullOrEmpty(token))
                {
                    if (callback != null)
                        callback(token);
                }
                else
                {
                    if (errorCallback != null)
                        errorCallback("Failed to parse token from response");
                }
            }, errorCallback);
        }

        public void GetSessionAsync(string token, Action<LastFmSession> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "auth.getSession" },
                { "api_key", _apiKey },
                { "token", token }
            };

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                var session = ParseSession(json);
                if (session != null && session.IsAuthenticated)
                {
                    _session = session;
                    SaveSession();
                    if (callback != null)
                        callback(session);
                }
                else
                {
                    if (errorCallback != null)
                        errorCallback("Authentication failed. Please ensure you authorized the app.");
                }
            }, errorCallback);
        }

        public void UpdateNowPlayingAsync(string artist, string track, string album, int duration, Action callback, Action<string> errorCallback)
        {
            if (!IsAuthenticated)
            {
                if (errorCallback != null)
                    errorCallback("Not authenticated with Last.fm");
                return;
            }

            var parameters = new Dictionary<string, string>
            {
                { "method", "track.updateNowPlaying" },
                { "api_key", _apiKey },
                { "sk", _session.SessionKey },
                { "artist", artist },
                { "track", track },
                { "duration", duration.ToString() }
            };

            if (!string.IsNullOrEmpty(album))
            {
                parameters.Add("album", album);
            }

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "POST", parameters, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }
                if (callback != null)
                    callback();
            }, errorCallback);
        }

        public void ScrobbleAsync(string artist, string track, string album, int duration, DateTime timestamp, Action callback, Action<string> errorCallback)
        {
            if (!IsAuthenticated)
            {
                if (errorCallback != null)
                    errorCallback("Not authenticated with Last.fm");
                return;
            }

            var parameters = new Dictionary<string, string>
            {
                { "method", "track.scrobble" },
                { "api_key", _apiKey },
                { "sk", _session.SessionKey },
                { "artist", artist },
                { "track", track },
                { "duration", duration.ToString() },
                { "timestamp", ToUnixTimestamp(timestamp).ToString() }
            };

            if (!string.IsNullOrEmpty(album))
            {
                parameters.Add("album", album);
            }

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "POST", parameters, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }
                if (callback != null)
                    callback();
            }, errorCallback);
        }

        public void LoveTrackAsync(string artist, string track, Action callback, Action<string> errorCallback)
        {
            if (!IsAuthenticated)
            {
                if (errorCallback != null)
                    errorCallback("Not authenticated with Last.fm");
                return;
            }

            var parameters = new Dictionary<string, string>
            {
                { "method", "track.love" },
                { "api_key", _apiKey },
                { "sk", _session.SessionKey },
                { "artist", artist },
                { "track", track }
            };

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "POST", parameters, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }
                if (callback != null)
                    callback();
            }, errorCallback);
        }

        public void UnloveTrackAsync(string artist, string track, Action callback, Action<string> errorCallback)
        {
            if (!IsAuthenticated)
            {
                if (errorCallback != null)
                    errorCallback("Not authenticated with Last.fm");
                return;
            }

            var parameters = new Dictionary<string, string>
            {
                { "method", "track.unlove" },
                { "api_key", _apiKey },
                { "sk", _session.SessionKey },
                { "artist", artist },
                { "track", track }
            };

            string sig = ApiSignatureHelper.CreateSignature(parameters, _apiSecret);
            parameters.Add("api_sig", sig);

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "POST", parameters, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }
                if (callback != null)
                    callback();
            }, errorCallback);
        }

        public void GetRecentTracksAsync(string username, int limit, Action<List<RecentTrackItem>> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "user.getRecentTracks" },
                { "api_key", _apiKey },
                { "user", username },
                { "limit", limit.ToString() }
            };

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(RecentTracksResponse));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var response = serializer.ReadObject(ms) as RecentTracksResponse;
                        if (response != null && response.recenttracks != null)
                        {
                            if (callback != null)
                                callback(response.recenttracks.track ?? new List<RecentTrackItem>());
                            return;
                        }
                    }
                }
                catch { }

                if (errorCallback != null)
                    errorCallback("Failed to parse recent tracks");
            }, errorCallback);
        }

        public void GetTopArtistsAsync(string username, int limit, string period, Action<List<TopArtistItem>> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "user.getTopArtists" },
                { "api_key", _apiKey },
                { "user", username },
                { "limit", limit.ToString() },
                { "period", period }
            };

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(TopArtistsResponse));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var response = serializer.ReadObject(ms) as TopArtistsResponse;
                        if (response != null && response.topartists != null)
                        {
                            if (callback != null)
                                callback(response.topartists.artist ?? new List<TopArtistItem>());
                            return;
                        }
                    }
                }
                catch { }

                if (errorCallback != null)
                    errorCallback("Failed to parse top artists");
            }, errorCallback);
        }

        public void GetTopTracksAsync(string username, int limit, string period, Action<List<TopTrackItem>> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "user.getTopTracks" },
                { "api_key", _apiKey },
                { "user", username },
                { "limit", limit.ToString() },
                { "period", period }
            };

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(TopTracksResponse));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var response = serializer.ReadObject(ms) as TopTracksResponse;
                        if (response != null && response.toptracks != null)
                        {
                            if (callback != null)
                                callback(response.toptracks.track ?? new List<TopTrackItem>());
                            return;
                        }
                    }
                }
                catch { }

                if (errorCallback != null)
                    errorCallback("Failed to parse top tracks");
            }, errorCallback);
        }

        public void GetTrackInfoAsync(string artist, string track, Action<TrackInfoData> callback, Action<string> errorCallback)
        {
            var parameters = new Dictionary<string, string>
            {
                { "method", "track.getInfo" },
                { "api_key", _apiKey },
                { "artist", artist },
                { "track", track }
            };

            if (!string.IsNullOrEmpty(_session.Username))
            {
                parameters.Add("username", _session.Username);
            }

            string url = BuildRequestUrl(parameters);
            MakeRequest(url, "GET", null, (json) =>
            {
                string errorMsg = GetErrorFromJson(json);
                if (errorMsg != null)
                {
                    if (errorCallback != null)
                        errorCallback(errorMsg);
                    return;
                }

                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(TrackInfoResponse));
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        var response = serializer.ReadObject(ms) as TrackInfoResponse;
                        if (response != null && response.track != null)
                        {
                            if (callback != null)
                                callback(response.track);
                            return;
                        }
                    }
                }
                catch { }

                if (errorCallback != null)
                    errorCallback("Failed to parse track info");
            }, errorCallback);
        }

        public void LoadSession()
        {
            try
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains("LastFmSessionKey"))
                {
                    _session.SessionKey = settings["LastFmSessionKey"] as string;
                }
                if (settings.Contains("LastFmUsername"))
                {
                    _session.Username = settings["LastFmUsername"] as string;
                }
            }
            catch (Exception)
            {
            }
        }

        private void SaveSession()
        {
            try
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings["LastFmSessionKey"] = _session.SessionKey;
                settings["LastFmUsername"] = _session.Username;
                settings.Save();
            }
            catch (Exception)
            {
            }
        }

        public void ClearSession()
        {
            _session = new LastFmSession();
            try
            {
                var settings = IsolatedStorageSettings.ApplicationSettings;
                settings.Remove("LastFmSessionKey");
                settings.Remove("LastFmUsername");
                settings.Save();
            }
            catch (Exception)
            {
            }
        }

        private string BuildRequestUrl(Dictionary<string, string> parameters)
        {
            var sb = new StringBuilder();
            sb.Append(ApiBaseUrl);
            sb.Append("?");
            bool first = true;
            foreach (var kvp in parameters)
            {
                if (!first)
                    sb.Append("&");
                sb.Append(Uri.EscapeDataString(kvp.Key));
                sb.Append("=");
                sb.Append(Uri.EscapeDataString(kvp.Value));
                first = false;
            }
            sb.Append("&format=json");
            return sb.ToString();
        }

        private void MakeRequest(string url, string method, Dictionary<string, string> postData, Action<string> callback, Action<string> errorCallback)
        {
            lock (_requestLock)
            {
                if (_activeRequests >= 4)
                {
                    if (errorCallback != null)
                        Deployment.Current.Dispatcher.BeginInvoke(() => errorCallback("Too many concurrent requests"));
                    return;
                }
                _activeRequests++;
            }

            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "WebRadioFM/1.0";
            request.Method = method;
            request.Accept = "application/json";

            var state = new RequestState
            {
                Request = request,
                Callback = callback,
                ErrorCallback = errorCallback,
                OnComplete = () =>
                {
                    lock (_requestLock)
                    {
                        _activeRequests--;
                    }
                }
            };

            if (method == "POST" && postData != null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                var sb = new StringBuilder();
                bool first = true;
                foreach (var kvp in postData)
                {
                    if (!first)
                        sb.Append("&");
                    sb.Append(Uri.EscapeDataString(kvp.Key));
                    sb.Append("=");
                    sb.Append(Uri.EscapeDataString(kvp.Value));
                    first = false;
                }
                sb.Append("&format=json");

                byte[] data = Encoding.UTF8.GetBytes(sb.ToString());

                request.BeginGetRequestStream((asyncResult) =>
                {
                    try
                    {
                        using (var stream = request.EndGetRequestStream(asyncResult))
                        {
                            stream.Write(data, 0, data.Length);
                        }
                        request.BeginGetResponse(HandleResponse, state);
                    }
                    catch (Exception ex)
                    {
                        state.OnComplete();
                        if (errorCallback != null)
                            Deployment.Current.Dispatcher.BeginInvoke(() => errorCallback(ex.Message));
                    }
                }, null);
            }
            else
            {
                request.BeginGetResponse(HandleResponse, state);
            }
        }

        private void HandleResponse(IAsyncResult asyncResult)
        {
            var state = asyncResult.AsyncState as RequestState;
            try
            {
                using (var response = state.Request.EndGetResponse(asyncResult))
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    state.OnComplete();
                    if (state.Callback != null)
                        Deployment.Current.Dispatcher.BeginInvoke(() => state.Callback(json));
                }
            }
            catch (WebException webEx)
            {
                state.OnComplete();
                string errorMsg = "Network error: " + webEx.Message;

                try
                {
                    using (var stream = webEx.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        string apiError = GetErrorFromJson(json);
                        if (apiError != null)
                            errorMsg = apiError;
                    }
                }
                catch { }

                if (state.ErrorCallback != null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => state.ErrorCallback(errorMsg));
            }
            catch (Exception ex)
            {
                state.OnComplete();
                if (state.ErrorCallback != null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => state.ErrorCallback("Request failed: " + ex.Message));
            }
        }

        private string GetErrorFromJson(string json)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(ErrorResponse));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var error = serializer.ReadObject(ms) as ErrorResponse;
                    if (error != null && error.error != 0)
                    {
                        return string.Format("Last.fm error {0}: {1}", error.error, error.message);
                    }
                }
            }
            catch { }
            return null;
        }

        private string ParseToken(string json)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(TokenResponse));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var response = serializer.ReadObject(ms) as TokenResponse;
                    if (response != null && !string.IsNullOrEmpty(response.token))
                    {
                        return response.token;
                    }
                }
            }
            catch { }
            return null;
        }

        private LastFmSession ParseSession(string json)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SessionResponse));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var response = serializer.ReadObject(ms) as SessionResponse;
                    if (response != null && response.session != null && !string.IsNullOrEmpty(response.session.key))
                    {
                        return new LastFmSession
                        {
                            SessionKey = response.session.key,
                            Username = response.session.name
                        };
                    }
                }
            }
            catch { }

            return null;
        }

        private static long ToUnixTimestamp(DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        private class RequestState
        {
            public HttpWebRequest Request { get; set; }
            public Action<string> Callback { get; set; }
            public Action<string> ErrorCallback { get; set; }
            public Action OnComplete { get; set; }
        }
    }
}
