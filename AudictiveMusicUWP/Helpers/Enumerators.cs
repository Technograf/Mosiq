using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudictiveMusicUWP.Helpers
{
    public static class Enumerators
    {
        public enum MediaItemType
        {
            Song,
            Artist,
            Album,
            Playlist
        }

        public enum SettingsPageContent
        {
            Personalization,
            AppInfo,
            Playback,
            Permissions,
            DataManagement,
            Menu,
            Feedback,
            None
        }
    }
}
