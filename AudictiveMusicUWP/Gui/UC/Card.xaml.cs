using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class Card : UserControl
    {
        private Enumerators.MediaItemType ContextMode;

        public Card()
        {
            this.SizeChanged += Card_SizeChanged;
            this.InitializeComponent();
        }

        public void SetContext(object context)
        {
            this.DataContext = context;

            if (context.GetType() == typeof(Song))
            {
                this.ContextMode = Enumerators.MediaItemType.Song;
                Song song = context as Song;
                textRow1.Text = song.Title;
                textRow2.Text = song.Artist;

                Color color = ImageHelper.GetColorFromHex(song.HexColor);
                gradientStop1.Color = color.ChangeColorBrightness(-0.6f);
                gradientStop2.Color = color;

                BitmapImage bmp = new BitmapImage();
                bmp.ImageFailed += Bmp_ImageFailed;
                backgroundImage.ImageSource = bmp;

                bmp.UriSource = new Album() { AlbumID = song.AlbumID }.GetCoverUri();
            }
            else if (context.GetType() == typeof(Album))
            {
                this.ContextMode = Enumerators.MediaItemType.Album;

            }
            else if (context.GetType() == typeof(Artist))
            {
                this.ContextMode = Enumerators.MediaItemType.Artist;

            }
        }

        private void Bmp_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (this.ContextMode == Enumerators.MediaItemType.Song || this.ContextMode == Enumerators.MediaItemType.Album)
                ((BitmapImage)sender).UriSource = new Uri("ms-appx:///Assets/cover-error.png", UriKind.Absolute);
        }

        private void Card_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Height = e.NewSize.Width / 1.5;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ContextMode == Enumerators.MediaItemType.Song)
            {
                Song song = this.DataContext as Song;

                MessageService.SendMessageToBackground(new SetPlaylistMessage(new List<string>() { song.SongURI }));
            }
            else if (this.ContextMode == Enumerators.MediaItemType.Album)
            {

            }
            else if (this.ContextMode == Enumerators.MediaItemType.Artist)
            {

            }
        }

        private void dotsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ShowPopupMenu(this.DataContext, sender, e.GetPosition((FrameworkElement)sender), this.ContextMode);
        }
    }
}
