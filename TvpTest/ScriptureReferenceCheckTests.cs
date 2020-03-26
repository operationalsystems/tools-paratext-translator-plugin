using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TvpMain.Reference;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpTest
{
    /// <summary>
    /// Scripture reference tests.
    /// </summary>
    [TestClass]
    public class ScriptureReferenceCheckTests : AbstractCheckTests
    {
        /// <summary>
        /// Per-test context, provided by MsTest framework.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Reference checker under test.
        /// </summary>
        protected ScriptureReferenceCheck ReferenceCheck;

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            base.AbstractTestSetup(TestContext);

            ReferenceCheck = new ScriptureReferenceCheck(MockProjectManager.Object);
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
            ReferenceCheck.CheckText(partData, resultList);

            // Empty results list = no exceptions
            Assert.IsTrue(resultList.Count == 0);
        }


        /// The following unit tests all pertain to the table of contents section of the text.

        /// <summary>
        /// A test for a wrong verse separator malformation reference in the table of contents.
        /// </summary>
        [TestMethod]
        public void MalformedToCReferenceCheck()
        {
            // Arrange

            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Genesis 1;3 \ior*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count,
                "The reference has an error in spacing or punctuation.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.LooseFormatting),
                "The reference has an error in spacing or punctuation.");
            Assert.AreEqual(@"\ior Genesis 1;3 \ior*",
                resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \ior tag in the table of contents.
        ///</summary>
        [TestMethod]
        public void MissingOpeningTagInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(Genesis 1:1-4 \ior*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing opening \ior tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"Reference is missing opening, closing tags or a \.");
            Assert.AreEqual(@"Genesis 1:1-4 \ior*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \ior* tag in the table of contents.
        ///</summary>
        [TestMethod]
        public void MissingClosingTagInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Genesis 1:1; 4:4)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing closing \ior* tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"Reference is missing opening, closing tags or a \.");
            Assert.AreEqual(@"\ior Genesis 1:1; 4:4",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \ior tags in the table of contents.
        ///</summary>
        [TestMethod]
        public void MissingAllTagsInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(Genesis 1:14)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing all \ior tags in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "Reference is missing tags.");
            Assert.AreEqual(@"Genesis 1:14",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a short name in the table of contents.
        ///</summary>
        [TestMethod]
        public void ShortNameInToCReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior GEN 1:1; 4:4 \ior*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Incorrect name style is used in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                "Incorrect name style is used in reference.");
            Assert.AreEqual(@"\ior GEN 1:1; 4:4 \ior*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the table of contents.
        ///</summary>
        [TestMethod]
        public void AbbreviatedNameReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior GEN 1:1; 4:3 \ior*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Short name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Short name is expected in reference.");
            Assert.AreEqual(@"\ior GEN 1:1; 4:3 \ior*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \fr tags in the table of contents.
        ///</summary>
        [TestMethod]
        public void FrTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\fr \+xt Genesis 1:1\+xt*\fr*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(3, resultList.Count,
                @"An \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(2, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\fr",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the table of contents.
        ///</summary>
        [TestMethod]
        public void PlusXtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\+xt Genesis 1:4\+xt*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(2, resultList.Count,
                @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\+xt Genesis 1:4\+xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the table of contents.
        ///</summary>
        [TestMethod]
        public void FtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ft \+xt Genesis 1:1\+xt* \ft*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(3, resultList.Count,
                @"A \ft tag surrounding a reference outside of footnotes area.");
            var errorType = (ScriptureReferenceErrorType)resultList[0].ResultTypeCode;
            Assert.AreEqual(2, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\ft",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the table of contents.
        ///</summary>
        [TestMethod]
        public void XtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\xt Genesis 1:1; 4:3\xt*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(2, resultList.Count,
                @"A \xt tag surrounding a reference in table of contents area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\xt Genesis 1:1; 4:3\xt*",
                resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Introduction section of the text.

        /// <summary>
        /// A test for a space malformation reference in the introduction.
        /// </summary>
        [TestMethod]
        public void MalformedIntroductionReference()
        {
            // Arrange

            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Luke 1: 0\xt*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count,
                "No results generated.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.LooseFormatting),
                "Incorrect name style is used in reference.");
            Assert.AreEqual(@"\xt Luke 1: 0\xt*",
                resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \xt tag in the introduction.
        ///</summary>
        [TestMethod]
        public void MissingOpeningTagInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"Genesis 1:5\xt*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing opening \xt tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"Reference is missing opening, closing, or a \.");
            Assert.AreEqual(@"Genesis 1:5\xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \xt* tag in the introduction.
        ///</summary>
        [TestMethod]
        public void MissingClosingTagInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Genesis 1:8",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing closing \xt* tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"Reference is missing opening, closing, or a \.");
            Assert.AreEqual(@"\xt Genesis 1:8",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \xt tags in the introduction.
        ///</summary>
        [TestMethod]
        public void MissingAllTagsInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"Luke 1:12",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(0, resultList.Count,
                @"\xt tags may be missing in reference.");
        }

        /// <summary>
        /// A test case for a reference using a long name in the introduction.
        ///</summary>
        [TestMethod]
        public void AbbreviationNameInInIntroductionReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt GEN 1:7\xt*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Short name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Short name is expected in reference.");
            Assert.AreEqual(@"\xt GEN 1:7\xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the introduction.
        ///</summary>
        [TestMethod]
        public void AbbreviatedNameReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt EXO 5:1\xt*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Short name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Short name is expected in reference.");
            Assert.AreEqual(@"\xt EXO 5:1\xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ior tags in the introduction.
        ///</summary>
        [TestMethod]
        public void IorTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ior Exodus 3:1-4\ior*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"An \ior tag surrounding a reference outside of ToC area.");
            var errorType = (ScriptureReferenceErrorType)resultList[0].ResultTypeCode;
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\ior Exodus 3:1-4\ior*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the introduction.
        ///</summary>
        [TestMethod]
        public void PlusXtTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\+xt Numbers 10:1\+xt*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\+xt Numbers 10:1\+xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \fr tags in the introduction.
        ///</summary>
        [TestMethod]
        public void FrTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fr \xt Numbers 3:5\xt*\fr*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"A \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\fr",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the introduction.
        ///</summary>
        [TestMethod]
        public void FtTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ft \xt Deuteronomy 3:2\xt*\ft*",
                PartContext.Introductions);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"A \ft tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\ft",
                resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Main Text section of the text.

        /// <summary>
        /// A test for a wrong verse separator malformation reference in the table of contents.
        /// </summary>
        [TestMethod]
        public void MalformedMainTextReference()
        {
            // Arrange

            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Genesis 1;3 \ior*)",
                PartContext.Outlines);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count,
                "Reference is malformed.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.LooseFormatting),
                "The reference has an error in spacing or punctuation.");
            Assert.AreEqual(@"\ior Genesis 1;3 \ior*",
                resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \xt tag in the main text.
        ///</summary>
        [TestMethod]
        public void MissingOpeningTagInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"Deuteronomy 1:3\xt*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing opening \xt tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"The reference is missing opening, closing or a \.");
            Assert.AreEqual(@"Deuteronomy 1:3\xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \xt* tag in the main text.
        ///</summary>
        [TestMethod]
        public void MissingClosingTagInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Deuteronomy 3:1",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing closing \xt* tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"The reference is missing opening, closing or a \.");
            Assert.AreEqual(@"\xt Deuteronomy 3:1",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \ior tags in the main text.
        ///</summary>
        [TestMethod]
        public void MissingAllTagsInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"NUM 11:1",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing all \xt tags in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The reference has no tags, but should.");
            Assert.AreEqual(@"NUM 11:1",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a short name in the main text.
        ///</summary>
        [TestMethod]
        public void LongNameInMainTextReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @" \xt Leviticus 1:4 \xt*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Abbreviation is expected in reference.");
            Assert.AreEqual(@"\xt Leviticus 1:4 \xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the main text.
        ///</summary>
        [TestMethod]
        public void ShortNameInMainTextReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Leviticus 2:4 \xt*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                    @"Abbreviated name is expected in reference.");
            Assert.AreEqual(@"\xt Leviticus 2:4 \xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ior tags in the main text.
        ///</summary>
        [TestMethod]
        public void IorTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ior GEN 5:1\ior*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(2, resultList.Count,
                @"An \ior tag surrounding a reference outside of table of contents area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\ior GEN 5:1\ior*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the main text.
        ///</summary>
        [TestMethod]
        public void PlusXtTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @" \+xt NUM 3:1\+xt*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(2, resultList.Count,
                @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\+xt NUM 3:1\+xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the main text.
        ///</summary>
        [TestMethod]
        public void FtTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ft \+xt GEN 1:1\+xt* \ft*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(3, resultList.Count,
                @"A \ft tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(2, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\ft",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the main text.
        ///</summary>
        [TestMethod]
        public void FrTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fr \xt NUM 3:5\xt*\fr*",
                PartContext.MainText);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"A \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectTag),
                "The wrong tag is used for the context.");
            Assert.AreEqual(@"\fr",
                resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Footnotes/References section of the text.

        /// <summary>
        /// A test for a wrong verse separator malformation reference in the footnotes/references area.
        /// </summary>
        [TestMethod]
        public void MalformedFootnoteReferenceCheck()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt 7:1, 4\+xt*\f*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count,
                "Reference is malformed.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.LooseFormatting),
                "The reference has an error in spacing or punctuation.");
            Assert.AreEqual(@"\+xt 7:1, 4\+xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference missing an opening \+xt tag in footnotes/references area.
        ///</summary>
        [TestMethod]
        public void MissingOpeningTagInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp Genesis 1:1\+xt*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing opening \+xt tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                "The reference has an error in spacing or punctuation.");
            Assert.AreEqual(@"Genesis 1:1\+xt*",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \+xt* tag in the footnotes/references area.
        ///</summary>
        [TestMethod]
        public void MissingClosingTagInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Luke 3:4 \f*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing closing \+xt* tag in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MalformedTag),
                @"The reference is missing opening, closing or a \.");
            Assert.AreEqual(@"\+xt Luke 3:4",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \+xt \ft \fr tags in the footnotes/references area.
        ///</summary>
        [TestMethod]
        public void MissingAllTagsInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp GEN 1:1\f*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Missing all \+xt \ft \fr tags in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.MissingTag),
                "The reference has no tags, but should.");
            Assert.AreEqual(@"GEN 1:1",
                resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a long name in the footnotes/references area.
        ///</summary>
        [TestMethod]
        public void LongNameInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Numbers 5:1\+xt*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(@"\+xt Numbers 5:1\+xt*",
                    resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the footnotes/references area.
        ///</summary>
        [TestMethod]
        public void ShortNameReferenceInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Numbers 8:8\+xt*\f*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count,
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(1, CountErrors(resultList,
                    ScriptureReferenceErrorType.IncorrectNameStyle),
                @"Abbreviated name is expected in reference.");
            Assert.AreEqual(@"\+xt Numbers 8:8\+xt*",
                    resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the footnotes/references area.
        ///</summary>
        [TestMethod]
        public void XtTaggedReferenceInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \xt LUK 3:3\xt*\f*",
                PartContext.NoteOrReference);

            // Executes the check
            ReferenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(0, resultList.Count,
                @"A \xt tag surrounding a reference in table of footnotes/references area.");
        }

        /// <summary>
        /// Count scripture reference errors of a given type.
        /// </summary>
        /// <param name="inputItems">Input result items to search (required).</param>
        /// <param name="searchTypes">Input error types to count (required).</param>
        /// <returns>Sum of found errors of the supplied types.</returns>
        private static int CountErrors(
            IEnumerable<ResultItem> inputItems,
            params ScriptureReferenceErrorType[] searchTypes) =>
            searchTypes
                .Select(typeItem =>
                    inputItems.Select(resultItem =>
                        (ScriptureReferenceErrorType)resultItem.ResultTypeCode)
                    .Count(codeItem =>
                        codeItem == typeItem))
                .Sum();
    }
}
