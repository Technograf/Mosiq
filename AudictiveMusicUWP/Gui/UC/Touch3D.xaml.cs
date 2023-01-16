using ClassLibrary.Entities;
using ClassLibrary.Helpers;
using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class Touch3D : UserControl
    {
        public EventHandler<Touch3DEventArgs> ActionRequested;
        public delegate void VisibilityChangedEventArgs(bool isVisible);
        public event VisibilityChangedEventArgs VisibilityChanged;

        private ResourceLoader res;
        public bool IsTouch3DOpened;

        public Thickness IconsPosition
        {
            get
            {
                return (Thickness)GetValue(IconsPositionProperty);
            }
            set
            {
                SetValue(IconsPositionProperty, value);

                pointerArea.Margin = new Thickness(value.Left, 0, 0, 0);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconsPositionProperty =
            DependencyProperty.Register("IconsPosition", typeof(Thickness), typeof(Touch3D), new PropertyMetadata(new Thickness()));




        private Song CurrentSong
        {
            get
            {
                //return itemLenght;
                return (Song)GetValue(CurrentSongProperty);
            }
            set
            {
                //itemLenght = value;
                SetValue(CurrentSongProperty, value);
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumItemLenght"));
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty CurrentSongProperty =
            DependencyProperty.Register("CurrentSong", typeof(Song), typeof(Touch3D), new PropertyMetadata(new Song()));

        public enum Mode
        {
            Album,
            NowPlaying
        }

        private Mode mode;

        public Mode DisplayMode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                if (value == Mode.Album)
                {

                }
                else if (value == Mode.NowPlaying)
                {

                }
            }
        }

        public Touch3D()
        {
            this.InitializeComponent();

            IsTouch3DOpened = false;
            res = new ResourceLoader();
        }

        public void Set(Mode displayMode, Song song)
        {
            this.DisplayMode = displayMode;
            this.CurrentSong = song;
            this.IsHitTestVisible = false;
        }

        public void Show()
        {
            love.PointerExited += Love_PointerExited;
            share.PointerExited += Share_PointerExited;
            artist.PointerExited += Artist_PointerExited;
            add.PointerExited += Add_PointerExited;

            //item1.PointerExited += item1_PointerExited;
            //item2.PointerExited += item2_PointerExited;
            //item3.PointerExited += item3_PointerExited;

            this.IsTouch3DOpened = true;
            Storyboard sb = this.Resources["ShowAnimation"] as Storyboard;
            sb.Begin();
        }

        private void Love_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(0);
        }

        private void Share_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(1);
        }

        private void Artist_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(2);
        }

        private void Add_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(3);
        }

        public void Hide()
        {
            love.PointerExited -= Love_PointerExited;
            share.PointerExited -= Share_PointerExited;
            artist.PointerExited -= Artist_PointerExited;
            add.PointerExited -= Add_PointerExited;

            actionName.Text = "";
            this.IsTouch3DOpened = false;
            this.IsHitTestVisible = false;

            Storyboard sb = this.Resources["HideAnimation"] as Storyboard;
            sb.Begin();
        }

        private void RemoveFocus(int v)
        {
            actionName.Text = "";

            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(150),
                EnableDependentAnimation = false,
            };

            

            DoubleAnimation da1 = new DoubleAnimation()
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(150),
                EnableDependentAnimation = false,
            };

            if (v == 0)
            {
                Storyboard.SetTarget(da, loveScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, loveScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 1)
            {
                Storyboard.SetTarget(da, shareScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, shareScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 2)
            {
                Storyboard.SetTarget(da, artistScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, artistScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 3)
            {
                Storyboard.SetTarget(da, addScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, addScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else
            {
                return;
            }

            sb.Children.Add(da);
            sb.Children.Add(da1);
            sb.Begin();
        }


        private void item2_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddFocus(2);
            
        }

        private void AddFocus(int v)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                To = 1.28,
                Duration = TimeSpan.FromMilliseconds(150),
                EnableDependentAnimation = false,
            };

            DoubleAnimation da1 = new DoubleAnimation()
            {
                To = 1.28,
                Duration = TimeSpan.FromMilliseconds(150),
                EnableDependentAnimation = false,
            };

            if (v == 0)
            {
                if (this.CurrentSong.IsFavorite)
                {
                    actionName.Text = ApplicationInfo.Current.Resources.GetString("RemoveFromFavoritesString");
                }
                else
                {
                    actionName.Text = ApplicationInfo.Current.Resources.GetString("AddToFavoritesString");
                }

                Storyboard.SetTarget(da, loveScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, loveScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 1)
            {
                actionName.Text = ApplicationInfo.Current.Resources.GetString("Share");

                Storyboard.SetTarget(da, shareScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, shareScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 2)
            {
                actionName.Text = ApplicationInfo.Current.Resources.GetString("Open") + " " + this.CurrentSong.Artist + "";

                Storyboard.SetTarget(da, artistScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, artistScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else if (v == 3)
            {
                actionName.Text = ApplicationInfo.Current.Resources.GetString("AddToPlaylistFile");

                Storyboard.SetTarget(da, addScale);
                Storyboard.SetTargetProperty(da, "ScaleX");

                Storyboard.SetTarget(da1, addScale);
                Storyboard.SetTargetProperty(da1, "ScaleY");
            }
            else
            {
                actionName.Text = "";

                return;
            }

            sb.Children.Add(da);
            sb.Children.Add(da1);
            sb.Begin();

            //ApplicationInfo.Current.VibrateDevice(15);
        }

        private void showAnimation_Completed(object sender, object e)
        {
            if (IsTouch3DOpened)
                this.IsHitTestVisible = true;
        }

        private void background_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Hide();
        }

        private void love_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddFocus(0);
        }

        private void love_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(0);

            Hide();

            ActionRequested?.Invoke(this, new Touch3DEventArgs(Touch3DEventArgs.Type.LikeSong));
        }

        private void share_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddFocus(1);
        }

        private void share_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(1);

            Hide();
            ActionRequested?.Invoke(this, new Touch3DEventArgs(Touch3DEventArgs.Type.ShareSong));
        }

        private void artist_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddFocus(2);
        }

        private void artist_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(2);

            string artist;

            if (this.CurrentSong != null)
                artist = this.CurrentSong.Artist;
            else
                return;

            Hide();

            ActionRequested?.Invoke(this, new Touch3DEventArgs(artist, Touch3DEventArgs.Type.OpenArtist));
        }

        private void add_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            AddFocus(3);
        }

        private void add_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            RemoveFocus(3);

            Hide();

            ActionRequested?.Invoke(this, new Touch3DEventArgs(Touch3DEventArgs.Type.AddSong));
        }

        private void background_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
                Hide();
        }

        private void hideAnimation_Completed(object sender, object e)
        {
            VisibilityChanged?.Invoke(false);
        }
    }

    public sealed class Touch3DEventArgs : RoutedEventArgs
    {
        public enum Type
        {
            LikeSong,
            ShareSong,
            OpenArtist,
            AddSong,
        }

        public Type Action
        {
            get;
            set;
        }

        public string Argument
        {
            get;
            set;
        }

        public Touch3DEventArgs(string argument, Type type)
        {
            this.Argument = argument;
            this.Action = type;
        }

        public Touch3DEventArgs(Type action)
        {
            this.Action = action;
        }
    }
}
