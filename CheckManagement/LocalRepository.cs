using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class works with a local, file-based repository for checks and fixes.
    /// </summary>
    public class LocalRepository : IRepository
    {
        /// <summary>
        /// The path where checks should be persisted.
        /// </summary>
        private string FolderPath { get; }

        public LocalRepository(string folderPath)
        {
            FolderPath = folderPath;
        }

        public void AddCheckAndFixItem(string filename, CheckAndFixItem item)
        {
            if (string.IsNullOrEmpty(filename)) throw new ArgumentNullException(nameof(filename));

            VerifyFolderPath();
            var filePath = Path.Combine(FolderPath, filename);

            try
            {
                item.SaveToXmlFile(filePath);
            }
            catch (Exception e)
            {
                throw new FileWriteException($"There was a problem writing to '{filePath}'.", e.InnerException);
            }
        }

        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item)
        {
            return Task.Run(() => AddCheckAndFixItem(filename, item));
        }

        public void RemoveCheckAndFixItem(string filename)
        {
            var filePath = Path.Combine(FolderPath, filename);
            if (!File.Exists(filePath)) return;

            File.Delete(filePath);
        }

        public List<CheckAndFixItem> GetCheckAndFixItems()
        {
            var checkAndFixItems = new List<CheckAndFixItem>();
            VerifyFolderPath();

            var checkFiles = Directory.GetFiles(FolderPath, "*.xml");
            foreach (var checkFilePath in checkFiles)
            {
                try
                {
                    var checkAndFixItem = CheckAndFixItem.LoadFromXmlFile(checkFilePath);
                    checkAndFixItems.Add(checkAndFixItem);
                }
                catch (Exception e)
                {
                    throw new FileLoadException($"Unable to load '{checkFilePath}'.", e.InnerException);
                }
            }

            return checkAndFixItems;
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
