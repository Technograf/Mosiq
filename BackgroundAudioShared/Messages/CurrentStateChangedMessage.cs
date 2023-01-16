using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class CurrentStateChangedMessage
    {
        [DataMember]
        public MediaPlaybackState State
        {
            get;
            set;
        }
        public CurrentStateChangedMessage(MediaPlaybackState state)
        {
            State = state;
        }
    }
}
