using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class PlaylistMessage
    {
        [DataMember]
        public List<string> Playlist
        {
            get;
            set;
        }

        public PlaylistMessage(List<string> list)
        {
            Playlist = list;
        }
    }
}
