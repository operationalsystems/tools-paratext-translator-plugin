using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.Check;
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
            // holds any results the check will find (i.e., exceptions)
            var resultList = new List<ResultItem>();

            // describes location and nature of the text being checked
            var textLocation = new TextLocation(1, 1, 1, TextContext.MainText);

            // executes the check
            _referenceCheck.CheckVerse(textLocation, "Testing!", resultList);

            // empty results list = no exceptions
            Assert.IsTrue(resultList.Count == 0);
        }
    }
}
