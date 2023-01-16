using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary.Model
{
    public class Song
    {
        public string Title
        {
            get;
            set;
        }

        public string Artist
        {
            get;
            set;
        }

        //public string[] Artists { get; set; }

        //public string AlbumArtist { get; set; }

        public string Album
        {
            get;
            set;
        }

        public string Genre
        {
            get;
            set;
        }

        public string Year
        {
            get;
            set;
        }

        public string Track
        {
            get;
            set;
        }

        public int TrackNumber
        {
            get
            {
                return Convert.ToInt32(Track);
            }
        }

        public string AlbumID
        {
            get;
            set;
        }

        public string ID
        {
            get;
            set;
        }

        public string SongURI
        {
            get;
            set;
        }

        public string HexColor
        {
            get;
            set;
        }

        public bool IsFavorite { get; set; }

        public DateTime DateAdded { get; set; }

        //public ImageSource Image
        //{
        //    get
        //    {
        //        return new BitmapImage(new Uri("ms-appdata:///local/Covers/cover_" + AlbumID + ".jpg", UriKind.Absolute));
        //    }
        //}

        public Song()
        {
            this.Title = string.Empty;
            this.Artist = string.Empty;
            this.Album = string.Empty;
            this.AlbumID = string.Empty;
            this.ID = string.Empty;
            this.Year = string.Empty;
            this.Track = string.Empty;
            this.Genre = string.Empty;
            this.SongURI = string.Empty;
            this.IsFavorite = false;
        }
    }
}
