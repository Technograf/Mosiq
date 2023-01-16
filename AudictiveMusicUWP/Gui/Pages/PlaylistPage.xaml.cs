using AudictiveMusicUWP.Gui.Util;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
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
    public sealed partial class PlaylistPage : Page
    {

        private Playlist playlist
        {
            get;
            set;
        }

        private NavigationMode NavMode
        {
            get;
            set;
        }

        private bool changed;

        private bool PlaylistHasChanged
        {
            get
            {
                return changed;
            }
            set
            {
                changed = value;

                saveButton.IsEnabled = value;
            }
        }

        private ObservableCollection<Song> listOfSongs;

        public PlaylistPage()
        {
            this.SizeChanged += PlaylistPage_SizeChanged;
            this.InitializeComponent();

            PlaylistHasChanged = false;
        }

        private void PlaylistPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ApplicationInfo.Current.IsMobile)
            {
                actionButtons.Margin = new Thickness(0, 0, 0, ApplicationInfo.Current.FooterHeight);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;

            if (ApplicationInfo.Current.IsMobile)
            {
                Grid.SetRow(actionButtons, 2);
                Grid.SetColumnSpan(pageTitleGrid, 2);
                actionButtons.MinHeight = 48;
                actionButtons.Margin = new Thickness(0, 0, 0, ApplicationInfo.Current.FooterHeight);
            }
            else
            {
                Grid.SetRow(actionButtons, 0);
            }

            if (e.Parameter != null)
            {
                playlist = e.Parameter as Playlist;

                pageTitle.Text = playlist.Name;
                LoadSongs();
            }

            PageHelper.PlaylistPage = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.PlaylistPage = null;
        }

        private void LoadSongs()
        {
            listOfSongs = new ObservableCollection<Song>();
            Song aux;

            foreach (string path in playlist.Songs)
            {
                aux = Ctr_Song.Current.GetSong(new Song() { SongURI = path });
                listOfSongs.Add(aux);
            }

            SongsList.ItemsSource = listOfSongs;
            listOfSongs.CollectionChanged += ListOfSongs_CollectionChanged;

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

        private void ListOfSongs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PlaylistHasChanged = true;
        }

        private void SongItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void SongItem_Holding(object sender, HoldingRoutedEventArgs e)
        {

        }

        private void SongsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeButton.IsEnabled = SongsList.SelectedItems.Count > 0;
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            this.ShowPopupMenu(playlist, sender, new Point(), Enumerators.MediaItemType.Playlist);
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            List<Song> listToRemove = new List<Song>();

            foreach (Song s in SongsList.SelectedItems)
            {
                listToRemove.Add(s);
            }

            foreach (Song s in listToRemove)
            {
                listOfSongs.Remove(s);
            }
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            ShowRenameDialog();
        }

        private async void ShowRenameDialog()
        {
            ContentDialog cd = new ContentDialog();

            TextBox tbox = new TextBox()
            {
                Text = playlist.Name,
                Margin = new Thickness(0,10,0,10),
                Header = ApplicationInfo.Current.Resources.GetString("NamePlaylistHeader"),
            };

            cd.Title = ApplicationInfo.Current.Resources.GetString("NamePlaylist");

            cd.PrimaryButtonText = ApplicationInfo.Current.Resources.GetString("Rename");
            cd.SecondaryButtonText = ApplicationInfo.Current.Resources.GetString("Cancel");

            cd.Content = tbox;

            var result = await cd.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (string.IsNullOrWhiteSpace(tbox.Text) == false)
                {
                    pageTitle.Text = tbox.Text;
                    playlist.Name = tbox.Text;

                    PlaylistHasChanged = true;
                }
                else
                {
                    MessageDialog md = new MessageDialog(ApplicationInfo.Current.Resources.GetString("ErrorInvalidNameChosen"));
                    await md.ShowAsync();

                    ShowRenameDialog();
                }
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            playlist.Songs = new List<string>();

            foreach (Song s in listOfSongs)
                playlist.Songs.Add(s.SongURI);

            bool result = await CustomPlaylistsHelper.SaveToPlaylist(playlist);

            if (result)
            {
                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
            else
            {

            }

        }
    }
}
