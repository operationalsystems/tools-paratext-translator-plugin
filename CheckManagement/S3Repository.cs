using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    public class S3Repository : IRepository
    {
        public virtual IRemoteService Service { get; set; } = new S3Service();

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            Service.PutFileStream(filename, item.WriteToXmlStream());
        }

        public async Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            await Service.PutFileStreamAsync(filename, item.WriteToXmlStream());
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = Service.ListAllFiles();
            foreach (string file in filenames)
            {
                using Stream fileStream = Service.GetFileStream(file);
                CheckAndFixItem checkAndFixItem = ReadCheckAndFixItemFromStream(fileStream);
                if (checkAndFixItem != null) checkAndFixItems.Add(checkAndFixItem);
            }

            return checkAndFixItems;
        }

        public async Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = await Service.ListAllFilesAsync();
            foreach (string file in filenames)
            {
                using Stream fileStream = await Service.GetFileStreamAsync(file);
                CheckAndFixItem checkAndFixItem = ReadCheckAndFixItemFromStream(fileStream);
                if (checkAndFixItem != null) checkAndFixItems.Add(checkAndFixItem);
            }

            return checkAndFixItems;
        }

        /// <summary>
        /// This loads a <c>CheckAndFixItem</c> from a <c>Stream</c>, guarding against invalid files.
        /// </summary>
        /// <param name="stream">The <c>Stream</c> of a file representing a <c>CheckAndFixItem</c>.</param>
        /// <returns></returns>
        private CheckAndFixItem ReadCheckAndFixItemFromStream(Stream stream)
        {
            CheckAndFixItem checkAndFixItem = null;
            checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(stream);
            
            return checkAndFixItem;
        }

        public void RemoveCheckAndFixItem(string filename)
        {
            Service.DeleteFile(filename);
        }

        public Task RemoveCheckAndFixItemAsync(string filename)
        {
            return Service.DeleteFileAsync(filename);
        }
    }
}
