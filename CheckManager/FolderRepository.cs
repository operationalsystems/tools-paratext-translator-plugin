using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManager
{
    /// <summary>
    /// This class works with a local, file-based repository for checks and fixes.
    /// </summary>
    public class FolderRepository : IRepository
    {
        /// <summary>
        /// The filesystem path where checks are located.
        /// </summary>
        public virtual string FolderPath { get; private set; } = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.CHECK_FOLDER_NAME);

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (String.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            string filePath = Path.Combine(FolderPath, filename);

            try
            {
                item.SaveToXmlFile(filePath);
            }
            catch (Exception e)
            {
                new FileWriteException($"There was a problem writing to '{filePath}'.", e.InnerException);
            }
        }

        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            return Task.Run(() => AddCheckAndFixItem(filename, item));
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            List<CheckAndFixItem> checkAndFixItems = new List<CheckAndFixItem>();

            VerifyFolderPath();
            string[] checkFiles = Directory.GetFiles(FolderPath, "*.xml");
            foreach (string checkFilePath in checkFiles)
            {
                try
                {
                    CheckAndFixItem checkAndFixItem = CheckAndFixItem.LoadFromXmlFile(checkFilePath);
                    checkAndFixItems.Add(checkAndFixItem);
                }
                catch (Exception e)
                {
                    new FileLoadException($"Unable to load '{checkFilePath}'.", e.InnerException);
                }
            }

            return checkAndFixItems;
        }

        public Task<List<CheckAndFixItem>> GetCheckAndFixItemsAsync()
        {
            return Task.Run<List<CheckAndFixItem>>(GetCheckAndFixItems);
        }

        /// <summary>
        /// This method ensures that the folder exists.
        /// </summary>
        private void VerifyFolderPath()
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}
