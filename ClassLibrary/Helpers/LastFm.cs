using BackgroundAudioShared;
using ClassLibrary.Entities;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ClassLibrary.Helpers
{
    public class LastFm
    {
        public delegate void LastFmConnectedEventArgs(LastUser loggedUser);
        public event LastFmConnectedEventArgs Connected;
        public delegate void LastFmDisconnectedEventArgs();
        public event LastFmDisconnectedEventArgs Disconnected;
        public delegate void RoutedEventArgs(Artist artist);
        public static event RoutedEventArgs DownloadCompleted;

        private string ApiKey = ApplicationSettings.LastFmAPIKey;
        private string Secret = ApplicationSettings.LastFmSecret;
        private LastfmClient lfmClient;
        public LastfmClient Client
        {
            get
            {
                if (lfmClient == null)
                {
                    lfmClient = new LastfmClient(new LastAuth(ApiKey, Secret));
                    LastUserSession lastUserSession = new LastUserSession()
                    {
                        Token = ApplicationSettings.LastFmSessionToken,
                        Username = ApplicationSettings.LastFmSessionUsername,
                    };
                    lfmClient.Auth.LoadSession(lastUserSession);
                }

                return lfmClient;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ApplicationSettings.LastFmSessionToken);
            }
        }

        private LastFm()
        {

        }

        private static LastFm current;
        public static LastFm Current
        {
            get
            {
                if (current == null)
                    current = new LastFm();

                return current;
            }
        }


        private string SessionKey
        {
            get
            {
                object value = ApplicationSettings.ReadSettingsValue("LastFmSessionKey");
                if (value == null)
                    return string.Empty;
                else
                    return value.ToString();
            }
            set
            {
                ApplicationSettings.SaveSettingsValue("LastFmSessionKey", value);
            }
        }

        public static async Task DownloadImage(Artist artist, bool fullQuality = false)
        {
            //GC.Collect();
            string lang = ApplicationInfo.Current.Language;

            var response = await Current.Client.Artist.GetInfoAsync(artist.Name, lang, true);
            var art = response.Content;

            Uri image;

            if (fullQuality)
                image = art.MainImage.Mega;
            else
                image = art.MainImage.ExtraLarge;


            string artistNameWithOutSpaces = StringHelper.RemoveSpecialChar(artist.Name);
            StorageFolder artistImagesFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);

            try
            {
                HttpClient client = new HttpClient();

                HttpResponseMessage message = await client.GetAsync(image.AbsoluteUri);

                StorageFile file = await artistImagesFolder.CreateFileAsync("artist_" + artistNameWithOutSpaces + ".jpg", CreationCollisionOption.OpenIfExists);// this line throws an exception
                byte[] bytes = await message.Content.ReadAsByteArrayAsync();

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    using (var outputStream = stream.GetOutputStreamAt(0))
                    {
                        DataWriter writer = new DataWriter(outputStream);
                        writer.WriteBytes(bytes);
                        await writer.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }



                //await FileIO.WriteBytesAsync(sampleFile, file);




                //        var image = new WriteableBitmap(50, 50);

                //        var fil = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/StoreLogo.png"));

                //        var content = await fil.OpenReadAsync();

                //        image.SetSource(stream.AsRandomAccessStream());

                //        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(
                //BitmapEncoder.JpegEncoderId,
                //await sampleFile.OpenAsync(FileAccessMode.ReadWrite));



                //        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)image.PixelWidth, (uint)image.PixelHeight, 96.0, 96.0, file);

                //        await encoder.FlushAsync();





                DownloadCompleted?.Invoke(artist);

                //ImageHelper.BlurArtistImage(name);
            }
            catch (Exception ex)
            {
                DownloadCompleted?.Invoke(artist);
            }
            //}

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

        public async Task<bool> Login(string username, string password)
        {
            LastResponse response = await Client.Auth.GetSessionTokenAsync(username, password);
            if (response.Success)
            {
                LastUserSession us = Client.Auth.UserSession;

                var userResponse = await Client.User.GetInfoAsync(us.Username);
                LastUser user = userResponse.Content;

                if (user != null)
                {
                    //MessageDialog md = new MessageDialog($"Welcome, {user.FullName}");
                    //await md.ShowAsync();
                    ApplicationSettings.LastFmSessionToken = us.Token;
                    ApplicationSettings.LastFmSessionUsername = us.Username;
                    ApplicationSettings.LastFmSessionUserImageUri = user.Avatar.Large.AbsoluteUri;

                    this.Connected?.Invoke(user);
                }
            }

            return response.Success;
        }

        public void Logout()
        {
            ApplicationSettings.LastFmSessionToken = "";
            ApplicationSettings.LastFmSessionUsername = "";

            this.Disconnected?.Invoke();
        }

        public async Task<LastUser> GetUserInfo(string username)
        {
            var result = await Client.User.GetInfoAsync(username);
            if (result.Success)
            {
                return result.Content;
            }
            else
                return null;
        }

        private static string EncodeUTF8(string str)
        {
            string propEncodeString = string.Empty;

            byte[] utf8_Bytes = new byte[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                utf8_Bytes[i] = (byte)str[i];
            }

            return Encoding.UTF8.GetString(utf8_Bytes, 0, utf8_Bytes.Length);
        }
    }

    public class CustomLastTrack
    {
        public bool? IsLoved { get; set; }
        public DateTimeOffset? TimePlayed { get; set; }
        public IEnumerable<LastTag> TopTags { get; set; }
        public int? PlayCount { get; set; }
        public int? ListenerCount { get; set; }
        public string AlbumName { get; set; }
        public bool? IsNowPlaying { get; set; }
        public LastImageSet Images { get; set; }
        public string ArtistMbid { get; set; }
        public string ArtistName { get; set; }
        public string Mbid { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public Uri Url { get; set; }
        public int? Rank { get; set; }

        public static CustomLastTrack Convert(LastTrack track)
        {
            CustomLastTrack clt = new CustomLastTrack()
            {
                IsLoved = track.IsLoved,
                TimePlayed = track.TimePlayed,
                TopTags = track.TopTags,
                PlayCount = track.PlayCount,
                AlbumName = track.AlbumName,
                ArtistMbid = track.ArtistMbid,
                ArtistName = track.ArtistName,
                Duration = track.Duration,
                Id = track.Id,
                Images = track.Images,
                IsNowPlaying = track.IsNowPlaying,
                ListenerCount = track.ListenerCount,
                Mbid = track.Mbid,
                Name = track.Name,
                Rank = track.Rank,
                Url = track.Url
            };

            return clt;
        }


    }
}
