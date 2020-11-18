using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TvpMain.Check;

namespace TvpTest
{
    [TestClass]

    public class CheckAndFixTests
    {
        [TestMethod]
        public void TestFileSerialization()
        {
            var testCafItem = new CheckAndFixItem()
            {
                Name = "testName",
                Description = "testDescription",
                Version = "1.0.0",
                CheckRegex = @"\b\w*z+\w*\b",
                CheckScript = @"function(input){
                    return input;
                }"
            };

            var testSavePath = Path.Combine(Path.GetTempPath(), "tempCafItem1.xml");

            testCafItem.SaveToXmlFile(testSavePath);

            var loadedCafItem = CheckAndFixItem.LoadFromXmlFile(testSavePath);
            Assert.AreEqual(testCafItem, loadedCafItem);
        }

        [TestMethod]
        public void TestFromFileSerialization()
        {
            var testFilePath = @"Resources/checkFixes/checkFixExample.xml";

            CheckAndFixItem checkAndFixItem = CheckAndFixItem.LoadFromXmlFile(testFilePath);

            Assert.AreEqual("Example Check Fix", checkAndFixItem.Name);
            Assert.AreEqual(CheckAndFixItem.CheckScope.VERSE, checkAndFixItem.Scope);
        }
    }
}
