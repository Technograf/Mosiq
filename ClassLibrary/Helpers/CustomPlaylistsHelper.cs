using ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace ClassLibrary.Helpers
{
    public static class CustomPlaylistsHelper
    {
        public static string CurrentTrackPath
        {
            get
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CurrentTrackPath"))
                    return Convert.ToString(ApplicationData.Current.LocalSettings.Values["CurrentTrackPath"]);
                else
                    return string.Empty;
            }
        }

        public async static Task<Playlist> GetFavorites()
        {
            StorageFile favsFile;
            IStorageItem favsItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Favorites.xml");
            if (favsItem != null)
            {
                favsFile = favsItem as StorageFile;
            }
            else
            {
                favsFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Favorites.xml", CreationCollisionOption.OpenIfExists);

                return new Playlist("FAVORITES", "Favorites.xml", new List<string>());
            }

            Playlist playlist;
            List<string> songs;


            try
            {
                string content = await FileIO.ReadTextAsync(favsFile);

                if (string.IsNullOrWhiteSpace(content) == false)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(content);

                    XmlElement head = xmlDoc.FirstChild as XmlElement;
                    string name = head.GetAttribute("Name");

                    XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Song");

                    songs = new List<string>();

                    foreach (XmlElement songElement in itemNodes)
                    {
                        songs.Add(songElement.GetAttribute("Path"));
                    }

                    playlist = new Playlist(name, favsFile.Name, songs);
                }
                else
                {
                    playlist = new Playlist("FAVORITES", "Favorites.xml", new List<string>());
                }
            }
            catch
            {
                playlist = new Playlist("FAVORITES", "Favorites.xml", new List<string>());
            }

            return playlist;
        }

        public async static Task<List<Playlist>> GetPlaylists()
        {
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Playlists", CreationCollisionOption.OpenIfExists);

            var files = await folder.GetFilesAsync();

            List<Playlist> playlists = new List<Playlist>();
            Playlist playlist;
            List<string> songs;

            foreach (StorageFile file in files)
            {
                if (file.FileType == ".xml")
                {
                    try
                    {
                        string content = await FileIO.ReadTextAsync(file);

                        Debug.WriteLine(content);
                        if (string.IsNullOrWhiteSpace(content) == false)
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(content);

                            XmlElement head = xmlDoc.FirstChild as XmlElement;
                            string name = head.GetAttribute("Name");

                            XmlNodeList itemNodes = xmlDoc.GetElementsByTagName("Song");

                            songs = new List<string>();

                            foreach (XmlElement songElement in itemNodes)
                            {
                                songs.Add(songElement.GetAttribute("Path"));
                            }

                            playlist = new Playlist(name, file.Name, songs);

                            playlists.Add(playlist);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            return playlists;
        }

        public static async Task<bool> SaveToPlaylist(Playlist playlist)
        {
            try
            {
                StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Playlists", CreationCollisionOption.OpenIfExists);

                StorageFile playlistFile = null;
                XmlDocument DOC = new XmlDocument();

                // A PLAYLIST NÃO EXISTE
                XmlElement mainElem = DOC.DocumentElement;
                XmlElement ELE = DOC.CreateElement("Playlist");
                ELE.SetAttribute("Name", playlist.Name);
                DOC.AppendChild(ELE);

                foreach (string file in playlist.Songs)
                {
                    XmlElement x;

                    x = DOC.CreateElement("Song");
                    //x.SetAttribute("ID", Guid.NewGuid().ToString());
                    x.SetAttribute("Path", file);

                    DOC.FirstChild.AppendChild(x);
                }

                playlistFile = await folder.CreateFileAsync(playlist.PlaylistFileName, CreationCollisionOption.ReplaceExisting);

                await DOC.SaveToFileAsync(playlistFile);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> AddToFavorites(Playlist playlist)
        {
            try
            {
                StorageFile favoritesFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(playlist.PlaylistFileName, CreationCollisionOption.ReplaceExisting);
                XmlDocument DOC = new XmlDocument();

                // A PLAYLIST NÃO EXISTE
                XmlElement mainElem = DOC.DocumentElement;
                XmlElement ELE = DOC.CreateElement("Playlist");
                ELE.SetAttribute("Name", playlist.Name);
                DOC.AppendChild(ELE);

                foreach (string file in playlist.Songs)
                {
                    XmlElement x;

                    x = DOC.CreateElement("Song");
                    //x.SetAttribute("ID", Guid.NewGuid().ToString());
                    x.SetAttribute("Path", file);

                    DOC.FirstChild.AppendChild(x);
                }


                await DOC.SaveToFileAsync(favoritesFile);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> RemoveFromFavorites(string path)
        {
            try
            {
                StorageFile favoritesFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Favorites.xml", CreationCollisionOption.OpenIfExists);
                XmlDocument DOC = new XmlDocument();

                string xml = await FileIO.ReadTextAsync(favoritesFile);

                if (string.IsNullOrWhiteSpace(xml))
                    return false;

                DOC.LoadXml(xml);

                IXmlNode node = DOC.SelectSingleNode("/Playlist/Song[@Path=\"" + path + "\"]");

                if (node == null)
                    return false;

                IXmlNode parent = node.ParentNode;

                if (parent == null)
                    return false;

                // remove the child node
                parent.RemoveChild(node);


                await DOC.SaveToFileAsync(favoritesFile);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<List<string>> GetLastPlaylistFromFile()
        {
            IStorageItem lastPlaylistItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("LastPlayback.xml");

            if (lastPlaylistItem != null)
            {
                XmlDocument doc = new XmlDocument();

                string content = await FileIO.ReadTextAsync(lastPlaylistItem as StorageFile);

                if (content != null && string.IsNullOrWhiteSpace(content) == false)
                {
                    doc.LoadXml(content);

                    List<string> list = new List<string>();

                    var elements = doc.GetElementsByTagName("Song");

                    for (int i = 0; i < elements.Count; i++)
                    {
                        XmlElement element = elements[i] as XmlElement;
                        list.Add(element.InnerText);
                    }

                    return list;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
