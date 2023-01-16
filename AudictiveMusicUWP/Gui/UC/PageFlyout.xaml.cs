using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class PageFlyout : UserControl
    {
        public delegate void RoutedEventArgs(object sender, EventArgs e);

        public event RoutedEventArgs Closed;
        public event RoutedEventArgs Opened;

        private bool CursorEnteredMegaFlyout;

        public bool IsPageFlyoutOpened
        {
            get;
            private set;
        }

        public PageFlyout()
        {
            IsPageFlyoutOpened = false;
            this.InitializeComponent();
        }

        public void Show(Type targetPage, object parameter, bool fromTouchGesture = false)
        {
            CursorEnteredMegaFlyout = false;
            ApplicationData.Current.LocalSettings.Values["UseTransition"] = false;

            megaFlyoutFrame.Navigate(targetPage, parameter);
            megaFlyoutFrame.BackStack.Clear();
            megaFlyoutBackButton.Visibility = Visibility.Collapsed;

            if (fromTouchGesture)
            {
                Storyboard sb = this.Resources["ShowMegaFlyoutAnimation_FromTouchGesture"] as Storyboard;
                sb.Begin();
            }
            else
            {
                Storyboard sb = this.Resources["ShowMegaFlyoutAnimation"] as Storyboard;
                sb.Begin();
            }
            IsPageFlyoutOpened = true;
            Opened?.Invoke(this, EventArgs.Empty);
        }

        public void Hide()
        {
            Closed?.Invoke(this, EventArgs.Empty);
            CursorEnteredMegaFlyout = false;
            Storyboard sb = this.Resources["HideMegaFlyoutAnimation"] as Storyboard;
            sb.Begin();
        }

        private void megaFlyoutOverlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Hide();
        }

        private void megaFlyoutOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (CursorEnteredMegaFlyout)
            {
                Hide();
            }
        }

        private void megaFlyoutCloseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }

        private void megaFlyout_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CursorEnteredMegaFlyout = true;
        }

        private void megaFlyoutFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (megaFlyoutFrame.CanGoBack)
                megaFlyoutBackButton.Visibility = Visibility.Visible;
            else
                megaFlyoutBackButton.Visibility = Visibility.Collapsed;
        }

        private void megaFlyoutBackButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            megaFlyoutFrame.GoBack();
        }
    }
}
