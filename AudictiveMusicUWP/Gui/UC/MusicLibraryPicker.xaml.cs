using AudictiveMusicUWP.Gui.Util;
using ClassLibrary.Control;
using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class MusicLibraryPicker : UserControl
    {
        private Compositor _compositor;
        private SpriteVisual blurSprite;
        private CompositionEffectBrush _brush;
        private List<StorageFolder> listOfFolders;

        public MusicLibraryPicker()
        {
            this.SizeChanged += MusicLibraryPicker_SizeChanged;
            this.Loaded += MusicLibraryPicker_Loaded;
            this.InitializeComponent();

            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void MusicLibraryPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (blurSprite != null)
            {
                blurSprite.Size = e.NewSize.ToVector2();
            }
        }

        private void MusicLibraryPicker_Loaded(object sender, RoutedEventArgs e)
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

            blurSprite = _compositor.CreateSpriteVisual();
            blurSprite.Size = new Vector2((float)this.ActualWidth, (float)ApplicationInfo.Current.WindowSize.Height);
            blurSprite.Brush = _brush;

            ElementCompositionPreview.SetElementChildVisual(blur, blurSprite);
        }

        public async void LoadFolders()
        {
            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            listOfFolders = musicLibrary.Folders.ToList();

            foldersList.ItemsSource = listOfFolders;

            if (listOfFolders.Count == 0)
            {
                ChooseFoldersNothingToDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                ChooseFoldersNothingToDisplay.Visibility = Visibility.Collapsed;
            }
        }

        private void blur_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (PageHelper.MainPage != null)
            {
                PageHelper.MainPage.RemovePicker();
            }
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageHelper.MainPage != null)
            {
                PageHelper.MainPage.RemovePicker();
            }
        }

        private async void removeButton_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            StorageFolder folder = element.DataContext as StorageFolder;

            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            bool result = await musicLibrary.RequestRemoveFolderAsync(folder);

            if (result)
            {
                LoadFolders();
                Collection.LoadCollectionChanges();
            }
        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            StorageFolder result = await musicLibrary.RequestAddFolderAsync();

            if (result != null)
            {
                LoadFolders();
                Collection.LoadCollectionChanges();
            }
        }

        private void foldersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
