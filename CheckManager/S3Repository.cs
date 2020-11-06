using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManager
{
    public class S3Repository : IRepository
    {
        private S3Service service = new S3Service();

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            service.PutFileStream(filename, item.WriteToXmlStream());
        }

        public async Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) new ArgumentNullException(nameof(filename));

            await service.PutFileStreamAsync(filename, item.WriteToXmlStream());
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = service.ListAllFiles();
            foreach (string file in filenames)
            {
                using Stream fileStream = service.GetFileStream(file);
                CheckAndFixItem checkAndFixItem = ReadCheckAndFixItemFromStream(fileStream);
                if (checkAndFixItem != null) checkAndFixItems.Add(checkAndFixItem);
            }

            return checkAndFixItems;
        }

        public async Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            List<String> filenames = await service.ListAllFilesAsync();
            foreach (string file in filenames)
            {
                using Stream fileStream = await service.GetFileStreamAsync(file);
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
            try
            {
                checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(stream);
            }
            catch (Exception)
            {
                //TODO: log the error but continue
            }
            return checkAndFixItem;
        }
    }
}
