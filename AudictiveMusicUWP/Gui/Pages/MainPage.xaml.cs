using AudictiveMusicUWP.Gui.UC;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ActionableNotification Notification { get => notice; }

        public PlayerControl Player
        {
            get { return player; }
        }

        private bool IsAppOpened { get; set; }

        public Frame PageFrame
        {
            get
            {
                return MainFrame;
            }
        }

        public bool IsNoticeVisible
        {
            get
            {
                return notice.IsVisible;
            }
        }


        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual footerBlurSprite;
        private SpriteVisual backgroundHostSprite;
        private SpriteVisual menuHostSprite;
        private SpriteVisual titleBarHostSprite;
        private CoreApplicationViewTitleBar coreTitleBar;
        public SearchPane searchGrid;
        public PlaylistPicker playlistPicker;
        public MusicLibraryPicker libraryPicker;
        public NextTooltip nextTooltip;

        public MainPage()
        {
            IsAppOpened = false;
            Application.Current.UnhandledException += Current_UnhandledException;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

            coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

            //Window.Current.SizeChanged += Current_SizeChanged;
            Window.Current.Activated += Current_Activated;
            this.SizeChanged += MainPage_SizeChanged;
            this.Loaded += MainPage_Loaded;
            this.InitializeComponent();
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            Application.Current.Suspending += Current_Suspending;
            playlistPicker = null;
            searchGrid = null;

            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
       CoreDispatcher_AcceleratorKeyActivated;

            ApplicationSettings.CurrentThemeColorChanged += ApplicationSettings_CurrentThemeColorChanged;
            //Window.Current.CoreWindow.CharacterReceived += CoreWindow_CharacterReceived;
        }

        private void ApplicationSettings_CurrentThemeColorChanged()
        {
            UpdateThemeColor(ApplicationSettings.CurrentThemeColor);
        }

        //private void CoreWindow_CharacterReceived(CoreWindow sender, CharacterReceivedEventArgs args)
        //{
        //    args.Handled = true;
        //    char c = Convert.ToChar(args.KeyCode);
        //    CreateSearchGrid();
        //    searchGrid.SetInitialChar(c);
        //}

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Escape && args.KeyStatus.IsKeyReleased)
            {
                args.Handled = true;
                RemovePicker();

                return;
            }

            var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);

            if (ctrl && args.VirtualKey == VirtualKey.S && args.KeyStatus.IsKeyReleased)
            {
                args.Handled = true;
                CreateSearchGrid();

                return;
            }
        }

        private async void Current_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            MessageDialog md = new MessageDialog(e.Message);
            await md.ShowAsync();
        }

        private async void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                if (ApplicationInfo.Current.IsMobile == false)
                    titleBar.Opacity = 1;

                Debug.WriteLine("Evento activated ocorreu");


                Collection.LoadCollectionChanges();

                SetAppTheme();
            }
            else
            {
                if (ApplicationInfo.Current.IsMobile == false)
                    titleBar.Opacity = 0.7;
            }
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetUpFluentDesign();
        }

        public void SetUpFluentDesign()
        {
            bool transparencyEnabled = false;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableTransparency"))
            {
                transparencyEnabled = (bool)ApplicationData.Current.LocalSettings.Values["EnableTransparency"];
            }
            else
            {
                transparencyEnabled = true;
            }

            if (transparencyEnabled)
            {
                menuHostSprite = _compositor.CreateSpriteVisual();
                titleBarHostSprite = _compositor.CreateSpriteVisual();
                footerBlurSprite = _compositor.CreateSpriteVisual();
                backgroundHostSprite = _compositor.CreateSpriteVisual();
            }
            else
            {
                menuHostSprite = null;
                footerBlurSprite = null;
                titleBarHostSprite = null;
                backgroundHostSprite = null;
            }

            if (transparencyEnabled)
            {

                if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
                {
                    if (ApplicationInfo.Current.IsMobile == false && ApplicationInfo.Current.IsTabletModeEnabled == false)
                    {
                        /// transparência no menu
                        //acrillic.Visibility = Visibility.Visible;
                        //menuHostSprite.Size = new Vector2((float)acrillic.ActualWidth, (float)acrillic.ActualHeight);

                        //menuHostSprite.Brush = _compositor.CreateHostBackdropBrush();

                        //ElementCompositionPreview.SetElementChildVisual(acrillic, menuHostSprite);


                        /// transparência no fundo
                        acrillicPageBG.Visibility = Visibility.Visible;
                        backgroundHostSprite.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);

                        backgroundHostSprite.Brush = _compositor.CreateHostBackdropBrush();

                        ElementCompositionPreview.SetElementChildVisual(acrillicPageBG, backgroundHostSprite);


                        /// transparência na barra de título
                        titleBarAcrillic.Visibility = Visibility.Visible;
                        titleBarHostSprite.Size = new Vector2((float)this.ActualWidth, (float)coreTitleBar.Height);

                        titleBarHostSprite.Brush = _compositor.CreateHostBackdropBrush();

                        ElementCompositionPreview.SetElementChildVisual(titleBarAcrillic, titleBarHostSprite);
                    }
                }
            }


            /*acrillic.Visibility = */acrillicPageBG.Visibility = titleBarAcrillic.Visibility = transparencyEnabled ? Visibility.Visible : Visibility.Collapsed;

            footerBlurSprite = _compositor.CreateSpriteVisual();

            BlendEffectMode blendmode = BlendEffectMode.Overlay;

            var graphicsEffect = new BlendEffect
            {
                Mode = blendmode,
                Background = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Colors.Transparent,
                },

                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("Backdrop"),
                    BlurAmount = 18.0f,
                    BorderMode = EffectBorderMode.Hard,
                }
            };

            var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect,
                new[] { "Blur.BlurAmount", "Tint.Color" });

            _brush = blurEffectFactory.CreateBrush();

            var destinationBrush = _compositor.CreateBackdropBrush();
            _brush.SetSourceParameter("Backdrop", destinationBrush);

            footerBlurSprite.Size = new Vector2((float)Footer.ActualWidth, (float)Footer.ActualHeight);
            footerBlurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(footerBlur, footerBlurSprite);


            //if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.XamlCompositionBrushBase")
            //    && Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.AcrylicBrush"))
            //{
            //    Windows.UI.Xaml.Media.AcrylicBrush hostBackdropBrush = new Windows.UI.Xaml.Media.AcrylicBrush();
            //    hostBackdropBrush.BackgroundSource = Windows.UI.Xaml.Media.AcrylicBackgroundSource.HostBackdrop;
            //    hostBackdropBrush.FallbackColor = Colors.Transparent;

            //    acrillicPageBG.Background = acrillic.Background = titleBarDragArea.Background = hostBackdropBrush;
            //}
            //else
            //{
            //    acrillicPageBG.Background = acrillic.Background = (SolidColorBrush)Application.Current.Resources["ApplicationPageBackgroundThemeBrush"];
            //}

        }

        private void SetTitleBar()
        {
            coreTitleBar.ExtendViewIntoTitleBar = true;
            //Window.Current.SetTitleBar(null);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
            titleBar.Height = coreTitleBar.Height;
            titleBarTitle.Text = ApplicationInfo.Current.AppPackageName.ToUpper();
            Window.Current.SetTitleBar(titleBarDragArea);
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            //if (titleBarHostSprite != null)
            //{
            //    titleBarHostSprite.Size = new Size(this.ActualWidth, sender.Height).ToVector2();
            //}

            titleBar.Height = sender.Height;
            rightColumn.Width = new GridLength(sender.SystemOverlayRightInset, GridUnitType.Pixel);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (titleBarHostSprite != null)
            {
                titleBarHostSprite.Size = new Size(this.ActualWidth, sender.Height).ToVector2();
            }

            titleBar.Visibility = sender.IsVisible ? Visibility.Visible : Visibility.Visible;
        }

        private void Current_Resuming(object sender, object e)
        {
            player.InitializePlayer();
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (menuHostSprite != null)
            {
                menuHostSprite.Size = e.NewSize.ToVector2();
            }

            if (backgroundHostSprite != null)
            {
                backgroundHostSprite.Size = e.NewSize.ToVector2();
            }

            if (titleBarHostSprite != null)
            {
                titleBarHostSprite.Size = new Size(this.ActualWidth, coreTitleBar.Height).ToVector2();
            }

            SetMenuLayout(e.NewSize);



            if (e.NewSize.Width <= 400)
            {
                //acrillic.Visibility = Visibility.Collapsed;
                //menu.DisplayMode = SplitViewDisplayMode.Overlay;
                //menu.IsPaneOpen = false;
                ////MenuBackgroundColor.Opacity = 1;
                //MainFrame.Margin = new Thickness(0);
            }
            else if (e.NewSize.Width > 400 && e.NewSize.Width <= 700)
            {
                //if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
                //    acrillic.Visibility = Visibility.Visible;
                //menu.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                //menu.IsPaneOpen = false;
                //MainFrame.Margin = new Thickness(54, 0, 0, 0);
                ////MenuBackgroundColor.Opacity = 1;
            }
            else if (e.NewSize.Width > 700)
            {
                //if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
                //    acrillic.Visibility = Visibility.Visible;
                //menu.DisplayMode = SplitViewDisplayMode.CompactInline;
                //menu.IsPaneOpen = true;
                //MainFrame.Margin = new Thickness(274, 0, 0, 0);
                ////MenuBackgroundColor.Opacity = 0.8;
            }
        }

        private void SetMenuLayout(Size newSize)
        {
            //mainAppSolidBG.Margin = new Thickness(0, 0, 0, ApplicationInfo.Current.FooterHeight);

            if (ApplicationInfo.Current.IsWideView == false)
            {
                footerBlur.Height = ApplicationInfo.Current.FooterHeight;
                player.SetCompactViewMargin(new Thickness(0, 0, 0, 50));

                if (player.Mode == PlayerControl.DisplayMode.Compact)
                    bottomNavBar.Visibility = Visibility.Visible;

                leftNavBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                footerBlur.Height = 60;
                player.SetCompactViewMargin(new Thickness(0, 0, 0, 0));
                bottomNavBar.Visibility = Visibility.Collapsed;
                leftNavBar.Visibility = Visibility.Visible;
            }
        }

        public async void UpdateThemeColor(Color newColor)
        {
            if (newColor.IsDarkColor())
            {
                leftNavBar.RequestedTheme = bottomNavBar.RequestedTheme = ElementTheme.Dark;

                if (ApplicationInfo.Current.IsMobile == false)
                {
                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;

                    titleBar.RequestedTheme = ElementTheme.Dark;
                }
            }
            else
            {
                leftNavBar.RequestedTheme = bottomNavBar.RequestedTheme = ElementTheme.Light;

                if (ApplicationInfo.Current.IsMobile == false)
                {
                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;

                    titleBar.RequestedTheme = ElementTheme.Light;
                }
            }

            //await Task.Delay(100);

            Storyboard sb = new Storyboard();

            ColorAnimation ca = new ColorAnimation()
            {
                To = newColor,
                Duration = TimeSpan.FromMilliseconds(395),
                EnableDependentAnimation = true,
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(ca, leftNavBar);
            Storyboard.SetTargetProperty(ca, "Tint");

            sb.Children.Add(ca);

            ColorAnimation ca1 = new ColorAnimation()
            {
                To = newColor,
                Duration = TimeSpan.FromMilliseconds(395),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut },
            };

            Storyboard.SetTarget(ca1, footerBackgroundColor);
            Storyboard.SetTargetProperty(ca1, "Color");

            sb.Children.Add(ca1);

            ColorAnimation ca2 = new ColorAnimation()
            {
                To = newColor,
                Duration = TimeSpan.FromMilliseconds(395),
                EnableDependentAnimation = false,
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(ca2, titleBarBackgroundBrush);
            Storyboard.SetTargetProperty(ca2, "Color");

            sb.Children.Add(ca2);

            await Task.Delay(100);
            sb.Begin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Application.Current.Resuming -= Current_Resuming;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetAppTheme();

            PageHelper.MainPage = this;

            Collection.LoadCollectionChanges();

            Application.Current.Suspending += Current_Suspending;
            Application.Current.Resuming += Current_Resuming;

            Navigate(typeof(StartPage), null);

            bool isResumingPlayback = false;

            string arguments = string.Empty;

            if (e.Parameter != null)
                arguments = e.Parameter.ToString();

            if (string.IsNullOrWhiteSpace(arguments) == false)
            {
                if (NavigationHelper.ContainsAttribute(arguments, "action"))
                {
                    if (NavigationHelper.GetParameter(arguments, "action") == "resumePlayback")
                    {
                        if (this.IsAppOpened == false)
                            player.InitializePlayer();

                        isResumingPlayback = true;
                        PlayPauseOrResume();
                        OpenPlayer();
                    }
                    else if (NavigationHelper.GetParameter(arguments, "action") == "playEverything")
                    {
                        if (this.IsAppOpened == false)
                            player.InitializePlayer();
                        MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.PlayEverything));
                        OpenPlayer();
                    }
                    else if (NavigationHelper.GetParameter(arguments, "action") == "navigate")
                    {
                        player.InitializePlayer();

                        string target = NavigationHelper.GetParameter(arguments, "target");
                        string path = NavigationHelper.GetParameter(arguments, "path");

                        if (string.IsNullOrEmpty(target) == false)
                        {
                            if (target == "settings")
                            {
                                Navigate(typeof(Settings), "path=" + path);
                            }
                        }
                    }
                }
            }
            else
            {
                player.InitializePlayer();

                if (BackgroundMediaPlayer.Current != null)
                {
                    if (BackgroundMediaPlayer.Current.PlaybackSession.PlaybackState != MediaPlaybackState.None || isResumingPlayback)
                        OpenPlayer();
                }
            }

            IsAppOpened = true;
        }

        public async void SetAppTheme()
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AppTheme"))
            {
                switch ((int)ApplicationData.Current.LocalSettings.Values["AppTheme"])
                {
                    case 0:
                        //if (ApplicationInfo.Current.ColorEnabled == false)
                        //{
                        //    if (ApplicationInfo.Current.IsMobile == false)
                        //        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                        //}
                            this.RequestedTheme = ElementTheme.Dark;



                        break;
                    case 1:
                        //if (ApplicationInfo.Current.ColorEnabled == false)
                        //{
                        //    if (ApplicationInfo.Current.IsMobile == false)
                        //        ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                        //}

                        this.RequestedTheme = ElementTheme.Light;

                        break;
                    case 2:
                        //if (ApplicationInfo.Current.ColorEnabled == false)
                        //{
                        //    if (ApplicationInfo.Current.IsMobile == false)
                        //    {
                        //        if ((bool)Application.Current.Resources["IsDarkTheme"])
                        //        {
                        //            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                        //        }
                        //        else
                        //        {
                        //            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                        //        }
                        //    }
                        //}
                        this.RequestedTheme = ElementTheme.Default;

                        break;
                }
            }
            else
            {
                //if (ApplicationInfo.Current.ColorEnabled == false)
                //{
                //    if (ApplicationInfo.Current.IsMobile == false)
                //    {
                //        if ((bool)Application.Current.Resources["IsDarkTheme"])
                //        {
                //            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                //        }
                //        else
                //        {
                //            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                //        }
                //    }
                //}
                this.RequestedTheme = ElementTheme.Default;
            }




            if (ApplicationInfo.Current.IsMobile == false)
            {
                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Colors.Transparent;
                    ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                    ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                }
            }
            else
            {
                await StatusBar.GetForCurrentView().ShowAsync();

                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
                StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                StatusBar.GetForCurrentView().BackgroundColor = Colors.Black;
            }

        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            player.ClearPlayerState(true);
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            e.Handled = GoBack();
        }

        //private void SideMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.IsPaneOpen)
        //        CloseMenu();
        //    else
        //        OpenMenu();
        //}

        //private void CloseMenu()
        //{
        //    if (ApplicationInfo.Current.WindowSize.Width > 400)
        //    {
        //        MainFrame.Margin = new Thickness(54, 0, 0, 0);
        //    }

        //    //MenuBackgroundColor.Opacity = 1;

        //    menu.IsPaneOpen = false;
        //}

        //private void OpenMenu()
        //{
        //    menu.IsPaneOpen = true;

        //    if (ApplicationInfo.Current.WindowSize.Width > 700)
        //    {
        //        MainFrame.Margin = new Thickness(274, 0, 0, 0);
        //        //MenuBackgroundColor.Opacity = 0.8;
        //    }
        //    else if (ApplicationInfo.Current.WindowSize.Width > 400 && ApplicationInfo.Current.WindowSize.Width <= 700)
        //    {
        //        MainFrame.Margin = new Thickness(54, 0, 0, 0);
        //        //MenuBackgroundColor.Opacity = 1;
        //    }
        //    else
        //    {
        //        MainFrame.Margin = new Thickness(0, 0, 0, 0);
        //        //MenuBackgroundColor.Opacity = 1;
        //    }
        //}

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (BackButton != null)
            {
                //BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
                BackButton.Content = MainFrame.CanGoBack ? "" : "";
                titleBarLogo.Visibility = MainFrame.CanGoBack ? Visibility.Collapsed : Visibility.Visible;
            }

            if (MainFrame.SourcePageType == typeof(StartPage))
            {
                bottomNavBar.SyncNavigationState(NavigationBar.NavigationView.Home);
                leftNavBar.SyncNavigationState(NavigationBar.NavigationView.Home);
            }

            else if (MainFrame.SourcePageType == typeof(CollectionPage)
                || MainFrame.SourcePageType == typeof(ArtistPage)
                || MainFrame.SourcePageType == typeof(AlbumPage)
                || MainFrame.SourcePageType == typeof(FolderPage))
            {
                bottomNavBar.SyncNavigationState(NavigationBar.NavigationView.Collection);
                leftNavBar.SyncNavigationState(NavigationBar.NavigationView.Collection);
            }

            else if (MainFrame.SourcePageType == typeof(Playlists)
                || MainFrame.SourcePageType == typeof(PlaylistPage)
                || MainFrame.SourcePageType == typeof(Favorites))
            {
                bottomNavBar.SyncNavigationState(NavigationBar.NavigationView.Playlists);
                leftNavBar.SyncNavigationState(NavigationBar.NavigationView.Playlists);
            }
            else if (MainFrame.SourcePageType == typeof(CloudPage)
                || MainFrame.SourcePageType == typeof(ClPlaylistPage)
                || MainFrame.SourcePageType == typeof(ClSearchPage))
            {
                bottomNavBar.SyncNavigationState(NavigationBar.NavigationView.Cloud);
                leftNavBar.SyncNavigationState(NavigationBar.NavigationView.Cloud);
            }

            else
            {
                bottomNavBar.SyncNavigationState(NavigationBar.NavigationView.Unknown);
                leftNavBar.SyncNavigationState(NavigationBar.NavigationView.Unknown);
            }
        }

        //private void CheckMenuButton(ToggleButton button)
        //{
        //    if (button == settingsButton)
        //        settingsButton.IsChecked = true;
        //    else
        //        settingsButton.IsChecked = false;

        //    foreach (FrameworkElement e in buttonsContainer.Children)
        //    {
        //        if (e.GetType() == typeof(ToggleButton))
        //        {
        //            if ((ToggleButton)e != button)
        //                ((ToggleButton)e).IsChecked = false;
        //            else
        //                ((ToggleButton)e).IsChecked = true;

        //        }
        //    }
        //}

        //private void StartMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(StartPage), null);
        //}

        //private void ArtistsMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(Artists), null);
        //}

        //private void AlbumsMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(Albums), null);
        //}

        //private void SongsMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(Songs), null);
        //}

        //private void NowPlayingButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    //if (MainFrame.CurrentSourcePageType != typeof(NowPlaying))
        //    //Navigate(typeof(NowPlaying));
        //    if (player.Mode == PlayerControl.DisplayMode.Compact)
        //        player.Mode = PlayerControl.DisplayMode.Full;
        //}

        //private void searchButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;

        //    CreateSearchGrid();
        //}

        //private void SettingsButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(Settings), "path=menu");
        //}

        //private void PlaylistsMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Navigate(typeof(Playlists), null);

        //    if (menu.DisplayMode == SplitViewDisplayMode.Overlay || menu.DisplayMode == SplitViewDisplayMode.CompactOverlay)
        //        CloseMenu();
        //}

        //private void CollectionMenuButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (menu.DisplayMode == SplitViewDisplayMode.CompactOverlay || menu.DisplayMode == SplitViewDisplayMode.Overlay)
        //        menu.IsPaneOpen = false;
        //    Navigate(typeof(CollectionPage), null);
        //}


        private void CreateSongPopup(Song song, object sender, Point point)
        {
            this.ShowPopupMenu(song, sender, point, Enumerators.MediaItemType.Song);
        }

        //private void showMenu_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        //{
        //    if (e.Cumulative.Translation.X > 100 && menu.IsPaneOpen == false)
        //        OpenMenu();
        //}

        private void albumImageMenu_ImageOpened(object sender, RoutedEventArgs e)
        {

        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            PlayPauseOrResume();
        }

        private async void PlayPauseOrResume()
        {
            if (BackgroundMediaPlayer.Current.PlaybackSession.PlaybackState != MediaPlaybackState.Playing
    && BackgroundMediaPlayer.Current.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
            {
                MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.Resume));

                return;
            }

            MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.PlayPause));

        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.SkipToPrevious));
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.SkipToNext));
        }

        private void BottomBarTapToResumeMessageGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlayPauseOrResume();
        }

        private void PlayerBottomBarInfo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private void PlayerBottomBarInfo_PointerExited(object sender, PointerRoutedEventArgs e)
        {

        }

        private void PlayerBottomBarInfo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void PlayerBottomBarInfo_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageHelper.MainPage?.IsNoticeVisible == true || MainFrame.CanGoBack == true)
            {
                GoBack();
            }
            else
            {
                Navigate(typeof(About), null);
            }
        }

        public void OpenPlayer()
        {
            player.Mode = PlayerControl.DisplayMode.Full;
        }

        public bool GoBack()
        {
            bool handled = false;

            handled = RemovePicker();

            if (player.IsPlaylistOpened)
            {
                player.HidePlaylist();

                handled = true;
            }

            if (PageHelper.MainPage?.IsNoticeVisible == true)
            {
                PageHelper.MainPage?.HideEmptyLibraryNotice();
                handled = true;
            }

            if (handled == false)
            {
                if (PageHelper.Settings != null)
                {
                    if (PageHelper.Settings.CurrentView == Enumerators.SettingsPageContent.Menu)
                    {
                        handled = false;
                    }
                    else
                    {
                        PageHelper.Settings.CurrentView = Enumerators.SettingsPageContent.Menu;
                        handled = true;
                    }
                }
            }

            if (handled == false)
            {
                if (MainFrame.CanGoBack)
                {
                    MainFrame.GoBack();
                    handled = true;
                }
            }

            return handled;
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationView.GetForCurrentView().IsFullScreenMode == false)
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            else
                ApplicationView.GetForCurrentView().ExitFullScreenMode();
        }

        private void titleBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationInfo.Current.IsMobile == false)
            {
                SetTitleBar();
            }
            else
            {
                titleBar.Visibility = Visibility.Collapsed;
            }
        }

        private void titleBarLogo_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        public async void CreateAddToPlaylistPopup(List<string> songs)
        {
            playlistPicker = new PlaylistPicker();

            customPopupsArea.Children.Add(playlistPicker);

            List<Playlist> playlists = await CustomPlaylistsHelper.GetPlaylists();

            playlistPicker.Set(playlists, songs);

            customPopupsArea.Visibility = Visibility.Visible;
        }

        public void CreateSearchGrid()
        {
            if (searchGrid != null)
                return;

            searchGrid = new SearchPane();

            customPopupsArea.Children.Add(searchGrid);

            customPopupsArea.Visibility = Visibility.Visible;
        }

        public void CreateLibraryPicker()
        {
            libraryPicker = new MusicLibraryPicker();

            customPopupsArea.Children.Add(libraryPicker);

            customPopupsArea.Visibility = Visibility.Visible;

            libraryPicker.LoadFolders();
        }

        public bool RemovePicker()
        {
            bool handled = false;

            if (playlistPicker != null)
            {
                if (customPopupsArea.Children.Contains(playlistPicker))
                {
                    customPopupsArea.Children.Remove(playlistPicker);
                    playlistPicker = null;

                    Debug.WriteLine("REMOVEU PLAYLIST PICKER!");

                    return true;
                }
            }

            else if (searchGrid != null)
            {
                if (customPopupsArea.Children.Contains(searchGrid))
                {
                    customPopupsArea.Children.Remove(searchGrid);
                    searchGrid = null;

                    Debug.WriteLine("REMOVEU SEARCH GRID!");

                    return true;
                }
            }
            else if (libraryPicker != null)
            {
                if (customPopupsArea.Children.Contains(libraryPicker))
                {
                    customPopupsArea.Children.Remove(libraryPicker);
                    libraryPicker = null;

                    Debug.WriteLine("REMOVEU LIBRARY PICKER!");

                    return true;
                }
            }
            else if (player.IsPlaylistOpened)
            {
                player.HidePlaylist();

                Debug.WriteLine("FECHOU PLAYLIST!");

                return true;
            }
            else if (player.Mode == PlayerControl.DisplayMode.Full)
            {
                player.Mode = PlayerControl.DisplayMode.Compact;

                Debug.WriteLine("FECHOU PLAYER!");

                return true;
            }

            customPopupsArea.Visibility = Visibility.Collapsed;

            return false;
        }

        public bool RemoveMusicLibraryPopup()
        {
            bool handled = false;

            if (libraryPicker != null)
            {
                if (customPopupsArea.Children.Contains(libraryPicker))
                {
                    handled = true;
                    customPopupsArea.Children.Remove(libraryPicker);
                }
            }

            libraryPicker = null;
            customPopupsArea.Visibility = Visibility.Collapsed;

            return handled;
        }


        private void showMenu_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private async void contactSupportButton_Click(object sender, RoutedEventArgs e)
        {
            var mailto = new Uri("mailto:?to=audictivemusic@outlook.com&subject=Audictive Music 10 Support&body=\n\n\n\nAudictive Music: " + ApplicationInfo.Current.AppVersion);
            await Launcher.LaunchUriAsync(mailto);
        }

        private void favoritesButton_Click(object sender, RoutedEventArgs e)
        {
            //Navigate(typeof(Favorites));

            //if (menu.DisplayMode == SplitViewDisplayMode.Overlay || menu.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            //    CloseMenu();
        }

        public void Navigate(Type targetPage, object parameter = null)
        {
            while (RemovePicker())
            {
            }

            MainFrame.Navigate(targetPage, parameter);
        }

        private void nowPlaying_Drop(object sender, DragEventArgs e)
        {
            List<string> songs = new List<string>();
            if (e.DataView.Properties.ContainsKey("mediaItem"))
            {
                var mediaItem = e.DataView.Properties["mediaItem"];

                if (mediaItem == null)
                    return;

                if (mediaItem is Artist)
                {
                    Artist art = mediaItem as Artist;

                    var temp = Ctr_Song.Current.GetSongsByArtist(art);

                    foreach (Song song in temp)
                        songs.Add(song.SongURI);

                }
                else if (mediaItem is Album)
                {
                    Album alb = mediaItem as Album;

                    var temp = Ctr_Song.Current.GetSongsByAlbum(alb);

                    foreach (Song song in temp)
                        songs.Add(song.SongURI);
                }
                else if (mediaItem is Song)
                {
                    Song song = mediaItem as Song;

                    songs.Add(song.SongURI);
                }
                else if (mediaItem is Playlist)
                {
                    Playlist plt = mediaItem as Playlist;
                    songs = plt.Songs;
                }

                MessageService.SendMessageToBackground(new AddSongsToPlaylist(songs));
            }
        }

        private void nowPlaying_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("mediaItem"))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        private void playlistsButton_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey("mediaItem")
                || e.DataView.Properties.ContainsKey("currentPlayingSong"))
                e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private void playlistsButton_Drop(object sender, DragEventArgs e)
        {
            List<string> songs = new List<string>();

            object mediaItem = null;

            if (e.DataView.Properties.ContainsKey("mediaItem"))
            {
                mediaItem = e.DataView.Properties["mediaItem"];

                if (mediaItem == null)
                    return;

                if (mediaItem is Artist)
                {
                    Artist art = mediaItem as Artist;

                    var temp = Ctr_Song.Current.GetSongsByArtist(art);

                    foreach (Song song in temp)
                    {
                        songs.Add(song.SongURI);
                    }
                }
                else if (mediaItem is Album)
                {
                    Album alb = mediaItem as Album;

                    var temp = Ctr_Song.Current.GetSongsByAlbum(alb);

                    foreach (Song s in temp)
                        songs.Add(s.SongURI);
                }
                else if (mediaItem is Song)
                {
                    Song song = mediaItem as Song;

                    songs.Add(song.SongURI);
                }
                else if (mediaItem is Playlist)
                {
                    Playlist plt = mediaItem as Playlist;
                    songs = plt.Songs;
                }
            }
            else if (e.DataView.Properties.ContainsKey("currentPlayingSong"))
            {
                mediaItem = e.DataView.Properties["currentPlayingSong"];

                if (mediaItem == null)
                    return;

                if (mediaItem is Song)
                {
                    Song song = mediaItem as Song;

                    songs.Add(song.SongURI);
                }
            }

            if (PageHelper.MainPage != null)
                PageHelper.MainPage.CreateAddToPlaylistPopup(songs);
        }

        private void PlayerBottomBarInfo_DragStarting(UIElement sender, DragStartingEventArgs args)
        {
            Song song = Ctr_Song.Current.GetSong(new Song() { SongURI = ApplicationSettings.CurrentTrackPath });

            if (song != null)
                args.Data.Properties.Add("currentPlayingSong", song);
            else
                args.Cancel = true;
        }

        public void ShowEmptyLibraryNotice()
        {
            string subtitle;
            if (ApplicationInfo.Current.GetDeviceFormFactorType() == ApplicationInfo.DeviceFormFactorType.Desktop
                || ApplicationInfo.Current.GetDeviceFormFactorType() == ApplicationInfo.DeviceFormFactorType.Tablet)
            {
                subtitle = ApplicationInfo.Current.Resources.GetString("EmptyLibraryDesktopTip");
                Notification.PrimaryActionVisibility = Visibility.Collapsed;
                Notification.SecondaryActionVisibility = Visibility.Visible;
                Notification.SecondaryActionContent = ApplicationInfo.Current.Resources.GetString("SettingsString");
                Notification.SecondaryActionClick += (s, a) => { Navigate(typeof(Settings), "path=dataManagement"); };
            }
            else
            {
                subtitle = ApplicationInfo.Current.Resources.GetString("EmptyLibraryMobileTip");
                Notification.PrimaryActionVisibility = Visibility.Collapsed;
                Notification.SecondaryActionVisibility = Visibility.Collapsed;
            }

            Notification.SetContent(ApplicationInfo.Current.Resources.GetString("EmptyLibrary"), subtitle, "");

            Notification.Show();
        }

        public void HideEmptyLibraryNotice()
        {
            notice.Hide();
        }

        public void CreatePlayerTooltip(NextTooltip.Mode mode)
        {
            if (BackgroundMediaPlayer.Current.PlaybackSession.PlaybackState == MediaPlaybackState.None)
                return;

            string prev = string.Empty;
            string next = string.Empty;
            string status;
            string title;
            string subtitle;
            Uri source;
            Song song;
            //Color color;

            if (mode == NextTooltip.Mode.Previous)
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("PreviousSong"))
                {
                    prev = ApplicationData.Current.LocalSettings.Values["PreviousSong"].ToString();
                }

                if (string.IsNullOrWhiteSpace(prev) || prev == ApplicationSettings.CurrentTrackPath)
                    return;

                song = Ctr_Song.Current.GetSong(new Song() { SongURI = prev });
                title = song.Title;
                subtitle = song.Artist;
                source = new Album() { AlbumID = song.AlbumID }.GetCoverUri();
                status = ApplicationInfo.Current.Resources.GetString("Previous").ToUpper();
                //color = ImageHelper.GetColorFromHex(song.HexColor);
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NextSong"))
                {
                    next = ApplicationData.Current.LocalSettings.Values["NextSong"].ToString();
                }

                if (string.IsNullOrWhiteSpace(next) || next == ApplicationSettings.CurrentTrackPath)
                    return;

                song = Ctr_Song.Current.GetSong(new Song() { SongURI = next });
                title = song.Title;
                subtitle = song.Artist;
                source = new Album() { AlbumID = song.AlbumID }.GetCoverUri();
                status = ApplicationInfo.Current.Resources.GetString("Next").ToUpper();
                //color = ImageHelper.GetColorFromHex(song.HexColor);
            }

            nextTooltip = new NextTooltip()
            {
                Status = status,
                Title = title,
                Subtitle = subtitle,
                AccentColor = ImageHelper.GetColorFromHex(song.HexColor),
                Source = new BitmapImage(source),
                Margin = new Thickness(0, 0, 0, ApplicationInfo.Current.FooterHeight),
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                //BackgroundColor = color,
            };

            customPopupsArea.Visibility = Visibility.Visible;
            customPopupsArea.Children.Add(nextTooltip);

        }

        public void RemovePlayerTooltip()
        {
            if (nextTooltip != null)
            {
                customPopupsArea.Children.Remove(nextTooltip);
                customPopupsArea.Visibility = Visibility.Collapsed;
            }
        }

        private void previousButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            CreatePlayerTooltip(NextTooltip.Mode.Previous);
        }

        private void nextButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
                return;

            CreatePlayerTooltip(NextTooltip.Mode.Next);
        }

        private void previousButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (nextTooltip != null)
            {
                customPopupsArea.Children.Remove(nextTooltip);
                customPopupsArea.Visibility = Visibility.Collapsed;
            }
        }

        private void nextButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (nextTooltip != null)
            {
                customPopupsArea.Children.Remove(nextTooltip);
                customPopupsArea.Visibility = Visibility.Collapsed;
            }
        }

        private void player_ViewChanged(PlayerControl.DisplayMode mode)
        {
            if (mode == PlayerControl.DisplayMode.Compact)
            {
                SetMenuLayout(new Size(this.ActualWidth, this.ActualHeight));

            }
            else
                bottomNavBar.Visibility = Visibility.Collapsed;
        }

        private void bottomNavBar_ActionRequested(NavigationBar.NavigationView target)
        {
            if (target == NavigationBar.NavigationView.Home)
                Navigate(typeof(StartPage), null);
            else if (target == NavigationBar.NavigationView.Collection)
                Navigate(typeof(CollectionPage), null);
            else if (target == NavigationBar.NavigationView.Playlists)
                Navigate(typeof(Playlists), null);
            else if (target == NavigationBar.NavigationView.Cloud)
                Navigate(typeof(CloudPage), null);
            else if (target == NavigationBar.NavigationView.Search)
                CreateSearchGrid();
        }

        private void Footer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (footerBlurSprite != null)
            {
                footerBlurSprite.Size = e.NewSize.ToVector2();
            }
        }
    }
}
