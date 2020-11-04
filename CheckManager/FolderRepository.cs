using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManager
{
    public class FolderRepository : IRepository
    {
        public virtual string FolderPath { get; private set; } = Path.Combine(Directory.GetCurrentDirectory(), MainConsts.CHECK_FOLDER_NAME);


        public void AddCheckAndFixItem(CheckAndFixItem item)
        {
            throw new NotImplementedException();
        }

        public Task AddCheckAndFixItemAsync(CheckAndFixItem item)
        {
            throw new NotImplementedException();
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
        public void VerifyFolderPath()
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}
