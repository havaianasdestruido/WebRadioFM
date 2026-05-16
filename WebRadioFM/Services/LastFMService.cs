using System;
using System.Collections.Generic;
using System.IO;
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
                var session = ParseSession(json, token);
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
                if (callback != null)
                    callback();
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
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "WebRadioFM/1.0";
            request.Method = method;
            request.Accept = "application/json";

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
                        request.BeginGetResponse(HandleResponse, new RequestState { Request = request, Callback = callback, ErrorCallback = errorCallback });
                    }
                    catch (Exception ex)
                    {
                        if (errorCallback != null)
                            Deployment.Current.Dispatcher.BeginInvoke(() => errorCallback(ex.Message));
                    }
                }, null);
            }
            else
            {
                request.BeginGetResponse(HandleResponse, new RequestState { Request = request, Callback = callback, ErrorCallback = errorCallback });
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
                    if (state.Callback != null)
                        Deployment.Current.Dispatcher.BeginInvoke(() => state.Callback(json));
                }
            }
            catch (Exception ex)
            {
                if (state.ErrorCallback != null)
                    Deployment.Current.Dispatcher.BeginInvoke(() => state.ErrorCallback(ex.Message));
            }
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
            catch (Exception) { }
            return null;
        }

        private LastFmSession ParseSession(string json, string token)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SessionResponse));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var response = serializer.ReadObject(ms) as SessionResponse;
                    if (response != null && response.session != null)
                    {
                        return new LastFmSession
                        {
                            SessionKey = response.session.key,
                            Username = response.session.name
                        };
                    }
                }
            }
            catch (Exception) { }

            try
            {
                var errorSerializer = new DataContractJsonSerializer(typeof(ErrorResponse));
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var error = errorSerializer.ReadObject(ms) as ErrorResponse;
                    if (error != null)
                    {
                        return null;
                    }
                }
            }
            catch (Exception) { }

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
        }
    }
}
