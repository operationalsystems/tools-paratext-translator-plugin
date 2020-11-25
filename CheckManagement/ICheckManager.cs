using System.Collections.Generic;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This enum defines the results that can be expected from a synchronization operation.
    /// </summary>
    public enum SynchronizationResultType : int
    {
        New,
        Outdated,
        Updated,
        Deprecated
    }

    /// <summary>
    /// This interface defines a class that saves, deletes, publishes, and synchronizes checks.
    /// "Installed/Uninstalled" <c>CheckAndFixItem</c>s are those which have been persisted to the local filesystem from a remote repository.
    /// "Saved/Deleted" <c>CheckAndFixItem</c>s are those which have been developed locally and only exist locally.
    /// </summary>
    interface ICheckManager
    {
        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public List<CheckAndFixItem> GetInstalledCheckAndFixItems();

        /// <summary>
        /// This method synchronizes installed <c>CheckAndFixItem</c>s with the remote repository.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the result of the operation without applying it.</param>
        /// <returns>A key/value pair mapping of the result, with <c>CheckAndFixItem</c>s grouped by enumerated values.</returns>
        public Dictionary<SynchronizationResultType, List<CheckAndFixItem>> SynchronizeInstalledChecks(bool dryRun = false);

        /// <summary>
        /// This method publishes a locally-developed <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method returns a list of locally-developed and saved <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public List<CheckAndFixItem> GetSavedCheckAndFixItems();

        /// <summary>
        /// This method saves a new, or modified, locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        public void SaveCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method deletes a locally-developed <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to delete locally.</param>
        public void DeleteCheckAndFixItem(CheckAndFixItem item);
    }
}
