using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ClassLibrary.Themes
{
    public class ThemeColor
    {
        public ThemeColor()
        {

        }

        public Color Color { get; set; }

        public bool? IsSelected
        {
            get
            {
                return ApplicationSettings.CustomThemeColor == this.Color;
            }
            set
            {
                if (value == true)
                    ApplicationSettings.CustomThemeColor = this.Color;
            }
        }
    }
}
