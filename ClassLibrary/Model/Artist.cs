using BackgroundAudioShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary.Model
{
    public class Artist
    {
        public string Name
        {
            get;
            set;
        }

        public Artist()
        {
            this.Name = string.Empty;
            this.IsUpdatingImage = false;
        }

        public Artist Clone()
        {
            Artist a = new Artist();
            a.Name = Name;

            return a;
        }

        public ImageSource BlurImage
        {
            get
            {
                return new BitmapImage(new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(Name) + "_blur.jpg", UriKind.Absolute));
            }
        }

        public ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(Name) + ".jpg", UriKind.Absolute));
            }
        }

        public bool IsUpdatingImage { get; set; }

        public Uri ImageUri
        {
            get
            {
                if (this.IsUpdatingImage)
                    return null;
                else
                    return new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(Name) + ".jpg", UriKind.Absolute);
            }
        }
    }
}
