using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Albums : Page //, INotifyPropertyChanged
    {
        private bool updated;

        public event PropertyChangedEventHandler PropertyChanged;

        public double AlbumItemLength
        {
            get
            {
                //return itemLenght;
                return (double)GetValue(AlbumItemLengthProperty);
            }
            set
            {
                //itemLenght = value;
                SetValue(AlbumItemLengthProperty, value);
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumItemLenght"));
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumItemLengthProperty =
            DependencyProperty.Register("AlbumItemLength", typeof(double), typeof(Albums), new PropertyMetadata(100));


        private NavigationMode NavMode
        {
            get;
            set;
        }

        public bool CollectionHasBeenUpdated
        {
            get
            {
                return updated;
            }
            set
            {
                updated = value;
                if (value == true)
                {
                    LoadAlbums();
                }
            }
        }

        private int RepeatButtonCycleCounter;
        private bool ClickEventIsTouch;
        private bool CursorEnteredMegaFlyout;
        private Thickness TouchFlyoutMargin;
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual selectionBlurSprite;
        private SpriteVisual _hostSprite;
        private SpriteVisual _hostSpritemenu;

        public Albums()
        {
            this.SizeChanged += Albums_SizeChanged;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.Loaded += Albums_Loaded;
            CollectionHasBeenUpdated = false;
            this.InitializeComponent();
            RepeatButtonCycleCounter = 0;
            ClickEventIsTouch = false;
            CursorEnteredMegaFlyout = false;
            TouchFlyoutMargin = new Thickness();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Albums_Loaded(object sender, RoutedEventArgs e)
        {
            if (BuildInfo.BeforeAprilUpdate)
                return;

            selectionLabelBackground.Opacity = 0.5;

            BlendEffectMode blendmode = BlendEffectMode.Overlay;

            // Create a chained effect graph using a BlendEffect, blending color and blur
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

            // Create EffectBrush, BackdropBrush and SpriteVisual
            _brush = blurEffectFactory.CreateBrush();

            var destinationBrush = _compositor.CreateBackdropBrush();
            _brush.SetSourceParameter("Backdrop", destinationBrush);

            selectionBlurSprite = _compositor.CreateSpriteVisual();
            selectionBlurSprite.Size = new Vector2((float)0, (float)0);
            selectionBlurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(selectionBlur, selectionBlurSprite);
        }

        private void Albums_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 510)
            {
                AlbumItemLength = (e.NewSize.Width - 55) / 2;
            }
            else if (e.NewSize.Width >= 510 && e.NewSize.Width < 610)
            {
                AlbumItemLength = (e.NewSize.Width - 80) / 3;
            }
            else if (e.NewSize.Width >= 610 && e.NewSize.Width < 710)
            {
                AlbumItemLength = (e.NewSize.Width - 100) / 4;
            }
            else if (e.NewSize.Width >= 710 && e.NewSize.Width < 810)
            {
                AlbumItemLength = (e.NewSize.Width - 110) / 5;
            }
            //else if (e.NewSize.Width >= 810)
            //{
            //    AlbumItemLength = 150;
            //}
            else
            {
                AlbumItemLength = 180;

            }

            selectionGrid.Margin = new Thickness(20, 20, 20, ApplicationInfo.Current.FooterHeight + 20);

            //pageTitle.Text = AlbumItemLength.ToString() + " / " + e.NewSize.Width.ToString();

            //var panel = (ItemsWrapGrid)AlbumsList.ItemsPanelRoot;
            //panel.ItemWidth = AlbumItemLength;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            progress.IsActive = true;
            Storyboard sb = this.Resources["ExitPageTransition"] as Storyboard;
            sb.Begin();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;

            PageHelper.Albums = this;

            if (ApplicationInfo.Current.IsMobile == false)
            {
                AlbumsList.ItemContainerTransitions.Add(new EntranceThemeTransition() { FromVerticalOffset = 250, IsStaggeringEnabled = true });
            }
            else
            {
                AlbumsList.ItemContainerTransitions.Add(new EntranceThemeTransition() { FromVerticalOffset = 250, IsStaggeringEnabled = false });
            }

            if (((CollectionViewSource)Resources["ListOfAlbums"]).Source == null || CollectionHasBeenUpdated)
            {
                CollectionHasBeenUpdated = false;
                OpenPage(NavMode == NavigationMode.Back);
                await Task.Run(() => LoadAlbums());
            }
            else
            {
                OpenPage(NavMode == NavigationMode.Back);
            }
        }


        public async void LoadAlbums()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                ((CollectionViewSource)Resources["ListOfAlbums"]).Source = null;
            });

            List<Album> listOfAlbums = Ctr_Album.Current.GetAlbums();
            List<AlphaKeyGroup<Album>> itemSource;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AlbumsSortBy"))
            {
                if (ApplicationData.Current.LocalSettings.Values["AlbumsSortBy"].ToString() == Sorting.SortByTitle)
                {
                    //listOfAlbums.OrderBy(s => s.Name);
                    itemSource = AlphaKeyGroup<Album>.CreateGroups(listOfAlbums,
    CultureInfo.InvariantCulture,
    a => a.Name, true);
                }
                else if (ApplicationData.Current.LocalSettings.Values["AlbumsSortBy"].ToString() == Sorting.SortByArtist)
                {
                    //listOfAlbums.OrderBy(s => s.Artist);
                    itemSource = AlphaKeyGroup<Album>.CreateGroups(listOfAlbums,
    CultureInfo.InvariantCulture,
    a => a.Artist, true);
                }
                else if (ApplicationData.Current.LocalSettings.Values["AlbumsSortBy"].ToString() == Sorting.SortByYear)
                {
                    //listOfAlbums.OrderBy(s => s.Year);
                    itemSource = AlphaKeyGroup<Album>.CreateGroups(listOfAlbums,
    CultureInfo.InvariantCulture,
    a => Convert.ToString(a.Year), true);
                }
                else
                {
                    itemSource = AlphaKeyGroup<Album>.CreateGroups(listOfAlbums,
    CultureInfo.InvariantCulture,
    a => a.Name, true);
                }
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["AlbumsSortBy"] = Sorting.SortByTitle;
                itemSource = AlphaKeyGroup<Album>.CreateGroups(listOfAlbums,
    CultureInfo.InvariantCulture,
    a => a.Name, true);
            }

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                ((CollectionViewSource)Resources["ListOfAlbums"]).Source = itemSource;
            });
        }

        private void OpenPage(bool reload)
        {
            progress.IsActive = false;
            Storyboard sb = this.Resources["OpenPageTransition"] as Storyboard;

            if (reload)
            {
                layoutRootScale.ScaleX = layoutRootScale.ScaleY = 1.1;
            }

            sb.Begin();
        }
        private void AlbumsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (AlbumsList.SelectionMode == ListViewSelectionMode.None)
            {
                ApplicationData.Current.LocalSettings.Values["UseTransition"] = true;
                PageHelper.MainPage.Navigate(typeof(AlbumPage), e.ClickedItem);
            }
        }

        private void AlbumCover_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                To = 1,
                BeginTime = TimeSpan.FromMilliseconds(200),
                Duration = TimeSpan.FromMilliseconds(1200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTargetProperty(da, "Opacity");
            Storyboard.SetTarget(da, sender as Image);

            sb.Children.Add(da);

            sb.Begin();
        }




        private void albumItemOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {

            }
        }

        private void albumItemOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {

            }
        }

        private void Album_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                Album album = (sender as FrameworkElement).DataContext as Album;

                CreateAlbumPopup(album, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void CreateAlbumPopup(Album album, object sender, Point point)
        {
            this.ShowPopupMenu(album, sender, point, Enumerators.MediaItemType.Album);
        }

        private void playAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Album alb = btn.DataContext as Album;

            List<Song> songs = Ctr_Song.Current.GetSongsByAlbum(alb);
            List<string> list = new List<string>();
            foreach (Song s in songs)
                list.Add(s.SongURI);
            MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> songs = Ctr_Song.Current.GetAllSongsPaths();

            Random rng = new Random();
            int n = songs.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = songs[k];
                songs[k] = songs[n];
                songs[n] = value;
            }

            MessageService.SendMessageToBackground(new SetPlaylistMessage(songs));
        }

        private void AlbumsList_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.AlbumsList, 0), 0);
            sv.ViewChanged += Sv_ViewChanged;
        }

        private void Sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            AlbumItemHelper.PointerIsInContact = false;
        }


        private void pageFlyout_Closed(object sender, EventArgs e)
        {
            pageFlyout.IsHitTestVisible = false;
        }

        private void pageFlyout_Opened(object sender, EventArgs e)
        {
            pageFlyout.IsHitTestVisible = true;
        }

        private void Album_RightTapped(object sender, RoutedEventArgs e)
        {
            Album album = (sender as FrameworkElement).DataContext as Album;

            CreateAlbumPopup(album, sender, new Point(0,0));
        }

        private void AlbumItem_LongHover(object sender, object context)
        {
            if (AlbumsList.SelectionMode == ListViewSelectionMode.None)
                pageFlyout.Show(typeof(AlbumPage), context, false);
        }



        private void AlbumItem_MenuTriggered(object sender, Point point)
        {
            Album album = (sender as FrameworkElement).DataContext as Album;

            //if (
            CreateAlbumPopup(album, sender, point);
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            DisableSelectionMode();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlbumsList.SelectionMode == ListViewSelectionMode.None)
                EnableSelectionMode();
            else
                DisableSelectionMode();
        }

        private void EnableSelectionMode()
        {
            selectButton.Content = "";
            AlbumsList.SelectionMode = ListViewSelectionMode.Multiple;
            AlbumsList.SelectionChanged += AlbumsList_SelectionChanged;
            topAppBar.Visibility = Visibility.Visible;
        }

        private void DisableSelectionMode()
        {
            selectButton.Content = "";
            AlbumsList.SelectedItem = null;
            AlbumsList.SelectionChanged -= AlbumsList_SelectionChanged;
            AlbumsList.SelectionMode = ListViewSelectionMode.None;
            topAppBar.Visibility = Visibility.Collapsed;
        }

        private void ActivateSelecionMode(Album album)
        {
            if (AlbumsList.SelectedItems.Contains(album))
                return;

            ApplicationInfo.Current.VibrateDevice(25);
            EnableSelectionMode();
            AlbumsList.SelectedItems.Add(album);
        }

        private void AlbumsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumsList.SelectedItems.Count > 0)
            {
                topPlay.IsEnabled = topAdd.IsEnabled = topMore.IsEnabled = true;
            }
            else
            {
                topPlay.IsEnabled = topAdd.IsEnabled = topMore.IsEnabled = false;
                selectedItemsLabel.Text = string.Empty;
                selectedItemsLabel.Visibility = Visibility.Collapsed;
                return;
            }

            int i = AlbumsList.SelectedItems.Count;

            string s = i + " " + ApplicationInfo.Current.GetSingularPlural(i, "ItemSelected");

            selectedItemsLabel.Text = s;
            selectedItemsLabel.Visibility = Visibility.Visible;
        }



        private void topPlay_Click(object sender, RoutedEventArgs e)
        {
            List<string> listSongs = new List<string>();

            foreach (Album album in AlbumsList.SelectedItems)
            {
                var songs = Ctr_Song.Current.GetSongsByAlbum(album);

                foreach (Song s in songs)
                    listSongs.Add(s.SongURI);
            }

            MessageService.SendMessageToBackground(new SetPlaylistMessage(listSongs));

            DisableSelectionMode();
        }

        private void topAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> listSongs = new List<string>();

            foreach (Album album in AlbumsList.SelectedItems)
            {
                var songs = Ctr_Song.Current.GetSongsByAlbum(album);

                foreach (Song s in songs)
                    listSongs.Add(s.SongURI);
            }

            PageHelper.MainPage.CreateAddToPlaylistPopup(listSongs);

            DisableSelectionMode();

        }

        private void topMore_Click(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();

            foreach (Album album in AlbumsList.SelectedItems)
            {
                var songs = Ctr_Song.Current.GetSongsByAlbum(album);
                foreach (Song s in songs)
                    list.Add(s.SongURI);
            }

            MenuFlyout menu = new MenuFlyout()
            {
                MenuFlyoutPresenterStyle = Application.Current.Resources["MenuFlyoutModernStyle"] as Style,
            };

            MenuFlyoutItem item1 = new MenuFlyoutItem()
            {
                Text = ApplicationInfo.Current.Resources.GetString("Play"),
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };
            item1.Click += (s, a) =>
            {
                MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
            };

            menu.Items.Add(item1);

            menu.Items.Add(new MenuFlyoutSeparator());

            MenuFlyoutItem item2 = new MenuFlyoutItem()
            {
                Text = ApplicationInfo.Current.Resources.GetString("AddToPlaylist"),
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };
            item2.Click += (s, a) =>
            {
                MessageService.SendMessageToBackground(new AddSongsToPlaylist(list));
            };

            menu.Items.Add(item2);

            MenuFlyoutItem item3 = new MenuFlyoutItem()
            {
                Text = ApplicationInfo.Current.Resources.GetString("AddToPlaylistFile"),
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };
            item3.Click += (s, a) =>
            {
                if (PageHelper.MainPage != null)
                {
                    PageHelper.MainPage.CreateAddToPlaylistPopup(list);
                }
            };

            menu.Items.Add(item3);

            MenuFlyoutItem item4 = new MenuFlyoutItem()
            {
                Text = ApplicationInfo.Current.Resources.GetString("PlayNext"),
                Tag = "\uEA52",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };
            item4.Click += (s, a) =>
            {
                MessageService.SendMessageToBackground(new AddSongsToPlaylist(list, true));
            };

            menu.Items.Add(item4);

            menu.Items.Add(new MenuFlyoutSeparator());

            MenuFlyoutItem item5 = new MenuFlyoutItem()
            {
                Text = ApplicationInfo.Current.Resources.GetString("Share"),
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };
            item5.Click += async (s, a) =>
            {
                if (await this.ShareMediaItem(list, Enumerators.MediaItemType.Song) == false)
                {
                    MessageDialog md = new MessageDialog(ApplicationInfo.Current.Resources.GetString("ShareErrorMessage"));
                    await md.ShowAsync();
                }
            };

            menu.Items.Add(item5);

            menu.ShowAt(sender as FrameworkElement);


        }

        private void AlbumItem_LongPressed(object sender, LongPressEventArgs args)
        {
            Album album = (sender as FrameworkElement).DataContext as Album;
            if (args.TriggeredByTouch && AlbumsList.SelectionMode == ListViewSelectionMode.None)
                ActivateSelecionMode(album);
        }

        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (((SemanticZoom)sender).IsZoomedInViewActive)
            {
                selectionGrid.Visibility = Visibility.Visible;
            }
            else
            {
                selectionGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void selection_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DisableSelectionMode();
        }

        private void selectionBlur_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (selectionBlurSprite != null)
            {
                selectionBlurSprite.Size = e.NewSize.ToVector2();
            }
        }
    }
}
