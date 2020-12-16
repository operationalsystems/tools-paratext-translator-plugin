using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
        private string Dan6_Quotes;
        private string Dan6_NoQuotes;

        CheckAndFixRunner checkAndFixRunner = new CheckAndFixRunner();

        public CheckAndFixRunnerTests()
        {
            Jer1 = File.ReadAllText(@"Resources/testReferences/Jer1_Intro.sfm");
            Jhn1 = File.ReadAllText(@"Resources/testReferences/Jhn1_Intro.sfm");
            Dan6_Quotes = File.ReadAllText(@"Resources/testReferences/Dan6_PoetryQuoteMatch.sfm");
            Dan6_NoQuotes = File.ReadAllText(@"Resources/testReferences/Dan6_PoetryNoQuoteMatch.sfm");
        }

        /// <summary>
        /// Test to find the text in a footnote
        /// </summary>
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
            Assert.AreEqual("’" + '\u2006' + "”", results[0].FixText);
        }

        /// <summary>
        /// Test to replace the non-space b/t a single and double curly brace with 1/6 space
        /// </summary>
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

        /// <summary>
        /// Find poetry that does not have quotes around it. Maybe it should?
        /// </summary>
        [TestMethod()]
        public void TestVerifyQuotedPoetryNoQuotes()
        {
            // using Daniel chapter 6
            string testText = Dan6_NoQuotes;

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/PoetryQuoteCheck.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            string expectedMatch = "For he is the living God\r\n\\qm2 and he endures forever;\r\n\\qm1 his kingdom will not be destroyed,\r\n\\qm2 his dominion will never end.\r\n\\qm1\r\n\\v 27 He rescues and he saves;\r\n\\qm2 he performs signs and wonders\r\n\\qm2 in the heavens and on the earth.\r\n\\qm1 He has rescued Daniel\r\n\\qm2 from the power of the lions.";

            string expectedFix = "“For he is the living God\r\n\\qm2 and he endures forever;\r\n\\qm1 his kingdom will not be destroyed,\r\n\\qm2 his dominion will never end.\r\n\\qm1\r\n\\v 27 He rescues and he saves;\r\n\\qm2 he performs signs and wonders\r\n\\qm2 in the heavens and on the earth.\r\n\\qm1 He has rescued Daniel\r\n\\qm2 from the power of the lions.”";

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatch, results[0].MatchText, "MatchText does not match");
            Assert.AreEqual(expectedFix, results[0].FixText, "FixText does not match");
        }

        /// <summary>
        /// Find poetry that does not have quotes around it. Maybe it should?
        /// </summary>
        [TestMethod()]
        public void TestVerifyQuotedPoetryQuotes()
        {
            // using Daniel chapter 6
            string testText = Dan6_Quotes;

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/PoetryQuoteCheck.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Make sure there is a space after (rtl) the end of a footnote
        /// </summary>
        [TestMethod()]
        public void TestRTLAddSpaceAfterFootnote()
        {
            // from Daniel chapter 6
            string testText = @"\v 28 به او گفت: «پس از اين نام تو ديگر يعقوب نخواهد بود، بلكه اسرائيل\f + \fr 32‏:28 \ft «اسرائيل» يعنی «کسی که نزد خدا مقاوم است».‏\f*، زيرا نزد خدا و مردم مقاوم بوده و پيروز شده‌ای.»";

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLAddMissingSpacesAfterFootnotes.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(@"\f*", results[0].MatchText);
            Assert.AreEqual(@"\f*" + '\u0020', results[0].FixText);
        }

        /// <summary>
        /// Make sure there is a space after (rtl) the end of a footnote, unless there is already one there!
        /// </summary>
        [TestMethod()]
        public void TestRTLAddSpaceAfterFootnoteInverse()
        {
            // from Daniel chapter 6
            string testText = @"\v 28 به او گفت: «پس از اين نام تو ديگر يعقوب نخواهد بود، بلكه اسرائيل\f + \fr 32‏:28 \ft «اسرائيل» يعنی «کسی که نزد خدا مقاوم است».‏\f* ، زيرا نزد خدا و مردم مقاوم بوده و پيروز شده‌ای.»";

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLAddMissingSpacesAfterFootnotes.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Check for embedded tags
        /// </summary>
        [TestMethod()]
        public void TestEmbeddedChecks()
        {
            // from Daniel chapter 6
            string testText = @"\v 36 Decía: \wj «\w \+tl Abba\+tl*\w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*e \wj pero no sea lo que yo quiero, sino lo que quieres tú».\wj*";

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck3.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(@"\wj «\w \+tl Abba\+tl*\w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*", results[0].MatchText);
            Assert.AreEqual(@"\wj «\+w \+tl Abba\+tl* \+w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj* ", results[0].FixText);
        }

        /// <summary>
        /// Check for embedded tags, unless they are already marked correctly
        /// </summary>
        [TestMethod()]
        public void TestEmbeddedChecksInverse()
        {
            // from Daniel chapter 6
            string testText = @"\v 36 Decía: \wj «\+w \+tl Abba\+tl*\+w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*e \wj pero no sea lo que yo quiero, sino lo que quieres tú».\wj*";

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck3.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Check for text in a project while ignoring markers and headings
        /// </summary>
        [TestMethod()]
        public void TestInText()
        {
            // From spaNBV08 - GEN 1:1
            var testText = @"\mt1 \nGénesis\c \n1\s1 La creación\p\n\v 1 En el principio creó Dios los cielos y la tierra.\n\v 2 La tierra estaba desordenada y no tenía forma. La oscuridad cubría el profundo abismo, mientras que el Espíritu de Dios se movía sobre las aguas.";
            var expectedMatchText = @"Génesis \nLa creación \nEn el principio creó Dios los cielos y la tierra. \nLa tierra estaba desordenada y no tenía forma. \nLa oscuridad cubría el profundo abismo, mientras que el Espíritu de Dios se movía sobre las aguas.";

            // Perform the check and fix assessment
            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/InText.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
        }

        /// <summary>
        /// Check for a missing IE tag and add it
        /// </summary>
        [TestMethod()]
        public void TextAddTagIE()
        {
            var testText = @"\toc3\n Éx\mt1\n Éxodo\c\n 1\s1\n Los egipcios oprimen a los israelitas";;
            var expectedMatchText = @"\mt1 Génesis \n\ie \n\c 1 \n\s1 La creación";

            // Perform the check and fix assessment
            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddIETag.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);

        }

        /// <summary>
        /// Check for a missing IE tag and add it
        /// </summary>
        [TestMethod()]
        public void TestReplaceIorTag()
        {
            var testText = @"\io Genesis\io*";
            //var expectedMatchText = @"\io Genesis\io*";

            // Perform the check and fix assessment
            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddIETag.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(@"\io Genesis\io*", results[0].MatchText);
            Assert.AreEqual(@"\ior Genesis\ior*", results[0].FixText);

        }

    }
}
