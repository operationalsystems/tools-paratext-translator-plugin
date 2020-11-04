using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManager
{
    public class CheckManager : ICheckManager
    {
        private readonly FolderRepository folderRepository;

        private readonly S3Repository s3Repository;

        public CheckManager()
        {
            folderRepository = new FolderRepository();
            s3Repository = new S3Repository();
        }

        public List<CheckAndFixItem> GetLocalCheckAndFixItems()
        {
            return folderRepository.GetCheckAndFixItems();
        }

        public List<CheckAndFixItem> GetRemoteCheckAndFixItems()
        {
            return s3Repository.GetCheckAndFixItems();
        }

        public void InstallCheckAndFixItem(CheckAndFixItem item)
        {
            folderRepository.AddCheckAndFixItem(item);
        }

        public Task InstallCheckAndFixItemAsync(CheckAndFixItem item)
        {
            return folderRepository.AddCheckAndFixItemAsync(item);
        }

        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            s3Repository.AddCheckAndFixItem(item);
        }

        public Task PublishCheckAndFixItemAsync(CheckAndFixItem item)
        {
            return s3Repository.AddCheckAndFixItemAsync(item);
        }
    }
}
