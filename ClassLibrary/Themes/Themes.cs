using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Themes
{
    public class CleanTheme : ThemeBase
    {
        public static bool HasSettings { get => false; }
        public CleanTheme()
        {
            this.Name = "Clean";
            this.PurchaseID = 0;
            this.IsSelected = false;
            this.SampleImageUri = new Uri("ms-appx:///Assets/Themes/clean.png", UriKind.Absolute);
        }
    }

    public class ModernTheme : ThemeBase
    {
        public static bool HasSettings { get => false; }

        public ModernTheme()
        {
            this.Name = "Modern";
            this.PurchaseID = 0;
            this.IsSelected = false;
            this.SampleImageUri = new Uri("ms-appx:///Assets/Themes/modern.png", UriKind.Absolute);
        }
    }

    public class BlurTheme : ThemeBase
    {
        public static bool HasSettings { get => true; }

        public BlurTheme()
        {
            this.Name = "Blur";
            this.PurchaseID = 0;
            this.IsSelected = false;
            this.SampleImageUri = new Uri("ms-appx:///Assets/Themes/blur.png", UriKind.Absolute);
        }
    }


    public class NeonTheme : ThemeBase
    {
        public static bool HasSettings { get => false; }

        public NeonTheme()
        {
            this.Name = "Neon";
            this.PurchaseID = 1;
            this.IsSelected = false;
            this.SampleImageUri = new Uri("ms-appx:///Assets/Themes/neon.png", UriKind.Absolute);
        }
    }


    public class MaterialTheme : ThemeBase
    {
        public static bool HasSettings { get => false; }

        public MaterialTheme()
        {
            this.Name = "Material";
            this.PurchaseID = 0;
            this.IsSelected = false;
            this.SampleImageUri = new Uri("ms-appx:///Assets/Themes/material.png", UriKind.Absolute);
        }
    }

}
