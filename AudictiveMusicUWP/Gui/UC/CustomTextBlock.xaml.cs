using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// O modelo de item de Controle de Usuário está documentado em https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class CustomTextBlock : UserControl
    {
        public string Text
        {
            get { return ((string)GetValue(TextProperty)); }
            set
            {
                if (value != Text)
                    SetText(value);
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomTextBlock), new PropertyMetadata(string.Empty));

        private string nextTxt;

        public FontWeight Weight
        {
            get { return ((FontWeight)GetValue(WeightProperty)); }
            set
            {
                SetValue(WeightProperty, value);
            }
        }

        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(FontWeight), typeof(CustomTextBlock), new PropertyMetadata(FontWeights.Normal));


        public double Size
        {
            get { return ((double)GetValue(SizeProperty)); }
            set
            {
                SetValue(SizeProperty, value);
            }
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(CustomTextBlock), new PropertyMetadata(14));


        public TextTrimming TextTrimming
        {
            get { return ((TextTrimming)GetValue(TextTrimmingProperty)); }
            set
            {
                SetValue(TextTrimmingProperty, value);
            }
        }

        public static readonly DependencyProperty TextTrimmingProperty =
            DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(CustomTextBlock), new PropertyMetadata(TextTrimming.None));


        public TextAlignment TextAlignment
        {
            get { return ((TextAlignment)GetValue(TextAlignmentProperty)); }
            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(CustomTextBlock), new PropertyMetadata(TextAlignment.Left));

        private void SetText(string value)
        {
            nextTxt = value;
            Storyboard sb = this.Resources["flipStart"] as Storyboard;
            sb.Begin();
        }

        public CustomTextBlock()
        {
            this.InitializeComponent();
        }

        private async void flipStart_Completed(object sender, object e)
        {
            await Dispatcher.RunIdleAsync((s) =>
            {
                //tblock.Opacity = 0;
                projection.RotationX = 90;
                SetValue(TextProperty, nextTxt);
                tblock.Text = nextTxt;
                Storyboard sb = this.Resources["flipEnd"] as Storyboard;
                sb.Begin();
            });
        }
    }
}
