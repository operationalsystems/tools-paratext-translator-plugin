using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.Check;
using TvpMain.Export;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpTest
{
    /// <summary>
    /// Export manager tests.
    /// </summary>
    [TestClass]
    public class ExportManagerTests : AbstractCheckTests
    {
        /// <summary>
        /// Per-test context, provided by MsTest framework.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Export manager under test.
        /// </summary>
        protected ExportManager ExportManager;

        [TestInitialize]
        public void TestSetup()
        {
            base.AbstractTestSetup(TestContext);

            ExportManager = new ExportManager(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockProjectManager.Object,
                MockResultManager.Object);
        }

        /// <summary>
        /// Sample test.
        /// </summary>
        [TestMethod]
        public void FirstTest()
        {
            const string testVerseText = "Testing 1...2...3...";
            var resultList = new List<ResultItem>
            {
                CreateTestResultItem(testVerseText,
                    "1...2.",
                    8, "A...B."),
                CreateTestResultItem(testVerseText,
                    ".3..",
                    15, ".C..")
            };

            var outputVerseText = ExportManager.ReplaceSuggestionText(
                testVerseText,
                resultList);
            Assert.AreEqual("Testing A...B...C...", outputVerseText);
        }

        /// <summary>
        /// Creates a test result item.
        /// </summary>
        /// <param name="verseText">Verse text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Match start in verse (0-based).</param>
        /// <param name="suggestionText">Suggestion (replacement) text (required).</param>
        /// <returns>Created result item.</returns>
        static ResultItem CreateTestResultItem(
            string verseText,
            string matchText, int matchStart,
            string suggestionText)
        {
            return new ResultItem(VersePart.Create(1, 1, 1, verseText, PartContext.MainText),
                "Error text!", matchText, matchStart, suggestionText,
                CheckType.ScriptureReference, 0);
        }
    }
}
