using Microsoft.VisualStudio.TestTools.UnitTesting;
using TvpMain.Check;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TvpMain.Check.Tests
{

    /// <summary>
    /// Tests for the Check and Fix Runner
    /// </summary>
    [TestClass()]
    public class CheckAndFixRunnerTests
    {
        private string Jer1;
        private string Jhn1;

        CheckAndFixRunner checkAndFixRunner = new CheckAndFixRunner();

        public CheckAndFixRunnerTests()
        {
            Jer1 = File.ReadAllText(@"Resources/testReferences/Jer1_Intro.sfm");
            Jhn1 = File.ReadAllText(@"Resources/testReferences/Jhn1_Intro.sfm");
        }

        /// <summary>
        /// Test to find the text in a footnote
        /// </summary>
        [TestCategory("IgnoreOnBuild")]
        [TestMethod()]
        public void TestInFootnotes()
        {
            // using Jerimiah chapter 1
            string testText = Jer1;

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/InFootnotes.xml"));

            // Should have two results
            Assert.AreEqual(2, results.Count);

            // Should both have + in the results
            Assert.AreEqual("+ ", results[0].MatchText);
            Assert.AreEqual("+ ", results[1].MatchText);
        }

        /// <summary>
        /// Test to replace the non-space b/t a single and double curly brace with 1/6 space
        /// </summary>
        [TestCategory("IgnoreOnBuild")]
        [TestMethod()]
        public void TestFinalizationCheck1()
        {
            // using John chapter 1
            string testText = Jhn1;

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck1.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual("’”", results[0].MatchText);
            Assert.AreEqual("’"+ '\u2006' + "”", results[0].FixText);
        }

        /// <summary>
        /// Test to replace the non-space b/t a single and double curly brace with 1/6 space
        /// </summary>
        [TestCategory("IgnoreOnBuild")]
        [TestMethod()]
        public void TestBasicLoremIpsum()
        {
            string testText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut " +
                "labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut " +
                "aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                "eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/checkFixExample.xml"));

            // Should have one result
            Assert.AreEqual(10, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual("Lorem", results[0].MatchText);
            Assert.AreEqual("LOREM", results[0].FixText);
        }
    }
}
