using AudictiveMusicUWP.Gui.Pages;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using IF.Lastfm.Core.Objects;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.Globalization.NumberFormatting;
using Windows.Graphics.DirectX;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class MediaPageHeader : UserControl
    {
        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual headerSprite;

        private Artist ART
        {
            get;
            set;
        }

        private LastUser LastUser
        {
            get;
            set;
        }

        private LastArtist LastART
        {
            get;
            set;
        }

        public MediaPageHeader()
        {
            this.SizeChanged += MediaPageHeader_SizeChanged;
            this.Loaded += MediaPageHeader_Loaded;
            this.InitializeComponent();

            LastFm.DownloadCompleted += LastFm_DownloadCompleted;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void LastFm_DownloadCompleted(Artist artist)
        {
            if (artist.Name == ART.Name)
            {
                SetContext(ART);
            }
        }

        private void MediaPageHeader_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 420)
            {
                this.Height = 250;
                ellipse.Margin = new Thickness(25, 0, 10, 0);
                ellipse.Height = ellipse.Width = 150;
                secondRow.Height = thirdRow.Height = new GridLength(75);
            }
            else
            {
                this.Height = 200;
                ellipse.Margin = new Thickness(15, 5, 10, -5);
                ellipse.Height = ellipse.Width = 100;
                secondRow.Height = thirdRow.Height = new GridLength(50);
            }

            //if (ApplicationInfo.Current.WindowSize.Height > 700)
            //{
            //    this.Height = 260;
            //    blackLayer.Height = 200;
            //    firstRow.Height = new GridLength(140, GridUnitType.Pixel);
            //}
            //else if (ApplicationInfo.Current.WindowSize.Height > 550)
            //{
            //    this.Height = 240;
            //    blackLayer.Height = 180;
            //    firstRow.Height = new GridLength(120, GridUnitType.Pixel);
            //}
            //else
            //{
            //    this.Height = 220;
            //    blackLayer.Height = 160;
            //    firstRow.Height = new GridLength(100, GridUnitType.Pixel);
            //}

            if (headerSprite != null)
            {
                headerSprite.Size = background.RenderSize.ToVector2();
            }
        }

        private void MediaPageHeader_Loaded(object sender, RoutedEventArgs e)
        {
            switch (ApplicationSettings.NowPlayingTheme)
            {
                case ClassLibrary.Themes.Theme.Clean:


                    break;
                case ClassLibrary.Themes.Theme.Blur:
                    SetBlur();
                    break;
                case ClassLibrary.Themes.Theme.Modern:
                    SetModernStyle();
                    break;
                case ClassLibrary.Themes.Theme.Material:
                    SetModernStyle();
                    break;
            }


            
        }

        private async void SetModernStyle()
        {
            var canvasDevice = CanvasDevice.GetSharedDevice();
            var graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, canvasDevice);

            var bitmap = await CanvasBitmap.LoadAsync(canvasDevice, new Uri("ms-appx:///Assets/points.png"));

            var drawingSurface = graphicsDevice.CreateDrawingSurface(bitmap.Size,
                DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
            using (var ds = CanvasComposition.CreateDrawingSession(drawingSurface))
            {
                ds.Clear(Colors.Transparent);
                ds.DrawImage(bitmap);
            }

            var surfaceBrush = _compositor.CreateSurfaceBrush(drawingSurface);
            surfaceBrush.Stretch = CompositionStretch.None;

            var border = new BorderEffect
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                Source = new CompositionEffectSourceParameter("source")
            };

            var fxFactory = _compositor.CreateEffectFactory(border);
            var fxBrush = fxFactory.CreateBrush();
            fxBrush.SetSourceParameter("source", surfaceBrush);

            headerSprite = _compositor.CreateSpriteVisual();
            headerSprite.Size = new Vector2((float)blurGlass.ActualWidth, (float)blurGlass.ActualHeight);
            headerSprite.Brush = fxBrush;

            ElementCompositionPreview.SetElementChildVisual(blurGlass, headerSprite);
        }

        private void SetBlur()
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
                    BlurAmount = 4.0f,
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

        public void ClearContext()
        {
            this.DataContext = null;
            //ellipseBrush.ImageSource = null;
            ellipse.Source = null;
            rootBrush.ImageSource = null;
            //rootBrush.Color = Colors.Transparent;
            subtitle1.Text = subtitle2.Text = "";
            subtitleSeparator.Visibility = Visibility.Collapsed;
        }

        public async void SetContext(Artist artist)
        {
            this.DataContext = ART = artist;
            this.ART.IsUpdatingImage = false;
            //Color color;

            StorageFile imgFile = null;

            try
            {
                imgFile = await StorageFile.GetFileFromApplicationUriAsync(artist.ImageUri);
            }
            catch
            {

            }

            try
            {
                if (imgFile != null)
                {
                    using (var stream = await imgFile.OpenAsync(FileAccessMode.Read))
                    {
                        //color = await ImageHelper.GetDominantColor(stream);
                        BitmapImage b = new BitmapImage();
                        rootBrush.ImageSource = b;
                        b.SetSource(stream);
                    }

                    ellipse.SetSource(artist.ImageUri);
                }
                else
                {
                    await LastFm.DownloadImage(artist, true);
                }
            }
            catch
            {

            }

            //AnimateBackgroundToColor(color);

            ////ellipseBrush.ImageSource = bmp;
            //ellipse.Source = bmp;
            //bmp.UriSource = new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(ART.Name) + ".jpg", UriKind.Absolute);


            //BitmapImage blurbmp = new BitmapImage();
            //blurbmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //rootBrush.ImageSource = blurbmp;
            //blurbmp.UriSource = new Uri("ms-appdata:///local/Artists/artist_" + StringHelper.RemoveSpecialChar(ART.Name) + ".jpg", UriKind.Absolute);


            title.Text = ART.Name.ToUpper();
            subtitleSeparator.Visibility = subtitle2.Visibility = Visibility.Visible;
        }

        public async void SetContext(LastUser user)
        {
            this.DataContext = LastUser = user;

            buttonsArea.Visibility = Visibility.Collapsed;
            //MessageDialog md = new MessageDialog(user.Avatar.Medium.AbsoluteUri);
            //await md.ShowAsync();

            //color = await ImageHelper.GetDominantColor(stream);
            BitmapImage b = new BitmapImage();
            rootBrush.ImageSource = b;
            b.UriSource = LastUser.Avatar.Large;

            ellipse.SetSource(LastUser.Avatar.Large, CircleImage.ImageType.LastFmUser);

            title.Text = user.Name.ToLower();
            subtitle1.Text = user.FullName;
            playCount.Text = Convert.ToString(user.Playcount) + " " + ApplicationInfo.Current.Resources.GetString("Scrobbles").ToUpper();
        }

        public async void SetContext(LastArtist artist)
        {
            this.DataContext = LastART = artist;

           // audictiveButton.Visibility = Ctr_Artist.Current.ArtistExists(new Artist() { Name = artist.Name }) ? Visibility.Visible : Visibility.Collapsed;

            buttonsArea.Visibility = Visibility.Collapsed;

            BitmapImage b = new BitmapImage();
            rootBrush.ImageSource = b;
            b.UriSource = LastART.MainImage.Large;

            ellipse.SetSource(LastART.MainImage.Large, CircleImage.ImageType.LastFmArtist);

            title.Text = LastART.Name.ToUpper();
            //subtitle1.Text = Convert.ToString(LastART.PlayCount);
            var tags = LastART.Tags.ToList();

            // FORMAT THE LISTENERS COUNT TO DISPLAY AS A GROUPED INT (1,000,000)
            DecimalFormatter formatter = new DecimalFormatter()
            {
                IsGrouped = true,
                FractionDigits = 0
            };

            try
            {
                playCount.Text = formatter.FormatInt(LastART.Stats.Listeners) + " " + ApplicationInfo.Current.Resources.GetString("Listeners").ToUpper();
            }
            catch
            {

            }
        }

        private void AnimateBackgroundToColor(Color color)
        {
            Storyboard sb = new Storyboard();

            ColorAnimation ca = new ColorAnimation()
            {
                From = Colors.Transparent,
                To = color,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(ca, rootBrush);
            Storyboard.SetTargetProperty(ca, "Color");

            sb.Children.Add(ca);
            sb.Begin();
        }

        public void UpdateNumberOfItems(int nSongs, int nAlbums)
        {
            if (nAlbums > 1)
                // BUSCA A STRING PLURAL JÁ QUE HÁ MAIS DE UM ÁLBUM NA LISTA
                subtitle1.Text = nAlbums + " " + ApplicationInfo.Current.Resources.GetString("AlbumPlural").ToLower();
            else
                // BUSCA A STRING SINGULAR JÁ QUE HÁ APENAS UM ÁLBUM NA LISTA
                subtitle1.Text = nAlbums + " " + ApplicationInfo.Current.Resources.GetString("AlbumSingular").ToLower();

            if (nSongs > 1)
                // BUSCA A STRING PLURAL JÁ QUE HÁ MAIS DE UMA MÚSICA NA LISTA
                subtitle2.Text = nSongs + " " + ApplicationInfo.Current.Resources.GetString("SongPlural").ToLower();
            else
                // BUSCA A STRING SINGULAR JÁ QUE HÁ APENAS UMA MÚSICA NA LISTA
                subtitle2.Text = nSongs + " " + ApplicationInfo.Current.Resources.GetString("SongSingular").ToLower();
        }

        public void UpdateNumberOfItems(int nSongs)
        {

        }

        private void ellipse_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            var songs = Ctr_Song.Current.GetSongsByArtist(ART);

            foreach (Song song in songs)
                list.Add(song.SongURI);

            MessageService.SendMessageToBackground(new SetPlaylistMessage(list));
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            var songs = Ctr_Song.Current.GetSongsByArtist(ART);

            foreach (Song song in songs)
                list.Add(song.SongURI);

            PageHelper.MainPage.CreateAddToPlaylistPopup(list);
        }

        private void moreButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageHelper.MainPage != null)
                PageHelper.MainPage.ShowPopupMenu(ART, sender, new Point(0, 0), Enumerators.MediaItemType.Artist);
        }

        private async void ellipseBitmap_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(da, ellipse);
            Storyboard.SetTargetProperty(da, "Opacity");

            sb.Children.Add(da);

            sb.Begin();

            //MessageDialog md = new MessageDialog("Carregou imagem");
            //await md.ShowAsync();
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

        private void artistImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MenuFlyout menu = new MenuFlyout()
            {
                MenuFlyoutPresenterStyle = Application.Current.Resources["MenuFlyoutModernStyle"] as Style,
            };

            MenuFlyoutItem item1 = new MenuFlyoutItem()
            {
                Text = "Ampliar",
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };

            MenuFlyoutItem item2 = new MenuFlyoutItem()
            {
                Text = "Salvar imagem",
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };

            MenuFlyoutItem item3 = new MenuFlyoutItem()
            {
                Text = "Compartilhar imagem",
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };

            MenuFlyoutItem item4 = new MenuFlyoutItem()
            {
                Text = "Buscar nova imagem",
                Tag = "",
                Style = Application.Current.Resources["ModernMenuFlyoutItem"] as Style,
            };

            item1.Click += (s, a) =>
            {
                //PageHelper.MainPage.Navigate(typeof(ImagePreview), ART);
            };

            //item2.Click += async (s, a) =>
            //{
            //    try
            //    {
            //        var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Artists");
            //        StorageFile imgFile = await folder.GetFileAsync("artist_" + StringHelper.RemoveSpecialChar(ART.Name) + ".jpg");

            //        await imgFile.CopyAsync(KnownFolders.PicturesLibrary);

            //        MessageDialog md = new MessageDialog("Imagem salva em sua biblioteca de imagens", "Imagem salva");
            //        await md.ShowAsync();
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(ex.Message);
            //    }
            //};

            item3.Click += (s, a) =>
            {

            };

            item4.Click += async (s, a) =>
            {
                if (ApplicationInfo.Current.HasInternetConnection)
                {
                    this.ART.IsUpdatingImage = true;
                    //if (PageHelper.Artists != null)
                    //    PageHelper.Artists.NavigationCacheMode = NavigationCacheMode.Disabled;

                    ellipse.RemoveSource();
                    rootBrush.ImageSource = null;
                    await LastFm.DownloadImage(ART, true);
                }
            };


            menu.Items.Add(item1);
            menu.Items.Add(item2);
            menu.Items.Add(item3);
            menu.Items.Add(item4);
            menu.Placement = FlyoutPlacementMode.Bottom;
            menu.ShowAt(ellipse);
        }


        private void ellipseBitmap_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //ellipseBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/artist-error.png", UriKind.Absolute));
        }

        private void audictiveButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(ArtistPage), new Artist() { Name = this.LastART.Name });
        }
    }
}
