using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary;
using ClassLibrary.Helpers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CloudPage : Page
    {
        private NavigationMode NavMode
        {
            get;
            set;
        }
        public CloudPage()
        {
            this.Loaded += CloudPage_Loaded;
            this.SizeChanged += CloudPage_SizeChanged;
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private void CloudPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 400)
            {
                navigationButtons.HorizontalAlignment = HorizontalAlignment.Left;
                songsButton.FontSize = searchButton.FontSize = songsButton.FontSize = 14;
            }
            else
            {
                navigationButtons.HorizontalAlignment = HorizontalAlignment.Center;
                songsButton.FontSize = searchButton.FontSize = songsButton.FontSize = 16;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.NavigationMode == NavigationMode.Refresh)
                return;

            progress.IsActive = true;
            Storyboard sb = this.Resources["ExitPageTransition"] as Storyboard;
            sb.Begin();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavMode = e.NavigationMode;

            string arguments = string.Empty;

            if (e.Parameter != null)
                arguments = e.Parameter.ToString();

            if (string.IsNullOrWhiteSpace(arguments) == false)
            {
                if (NavigationHelper.ContainsAttribute(arguments, "page"))
                {
                    if (NavigationHelper.GetParameter(arguments, "page") == "playlists")
                    {
                        frame.Navigate(typeof(ClPlaylistPage));
                    }
                    else if (NavigationHelper.GetParameter(arguments, "page") == "search")
                    {
                        frame.Navigate(typeof(ClSearchPage));
                    }
                    else
                    {
                        //OTHER ACTIONS
                        frame.Navigate(typeof(ClPlaylistPage));
                    }
                }
                else
                {
                    frame.Navigate(typeof(ClPlaylistPage));
                }
            }
            else
            {
                frame.Navigate(typeof(ClPlaylistPage));
            }

            if (NavMode == NavigationMode.Refresh)
                return;

            OpenPage(NavMode == NavigationMode.Back);
        }

        private void OpenPage(bool reload)
        {
            progress.IsActive = false;
            Storyboard sb = this.Resources["OpenPageTransition"] as Storyboard;

            if (reload)
            {
                layoutRootScale.ScaleX = layoutRootScale.ScaleY = 1.1;
            }

            sb.Begin();
        }

        private void CloudPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(CloudPage), "page=search");
        }

        private void songsButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(CloudPage), "page=playlist");
        }


        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(Albums))
            {
                songsButton.IsChecked = true;
            }
            else if (e.SourcePageType == typeof(Songs))
            {
                songsButton.IsChecked = true;
            }
        }

        private void songsButton_Checked(object sender, RoutedEventArgs e)
        {
            searchButton.IsChecked = false;
        }

        private void searchButton_Checked(object sender, RoutedEventArgs e)
        {
            songsButton.IsChecked = false;
        }

        private void accButton_Click(object sender, RoutedEventArgs e)
        {
            PageHelper.MainPage.Navigate(typeof(AccountPage), null);
        }
    }
}

