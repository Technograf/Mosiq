using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class JumpToIndexMessage
    {
        [DataMember]
        public int Index
        {
            get;
            set;
        }

        public JumpToIndexMessage(int index)
        {
            Index = index;
        }
    }
}
