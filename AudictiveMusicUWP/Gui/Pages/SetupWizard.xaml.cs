using AudictiveMusicUWP.Gui.Util;
using ClassLibrary.Control;
using ClassLibrary.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class SetupWizard : Page
    {
        private bool IsLoadingCompleted
        {
            get;
            set;
        }

        public SetupWizard()
        {
            IsLoadingCompleted = false;
            this.SizeChanged += SetupWizard_SizeChanged;
            this.Loaded += SetupWizard_Loaded;

            Collection.ProgressChanged += Ctr_Collection_ProgressChanged;

            this.InitializeComponent();
        }

        private async void Ctr_Collection_ProgressChanged(int progress, bool isLoading)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                UpdateStatus(progress);

                if (isLoading == false)
                    LoadingCompleted();
            });
        }

        private void SetupWizard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Panel1.Width = Panel2.Width = Panel3.Width = e.NewSize.Width;
        }

        private void SetupWizard_Loaded(object sender, RoutedEventArgs e)
        {
            flipview.SelectionChanged += flipview_SelectionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.SetupWizard = null;
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.SetupWizard = this;

            appName.Text = ApplicationInfo.Current.AppPackageName.ToUpper();
            VersionNumber.Text = ApplicationInfo.Current.AppVersion;

            LoadSettings();

            if (ApplicationSettings.IsCollectionLoaded == false)
            {
                IsLoadingCompleted = false;
                progressIndeterminate.Visibility = Visibility.Visible;
                await DeleteExistingAlbumCovers();

                await Collection.CheckMusicCollection();
            }
            else
            {
                IsLoadingCompleted = true;
                UpdateStatus(100);
                LoadingCompleted();
            }
        }

        private async Task DeleteExistingAlbumCovers()
        {
            try
            {
                IStorageItem coversFolderItem = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Covers");
                if (coversFolderItem != null)
                {
                    StorageFolder coversFolder = coversFolderItem as StorageFolder;
                    var covers = await coversFolder.GetFilesAsync();
                    foreach (StorageFile file in covers)
                    {
                        try
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }
                        catch
                        {

                        }
                    }
                }
            }
            catch
            {

            }
        }


        private void LoadSettings()
        {
            //ThemeComboBox.SelectionChanged += ThemeComboBox_SelectionChanged;
            darkRadioButton.Checked += DarkRadioButton_Checked;
            lightRadioButton.Checked += LightRadioButton_Checked;
            LockScreenToggleSwitch.Toggled += LockScreenToggleSwitch_Toggled;
            celullarDownloadToggleSwitch.Toggled += CelullarDownloadToggleSwitch_Toggled;

            if (ApplicationInfo.Current.GetDeviceFormFactorType() != ApplicationInfo.DeviceFormFactorType.Phone)
            {
                LockScreenToggleSwitch.Visibility = Visibility.Collapsed;
            }

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AppTheme"))
            {
                int theme = (int)ApplicationData.Current.LocalSettings.Values["AppTheme"];

                if (theme == 0)
                {
                    darkRadioButton.IsChecked = true;
                }
                else if (theme == 1)
                {
                    lightRadioButton.IsChecked = true;
                }
                else if (theme == 2)
                {
                    darkRadioButton.IsChecked = true;
                }
            }
            else
            {
                darkRadioButton.IsChecked = true;
            }

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("AppTheme"))
            //{
            //    ThemeComboBox.SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["AppTheme"];
            //}
            //else
            //{
            //    ThemeComboBox.SelectedIndex = 2;
            //}

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UpdateTitleBarColor"))
            //{
            //    TitleBarColorCheckBox.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["UpdateTitleBarColor"];
            //}
            //else
            //{
            //    ApplicationData.Current.LocalSettings.Values["UpdateTitleBarColor"] = TitleBarColorCheckBox.IsChecked = true;
            //}

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UpdatePlayBackBarColor"))
            //{
            //    PlaybackBarColorCheckBox.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["UpdatePlayBackBarColor"];
            //}
            //else
            //{
            //    ApplicationData.Current.LocalSettings.Values["UpdatePlayBackBarColor"] = PlaybackBarColorCheckBox.IsChecked = true;
            //}

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("UpdateMenuColor"))
            //{
            //    MenuColorCheckBox.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"];
            //}
            //else
            //{
            //    ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"] = MenuColorCheckBox.IsChecked = true;
            //}

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Lockscreen"))
            {
                LockScreenToggleSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values["Lockscreen"];
                //UseBlurImageCheckBox.IsEnabled = (bool)ApplicationData.Current.LocalSettings.Values["Lockscreen"];
            }
            else
            {
                //UseBlurImageCheckBox.IsEnabled = false;
            }

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("EnableTransparency"))
            //{
            //    transparentEffectsToggleSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values["EnableTransparency"];
            //}
            //else
            //{
            //    ApplicationData.Current.LocalSettings.Values["EnableTransparency"] = transparentEffectsToggleSwitch.IsOn = true;
            //}

            //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("LockscreenBlur"))
            //{
            //    UseBlurImageCheckBox.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["LockscreenBlur"];
            //}
            //else
            //{
            //    UseBlurImageCheckBox.IsChecked = true;
            //    ApplicationData.Current.LocalSettings.Values["LockscreenBlur"] = true;
            //}

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Download"))
            {
                sendInfoYes.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["Download"];
                sendInfoNo.IsChecked = (bool)ApplicationData.Current.LocalSettings.Values["Download"] == false;

                if (sendInfoYes.IsChecked == true)
                    celullarDownloadToggleSwitch.IsEnabled = true;
                else
                    celullarDownloadToggleSwitch.IsEnabled = false;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["Download"] = false;
                sendInfoYes.IsChecked = false;
                sendInfoNo.IsChecked = true;

                celullarDownloadToggleSwitch.IsEnabled = false;
            }

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CelullarDownload"))
            {
                celullarDownloadToggleSwitch.IsOn = (bool)ApplicationData.Current.LocalSettings.Values["CelullarDownload"];
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["CelullarDownload"] = celullarDownloadToggleSwitch.IsOn = true;
            }
        }

        #region EVENTS

        private void LightRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["AppTheme"] = 1;
            this.RequestedTheme = ElementTheme.Light;
        }

        private void DarkRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["AppTheme"] = 0;
            this.RequestedTheme = ElementTheme.Dark;
        }


        private void CelullarDownloadToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["CelullarDownload"] = celullarDownloadToggleSwitch.IsOn;
        }

        //private void TransparentEffectsToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        //{
        //    ApplicationData.Current.LocalSettings.Values["EnableTransparency"] = transparentEffectsToggleSwitch.IsOn;
        //}

        private void PlaybackBarColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdatePlayBackBarColor"] = true;
        }
        private void PlaybackBarColorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdatePlayBackBarColor"] = false;
        }

        private void LockScreenToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["Lockscreen"] = LockScreenToggleSwitch.IsOn;
            //UseBlurImageCheckBox.IsEnabled = LockScreenToggleSwitch.IsOn;

        }

        //private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ApplicationData.Current.LocalSettings.Values["AppTheme"] = ThemeComboBox.SelectedIndex;

        //    if (ThemeComboBox.SelectedIndex == 0)
        //    {
        //        this.RequestedTheme = ElementTheme.Dark;
        //    }
        //    else if (ThemeComboBox.SelectedIndex == 1)
        //    {
        //        this.RequestedTheme = ElementTheme.Light;
        //    }
        //    else if (ThemeComboBox.SelectedIndex == 2)
        //    {
        //        this.RequestedTheme = ElementTheme.Default;
        //    }
        //}

        private void UseBlurImageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["LockscreenBlur"] = false;
        }

        private void UseBlurImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["LockscreenBlur"] = true;
        }

        private void TitleBarColorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdateTitleBarColor"] = false;
        }

        private void TitleBarColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdateTitleBarColor"] = true;
        }

        private void MenuColorCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"] = false;
        }

        private void MenuColorCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["UpdateMenuColor"] = true;
        }

        private void sendInfoNo_Click(object sender, RoutedEventArgs e)
        {
            if (sendInfoYes.IsChecked == true)
                sendInfoYes.IsChecked = false;
            else
                sendInfoNo.IsChecked = true;

            celullarDownloadToggleSwitch.IsEnabled = false;
            ApplicationData.Current.LocalSettings.Values["Download"] = sendInfoYes.IsChecked == true;
        }

        private void sendInfoYes_Click(object sender, RoutedEventArgs e)
        {
            if (sendInfoNo.IsChecked == true)
                sendInfoNo.IsChecked = false;
            else
                sendInfoYes.IsChecked = true;

            celullarDownloadToggleSwitch.IsEnabled = true;
            ApplicationData.Current.LocalSettings.Values["Download"] = sendInfoYes.IsChecked == true;
        }

        #endregion

        private void forwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (flipview.SelectedIndex < flipview.Items.Count - 1)
            {
                if (flipview.SelectedItem == loadingProgressFlipItem)
                {
                    if (IsLoadingCompleted)
                        flipview.SelectedIndex++;
                }
                else
                    flipview.SelectedIndex++;
            }
            else
            {
                ApplicationData.Current.LocalSettings.Values["AudictiveMusic10RTM"] = true;

                Frame.Navigate(typeof(MainPage));
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (flipview.SelectedIndex > 0)
                flipview.SelectedIndex--;
        }

        private void flipview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flipview.SelectedIndex == flipview.Items.Count - 1 || flipview.SelectedItem == loadingProgressFlipItem)
            {
                forwardButton.IsEnabled = IsLoadingCompleted;
            }
            else
            {
                forwardButton.IsEnabled = true;
            }

            if (flipview.SelectedIndex == 0)
            {
                previousButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                previousButton.Visibility = Visibility.Visible;
            }

            foreach (FrameworkElement t in selectionIndicators.Children)
            {
                if (selectionIndicators.Children.IndexOf(t) == flipview.SelectedIndex)
                    t.Opacity = 1;
                else
                    t.Opacity = 0.6;
            }
        }

        private void flipview_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)VisualTreeHelper.GetChild(flipview, 0);
            for (int i = grid.Children.Count - 1; i >= 0; i--)
                if (grid.Children[i] is Button)
                    grid.Children.RemoveAt(i);
        }


        public void UpdateStatus(int currentValue)
        {
            progress.Value = currentValue;
            progressPercentage.Text = Convert.ToString(currentValue) + "%";
        }

        public void LoadingCompleted()
        {
            IsLoadingCompleted = true;
            progressIndeterminate.Visibility = Visibility.Collapsed;

            if (flipview.SelectedItem == loadingProgressFlipItem)
                flipview.SelectedIndex++;
        }
    }
}
