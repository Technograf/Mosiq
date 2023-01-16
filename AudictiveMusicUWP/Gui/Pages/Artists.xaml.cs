using AudictiveMusicUWP.Gui.UC;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class Artists : Page
    {
        public double GridItemSize
        {
            get
            {
                //return itemLenght;
                return (double)GetValue(GridItemSizeProperty);
            }
            set
            {
                //itemLenght = value;
                SetValue(GridItemSizeProperty, value);
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumItemLenght"));
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GridItemSizeProperty =
            DependencyProperty.Register("GridItemSize", typeof(double), typeof(Artists), new PropertyMetadata(100));

        //private ItemsWrapGrid artistsWrapGrid;
        private ResourceLoader res;
        private bool updated;

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
                    LoadArtists();
                }
            }
        }

        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual selectionBlurSprite;
        private SpriteVisual _hostSprite;
        private SpriteVisual _hostSpritemenu;

        public Artists()
        {
            res = new ResourceLoader();
            CollectionHasBeenUpdated = false;
            this.SizeChanged += Artists_SizeChanged;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.Loaded += Artists_Loaded;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Artists_Loaded(object sender, RoutedEventArgs e)
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

        //private void artistsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    this.artistsWrapGrid = sender as ItemsWrapGrid;

        //    if (this.ActualWidth < 510)
        //    {
        //        artistsWrapGrid.MaximumRowsOrColumns = 1;

        //    }
        //    else
        //    {
        //        artistsWrapGrid.MaximumRowsOrColumns = Convert.ToInt32(this.ActualWidth / 100);
        //    }
        //}

        private void Artists_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 510)
            {
                this.GridItemSize = (e.NewSize.Width - 60) / 3;

                //ArtistsList.ItemTemplate = this.Resources["listItemTemplate"] as DataTemplate;
                //if (artistsWrapGrid != null)
                //    artistsWrapGrid.MaximumRowsOrColumns = 1;

            }
            else
            {

                //ArtistsList.ItemTemplate = this.Resources["gridItemTemplate"] as DataTemplate;
                //if (artistsWrapGrid != null)
                //    artistsWrapGrid.MaximumRowsOrColumns = Convert.ToInt32(e.NewSize.Width / 100);

                if (e.NewSize.Width >= 510 && e.NewSize.Width < 610)
                {
                    this.GridItemSize = (e.NewSize.Width - 60) / 4;
                }
                else if (e.NewSize.Width >= 610 && e.NewSize.Width < 710)
                {
                    this.GridItemSize = (e.NewSize.Width - 75) / 5;
                }
                else if (e.NewSize.Width >= 710 && e.NewSize.Width < 810)
                {
                    this.GridItemSize = (e.NewSize.Width - 85) / 6;
                }
                else
                {
                    this.GridItemSize = (e.NewSize.Width - 100) / 7;
                }
            }

            selectionGrid.Margin = new Thickness(20, 20, 20, ApplicationInfo.Current.FooterHeight + 20);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            progress.IsActive = true;
            Storyboard sb = this.Resources["ExitPageTransition"] as Storyboard;
            sb.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;

            PageHelper.Artists = this;

            if (ApplicationInfo.Current.IsMobile == false)
            {
                ArtistsList.ItemContainerTransitions.Add(new EntranceThemeTransition() { FromVerticalOffset = 250, IsStaggeringEnabled = true });
            }
            else
            {
                ArtistsList.ItemContainerTransitions.Add(new EntranceThemeTransition() { FromVerticalOffset = 250, IsStaggeringEnabled = false });
            }

            if (((CollectionViewSource)Resources["ListOfArtists"]).Source == null || CollectionHasBeenUpdated)
            {
                CollectionHasBeenUpdated = false;
                LoadArtists();
            }
            else
            {
                OpenPage(NavMode == NavigationMode.Back);
            }
        }

        private void LoadArtists()
        {

            ((CollectionViewSource)Resources["ListOfArtists"]).Source = null;
            List<Artist> listOfArtists = Ctr_Artist.Current.GetArtists();

            listOfArtists.OrderBy(s => s.Name);

            List<AlphaKeyGroup<Artist>> itemSource = AlphaKeyGroup<Artist>.CreateGroups(listOfArtists,
    CultureInfo.CurrentUICulture,
    a => a.Name, true);

            ((CollectionViewSource)Resources["ListOfArtists"]).Source = itemSource;


            //DownloadImages(listOfArtists);
            OpenPage(NavMode == NavigationMode.Back);

            if (listOfArtists.Count == 0)
                PageHelper.MainPage?.ShowEmptyLibraryNotice();
            else
            {
                PageHelper.MainPage?.HideEmptyLibraryNotice();
            }
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

        private async void DownloadImages(Artist artist)
        {
            string lang = ApplicationInfo.Current.Language;

            //Debug.WriteLine("Preparing to download image: " + artist.Name);

            try
            {
                await LastFm.DownloadImage(artist);
            }
            catch
            {

            }
        }

        private void ArtistsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ArtistsList.SelectionMode == ListViewSelectionMode.None)
                PageHelper.MainPage.Navigate(typeof(ArtistPage), e.ClickedItem);
        }

        private void Artist_ImageOpened(object sender, RoutedEventArgs e)
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



        private async void Artist_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Artist art = ((Image)sender).DataContext as Artist;

            if (string.IsNullOrWhiteSpace(art.Name))
                return;

            await Task.Run(() =>
            {
                if (ImageHelper.IsDownloadEnabled)
                {
                    DownloadImages(art);
                }
            });
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.PlayEverything));
        }


        private void Artist_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                Artist artist = (sender as FrameworkElement).DataContext as Artist;

                CreateArtistPopup(artist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void Artist_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && e.HoldingState == HoldingState.Started && ArtistsList.SelectionMode == ListViewSelectionMode.None)
            {
                Artist artist = (sender as FrameworkElement).DataContext as Artist;

                ActivateSelecionMode(artist);
                //CreateArtistPopup(artist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void CreateArtistPopup(Artist artist, object sender, Point point)
        {
            this.ShowPopupMenu(artist, sender, point, Enumerators.MediaItemType.Artist);
        }

        private void SortByButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void SortByMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void CircleImage_ImageFailed(object sender, EventArgs e)
        {
            await Dispatcher.RunIdleAsync((s) =>
            {
                Artist art = ((FrameworkElement)sender).DataContext as Artist;

                if (ImageHelper.IsDownloadEnabled)
                {
                    DownloadImages(art);
                }
            });
        }

        private void CircleImage_ActionClick(object sender, EventArgs e)
        {
            CircleImage cimg = (CircleImage)sender;
            Artist art = cimg.DataContext as Artist;
            List<string> songs = new List<string>();
            var temp = Ctr_Song.Current.GetSongsByArtist(art);

            foreach (Song song in temp)
            {
                songs.Add(song.SongURI);
            }

            MessageService.SendMessageToBackground(new SetPlaylistMessage(songs));
        }

        private void ArtistsList_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            e.Data.Properties.Add("mediaItem", e.Items.First());
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            DisableSelectionMode();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ArtistsList.SelectionMode == ListViewSelectionMode.None)
                EnableSelectionMode();
            else
                DisableSelectionMode();
        }

        private void EnableSelectionMode()
        {
            selectButton.Content = "";
            ArtistsList.SelectionMode = ListViewSelectionMode.Multiple;
            ArtistsList.SelectionChanged += ArtistsList_SelectionChanged;
            topAppBar.Visibility = Visibility.Visible;
        }

        private void DisableSelectionMode()
        {
            selectButton.Content = "";
            ArtistsList.SelectedItem = null;
            ArtistsList.SelectionChanged -= ArtistsList_SelectionChanged;
            ArtistsList.SelectionMode = ListViewSelectionMode.None;
            topAppBar.Visibility = Visibility.Collapsed;
        }

        private void ActivateSelecionMode(Artist artist)
        {
            if (ArtistsList.SelectedItems.Contains(artist))
                return;

            ApplicationInfo.Current.VibrateDevice(25);

            EnableSelectionMode();
            ArtistsList.SelectedItems.Add(artist);
        }

        private void ArtistsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ArtistsList.SelectedItems.Count > 0)
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

            int i = ArtistsList.SelectedItems.Count;

            string s = i + " " + ApplicationInfo.Current.GetSingularPlural(i, "ItemSelected");

            selectedItemsLabel.Text = s;
            selectedItemsLabel.Visibility = Visibility.Visible;
        }

        private void topPlay_Click(object sender, RoutedEventArgs e)
        {
            List<string> listSongs = new List<string>();

            foreach (Artist artist in ArtistsList.SelectedItems)
            {
                var songs = Ctr_Song.Current.GetSongsByArtist(artist);

                foreach (Song s in songs)
                    listSongs.Add(s.SongURI);
            }

            MessageService.SendMessageToBackground(new SetPlaylistMessage(listSongs));

            DisableSelectionMode();
        }

        private void topAdd_Click(object sender, RoutedEventArgs e)
        {
            List<string> listSongs = new List<string>();

            foreach (Artist artist in ArtistsList.SelectedItems)
            {
                var songs = Ctr_Song.Current.GetSongsByArtist(artist);

                foreach (Song s in songs)
                    listSongs.Add(s.SongURI);
            }

            PageHelper.MainPage.CreateAddToPlaylistPopup(listSongs);

            DisableSelectionMode();

        }

        private void topMore_Click(object sender, RoutedEventArgs e)
        {

            List<string> list = new List<string>();

            foreach (Artist artist in ArtistsList.SelectedItems)
            {
                var temp = Ctr_Song.Current.GetSongsByArtist(artist);
                foreach (Song s in temp)
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
