using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;

namespace WebRadioFM.Helpers
{
    public static class SettingsManager
    {
        private const string ScrobblerEnabledKey = "ScrobblerEnabled";
        private const string NowPlayingEnabledKey = "NowPlayingEnabled";
        private const string ScrobbleDelaySecsKey = "ScrobbleDelaySecs";
        private const string ScrobbleDelayPercentKey = "ScrobbleDelayPercent";
        private const string MinTrackDurationKey = "MinTrackDuration";
        private const string FetchAlbumArtKey = "FetchAlbumArt";
        private const string ThemeNameKey = "ThemeName";
        private const string ThemeIsDarkKey = "ThemeIsDark";
        private const string LoveOnStartupKey = "LoveOnStartup";
        private const string RegexRulesKey = "RegexRules";
        private const string BlockedTracksKey = "BlockedTracks";
        private const string SimpleEditsKey = "SimpleEdits";

        public static bool ScrobblerEnabled
        {
            get { return Get(ScrobblerEnabledKey, true); }
            set { Set(ScrobblerEnabledKey, value); }
        }

        public static bool NowPlayingEnabled
        {
            get { return Get(NowPlayingEnabledKey, true); }
            set { Set(NowPlayingEnabledKey, value); }
        }

        public static int ScrobbleDelaySecs
        {
            get { return Get(ScrobbleDelaySecsKey, 180); }
            set { Set(ScrobbleDelaySecsKey, Math.Max(30, Math.Min(360, value))); }
        }

        public static int ScrobbleDelayPercent
        {
            get { return Get(ScrobbleDelayPercentKey, 50); }
            set { Set(ScrobbleDelayPercentKey, Math.Max(30, Math.Min(95, value))); }
        }

        public static int MinTrackDuration
        {
            get { return Get(MinTrackDurationKey, 30); }
            set { Set(MinTrackDurationKey, Math.Max(10, Math.Min(60, value))); }
        }

        public static bool FetchAlbumArt
        {
            get { return Get(FetchAlbumArtKey, false); }
            set { Set(FetchAlbumArtKey, value); }
        }

        public static string ThemeName
        {
            get { return Get(ThemeNameKey, "Default"); }
            set { Set(ThemeNameKey, value); }
        }

        public static bool ThemeIsDark
        {
            get { return Get(ThemeIsDarkKey, true); }
            set { Set(ThemeIsDarkKey, value); }
        }

        public static bool LoveOnStartup
        {
            get { return Get(LoveOnStartupKey, false); }
            set { Set(LoveOnStartupKey, value); }
        }

        public static List<MetadataEditRule> GetRegexRules()
        {
            return Get(RegexRulesKey, new List<MetadataEditRule>());
        }

        public static void SaveRegexRules(List<MetadataEditRule> rules)
        {
            Set(RegexRulesKey, rules);
        }

        public static List<MetadataEditRule> GetSimpleEdits()
        {
            return Get(SimpleEditsKey, new List<MetadataEditRule>());
        }

        public static void SaveSimpleEdits(List<MetadataEditRule> edits)
        {
            Set(SimpleEditsKey, edits);
        }

        public static List<string> GetBlockedTracks()
        {
            return Get(BlockedTracksKey, new List<string>());
        }

        public static void SaveBlockedTracks(List<string> tracks)
        {
            Set(BlockedTracksKey, tracks);
        }

        public static string ApplyEdits(string text, string field)
        {
            var edits = GetSimpleEdits();
            foreach (var edit in edits)
            {
                if (edit.Field == field || edit.Field == "all")
                {
                    text = text.Replace(edit.Pattern, edit.Replacement);
                }
            }

            var regexRules = GetRegexRules();
            foreach (var rule in regexRules)
            {
                if (rule.Field == field || rule.Field == "all")
                {
                    try
                    {
                        text = System.Text.RegularExpressions.Regex.Replace(text, rule.Pattern, rule.Replacement);
                    }
                    catch { }
                }
            }

            return text;
        }

        public static bool IsTrackBlocked(string artist, string track)
        {
            var blocked = GetBlockedTracks();
            string display = string.Format("{0} - {1}", artist, track).ToLowerInvariant();
            return blocked.Any(b => display.Contains(b.ToLowerInvariant()));
        }

        public static void UpgradeSettings()
        {
            var s = IsolatedStorageSettings.ApplicationSettings;
            if (!s.Contains("SettingsVersion"))
            {
                if (s.Contains("LastFmScrobblingEnabled"))
                {
                    ScrobblerEnabled = (bool)s["LastFmScrobblingEnabled"];
                    s.Remove("LastFmScrobblingEnabled");
                }
                s["SettingsVersion"] = 1;
                s.Save();
            }
        }

        private static T Get<T>(string key, T defaultValue)
        {
            try
            {
                var s = IsolatedStorageSettings.ApplicationSettings;
                if (s.Contains(key))
                    return (T)s[key];
            }
            catch { }
            return defaultValue;
        }

        private static void Set<T>(string key, T value)
        {
            try
            {
                var s = IsolatedStorageSettings.ApplicationSettings;
                s[key] = value;
                s.Save();
            }
            catch { }
        }
    }

    public class MetadataEditRule
    {
        public string Field { get; set; }
        public string Pattern { get; set; }
        public string Replacement { get; set; }
        public bool Enabled { get; set; } = true;
        public string Description { get; set; }

        public MetadataEditRule() { }

        public MetadataEditRule(string field, string pattern, string replacement, string desc = "")
        {
            Field = field;
            Pattern = pattern;
            Replacement = replacement;
            Description = desc;
            Enabled = true;
        }
    }
}
