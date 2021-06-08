using PtxUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class saves, deletes, publishes, and synchronizes checks.
    /// </summary>
    public class CheckManager : ICheckManager
    {
        private readonly IRepository installedChecksRepository;
        private readonly IRepository locallyDevelopedChecksRepository;
        private readonly IRepository s3Repository;

        /// <summary>
        /// Default constructor for CheckManager
        /// </summary>
        public CheckManager()
        {
            installedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME));
            locallyDevelopedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME));
            s3Repository = new S3Repository();
        }

        /// <summary>
        /// This method synchronizes installed <c>CheckAndFixItem</c>s with the remote repository.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the result of the operation without applying it.</param>
        /// <returns>A key/value pair mapping of the result, with <c>CheckAndFixItem</c>s grouped by enumerated values.</returns>
        public virtual Dictionary<SynchronizationResultType, List<CheckAndFixItem>> SynchronizeInstalledChecks(bool dryRun = false)
        {
            List<CheckAndFixItem> newCheckAndFixItems = GetNewCheckAndFixItems();
            Dictionary<CheckAndFixItem, CheckAndFixItem> outdatedCheckAndFixItems = GetOutdatedCheckAndFixItems();
            List<CheckAndFixItem> deprecatedCheckAndFixItems = GetDeprecatedCheckAndFixItems();

            if (!dryRun)
            {
                foreach (CheckAndFixItem check in deprecatedCheckAndFixItems)
                {
                    UninstallCheckAndFixItem(check);
                }

                // Handle any situation where a newer check was removed from remote, but an older version still exists.
                newCheckAndFixItems = GetNewCheckAndFixItems();

                foreach (CheckAndFixItem check in newCheckAndFixItems)
                {
                    InstallCheckAndFixItem(check);
                }
                foreach (CheckAndFixItem check in outdatedCheckAndFixItems.Keys)
                {
                    UninstallCheckAndFixItem(check);
                }
                foreach (CheckAndFixItem check in outdatedCheckAndFixItems.Values)
                {
                    InstallCheckAndFixItem(check);
                }
            }

            return new Dictionary<SynchronizationResultType, List<CheckAndFixItem>>
            {
                [SynchronizationResultType.New] = newCheckAndFixItems,
                [SynchronizationResultType.Outdated] = outdatedCheckAndFixItems.Keys.ToList(),
                [SynchronizationResultType.Updated] = outdatedCheckAndFixItems.Values.ToList(),
                [SynchronizationResultType.Deprecated] = deprecatedCheckAndFixItems
            };
        }

        /// <summary>
        /// This method retrieves any checks which exist in the remote repository, but for which no version has been installed.
        /// </summary>
        /// <returns>A list of new <c>CheckAndFixItem</c>s which can be installed.</returns>
        internal virtual List<CheckAndFixItem> GetNewCheckAndFixItems()
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
        internal virtual List<CheckAndFixItem> GetDeprecatedCheckAndFixItems()
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
        /// This method locally installs a <c>CheckAndFixItem</c> from a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to be installed.</param>
        internal virtual void InstallCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            installedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method saves a new, or modified, locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        public virtual void SaveCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);

            // Remove previous versions of the item before saving a new one.
            foreach (CheckAndFixItem check in GetSavedCheckAndFixItems().Where(check => check.Name == item.Name).ToList())
                DeleteCheckAndFixItem(check);

            locallyDevelopedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method uninstalls a <c>CheckAndFixItem</c> that was installed from a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to uninstall.</param>
        internal virtual void UninstallCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            installedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method deletes a locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to delete locally.</param>
        public virtual void DeleteCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            locallyDevelopedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method publishes a locally-developed <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            string filename = GetCheckAndFixItemFilename(item);
            s3Repository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method gets the <c>CheckAndFixItem</c>s available in the remote repository.
        /// </summary>
        /// <returns>The <c>CheckAndFixItem</c>s which are available in the remote repository.</returns>
        public virtual List<CheckAndFixItem> GetAvailableCheckAndFixItems()
        {
            return s3Repository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetInstalledCheckAndFixItems()
        {
            return installedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of locally-developed and saved <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetSavedCheckAndFixItems()
        {
            return locallyDevelopedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method find the <c>CheckAndFixItem</c>s that have an updated version available in the remote repository.
        /// </summary>
        /// <returns>A dictionary comprising of {KEY: the current <c>CheckAndFixItem</c>} and {VALUE: the updated <c>CheckAndFixItem</c> available in the remote repository}.</returns>
        internal virtual Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems()
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
        /// <param name="name">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <param name="version">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        public static string GetCheckAndFixItemFilename(string name, string version)
        {
            return $"{name.ConvertToTitleCase().Replace(" ", String.Empty)}-{version.Trim()}.{MainConsts.CHECK_FILE_EXTENSION}";
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> for which to produce a filename.</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        private string GetCheckAndFixItemFilename(CheckAndFixItem item)
        {
            return GetCheckAndFixItemFilename(item.Name, item.Version);
        }

        /// <summary>
        /// This method compares two checks and determines whether the candidate is an updated version of the original.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="candidate"></param>
        /// <returns>True if this candidate is greater than the original</returns>
        internal virtual Boolean IsNewVersion(CheckAndFixItem original, CheckAndFixItem candidate)
        {
            return String.Equals(candidate.Name, original.Name) &&
                    Version.Parse(candidate.Version) > Version.Parse(original.Version);
        }

        /// <summary>
        /// Get the local check folder path as a string for the editor to open files there.
        /// </summary>
        /// <returns>The local check folder path as a string for the editor to open files there</returns>
        public string GetLocalRepoDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME);
        }

        /// <summary>
        /// Get the local check folder path as a string for the editor to open files there.
        /// </summary>
        /// <returns>The local check folder path as a string for the editor to open files there</returns>
        public static string GetLocalRepoDirectory(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME);
        }
    }
}
