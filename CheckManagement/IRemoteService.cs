using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TvpMain.CheckManagement
{
    public interface IRemoteService
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

        /// <summary>
        /// This method uploads a file.
        /// </summary>
        /// <param name="filename">The name the file will have in the remote repository.</param>
        /// <param name="filestream">The file as a <c>Stream</c>.</param>
        /// <returns>The <c>HttpStatusCode</c> returned by the remote repository.</returns>
        HttpStatusCode PutFileStream(string filename, Stream filestream);

        /// <summary>
        /// This method asynchonously uploads a file.
        /// </summary>
        /// <param name="filename">The name the file will have in the remote repository.</param>
        /// <param name="filestream">The file as a <c>Stream</c>.</param>
        /// <returns>An operation that results in the <c>HttpStatusCode</c> returned by the remote repository.</returns>
        Task<HttpStatusCode> PutFileStreamAsync(string filename, Stream filestream);

        /// <summary>
        /// This method deletes a file.
        /// </summary>
        /// <param name="filename">The name of the file to delete from the remote repository.</param>
        /// <returns>The <c>HttpStatusCode</c> returned by the remote repository.</returns>
        HttpStatusCode DeleteFile(string filename);

        /// <summary>
        /// This method asynchronously deletes a file.
        /// </summary>
        /// <param name="filename">The name of the file to delete from the remote repository.</param>
        /// <returns>An operation that results in the <c>HttpStatusCode</c> returned by the remote repository.</returns>
        Task<HttpStatusCode> DeleteFileAsync(string filename);
    }
}
