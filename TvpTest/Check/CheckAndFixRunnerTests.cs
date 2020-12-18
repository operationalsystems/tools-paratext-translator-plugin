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
        /// Find instances of footnotes and add a word break.
        /// </summary>
        [TestMethod()]
        public void TestThaoAddWordBreaksAfterFootnotes()
        {
            // From thNCVw1 - GEN 8:21
            var inputText = @"\v 21 \nd องค์พระผู้เป็นเจ้า\nd*/ทรงได้/กลิ่น/อัน/เป็น/ที่/พอพระทัย/และ/ทรง/ดำริ/ว่า “เรา/จะ/ไม่/สาปแช่ง/แผ่นดิน/เพราะ/มนุษย์/อีก/ต่อไป ถึงแม้ว่า\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*จิตใจ/ของ/มนุษย์/จะ/โน้มเอียง/ไป/ใน/ทาง/ชั่ว/ตั้งแต่/วัยเด็ก แต่/เรา/จะ/ไม่/ทำลาย/ล้าง/สิ่งมีชีวิต/ทั้งปวง/อย่างที่/เรา/ได้/ทำ/ใน/คราวนี้/อีก";
            var expectedMatchText = @"\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*";
            var expectedFixText = @"\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*/";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ThaiBurmeseLaoFinalizationCheck1.xml");

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        /// <summary>
        /// Test that we can find instances of blank lines between non-poetry and poetry and that they're removed.
        /// </summary>
        [TestMethod()]
        public void TestRemovalOfBlanksBetweenPoetryAndNonPoetry()
        {
            // From usNIV11 - MIC 1:1
            var inputText = @"\p
\v 1 The word of the \nd Lord\nd* that came to Micah of Moresheth during the reigns of Jotham, Ahaz and Hezekiah, kings of Judah—the vision he saw concerning Samaria and Jerusalem.
\b
\b
\q1
";
            var expectedMatchText = "\r\n\\b";
            var expectedFixText = @" ";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/BlankLineCleanupCheck1.xml");

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        /// <summary>
        /// Test finding \id in a given RtL project and replace it with the desired text.
        /// </summary>
        [TestMethod()]
        public void TestRightToLeftIdReplace()
        {
            // From faPCB18 - GEN 1:0 (preliminary tags)
            var inputText = @"\id GEN - Persian Contemporary Bible‎
\rem Copyright © 1995‎, 2005, 2018  by Biblica‎, Inc‎.‎®‎‎
\h پيدايش
";
            var expectedMatchText = @"- Persian Contemporary Bible‎
\rem Copyright © 1995‎, 2005, 2018  by Biblica‎, Inc‎.‎®‎‎";
            var expectedFixText = "- <Name>\n\\rem Copyright © <Year> by Biblica, Inc.‎ ";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLReplaceIdCheck2.xml");

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText.Trim());
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }
        /// <summary>
        /// Replace content in Toc2 tag with the content in the Header tag.
        /// </summary>
        [TestMethod]
        public void TestReplaceToc2WithHContent()
        {
            var inputText = @"\h GENESIS
\toc1 Genesis
\toc2 Genesis
\toc3 Gen.
";
            var expectedMatchText = @"Genesis";
            var expectedFixText = @"GENESIS";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ReplaceToc2WithHContent.xml");

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(33, results[0].MatchStart);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Check for a missing \ie tag following a \mt1 tag and add it.
        /// </summary>
        [TestMethod()]
        public void TestAddIETag()
        {
            var testText = @"\toc3 Éx
\mt1 Éxodo\c 1
\s1 Los egipcios oprimen a los israelitas";
            var expectedMatchText = @"Éxodo";
            var expectedFixText = @"Éxodo
\\ie
";

            // Perform the check and fix assessment
            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddIETag.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Wrap an \ior...ior* tag in paranthesis.
        /// </summary>
        [TestMethod()]
        public void TestReplaceIorTag()
        {
            var inputText = @"\io1 Ìṣẹ̀dá ayé \ior 1.1–2.3\ior*. ";
            var expectedMatchText = @"\ior 1.1–2.3\ior*";
            var expectedFixText = @"(\ior 1.1–2.3\ior*)";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ReplaceIorTag.xml");

            List<CheckResultItem> results = checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

    }
}
