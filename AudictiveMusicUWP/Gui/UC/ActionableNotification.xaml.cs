using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class ActionableNotification : UserControl
    {
        public bool IsVisible
        {
            get;
            set;
        }

        public bool IsRevealEnabled
        {
            get { return ((bool)GetValue(IsRevealEnabledProperty)); }
            set
            {
                SetValue(IsRevealEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsRevealEnabledProperty =
            DependencyProperty.Register("IsRevealEnabled", typeof(bool), typeof(ActionableNotification), new PropertyMetadata(false));

        public bool IsPrimaryActionEnabled
        {
            get { return ((bool)GetValue(IsPrimaryActionEnabledProperty)); }
            set
            {
                SetValue(IsPrimaryActionEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsPrimaryActionEnabledProperty =
            DependencyProperty.Register("IsPrimaryActionEnabled", typeof(bool), typeof(ActionableNotification), new PropertyMetadata(true));

        public bool IsSecondaryActionEnabled
        {
            get { return ((bool)GetValue(IsSecondaryActionEnabledProperty)); }
            set
            {
                SetValue(IsSecondaryActionEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty IsSecondaryActionEnabledProperty =
            DependencyProperty.Register("IsSecondaryActionEnabled", typeof(bool), typeof(ActionableNotification), new PropertyMetadata(true));

        public Visibility PrimaryActionVisibility
        {
            get { return ((Visibility)GetValue(PrimaryActionVisibilityProperty)); }
            set
            {
                SetValue(PrimaryActionVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty PrimaryActionVisibilityProperty =
            DependencyProperty.Register("PrimaryActionVisibility", typeof(Visibility), typeof(ActionableNotification), new PropertyMetadata(Visibility.Collapsed));

        public Visibility SecondaryActionVisibility
        {
            get { return ((Visibility)GetValue(SecondaryActionVisibilityProperty)); }
            set
            {
                SetValue(SecondaryActionVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty SecondaryActionVisibilityProperty =
            DependencyProperty.Register("SecondaryActionVisibility", typeof(Visibility), typeof(ActionableNotification), new PropertyMetadata(Visibility.Collapsed));

        public string PrimaryActionContent
        {
            get { return ((string)GetValue(PrimaryActionContentProperty)); }
            set
            {
                SetValue(PrimaryActionContentProperty, value);
            }
        }

        public static readonly DependencyProperty PrimaryActionContentProperty =
            DependencyProperty.Register("PrimaryActionContent", typeof(string), typeof(ActionableNotification), new PropertyMetadata(string.Empty));

        public string SecondaryActionContent
        {
            get { return ((string)GetValue(SecondaryActionContentProperty)); }
            set
            {
                SetValue(SecondaryActionContentProperty, value);
            }
        }

        public static readonly DependencyProperty SecondaryActionContentProperty =
            DependencyProperty.Register("SecondaryActionContent", typeof(string), typeof(ActionableNotification), new PropertyMetadata(string.Empty));

        public string Icon
        {
            get { return ((string)GetValue(IconProperty)); }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(ActionableNotification), new PropertyMetadata(""));

        public string Title
        {
            get { return ((string)GetValue(TitleProperty)); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ActionableNotification), new PropertyMetadata(string.Empty));

        public string Subtitle
        {
            get { return ((string)GetValue(SubtitleProperty)); }
            set
            {
                SetValue(SubtitleProperty, value);
            }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(ActionableNotification), new PropertyMetadata(string.Empty));


        public delegate void RoutedEventArgs(object sender, EventArgs e);

        public event RoutedEventArgs PrimaryActionClick;

        public event RoutedEventArgs SecondaryActionClick;

        public event RoutedEventArgs Closed;


        public void Show()
        {
            this.IsHitTestVisible = true;
            IsVisible = true;

            Storyboard sb = this.Resources["showAnimation"] as Storyboard;
            sb.Begin();
        }

        public void Hide()
        {
            this.IsHitTestVisible = false;
            IsVisible = false;

            Storyboard sb = this.Resources["hideAnimation"] as Storyboard;
            sb.Begin();
        }

        public void SetContent(string title, string subtitle, string icon)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Icon = icon;

            customContentArea.Visibility = Visibility.Collapsed;
            defaultButtonsArea.Visibility = Visibility.Visible;
        }

        public void SetContent(string title, string subtitle, string icon, List<UIElement> children)
        {
            this.Title = title;
            this.Subtitle = subtitle;
            this.Icon = icon;

            customContentArea.Children.Clear();

            foreach (UIElement e in children)
                customContentArea.Children.Add(e);

            customContentArea.Visibility = Visibility.Visible;
            defaultButtonsArea.Visibility = Visibility.Collapsed;
        }

        public ActionableNotification()
        {
            this.IsHitTestVisible = false;
            IsVisible = false;
            this.Opacity = 0;
            this.InitializeComponent();
        }

        private void action1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PrimaryActionClick?.Invoke(this, EventArgs.Empty);
            Hide();
        }

        private void action2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SecondaryActionClick?.Invoke(this, EventArgs.Empty);
            Hide();
        }

        private void closeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Hide();
        }

        private void hideAnimation_Completed(object sender, object e)
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void Rectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Hide();
        }
    }
}
