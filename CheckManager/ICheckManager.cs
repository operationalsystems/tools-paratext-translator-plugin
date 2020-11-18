using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManager
{
    interface ICheckManager
    {
        /// <summary>
        /// This method returns a list of <c>CheckAndFixItem</c>s that can be installed to the local system.
        /// </summary>
        /// <returns>A list of <c>CheckAndFixItem</c>s that can be installed to the local system.</returns>
        public List<CheckAndFixItem> GetAvailableCheckAndFixItems();

        /// <summary>
        /// This method returns a list of installed <c>CheckAndFixItem</c>s that have updates available.
        /// </summary>
        /// <returns>A key-value pair of installed and available check versions.</returns>
        public Dictionary<CheckAndFixItem, CheckAndFixItem> GetOutdatedCheckAndFixItems();

        /// <summary>
        /// This method returns a list of installed <c>CheckAndFixItem</c>s.
        /// </summary>
        /// <returns>A list of installed <c>CheckAndFixItem</c>s.</returns>
        public List<CheckAndFixItem> GetInstalledCheckAndFixItems();

        /// <summary>
        /// This method gets check and fix items from a remote repository.
        /// </summary>
        /// <returns>A list of check and fix items that are available in a remote repository.</returns>
        public List<CheckAndFixItem> GetRemoteCheckAndFixItems();

        /// <summary>
        /// This method gets check and fix items that are locally installed.
        /// </summary>
        /// <returns>A list of check and fix items that are locally installed.</returns>
        public List<CheckAndFixItem> GetLocalCheckAndFixItems();

        /// <summary>
        /// This method publishes a check and fix item to a remote repository.
        /// </summary>
        /// <param name="item">The check and fix item to publish to the remote repository.</param>
        public void PublishCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously publishes a check and fix item to a remote repository.
        /// </summary>
        /// <param name="item">The check and fix item to publish to the remote repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task PublishCheckAndFixItemAsync(CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously installs a check and fix item from a remote repository.
        /// </summary>
        /// <param name="item">The check and fix item to install from the remote repository.</param>
        public void InstallCheckAndFixItem(CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously installs a check and fix item from a remote repository.
        /// </summary>
        /// <param name="item">The check and fix item to install from the remote repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task InstallCheckAndFixItemAsync(CheckAndFixItem item);
    }
}
