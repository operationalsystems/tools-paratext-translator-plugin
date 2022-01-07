/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.IO;
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
