using AudictiveMusicUWP.Collection;
using BackgroundAudioShared;
using BackgroundAudioShared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static AudictiveMusicUWP.Helpers.Enumerators;

namespace AudictiveMusicUWP.Helpers
{
    public static class PopupHelper
    {
        private static ResourceLoader res = new ResourceLoader();
        private static Page currentPage;

        public static void ShowPopupMenu(this Page page, object mediaItem, object sender, Point point, MediaItemType type)
        {
            currentPage = page;
            MenuFlyout menu = new MenuFlyout()
            {
                MenuFlyoutPresenterStyle = Application.Current.Resources["MenuFlyoutModernStyle"] as Style,
            };

            // SE O MENU FOR DO TIPO SONG
            if (type == MediaItemType.Song)
            {
                Song song = mediaItem as Song;

                MenuFlyoutItem item1 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("Play"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item1.Click += (s, a) =>
                {
                    List<string> songs = new List<string>();
                    songs.Add(song.SongURI);
                    MessageService.SendMessageToBackground(new SetPlaylistMessage(songs));
                };

                menu.Items.Add(item1);

                menu.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem item2 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("AddToPlaylist"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item2.Click += async (s, a) =>
                {
                    List<string> songs = new List<string>();
                    songs.Add(song.SongURI);

                    MessageService.SendMessageToBackground(new AddSongsToPlaylist(songs));

                    if (PageHelper.NowPlaying != null)
                    {
                        PageHelper.NowPlaying.ClearPlaylist();

                        MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.AskPlaylist));
                        await Task.Delay(200);
                        PageHelper.NowPlaying.UpdatePlayerInfo(CustomPlaylistsHelper.CurrentTrackPath);
                    }
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
                    List<string> songs = new List<string>();
                    songs.Add(song.SongURI);

                    if (PageHelper.MainPage != null)
                    {
                        PageHelper.MainPage.CreateAddToPlaylistPopup(songs);
                    }
                };

                menu.Items.Add(item3);


                MenuFlyoutItem item4 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("PlayNext"),
                    Tag = "\uEA52",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item4.Click += async (s, a) =>
                {
                    List<string> songs = new List<string>();
                    songs.Add(song.SongURI);

                    MessageService.SendMessageToBackground(new AddSongsToPlaylist(songs, true));

                    if (PageHelper.NowPlaying != null)
                    {
                        PageHelper.NowPlaying.ClearPlaylist();

                        MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.AskPlaylist));
                        await Task.Delay(200);
                        PageHelper.NowPlaying.UpdatePlayerInfo(CustomPlaylistsHelper.CurrentTrackPath);
                    }
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
                    if (await currentPage.ShareMediaItem(song, type) == false)
                    {
                        MessageDialog md = new MessageDialog(ApplicationInfo.Current.Resources.GetString("ShareErrorMessage"));
                        await md.ShowAsync();
                    }
                };

                menu.Items.Add(item5);

                //MenuFlyoutItem item6 = new MenuFlyoutItem()
                //{
                //    Text = "Editar",
                //    Tag = "",
                //    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                //};
                //item6.Click += (s, a) =>
                //{
                //    PageHelper.MainPage.PageFrame.Navigate(typeof(TagEditor), song);
                //};

                //menu.Items.Add(item6);
            }
            // SE O MENU FOR DO TIPO ALBUM
            else if (type == MediaItemType.Album)
            {
                Album album = mediaItem as Album;
                List<Song> songs = CollectionHelper.GetSongsByAlbumID(album.AlbumID);
                List<string> list = new List<string>();
                foreach (Song s in songs)
                    list.Add(s.SongURI);

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
                    if (await currentPage.ShareMediaItem(album, type) == false)
                    {
                        MessageDialog md = new MessageDialog("Não foi possível compartilhar este item");
                        await md.ShowAsync();
                    }
                };

                menu.Items.Add(item5);
            }
            // SE O MENU FOR DO TIPO ARTIST
            else if (type == MediaItemType.Artist)
            {
                Artist artist = mediaItem as Artist;
                List<Song> songs = CollectionHelper.GetSongsByArtist(artist.Name);
                List<string> list = new List<string>();
                foreach (Song s in songs)
                    list.Add(s.SongURI);

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
                    if (await currentPage.ShareMediaItem(artist, type) == false)
                    {
                        MessageDialog md = new MessageDialog("Não foi possível compartilhar este item");
                        await md.ShowAsync();
                    }
                };

                menu.Items.Add(item5);

            }
            // SE O MENU FOR DO TIPO PLAYLIST
            else if (type == MediaItemType.Playlist)
            {
                Playlist playlist = mediaItem as Playlist;

                MenuFlyoutItem item1 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("Play"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item1.Click += (s, a) =>
                {
                    MessageService.SendMessageToBackground(new SetPlaylistMessage(playlist.Songs));
                };

                menu.Items.Add(item1);

                menu.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem item2 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("AddToPlaylist"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item2.Click += async (s, a) =>
                {
                    MessageService.SendMessageToBackground(new AddSongsToPlaylist(playlist.Songs));

                    if (PageHelper.NowPlaying != null)
                    {
                        PageHelper.NowPlaying.ClearPlaylist();

                        MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.AskPlaylist));
                        await Task.Delay(200);
                        PageHelper.NowPlaying.UpdatePlayerInfo(CustomPlaylistsHelper.CurrentTrackPath);
                    }
                };

                menu.Items.Add(item2);


                MenuFlyoutItem item3 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("PlayNext"),
                    Tag = "\uEA52",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item3.Click += async (s, a) =>
                {
                    MessageService.SendMessageToBackground(new AddSongsToPlaylist(playlist.Songs, true));

                    if (PageHelper.NowPlaying != null)
                    {
                        PageHelper.NowPlaying.ClearPlaylist();

                        MessageService.SendMessageToBackground(new ActionMessage(BackgroundAudioShared.Messages.Action.AskPlaylist));
                        await Task.Delay(200);
                        PageHelper.NowPlaying.UpdatePlayerInfo(CustomPlaylistsHelper.CurrentTrackPath);
                    }
                };

                menu.Items.Add(item3);

                menu.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem item4 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("Share"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item4.Click += async (s, a) =>
                {
                    if (await currentPage.ShareMediaItem(playlist, type) == false)
                    {
                        MessageDialog md = new MessageDialog("Não foi possível compartilhar este item");
                        await md.ShowAsync();
                    }
                };

                menu.Items.Add(item4);

                menu.Items.Add(new MenuFlyoutSeparator());

                MenuFlyoutItem item5 = new MenuFlyoutItem()
                {
                    Text = ApplicationInfo.Current.Resources.GetString("Delete"),
                    Tag = "",
                    Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
                };
                item5.Click += async (s, a) =>
                {
                    MessageDialog md = new MessageDialog(ApplicationInfo.Current.Resources.GetString("DeletePlaylistMessage"), ApplicationInfo.Current.Resources.GetString("DeletePlaylistMessageTitle"));
                    md.Commands.Add(new UICommand(ApplicationInfo.Current.Resources.GetString("Yes"), async (t) =>
                    {
                        StorageFolder playlistsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Playlists", CreationCollisionOption.OpenIfExists);

                        IStorageItem playlistItem = await playlistsFolder.TryGetItemAsync(playlist.PlaylistFileName);

                        if (playlistItem != null)
                        {
                            try
                            {
                                await playlistItem.DeleteAsync(StorageDeleteOption.PermanentDelete);

                                if (PageHelper.PlaylistPage != null)
                                {
                                    if (PageHelper.MainPage != null)
                                        PageHelper.MainPage.GoBack();
                                }
                                else if (PageHelper.Playlists != null)
                                {
                                    PageHelper.Playlists.LoadPlaylists();
                                }
                            }
                            catch
                            {

                            }
                        }
                    }));

                    md.Commands.Add(new UICommand(ApplicationInfo.Current.Resources.GetString("No")));

                    md.CancelCommandIndex = 1;
                    md.DefaultCommandIndex = 1;

                    await md.ShowAsync();

                    
                };

                menu.Items.Add(item5);
            }

            try
            {
                menu.ShowAt(sender as FrameworkElement, point);
            }
            catch
            {
                menu.ShowAt(sender as FrameworkElement);
            }
        }
    }
}
