using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class SetPlaylistMessage
    {
        [DataMember]
        public List<string> Playlist
        {
            get;
            set;
        }

        [DataMember]
        public int Index
        {
            get;
            set;
        }

        [DataMember]
        public double InitialPosition
        {
            get;
            set;
        }

        public SetPlaylistMessage(List<string> list)
        {
            Playlist = list;
            Index = 0;
            InitialPosition = 0;
        }

        public SetPlaylistMessage(List<string> list, int index, double milliseconds)
        {
            Playlist = list;
            Index = index;
            InitialPosition = milliseconds;
        }
    }
}
