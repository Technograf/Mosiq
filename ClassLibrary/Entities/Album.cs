using ClassLibrary.Helpers;
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary.Entities
{
    public class Album
    {
        public string Name
        {
            get;
            set;
        }

        public string Artist
        {
            get;
            set;
        }

        public string AlbumID
        {
            get;
            set;
        }

        public string Genre
        {
            get;
            set;
        }

        public string HexColor
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        public ImageSource Image
        {
            get
            {
                return new BitmapImage(GetCoverUri());
            }
        }

        public Uri GetCoverUri()
        {
            return new Uri("ms-appdata:///local/Covers/cover_" + this.AlbumID + ".jpg", UriKind.Absolute);
        }

        public Color Color
        {
            get
            {
                return ImageHelper.GetColorFromHex(HexColor);
            }
        }

        public Album()
        {
            this.Name = string.Empty;
            this.AlbumID = string.Empty;
            this.Year = 0;
            this.Genre = string.Empty;
            this.Artist = string.Empty;
            this.HexColor = string.Empty;
        }
    }
}
