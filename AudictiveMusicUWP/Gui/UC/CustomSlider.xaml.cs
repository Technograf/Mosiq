using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class CustomSlider : UserControl
    {
        private double PvtValue
        {
            get
            {
                return (double)GetValue(PvtValueProperty);
            }
            set
            {
                SetValue(PvtValueProperty, value);
            }
        }

        private DependencyProperty PvtValueProperty = DependencyProperty.Register("PvtValue", typeof(double), typeof(CustomSlider), new PropertyMetadata((double)0));


        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                double v;

                if (value < Minimum)
                    v = Minimum;
                else
                    v = value;

                valueText.Text = v.ToString();

                SetValue(ValueProperty, v);

                Storyboard sb = new Storyboard();

                DoubleAnimation da = new DoubleAnimation()
                {
                    To = v,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EnableDependentAnimation = true,
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(da, this);
                Storyboard.SetTargetProperty(da, "CustomSlider.PvtValue");


                DoubleAnimation da1 = new DoubleAnimation()
                {
                    To = v,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EnableDependentAnimation = true,
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard.SetTarget(da1, this);
                Storyboard.SetTargetProperty(da1, "CustomSlider.TickPosition");

                sb.Children.Add(da);
                if (IsDragging == false)
                    sb.Children.Add(da1);

                sb.Begin();
            }
        }

        public DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CustomSlider), new PropertyMetadata((double)0));



        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(CustomSlider), new PropertyMetadata((double)100));


        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        public DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(CustomSlider), new PropertyMetadata((double)0));


        public double TickPosition
        {
            get
            {
                return (double)GetValue(TickPositionProperty);
            }
            private set
            {
                SetValue(TickPositionProperty, value);
            }
        }

        public DependencyProperty TickPositionProperty = DependencyProperty.Register("TickPosition", typeof(double), typeof(CustomSlider), new PropertyMetadata((double)0));

        private DispatcherTimer Tick;

        private Point InitialManipulationPosition;

        private bool IsDragging;

        public CustomSlider()
        {
            this.InitializeComponent();

            IsDragging = false;
            Tick = new DispatcherTimer();
            Tick.Interval = TimeSpan.FromSeconds(1);
            Tick.Tick -= Tick_Tick;
            Tick.Tick += Tick_Tick;
            Tick.Start();
        }

        private void Tick_Tick(object sender, object e)
        {
            try
            {
                Value = Value + 15;
            }
            catch
            {

            }
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Grid_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Grid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Grid_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Grid_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {

        }

        private void sliderThumb_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            IsDragging = true;
            InitialManipulationPosition = e.Position;
        }

        private void sliderThumb_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            TickPosition = e.Position.X;
        }

        private void sliderThumb_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            IsDragging = false;
            Value = e.Position.X;
        }
    }
}
