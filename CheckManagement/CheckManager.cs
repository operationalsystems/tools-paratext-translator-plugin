/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using PtxUtils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using TvpMain.Check;
using TvpMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class saves, deletes, publishes, and synchronizes checks.
    /// </summary>
    public class CheckManager : ICheckManager
    {
        private readonly IRepository _installedChecksRepository;
        private readonly IRepository _locallyDevelopedChecksRepository;
        private readonly IRepository _s3Repository;
        private static readonly string SyncStatusFileName = $"{Directory.GetCurrentDirectory()}\\{MainConsts.TVP_FOLDER_NAME}\\{MainConsts.LAST_SYNC_FILE_NAME}";

        /// <summary>
        /// Keeps track of whether a sync has run during this ParaText session.
        /// </summary>
        public static bool HasSyncRun
        {
            get
            {
                var foundText = "";
                if (File.Exists(SyncStatusFileName))
                {
                    foundText = File.ReadAllLines(SyncStatusFileName)[0];
                }

                return string.Equals(foundText,System.Diagnostics.Process.GetProcessesByName("Paratext")[0].Id.ToString());
            }
            set
            {
                var output = value ? new []
                {
                    System.Diagnostics.Process.GetProcessesByName("Paratext")[0].Id.ToString(), DateTime.Now.ToString(CultureInfo.CurrentCulture)
                } : new string[]{};
                File.WriteAllLines(SyncStatusFileName, output);
            }
        }

        /// <summary>
        /// The last time synchronization completed.
        /// </summary>
        public static DateTime? LastSyncTime
        {
            get
            {
                if (!File.Exists(SyncStatusFileName)) return null;
                var syncStatus = File.ReadAllLines(SyncStatusFileName);
                if (syncStatus.Length < 2) return null; 
                DateTime.TryParse(syncStatus[1], out var lastRunDate);

                return lastRunDate;
            }
        }

        /// <summary>
        /// Default constructor for CheckManager
        /// </summary>
        public CheckManager()
        {
            _installedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME));
            _locallyDevelopedChecksRepository = new LocalRepository(Path.Combine(Directory.GetCurrentDirectory(), MainConsts.LOCAL_CHECK_FOLDER_NAME));
            _s3Repository = new S3Repository();
        }

        /// <summary>
        /// This method synchronizes installed <c>CheckAndFixItem</c>s with the remote repository.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the result of the operation without applying it.</param>
        /// <returns>A key/value pair mapping of the result, with <c>CheckAndFixItem</c>s grouped by enumerated values.</returns>
        public virtual Dictionary<SynchronizationResultType, List<CheckAndFixItem>> SynchronizeInstalledChecks(bool dryRun = false)
        {
            var newCheckAndFixItems = GetNewCheckAndFixItems();
            var outdatedCheckAndFixItems = GetOutdatedCheckAndFixItems();
            var deprecatedCheckAndFixItems = GetDeprecatedCheckAndFixItems();

            if (dryRun)
            {
                return new Dictionary<SynchronizationResultType, List<CheckAndFixItem>>
                {
                    [SynchronizationResultType.New] = newCheckAndFixItems,
                    [SynchronizationResultType.Outdated] = outdatedCheckAndFixItems.Keys.ToList(),
                    [SynchronizationResultType.Updated] = outdatedCheckAndFixItems.Values.ToList(),
                    [SynchronizationResultType.Deprecated] = deprecatedCheckAndFixItems
                };
            }

            foreach (var check in deprecatedCheckAndFixItems)
            {
                UninstallCheckAndFixItem(check);
            }

            // Handle any situation where a newer check was removed from remote, but an older version still exists.
            newCheckAndFixItems = GetNewCheckAndFixItems();

            foreach (var check in newCheckAndFixItems)
            {
                InstallCheckAndFixItem(check);
            }
            foreach (var check in outdatedCheckAndFixItems.Keys)
            {
                UninstallCheckAndFixItem(check);
            }
            foreach (var check in outdatedCheckAndFixItems.Values)
            {
                InstallCheckAndFixItem(check);
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
            var availableChecks = GetAvailableCheckAndFixItems();
            var localChecks = from local in GetInstalledCheckAndFixItems()
                              select new
                              {
                                  local.Name
                              };
            var newChecks = availableChecks.Where(check =>
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
            var installedChecks = GetInstalledCheckAndFixItems();
            var remoteChecks = from remote in GetAvailableCheckAndFixItems()
                               select new
                               {
                                   remote.Name
                               };
            var deprecated = installedChecks.Where(check =>
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
            var filename = GetCheckAndFixItemFilename(item);
            _installedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method saves a new, or modified, locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        public virtual void SaveCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);

            // Remove previous versions of the item before saving a new one.
            foreach (var check in GetSavedCheckAndFixItems()
                .Where(check => check.Name == item.Name).ToList())
                DeleteCheckAndFixItem(check);
            _locallyDevelopedChecksRepository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method uninstalls a <c>CheckAndFixItem</c> that was installed from a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to uninstall.</param>
        internal virtual void UninstallCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _installedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method deletes a locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to delete locally.</param>
        public virtual void DeleteCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _locallyDevelopedChecksRepository.RemoveCheckAndFixItem(filename);
        }

        /// <summary>
        /// This method publishes a locally-developed <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item)
        {
            var filename = GetCheckAndFixItemFilename(item);
            _s3Repository.AddCheckAndFixItem(filename, item);
        }

        /// <summary>
        /// This method gets the <c>CheckAndFixItem</c>s available in the remote repository.
        /// </summary>
        /// <returns>The <c>CheckAndFixItem</c>s which are available in the remote repository.</returns>
        public virtual List<CheckAndFixItem> GetAvailableCheckAndFixItems()
        {
            return _s3Repository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetInstalledCheckAndFixItems()
        {
            return _installedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method returns a list of locally-developed and saved <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public virtual List<CheckAndFixItem> GetSavedCheckAndFixItems()
        {
            return _locallyDevelopedChecksRepository.GetCheckAndFixItems();
        }

        /// <summary>
        /// This method find the <c>CheckAndFixItem</c>s that have an updated version available in the remote repository.
        /// </summary>
        /// <returns>A dictionary comprising of {KEY: the current <c>CheckAndFixItem</c>} and {VALUE: the updated <c>CheckAndFixItem</c> available in the remote repository}.</returns>
        internal virtual Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems()
        {
            var availableCheckAndFixItems = GetAvailableCheckAndFixItems();
            availableCheckAndFixItems.Sort((x, y) =>
                new Version(y.Version).CompareTo(new Version(x.Version)));
            var installedCheckAndFixItems = GetInstalledCheckAndFixItems();
            var outdatedCheckAndFixItems = new Dictionary<CheckAndFixItem, CheckAndFixItem>();

            installedCheckAndFixItems.ForEach(installedCheck =>
            {
                var availableUpdate = availableCheckAndFixItems.Find(availableCheck =>
                    IsNewVersion(installedCheck, availableCheck));
                if (availableUpdate is { }) outdatedCheckAndFixItems.Add(installedCheck, availableUpdate);
            });

            return outdatedCheckAndFixItems;
        }

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>. 
        /// </summary>
        /// <param name="name">The <c>CheckAndFixItem</c> name</param>
        /// <param name="version">The <c>CheckAndFixItem</c> version</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        public string GetCheckAndFixItemFilename(string name, string version)
        {
            return $"{name.ConvertToFileName()}-{version.Trim()}.{MainConsts.CHECK_FILE_EXTENSION}";
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
        internal virtual bool IsNewVersion(CheckAndFixItem original, CheckAndFixItem candidate)
        {
            return string.Equals(candidate.Name, original.Name) &&
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
        /// Get the path to the folder where remote checks are installed.
        /// </summary>
        /// <returns>The path to the folder where remote checks are installed.</returns>
        public string GetInstalledChecksDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), MainConsts.INSTALLED_CHECK_FOLDER_NAME);
        }
    }
}
