
using AudictiveMusicUWP.Gui.UC;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using IF.Lastfm.Core.Objects;
using NotificationsVisualizerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed partial class StartPage : Page
    {
        private NavigationMode NavMode
        {
            get;
            set;
        }

        public StartPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;

            OpenPage(NavMode == NavigationMode.Back);
            //SetTile();
        }




        private void LoadCards()
        {
            Song song = Ctr_Song.Current.GetRandomSong();

            if (song == null)
            {
                card1.Visibility = Visibility.Collapsed;
                return;
            }

            card1.SetContext(song);
            card1.Visibility = Visibility.Visible;
        }
        private void pageTransition_Completed(object sender, object e)
        {
            LoadCards();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Storyboard sb = this.Resources["ExitPageTransition"] as Storyboard;
            sb.Begin();
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

        private void Tile_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                Song song = ((PreviewTile)sender).Tag as Song;

                this.ShowPopupMenu(song, sender, e.GetPosition((PreviewTile)sender), Enumerators.MediaItemType.Song);
            }
        }

        private void Tile_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Touch)
            {
                Song song = ((PreviewTile)sender).Tag as Song;

                this.ShowPopupMenu(song, sender, e.GetPosition((PreviewTile)sender), Enumerators.MediaItemType.Song);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //tile.TileSize = TileSize.Small;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //tile.TileSize = TileSize.Medium;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //tile.TileSize = TileSize.Wide;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //tile.TileSize = TileSize.Large;
        }

        private void Tile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new SetPlaylistMessage(new List<string>() { (((PreviewTile)sender).Tag as Song).SongURI }));
            //UpdateSingleTile(tilesContainer.Children.IndexOf((UIElement)sender));
        }

        private void PreviewTile_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //tilesContainer.ItemWidth = tilesContainer.ItemHeight = e.NewSize.Width + 4;
        }

        private void playButton_Tapped(object sender, TappedRoutedEventArgs e)
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

            PageHelper.MainPage.OpenPlayer();
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.PlayEverything));

            PageHelper.MainPage.OpenPlayer();
        }

        private void collectionButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(CollectionPage), "page=artists");
        }

        private void favoritesButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(Favorites), null);
        }

        private void foldersButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(FolderPage), null);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.CreateSearchGrid();
        }

        private async void FeatureNotReadyMessage()
        {
            MessageDialog md = new MessageDialog("Opa!! Você foi mais rápido que eu! Essa função ainda não está pronta mas em breve estará! :)");
            await md.ShowAsync();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Settings), "path=menu");
        }

    }
}
