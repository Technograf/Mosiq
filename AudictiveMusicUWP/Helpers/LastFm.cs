using BackgroundAudioShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace AudictiveMusicUWP.Helpers
{
    class LastFm
    {
        public static async Task<string> GetArtistXML(string artistName, string lang)
        {
            string xml = string.Empty;

            bool allow = false;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DownloadPermission"))
            {
                allow = (bool)ApplicationData.Current.LocalSettings.Values["DownloadPermission"];
            }
            else
            {
                allow = true;
            }

            if (allow)
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync("http://ws.audioscrobbler.com/2.0/?method=artist.getInfo&artist=" + artistName.Replace("+", "%2B").Replace("&", "%26") + "&api_key=cb9857dcb6699029b0ddd8e4db8f78dd&lang=" + lang);

                response.EnsureSuccessStatusCode();

                xml = await response.Content.ReadAsStringAsync();

            }
            else
                xml = null;

            return xml;
        }

        public static async Task SaveBiography(XmlDocument xmlDoc, string artist)
        {
            XmlElement fullNode = xmlDoc.SelectSingleNode("/lfm/artist/bio/content") as XmlElement;
            StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);
            StorageFile bioFile = await folder.CreateFileAsync(StringHelper.RemoveSpecialChar(artist) + "_bio.txt", CreationCollisionOption.ReplaceExisting);

            string bio = fullNode.InnerText;

            try
            {
                if (bio.Contains("<a href=\"https://www.last.fm/music"))
                    bio = bio.Split(new string[] { "<a href=\"https://www.last.fm/music" }, StringSplitOptions.None)[0];
            }
            catch
            {

            }

            await FileIO.WriteTextAsync(bioFile, fullNode.InnerText);
        }
    }
}
