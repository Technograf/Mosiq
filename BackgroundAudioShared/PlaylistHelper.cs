using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BackgroundAudioShared
{
    public static class PlaylistHelper
    {
        public static List<string> CurrentPlaylist = new List<string>();

        public static string TrackByIndex(int index)
        {
            if (CurrentPlaylist.Count > 0)
                return CurrentPlaylist[index];
            else
                return "LISTA VAZIA!!";
        }

        public async static Task SaveCurrentPlaylist()
        {
            if (CurrentPlaylist.Count == 0)
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("LastPlayback.xml", CreationCollisionOption.OpenIfExists);
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                return;
            }

            XmlDocument DOC = new XmlDocument();

            XmlElement mainElem = DOC.DocumentElement;
            XmlElement ELE = DOC.CreateElement("Playlist");
            ELE.SetAttribute("Name", "LastPlayback");
            DOC.AppendChild(ELE);

            foreach (string file in CurrentPlaylist)
            {
                XmlElement x = DOC.CreateElement("Song");
                x.InnerText = file;

                DOC.FirstChild.AppendChild(x);
            }

            StorageFile st = await ApplicationData.Current.LocalFolder.CreateFileAsync("LastPlayback.xml", CreationCollisionOption.ReplaceExisting);

            await DOC.SaveToFileAsync(st);

            ApplicationData.Current.LocalSettings.Values["ExistsLastPlayback"] = true;
        }

    }
}
