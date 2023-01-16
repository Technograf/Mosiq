using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class AppStateMessage
    {
        [DataMember]
        public AppState State
        {
            get;
            set;
        }

        public AppStateMessage(AppState state)
        {
            State = state;
        }
    }
}
