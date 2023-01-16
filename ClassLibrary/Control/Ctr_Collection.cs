using ClassLibrary.Dao;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TagLib;
using Windows.Storage;
using Windows.UI.Xaml;

namespace ClassLibrary.Control
{
    public class Collection
    {
        public delegate void LoadingCollectionProgressChangedEventArgs(int progress, bool isLoading);

        public static event LoadingCollectionProgressChangedEventArgs ProgressChanged;

        private static bool Stop = false;

        /// <summary>
        /// Returns false if the collection is busy
        /// </summary>
        private static bool LoadCollectionCompleted = true;

        /// <summary>
        /// Creates the entire database of songs based on user's music collection
        /// </summary>
        /// <returns></returns>
        public static async Task CheckMusicCollection()
        {
            if (LoadCollectionCompleted == false)
                return;

            LoadCollectionCompleted = false;

            IReadOnlyList<StorageFile> musicFiles = await StorageHelper.ScanFolder(KnownFolders.MusicLibrary);

            bool result = RefreshDatabase();

            StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);

            Song aux = null;

            int totalCount = musicFiles.Count;

            double currentValue = 0;
            double percentagePerItem = Convert.ToDouble(100) / Convert.ToDouble(totalCount);

            List<Song> listOfSongs = new List<Song>();

            foreach (StorageFile file in musicFiles)
            {
                if (Stop)
                {
                    Stop = false;
                    break;
                }

                aux = await CreateSongObjectByFile(file, listOfSongs);
                if (aux != null)
                    listOfSongs.Add(aux);

                currentValue = currentValue + percentagePerItem;

                ProgressChanged.Invoke(Convert.ToInt16(currentValue), true);
            }

            ProgressChanged.Invoke(100, false);

            ApplicationSettings.IsCollectionLoaded = true;
            LoadCollectionCompleted = true;
        }

        /// <summary>
        /// Searches for differences between the database and the music collection. Then, if needed, it adds or removes the songs that doesn't exist on both lists
        /// </summary>
        public static async void LoadCollectionChanges()
        {
            if (LoadCollectionCompleted == false)
                return;

            Debug.WriteLine("INICIANDO BUSCA");

            LoadCollectionCompleted = false;

            bool changed = false;

            if (ApplicationSettings.IsCollectionLoaded)
            {
                List<Song> currentSongs = SongDao.GetSongs(false);
                List<string> folderSongs = new List<string>();
                if (currentSongs != null)
                {
                    List<Song> listOfSongs = currentSongs;
                    Song aux = null;

                    StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);

                    var songs = await StorageHelper.ScanFolder(KnownFolders.MusicLibrary);


                    int totalCount = songs.Count;

                    try
                    {
                        foreach (StorageFile file in songs)
                        {
                            if (Stop)
                            {
                                Stop = false;
                                break;
                            }

                            folderSongs.Add(file.Path);
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        // Searches for new songs in the music library
                        foreach (StorageFile file in songs)
                        {
                            if (Stop)
                            {
                                Stop = false;
                                break;
                            }

                            if (currentSongs.Any(s => s.SongURI == file.Path) == false)
                            {
                                if (Stop)
                                {
                                    Stop = false;
                                    break;
                                }

                                aux = await CreateSongObjectByFile(file, listOfSongs);

                                changed = true;
                                listOfSongs.Add(aux);
                            }
                        }
                    }
                    catch
                    {

                    }

                    try
                    {
                        // Removes any registry for a song that doesn't exist anymore in music library
                        foreach (Song song in currentSongs)
                        {
                            if (Stop)
                            {
                                Stop = false;
                                break;
                            }

                            if (folderSongs.Contains(song.SongURI) == false)
                            {
                                SongDao.RemoveSong(song);
                                changed = true;
                            }
                        }
                    }
                    catch
                    {

                    }

                }
            }

            Debug.WriteLine("BUSCA CONCLUÍDA");

            LoadCollectionCompleted = true;
        }

        private static async Task<Song> CreateSongObjectByFile(StorageFile file, List<Song> listOfSongs)
        {
            Song song = null;

            Stream fileStream = null;
            TagLib.File tagFile = null;
            string title = "";
            string artist = "";
            string album = "";
            string genre = "";
            string trackNumber = "";
            string year = "";

            try
            {
                if (Stop)
                {
                    Stop = false;
                    return new Song();
                }
                fileStream = await file.OpenStreamForReadAsync();
                tagFile = TagLib.File.Create(new StreamFileAbstraction(file.Name, fileStream, fileStream));

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

                        SaveCoverImageResult imageResult = await ImageHelper.SaveAlbumCover(dataVector, albumid);
                        hexColor = imageResult.HexColor;
                        //ImageHelper.BlurAlbumCover(albumid);
                    }
                }

                fileStream.Dispose();

                string songid = Guid.NewGuid().ToString();

                song = new Song()
                {
                    Title = title,
                    Artist = artist,
                    Album = album,
                    AlbumID = albumid,
                    ID = songid,
                    Year = year,
                    Track = trackNumber,
                    Genre = genre,
                    SongURI = file.Path,
                    HexColor = hexColor
                };

                SongDao.AddSong(song);
            }
            catch
            {

            }

            return song;
        }

        public static bool RefreshDatabase()
        {
            Stop = true;
            LoadCollectionCompleted = true;

            ApplicationSettings.SaveSettingsValue("CollectionLoaded", false);

            SqliteConnection db =
        new SqliteConnection("Filename=database.db");

            try
            {

                String tableCommand = "DELETE FROM songs";

                SqliteCommand command = new SqliteCommand(tableCommand, db);

                db.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            Stop = false;

            return true;
        }
        
        //public static List<Song> SearchSong(string value)
        //{
        //    List<Song> list = new List<Song>();



        //    return list;
        //}

        /// <summary>
        /// Represents the result of the loading collection procedure
        /// </summary>
        public enum LoadCollectionResult
        {
            Success,
            NotFound,
            Empty,
            SintaxError,
            Other
        }
    }


    public class LoadingCollectionProgressChangedEventArgs : RoutedEventArgs
    {
        public int Progress;

        public bool IsLoading;

        public LoadingCollectionProgressChangedEventArgs(int progress, bool isLoading)
        {
            this.Progress = progress;
            this.IsLoading = isLoading;
        }
    }
}
