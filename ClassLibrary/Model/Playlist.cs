using ClassLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Model
{
    public class Playlist
    {

        public string Name
        {
            get;
            set;
        }

        public List<string> Songs
        {
            get;
            set;
        }

        public string PlaylistFileName
        {
            get;
            set;
        }

        public string FriendlyCountValue
        {
            get
            {
                if (Songs != null)
                {
                    if (Songs.Count > 1)
                        return Songs.Count + " " + ApplicationInfo.Current.Resources.GetString("SongPlural").ToLower();
                    else
                        return Songs.Count + " " + ApplicationInfo.Current.Resources.GetString("SongSingular").ToLower();
                }
                else
                {
                    return ApplicationInfo.Current.Resources.GetString("Empty");
                }
            }
        }

        public Playlist(string name, string playlistFileName, List<string> songs)
        {
            Name = name;
            PlaylistFileName = playlistFileName;
            Songs = new List<string>();
            Songs.AddRange(songs);
        }
    }
}
