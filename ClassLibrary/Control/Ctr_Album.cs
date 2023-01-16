using ClassLibrary.Dao;
using ClassLibrary.Entities;
using System;
using System.Collections.Generic;

namespace ClassLibrary.Control
{
    public class Ctr_Album
    {
        private static Ctr_Album instance;

        public static Ctr_Album Current
        {
            get
            {
                if (instance == null)
                    instance = new Ctr_Album();

                return instance;
            }
        }

        public List<Album> GetAlbums()
        {
            List<Album> list = new List<Album>();
            Album aux = null;

            var songs = Ctr_Song.Current.GetSongs(false);

            foreach (Song song in songs)
            {
                if (list.Exists(a => a.Name == song.Album && a.Artist == song.Artist) == false)
                {
                    aux = new Album()
                    {
                        Name = song.Album,
                        Artist = song.Artist,
                        AlbumID = song.AlbumID,
                        Year = Convert.ToInt32(song.Year),
                        Genre = song.Genre,
                        HexColor = song.HexColor
                    };

                    list.Add(aux);
                }
            }

            return list;
        }

        public Album GetAlbum(Album Album)
        {
            return AlbumDao.GetAlbum(Album);
        }

        public List<Album> GetAlbumsByArtist(Artist artist)
        {
            return AlbumDao.GetAlbumsByArtist(artist);
        }
    }
}
