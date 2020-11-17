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
        public void ExecCheckAndFixRunnerTest_InFootnotes()
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
        public void ExecCheckAndFixRunnerTest_FinalizationCheck1()
        {
            // using John chapter 1
            string testText = Jhn1;

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck1.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual("“I am the voice of one calling in the wilderness, ‘Make straight the way for the Lord.’”", results[0].MatchText);
            Assert.AreEqual("“I am the voice of one calling in the wilderness, ‘Make straight the way for the Lord.’ ”", results[0].FixText);
        }
    }
}