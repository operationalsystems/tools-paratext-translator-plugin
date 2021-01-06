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