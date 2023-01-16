using AudictiveMusicUWP.Gui.UC;
using AudictiveMusicUWP.Gui.Util;
using AudictiveMusicUWP.Purchase;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation.Metadata;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static ClassLibrary.Helpers.Enumerators;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Settings : Page
    {
        private string NavigationArguments;
        private double PageWidth;

        private SettingsPageContent view;

        public SettingsPageContent CurrentView
        {
            get
            {
                return view;
            }
            set
            {
                if (value != view)
                    GoToView(value);

                view = value;
            }
        }


        private NavigationMode NavMode
        {
            get;
            set;
        }

        public Settings()
        {
            PageWidth = 0;
            view = SettingsPageContent.Menu;
            this.SizeChanged += Settings_SizeChanged;
            this.NavigationCacheMode = NavigationCacheMode.Disabled;
            this.InitializeComponent();
        }

        private void Settings_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PageWidth = e.NewSize.Width;

            content.Width = e.NewSize.Width;

            if (this.CurrentView == SettingsPageContent.Menu)
            {
                pageFrameTranslate.X = e.NewSize.Width;
                menuTranslate.X = 0;
            }
            else
            {
                pageFrameTranslate.X = 0;
                menuTranslate.X = -e.NewSize.Width;
            }

            if (e.NewSize.Width < 450)
            {
                freeSpaceFlyout.Placement = FlyoutPlacementMode.Full;
            }
            else
            {
                freeSpaceFlyout.Placement = FlyoutPlacementMode.Top;
            }

            content.Padding = pageFrame.Margin = frameContent.Padding = new Thickness(0, 0, 0, ApplicationInfo.Current.FooterHeight);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PageHelper.Settings = this;
            NavMode = e.NavigationMode;

            string arguments = string.Empty;

            if (e.Parameter != null)
                arguments = e.Parameter.ToString();

            if (string.IsNullOrWhiteSpace(arguments) == false)
            {
                this.NavigationArguments = arguments;
            }
            else
            {
                this.CurrentView = SettingsPageContent.Menu;
            }

            ThemesButton.Content = ApplicationInfo.Current.Resources.GetString("Themes");
            OpenPage(NavMode == NavigationMode.Back);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.Settings = null;
            //TitleBarColorCheckBox.Checked -= TitleBarColorCheckBox_Checked;
            //TitleBarColorCheckBox.Unchecked -= TitleBarColorCheckBox_Unchecked;
            //PlaybackBarColorCheckBox.Checked -= PlaybackBarColorCheckBox_Checked;
            //PlaybackBarColorCheckBox.Unchecked -= PlaybackBarColorCheckBox_Unchecked;
            //MenuColorCheckBox.Checked -= MenuColorCheckBox_Checked;
            //MenuColorCheckBox.Unchecked -= MenuColorCheckBox_Unchecked;
            darkRadioButton.Checked -= DarkRadioButton_Checked;
            lightRadioButton.Checked -= LightRadioButton_Checked;
            //ThemeComboBox.SelectionChanged -= ThemeComboBox_SelectionChanged;
            LockScreenToggleSwitch.Toggled -= LockScreenToggleSwitch_Toggled;
            //UseBlurImageCheckBox.Checked -= UseBlurImageCheckBox_Checked;
            //UseBlurImageCheckBox.Unchecked -= UseBlurImageCheckBox_Unchecked;
            //transparentEffectsToggleSwitch.Toggled -= TransparentEffectsToggleSwitch_Toggled;
            celullarDownloadToggleSwitch.Toggled -= CelullarDownloadToggleSwitch_Toggled;
            WhatsNextNotification.Toggled -= WhatsNextNotification_Toggled;
            WhatsNextNotificationSuppressPopup.Toggled -= WhatsNextNotificationSuppressPopup_Toggled;
            TapToResumeSwitch.Toggled -= TapToResumeSwitch_Toggled;

            Storyboard sb = this.Resources["ExitPageTransition"] as Storyboard;
            sb.Begin();
            //StartupPage.SelectionChanged -= StartupPage_SelectionChanged;
        }


        private void LoadSettings()
        {
            if (ApplicationInfo.Current.GetDeviceFormFactorType() != ApplicationInfo.DeviceFormFactorType.Phone)
            {
                LockScreenToggleSwitch.Visibility = Visibility.Collapsed;
            }

            if (ApiInformation.IsMethodPresent("Windows.Storage.StorageLibrary", "RequestAddFolderAsync")
                && ApplicationInfo.Current.GetDeviceFormFactorType() != ApplicationInfo.DeviceFormFactorType.Phone)
            {
                ChooseLibraryBtn.Visibility = Visibility.Visible;
            }

            int theme = ApplicationSettings.AppTheme;

            if (theme == 0)
            {
                darkRadioButton.IsChecked = true;
            }
            else if (theme == 1)
            {
                lightRadioButton.IsChecked = true;
            }
            else if (theme == 2)
            {
                darkRadioButton.IsChecked = true;
            }


            sendInfoYes.IsChecked = ApplicationSettings.DownloadEnabled;
            sendInfoNo.IsChecked = ApplicationSettings.DownloadEnabled == false;

            if (sendInfoYes.IsChecked == true)
                celullarDownloadToggleSwitch.IsEnabled = true;
            else
                celullarDownloadToggleSwitch.IsEnabled = false;


            celullarDownloadToggleSwitch.IsOn = ApplicationSettings.CellularDownloadEnabled;

            LockScreenToggleSwitch.IsOn = ApplicationSettings.LockscreenEnabled;

            if (ApplicationSettings.NextSongInActionCenterEnabled)
                WhatsNextNotificationSuppressPopup.Visibility = Visibility.Visible;
            else
                WhatsNextNotificationSuppressPopup.Visibility = Visibility.Collapsed;

            WhatsNextNotification.IsOn = ApplicationSettings.NextSongInActionCenterEnabled;

            WhatsNextNotificationSuppressPopup.IsOn = ApplicationSettings.NextSongInActionCenterSuppressPopup;

            TapToResumeSwitch.IsOn = ApplicationSettings.TapToResumeNotificationEnabled;



            LoadTimerSettings();

            darkRadioButton.Checked += DarkRadioButton_Checked;
            lightRadioButton.Checked += LightRadioButton_Checked;
            LockScreenToggleSwitch.Toggled += LockScreenToggleSwitch_Toggled;
            celullarDownloadToggleSwitch.Toggled += CelullarDownloadToggleSwitch_Toggled;
            WhatsNextNotification.Toggled += WhatsNextNotification_Toggled;
            WhatsNextNotificationSuppressPopup.Toggled += WhatsNextNotificationSuppressPopup_Toggled;
            TapToResumeSwitch.Toggled += TapToResumeSwitch_Toggled;

            TimerBox.TextChanged += TimerBox_TextChanged;
            try
            {
                BackgroundMediaPlayer.MessageReceivedFromBackground += BackgroundMediaPlayer_MessageReceivedFromBackground;
            }
            catch
            {

            }
        }

        private async void BackgroundMediaPlayer_MessageReceivedFromBackground(object sender, MediaPlayerDataReceivedEventArgs e)
        {
            CurrentStateChangedMessage currentStateChangedMessage;
            if (MessageService.TryParseMessage(e.Data, out currentStateChangedMessage))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    LoadTimerSettings();
                });
            }
        }


        private void LoadTimerSettings()
        {
            if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Closed)
                return;

            TimerCancelButton.IsEnabled = true;

            if (ApplicationSettings.IsPlaybackTimerEnabled)
            {
                TimerBox.IsEnabled = TimerButton.IsEnabled = false;
                TimerBox.Text = ApplicationSettings.PlaybackTimerDuration.ToString();
                TimerBoxCaption.Text = ApplicationInfo.Current.Resources.GetString("TimerBoxCaptionApproximately").Replace("#", Convert.ToString(ApplicationSettings.PlaybackTimerDuration / 4));
            }
            else
            {
                TimerBox.IsEnabled = true;
                TimerButton.IsEnabled = false;
            }
        }

        private void TimerBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimerBox.Text.Length == 0)
            {
                if (ApplicationSettings.IsPlaybackTimerEnabled)
                {
                    TimerButton.IsEnabled = true;
                    TimerBoxCaption.Text = "";
                }
                else
                {
                    TimerButton.IsEnabled = false;
                    TimerBoxCaption.Text = "";
                }
                return;
            }

            int number = Convert.ToInt32(TimerBox.Text);
            if (number >= 4 && number <= 60)
            {
                TimerBoxCaption.Text = ApplicationInfo.Current.Resources.GetString("TimerBoxCaptionApproximately").Replace("#", Convert.ToString(number / 4));
                TimerButton.IsEnabled = true;
            }
            else
            {
                TimerBoxCaption.Text = ApplicationInfo.Current.Resources.GetString("TimerBoxCaptionError");
                TimerButton.IsEnabled = false;
            }

        }

        private void OpenPage(bool reload)
        {
            try
            {
                progress.IsActive = false;
                Storyboard sb = this.Resources["OpenPageTransition"] as Storyboard;

                if (reload)
                {
                    layoutRootScale.ScaleX = layoutRootScale.ScaleY = 1.1;
                }

                sb.Begin();
            }
            catch
            {

            }
        }

        #region EVENTS


        private void LightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["AppTheme"] = 1;
            this.RequestedTheme = ElementTheme.Light;
            PageHelper.MainPage.SetAppTheme();
        }

        private void DarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["AppTheme"] = 0;
            this.RequestedTheme = ElementTheme.Dark;
            PageHelper.MainPage.SetAppTheme();
        }

        private void StartupPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var item = StartupPage.SelectedItem as ComboBoxItem;
            //if (item != null)
            //    ApplicationData.Current.LocalSettings.Values["StartupPage"] = item.Tag.ToString();
            //else
            //    ApplicationData.Current.LocalSettings.Values["StartupPage"] = "Artists";
        }

        private void LockScreenToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["Lockscreen"] = LockScreenToggleSwitch.IsOn;
        }

        private void CelullarDownloadToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["CelullarDownload"] = celullarDownloadToggleSwitch.IsOn;
        }

        private void UseBlurImageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["LockscreenBlur"] = false;
        }

        private void UseBlurImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["LockscreenBlur"] = true;
        }

        private void sendInfoNo_Click(object sender, RoutedEventArgs e)
        {
            if (sendInfoYes.IsChecked == true)
                sendInfoYes.IsChecked = false;
            else
                sendInfoNo.IsChecked = true;

            celullarDownloadToggleSwitch.IsEnabled = false;
            ApplicationData.Current.LocalSettings.Values["Download"] = sendInfoYes.IsChecked == true;
        }

        private void sendInfoYes_Click(object sender, RoutedEventArgs e)
        {
            if (sendInfoNo.IsChecked == true)
                sendInfoNo.IsChecked = false;
            else
                sendInfoYes.IsChecked = true;

            celullarDownloadToggleSwitch.IsEnabled = true;
            ApplicationData.Current.LocalSettings.Values["Download"] = sendInfoYes.IsChecked == true;
        }

        private void WhatsNextNotificationSuppressPopup_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["WhatsNextNotificationSuppressPopup"] = WhatsNextNotificationSuppressPopup.IsOn;
        }

        private void WhatsNextNotification_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["WhatsNextNotification"] = WhatsNextNotification.IsOn;

            if (WhatsNextNotification.IsOn)
                WhatsNextNotificationSuppressPopup.Visibility = Visibility.Visible;
            else
                WhatsNextNotificationSuppressPopup.Visibility = Visibility.Collapsed;
        }

        private void TapToResumeSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["DisplayTapToResumeToast"] = TapToResumeSwitch.IsOn;
        }




        #endregion

        private async void FindYourMusic_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Frame.Navigate(typeof(PreparingCollection));
            PageHelper.MainPage.Frame.BackStack.Clear();
        }

        private void Prospectum_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void WindowsColorSettings_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:personalization-colors", UriKind.RelativeOrAbsolute), new LauncherOptions() { DesiredRemainingView = Windows.UI.ViewManagement.ViewSizePreference.UseHalf });
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }

        private void ChooseLibraryBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PageHelper.MainPage != null)
            {
                PageHelper.MainPage.CreateLibraryPicker();
            }
        }

        private void SettingsGroup_Click(object sender, EventArgs e)
        {
            SettingsGroup sg = sender as SettingsGroup;
            this.CurrentView = sg.ReferencesTo;
        }

        private void SettingsGroup_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            foreach (SettingsGroup sg in content.Children.OfType<SettingsGroup>())
            {
                if (sg != sender as SettingsGroup)
                {
                    Canvas.SetZIndex(sg, 3);
                }
                else
                {
                    Canvas.SetZIndex(sg, 1);
                }
            }
        }

        private void SettingsGroup_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            foreach (SettingsGroup sg in content.Children.OfType<SettingsGroup>())
            {
                Canvas.SetZIndex(sg, 1);
            }
        }

        public void GoToView(SettingsPageContent value)
        {
            Debug.WriteLine("SELECIONADO: " + value.ToString());

            Grid selected = null;

            if (value == SettingsPageContent.AppInfo)
            {
                selected = appInfoSection;
            }
            else if (value == SettingsPageContent.DataManagement)
            {
                selected = dataManagementSection;

                LoadStorageInfo();
            }
            else if (value == SettingsPageContent.Feedback)
            {
                selected = feedbackSection;
            }
            else if (value == SettingsPageContent.Permissions)
            {
                selected = permissionsSection;
            }
            else if (value == SettingsPageContent.Playback)
            {
                selected = playbackSection;
            }
            else if (value == SettingsPageContent.Personalization)
            {
                selected = personalizationSection;
            }
            else if (value == SettingsPageContent.Menu)
            {
                Back();
                return;
            }

            if (selected == null)
                return;

            selected.IsHitTestVisible = true;
            selected.Opacity = 1;

            foreach (Grid g in frameContent.Children)
            {
                if (g != selected)
                {
                    g.IsHitTestVisible = false;
                    g.Opacity = 0;
                }
            }

            Storyboard sb = new Storyboard();

            DoubleAnimation da = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                EnableDependentAnimation = false,
            };

            Storyboard.SetTarget(da, pageFrameTranslate);
            Storyboard.SetTargetProperty(da, "X");


            DoubleAnimation da1 = new DoubleAnimation()
            {
                To = -PageWidth / 3,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                EnableDependentAnimation = false,
            };

            Storyboard.SetTarget(da1, menuTranslate);
            Storyboard.SetTargetProperty(da1, "X");

            sb.Children.Add(da);
            sb.Children.Add(da1);

            sb.Begin();
        }

        private async void LoadStorageInfo()
        {
            storageProgressBar.Visibility = Visibility.Visible;

            if (ApplicationSettings.IsCollectionLoaded)
            {
                songsFound.Text = Ctr_Song.Current.GetAllSongsPaths().Count.ToString();
            }

            artistsSize.Text = albumsSize.Text = otherSize.Text = allSize.Text = string.Empty;

            Task.Run(async () =>
            {
                StorageFolder appFolder = ApplicationData.Current.LocalFolder;

                int appSize = await Package.Current.InstalledLocation.CalculateSize() + await appFolder.CalculateSize();

                StorageFolder artists = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);
                int artSize = await artists.CalculateSize();

                StorageFolder covers = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
                int albSize = await covers.CalculateSize();

                int othSize = appSize - (artSize + albSize);

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    artistsSize.Text = artSize + " MB";
                    albumsSize.Text = albSize + " MB";
                    otherSize.Text = othSize + " MB";
                    allSize.Text = appSize + " MB";

                    storageProgressBar.Visibility = Visibility.Collapsed;
                });
            });
        }

        public void Back()
        {
            Storyboard sb = new Storyboard();

            DoubleAnimation da = new DoubleAnimation()
            {
                To = PageWidth,
                BeginTime = TimeSpan.FromMilliseconds(100),
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                EnableDependentAnimation = false,
            };

            Storyboard.SetTarget(da, pageFrameTranslate);
            Storyboard.SetTargetProperty(da, "X");


            DoubleAnimation da1 = new DoubleAnimation()
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
                EnableDependentAnimation = false,
            };

            Storyboard.SetTarget(da1, menuTranslate);
            Storyboard.SetTargetProperty(da1, "X");

            sb.Children.Add(da);
            sb.Children.Add(da1);

            sb.Begin();
        }

        private void freeSpace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void freeSpaceDoneButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["FreeSpaceArtistsImages"] = freeSpaceArtistsCheckBox.IsChecked;
            ApplicationData.Current.LocalSettings.Values["FreeSpaceCoversImages"] = freeSpaceCoversCheckBox.IsChecked;

            freeSpaceFlyout.Hide();
        }

        private void rescanImages_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void rateButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
    new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }

        private async void supportButton_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=audictivemusic@outlook.com&subject=Audictive Music 10 Support&body=\n\n\n\nAudictive Music: " + ApplicationInfo.Current.AppVersion);
            await Launcher.LaunchUriAsync(mailto);
        }

        private async void donateButton_Click(object sender, RoutedEventArgs e)
        {
            donateButton.IsEnabled = false;

            if (ApplicationInfo.Current.HasInternetConnection)
            {
                try
                {
                    ApplicationData.Current.RoamingSettings.Values["DonatePrompt"] = true;

                    var iapList = await PurchaseHelper.GetDonationsIAP();

                    ComboBox box = new ComboBox()
                    {
                        Margin = new Thickness(10,10,10,0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };


                    Button setButton = new Button()
                    {
                        IsEnabled = false,
                        Content = ApplicationInfo.Current.Resources.GetString("Proceed"),
                        Margin = new Thickness(10,10,10,0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                    };



                    box.SelectionChanged += (snd, args) =>
                    {
                        if (box.SelectedItem != null)
                            setButton.IsEnabled = true;
                    };

                    setButton.Click += async (s, a) =>
                    {
                        ProductListing product = ((ComboBoxItem)box.SelectedItem).Tag as ProductListing;

                        ProductPurchaseStatus result = await PurchaseHelper.PurchaseAsync(product);

                        if (result == ProductPurchaseStatus.Succeeded)
                        {
                            PurchaseHelper.ShowDonationThanksMessage();
                        }
                        else
                        {
                            PurchaseHelper.ShowDonationErrorMessage();
                        }
                    };

                    foreach (ProductListing product in iapList)
                    {
                        ComboBoxItem cbi = new ComboBoxItem()
                        {
                            Tag = product,
                            Content = product.FormattedPrice,
                        };
                        box.Items.Add(cbi);
                    }

                    Grid.SetColumn(box, 0);
                    Grid.SetColumn(setButton, 1);

                    PageHelper.MainPage.Notification.SetContent(ApplicationInfo.Current.Resources.GetString("AskForDonationTitle"),
ApplicationInfo.Current.Resources.GetString("AskForDonationMessage"),
"", new System.Collections.Generic.List<UIElement>() { box, setButton });

                    PageHelper.MainPage.Notification.Show();

                }
                catch
                {

                    //await nf1.Show(res.GetString("AskForDonationErrorMessage"), res.GetString("AskForDonationErrorTitle"), btn1, btn2);
                }
            }
            else
            {

            }

            donateButton.IsEnabled = true;
        }


        private async void joinTelegramButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://t.me/audictivemusic", UriKind.Absolute));
        }

        private async void getTelegramButton_Click(object sender, RoutedEventArgs e)
        {
            string uri;
            if (ApplicationInfo.Current.GetDeviceFormFactorType() == ApplicationInfo.DeviceFormFactorType.Phone)
                uri = "https://www.microsoft.com/store/productId/9WZDNCRDZHS0";
            else
                uri = "https://www.microsoft.com/store/productId/9NZTWSQNTD0S";

            await Launcher.LaunchUriAsync(new Uri(uri, UriKind.Absolute));
        }

        private void OpenPageTransition_Completed(object sender, object e)
        {
            LoadSettings();
            HandleNavigation();
        }

        private void HandleNavigation()
        {
            string path = NavigationHelper.GetParameter(this.NavigationArguments, "path");

            if (path == "personalization")
            {
                this.CurrentView = SettingsPageContent.Personalization;
            }
            else if (path == "dataManagement")
            {
                this.CurrentView = SettingsPageContent.DataManagement;
            }
            else if (path == "permissions")
            {
                this.CurrentView = SettingsPageContent.Permissions;
            }
            else if (path == "playback")
            {
                this.CurrentView = SettingsPageContent.Playback;
            }
            else if (path == "feedback")
            {
                this.CurrentView = SettingsPageContent.Feedback;
            }
            else if (path == "appInfo")
            {
                this.CurrentView = SettingsPageContent.AppInfo;
            }
            else if (path == "menu")
            {
                this.CurrentView = SettingsPageContent.Menu;
            }
        }

        private void TimerButton_Click(object sender, RoutedEventArgs e)
        {
            int number = Convert.ToInt32(TimerBox.Text);
            SetPlaybackTimer(number);
        }

        private void SetPlaybackTimer(int minutes)
        {
            long starterTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ApplicationSettings.PlaybackTimerDuration = minutes;
            ApplicationSettings.PlaybackTimerStartTime = starterTime;
            TimerBox.IsEnabled = TimerButton.IsEnabled = false;
        }

        private void TimerCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelPlaybackTimer();
        }

        private void CancelPlaybackTimer()
        {
            TimerBox.IsEnabled = true;
            ApplicationSettings.PlaybackTimerDuration = 0;
            ApplicationSettings.PlaybackTimerStartTime = 0;
            TimerBox.Text = "";
        }

        private void Themes_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(ThemeSelector));
        }
    }
}
