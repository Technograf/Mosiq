using BackgroundAudioShared;
using ClassLibrary.Control;
using ClassLibrary.Dao;
using ClassLibrary.Entities;
using ClassLibrary.Themes;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI;

namespace ClassLibrary.Helpers
{
    /// <summary>
    /// Collection of string constants used in the entire solution. This file is shared for all projects
    /// </summary>
    public static class ApplicationSettings
    {
        public delegate void RoutedEventArgs();
        public static event RoutedEventArgs BlurLevelChanged;
        public static event RoutedEventArgs NowPlayingThemeChanged;
        public static event RoutedEventArgs CurrentThemeColorChanged;
        public static event RoutedEventArgs ThemeBackgroundPreferenceChanged;

        public static AppState AppState
        {
            get
            {
                object value = ReadSettingsValue("AppState");
                if (value == null)
                    return AppState.Unknown;
                else
                    return (AppState)Enum.Parse(typeof(AppState), value.ToString());
            }
            set
            {
                SaveSettingsValue("AppState", value.ToString());
            }
        }

        public static string CurrentTrackPath
        {
            get
            {
                object value = ReadSettingsValue("CurrentTrackPath");
                if (value == null)
                    return string.Empty;
                else
                    return (string)value;
            }
            set
            {
                SaveSettingsValue("CurrentTrackPath", value);
            }
        }

        public static double PlaybackLastPosition
        {
            get
            {
                object value = ReadSettingsValue("PlaybackLastPosition");
                if (value == null)
                    return 0;
                else
                    return Convert.ToDouble(value);
            }
            set
            {
                SaveSettingsValue("PlaybackLastPosition", value);
            }
        }

        public static bool IsCollectionLoaded
        {
            get
            {
                object setting = ReadSettingsValue("CollectionLoaded");
                if (setting == null)
                    return false;
                else
                    return (bool)setting;
            }
            set
            {
                SaveSettingsValue("CollectionLoaded", value);
            }
        }


        public static Song CurrentSong
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CurrentTrackPath))
                    return null;

                return SongDao.GetSong(new Song() { SongURI = CurrentTrackPath });
            }
        }

        public static int CurrentTrackIndex
        {
            get
            {
                object value = ReadSettingsValue("CurrentTrackIndex");
                if (value == null)
                    return 0;
                else
                    return (int)value;
            }
            set
            {
                SaveSettingsValue("CurrentTrackIndex", value);
            }
        }

        public static string LastFmSessionToken
        {
            get
            {
                object value = ReadSettingsValue("LastFmSessionToken");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                SaveSettingsValue("LastFmSessionToken", value);
            }
        }

        public static string LastFmSessionUsername
        {
            get
            {
                object value = ReadSettingsValue("LastFmSessionUsername");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                SaveSettingsValue("LastFmSessionUsername", value);
            }
        }

        public static string LastFmSessionUserImageUri
        {
            get
            {
                object value = ReadSettingsValue("LastFmSessionUserImageUri");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                SaveSettingsValue("LastFmSessionUserImageUri", value);
            }
        }

        public static string LastFmAPIKey
        {
            get
            {
                object value = ReadSettingsValue("LastFmAPIKey");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                SaveSettingsValue("LastFmAPIKey", value);
            }
        }

        public static string LastFmSecret
        {
            get
            {
                object value = ReadSettingsValue("LastFmSecret");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                SaveSettingsValue("LastFmSecret", value);
            }
        }

        public static bool IsScrobbleEnabled
        {
            get
            {
                object value = ReadSettingsValue("ScrobbleEnabled");
                if (value == null)
                    return true;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("ScrobbleEnabled", value);
            }
        }

        public static bool IsPlaybackTimerEnabled
        {
            get => PlaybackTimerDuration != 0;
        }

        public static int PlaybackTimerDuration
        {
            get
            {
                object value = ReadSettingsValue("PlaybackTimerDuration");
                if (value == null)
                    return 0;
                else
                    return (int)value;
            }
            set
            {
                SaveSettingsValue("PlaybackTimerDuration", value);
            }
        }

        public static int PlaybackTimerDurationInSeconds
        {
            get => PlaybackTimerDuration * 60;
        }

        public static long PlaybackTimerEndTime
        {
            get => PlaybackTimerStartTime + PlaybackTimerDurationInSeconds;
        }

        public static long PlaybackTimerStartTime
        {
            get
            {
                object value = ReadSettingsValue("PlaybackTimerStartTime");
                if (value == null)
                    return 0;
                else
                    return (long)value;
            }
            set
            {
                SaveSettingsValue("PlaybackTimerStartTime", value);
            }
        }


        public static Theme NowPlayingTheme
        {
            get
            {
                object value = ReadSettingsValue("NowPlayingTheme");
                if (value == null)
                    return Theme.Material;
                else
                {
                    Theme t = (Theme)Enum.Parse(typeof(Theme), value.ToString());
                    return t;
                }
            }
            set
            {
                SaveSettingsValue("NowPlayingTheme", value.ToString());

                NowPlayingThemeChanged?.Invoke();
            }
        }


        public static bool ThemesUserAware
        {
            get
            {
                object value = ReadSettingsValue("ThemesUserAware", SettingsContainerType.RoamingSettings);
                if (value == null)
                    return false;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("ThemesUserAware", value, SettingsContainerType.RoamingSettings);
            }
        }

        public static bool PlaylistReorderUserAware
        {
            get
            {
                object value = ReadSettingsValue("PlaylistReorderUserAware", SettingsContainerType.RoamingSettings);
                if (value == null)
                    return false;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("PlaylistReorderUserAware", value, SettingsContainerType.RoamingSettings);
            }
        }

        /// <summary>
        /// Retorna 0 para capa do álbum e 1 para imagem do artista
        /// </summary>
        public static int ThemeBackgroundPreference
        {
            get
            {
                object value = ReadSettingsValue("ThemeBackgroundPreference");
                if (value == null)
                    return 1;
                else
                    return (int)value;
            }
            set
            {
                SaveSettingsValue("ThemeBackgroundPreference", value);

                ThemeBackgroundPreferenceChanged?.Invoke();
            }
        }


        /// <summary>
        /// Retorna 0 se usa a cor do álbum, 1 de usa a cor do sistema e 2 se usa cor personalizada
        /// </summary>
        public static int ThemeColorPreference
        {
            get
            {
                object value = ReadSettingsValue("ThemeColorPreference");
                if (value == null)
                    return 0;
                else
                    return (int)value;
            }
            set
            {
                SaveSettingsValue("ThemeColorPreference", value);

                NowPlayingThemeChanged?.Invoke();
            }
        }

        public static Color CurrentThemeColor
        {
            get
            {
                object value = ReadSettingsValue("CurrentThemeColor");
                if (value == null)
                    return ImageHelper.GetColorFromHex("#FFDC572E");
                else
                    return ImageHelper.GetColorFromHex(value.ToString());
            }
            set
            {
                //if (CurrentThemeColor != value)
                //{
                    SaveSettingsValue("CurrentThemeColor", ImageHelper.GetHexFromColor(value));

                    CurrentThemeColorChanged?.Invoke();
                //}
            }
        }

        public static Color CustomThemeColor
        {
            get
            {
                object value = ReadSettingsValue("CustomThemeColor");
                if (value == null)
                    return ImageHelper.GetColorFromHex("#FFDC572E");
                else
                    return ImageHelper.GetColorFromHex(value.ToString());
            }
            set
            {
                SaveSettingsValue("CustomThemeColor", ImageHelper.GetHexFromColor(value));

                CurrentThemeColor = value;
            }
        }


        public static float NowPlayingBlurAmount
        {
            get
            {
                object value = ReadSettingsValue("NowPlayingBlurAmount");
                if (value == null)
                    return 10;
                else
                    return (float)value;
            }
            set
            {
                SaveSettingsValue("NowPlayingBlurAmount", value);

                BlurLevelChanged?.Invoke();
            }
        }

        public static bool NowPlayingGrayscale
        {
            get
            {
                object value = ReadSettingsValue("NowPlayingGrayscale");
                if (value == null)
                    return true;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("NowPlayingGrayscale", value);
            }
        }

        public static bool LockscreenEnabled
        {
            get
            {
                object value = ReadSettingsValue("Lockscreen");
                if (value == null)
                    return false;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("Lockscreen", value);
            }
        }

        public static bool CellularDownloadEnabled
        {
            get
            {
                object value = ReadSettingsValue("CelullarDownload");
                if (value == null)
                    return false;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("CelullarDownload", value);
            }
        }

        public static bool DownloadEnabled
        {
            get
            {
                object value = ReadSettingsValue("Download");
                if (value == null)
                    return false;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("Download", value);
            }
        }

        public static int AppTheme
        {
            get
            {
                object value = ReadSettingsValue("AppTheme");
                if (value == null)
                    return 0;
                else
                    return (int)value;
            }
            set
            {
                SaveSettingsValue("AppTheme", value);
            }
        }

        public static bool NextSongInActionCenterEnabled
        {
            get
            {
                object value = ReadSettingsValue("WhatsNextNotification");
                if (value == null)
                    return true;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("WhatsNextNotification", value);
            }
        }

        public static bool NextSongInActionCenterSuppressPopup
        {
            get
            {
                object value = ReadSettingsValue("WhatsNextNotificationSuppressPopup");
                if (value == null)
                    return true;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("WhatsNextNotificationSuppressPopup", value);
            }
        }

        public static bool TapToResumeNotificationEnabled
        {
            get
            {
                object value = ReadSettingsValue("DisplayTapToResumeToast");
                if (value == null)
                    return true;
                else
                    return (bool)value;
            }
            set
            {
                SaveSettingsValue("DisplayTapToResumeToast", value);
            }
        }


        /// <summary>
        /// Function to read a setting value
        /// </summary>
        public static object ReadSettingsValue(string key, SettingsContainerType type = SettingsContainerType.LocalSettings)
        {
            ApplicationDataContainer dataContainer;

            if (type == SettingsContainerType.LocalSettings)
                dataContainer = ApplicationData.Current.LocalSettings;
            else
                dataContainer = ApplicationData.Current.RoamingSettings;

            if (!dataContainer.Values.ContainsKey(key))
            {
                Debug.WriteLine("null returned");
                return null;
            }
            else
            {
                var value = dataContainer.Values[key];
                Debug.WriteLine("value found " + value.ToString());
                return value;
            }
        }

        /// <summary>
        /// Save a key value pair in settings. Create if it doesn't exist
        /// </summary>
        public static void SaveSettingsValue(string key, object value, SettingsContainerType type = SettingsContainerType.LocalSettings)
        {
            Debug.WriteLine(key + ":" + (value == null ? "null" : value.ToString()));

            ApplicationDataContainer dataContainer;

            if (type == SettingsContainerType.LocalSettings)
                dataContainer = ApplicationData.Current.LocalSettings;
            else
                dataContainer = ApplicationData.Current.RoamingSettings;

            if (!dataContainer.Values.ContainsKey(key))
            {
                dataContainer.Values.Add(key, value);
            }
            else
            {
                dataContainer.Values[key] = value;
            }
        }

        public enum SettingsContainerType
        {
            LocalSettings,
            RoamingSettings
        }

    }
}
