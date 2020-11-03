using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    public class LocalCheckAndFixManager
    {
        public DirectoryInfo RootPath { get; private set; }

        public LocalCheckAndFixManager(DirectoryInfo rootPath) {
            // validate input
            _ = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            RootPath = rootPath;
        }

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

        public CheckAndFixItem Get(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            // return the deserialized file
            return CheckAndFixItem.LoadFromXmlFile(GetCaFItemPathById(id));
        }

        public void Delete(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            File.Delete(GetCaFItemPathById(id));
        }

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

        private string GetCaFItemPathById(string id)
        {
            // validate input
            ValidateNonEmptyId(id);

            return Path.Combine(RootPath.FullName, $"{id}.xml");
        }

        private void ValidateNonEmptyId(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.");
            }

        }
    }
}
