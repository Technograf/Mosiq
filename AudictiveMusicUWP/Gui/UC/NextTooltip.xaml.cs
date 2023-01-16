using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class NextTooltip : UserControl
    {
        private Compositor _compositor;
        private SpriteVisual blurSprite;
        private CompositionEffectBrush _brush;

        public string Status
        {
            get { return ((string)GetValue(StatusProperty)); }
            set
            {
                SetValue(StatusProperty, value);
            }
        }

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(NextTooltip), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return ((string)GetValue(TitleProperty)); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NextTooltip), new PropertyMetadata(string.Empty));

        public string Subtitle
        {
            get { return ((string)GetValue(SubtitleProperty)); }
            set
            {
                SetValue(SubtitleProperty, value);
            }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(NextTooltip), new PropertyMetadata(string.Empty));


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(NextTooltip), null);


        public ImageSource FallbackSource
        {
            get { return (ImageSource)GetValue(FallbackSourceProperty); }
            set { SetValue(FallbackSourceProperty, value); }
        }

        public static readonly DependencyProperty FallbackSourceProperty =
            DependencyProperty.Register("FallbackSource", typeof(ImageSource), typeof(NextTooltip), new PropertyMetadata(new BitmapImage(new Uri("ms-appx:///Assets/cover-error.png", UriKind.Absolute))));

        public Color AccentColor
        {
            get { return ((Color)GetValue(AccentColorProperty)); }
            set
            {
                SetValue(AccentColorProperty, value);

                if (value.IsDarkColor())
                {
                    this.RequestedTheme = ElementTheme.Dark;
                    //MusicProgress.Foreground = new SolidColorBrush(lighterColor);
                }
                else
                {
                    this.RequestedTheme = ElementTheme.Light;
                    //MusicProgress.Foreground = new SolidColorBrush(darkerColor);
                }
            }
        }

        public static readonly DependencyProperty AccentColorProperty =
            DependencyProperty.Register("AccentColor", typeof(Color), typeof(NextTooltip), new PropertyMetadata(Colors.Transparent));


        public NextTooltip()
        {
            this.Loaded += NextTooltip_Loaded;
            this.SizeChanged += NextTooltip_SizeChanged;
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void NextTooltip_Loaded(object sender, RoutedEventArgs e)
        {
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

            blurSprite = _compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);
            blurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(acrylic, blurSprite);
        }

        private void NextTooltip_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (blurSprite != null)
            {
                blurSprite.Size = e.NewSize.ToVector2();
            }
        }

        private void image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (FallbackSource != null)
            {
                fallbackBrush.ImageSource = FallbackSource;
            }
        }

        public enum Mode
        {
            Previous,
            Next
        }
    }
}
