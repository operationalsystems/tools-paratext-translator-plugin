/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Generic;
using System.Threading.Tasks;
using TvpMain.Check;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This interface defines a class that manages a repository of checks.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// This method gets <c>CheckAndFixItem</c>s from a repository.
        /// </summary>
        /// <returns>A list of check and fix items that are available in the repository.</returns>
        public List<CheckAndFixItem> GetCheckAndFixItems();

        /// <summary>
        /// This method adds a <c>CheckAndFixItem</c> to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        public void AddCheckAndFixItem(string filename, CheckAndFixItem item);

        /// <summary>
        /// This method asynchronously adds a <c>CheckAndFixItem</c> to a repository.
        /// </summary>
        /// <param name="filename">The filename to use for the item in the repository.</param>
        /// <param name="item">The check and fix item to add to the repository.</param>
        /// <returns>A task representing the result of the operation.</returns>
        public Task AddCheckAndFixItemAsync(string filename, CheckAndFixItem item);

        /// <summary>
        /// This method removes a <c>CheckAndFixItem</c> from a repository.
        /// </summary>
        /// <param name="filename">The <c>CheckAndFixItem</c> to remove.</param>
        public void RemoveCheckAndFixItem(string filename);
    }
}
