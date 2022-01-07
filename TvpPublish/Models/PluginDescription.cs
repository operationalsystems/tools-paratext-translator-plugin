/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace TvpPublish.Models
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
