using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    interface ICheckManager
    {
        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that have been installed from a remote repository.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public List<CheckAndFixItem> GetInstalledCheckAndFixItems();

        /// <summary>
        /// This method synchronizes the locally-installed <c>CheckAndFixItem</c> repository with the remote repository by installing/updating/removing <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <param name="dryRun">(optional) If true, returns the checks that would be installed/updated/removed, without affecting the local repository.</param>
        /// <returns></returns>
        public Dictionary<string, List<CheckAndFixItem>> SynchronizeInstalledChecks(bool dryRun = false);

        /// <summary>
        /// This method publishes a <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously publishes a <c>CheckAndFixItem</c> to a remote repository.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to publish to the remote repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task PublishCheckAndFixItemAsync(CheckAndFixItem item);

        /// <summary>
        /// This method returns a list of locally-developed and saved <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of saved <c>CheckAndFixItem</c>s.</returns>
        public List<CheckAndFixItem> GetSavedCheckAndFixItems();

        /// <summary>
        /// This method saves a new or modified local <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        public void SaveCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously saves a new or modified local <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to save locally.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task SaveCheckAndFixItemAsync(CheckAndFixItem item);

        /// <summary>
        /// This method deletes a local <c>CheckAndFixItem</c>.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> to delete locally.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public void DeleteCheckAndFixItem(CheckAndFixItem item);
       
    }
}
