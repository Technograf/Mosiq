using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class CircleImage : UserControl
    {
        public enum ImageType
        {
            Artist,
            LastFmArtist,
            LastFmUser,
        }

        public ImageType Type;

        private bool HasButtonIcon
        {
            get;
            set;
        }

        private double EllipseLenght
        {
            get;
            set;
        }

        public CircleImage()
        {
            this.SizeChanged += CircleImage_SizeChanged;
            this.InitializeComponent();
        }

        private void CircleImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            border.CornerRadius = new CornerRadius(e.NewSize.Width * 2);
            EllipseLenght = pointerIndicator.Height = pointerIndicator.Width = e.NewSize.Width * 2;
        }

        public delegate void RoutedEventArgs(object sender, EventArgs e);

        public event RoutedEventArgs ImageFailed;
        public event RoutedEventArgs ActionClick;


        public bool IsRevealEnabled
        {
            get { return ((bool)GetValue(IsRevealEnabledProperty)); }
            set
            {
                SetValue(IsRevealEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsRevealEnabledProperty =
            DependencyProperty.Register("IsRevealEnabled", typeof(bool), typeof(CircleImage), new PropertyMetadata(false));


        public double RevealOpacity
        {
            get { return ((double)GetValue(RevealOpacityProperty)); }
            set
            {
                SetValue(RevealOpacityProperty, value);
            }
        }

        public static readonly DependencyProperty RevealOpacityProperty =
            DependencyProperty.Register("RevealOpacity", typeof(double), typeof(CircleImage), new PropertyMetadata(0.3));


        public Brush Stroke
        {
            get { return ((Brush)GetValue(StrokeProperty)); }
            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(CircleImage), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        public Thickness StrokeThickness
        {
            get { return ((Thickness)GetValue(StrokeThicknessProperty)); }
            set
            {
                SetValue(StrokeThicknessProperty, value);
            }
        }

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(Thickness), typeof(CircleImage), new PropertyMetadata(new Thickness(0)));



        public string Glyph
        {
            get { return Convert.ToString(GetValue(GlyphProperty)); }
            set
            {
                HasButtonIcon = true;
                SetValue(GlyphProperty, value);
            }
        }

        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(CircleImage), new PropertyMetadata(string.Empty));


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(CircleImage), null);

        public Uri ImageUri
        {
            get { return (Uri)GetValue(ImageUriProperty); }
            set
            {
                SetValue(ImageUriProperty, value);
                SetSource(value);
            }
        }

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(CircleImage), null);

        public ImageSource FallbackSource
        {
            get { return (ImageSource)GetValue(FallbackSourceProperty); }
            set { SetValue(FallbackSourceProperty, value); }
        }

        public static readonly DependencyProperty FallbackSourceProperty =
            DependencyProperty.Register("FallbackSource", typeof(ImageSource), typeof(CircleImage), null);


        public async Task SetSource(IRandomAccessStream stream)
        {
            BitmapImage bmp = new BitmapImage();
            image.ImageSource = bmp;
            bmp.SetSource(stream);

            stream.Dispose();
        }

        public async void SetSource(Uri source, ImageType type = ImageType.Artist)
        {
            if (source == null)
                this.RemoveSource();

            if (type == ImageType.Artist)
            {
                try
                {
                    StorageFile imgFile = await StorageFile.GetFileFromApplicationUriAsync(source);

                    if (imgFile != null)
                    {
                        using (var stream = await imgFile.OpenAsync(FileAccessMode.Read))
                        {
                            //color = await ImageHelper.GetDominantColor(stream);
                            BitmapImage bmp = new BitmapImage();
                            this.Source = bmp;
                            bmp.SetSource(stream);

                            stream.Dispose();
                        }
                    }
                }
                catch
                {

                }
            }
            else
            {
                BitmapImage bmp = new BitmapImage();
                this.Source = bmp;
                bmp.UriSource = source;
            }
        }

        public void RemoveSource()
        {
            this.Source = null;
            this.Source = FallbackSource;
        }

        private void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ImageFailed?.Invoke(this, EventArgs.Empty);

            if (FallbackSource != null)
            {
                fallbackBrush.ImageSource = FallbackSource;
            }
        }

        private void image_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Storyboard sb = this.Resources["imageOpenedAnimation"] as Storyboard;
            sb.Begin();
        }


#region BORDER POINTER EVENTS

        private void border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                return;

            if (IsRevealEnabled)
                pointerIndicator.Opacity = 1;

            //if (HasButtonIcon)
            //    actionButton.Visibility = Visibility.Visible;
            //else
            //    actionButton.Visibility = Visibility.Collapsed;
        }

        private void border_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                return;

            if (IsRevealEnabled)
                pointerIndicator.Opacity = 1;

            //if (HasButtonIcon)
            //    actionButton.Visibility = Visibility.Visible;
            //else
            //    actionButton.Visibility = Visibility.Collapsed;

            PointerPoint point = e.GetCurrentPoint(this);
            pointerIndicator.Margin = new Thickness(point.Position.X - (EllipseLenght / 2), point.Position.Y - (EllipseLenght / 2), 0, 0);
        }

        private void border_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            pointerIndicator.Opacity = 0;
            //actionButton.Visibility = Visibility.Collapsed;
        }

        private void border_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
                return;

            if (IsRevealEnabled)
                pointerIndicator.Opacity = 1;

            //if (HasButtonIcon)
            //    actionButton.Visibility = Visibility.Visible;
            //else
            //    actionButton.Visibility = Visibility.Collapsed;
        }

        private void border_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            pointerIndicator.Opacity = 0;
            //actionButton.Visibility = Visibility.Collapsed;
        }

        private void border_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            pointerIndicator.Opacity = 0;
            //actionButton.Visibility = Visibility.Collapsed;
        }

        private void border_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            pointerIndicator.Opacity = 0;
            //actionButton.Visibility = Visibility.Collapsed;
        }

#endregion BORDER POINTER REGION

        private void runAction_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ActionClick?.Invoke(this, EventArgs.Empty);
        }
    }
}
