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

namespace AudictiveMusicUWP.Helpers
{
    public class ApplicationInfo
    {
        /// <summary>
        /// Define e retorna o contador para a exibição do flyout
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

        public bool MenuIsHidden
        {
            get
            {
                return WindowSize.Width <= 400;
            }
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

        public static Type GetStartupPageType()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("StartupPage"))
            {
                string page = ApplicationData.Current.LocalSettings.Values["StartupPage"].ToString();

                if (page == "Artists")
                {
                    return typeof(Artists);
                }
                else if (page == "Albums")
                {
                    return typeof(Albums);
                }
                else if (page == "Songs")
                {
                    return typeof(Songs);
                }
                else if (page == "Playlists")
                {
                    return typeof(Playlists);
                }
                //else if (page == "Favorites")
                //{
                //    return typeof(Favorites);
                //}
                else
                {
                    return typeof(Artists);
                }
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["StartupPage"] = "Artists";
                return typeof(Artists);
            }
        }

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
            LoadCoversFolder();
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
    }
}
