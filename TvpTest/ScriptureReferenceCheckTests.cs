using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvpMain.Check;
using TvpMain.Project;
using TvpMain.Text;
using TvpMain.Result;

namespace TvpTest
{
    /// <summary>
    /// Scripture reference tests.
    /// </summary>
    [TestClass]
    public class ScriptureReferenceCheckTests : AbstractCheckTests
    {
        /// <summary>
        /// Reference checker under test.
        /// </summary>
        private ScriptureReferenceCheck _referenceCheck;

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        [TestInitialize]
        public override void TestSetup()
        {
            base.TestSetup();

            _referenceCheck = new ScriptureReferenceCheck(MockProjectManager.Object);
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
            // Note: "PartContext" tells the check what it's looking at.
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
