using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace ClassLibrary.Helpers
{
    public static class StorageHelper
    {
        public static async Task<int> CalculateSize(this StorageFolder folder)
        {
            // Query all files in the folder. Make sure to add the CommonFileQuery
            // So that it goes through all sub-folders as well
            var folders = folder.CreateFileQuery(CommonFileQuery.OrderByName);

            // Await the query, then for each file create a new Task which gets the size
            var fileSizeTasks = (await folders.GetFilesAsync()).Select(async file => (await file.GetBasicPropertiesAsync()).Size);

            // Wait for all of these tasks to complete. WhenAll thankfully returns each result
            // as a whole list
            var sizes = await Task.WhenAll(fileSizeTasks);

            // Sum all of them up. You have to convert it to a long because Sum does not accept ulong.
            long size = sizes.Sum(l => (long)l);

            double d = (Convert.ToDouble(size) / 1024) / 1024;
            return Convert.ToInt32(d);
        }

        public static async Task<IReadOnlyList<IStorageItem>> ReadFolder(StorageFolder folder)
        {
            var items = await folder.GetItemsAsync();
            return items;
        }

        //private static readonly Dictionary<string, string> SupportedMediaFiles = new Dictionary<string, string>();

        //private static readonly string[] SupportedMimeTypes =
        //{
        //    "audio/aac",
        //    "audio/flac",
        //    "audio/x-flac",
        //    "audio/m4a",
        //    "audio/mp4",
        //    "video/mp4",
        //    "audio/mpeg",
        //    "audio/wav",
        //    "audio/x-ms-wma"
        //};

        public static readonly string[] SupportedFileFormats = { ".aac", ".flac", ".m4a", ".mp3", ".mp4", ".wav", ".wma" };

        public static bool IsMusicFile(string fileName)
        {
            string extension;

            if (fileName.Contains('.') == false)
                return false;

            var s = fileName.Split('.');
            extension = s.Last();

            return SupportedFileFormats.Contains("." + extension);
            //return fileName.EndsWith(".aac") || fileName.EndsWith(".mp3") || fileName.EndsWith(".wma") || fileName.EndsWith(".m4a") || fileName.EndsWith(".flac") || fileName.EndsWith(".mp4");
            //return true;
        }

        /// <summary>
        /// Returns a list of music files in the user's music library
        /// </summary>
        /// <returns></returns>
        public static async Task<IReadOnlyList<StorageFile>> ScanFolder(StorageFolder folder)
        {
            QueryOptions options = new QueryOptions();
            foreach (string type in SupportedFileFormats)
            {
                options.FileTypeFilter.Add(type);
            }
            options.FolderDepth = FolderDepth.Deep;

            return await folder.CreateFileQueryWithOptions(options).GetFilesAsync();
        }
    }
}
