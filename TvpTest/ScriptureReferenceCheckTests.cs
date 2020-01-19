using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.Check;
using TvpMain.Text;
using TvpMain.Result;

namespace TvpTest
{
    /// <summary>
    /// Scripture reference tests.
    /// </summary>
    [TestClass]
    public class ScriptureReferenceCheckTests
    {
        /// <summary>
        /// Reference checker under test.
        /// </summary>
        private ScriptureReferenceCheck _referenceCheck;

        /// <summary>
        /// Test setup.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            _referenceCheck = new ScriptureReferenceCheck();
        }

        /// <summary>
        /// A starter test.
        /// </summary>
        [TestMethod]
        public void FirstReferenceCheckTest()
        {
            // Holds any results the check will find (i.e., exceptions)
            var resultList = new List<ResultItem>();

            // Describes location and nature of the text being checked
            // Note: "TextContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                "Testing 1...2...3...", 3, 3,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Empty results list = no exceptions
            Assert.IsTrue(resultList.Count == 0);
        }
    }
}
