using AudictiveMusicUWP.Collection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.ViewManagement;

namespace AudictiveMusicUWP.Helpers
{
    public class CollectionHelper
    {
        private static bool LoadCollectionCompleted = true;

        public static XmlDocument CollectionDocument
        {
            get;
            private set;
        }

        public static async void CheckCollection()
        {
            LoadCollectionCompleted = false;
            StorageFolder musicFolder = KnownFolders.MusicLibrary;
            StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
            QueryOptions options = new QueryOptions();
            options.FileTypeFilter.Add(".mp3");
            options.FileTypeFilter.Add(".wma");
            options.FileTypeFilter.Add(".aac");
            options.FileTypeFilter.Add(".m4a");
            options.FolderDepth = FolderDepth.Deep;

            var songs = await musicFolder.CreateFileQueryWithOptions(options).GetFilesAsync();
            Stream fileStream = null;
            TagLib.File tagFile = null;
            Song aux = null;
            string title = "";
            string artist = "";
            string album = "";
            string genre = "";
            string trackNumber = "";
            string year = "";

            int totalCount = songs.Count;

            double currentValue = 0;
            double percentagePerItem = Convert.ToDouble(100) / Convert.ToDouble(totalCount);

            List<Song> listOfSongs = new List<Song>();

            XmlDocument DOC = new XmlDocument();

            XmlElement mainElem = DOC.DocumentElement;
            XmlElement ELE = DOC.CreateElement("Collection");
            DOC.AppendChild(ELE);
            XmlElement x = null;

            foreach (StorageFile file in songs)
            {
                try
                {
                    x = DOC.CreateElement("Song");
                    fileStream = await file.OpenStreamForReadAsync();
                    tagFile = TagLib.File.Create(new StreamFileAbstraction(file.Name, fileStream, fileStream));

                    aux = new Song();

                    if (tagFile.Tag.Album != null || string.IsNullOrWhiteSpace(tagFile.Tag.Album) == false)
                        album = tagFile.Tag.Album;
                    else
                        album = "Unknown";

                    if (tagFile.Tag.Title == null || string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                        title = file.DisplayName;
                    else
                        title = tagFile.Tag.Title;

                    if (tagFile.Tag.FirstAlbumArtist != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstAlbumArtist) == false)
                        artist = tagFile.Tag.FirstAlbumArtist;
                    else if (tagFile.Tag.FirstPerformer != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer) == false)
                        artist = tagFile.Tag.FirstPerformer;
                    else
                        artist = "Unknown";

                    trackNumber = Convert.ToString(tagFile.Tag.Track);
                    year = Convert.ToString(tagFile.Tag.Year);

                    if (tagFile.Tag.FirstGenre != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstGenre) == false)
                    {
                        genre = tagFile.Tag.FirstGenre;
                    }
                    else
                        genre = string.Empty;

                    var matchingResults = listOfSongs.Where(s => s.GetArtist() == artist && s.GetAlbum() == album).ToList();

                    string albumid;
                    string hexColor = string.Empty;

                    if (matchingResults.Count() > 0)
                    {
                        albumid = matchingResults[0].GetAlbumID();
                        hexColor = matchingResults[0].HexColor;
                    }
                    else
                    {
                        albumid = Guid.NewGuid().ToString();

                        if (tagFile.Tag.Pictures.Length > 0)
                        {
                            var dataVector = tagFile.Tag.Pictures[0].Data.Data;

                            SaveCoverImageResult result = await ImageHelper.SaveAlbumCover(dataVector, albumid);
                            hexColor = result.HexColor;
                            //ImageHelper.BlurAlbumCover(albumid);
                        }
                    }

                    string songid = Guid.NewGuid().ToString();

                    x.SetAttribute("Artist", artist);
                    x.SetAttribute("Title", title);
                    x.SetAttribute("Album", album);
                    x.SetAttribute("Path", file.Path);
                    x.SetAttribute("Genre", genre);
                    x.SetAttribute("Year", year);
                    x.SetAttribute("Track", trackNumber);
                    x.SetAttribute("AlbumID", albumid);
                    x.SetAttribute("ID", Guid.NewGuid().ToString());
                    x.SetAttribute("HexColor", hexColor);
                    aux.Set(title, artist, album, albumid, songid, year, trackNumber, genre, file.Path, hexColor);

                    listOfSongs.Add(aux);

                    DOC.FirstChild.AppendChild(x);


                    fileStream.Dispose();
                }
                catch
                {

                }

                currentValue = currentValue + percentagePerItem;

                if (PageHelper.PreparingCollection != null)
                    PageHelper.PreparingCollection.UpdateStatus(Convert.ToInt32(currentValue));
                else if (PageHelper.SetupWizard != null)
                    PageHelper.SetupWizard.UpdateStatus(Convert.ToInt32(currentValue));

            }

            StorageFolder collectionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Collection", CreationCollisionOption.OpenIfExists);
            StorageFile collectionFile = await collectionFolder.CreateFileAsync("Collection.xml", CreationCollisionOption.ReplaceExisting);
            await DOC.SaveToFileAsync(collectionFile);

            await CollectionHelper.LoadCollectionFile();

            LoadCollectionCompleted = true;

            if (PageHelper.PreparingCollection != null)
                PageHelper.PreparingCollection.LoadingCompleted();
            else if (PageHelper.SetupWizard != null)
                PageHelper.SetupWizard.LoadingCompleted();
        }

        public static async void LoadCollectionChanges()
        {
            if (LoadCollectionCompleted == false)
                return;

            Debug.WriteLine("INICIANDO BUSCA");

            //if (ApplicationInfo.Current.IsMobile)
            //{
            //    StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = null;
            //    StatusBar.GetForCurrentView().ProgressIndicator.Text = "Looking for your music...";
            //}

            LoadCollectionCompleted = false;

            bool changed = false;

            if (CollectionHelper.IsCollectionLoaded)
            {
                List<string> currentSongs = CollectionHelper.GetAllSongsPaths();
                List<string> folderSongs = new List<string>();
                if (currentSongs != null)
                {
                    List<Song> listOfSongs = new List<Song>();
                    Song aux = null;

                    foreach (XmlElement element in GetAllSongs(false))
                    {
                        aux = new Song();
                        aux.Set(element.GetAttribute("Title"),
                            element.GetAttribute("Artist"),
                            element.GetAttribute("Album"),
                            element.GetAttribute("AlbumID"),
                            element.GetAttribute("ID"),
                            element.GetAttribute("Year"),
                            element.GetAttribute("Track"),
                            element.GetAttribute("Genre"),
                            element.GetAttribute("Path"),
                            element.GetAttribute("HexColor"));

                        listOfSongs.Add(aux);
                    }

                    StorageFolder musicFolder = KnownFolders.MusicLibrary;
                    StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
                    QueryOptions options = new QueryOptions();
                    options.FileTypeFilter.Add(".mp3");
                    options.FileTypeFilter.Add(".wma");
                    options.FileTypeFilter.Add(".aac");
                    options.FileTypeFilter.Add(".m4a");
                    options.FolderDepth = FolderDepth.Deep;

                    var songs = await musicFolder.CreateFileQueryWithOptions(options).GetFilesAsync();
                    XmlElement x = null;
                    Stream fileStream = null;
                    TagLib.File tagFile = null;
                    string title = "";
                    string artist = "";
                    string album = "";
                    string genre = "";
                    string trackNumber = "";
                    string year = "";

                    int totalCount = songs.Count;

                    foreach (StorageFile file in songs)
                    {
                        folderSongs.Add(file.Path);
                    }

                    foreach (StorageFile file in songs)
                    {
                        if (file.FileType == ".mp3" || file.FileType == ".wma" || file.FileType == ".aac" || file.FileType == ".m4a")
                        {
                            if (currentSongs.Contains(file.Path) == false)
                            {
                                try
                                {
                                    x = CollectionDocument.CreateElement("Song");
                                    fileStream = await file.OpenStreamForReadAsync();
                                    tagFile = TagLib.File.Create(new StreamFileAbstraction(file.Name, fileStream, fileStream));

                                    aux = new Song();

                                    if (tagFile.Tag.Album != null || string.IsNullOrWhiteSpace(tagFile.Tag.Album) == false)
                                        album = tagFile.Tag.Album;
                                    else
                                        album = "Unknown";

                                    if (tagFile.Tag.Title == null || string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                                        title = file.DisplayName;
                                    else
                                        title = tagFile.Tag.Title;

                                    if (tagFile.Tag.FirstAlbumArtist != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstAlbumArtist) == false)
                                        artist = tagFile.Tag.FirstAlbumArtist;
                                    else if (tagFile.Tag.FirstPerformer != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstPerformer) == false)
                                        artist = tagFile.Tag.FirstPerformer;
                                    else
                                        artist = "Unknown";

                                    trackNumber = Convert.ToString(tagFile.Tag.Track);
                                    year = Convert.ToString(tagFile.Tag.Year);

                                    if (tagFile.Tag.FirstGenre != null && string.IsNullOrWhiteSpace(tagFile.Tag.FirstGenre) == false)
                                    {
                                        genre = tagFile.Tag.FirstGenre;
                                    }
                                    else
                                        genre = string.Empty;

                                    var matchingResults = listOfSongs.Where(s => s.Artist == artist && s.Album == album).ToList();

                                    string albumid;
                                    string hexColor = string.Empty;

                                    if (matchingResults.Count() > 0)
                                    {
                                        albumid = matchingResults[0].AlbumID;
                                        hexColor = matchingResults[0].HexColor;
                                    }
                                    else
                                    {
                                        albumid = Guid.NewGuid().ToString();

                                        if (tagFile.Tag.Pictures.Length > 0)
                                        {
                                            var dataVector = tagFile.Tag.Pictures[0].Data.Data;

                                            SaveCoverImageResult result = await ImageHelper.SaveAlbumCover(dataVector, albumid);
                                            hexColor = result.HexColor;
                                            //ImageHelper.BlurAlbumCover(albumid);
                                        }
                                    }

                                    string songid = Guid.NewGuid().ToString();

                                    x.SetAttribute("Artist", artist);
                                    x.SetAttribute("Title", title);
                                    x.SetAttribute("Album", album);
                                    x.SetAttribute("Path", file.Path);
                                    x.SetAttribute("Genre", genre);
                                    x.SetAttribute("Year", year);
                                    x.SetAttribute("Track", trackNumber);
                                    x.SetAttribute("AlbumID", albumid);
                                    x.SetAttribute("ID", songid);
                                    x.SetAttribute("HexColor", hexColor);
                                    aux.Set(title, artist, album, albumid, songid, year, trackNumber, genre, file.Path, hexColor);

                                    CollectionDocument.FirstChild.AppendChild(x);


                                    fileStream.Dispose();
                                    changed = true;

                                    listOfSongs.Add(aux);
                                }
                                catch
                                {

                                }

                            }
                        }
                    }

                    foreach (string file in currentSongs)
                    {
                        if (folderSongs.Contains(file) == false)
                        {
                            IXmlNode node = CollectionDocument.SelectSingleNode("/Collection/Song[@Path=\"" + file + "\"]");

                            IXmlNode parent = node.ParentNode;

                            // remove the child node
                            parent.RemoveChild(node);
                            changed = true;
                        }
                    }

                    StorageFolder collectionFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Collection", CreationCollisionOption.OpenIfExists);
                    StorageFile collectionFile = await collectionFolder.CreateFileAsync("Collection.xml", CreationCollisionOption.ReplaceExisting);
                    await CollectionDocument.SaveToFileAsync(collectionFile);

                    await CollectionHelper.LoadCollectionFile();

                    if (changed)
                    {
                        if (PageHelper.Artists != null)
                        {
                            PageHelper.Artists.CollectionHasBeenUpdated = true;
                        }

                        if (PageHelper.Albums != null)
                        {
                            PageHelper.Albums.CollectionHasBeenUpdated = true;
                        }

                        if (PageHelper.Songs != null)
                        {
                            PageHelper.Songs.CollectionHasBeenUpdated = true;
                        }
                    }
                }
            }

            Debug.WriteLine("BUSCA CONCLUÍDA");

            if (ApplicationInfo.Current.IsMobile)
            {
                StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue = 0;
                StatusBar.GetForCurrentView().ProgressIndicator.Text = "";
            }

            LoadCollectionCompleted = true;
        }

        public static bool IsCollectionLoaded
        {
            get
            {
                if (CollectionDocument == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Carrega o XML da coleção
        /// </summary>
        /// <returns>True se carregou e false se o arquivo não existe ou está vazio</returns>
        public static async Task<bool> LoadCollectionFile()
        {
            IStorageItem storageItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Collection");

            if (storageItem != null)
            {
                StorageFolder folder = storageItem as StorageFolder;

                storageItem = await folder.TryGetItemAsync("Collection.xml");

                if (storageItem != null)
                {
                    StorageFile file = storageItem as StorageFile;

                    string content = await FileIO.ReadTextAsync(file);

                    if (string.IsNullOrWhiteSpace(content) == false)
                    {
                        try
                        {
                            CollectionDocument = new XmlDocument();
                            CollectionDocument.LoadXml(content);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static List<Song> GetSongsByArtist(string artistName)
        {
            List<Song> list = new List<Song>();
            Song aux;
            if (IsCollectionLoaded)
            {
                XmlNodeList nodes = CollectionDocument.SelectNodes("/Collection/Song[@Artist=\"" + artistName + "\"]") as XmlNodeList;

                foreach (XmlElement element in nodes)
                {
                    aux = new Song();
                    aux.Set(element.GetAttribute("Title"),
                                element.GetAttribute("Artist"),
                                element.GetAttribute("Album"),
                                element.GetAttribute("AlbumID"),
                                element.GetAttribute("ID"),
                                element.GetAttribute("Year"),
                                element.GetAttribute("Track"),
                                element.GetAttribute("Genre"),
                                element.GetAttribute("Path"),
                                element.GetAttribute("HexColor"));
                    list.Add(aux);
                }
            }

            list = list.OrderBy(s => s.TrackNumber).ToList();

            return list;
        }

        public static List<string> GetSongsPathsByArtist(string artistName)
        {
            if (IsCollectionLoaded)
            {
                List<string> list = new List<string>();

                IXmlNode[] nodeList = CollectionDocument.SelectNodes("/Collection/Song[@Artist=\"" + artistName + "\"]").OrderBy(node => ((XmlElement)node).GetAttribute("Album")).ToArray();

                for (int i = 0; i < nodeList.Count(); i++)
                {
                    XmlElement element = nodeList[i] as XmlElement;

                    list.Add(element.GetAttribute("Path"));
                }

                return list;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<IXmlNode> GetSongsByArtistAndAlbum(string artistName, string albumName)
        {
            if (IsCollectionLoaded)
            {
                return CollectionDocument.SelectNodes("/Collection/Song[@Artist=\"" + artistName + "\" and @Album=\"" + albumName + "\"]").OrderBy(x => ((XmlElement)x).GetAttribute("Title")) as IEnumerable<IXmlNode>;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<IXmlNode> GetAllArtists()
        {
            if (IsCollectionLoaded)
            {
                return CollectionDocument.SelectNodes("/Collection/Artist").OrderBy(node => ((XmlElement)node).GetAttribute("Name")) as IEnumerable<IXmlNode>;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<IXmlNode> GetAllAlbums()
        {

            string orderBy = "";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AlbumsSortBy"))
            {
                orderBy = ApplicationData.Current.LocalSettings.Values["AlbumsSortBy"].ToString();
            }
            else
            {
                orderBy = "Album";
            }

            if (orderBy == "Album")
                orderBy = "Title";

            if (IsCollectionLoaded)
            {
                return CollectionDocument.SelectNodes("/Collection/Album").OrderBy(node => ((XmlElement)node).GetAttribute(orderBy)) as IEnumerable<IXmlNode>;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<IXmlNode> GetAllSongs(bool sort)
        {
            string orderBy = "";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SongsSortBy"))
            {
                orderBy = ApplicationData.Current.LocalSettings.Values["SongsSortBy"].ToString();
            }
            else
            {
                orderBy = "Artist";
            }

            if (orderBy == "Song")
                orderBy = "Title";

            if (IsCollectionLoaded)
            {
                if (sort)
                    return CollectionDocument.SelectNodes("/Collection/Song").OrderBy(node => ((XmlElement)node).GetAttribute(orderBy)) as IEnumerable<IXmlNode>;
                else
                    return CollectionDocument.SelectNodes("/Collection/Song") as IEnumerable<IXmlNode>;
            }
            else
            {
                return null;
            }
        }

        public static List<Song> GetSongs(bool sort)
        {
            string orderBy = "";

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SongsSortBy"))
            {
                orderBy = ApplicationData.Current.LocalSettings.Values["SongsSortBy"].ToString();
            }
            else
            {
                orderBy = "Artist";
            }

            if (orderBy == "Song")
                orderBy = "Title";

            IEnumerable<IXmlNode> xmlList;
            List<Song> list = new List<Song>();

            if (IsCollectionLoaded)
            {
                if (sort)
                    xmlList = CollectionDocument.SelectNodes("/Collection/Song").OrderBy(node => ((XmlElement)node).GetAttribute(orderBy)) as IEnumerable<IXmlNode>;
                else
                    xmlList = CollectionDocument.SelectNodes("/Collection/Song") as IEnumerable<IXmlNode>;

                foreach (XmlElement element in xmlList)
                {
                    Song aux = new Song();
                    if (element == null)
                        return null;

                    aux.Set(element.GetAttribute("Title"),
                                    element.GetAttribute("Artist"),
                                    element.GetAttribute("Album"),
                                    element.GetAttribute("AlbumID"),
                                    element.GetAttribute("ID"),
                                    element.GetAttribute("Year"),
                                    element.GetAttribute("Track"),
                                    element.GetAttribute("Genre"),
                                    element.GetAttribute("Path"),
                                    element.GetAttribute("HexColor"));
                    list.Add(aux);
                }

                return list;
            }
            else
            {
                return null;
            }
        }


        public static List<string> GetAllSongsPaths()
        {
            if (IsCollectionLoaded)
            {
                string orderBy = "";

                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SongsSortBy"))
                {
                    orderBy = ApplicationData.Current.LocalSettings.Values["SongsSortBy"].ToString();
                }
                else
                {
                    orderBy = "Artist";
                }

                if (orderBy == "Song")
                    orderBy = "Title";

                List<string> list = new List<string>();

                IXmlNode[] nodeList = CollectionDocument.GetElementsByTagName("Song").OrderBy(node => ((XmlElement)node).GetAttribute(orderBy)).ToArray();

                for (int i = 0; i < nodeList.Count(); i++)
                {
                    XmlElement element = nodeList[i] as XmlElement;

                    list.Add(element.GetAttribute("Path"));
                }

                return list;
            }
            else
            {
                return null;
            }
        }

        public static List<Album> GetAlbumsByArtist(string artistName)
        {
            List<Album> list = new List<Album>();
            List<Song> listOfSongs = GetSongsByArtist(artistName);
            Album aux;
            if (IsCollectionLoaded)
            {
                foreach (Song s in listOfSongs)
                {
                    if (list.Exists(a => a.AlbumID == s.AlbumID) == false)
                    {
                        aux = CollectionHelper.GetAlbum(s.AlbumID);
                        list.Add(aux);
                    }
                }
            }

            list = list.OrderBy(s => s.Year).Reverse().ToList();

            return list;
        }

        public static XmlElement GetSongByPath(string path)
        {
            if (IsCollectionLoaded)
            {
                return CollectionDocument.SelectSingleNode("/Collection/Song[@Path=\"" + path + "\"]") as XmlElement;
            }
            else
            {
                return null;
            }
        }

        public static Album GetAlbum(string albumID)
        {
            if (IsCollectionLoaded)
            {
                Album aux = new Album();
                XmlElement element = CollectionDocument.SelectSingleNode("/Collection/Song[@AlbumID=\"" + albumID + "\"]") as XmlElement;
                aux.Set(element.GetAttribute("Album"),
                                element.GetAttribute("Artist"),
                                albumID,
                                element.GetAttribute("Year"),
                                element.GetAttribute("Genre"),
                                element.GetAttribute("HexColor"));
                return aux;
            }
            else
            {
                return null;
            }
        }

        public static Song GetSong(string path)
        {
            if (IsCollectionLoaded)
            {
                Song aux = new Song();
                XmlElement element = CollectionDocument.SelectSingleNode("/Collection/Song[@Path=\"" + path + "\"]") as XmlElement;
                if (element == null)
                    return null;

                aux.Set(element.GetAttribute("Title"),
                                element.GetAttribute("Artist"),
                                element.GetAttribute("Album"),
                                element.GetAttribute("AlbumID"),
                                element.GetAttribute("ID"),
                                element.GetAttribute("Year"),
                                element.GetAttribute("Track"),
                                element.GetAttribute("Genre"),
                                element.GetAttribute("Path"),
                                element.GetAttribute("HexColor"));
                return aux;
            }
            else
            {
                return null;
            }
        }

        public static XmlElement GetArtistByName(string artistName)
        {
            if (IsCollectionLoaded)
            {
                return CollectionDocument.SelectSingleNode("/Collection/Artist[@Name=\"" + artistName + "\"]") as XmlElement;
            }
            else
            {
                return null;
            }
        }

        public static async Task<string[]> GetSongFromLastPlaylist(int index)
        {
            IStorageItem lastPlaylistItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("LastPlaylist.xml");

            if (lastPlaylistItem != null)
            {
                XmlDocument doc = new XmlDocument();

                string content = await FileIO.ReadTextAsync(lastPlaylistItem as StorageFile);

                if (content != null && string.IsNullOrWhiteSpace(content) == false)
                {
                    doc.LoadXml(content);

                    string[] list = new string[3];

                    XmlElement song = null;

                    var elements = doc.GetElementsByTagName("Song");


                    if (index == 0)
                    {
                        song = elements[elements.Count - 1] as XmlElement;
                    }
                    else
                    {
                        song = elements[index - 1] as XmlElement;
                    }

                    list[0] = song.InnerText;

                    song = elements[index] as XmlElement;
                    list[1] = song.InnerText;


                    if (index < elements.Count - 1)
                    {
                        song = elements[index + 1] as XmlElement;
                    }
                    else
                    {
                        song = elements[0] as XmlElement;
                    }

                    list[2] = song.InnerText;

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

        /// <summary>
        /// Retorna uma lista com as músicas de determinado álbum em ordem de número de faixa
        /// </summary>
        /// <param name="albumID">Código de identificação do álbum</param>
        /// <returns></returns>
        public static List<Song> GetSongsByAlbumID(string albumID)
        {
            List<Song> list = new List<Song>();
            Song aux;
            if (IsCollectionLoaded)
            {
                XmlNodeList nodes = CollectionDocument.SelectNodes("/Collection/Song[@AlbumID=\"" + albumID + "\"]") as XmlNodeList;

                foreach (XmlElement element in nodes)
                {
                    aux = new Song();
                    aux.Set(element.GetAttribute("Title"),
                                element.GetAttribute("Artist"),
                                element.GetAttribute("Album"),
                                element.GetAttribute("AlbumID"),
                                element.GetAttribute("ID"),
                                element.GetAttribute("Year"),
                                element.GetAttribute("Track"),
                                element.GetAttribute("Genre"),
                                element.GetAttribute("Path"),
                                element.GetAttribute("HexColor"));
                    list.Add(aux);
                }
            }

            list = list.OrderBy(s => s.TrackNumber).ToList();

            return list;
        }

        public static Uri GetAlbumCoverPath(string albumID)
        {
            return new Uri("ms-appdata:///local/Covers/cover_" + albumID + ".jpg", UriKind.Absolute);
        }
    }
}
