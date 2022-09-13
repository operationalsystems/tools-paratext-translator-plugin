/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;

namespace TvpMain.Check
{
    /// <summary>
    /// This class will manage access to <c>CheckAndFixItem</c> artifacts on disk and the artifacts themselves.
    /// </summary>
    public class LocalCheckAndFixManager
    {
        /// <summary>
        /// This defines the directory to store and retrieve the <c>CheckAndFixItem</c> artifacts on disk.
        /// </summary>
        public DirectoryInfo RootPath { get; private set; }

        /// <summary>
        /// The ctor.
        /// </summary>
        /// <param name="rootPath">The directory to store and retrieve the <c>CheckAndFixItem</c> items on disk. (required)</param>
        public LocalCheckAndFixManager(DirectoryInfo rootPath)
        {
            // validate input
            _ = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            RootPath = rootPath;
        }

        /// <summary>
        /// This function will retrieve all the <c>CheckAndFixItem</c> artifacts on disk.
        /// </summary>
        /// <returns>A <c>List</c> with all corresponding <c>CheckAndFixItem</c>s. (required)</returns>
        public List<CheckAndFixItem> GetAll()
        {
            var map = new List<CheckAndFixItem>();

            foreach (FileInfo file in RootPath.GetFiles("*.xml"))
            {
                Console.Out.WriteLine(file.FullName);
                try
                {
                    // add the item to cache dictionary, indexed by the ID
                    var item = CheckAndFixItem.LoadFromXmlFile(file.FullName);
                    map.Add(item);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"'{file.FullName}' could not be deserialized into a CheckAndFixItem object. Error: {ex.Message}");
                }
            }

            // return the map.
            return map;
        }

        /// <summary>
        /// This function will retrieve the corresponding <c>CheckAndFixItem</c> artifact on disk by its name and version, If available.
        /// </summary>
        /// <param name="name">The name of the <c>CheckAndFixItem</c> artifact to retrieve. (required)</param>
        /// <param name="version">The version of the <c>CheckAndFixItem</c> artifact to retrieve. (required)</param>
        /// <returns>The <c>CheckAndFixItem</c> artifact</returns>
        public CheckAndFixItem Get(string name, string version)
        {
            // validate input
            ValidateNonEmptyNameAndVersion(name, version);

            // return the deserialized file
            return CheckAndFixItem.LoadFromXmlFile(GetCaFItemPathByNameAndVersion(name, version));
        }

        /// <summary>
        /// This function will delete the corresponding <c>CheckAndFixItem</c> artifact on disk by its name and version, If available.
        /// </summary>
        /// <param name="name">The name of the <c>CheckAndFixItem</c> artifact to delete. (required)</param>
        /// <param name="version">The version of the <c>CheckAndFixItem</c> artifact to delete. (required)</param>
        public void Delete(string name, string version)
        {
            // validate input
            ValidateNonEmptyNameAndVersion(name, version);

            File.Delete(GetCaFItemPathByNameAndVersion(name, version));
        }

        /// <summary>
        /// This function will create a new <c>CheckAndFixItem</c> artifact on disk.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> artifact to be saved. (required)</param>
        /// <returns>The saved <c>CheckAndFixItem</c> artifact.</returns>
        public CheckAndFixItem Create(CheckAndFixItem item)
        {
            // validate input
            _ = item ?? throw new ArgumentNullException(nameof(item));

            var pathById = GetCaFItemPathByNameAndVersion(item.Name, item.Version);
            if (File.Exists(pathById))
            {
                throw new IOException($"An item with name '{item.Name}' and version '{item.Version}' already exists.");
            }

            item.SaveToXmlFile(pathById);

            return item;
        }

        /// <summary>
        /// This function will update an existing <c>CheckAndFixItem</c> artifact on disk. If the version hasn't been changed, the build version will be incremented by 1.
        /// </summary>
        /// <param name="item">The <c>CheckAndFixItem</c> artifact to be updated. (required)</param>
        public void Update(CheckAndFixItem item)
        {
            // validate input
            _ = item ?? throw new ArgumentNullException(nameof(item));

            // get the original item for comparison
            var origItem = Get(item.Name, item.Version);

            // compare if the version changed; otherwise rev the build version
            if (origItem.Version.Equals(item.Version))
            {
                var currentVersion = new Version(item.Version);
                var updatedVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1);
                item.Version = updatedVersion.ToString();
            }

            item.SaveToXmlFile(GetCaFItemPathByNameAndVersion(item.Name, item.Version));
        }

        /// <summary>
        /// A helper function to create the file path of a <c>CheckAndFixItem</c> artifact on disk by its name, version, and the root directory we're storing items.
        /// </summary>
        /// <param name="name">The name of the <c>CheckAndFixItem</c> artifact to create a path from. (required)</param>
        /// <param name="version">The version of the <c>CheckAndFixItem</c> artifact to create a path from. (required)</param>
        /// <returns>The normalized path.</returns>
        private string GetCaFItemPathByNameAndVersion(string name, string version)
        {
            // validate input
            ValidateNonEmptyNameAndVersion(name, version);

            return Path.Combine(RootPath.FullName, $"{name}-{version}.xml");
        }
        /// <summary>
        /// </summary>
        /// <param name="id">The ID of the <c>CheckAndFixItem</c> artifact to validate. (required)</param>


        /// <summary>
        /// A helper function to validate a name and version are not null and not empty.
        /// </summary>
        /// <param name="name">The name to validate. (required)</param>
        /// <param name="version">The version to validate. (required)</param>
        private void ValidateNonEmptyNameAndVersion(string name, string version)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException($"'{nameof(version)}' cannot be null or empty.");
            }
        }
    }
}
