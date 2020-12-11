using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TvpMain.Project
{
    /// <summary>
    /// Defines project settings that are specific to <c>CheckAndFixItem</c>s and their usage.
    /// </summary>
    public class ProjectCheckSettings
    {
        /// <summary>
        /// A list of <c>CheckAndFixItem</c> IDs that are run by default for the project.
        /// </summary>
        public List<string> DefaultCheckIds { get; set; } = new List<string>();

        /// <summary>
        /// Serialize the current <c>ProjectCheckSettings</c> object into an XML string.
        /// </summary>
        /// <returns>Corresponding <c>ProjectCheckSettings</c> object as an XML string.</returns>
        public string WriteToXmlString()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

            using StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, this);
            return textWriter.ToString();
        }

        /// <summary>
        /// Deserialize <c>ProjectCheckSettings</c> XML content into a corresonding object.
        /// </summary>
        /// <param name="xmlContent">A string representing a <c>ProjectCheckSettings</c>. (required)</param>
        /// <returns>Corresponding <c>ProjectCheckSettings</c> object.</returns>
        public static ProjectCheckSettings LoadFromXmlContent(string xmlContent)
        {
            // validate input
            _ = xmlContent ?? throw new ArgumentNullException(nameof(xmlContent));

            // deserialize the file into an object
            var serializer = new XmlSerializer(typeof(ProjectCheckSettings));

            using TextReader reader = new StringReader(xmlContent);
            ProjectCheckSettings result = (ProjectCheckSettings)serializer.Deserialize(reader);
            return result;
        }
    }
}
