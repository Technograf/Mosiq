using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class CurrentIndexMessage
    {
        [DataMember]
        public int Index
        {
            get;
            set;
        }

        public CurrentIndexMessage(int index)
        {
            Index = index;
        }
    }
}
