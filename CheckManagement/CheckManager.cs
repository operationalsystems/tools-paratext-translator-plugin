using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    public class CheckManager : ICheckManager
    {
        private readonly IRepository installedChecksRepository;
        private readonly IRepository locallyDevelopedChecksRepository;
        private readonly IRepository s3Repository;

        public CheckManager()
        {
            installedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME));
            locallyDevelopedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME));
            s3Repository = new S3Repository();
        }

        public virtual Dictionary<string, List<CheckAndFixItem>> SynchronizeInstalledChecks(bool dryRun = false)
        {
            List<CheckAndFixItem> newCheckAndFixItems = GetNewCheckAndFixItems();
            Dictionary<CheckAndFixItem, CheckAndFixItem> outdatedCheckAndFixItems = GetOutdatedCheckAndFixItems();
            List<CheckAndFixItem> deprecatedCheckAndFixItems = GetDeprecatedCheckAndFixItems();

            if (!dryRun)
            {
                foreach (CheckAndFixItem check in newCheckAndFixItems)
                    InstallCheckAndFixItem(check);
                foreach (CheckAndFixItem check in outdatedCheckAndFixItems.Keys)
                    UninstallCheckAndFixItem(check);
                foreach (CheckAndFixItem check in outdatedCheckAndFixItems.Values)
                    InstallCheckAndFixItem(check);
                foreach (CheckAndFixItem check in deprecatedCheckAndFixItems)
                    UninstallCheckAndFixItem(check);
            }

            return new Dictionary<string, List<CheckAndFixItem>>
            {
                ["new"] = newCheckAndFixItems,
                ["outdated"] = outdatedCheckAndFixItems.Keys.ToList(),
                ["updated"] = outdatedCheckAndFixItems.Values.ToList(),
                ["deprecated"] = deprecatedCheckAndFixItems
            };
        }

        /// <summary>
        /// This method retrieves any checks which exist in the remote repository, but for which no version has been installed.
        /// </summary>
        /// <returns>A list of new <c>CheckAndFixItem</c>s which can be installed.</returns>
        public virtual List<CheckAndFixItem> GetNewCheckAndFixItems()
        {
            List<CheckAndFixItem> availableChecks = GetAvailableCheckAndFixItems();
            var localChecks = from local in GetInstalledCheckAndFixItems()
                               select new
                               {
                                   local.Name,
                               };
            List<CheckAndFixItem> newChecks = availableChecks.Where(check =>
            {
                var installed = new { check.Name };
                return !localChecks.Contains(installed);
            }).ToList();
            return newChecks;
        }

        /// <summary>
        /// This method determines which <c>CheckAndFixItem</c>s have been installed, but are no longer available in the remote repository.
        /// </summary>
        /// <returns>A list of deprecated <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetDeprecatedCheckAndFixItems()
        {
            List<CheckAndFixItem> installedChecks = GetInstalledCheckAndFixItems();
            var remoteChecks = from remote in GetAvailableCheckAndFixItems()
                               select new
                               {
                                   remote.Name,
                               };
            List<CheckAndFixItem> deprecated = installedChecks.Where(check =>
            {
                var installed = new { check.Name };
                return !remoteChecks.Contains(installed);
            }).ToList();
            return deprecated;
        }

        /// <summary>
        /// This method installs a <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to be installed.</param>
        public virtual void InstallCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            installedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method asynchronously installs a <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to be installed.</param>>
        /// <returns>A task representing the result of the operation.</returns>
        public Task InstallCheckAndFixItemAsync(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            return installedChecksRepository.AddCheckAndFixItemAsync(filename, item);
        }

        public virtual void SaveCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            locallyDevelopedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        public Task SaveCheckAndFixItemAsync(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            return locallyDevelopedChecksRepository.AddCheckAndFixItemAsync(filename, item);
        }

        /// <summary>
        /// This method uninstalls a <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to uninstall.</param>
        public virtual void UninstallCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            installedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        public virtual void DeleteCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            locallyDevelopedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            s3Repository.AddCheckAndFixItem(filename, item);
        }

        public Task PublishCheckAndFixItemAsync(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            return s3Repository.AddCheckAndFixItemAsync(filename, item);
        }

        /// <summary>
        /// This method gets the <c>CheckAndFixItem</c>s available in the remote repository.
        /// </summary>
        /// <returns>The <c>CheckAndFixItem</c>s which are available in the remote repository.</returns>
        public virtual List<CheckAndFixItem> GetAvailableCheckAndFixItems()
        {
            return s3Repository.GetCheckAndFixItems();
        }

        public virtual List<CheckAndFixItem> GetInstalledCheckAndFixItems()
        {
            return installedChecksRepository.GetCheckAndFixItems();
        }

        public virtual List<CheckAndFixItem> GetSavedCheckAndFixItems()
        {
            return locallyDevelopedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method find the <c>CheckAndFixItem</c>s that have an updated version available in the remote repository.
        /// </summary>
        /// <returns>A dictionary comprising of {KEY: the current <c>CheckAndFixItem</c>} and {VALUE: the updated <c>CheckAndFixItem</c> available in the remote repository}.</returns>
        public virtual Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems()
        {
            List<CheckAndFixItem> availableCheckAndFixItems = GetAvailableCheckAndFixItems();
            availableCheckAndFixItems.Sort((x, y) => new Version(y.Version).CompareTo(new Version(x.Version)));
            List<CheckAndFixItem> installedCheckAndFixItems = GetInstalledCheckAndFixItems();
            Dictionary<CheckAndFixItem, CheckAndFixItem> outdatedCheckAndFixItems = new Dictionary<CheckAndFixItem, CheckAndFixItem>();

            installedCheckAndFixItems.ForEach(installedCheck =>
            {
                CheckAndFixItem availableUpdate = availableCheckAndFixItems.Find(availableCheck => IsNewVersion(installedCheck, availableCheck));
                if (availableUpdate != default(CheckAndFixItem)) outdatedCheckAndFixItems.Add(installedCheck, availableUpdate);
            });

            return outdatedCheckAndFixItems;
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        private string GetCheckAndFixItemFilename(CheckAndFixItem item)
        {
            return $"{item.Name.ConvertToTitleCase().Replace(" ", String.Empty)}-{item.Version.Trim()}.{MainConsts.CHECK_FILE_EXTENSION}";
        }

        /// <summary>
        /// This method compares two checks and determines whether the candidate is an updated version of the original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public virtual Boolean IsNewVersion(CheckAndFixItem original, CheckAndFixItem candidate)
        {
            return String.Equals(candidate.Name, original.Name) &&
                    Version.Parse(candidate.Version) > Version.Parse(original.Version);
        }
    }
}
