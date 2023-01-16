using AudictiveMusicUWP.Gui.Util;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
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
    public sealed partial class Playlists : Page
    {
        private NavigationMode NavMode
        {
            get;
            set;
        }
        public Playlists()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;
            PageHelper.Playlists = this;
            LoadPlaylists();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.Playlists = null;
        }

        public async void LoadPlaylists()
        {
            playlistsList.ItemsSource = await CustomPlaylistsHelper.GetPlaylists();
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

        private void PlaylistItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Touch)
            {
                Playlist playlist = (sender as FrameworkElement).DataContext as Playlist;

                CreatePlaylistPopup(playlist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void CreatePlaylistPopup(Playlist playlist, object sender, Point point)
        {
            this.ShowPopupMenu(playlist, sender, point, Enumerators.MediaItemType.Playlist);
        }

        private void PlaylistItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && e.HoldingState == HoldingState.Started)
            {
                Playlist playlist = (sender as FrameworkElement).DataContext as Playlist;

                CreatePlaylistPopup(playlist, sender, e.GetPosition(sender as FrameworkElement));
            }
        }

        private void playlistsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Playlist playlist = e.ClickedItem as Playlist;
            Frame.Navigate(typeof(PlaylistPage), playlist);
        }
    }
}
