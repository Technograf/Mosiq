using ClassLibrary.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// O modelo de item de Controle de Usuário está documentado em https://go.microsoft.com/fwlink/?LinkId=234236

namespace AudictiveMusicUWP.Gui.UC
{
    public sealed partial class SimpleNotice : UserControl
    {
        public delegate void RoutedEventArgs(object sender);

        public event RoutedEventArgs Dismissed;

        public string Icon
        {
            get { return ((string)GetValue(IconProperty)); }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(SimpleNotice), new PropertyMetadata(""));


        public string Message
        {
            get { return ((string)GetValue(MessageProperty)); }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(SimpleNotice), new PropertyMetadata(string.Empty));

        public string Title
        {
            get { return ((string)GetValue(TitleProperty)); }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SimpleNotice), new PropertyMetadata(string.Empty));

        public string Caption
        {
            get { return ((string)GetValue(CaptionProperty)); }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(SimpleNotice), new PropertyMetadata(""));



        public SimpleNotice()
        {
            this.Caption = ApplicationInfo.Current.Resources.GetString("TapToDismiss");
            this.InitializeComponent();
        }

        public void Show(string message, string title = "")
        {
            this.Message = message;
            this.Title = title;
            this.Visibility = Visibility.Visible;
        }

        private void layoutRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        public void Hide()
        {
            this.Visibility = Visibility.Collapsed;

            Dismissed?.Invoke(this);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Hide();
        }
    }
}
