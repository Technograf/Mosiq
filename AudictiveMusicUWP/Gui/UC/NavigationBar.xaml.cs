using ClassLibrary.Helpers;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AudictiveMusicUWP.Gui.UC;
using AudictiveMusicUWP.Gui.Util;
using BackgroundAudioShared.Messages;
using ClassLibrary.Control;
using ClassLibrary.Entities;

using IF.Lastfm.Core.Objects;
using NotificationsVisualizerLibrary;

using Windows.UI.Popups;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class NavigationBar : UserControl
    {
        public delegate void ActionRequestedEventArgs(NavigationView target);

        public event ActionRequestedEventArgs ActionRequested;

        public enum NavigationView
        {
            Collection,
            Home,
            Playlists,
            Cloud,
            Search,
            Unknown
        }

        public NavigationView CurrentView
        {
            get; private set;
        }

        public Orientation Orientation
        {
            get { return ((Orientation)GetValue(OrientationProperty)); }
            set
            {
                SetValue(OrientationProperty, value);
                UpdateView();
            }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(NavigationBar), new PropertyMetadata(Orientation.Horizontal));


        public Color Tint
        {
            get { return ((Color)GetValue(TintProperty)); }
            set
            {
                SetValue(TintProperty, value);
            }
        }

        public static readonly DependencyProperty TintProperty =
            DependencyProperty.Register("Tint", typeof(Color), typeof(NavigationBar), new PropertyMetadata(Colors.Transparent));

        public bool IsBlurEnabled
        {
            get { return ((bool)GetValue(IsBlurEnabledProperty)); }
            set
            {
                SetValue(IsBlurEnabledProperty, value);

                SetUpFluentDesign(value);
            }
        }

        public static readonly DependencyProperty IsBlurEnabledProperty =
            DependencyProperty.Register("IsBlurEnabled", typeof(bool), typeof(NavigationBar), new PropertyMetadata(false));

        private CompositionEffectBrush _brush;
        private Compositor _compositor;
        private SpriteVisual backgroundSprite;

        public NavigationBar()
        {
            this.CurrentView = NavigationView.Unknown;
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            this.InitializeComponent();
        }

        public void SyncNavigationState(NavigationView currentView)
        {
            this.CurrentView = currentView;

            foreach (ToggleButton e in buttonsContainer.Children)
            {
                e.IsChecked = false;
                e.IsHitTestVisible = true;
            }

            if (this.CurrentView == NavigationView.Home)
            {
                (buttonsContainer.Children[0] as ToggleButton).IsChecked = true;
                (buttonsContainer.Children[0] as ToggleButton).IsHitTestVisible = false;
            }
            else if (this.CurrentView == NavigationView.Collection)
            {
                (buttonsContainer.Children[1] as ToggleButton).IsChecked = true;
                (buttonsContainer.Children[1] as ToggleButton).IsHitTestVisible = false;
            }
            else if (this.CurrentView == NavigationView.Playlists)
            {
                (buttonsContainer.Children[2] as ToggleButton).IsChecked = true;
                (buttonsContainer.Children[2] as ToggleButton).IsHitTestVisible = false;
            }
            else if (this.CurrentView == NavigationView.Cloud)
            {
                (buttonsContainer.Children[3] as ToggleButton).IsChecked = true;
                (buttonsContainer.Children[3] as ToggleButton).IsHitTestVisible = false;
            }
            
        }

        private void UpdateView()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                for (int i = 0; i < buttonsContainer.Children.Count; i++)
                {
                    ((ToggleButton)buttonsContainer.Children[i]).Style = this.Resources["HorizontalNavigationButtonStyle"] as Style;
                    ((ToggleButton)buttonsContainer.Children[i]).MinHeight = 0;
                    Grid.SetColumnSpan((ToggleButton)buttonsContainer.Children[i], 1);
                    Grid.SetColumn((ToggleButton)buttonsContainer.Children[i], i);
                    Grid.SetRowSpan((ToggleButton)buttonsContainer.Children[i], 7);
                    Grid.SetRow((ToggleButton)buttonsContainer.Children[i], 0);

                }
            }
            else
            {
                for (int i = 0; i < buttonsContainer.Children.Count; i++)
                {
                    ((ToggleButton)buttonsContainer.Children[i]).Style = this.Resources["VerticalNavigationButtonStyle"] as Style;
                    ((ToggleButton)buttonsContainer.Children[i]).MinHeight = 50;
                    Grid.SetColumnSpan((ToggleButton)buttonsContainer.Children[i], 5);
                    Grid.SetColumn((ToggleButton)buttonsContainer.Children[i], 0);
                    Grid.SetRowSpan((ToggleButton)buttonsContainer.Children[i], 1);
                    Grid.SetRow((ToggleButton)buttonsContainer.Children[i], i + 1);

                }
            }
        }

        private void SetUpFluentDesign(bool value)
        {
            blurContainer.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

            if (value && ApplicationInfo.Current.GetDeviceFormFactorType() != ApplicationInfo.DeviceFormFactorType.Phone)
            {
                backgroundSprite = _compositor.CreateSpriteVisual();

                backgroundSprite.Size = new Vector2((float)this.ActualWidth, (float)this.ActualHeight);

                backgroundSprite.Brush = _compositor.CreateHostBackdropBrush();

                ElementCompositionPreview.SetElementChildVisual(blurGrid, backgroundSprite);
            }
        }

        private void StartMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ActionRequested?.Invoke(NavigationView.Home);
        }

        private void CollectionMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ActionRequested?.Invoke(NavigationView.Collection);
        }

        private  void PlaylistsMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ActionRequested?.Invoke(NavigationView.Playlists);
        }
         private  void CloudMenuButton_Click(object sender, RoutedEventArgs e)
        {
            ActionRequested?.Invoke(NavigationView.Cloud);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            ActionRequested?.Invoke(NavigationView.Search);
            searchButton.IsChecked = false;
        }

        private void blurContainer_Loaded(object sender, RoutedEventArgs e)
        {
            SetUpFluentDesign(this.IsBlurEnabled);
        }

        private void blurContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (backgroundSprite != null)
            {
                backgroundSprite.Size = e.NewSize.ToVector2();
            }
        }
    }
}
