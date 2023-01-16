using System;
using Windows.Globalization.NumberFormatting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AudictiveMusicUWP.Gui.Util
{
    public class DateTimePassedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTimeOffset dateTime = DateTimeOffset.Now;
            TimeSpan ts = DateTimeOffset.Now.ToUniversalTime().Subtract(dateTime);

            string str = "";

            if (ts.Days >= 1)
            {
                str = $"{ts.Days.ToString()}d";
            }
            else if (ts.Hours >= 1 && ts.Hours <= 24)
            {
                str = $"{ts.Hours.ToString()}h";
            }
            else if (ts.Minutes >= 1 && ts.Minutes <= 60)
            {
                str = $"{ts.Minutes.ToString()}m";
            }
            else if (ts.Minutes < 1)
            {
                str = $"{ts.Seconds.ToString()}s";
            }
            
            


            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IsNowPlayingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                bool inp = (bool)value;

                if (inp)
                {
                    return "";
                }
                else
                {
                    return "";
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToInt32(value);
        }
    }

    public class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToString(value).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int number = (int)value;
            DecimalFormatter formatter = new DecimalFormatter()
            {
                IsGrouped = true,
                FractionDigits = 0
            };

            return formatter.FormatInt(number);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }

    public class RequestedThemeByColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color = (Color)value;
            return color.IsDarkColor() ? ElementTheme.Dark : ElementTheme.Light;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }


    public class FavoriteHeartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string str = System.Convert.ToString(value);

            if (str == "")
                return true;
            else
                return false;
        }
    }

}
