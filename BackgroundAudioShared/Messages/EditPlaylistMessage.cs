using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class EditPlaylistMessage
    {
        [DataMember]
        public Mode Mode
        {
            get;
            set;
        }

        [DataMember]
        public int TrackIndex
        {
            get;
            set;
        }

        [DataMember]
        public int NewIndex
        {
            get;
            set;
        }

        public EditPlaylistMessage(Mode mode, int index)
        {
            Mode = mode;
            TrackIndex = index;
        }

        public EditPlaylistMessage(Mode mode, int oldIndex, int newIndex)
        {
            Mode = mode;
            TrackIndex = oldIndex;
            NewIndex = newIndex;
        }
    }

    public enum Mode
    {
        Remove,
        MoveDown,
        MoveUp,
        DragAndDrop
    }
}
