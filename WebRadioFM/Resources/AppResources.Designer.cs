using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows;

namespace WebRadioFM.Resources
{
    public class AppResources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    resourceMan = new ResourceManager("WebRadioFM.Resources.AppResources", typeof(AppResources).Assembly);
                }
                return resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static CultureInfo Culture
        {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }

        public static string ResourceFlowDirection
        {
            get { return ResourceManager.GetString("ResourceFlowDirection", resourceCulture); }
        }

        public static string ResourceLanguage
        {
            get { return ResourceManager.GetString("ResourceLanguage", resourceCulture); }
        }

        public static string ApplicationTitle
        {
            get { return ResourceManager.GetString("ApplicationTitle", resourceCulture); }
        }

        public static string MainTitle
        {
            get { return ResourceManager.GetString("MainTitle", resourceCulture); }
        }

        public static string SettingsTitle
        {
            get { return ResourceManager.GetString("SettingsTitle", resourceCulture); }
        }

        public static string AppBarPlay
        {
            get { return ResourceManager.GetString("AppBarPlay", resourceCulture); }
        }

        public static string AppBarPause
        {
            get { return ResourceManager.GetString("AppBarPause", resourceCulture); }
        }

        public static string AppBarNext
        {
            get { return ResourceManager.GetString("AppBarNext", resourceCulture); }
        }

        public static string AppBarPrev
        {
            get { return ResourceManager.GetString("AppBarPrev", resourceCulture); }
        }

        public static string AppBarShuffle
        {
            get { return ResourceManager.GetString("AppBarShuffle", resourceCulture); }
        }

        public static string AppBarSettings
        {
            get { return ResourceManager.GetString("AppBarSettings", resourceCulture); }
        }

        public static string NoTracksMessage
        {
            get { return ResourceManager.GetString("NoTracksMessage", resourceCulture); }
        }

        public static string LastFmSettingsHeader
        {
            get { return ResourceManager.GetString("LastFmSettingsHeader", resourceCulture); }
        }

        public static string LastFmApiKeyLabel
        {
            get { return ResourceManager.GetString("LastFmApiKeyLabel", resourceCulture); }
        }

        public static string LastFmApiSecretLabel
        {
            get { return ResourceManager.GetString("LastFmApiSecretLabel", resourceCulture); }
        }

        public static string LastFmLoginButton
        {
            get { return ResourceManager.GetString("LastFmLoginButton", resourceCulture); }
        }

        public static string LastFmLogoutButton
        {
            get { return ResourceManager.GetString("LastFmLogoutButton", resourceCulture); }
        }

        public static string LastFmAuthenticated
        {
            get { return ResourceManager.GetString("LastFmAuthenticated", resourceCulture); }
        }

        public static string LastFmNotAuthenticated
        {
            get { return ResourceManager.GetString("LastFmNotAuthenticated", resourceCulture); }
        }

        public static string LastFmScrobblingEnabled
        {
            get { return ResourceManager.GetString("LastFmScrobblingEnabled", resourceCulture); }
        }
    }
}
