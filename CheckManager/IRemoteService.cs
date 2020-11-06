using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.CheckManager
{
    interface IRemoteService
    {
        /// <summary>
        /// This method retrieves a file as a <c>Stream</c>.
        /// </summary>
        /// <param name="filename">The filename to retrieve.</param>
        /// <returns>A <c>Stream</c> containing the file data.</returns>
        public Stream GetFileStream(string filename);

        /// <summary>
        /// This method asynchronously retrieves a file as a <c>Stream</c>.
        /// </summary>
        /// <param name="filename">The filename to retrieve.</param>
        /// <returns>An operation that results in a <c>Stream</c> containing the file data.</returns>
        public Task<Stream> GetFileStreamAsync(string key);

        /// <summary>
        /// This method retrieves all the available filenames.
        /// </summary>
        /// <returns>The available filenames.</returns>
        public List<string> ListAllFiles();

        /// <summary>
        /// This method asynchronously retrieves all the available filenames.
        /// </summary>
        /// <returns>An operation that results in the available filenames.</returns>
        public Task<List<string>> ListAllFilesAsync();
    }
}
