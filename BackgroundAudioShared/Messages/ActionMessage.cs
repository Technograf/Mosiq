using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class ActionMessage
    {
        [DataMember]
        public Action Action
        {
            get;
            set;
        }

        [DataMember]
        public string Parameter
        {
            get;
            set;
        }

        public ActionMessage(Action action)
        {
            Action = action;
            Parameter = string.Empty;
        }

        public ActionMessage(Action action, string parameter)
        {
            Action = action;
            Parameter = parameter;
        }
    }

    public enum Action
    {
        Shuffle,
        Stop,
        Resume,
        PlayEverything,
        AskCurrentTrack,
        AskPlaylist,
        SkipToPrevious,
        SkipToNext,
        PlayPause,
        ClearPlayback
    }
}
