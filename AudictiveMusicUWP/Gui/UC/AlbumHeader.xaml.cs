using AudictiveMusicUWP.Gui.Pages;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class AlbumHeader : UserControl
    {
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual headerSprite;

        private Album ALB
        {
            get;
            set;
        }

        public AlbumHeader()
        {
            this.Loaded += AlbumHeader_Loaded;
            this.SizeChanged += AlbumHeader_SizeChanged;
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void AlbumHeader_Loaded(object sender, RoutedEventArgs e)
        {
            headerSprite = _compositor.CreateSpriteVisual();

            BlendEffectMode blendmode = BlendEffectMode.Overlay;

            // Create a chained effect graph using a BlendEffect, blending color and blur
            var graphicsEffect = new BlendEffect
            {
                Mode = blendmode,
                Background = new ColorSourceEffect()
                {
                    Name = "Tint",
                    Color = Colors.Transparent,
                },

                Foreground = new GaussianBlurEffect()
                {
                    Name = "Blur",
                    Source = new CompositionEffectSourceParameter("Backdrop"),
                    BlurAmount = 10.0f,
                    BorderMode = EffectBorderMode.Hard,
                }
            };

            var blurEffectFactory = _compositor.CreateEffectFactory(graphicsEffect,
                new[] { "Blur.BlurAmount", "Tint.Color" });

            // Create EffectBrush, BackdropBrush and SpriteVisual
            _brush = blurEffectFactory.CreateBrush();

            var destinationBrush = _compositor.CreateBackdropBrush();
            _brush.SetSourceParameter("Backdrop", destinationBrush);

            headerSprite.Size = new Vector2((float)blurGlass.ActualWidth, (float)blurGlass.ActualHeight);
            headerSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(blurGlass, headerSprite);
        }

        private void AlbumHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 420)
            {
                this.Height = 250;
                border.Margin = new Thickness(25, 0, 10, 0);
                border.Height = border.Width = 150;
                secondRow.Height = thirdRow.Height = new GridLength(75);
            }
            else
            {
                this.Height = 230;
                border.Margin = new Thickness(10, -2, 0, 2);
                border.Height = border.Width = 130;
                secondRow.Height = thirdRow.Height = new GridLength(65);
            }

            if (headerSprite != null)
            {
                headerSprite.Size = e.NewSize.ToVector2();
            }
        }

        public void ClearContext()
        {
            this.DataContext = null;
            borderBrush.ImageSource = null;
            rootBrush.ImageSource = null;
            countOfSongs.Text = "";
        }

        public void SetContext(Album album)
        {
            this.DataContext = ALB = album;

            BitmapImage bmp = new BitmapImage();
            borderBrush.ImageSource = bmp;
            bmp.UriSource = new Uri("ms-appdata:///local/Covers/cover_" + ALB.AlbumID + ".jpg", UriKind.Absolute);


            BitmapImage blurbmp = new BitmapImage();
            rootBrush.ImageSource = blurbmp;
            blurbmp.UriSource = new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(ALB.Artist) + ".jpg", UriKind.Absolute);


            albumName.Text = ALB.Name.ToUpper();
        }

        public void UpdateNumberOfItems(int nSongs)
        {
            if (nSongs > 1)
                // BUSCA A STRING PLURAL JÁ QUE HÁ MAIS DE UMA MÚSICA NA LISTA
                countOfSongs.Text = nSongs + " " + ApplicationInfo.Current.Resources.GetString("SongPlural").ToLower();
            else
                // BUSCA A STRING SINGULAR JÁ QUE HÁ APENAS UMA MÚSICA NA LISTA
                countOfSongs.Text = nSongs + " " + ApplicationInfo.Current.Resources.GetString("SongSingular").ToLower();
        }

        private void border_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(da, border);
            Storyboard.SetTargetProperty(da, "Opacity");

            sb.Children.Add(da);

            sb.Begin();
        }

        private void border_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            borderBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/cover-error.png", UriKind.Absolute));
        }

        private void Blurbmp_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 0,
                To = 0.4,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(da, background);
            Storyboard.SetTargetProperty(da, "Opacity");

            sb.Children.Add(da);

            sb.Begin();
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageHelper.MainPage != null)
                PageHelper.MainPage.ShowPopupMenu(ALB, sender, new Point(0, 0), Enumerators.MediaItemType.Album);
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            var songs = Ctr_Song.Current.GetSongsByAlbum(ALB);

            foreach (Song song in songs)
                list.Add(song.SongURI);

            MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
        }

        private void artistName_Click(object sender, RoutedEventArgs e)
        {
            Artist art = new Artist()
            {
                Name = ALB.Artist
            };

            if (PageHelper.MainPage != null)
            {
                PageHelper.MainPage.Navigate(typeof(ArtistPage), art);
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            var songs = Ctr_Song.Current.GetSongsByAlbum(ALB);

            foreach (Song song in songs)
                list.Add(song.SongURI);

            PageHelper.MainPage.CreateAddToPlaylistPopup(list);
        }
    }
}
