using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Windows.UI
{
    public static class ColorExtensions
    {
        public static float GetBrightness(this Color color)
        {
            float num = ((float)color.R) / 255f;
            float num2 = ((float)color.G) / 255f;
            float num3 = ((float)color.B) / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
                num4 = num2;
            if (num3 > num4)
                num4 = num3;
            if (num2 < num5)
                num5 = num2;
            if (num3 < num5)
                num5 = num3;
            return ((num4 + num5) / 2f);
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(this Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        public static Color GetStrongestRGBColor (this Color color)
        {
            if (color.GetBrightness() >= 0.9f)
                return Color.FromArgb(255, 33, 33, 33);
            else if (color.GetBrightness() <= 0.1f)
                return Color.FromArgb(255, 222, 222, 222);

            bool rMaior = false;
            bool gMaior = false;
            bool bMaior = false;

            if (color.R > color.G)
                rMaior = true;
            else
                gMaior = true;

            if (rMaior)
            {
                if (color.R > color.B)
                    rMaior = true;
                else
                    bMaior = true;
            }
            else
            {
                if (color.G > color.B)
                    gMaior = true;
                else
                    bMaior = true;
            }


            if (rMaior)
            {
                if (color.R > 127)
                    return Colors.Red;
                else
                    return Color.FromArgb(255, 144, 144, 144);
            }
            else if (gMaior)
            {
                if (color.G > 127)
                    return Colors.Green;
                else
                    return Color.FromArgb(255, 144, 144, 144);
            }
            else if (bMaior)
            {
                if (color.B > 127)
                    return Colors.Blue;
                else
                    return Color.FromArgb(255, 144, 144, 144);
            }
            else return Colors.Orange;

        }

        public static Color GetOppositeColor(this Color color)
        {
            //return Color.FromArgb(255, Convert.ToByte(color.R > 127 ? 0 : 255), Convert.ToByte(color.G > 127 ? 0 : 255), Convert.ToByte(color.B > 127 ? 0 : 255));

            int RGBMAX = 255;
            byte invR = Convert.ToByte(RGBMAX - color.R);
            byte invG = Convert.ToByte(RGBMAX - color.G);
            byte invB = Convert.ToByte(RGBMAX - color.B);


            return Color.FromArgb(255, invR, invG, invB);

            //byte r = color.R;
            //byte g = color.G;
            //byte b = color.B;

            //if (r < 60 && r > 20
            //    && g < 60 && g > 20
            //    && b < 60 && b > 20)
            //{
            //    r = Convert.ToByte(r * 2);
            //    g = Convert.ToByte(g * 2);
            //    b = Convert.ToByte(b * 2);

            //    return Color.FromArgb(color.A, r, g, b);
            //}
            //else
            //if (color.GetBrightness() < 0.1f)
            //    return Color.FromArgb(color.A, 127, 127, 127);

            //return Color.FromArgb(color.A, color.R, color.B, color.G);


        }

        public static bool IsDarkColor(this Color color)
        {
            return color.GetBrightness() <= 0.5f;
        }
    }


}
