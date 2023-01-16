using AudictiveMusicUWP.Gui.Util;
using ClassLibrary.Control;
using ClassLibrary.Helpers;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Navigation;

// O modelo de item de Página em Branco está documentado em https://go.microsoft.com/fwlink/?LinkId=234238

namespace AudictiveMusicUWP.Gui.Pages
{
    /// <summary>
    /// Uma página vazia que pode ser usada isoladamente ou navegada dentro de um Quadro.
    /// </summary>
    public sealed partial class PreparingCollection : Page
    {
        private Compositor _compositor;
        private SpriteVisual _hostSprite;

        public PreparingCollection()
        {
            this.Loaded += PreparingCollection_Loaded;
            this.SizeChanged += PreparingCollection_SizeChanged;

            Collection.ProgressChanged += Ctr_Collection_ProgressChanged;
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
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

        private void PreparingCollection_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_hostSprite != null)
            {
                _hostSprite.Size = e.NewSize.ToVector2();
            }
        }

        private void PreparingCollection_Loaded(object sender, RoutedEventArgs e)
        {
            if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
            {
                _hostSprite = _compositor.CreateSpriteVisual();
                _hostSprite.Size = new Vector2((float)acrillic.ActualWidth, (float)acrillic.ActualHeight);

                _hostSprite.Brush = _compositor.CreateHostBackdropBrush();

                ElementCompositionPreview.SetElementChildVisual(acrillic, _hostSprite);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PageHelper.PreparingCollection = null;
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string args = string.Empty;
            if (e.Parameter != null)
            {
                args = e.Parameter.ToString();
            }
            PageHelper.PreparingCollection = this;
            if (string.IsNullOrWhiteSpace(args) == false)
            {
                if (NavigationHelper.GetParameter(args, "action") == "resizeImages")
                {
                    StorageFolder artistsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);
                    //StorageFolder albumsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);

                    var artImages = await artistsFolder.GetFilesAsync();
                    //var albImages = await albumsFolder.GetFilesAsync();

                    foreach (StorageFile file in artImages)
                    {
                        ImageHelper.ResizeImage(file, 450);
                    }

                    LoadingCompleted();
                    return;
                }
            }
            await DeleteExistingAlbumCovers();

            //Está retornando apenas 67 MÚSICAS!!!!! Verificar o que há de errado
            await Collection.CheckMusicCollection();
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

        public void UpdateStatus(int currentValue)
        {
            progress.Value = currentValue;
            progressPercentage.Text = Convert.ToString(currentValue) + "%";
        }

        public void LoadingCompleted()
        {
            ApplicationData.Current.LocalSettings.Values["FreeSpaceArtistsImages"] = false;
            ApplicationData.Current.LocalSettings.Values["FreeSpaceCoversImages"] = false;
            Frame.Navigate(typeof(MainPage));
            Frame.BackStack.Clear();
        }
    }
}
