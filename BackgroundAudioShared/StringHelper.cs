using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundAudioShared
{
    public static class StringHelper
    {
        public static string RemoveSpecialChar(string artist)
        {
            return artist.Replace(" ", "").ToLower().Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ç", "c").Replace("ã", "a").Replace("õ", "o").Replace("ñ", "n").Replace("ï", "i").Replace("&", "and").Replace("!", "i").Replace("|", "/");
        }

        public static string EscapeString(string str)
        {
            return str.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("\'", "&apos;");
        }
    }
}
