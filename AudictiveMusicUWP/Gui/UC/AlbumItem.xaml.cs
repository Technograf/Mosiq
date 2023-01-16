using AudictiveMusicUWP.Gui.Util;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class AlbumItem : UserControl
    {
        private bool ClickEventIsTouch;
        private int RepeatButtonCycleCounter;

        public delegate void LongHoverEventArgs(object sender, object context);
        public delegate void MenuTriggeredEventArgs(object sender, Point point);

        public event MenuTriggeredEventArgs MenuTriggered;
        public event LongHoverEventArgs LongHover;
        public AlbumItem()
        {
            ClickEventIsTouch = false;
            RepeatButtonCycleCounter = 0;
            this.InitializeComponent();
        }

        public double ItemLength
        {
            get
            {
                return (double)GetValue(AlbumItemLengthProperty);
            }
            set
            {
                SetValue(AlbumItemLengthProperty, value);
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumItemLenght"));
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumItemLengthProperty =
            DependencyProperty.Register("ItemLength", typeof(double), typeof(AlbumItem), new PropertyMetadata(100));

        public Color AlbumColor
        {
            get
            {
                return (Color)GetValue(AlbumColorProperty);
            }
            set
            {
                SetValue(AlbumColorProperty, value);
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AlbumItemLenght"));
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumColorProperty =
            DependencyProperty.Register("AlbumColor", typeof(double), typeof(AlbumItem), new PropertyMetadata(Colors.Gray));



        private void repeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClickEventIsTouch)
            {
                RepeatButtonCycleCounter = 0;
                return;
            }

            if (RepeatButtonCycleCounter == 1 && AlbumItemHelper.PointerIsInContact)
            {
                LongHover?.Invoke(this, this.DataContext);
                //pageFlyout.Show(typeof(AlbumPage), SelectedAlbum, false);
            }
            else if (RepeatButtonCycleCounter > 1)
                RepeatButtonCycleCounter = 0;

            RepeatButtonCycleCounter++;
        }

        private void repeatButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ClickEventIsTouch = e.Pointer.PointerDeviceType == PointerDeviceType.Touch;
            AlbumItemHelper.PointerIsInContact = e.Pointer.IsInRange;

            Storyboard sb = this.Resources["hoverAnimation"] as Storyboard;
            sb.Begin();
        }

        private void repeatButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            AlbumItemHelper.PointerIsInContact = e.Pointer.IsInRange;

            repeatButton.Click -= repeatButton_Click;
            RepeatButtonCycleCounter = 0;
            repeatButton.Click += repeatButton_Click;

            Storyboard sb = this.Resources["restoreAnimation"] as Storyboard;
            sb.Begin();
        }

        private void AlbumCover_ImageOpened(object sender, RoutedEventArgs e)
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation()
            {
                To = 1,
                BeginTime = TimeSpan.FromMilliseconds(200),
                Duration = TimeSpan.FromMilliseconds(1200),
                EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTargetProperty(da, "Opacity");
            Storyboard.SetTarget(da, sender as Image);

            sb.Children.Add(da);

            sb.Begin();
        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            RepeatButtonCycleCounter = 0;
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Mouse)
                MenuTriggered?.Invoke(this, e.GetPosition(this));
        }

        private void Grid_Holding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.PointerDeviceType == PointerDeviceType.Touch && e.HoldingState == Windows.UI.Input.HoldingState.Started)
            {
                //LongPressed?.Invoke(this, e);
                //MenuTriggered?.Invoke(this, e.GetPosition(this), true);
                EventRegistrationTokenTable<EventHandler<LongPressEventArgs>>
.GetOrCreateEventRegistrationTokenTable(ref m_LongPressedTokenTable)
.InvocationList?.Invoke(this, new LongPressEventArgs(e.GetPosition(this), true));
            }
        }

        private void action_Click(object sender, TappedRoutedEventArgs e)
        {
            MenuTriggered?.Invoke(this, e.GetPosition(this));
        }

        private void shareButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {

        }


        private EventRegistrationTokenTable<EventHandler<LongPressEventArgs>>
    m_LongPressedTokenTable = null;

        public event EventHandler<LongPressEventArgs> LongPressed
        {
            add
            {
                EventRegistrationTokenTable<EventHandler<LongPressEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref m_LongPressedTokenTable)
                    .AddEventHandler(value);

                return;
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<LongPressEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref m_LongPressedTokenTable)
                    .RemoveEventHandler(value);
            }
        }
    }
}
