﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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

        /// <summary>
        /// Get the local check folder path as a string for the editor to open files there.
        /// </summary>
        /// <returns>The local check folder path as a string for the editor to open files there</returns>
        public string GetLocalRepoDirectory();

        /// <summary>
        /// Get the path to the folder where remote checks are installed.
        /// </summary>
        /// <returns>The path to the folder where remote checks are installed.</returns>
        public string GetInstalledChecksDirectory();

        /// <summary>
        /// This method creates a filename for the provided <c>CheckAndFixItem</c>. 
        /// </summary>
        /// <param name="name">The <c>CheckAndFixItem</c> name</param>
        /// <param name="version">The <c>CheckAndFixItem</c> version</param>
        /// <returns>The filename produced for the provided <c>CheckAndFixItem</c>.</returns>
        public string GetCheckAndFixItemFilename(string name, string version);
    }
}
