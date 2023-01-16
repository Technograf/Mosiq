using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class AlbumPage : Page
    {
        private ResourceLoader res;

        private List<Song> listOfSongs
        {
            get;
            set;
        }

        private NavigationMode NavMode
        {
            get;
            set;
        }

        private Album Album
        {
            get;
            set;
        }

        public AlbumPage()
        {

            this.InitializeComponent();

            res = new ResourceLoader();            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            header.ClearContext();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;
            Album = e.Parameter as Album;
            header.SetContext(Album);

            LoadSongs();
        }

        private async void LoadSongs()
        {
            listOfSongs = Ctr_Song.Current.GetSongsByAlbum(Album);

            await Task.Delay(10);
            SongsList.ItemsSource = listOfSongs.OrderBy(s => s.TrackNumber);

            header.UpdateNumberOfItems(listOfSongs.Count);

            OpenPage(NavMode == NavigationMode.Back);
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

        //private void SetOverlayColor()
        //{
        //    overlayBrush.Color = Album.Color;

        //    Storyboard sb = new Storyboard();

        //    DoubleAnimation da = new DoubleAnimation()
        //    {
        //        To = 0.4,
        //        Duration = TimeSpan.FromMilliseconds(400),
        //    };

        //    Storyboard.SetTarget(da, overlay);
        //    Storyboard.SetTargetProperty(da, "Opacity");

        //    sb.Children.Add(da);

        //    sb.Begin();
        //}

        private void artistName_Click(object sender, RoutedEventArgs e)
        {
            Artist artist = new Artist()
            {
                Name = Album.Artist,
            };

            Frame.Navigate(typeof(ArtistPage), artist);
        }

        private void playAlbum_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            var songs = Ctr_Song.Current.GetSongsByAlbum(Album);

            foreach (Song song in songs)
                list.Add(song.SongURI);

            MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddToNowPlaying_Click(object sender, RoutedEventArgs e)
        {
            var songs = Ctr_Song.Current.GetSongsByAlbum(Album);

            List<string> list = new List<string>();

            foreach (Song song in songs)
                list.Add(song.SongURI);

            MessageService.SendMessageToBackground(new AddSongsToPlaylist(list, false));
        }

        private async void shareAlbum_Click(object sender, RoutedEventArgs e)
        {
            if (await this.ShareMediaItem(Album, Enumerators.MediaItemType.Album) == false)
            {
                string message = ApplicationInfo.Current.Resources.GetString("ShareErrorMessage").Replace("%", "0x001");

                MessageDialog md = new MessageDialog(message);
                await md.ShowAsync();
            }
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

        private void moreOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            this.ShowPopupMenu(Album, sender, new Point(0, 0), Enumerators.MediaItemType.Album);
        }

        private async void SongsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SongsList.SelectionMode == ListViewSelectionMode.None)
            {
                Song clickedSong = e.ClickedItem as Song;

                List<string> list = new List<string>();

                await listOfSongs.Shuffle();
                list.Add(clickedSong.SongURI);

                foreach (Song s in listOfSongs)
                {
                    if (s.SongURI != clickedSong.SongURI)
                        list.Add(s.SongURI);
                }

                MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
            }
        }
    }
}
