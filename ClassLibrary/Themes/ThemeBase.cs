using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ClassLibrary.Themes
{
    public class ThemeBase
    {
        protected ThemeBase()
        {

        }

        //public static async Task<List<FrameworkElement>> LoadThemeXamlSettings(Theme theme)
        //{
        //    var list = new List<FrameworkElement>();
        //    XmlDocument doc = await XmlDocument.LoadFromUriAsync(new Uri("ms-appx:///Themes/" + theme.ToString() + ".xml", UriKind.Absolute));
        //    IXmlNode node = doc.GetElementsByTagName("Elements").FirstOrDefault();
        //    foreach (XmlElement e in node.ChildNodes)
        //        list.Add(GlobalizeElement(XamlReader.Load(e.GetXml())));

        //    return list;
        //}

        //private static FrameworkElement GlobalizeElement(object obj)
        //{
        //    Type type = obj.GetType();

        //    if (type == typeof(CheckBox))
        //    {
        //        CheckBox c = obj as CheckBox;
        //        c.Content = ApplicationInfo.Current.Resources.GetString(c.Content.ToString());

        //        return c;
        //    }
        //    else if (type == typeof(Slider))
        //    {
        //        Slider s = obj as Slider;
        //        s.Header = ApplicationInfo.Current.Resources.GetString(s.Header.ToString());

        //        return s;
        //    }
        //    else if (type == typeof(TextBlock))
        //    {
        //        TextBlock t = obj as TextBlock;
        //        t.Text = ApplicationInfo.Current.Resources.GetString(t.Text);

        //        return t;
        //    }
        //    else if (type == typeof(ToggleSwitch))
        //    {
        //        ToggleSwitch t = obj as ToggleSwitch;
        //        t.Header = ApplicationInfo.Current.Resources.GetString(t.Header.ToString());

        //        return t;
        //    }
        //    else
        //        return null;
        //}

        public string Name { get; set; }
        public Uri SampleImageUri { get; set; }
        public bool IsFree { get { return this.PurchaseID == 0; } }
        
        /// <summary>
        /// Identificador do pacote. 0 para gratuito
        /// </summary>
        public int PurchaseID { get; set; }

        public bool? IsSelected
        {
            get
            {
                return ApplicationSettings.NowPlayingTheme == this.Theme;
            }
            set
            {
                if (value == true)
                    ApplicationSettings.NowPlayingTheme = this.Theme;
            }
        }
        public Theme Theme { get => (Theme)Enum.Parse(typeof(Theme), this.Name); }
    }
}
