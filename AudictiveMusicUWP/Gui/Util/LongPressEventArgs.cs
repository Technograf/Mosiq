using Windows.Foundation;
using Windows.UI.Xaml;

namespace AudictiveMusicUWP.Gui.Util
{
    public sealed class LongPressEventArgs : RoutedEventArgs
    {
        public Point Position
        {
            get;
            set;
        }

        public bool TriggeredByTouch
        {
            get;
            set;
        }

        public LongPressEventArgs(Point position, bool touchHolding)
        {
            this.Position = position;
            this.TriggeredByTouch = touchHolding;
        }
    }

}
