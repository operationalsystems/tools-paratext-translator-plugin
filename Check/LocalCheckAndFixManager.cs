using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public LocalCheckAndFixManager(DirectoryInfo rootPath) {
            // validate input
            _ = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            RootPath = rootPath;
        }

        /// <summary>
        /// This function will retrieve all the <c>CheckAndFixItem</c> artifacts on disk.
        /// </summary>
        /// <returns>A <c>Dictionary</c> with the ID as the key, and the corresponding <c>CheckAndFixItem</c> as the value. (required)</returns>
        public Dictionary<String, CheckAndFixItem> GetAll()
        {
            var map = new Dictionary<String, CheckAndFixItem>();

            foreach (FileInfo file in RootPath.GetFiles("*.xml"))
            {
                Console.Out.WriteLine(file.FullName);
                try
                {
                    // add the item to cache dictionary, indexed by the ID
                    var item = CheckAndFixItem.LoadFromXmlFile(file.FullName);
                    map.Add(item.Id, item);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"We were unable to deserialize '{file.FullName}' into a CheckAndFixItem object. Error: {ex.Message}");
                }
            }

            // return the map.
            return map;
        }

        /// <summary>
        /// This function will retrieve the corresponding <c>CheckAndFixItem</c> artifact on disk by its ID, If available.
        /// </summary>
        /// <param name="id">The ID of the <c>CheckAndFixItem</c> artifact to retrieve. (required)</param>
        /// <returns>The <c>CheckAndFixItem</c> artifact</returns>
        public CheckAndFixItem Get(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            // return the deserialized file
            return CheckAndFixItem.LoadFromXmlFile(GetCaFItemPathById(id));
        }

        /// <summary>
        /// This function will delete the corresponding <c>CheckAndFixItem</c> artifact on disk by its ID, If available.
        /// </summary>
        /// <param name="id">The ID of the <c>CheckAndFixItem</c> artifact to delete. (required)</param>
        public void Delete(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            File.Delete(GetCaFItemPathById(id));
        }

        /// <summary>
        /// This function will create a new <c>CheckAndFixItem</c> artifact on disk..
        /// </summary>
        /// <param name="name">The name of the <c>CheckAndFixItem</c> artifact. (required)</param>
        /// <returns></returns>
        public CheckAndFixItem Create(string name)
        {
            // validate input
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.");
            }

            return Create(new CheckAndFixItem()
            {
                Name = name
            });
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

            var pathById = GetCaFItemPathById(item.Id);
            if (File.Exists(pathById))
            {
                throw new IOException($"An item with ID '{item.Id}' already exists.");
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
            var origItem = Get(item.Id);

            // compare if the version changed; otherwise rev the build version
            if (origItem.Version.Equals(item.Version))
            {
                var currentVersion = new Version(item.Version);
                var updatedVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1);
                item.Version = updatedVersion.ToString();
            }

            item.SaveToXmlFile(GetCaFItemPathById(item.Id));
        }

        /// <summary>
        /// A helper function to create the file path of a <c>CheckAndFixItem</c> artifact on disk by its ID and the root directory we're storing items.
        /// </summary>
        /// <param name="id">The ID of the <c>CheckAndFixItem</c> artifact to create a path for. (required)</param>
        /// <returns></returns>
        private string GetCaFItemPathById(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            return Path.Combine(RootPath.FullName, $"{id}.xml");
        }

        /// <summary>
        /// A helper function to validate a a <c>CheckAndFixItem</c>'s ID is not null and not empty.
        /// </summary>
        /// <param name="id">The ID of the <c>CheckAndFixItem</c> artifact to validate. (required)</param>
        private void ValidateNonEmptyId(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.");
            }
        }
    }
}
