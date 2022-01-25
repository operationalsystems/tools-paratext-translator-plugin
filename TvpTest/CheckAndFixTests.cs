/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
