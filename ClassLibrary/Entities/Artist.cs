using BackgroundAudioShared;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ClassLibrary.Entities
{
    public class Artist
    {
        public string Name
        {
            get;
            set;
        }

        public bool IsUpdatingImage 
        { 
            get;
            set; 
        }

        public Artist()
        {
            Name = string.Empty;
            IsUpdatingImage = false;
        }

        public void Set(string artistName)
        {
            Name = artistName;
        }

        public string GetName()
        {
            return Name;
        }

        public Artist Clone()
        {
            Artist a = new Artist();
            a.Set(Name);

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
