using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary.Entities
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

        public bool IsFavorite 
        {
            get; 
            set; 
        }

        public DateTime DateAdded 
        { 
            get; 
            set; 
        }

        public ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("ms-appdata:///local/Covers/cover_" + AlbumID + ".jpg", UriKind.Absolute));
            }
        }

        public Song()
        {
            Title = string.Empty;
            Artist = string.Empty;
            Album = string.Empty;
            AlbumID = string.Empty;
            ID = string.Empty;
            Year = string.Empty;
            Track = string.Empty;
            Genre = string.Empty;
            SongURI = string.Empty;
        }

        //public void Set(string title, string artist, string album, string albumID,
        //    string id, string year, string trackNumber, string genre, string path, string hexColor)
        //{
        //    Title = title;
        //    Artist = artist;
        //    Album = album;
        //    AlbumID = albumID;
        //    ID = id;
        //    Year = year;
        //    Track = trackNumber;
        //    Genre = genre;
        //    SongURI = path;
        //    HexColor = hexColor;
        //}
    }
}
