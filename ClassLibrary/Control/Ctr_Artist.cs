using ClassLibrary.Dao;
using ClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Control
{
    public class Ctr_Artist
    {
        public delegate void FavoritesChangedEventArgs();

        public static event FavoritesChangedEventArgs FavoritesChanged;

        private static Ctr_Artist instance;

        public static Ctr_Artist Current
        {
            get
            {
                if (instance == null)
                    instance = new Ctr_Artist();

                return instance;
            }
        }

        public bool ArtistExists(Artist artist)
        {
            return ArtistDao.ArtistExists(artist);
        }

        public List<Artist> GetArtists()
        {
            List<Artist> list = new List<Artist>();

            Artist aux = null;

            var songs = Ctr_Song.Current.GetSongs(false);

            foreach (Song song in songs)
            {
                if (list.Exists(a => a.Name == song.Artist) == false)
                {
                    aux = new Artist()
                    {
                        Name = song.Artist,
                    };

                    list.Add(aux);
                }
            }

            return list;
        }
    }
}
