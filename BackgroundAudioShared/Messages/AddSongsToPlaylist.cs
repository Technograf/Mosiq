using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class AddSongsToPlaylist
    {
        [DataMember]
        public List<string> SongsToAdd;

        [DataMember]
        public bool AsNext;

        public AddSongsToPlaylist(List<string> songs)
        {
            SongsToAdd = songs;
            AsNext = false;
        }

        public AddSongsToPlaylist(List<string> songs, bool asNext)
        {
            SongsToAdd = songs;
            AsNext = asNext;
        }
    }
}
