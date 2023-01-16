using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudictiveMusicUWP.Helpers
{
    public class PageHelper
    {
        public static MainPage MainPage
        {
            get;
            set;
        }

        public static Settings Settings
        {
            get;
            set;
        }

        public static NowPlaying NowPlaying
        {
            get;
            set;
        }

        public static PreparingCollection PreparingCollection
        {
            get;
            set;
        }

        public static Artists Artists
        {
            get;
            set;
        }

        public static Albums Albums
        {
            get;
            set;
        }

        public static Songs Songs
        {
            get;
            set;
        }

        public static Playlists Playlists
        {
            get;
            set;
        }

        public static PlaylistPage PlaylistPage
        {
            get;
            set;
        }

        public static SetupWizard SetupWizard
        {
            get;
            set;
        }
    }
}
