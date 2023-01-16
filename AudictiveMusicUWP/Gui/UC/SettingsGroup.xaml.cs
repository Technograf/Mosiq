using System;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static ClassLibrary.Helpers.Enumerators;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class SettingsGroup : UserControl
    {
        private double PointerIndicatorLength;

        public delegate void RoutedEventArgs(object sender, EventArgs e);

        public event RoutedEventArgs Click;

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(SettingsGroup), new PropertyMetadata(string.Empty));

        public SettingsPageContent ReferencesTo
        {
            get { return (SettingsPageContent)GetValue(ReferencesToProperty); }
            set { SetValue(ReferencesToProperty, value); }
        }

        public static readonly DependencyProperty ReferencesToProperty =
            DependencyProperty.Register("ReferencesTo", typeof(SettingsPageContent), typeof(SettingsGroup), new PropertyMetadata(SettingsPageContent.None));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SettingsGroup), new PropertyMetadata(string.Empty));


        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(SettingsGroup), new PropertyMetadata(string.Empty));

        public bool IsRevealEnabled
        {
            get { return ((bool)GetValue(IsRevealEnabledProperty)); }
            set
            {
                SetValue(IsRevealEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsRevealEnabledProperty =
            DependencyProperty.Register("IsRevealEnabled", typeof(bool), typeof(SettingsGroup), new PropertyMetadata(false));


        public int IconSize
        {
            get { return ((int)GetValue(IconSizeProperty)); }
            set
            {
                SetValue(IconSizeProperty, value);
            }
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(int), typeof(SettingsGroup), new PropertyMetadata(30));


        public double RevealOpacity
        {
            get { return ((double)GetValue(RevealOpacityProperty)); }
            set
            {
                SetValue(RevealOpacityProperty, value);
            }
        }

        public static readonly DependencyProperty RevealOpacityProperty =
            DependencyProperty.Register("RevealOpacity", typeof(double), typeof(SettingsGroup), new PropertyMetadata(0.2));

        public SettingsGroup()
        {
            this.SizeChanged += SettingsGroup_SizeChanged;
            this.InitializeComponent();
        }

        private void SettingsGroup_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //PointerIndicatorLength = pointerIndicator.Height = pointerIndicator.Width = e.NewSize.Width * 2;
            this.Height = content.ActualHeight;
        }

        private void overlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }

        private void overlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //if (IsRevealEnabled)
            //    pointerIndicator.Opacity = 1;

            overlay.Fill = new SolidColorBrush(Color.FromArgb(50, 128, 128, 128));
        }

        private void overlay_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //if (IsRevealEnabled)
            //    pointerIndicator.Opacity = 1;

            overlay.Fill = new SolidColorBrush(Color.FromArgb(70, 100, 100, 100));
        }

        private void overlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            //pointerIndicator.Opacity = 0;
            overlay.Fill = new SolidColorBrush(Colors.Transparent);
        }

        private void overlay_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //pointerIndicator.Opacity = 0;
            overlay.Fill = new SolidColorBrush(Colors.Transparent);
        }

        private void overlay_PointerMoved(object sender, PointerRoutedEventArgs e)
        {

            PointerPoint point = e.GetCurrentPoint(this);
            //pointerIndicator.Margin = new Thickness(point.Position.X - (PointerIndicatorLength / 2), point.Position.Y - (PointerIndicatorLength / 2), 0, 0);
        }

        private void overlay_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            //pointerIndicator.Opacity = 0;
            overlay.Fill = new SolidColorBrush(Colors.Transparent);
        }

        private void overlay_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            //pointerIndicator.Opacity = 0;
            overlay.Fill = new SolidColorBrush(Colors.Transparent);
        }

    }
}
