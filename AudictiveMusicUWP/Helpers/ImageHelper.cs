using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using Windows.Networking.Connectivity;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Diagnostics;
using BackgroundAudioShared;

namespace AudictiveMusicUWP.Helpers
{
    public static class ImageHelper
    {
        public static async Task<Color> GetDominantColor(IRandomAccessStream imageData)
        {
            var decoderCopy = await BitmapDecoder.CreateAsync(imageData);

            //Create a transform to get a 1x1 image
            var myTransform = new BitmapTransform { ScaledHeight = 1, ScaledWidth = 1 };

            //Get the pixel provider
            var pixelData = await decoderCopy.GetPixelDataAsync(
                BitmapPixelFormat.Rgba8,
                BitmapAlphaMode.Ignore,
                myTransform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage);

            //Get the bytes of the 1x1 scaled image
            var bytes = pixelData.DetachPixelData();

            //read the color 
            return Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
        }

        public static async Task<string> GetDominantColorHex(IRandomAccessStream imageData)
        {
            Color color = await GetDominantColor(imageData);

            return String.Format("#{0}{1}{2}{3}",
                         color.A.ToString("X2"),
                         color.R.ToString("X2"),
                         color.G.ToString("X2"),
                         color.B.ToString("X2"));

        }

        public static Color GetColorFromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex) == false)
            {
                hex = hex.Replace("#", string.Empty);
                byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                return Color.FromArgb(255, 70, 70, 70);
            }
        }

        public static async Task<SaveCoverImageResult> SaveAlbumCover(byte[] dataVector, string albumid)
        {
            var displayInformation = DisplayInformation.GetForCurrentView();

            StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);

            SaveCoverImageResult result;
            string hexColor = string.Empty;
            SaveCoverImageResult.Result resultState;

            if (dataVector != null)
            {
                try
                {
                    MemoryStream ms = new MemoryStream(dataVector);

                    ms.Position = 0;

                    StorageFile f = await coversFolder.CreateFileAsync("cover_" + albumid + ".jpg", CreationCollisionOption.ReplaceExisting);

                    IRandomAccessStream ras = ms.AsRandomAccessStream();

                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ras);

                    double oldHeight = decoder.PixelHeight;
                    double oldWidth = decoder.PixelWidth;
                    double newWidth = 500;

                    if (oldWidth < 500)
                        newWidth = oldWidth;

                    double newHeight = (oldHeight * newWidth) / oldWidth;

                    BitmapTransform transform = new BitmapTransform() { ScaledWidth = (uint)newWidth, ScaledHeight = (uint)newHeight, InterpolationMode = BitmapInterpolationMode.NearestNeighbor };
                    PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                        BitmapPixelFormat.Rgba8,
                        BitmapAlphaMode.Ignore,
                        transform,
                        ExifOrientationMode.RespectExifOrientation,
                        ColorManagementMode.DoNotColorManage);

                    using (var destinationStream = await f.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);
                        encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, (uint)newWidth, (uint)newHeight, displayInformation.RawDpiX, displayInformation.RawDpiY, pixelData.DetachPixelData());
                        await encoder.FlushAsync();
                    }

                    hexColor = await ImageHelper.GetDominantColorHex(ras);

                    ras.Dispose();
                    ms.Dispose();

                    resultState = SaveCoverImageResult.Result.Succeded;
                }
                catch
                {
                    resultState = SaveCoverImageResult.Result.Failed;
                }
            }
            else
                resultState = SaveCoverImageResult.Result.Failed;

            result = new SaveCoverImageResult(hexColor, resultState);

            return result;
        }

        public static async void BlurAlbumCover(string albumID)
        {
            try
            {
                StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
                IStorageItem storageItem = await coversFolder.TryGetItemAsync("cover_" + albumID + ".jpg");
                StorageFile f = await coversFolder.CreateFileAsync("cover_" + albumID + "_blur.jpg", CreationCollisionOption.ReplaceExisting);

                if (storageItem != null)
                {
                    StorageFile image = storageItem as StorageFile;

                    var imageData = await image.OpenReadAsync();
                    var decoder = await BitmapDecoder.CreateAsync(imageData);

                    IRandomAccessStream sx = await f.OpenAsync(FileAccessMode.ReadWrite);

                    var i = await decoder.GetPixelDataAsync();

                    var pixels = i.DetachPixelData();

                    // Useful for rendering in the correct DPI
                    var displayInformation = DisplayInformation.GetForCurrentView();

                    //var stream = new InMemoryRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, sx);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Premultiplied,
                                         (uint)decoder.PixelWidth,
                                         (uint)decoder.PixelHeight,
                                         displayInformation.RawDpiX,
                                         displayInformation.RawDpiY,
                                         pixels);

                    await encoder.FlushAsync();
                    sx.Seek(0);

                    var device = new CanvasDevice();
                    var bitmap = await CanvasBitmap.LoadAsync(device, sx);

                    var renderer = new CanvasRenderTarget(device,
                                                          bitmap.SizeInPixels.Width,
                                                          bitmap.SizeInPixels.Height, bitmap.Dpi);

                    using (var ds = renderer.CreateDrawingSession())
                    {
                        var blur = new GaussianBlurEffect();
                        blur.BlurAmount = 4.0f;
                        blur.Source = bitmap;
                        ds.DrawImage(blur);
                    }

                    sx.Seek(0);
                    await renderer.SaveAsync(sx, CanvasBitmapFileFormat.Jpeg);
                }
            }
            catch
            {

            }
        }


        public static async void ResizeImage(StorageFile file, int width)
        {
            try
            {
                var displayInformation = DisplayInformation.GetForCurrentView();

                IRandomAccessStream ras = await file.OpenAsync(FileAccessMode.Read);

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(ras);

                double oldHeight = decoder.PixelHeight;
                double oldWidth = decoder.PixelWidth;
                double newWidth = width;

                if (oldWidth < width)
                    newWidth = oldWidth;

                double newHeight = (oldHeight * newWidth) / oldWidth;

                BitmapTransform transform = new BitmapTransform() { ScaledWidth = (uint)newWidth, ScaledHeight = (uint)newHeight, InterpolationMode = BitmapInterpolationMode.NearestNeighbor };
                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Ignore,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);

                ras.Dispose();

                using (var destinationStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, destinationStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, (uint)newWidth, (uint)newHeight, displayInformation.RawDpiX, displayInformation.RawDpiY, pixelData.DetachPixelData());
                    await encoder.FlushAsync();
                }

            }
            catch
            {

            }
        }

        public static async Task DownloadImage(string uri, string name)
        {
            //Debug.WriteLine("Downloading image: " + name);

            string ImageURL = uri;
            string artistNameWithOutSpaces = StringHelper.RemoveSpecialChar(name);
            StorageFolder artistImagesFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);

            bool bgImgExistsInServer;
            try
            {
                HttpWebRequest request = (HttpWebRequest)System.Net.WebRequest.Create(ImageURL);
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    bgImgExistsInServer = response.StatusCode == HttpStatusCode.OK;
                    bgImgExistsInServer = true;
                }
            }
            catch
            {
                bgImgExistsInServer = false;
            }

            if (bgImgExistsInServer == true)
            {
                try
                {
                    HttpClient client = new HttpClient();

                    HttpResponseMessage message = await client.GetAsync(ImageURL);

                    StorageFile sampleFile = await artistImagesFolder.CreateFileAsync("artist_" + artistNameWithOutSpaces + ".jpg", CreationCollisionOption.ReplaceExisting);// this line throws an exception
                    byte[] file = await message.Content.ReadAsByteArrayAsync();

                    await FileIO.WriteBytesAsync(sampleFile, file);

                    BlurArtistImage(name);
                }
                catch
                {

                }
            }
        }

        public static async void BlurArtistImage(string artist)
        {
            try
            {
                string artistNameWithoutSpaces = StringHelper.RemoveSpecialChar(artist);
                StorageFolder coversFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Artists", CreationCollisionOption.OpenIfExists);
                IStorageItem storageItem = await coversFolder.TryGetItemAsync("artist_" + artistNameWithoutSpaces + ".jpg");
                StorageFile f = await coversFolder.CreateFileAsync("artist_" + artistNameWithoutSpaces + "_blur.jpg", CreationCollisionOption.ReplaceExisting);

                if (storageItem != null)
                {
                    StorageFile image = storageItem as StorageFile;

                    var imageData = await image.OpenReadAsync();
                    var decoder = await BitmapDecoder.CreateAsync(imageData);

                    IRandomAccessStream sx = await f.OpenAsync(FileAccessMode.ReadWrite);

                    var i = await decoder.GetPixelDataAsync();

                    var pixels = i.DetachPixelData();

                    // Useful for rendering in the correct DPI
                    var displayInformation = DisplayInformation.GetForCurrentView();

                    //var stream = new InMemoryRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, sx);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Premultiplied,
                                         (uint)decoder.PixelWidth,
                                         (uint)decoder.PixelHeight,
                                         displayInformation.RawDpiX,
                                         displayInformation.RawDpiY,
                                         pixels);

                    await encoder.FlushAsync();
                    sx.Seek(0);

                    var device = new CanvasDevice();
                    var bitmap = await CanvasBitmap.LoadAsync(device, sx);

                    var renderer = new CanvasRenderTarget(device,
                                                          bitmap.SizeInPixels.Width,
                                                          bitmap.SizeInPixels.Height, bitmap.Dpi);

                    using (var ds = renderer.CreateDrawingSession())
                    {
                        var blur = new GaussianBlurEffect();
                        blur.BlurAmount = 4.0f;
                        blur.Source = bitmap;
                        ds.DrawImage(blur);
                    }

                    sx.Seek(0);
                    await renderer.SaveAsync(sx, CanvasBitmapFileFormat.Jpeg);
                }
            }
            catch
            {

            }
        }

        public static bool IsDownloadEnabled
        {
            get
            {
                bool download;

                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Download"))
                    download = (bool)ApplicationData.Current.LocalSettings.Values["Download"];
                else
                {
                    ApplicationData.Current.LocalSettings.Values["Download"] = download = true;
                }

                if (download)
                {
                    var profile = NetworkInformation.GetInternetConnectionProfile();

                    // O USUÁRIO PERMITE BAIXAR EM CONEXÃO LIMITADA (REDE CELULAR)?
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("CelullarDownload"))
                    {
                        // SE NÃO, VERIFICA O TIPO DE CONEXÃO PORQUE O DOWNLOAD SÓ SERÁ FEITO EM OUTRO TIPO DE CONEXÃO
                        if ((bool)ApplicationData.Current.LocalSettings.Values["CelullarDownload"] == false)
                        {
                            if (profile.IsWwanConnectionProfile)
                                download = false;
                        }
                    }

                    //if (ApplicationData.Current.LocalSettings.Values.ContainsKey("DownloadConnection"))
                    //{
                    //    string type = ApplicationData.Current.LocalSettings.Values["DownloadConnection"].ToString();

                    //    if (type == "Wi-FiAndEthernet")
                    //    {
                    //        download = !profile.IsWwanConnectionProfile;
                    //    }
                    //    else if (type == "Wi-FiOnly")
                    //    {
                    //        download = profile.IsWlanConnectionProfile;
                    //    }
                    //    else if (type == "EthernetOnly")
                    //    {
                    //        download = !profile.IsWlanConnectionProfile && !profile.IsWwanConnectionProfile;
                    //    }
                    //    else if (type == "Cellular")
                    //        download = profile.IsWwanConnectionProfile;
                    //    else if (type == "Both")
                    //    {
                    //        download = true;
                    //    }

                    //}
                    //else
                    //{
                    //    ApplicationData.Current.LocalSettings.Values["DownloadConnection"] = "Wi-FiAndEthernet";

                    //    download = !profile.IsWwanConnectionProfile;
                    //}
                }

                return download;
            }
            set
            {
                ApplicationData.Current.RoamingSettings.Values["Download"] = value;
                ApplicationData.Current.LocalSettings.Values["DownloadPermission"] = true;
            }
        }
    }

    public class SaveCoverImageResult
    {
        public enum Result
        {
            Failed,
            Succeded
        }

        public string HexColor
        {
            get;
            set;
        }

        public Result OperationResult
        {
            get;
            set;
        }

        public SaveCoverImageResult(string hexColor, Result result)
        {
            this.HexColor = hexColor;
            this.OperationResult = result;
        }
    }
}
