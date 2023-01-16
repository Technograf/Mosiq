using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.Phone.Devices.Notification;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI;
using BackgroundAudioShared;
using Windows.ApplicationModel.Store;
using System.Collections.Generic;

namespace ClassLibrary.Helpers
{
    public class ApplicationInfo
    {
        public delegate void RoutedEventArgs(ProductListing product);
        public static event RoutedEventArgs PurchaseFailed;
        public static event RoutedEventArgs PurchaseSucceeded;

        /// <summary>
        /// Define e retorna o contador do RepeatButton que controla a exibição do flyout do Álbum
        /// </summary>
        public int AlbumFlyoutCycleCounter
        {
            get;
            set;
        }

        public StorageFolder CoversFolder
        {
            get;
            private set;
        }

        private ResourceLoader resources = new ResourceLoader();
        public ResourceLoader Resources { get => resources; }

        public Color CurrentSystemAccentColor
        {
            get
            {
                SolidColorBrush brush = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush;
                return brush.Color;
            }
        }

        public string Language
        {
            get => Resources.GetString("Language");
        }
        public bool ColorEnabled
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UpdateMenuColor"))
                {
                    return (bool)ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"];
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"] = true;
                    return true;
                }
            }
        }

        /// <summary>
        /// If possible, vibrate the device
        /// </summary>
        /// <param name="duration">Duration in milliseconds</param>
        public void VibrateDevice(int duration)
        {
            if (!ApiInformation.IsTypePresent("Windows.Phone.Devices.Notification.VibrationDevice"))
                return;

            VibrationDevice v = VibrationDevice.GetDefault();
            v.Vibrate(TimeSpan.FromMilliseconds(duration));
        }

        public bool IsTransparencyEnabled
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableTransparency"))
                {
                    return (bool)ApplicationData.Current.LocalSettings.Values["EnableTransparency"];
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsWideView
        {
            get
            {
                return WindowSize.Width > 500;
            }
        }

        public double FooterHeight
        {
            get
            {
                if (IsWideView)
                    return 60;
                else
                    return 110;
            }
        }

        public string AppPackageName
        {
            get
            {
                return Package.Current.DisplayName;
            }
        }

        public string AppVersion
        {
            get
            {
                return String.Format("{0}.{1}.{2}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build);
            }
        }

        public string GetSingularPlural(int a, string resourceIdentifier)
        {
            string s = "";


            if (a > 1)
            {
                try
                {
                    s = this.Resources.GetString(resourceIdentifier + "Plural");
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    s = this.Resources.GetString(resourceIdentifier + "Singular");
                }
                catch
                {

                }

            }

            return s;
        }

        public bool IsMobile
        {
            get
            {
                return ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar");
            }
        }

        public bool IsTabletModeEnabled
        {
            get
            {
                return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
            }
        }

        public double ScreenScaleFactor
        {
            get
            {
                return DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            }
        }

        public double TitleBarHeight
        {
            get
            {
                CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                return coreTitleBar.Height;
            }
        }

        /// <summary>
        /// Returns the size of the usable area of the app (title bar not included)
        /// </summary>
        public Size AppArea
        {
            get
            {
                if (IsMobile)
                {
                    return new Size(WindowSize.Width, WindowSize.Height);
                }
                else
                {
                    return new Size(WindowSize.Width, WindowSize.Height - TitleBarHeight);
                }
            }
        }

        public Rect WindowSize
        {
            get
            {
                return ApplicationView.GetForCurrentView().VisibleBounds;
            }
        }

        public bool IsSecondViewEnabled
        {
            get
            {
                return WindowSize.Width >= 800;
            }
        }

        private static ApplicationInfo instance;

        public static ApplicationInfo Current
        {
            get
            {
                if (instance == null)
                    instance = new ApplicationInfo();

                return instance;
            }
        }

        public DeviceFormFactorType GetDeviceFormFactorType()
        {
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    return DeviceFormFactorType.Phone;
                case "Windows.Desktop":
                    return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse
                        ? DeviceFormFactorType.Desktop
                        : DeviceFormFactorType.Tablet;
                case "Windows.Universal":
                    return DeviceFormFactorType.IoT;
                case "Windows.Team":
                    return DeviceFormFactorType.SurfaceHub;
                default:
                    return DeviceFormFactorType.Other;
            }
        }

        public enum DeviceFormFactorType
        {
            Phone,
            Desktop,
            Tablet,
            IoT,
            SurfaceHub,
            Other
        }

        //public static Type GetStartupPageType()
        //{
        //    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("StartupPage"))
        //    {
        //        string page = ApplicationData.Current.LocalSettings.Values["StartupPage"].ToString();

        //        if (page == "Artists")
        //        {
        //            return typeof(Artists);
        //        }
        //        else if (page == "Albums")
        //        {
        //            return typeof(Albums);
        //        }
        //        else if (page == "Songs")
        //        {
        //            return typeof(Songs);
        //        }
        //        else if (page == "Playlists")
        //        {
        //            return typeof(Playlists);
        //        }
        //        //else if (page == "Favorites")
        //        //{
        //        //    return typeof(Favorites);
        //        //}
        //        else
        //        {
        //            return typeof(Artists);
        //        }
        //    }
        //    else
        //    {
        //        ApplicationData.Current.LocalSettings.Values["StartupPage"] = "Artists";
        //        return typeof(Artists);
        //    }
        //}

        public async Task<StorageFile> GetCoverFile(string albumID)
        {
            IStorageItem it = await CoversFolder.TryGetItemAsync("cover_" + albumID + ".jpg");
            if (it != null)
            {
                return it as StorageFile;
            }
            else
            {
                return null;
            }
        }

        public ApplicationInfo()
        {
            //LoadCoversFolder();
        }

        private async void LoadCoversFolder()
        {
            try
            {
                CoversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
            }
            catch
            {

            }
        }

        public bool HasInternetConnection
        {
            get
            {
                var profile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                    return false;
                return profile.GetNetworkConnectivityLevel() ==
                       Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;
            }
        }
    }
}
