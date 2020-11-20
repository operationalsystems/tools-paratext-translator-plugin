using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    public class S3Repository : IRepository
    {
        private S3Service service = new S3Service();

        public virtual S3Service GetService()
        {
            return service;
        }

        private void SetService(S3Service value)
        {
            service = value;
        }

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            GetService().PutFileStream(filename, item.WriteToXmlStream());
        }

        public async Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            await GetService().PutFileStreamAsync(filename, item.WriteToXmlStream());
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = GetService().ListAllFiles();
            foreach (string file in filenames)
            {
                using Stream fileStream = GetService().GetFileStream(file);
                CheckAndFixItem checkAndFixItem = ReadCheckAndFixItemFromStream(fileStream);
                if (checkAndFixItem != null) checkAndFixItems.Add(checkAndFixItem);
            }

            return checkAndFixItems;
        }

        public async Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = await GetService().ListAllFilesAsync();
            foreach (string file in filenames)
            {
                using Stream fileStream = await GetService().GetFileStreamAsync(file);
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
            GetService().DeleteFile(filename);
        }

        public Task RemoveCheckAndFixItemAsync(string filename)
        {
            return GetService().DeleteFileAsync(filename);
        }
    }
}
