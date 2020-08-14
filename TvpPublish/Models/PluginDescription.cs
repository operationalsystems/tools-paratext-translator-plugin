using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace TptPublish.Models
{
    /// <summary>
    /// The description of an installed plugin
    /// </summary>
    public class PluginDescription
    {
        /// <summary>
        /// The plugin name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique shortname representing the plugin.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The version of the plugin.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// A description of the plugin.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A description of what has changed in this version of the plugin.
        /// </summary>
        public string VersionDescription { get; set; }

        /// <summary>
        /// The versions of ParaText that this plugin version is compatible with.
        /// </summary>
        public List<string> PtVersions { get; set; }

        /// <summary>
        /// The license for this plugin.
        /// </summary>
        public string License { get; set; }

        public override string ToString()
        {
            // Return the JSON interpretation of this object.
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                // ensure that the fields are saved as camel case (instead of pascal case).
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                // Pretty print the JSON.
                Formatting = Formatting.Indented
            });
        }

        /// <summary>
        /// This function creates a <c>PluginDescription</c> from a JSON file.
        /// </summary>
        /// <param name="pluginDescriptionFilePath">The <c>PluginDescription</c> JSON filepath to read from. (required)</param>
        /// <returns>The deserialized <c>PluginDescription</c></returns>
        public static PluginDescription FromFile(string pluginDescriptionFilePath)
        {
            // validate inputs
            _ = pluginDescriptionFilePath ?? throw new ArgumentNullException(nameof(pluginDescriptionFilePath));

            string rawPluginDescription = File.ReadAllText(pluginDescriptionFilePath);
            PluginDescription pluginDescription = JsonConvert.DeserializeObject<PluginDescription>(rawPluginDescription);

            return pluginDescription;
        }

        /// <summary>
        /// This function saves a <c>PluginDescription</c> to a JSON file.
        /// </summary>
        /// <param name="pluginDescriptionFilePath">The <c>PluginDescription</c> JSON filepath to save to. (required)</param>
        public void SaveToFile(string pluginDescriptionFilePath)
        {
            // validate inputs
            _ = pluginDescriptionFilePath ?? throw new ArgumentNullException(nameof(pluginDescriptionFilePath));

            // serialize JSON to a string and then write string to a file
            File.WriteAllText(pluginDescriptionFilePath, this.ToString());
        }
    }
}
