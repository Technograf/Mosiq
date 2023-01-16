using ClassLibrary.Helpers;
using ClassLibrary.Themes;
using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class ThemeSelector : Page
    {
        private NavigationMode NavMode
        {
            get;
            set;
        }

        public ThemeSelector()
        {
            this.SizeChanged += ThemeSelector_SizeChanged;
            this.InitializeComponent();
        }

        private void ThemeSelector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            themesList.Width = e.NewSize.Width;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            pageTitle.Text = ApplicationInfo.Current.Resources.GetString("Themes");
            this.NavMode = e.NavigationMode;

            LoadThemes();
            LoadThemeSettings(ApplicationSettings.NowPlayingTheme);

            backgroundPreference.SelectedIndex = ApplicationSettings.ThemeBackgroundPreference;
            colorPreference.SelectedIndex = ApplicationSettings.ThemeColorPreference;

            colorsList.Visibility = ApplicationSettings.ThemeColorPreference == 2 ? Visibility.Visible : Visibility.Collapsed;


            backgroundPreference.SelectionChanged += BackgroundPreference_SelectionChanged;
            colorPreference.SelectionChanged += ColorPreference_SelectionChanged;
            OpenPage(NavMode == NavigationMode.Back);
        }

        private void BackgroundPreference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.ThemeBackgroundPreference = backgroundPreference.SelectedIndex;
        }

        private void ColorPreference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationSettings.ThemeColorPreference = colorPreference.SelectedIndex;

            if (ApplicationSettings.ThemeColorPreference == 0)
            {
                ApplicationSettings.CurrentThemeColor = ImageHelper.GetColorFromHex(ApplicationSettings.CurrentSong.HexColor);
            }
            else if (ApplicationSettings.ThemeColorPreference == 1)
            {
                ApplicationSettings.CurrentThemeColor = ApplicationInfo.Current.CurrentSystemAccentColor;
            }
            else if (ApplicationSettings.ThemeColorPreference == 2)
            {
                ApplicationSettings.CurrentThemeColor = ApplicationSettings.CustomThemeColor;
            }

            LoadColors();
            colorsList.Visibility = ApplicationSettings.ThemeColorPreference == 2 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadThemes()
        {
            List<ThemeBase> themes = new List<ThemeBase>()
            {
                new ModernTheme(), new BlurTheme(), new MaterialTheme() /*, new NeonTheme(), new MaterialTheme()*/
            };

            themesList.ItemsSource = themes;

            LoadColors();
        }

        private void LoadColors()
        {
            List<ThemeColor> brushes = new List<ThemeColor>();

            foreach (var color in typeof(Colors).GetRuntimeProperties())
            {
                brushes.Add(new ThemeColor() { Color = (Color)color.GetValue(null) });
            }

            colorsList.ItemsSource = brushes;
        }

        private void OpenPage(bool reload)
        {
            try
            {
                progress.IsActive = false;
                Storyboard sb = this.Resources["OpenPageTransition"] as Storyboard;

                if (reload)
                {
                    layoutRootScale.ScaleX = layoutRootScale.ScaleY = 1.1;
                }

                sb.Begin();
            }
            catch
            {

            }
        }

        private void LoadThemeSettings(Theme theme)
        {
            switch (theme)
            {
                case Theme.Blur:

                    blurOptions.Visibility = Visibility.Visible;

                    cleanOptions.Visibility = Visibility.Collapsed;
                    materialOptions.Visibility = Visibility.Collapsed;
                    modernOptions.Visibility = Visibility.Collapsed;
                    neonOptions.Visibility = Visibility.Collapsed;


                    ThemeBlurAmount.Value = Convert.ToDouble(ApplicationSettings.NowPlayingBlurAmount);

                    ThemeBlurAmount.ValueChanged -= ThemeBlurAmount_ValueChanged;
                    ThemeBlurAmount.ValueChanged += ThemeBlurAmount_ValueChanged;

                    themeColorOptions.Visibility = Visibility.Visible;

                    break;

                case Theme.Clean:

                    cleanOptions.Visibility = Visibility.Visible;

                    blurOptions.Visibility = Visibility.Collapsed;
                    materialOptions.Visibility = Visibility.Collapsed;
                    modernOptions.Visibility = Visibility.Collapsed;
                    neonOptions.Visibility = Visibility.Collapsed;

                    themeColorOptions.Visibility = Visibility.Visible;

                    break;

                case Theme.Material:

                    materialOptions.Visibility = Visibility.Visible;

                    blurOptions.Visibility = Visibility.Collapsed;
                    cleanOptions.Visibility = Visibility.Collapsed;
                    modernOptions.Visibility = Visibility.Collapsed;
                    neonOptions.Visibility = Visibility.Collapsed;

                    themeColorOptions.Visibility = Visibility.Visible;
                    //ApplicationSettings.CustomThemeColor = ImageHelper.GetColorFromHex("#FFDC572E");
                    //ApplicationSettings.ThemeColorPreference = 2;

                    //themeColorOptions.Visibility = Visibility.Collapsed;

                    break;

                case Theme.Modern:

                    modernOptions.Visibility = Visibility.Visible;

                    blurOptions.Visibility = Visibility.Collapsed;
                    cleanOptions.Visibility = Visibility.Collapsed;
                    materialOptions.Visibility = Visibility.Collapsed;
                    neonOptions.Visibility = Visibility.Collapsed;

                    themeColorOptions.Visibility = Visibility.Visible;

                    break;

                case Theme.Neon:

                    neonOptions.Visibility = Visibility.Visible;

                    blurOptions.Visibility = Visibility.Collapsed;
                    cleanOptions.Visibility = Visibility.Collapsed;
                    materialOptions.Visibility = Visibility.Collapsed;
                    modernOptions.Visibility = Visibility.Collapsed;

                    themeColorOptions.Visibility = Visibility.Visible;

                    break;

            }


        }

        private void ThemeBlurAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ApplicationSettings.NowPlayingBlurAmount = (float)e.NewValue;
        }

        private void themesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var theme = e.ClickedItem as ThemeBase;
            List<ThemeBase> list = new List<ThemeBase>();

            foreach (ThemeBase t in themesList.Items)
            {
                if (t.Name == theme.Name)
                    t.IsSelected = true;
                else
                    t.IsSelected = false;

                list.Add(t);
            }

            themesList.ItemsSource = list;

            LoadThemeSettings(theme.Theme);
        }

        private void colorsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var color = e.ClickedItem as ThemeColor;
            color.IsSelected = true;

            ApplicationSettings.CurrentThemeColor = color.Color;

            LoadColors();
        }
    }
}
