using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace AudictiveMusicUWP.Helpers
{
    internal static class StorageHelper
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
    }
}
