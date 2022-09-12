/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TvpMain.Project;

namespace TvpTest
{
    [TestClass()]
    public class ProjectCheckSettingsTests
    {
        /// <summary>
        /// This test verifies that a <c>ProjectCheckSettings</c> object can be written to and created from an XML string.
        /// </summary>
        [TestMethod()]
        public void It_can_be_serialized_to_and_from_XML()
        {
            // Create the initial settings object.
            ProjectCheckSettings settings = new ProjectCheckSettings
            {
                DefaultCheckIds = new List<string>
                {
                    "Check Number One",
                    "Check Number Two"
                }
            };

            // Convert the object to an XML string.
            string settingsAsXML = settings.WriteToXmlString();

            // Create a new object from that XML string.
            ProjectCheckSettings settingsFromXML = ProjectCheckSettings.LoadFromXmlContent(settingsAsXML);

            // Compare the two objects to ensure that their properties match up.
            Assert.IsTrue(Enumerable.SequenceEqual(settings.DefaultCheckIds, settingsFromXML.DefaultCheckIds));
        }
    }
}