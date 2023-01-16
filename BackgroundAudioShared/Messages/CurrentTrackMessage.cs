using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared.Messages
{
    [DataContract]
    public class CurrentTrackMessage
    {
        [DataMember]
        public string Path
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

        /// <summary>
        /// Inicializa uma instância da mensagem. Serve para enviar o caminho da música (path)
        /// </summary>
        /// <param name="path"></param>
        public CurrentTrackMessage(string path, int index)
        {
            Path = path;
            Index = index;
        }
    }
}
