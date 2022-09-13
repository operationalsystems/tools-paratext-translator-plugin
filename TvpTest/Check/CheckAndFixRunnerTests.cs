/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace TvpMain.Check.Tests
{

    /// <summary>
    /// Tests for the Check and Fix Runner
    /// </summary>
    [TestClass]
    public class CheckAndFixRunnerTests
    {
        private string _jer1;
        private string _jhn1;
        private string _dan6Quotes;
        private string _dan6NoQuotes;
        private string _mat17;
        private string _rom1;
        private string _escapeCodes;

        CheckAndFixRunner _checkAndFixRunner = new CheckAndFixRunner();


        public CheckAndFixRunnerTests()
        {
            _jer1 = File.ReadAllText(@"Resources/testReferences/Jer1_Intro.sfm");
            _jhn1 = File.ReadAllText(@"Resources/testReferences/Jhn1_Intro.sfm");
            _dan6Quotes = File.ReadAllText(@"Resources/testReferences/Dan6_PoetryQuoteMatch.sfm");
            _dan6NoQuotes = File.ReadAllText(@"Resources/testReferences/Dan6_PoetryNoQuoteMatch.sfm");
            _mat17 = File.ReadAllText(@"Resources/testReferences/Mat17.sfm");
            _rom1 = File.ReadAllText(@"Resources/testReferences/Rom1.sfm");
            _escapeCodes = File.ReadAllText(@"Resources/testReferences/EscapeCodeMatch.sfm");
        }

        [TestMethod]
        public void TestCrLfEscapeCodes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1.
";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\r\n"));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        [TestMethod]
        public void TestTabEscapeCodes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1." + "\t";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\t"));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        [TestMethod]
        public void TestBackslashEscapeCodes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1.\";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\"));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        [TestMethod]
        public void TestUnicodeEscapeCodes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1.\u\uab☺☺\u123";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\u\uab\u263A\u263a\u123"));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        [TestMethod]
        public void TestSingleQuotes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1.‘’‘’'";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\u2018\u2019‘’'"));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        [TestMethod]
        public void TestDoubleQuotes()
        {
            var inputText = _escapeCodes;
            const string expectedMatchText = @"\rem Line #1.";
            const string expectedFixText = @"\rem Line #1.“”“”""";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlContent(
                File.ReadAllText(@"Resources/checkFixes/EscapeCodeCheck.xml")
                    .Replace("XXX", @"\u201C\u201d“”"""));
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(3, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        /// <summary>
        /// Test to find the text in a footnote
        /// </summary>
        [TestMethod]
        public void TestInFootnotes()
        {
            // using Jeremiah chapter 1
            var testText = _jer1;
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/InFootnotes.xml"));

            // Should have two results
            Assert.AreEqual(2, results.Count);

            // Should both have + in the results
            Assert.AreEqual("+ ", results[0].MatchText);
            Assert.AreEqual("+ ", results[1].MatchText);
        }

        /// <summary>
        /// Test to replace the non-space b/t a single and double curly brace with 1/6 space
        /// </summary>
        [TestMethod]
        public void TestFinalizationCheck1()
        {
            // using John chapter 1
            var testText = _jhn1;
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck1.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual("’”", results[0].MatchText);
            Assert.AreEqual("’" + '\u2006' + "”", results[0].FixText);
        }

        /// <summary>
        /// Test to replace the non-space b/t a single and double curly brace with 1/6 space
        /// </summary>
        [TestMethod]
        public void TestBasicLoremIpsum()
        {
            var testText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut " +
                           "labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut " +
                           "aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                           "eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/checkFixExample.xml"));

            // Should have one result
            Assert.AreEqual(10, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual("Lorem", results[0].MatchText);
            Assert.AreEqual("LOREM", results[0].FixText);
        }

        /// <summary>
        /// Find poetry that does not have quotes around it. Maybe it should?
        /// </summary>
        [TestMethod]
        public void TestVerifyQuotedPoetryNoQuotes()
        {
            // using Daniel chapter 6
            var testText = _dan6NoQuotes;
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/PoetryQuoteCheck.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);
            const string expectedMatch = "For he is the living God\r\n\\qm2 and he endures forever;\r\n\\qm1 his kingdom will not be destroyed,\r\n\\qm2 his dominion will never end.\r\n\\qm1\r\n\\v 27 He rescues and he saves;\r\n\\qm2 he performs signs and wonders\r\n\\qm2 in the heavens and on the earth.\r\n\\qm1 He has rescued Daniel\r\n\\qm2 from the power of the lions.";
            const string expectedFix = "“For he is the living God\r\n\\qm2 and he endures forever;\r\n\\qm1 his kingdom will not be destroyed,\r\n\\qm2 his dominion will never end.\r\n\\qm1\r\n\\v 27 He rescues and he saves;\r\n\\qm2 he performs signs and wonders\r\n\\qm2 in the heavens and on the earth.\r\n\\qm1 He has rescued Daniel\r\n\\qm2 from the power of the lions.”";

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatch, results[0].MatchText, "MatchText does not match");
            Assert.AreEqual(expectedFix, results[0].FixText, "FixText does not match");
        }

        /// <summary>
        /// Find poetry that does not have quotes around it. Maybe it should?
        /// </summary>
        [TestMethod]
        public void TestVerifyQuotedPoetryQuotes()
        {
            // using Daniel chapter 6
            var testText = _dan6Quotes;
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/PoetryQuoteCheck.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Make sure there is a space after (rtl) the end of a footnote
        /// </summary>
        [TestMethod]
        public void TestRtlAddSpaceAfterFootnote()
        {
            // from Daniel chapter 6
            const string testText = @"\v 28 به او گفت: «پس از اين نام تو ديگر يعقوب نخواهد بود، بلكه اسرائيل\f + \fr 32‏:28 \ft «اسرائيل» يعنی «کسی که نزد خدا مقاوم است».‏\f*، زيرا نزد خدا و مردم مقاوم بوده و پيروز شده‌ای.»";
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLAddMissingSpacesAfterFootnotes.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(@"\f*", results[0].MatchText);
            Assert.AreEqual(@"\f*" + '\u0020', results[0].FixText);
        }

        /// <summary>
        /// Make sure there is a space after (rtl) the end of a footnote, unless there is already one there!
        /// </summary>
        [TestMethod]
        public void TestRtlAddSpaceAfterFootnoteInverse()
        {
            // from Daniel chapter 6
            const string testText = @"\v 28 به او گفت: «پس از اين نام تو ديگر يعقوب نخواهد بود، بلكه اسرائيل\f + \fr 32‏:28 \ft «اسرائيل» يعنی «کسی که نزد خدا مقاوم است».‏\f* ، زيرا نزد خدا و مردم مقاوم بوده و پيروز شده‌ای.»";
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLAddMissingSpacesAfterFootnotes.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Check for embedded tags
        /// </summary>
        [TestMethod]
        public void TestEmbeddedChecks()
        {
            // from Daniel chapter 6
            const string testText = @"\v 36 Decía: \wj «\w \+tl Abba\+tl*\w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*e \wj pero no sea lo que yo quiero, sino lo que quieres tú».\wj*";
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck3.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(@"\wj «\w \+tl Abba\+tl*\w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*", results[0].MatchText);
            Assert.AreEqual(@"\wj «\+w \+tl Abba\+tl* \+w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj* ", results[0].FixText);
        }

        /// <summary>
        /// Check for embedded tags, unless they are already marked correctly
        /// </summary>
        [TestMethod]
        public void TestEmbeddedChecksInverse()
        {
            // from Daniel chapter 6
            const string testText = @"\v 36 Decía: \wj «\+w \+tl Abba\+tl*\+w*, Padre, todo es posible para ti. No me hagas beber este trago amargo,\wj*e \wj pero no sea lo que yo quiero, sino lo que quieres tú».\wj*";
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/FinalizationCheck3.xml"));

            // Should have one result
            Assert.AreEqual(0, results.Count);
        }

        /// <summary>
        /// Find instances of footnotes and add a word break.
        /// </summary>
        [TestMethod]
        public void TestThaoAddWordBreaksAfterFootnotes()
        {
            // From thNCVw1 - GEN 8:21
            const string inputText = @"\v 21 \nd องค์พระผู้เป็นเจ้า\nd*/ทรงได้/กลิ่น/อัน/เป็น/ที่/พอพระทัย/และ/ทรง/ดำริ/ว่า “เรา/จะ/ไม่/สาปแช่ง/แผ่นดิน/เพราะ/มนุษย์/อีก/ต่อไป ถึงแม้ว่า\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*จิตใจ/ของ/มนุษย์/จะ/โน้มเอียง/ไป/ใน/ทาง/ชั่ว/ตั้งแต่/วัยเด็ก แต่/เรา/จะ/ไม่/ทำลาย/ล้าง/สิ่งมีชีวิต/ทั้งปวง/อย่างที่/เรา/ได้/ทำ/ใน/คราวนี้/อีก";
            const string expectedMatchText = @"\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*";
            const string expectedFixText = @"\f + \fr 8:21 \ft หรือ/\fqa เพราะว่า\f*/";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ThaiBurmeseLaoFinalizationCheck1.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        /// <summary>
        /// Test that we can find instances of blank lines between non-poetry and poetry and that they're removed.
        /// </summary>
        [TestMethod]
        public void TestRemovalOfBlanksBetweenPoetryAndNonPoetry()
        {
            // From usNIV11 - MIC 1:1
            const string inputText = @"\p
\v 1 The word of the \nd Lord\nd* that came to Micah of Moresheth during the reigns of Jotham, Ahaz and Hezekiah, kings of Judah—the vision he saw concerning Samaria and Jerusalem.
\b
\b
\q1
";
            const string expectedMatchText = "\r\n\\b";
            const string expectedFixText = @" ";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/BlankLineCleanupCheck1.xml");

            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);
        }

        /// <summary>
        /// Test finding \id in a given RtL project and replace it with the desired text.
        /// </summary>
        [TestMethod]
        public void TestRightToLeftIdReplace()
        {
            // From faPCB18 - GEN 1:0 (preliminary tags)
            const string inputText = @"\id GEN - Persian Contemporary Bible‎
\rem Copyright © 1995‎, 2005, 2018  by Biblica‎, Inc‎.‎®‎‎
\h پيدايش
";
            const string expectedMatchText = @"- Persian Contemporary Bible‎
\rem Copyright © 1995‎, 2005, 2018  by Biblica‎, Inc‎.‎®‎‎";
            const string expectedFixText = @"- <Name>
\rem Copyright © <Year> by Biblica, Inc.‎ ";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/RTLReplaceIdCheck2.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

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
            const string inputText = @"\h GENESIS
\toc1 Genesis
\toc2 Genesis
\toc3 Gen.
";
            const string expectedMatchText = @"Genesis";
            const string expectedFixText = @"GENESIS";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ReplaceToc2WithHContent.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

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
        [TestMethod]
        public void TestAddIeTag()
        {
            const string testText = @"\toc3 Éx
\mt1 Éxodo\c 1
\s1 Los egipcios oprimen a los israelitas";
            const string expectedMatchText = @"Éxodo";
            const string expectedFixText = @"Éxodo
\ie
";

            // Perform the check and fix assessment
            var results = _checkAndFixRunner.ExecCheckAndFix(testText, CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddIETag.xml"));

            // Should have one result
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Wrap an \ior...ior* tag in paranthesis.
        /// </summary>
        [TestMethod]
        public void TestReplaceIorTag()
        {
            const string inputText = @"\io1 Ìṣẹ̀dá ayé \ior 1.1–2.3\ior*. ";
            const string expectedMatchText = @"\ior 1.1–2.3\ior*";
            const string expectedFixText = @"(\ior 1.1–2.3\ior*)";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/ReplaceIorTag.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Add a space between \v tag and number.
        /// </summary>
        [TestMethod]
        public void AddSpaceBetweenVerseTagAndNumber()
        {
            const string inputText = @"\v1 Paul, a servant of Christ Jesus, called to be an apostle and set apart for the gospel of God—";
            const string expectedMatchText = @"\v1";
            const string expectedFixText = @"\v 1";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddSpaceBetweenVerseTagAndNumber.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Add alternate separtion character for 2 verse spans
        /// </summary>
        [TestMethod]
        public void AlternateSeparationCharacterForVerseSpans()
        {
            var inputText = _mat17;
            const string expectedMatchText = @"\v 20-21";
            const string expectedFixText = @"\v 20-21 \vp 20,21 \vp*";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AlternateSeparationCharacterForVerseSpans.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(1, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }

        /// <summary>
        /// Add a \po tag following a \b tag.
        /// </summary>
        [TestMethod]
        public void AddPoTagFollowingBTags()
        {
            var inputText = _rom1;
            const string expectedMatchText = @"\b
\p";
            const string expectedFixText = @"\b
\po";

            // Perform the check and fix assessment
            var checkAndFix = CheckAndFixItem.LoadFromXmlFile(@"Resources/checkFixes/AddPoTagFollowingBTags.xml");
            var results = _checkAndFixRunner.ExecCheckAndFix(inputText, checkAndFix);

            // Should have one result
            Assert.AreEqual(2, results.Count);

            // Check the found value and the replacement suggestion
            Assert.AreEqual(expectedMatchText, results[0].MatchText);
            Assert.AreEqual(expectedFixText, results[0].FixText);

        }
    }
}
