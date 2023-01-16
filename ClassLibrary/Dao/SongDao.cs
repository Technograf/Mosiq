using ClassLibrary.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Dao
{
    public class SongDao
    {
        public delegate void FavoritesChangedEventArgs();

        public static event FavoritesChangedEventArgs FavoritesChanged;

        public static bool AddSong(Song song)
        {
            bool result = false;

            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;
                command.CommandText = @"INSERT INTO songs (Title,Artist,Album,Genre,Year,Track,AlbumID,URI,HexColor,Star,DateAdded) VALUES (@TITLE, @ARTIST, @ALBUM, @GENRE, @YEAR, @TRACK, @ALBUMID, @URI, @HEXCOLOR, 0, @DATEADDED);";
                command.Parameters.AddWithValue("@TITLE", song.Title);
                command.Parameters.AddWithValue("@ARTIST", song.Artist);
                command.Parameters.AddWithValue("@ALBUM", song.Album);
                command.Parameters.AddWithValue("@GENRE", song.Genre);
                command.Parameters.AddWithValue("@YEAR", song.Year);
                command.Parameters.AddWithValue("@TRACK", song.Track);
                command.Parameters.AddWithValue("@ALBUMID", song.AlbumID);
                command.Parameters.AddWithValue("@URI", song.SongURI);
                command.Parameters.AddWithValue("@HEXCOLOR", song.HexColor);
                command.Parameters.AddWithValue("@DATEADDED", DateTime.Now);

                command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!!! " + ex.Message);
                result = false;
            }
            finally
            {
                db.Close();
            }

            return result;
        }

        public static List<Song> GetSongsByArtist(Artist artist)
        {
            List<Song> list = new List<Song>();
            Song song;

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs WHERE Artist = @ARTIST ";
                command.Parameters.AddWithValue("@ARTIST", artist.Name);

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song = new Song();
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);

                    list.Add(song);
                }


            }
            catch
            {

            }
            finally
            {
                db.Close();
            }

            return list;

        }

        public static List<Song> GetSongsByAlbum(Album album)
        {
            List<Song> list = new List<Song>();
            Song song;

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs WHERE AlbumID = @ALBUMID ";
                command.Parameters.AddWithValue("@ALBUMID", album.AlbumID);

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song = new Song();
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);

                    list.Add(song);
                }


            }
            catch
            {

            }
            finally
            {
                db.Close();
            }

            return list;

        }

        public static Song GetSong(Song song)
        {
            bool result;
            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs WHERE URI = @URI ";
                command.Parameters.AddWithValue("@URI", song.SongURI);

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);
                    song.IsFavorite = Convert.ToBoolean(query.GetInt16(10));
                    song.DateAdded = DateTime.Parse(query.GetString(11));
                }

                result = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
                result = false;
            }
            finally
            {
                db.Close();
            }

            if (result)
                return song;
            else
                return null;
        }

        public static List<Song> GetSongsByPath(string path)
        {
            List<Song> list = new List<Song>();
            Song song;

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs WHERE URI LIKE @URI ";
                command.Parameters.AddWithValue("@URI", path + "%");

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song = new Song();
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);
                    song.IsFavorite = Convert.ToBoolean(query.GetInt16(10));
                    song.DateAdded = DateTime.Parse(query.GetString(11));

                    list.Add(song);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            return list;
        }

        public static List<Song> GetSongs(bool sort)
        {
            List<Song> list = new List<Song>();
            Song song;

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs ";

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song = new Song();
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);
                    song.IsFavorite = Convert.ToBoolean(query.GetInt16(10));
                    song.DateAdded = DateTime.Parse(query.GetString(11));

                    list.Add(song);
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            return list;

        }

        public static List<string> GetAllSongsPaths()
        {
            List<string> list = new List<string>();

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT URI FROM songs ";

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    list.Add(query.GetString(0));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            return list;
        }

        public static List<Song> GetFavoriteSongs()
        {
            List<Song> list = new List<Song>();
            Song song;

            SqliteConnection db =
new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT * FROM songs WHERE Star = 1";

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    song = new Song();
                    song.ID = query.GetString(0);
                    song.Title = query.GetString(1);
                    song.Artist = query.GetString(2);
                    song.Album = query.GetString(3);
                    song.Genre = query.GetString(4);
                    song.Year = query.GetString(5);
                    song.Track = query.GetString(6);
                    song.AlbumID = query.GetString(7);
                    song.SongURI = query.GetString(8);
                    song.HexColor = query.GetString(9);
                    song.IsFavorite = Convert.ToBoolean(query.GetInt16(10));
                    song.DateAdded = DateTime.Parse(query.GetString(11));

                    list.Add(song);
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            return list;

        }

        public static bool SongExists(Song song)
        {
            int result = 0;
            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "SELECT EXISTS(SELECT 1 FROM songs WHERE URI = @URI LIMIT 1)";
                command.Parameters.AddWithValue("@URI", song.SongURI);

                SqliteDataReader query = command.ExecuteReader();

                while (query.Read())
                {
                    result = query.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO!!! " + ex.Message);
            }
            finally
            {
                db.Close();
            }

            if (result == 0)
                return false;
            else
                return true;
        }

        public static bool UpdateSong(Song song)
        {
            bool result = false;

            using (SqliteConnection db =
        new SqliteConnection("Filename=database.db"))
            {
                try
                {
                    db.Open();

                    SqliteCommand command = new SqliteCommand();
                    command.Connection = db;

                    command.CommandText = "UPDATE songs SET Title = @TITLE, Artist = @ARTIST, Album = @ALBUM, Genre = @GENRE, Year = @YEAR, Track = @TRACK, AlbumID = @ALBUMID, URI = @URI, HexColor = @HEXCOLOR WHERE ID = @ID;";
                    command.Parameters.AddWithValue("@TITLE", song.Title);
                    command.Parameters.AddWithValue("@ARTIST", song.Artist);
                    command.Parameters.AddWithValue("@ALBUM", song.Album);
                    command.Parameters.AddWithValue("@GENRE", song.Genre);
                    command.Parameters.AddWithValue("@YEAR", song.Year);
                    command.Parameters.AddWithValue("@TRACK", song.Track);
                    command.Parameters.AddWithValue("@ALBUMID", song.AlbumID);
                    command.Parameters.AddWithValue("@URI", song.SongURI);
                    command.Parameters.AddWithValue("@HEXCOLOR", song.HexColor);
                    command.Parameters.AddWithValue("@ID", song.ID);

                    command.ExecuteNonQuery();

                    result = true;
                }
                catch
                {
                    result = false;
                }
                finally
                {
                    db.Close();
                }
            }

            return result;
        }

        public static bool SetSongFavoriteState(Song song, bool state)
        {
            bool result = false;

            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "UPDATE songs SET Star = @STAR WHERE ID = @ID;";
                command.Parameters.AddWithValue("@STAR", Convert.ToInt16(state));
                command.Parameters.AddWithValue("@ID", song.ID);

                command.ExecuteNonQuery();

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                db.Close();
            }

            FavoritesChanged?.Invoke();

            return result;
        }

        public static bool RemoveSong(Song song)
        {
            bool result = false;

            SqliteConnection db =
        new SqliteConnection("Filename=database.db");
            try
            {
                db.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = db;

                command.CommandText = "DELETE FROM songs WHERE ID = @ID;";
                command.Parameters.AddWithValue("@ID", song.ID);

                command.ExecuteNonQuery();

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                db.Close();
            }


            return result;
        }
    }
}
