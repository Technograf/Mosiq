using AudictiveMusicUWP.Gui.Pages;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class SearchPane : UserControl
    {
        private List<Song> listOfSongs;
        private string CurrentSearchTerm;
        private int keyPressTimer;
        private DispatcherTimer timer;
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual blurSprite;

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
            DependencyProperty.Register("AlbumItemLength", typeof(double), typeof(SearchPane), new PropertyMetadata(150));
        public SearchPane()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.Loaded += SearchGrid_Loaded;
            this.SizeChanged += SearchGrid_SizeChanged;
            this.InitializeComponent();
            this.keyPressTimer = 0;
            
        }

        private void SearchGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (blurSprite != null)
            {
                blurSprite.Size = new Size(e.NewSize.Width, e.NewSize.Height).ToVector2();
            }

            artistsList.Width = e.NewSize.Width;
            albumsList.Width = e.NewSize.Width;

            if (e.NewSize.Width >= 1200)
            {
                searchBox.FontSize = 50;
            }
            else if (e.NewSize.Width < 1200 && e.NewSize.Width >= 900)
            {
                searchBox.FontSize = 38;
            }
            else if (e.NewSize.Width < 900 && e.NewSize.Width >= 700)
            {
                searchBox.FontSize = 32;
            }
            else if (e.NewSize.Width < 700 && e.NewSize.Width >= 500)
            {
                searchBox.FontSize = 24;
            }
            else if (e.NewSize.Width < 500)
            {
                searchBox.FontSize = 18;
            }
        }

        private void SearchGrid_Loaded(object sender, RoutedEventArgs e)
        {
            SetUpFluentDesign();

            Storyboard sb = this.Resources["OpenAnimation"] as Storyboard;
            sb.Begin();

            searchBox.Focus(FocusState.Keyboard);
        }

        public void SetUpFluentDesign()
        {
            blurSprite = _compositor.CreateSpriteVisual();

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

            blurSprite.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            blurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(blurGrid, blurSprite);
        }


        private void Timer_Tick(object sender, object e)
        {
            timer.Stop();

            if (searchBox.Text.Length > 3)
            {
                this.CurrentSearchTerm = searchBox.Text;

                RunSearch(this.CurrentSearchTerm);
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

        private void SongItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                Song song = (sender as FrameworkElement).DataContext as Song;
                CreateSongPopup(song, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void SongItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch && e.HoldingState == HoldingState.Started)
            {
                Song song = (sender as FrameworkElement).DataContext as Song;
                CreateSongPopup(song, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void CreateSongPopup(Song song, object sender, Point point)
        {
            this.ShowPopupMenu(song, sender, point, Enumerators.MediaItemType.Song);
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

        private async void DownloadImage(Artist artist)
        {
            try
            {
                //LastFm lfm = new LastFm();
                //lfm.DownloadCompleted += Lfm_DownloadCompleted;
                await LastFm.DownloadImage(artist);
            }
            catch
            {

            }
        }

        private void Lfm_DownloadCompleted(Artist artist)
        {
            
        }

        private void albumsList_Loaded(object sender, RoutedEventArgs e)
        {
            var sv = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.albumsList, 0), 0);
            sv.ViewChanged += Sv_ViewChanged;
        }

        private void Sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            AlbumItemHelper.PointerIsInContact = false;
        }

        private void albumsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UseTransition"] = true;
            PageHelper.MainPage.Navigate(typeof(AlbumPage), e.ClickedItem);
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

        private void albumItemOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation da = new DoubleAnimation()
                {
                    To = 0.7,
                    Duration = TimeSpan.FromMilliseconds(200),
                };

                Storyboard.SetTarget(da, sender as Border);
                Storyboard.SetTargetProperty(da, "Opacity");

                sb.Children.Add(da);

                sb.Begin();
            }
        }

        private void albumItemOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                Storyboard sb = new Storyboard();

                DoubleAnimation da = new DoubleAnimation()
                {
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(200),
                };

                Storyboard.SetTarget(da, sender as Border);
                Storyboard.SetTargetProperty(da, "Opacity");

                sb.Children.Add(da);

                sb.Begin();
            }
        }

        private void Artist_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && e.HoldingState == HoldingState.Started)
            {
                Artist artist = (sender as FrameworkElement).DataContext as Artist;

                CreateArtistPopup(artist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void Artist_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                Artist artist = (sender as FrameworkElement).DataContext as Artist;

                CreateArtistPopup(artist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void CreateArtistPopup(Artist artist, object sender, Point point)
        {
            this.ShowPopupMenu(artist, sender, point, Enumerators.MediaItemType.Artist);
        }

        private void artistsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(ArtistPage), e.ClickedItem);
        }

        private void CircleImage_ImageFailed(object sender, EventArgs e)
        {
            Artist art = ((FrameworkElement)sender).DataContext as Artist;

            DownloadImage(art);
        }

        private void CircleImage_ActionClick(object sender, EventArgs e)
        {
            CircleImage cimg = (CircleImage)sender;
            Artist art = cimg.DataContext as Artist;
            List<string> songs = new List<string>();
            var temp = Ctr_Song.Current.GetSongsByArtist(art);

            foreach (Song song in temp)
                songs.Add(song.SongURI);

            MessageService.SendMessageToBackground(new SetPlaylistMessage(songs));
        }

        private void AlbumItem_MenuTriggered(object sender, Point point)
        {
            Album album = (sender as FrameworkElement).DataContext as Album;

            CreateAlbumPopup(album, sender, point);
        }

        private void searchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;

                if (timer != null)
                    timer.Stop();
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                this.CurrentSearchTerm = searchBox.Text;
                RunSearch(this.CurrentSearchTerm);
            }
        }

        //public void SetInitialChar(char c)
        //{
        //    searchBox.Text = c.ToString();
        //    searchBox.SelectionStart = 1;
        //    searchBox.Focus(FocusState.Keyboard);
        //}

        private void RunSearch(string searchTerm)
        {
            var songs = Ctr_Song.Current.GetSongs(false);

            listOfSongs = new List<Song>();
            List<Artist> listOfArtists = new List<Artist>();
            List<Album> listOfAlbums = new List<Album>();

            Song songAUX;
            Artist artistAUX;
            Album albumAUX;
            string title = string.Empty;
            string artist = string.Empty;
            string album = string.Empty;
            string genre = string.Empty;

            foreach (Song song in songs)
            {
                title = song.Title;
                artist = song.Artist;
                album = song.Album;
                genre = song.Genre;

                if (title.ToLower().Contains(searchTerm.ToLower()) ||
                    artist.ToLower().Contains(searchTerm.ToLower()) ||
                    album.ToLower().Contains(searchTerm.ToLower()) ||
                    genre.ToLower().Contains(searchTerm.ToLower()))
                {

                    listOfSongs.Add(song);

                    if (artist.ToLower().Contains(searchTerm.ToLower()))
                    {
                        artistAUX = new Artist();
                        if (listOfArtists.Exists(a => a.Name == artist) == false)
                        {
                            artistAUX.Name = artist;
                            listOfArtists.Add(artistAUX);
                        }

                        albumAUX = new Album();
                        if (listOfAlbums.Exists(a => a.AlbumID == song.AlbumID) == false)
                        {
                            albumAUX.Name = album;
                            albumAUX.Artist = artist;
                            albumAUX.AlbumID = song.AlbumID;
                            albumAUX.Year = Convert.ToInt32(song.Year);
                            albumAUX.Genre = song.Genre;
                            albumAUX.HexColor = song.HexColor;

                            listOfAlbums.Add(albumAUX);
                        }
                    }
                    else if (album.ToLower().Contains(searchTerm.ToLower()))
                    {
                        albumAUX = new Album();
                        if (listOfAlbums.Exists(a => a.AlbumID == song.AlbumID) == false)
                        {
                            albumAUX.Name = album;
                            albumAUX.Artist = artist;
                            albumAUX.AlbumID = song.AlbumID;
                            albumAUX.Year = Convert.ToInt32(song.Year);
                            albumAUX.Genre = song.Genre;
                            albumAUX.HexColor = song.HexColor;

                            listOfAlbums.Add(albumAUX);
                        }
                    }
                }
            }

            if (listOfArtists.Count == 0)
            {
                artistsHeader.Visibility = Visibility.Collapsed;
                artistsScroll.Visibility = Visibility.Collapsed;
                artistsList.Visibility = Visibility.Collapsed;
            }
            else
            {
                artistsHeader.Visibility = Visibility.Visible;
                artistsScroll.Visibility = Visibility.Visible;
                artistsList.Visibility = Visibility.Visible;
            }

            if (listOfAlbums.Count == 0)
            {
                albumsHeader.Visibility = Visibility.Collapsed;
                albumsScroll.Visibility = Visibility.Collapsed;
                albumsList.Visibility = Visibility.Collapsed;
            }
            else
            {
                albumsHeader.Visibility = Visibility.Visible;
                albumsScroll.Visibility = Visibility.Visible;
                albumsList.Visibility = Visibility.Visible;
            }

            songsList.ItemsSource = listOfSongs.OrderBy(s => s.Title).ToList();
            albumsList.ItemsSource = listOfAlbums.OrderBy(s => s.Name).ToList();
            artistsList.ItemsSource = listOfArtists.OrderBy(s => s.Name).ToList();

            if (listOfSongs.Count == 0)
            {
                ResetSearch();

                NoResults.Visibility = Visibility.Visible;
            }
            else
            {
                NoResults.Visibility = Visibility.Collapsed;

                songsHeader.Visibility = Visibility.Visible;
                songsList.Visibility = Visibility.Visible;
            }
        }

        private void ResetSearch()
        {
            artistsHeader.Visibility = Visibility.Collapsed;
            artistsScroll.Visibility = Visibility.Collapsed;
            artistsList.Visibility = Visibility.Collapsed;

            albumsHeader.Visibility = Visibility.Collapsed;
            albumsScroll.Visibility = Visibility.Collapsed;
            albumsList.Visibility = Visibility.Collapsed;

            songsHeader.Visibility = Visibility.Collapsed;
            songsList.Visibility = Visibility.Collapsed;

            NoResults.Visibility = Visibility.Collapsed;
        }

        private void AlbumItem_LongHover(object sender, object context)
        {
            pageFlyout.Show(typeof(AlbumPage), context, false);
        }

        private void pageFlyout_Closed(object sender, EventArgs e)
        {
            pageFlyout.IsHitTestVisible = false;
        }

        private void pageFlyout_Opened(object sender, EventArgs e)
        {
            pageFlyout.IsHitTestVisible = true;
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetKeyPressTimer();

            if (searchBox.Text.Length == 0)
            {
                ResetSearch();
            }
        }

        private void ResetKeyPressTimer()
        {
            if (timer != null)
                timer.Stop();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
    }
}
