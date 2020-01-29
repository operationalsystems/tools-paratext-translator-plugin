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


        /// The following unit tests all pertain to the table of contents section of the text.
        
        /// <summary>
        /// A test for a wrong verse separator malformation reference in the table of contents.
        /// <summary>
        [TestMethod]
        public void MalformedToCReferenceCheck()
        {
            // Arrange

            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Gen. 1;3 \ior*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count, "Reference is malformed.");
            Assert.AreEqual(@"(\ior Gen. 1;3 \ior*)", resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \ior tag in the table of contents.
        ///
        [TestMethod]
        public void MissingOpeningTagInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(Genesis 1:1-4 \ior)*", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing opening \ior tag in reference.");
            Assert.AreEqual(@"(Genesis 1:1-4 \ior*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \ior* tag in the table of contents.
        ///
        [TestMethod]
        public void MissingClosingTagInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Genesis 1:1; 4:4)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing closing \ior* tag in reference.");
            Assert.AreEqual(@"(\ior Genesis 1:1; 4:4)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \ior tags in the table of contents.
        ///
        [TestMethod]
        public void MissingAllTagsInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(Genesis 1:14)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing all \ior tags in reference.");
            Assert.AreEqual(@"(Genesis 1:14)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a short name in the table of contents.
        ///
        [TestMethod]
        public void ShortNameReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Gen. 1:1; 4:4 \ior*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Short name is used in reference.");
            Assert.AreEqual(@"(\ior Gen. 1:1; 4:4 \ior*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the table of contents.
        ///
        [TestMethod]
        public void AbbreviatedNameReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Ge 1:1; 4:3 \ior*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Abbreviated name is used in reference.");
            Assert.AreEqual(@"(\ior Ge 1:1; 4:3 \ior*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \fr tags in the table of contents.
        ///
        [TestMethod]
        public void FrTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\fr \+xt Gen. 1:1\+xt*\fr*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"An \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"(\fr \+xt Gen. 1:1\+xt*\fr*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the table of contents.
        ///
        [TestMethod]
        public void PlusXtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\+xt Gen. 1:4\+xt*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"(\+xt Gen. 1:4\+xt*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the table of contents.
        ///
        [TestMethod]
        public void FtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ft \+xt Gen. 1:1\+xt* \ft*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \ft tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"(\ft \+xt Gen. 1:1\+xt* \ft*)", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the table of contents.
        ///
        [TestMethod]
        public void XtTaggedReferenceInToC()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\xt Gen. 1:1; 4:3\xt*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \xt tag surrounding a reference in table of contents area.");
            Assert.AreEqual(@"(\xt Gen. 1:1; 4:3\xt*)", resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Introduction section of the text.

        /// <summary>
        /// A test for a space malformation reference in the introduction.
        /// <summary>
        [TestMethod]
        public void MalformedIntroductionReference()
        {
            // Arrange
           
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Luke1:0\xt*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);
          

            // Assert
            Assert.AreEqual(1, resultList.Count, "No results generated.");
            Assert.AreEqual(@"\xt Luke1:0\xt*", resultList[0].MatchText);
            
        }

        /// <summary>
        /// A test case for a reference missing an opening \xt tag in the introduction.
        ///
        [TestMethod]
        public void MissingOpeningTagInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1,1,1, 
                @"Gen. 1:5\xt*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing opening \xt tag in reference.");
            Assert.AreEqual(@"Gen. 1:5\xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \xt* tag in the introduction.
        ///
        [TestMethod]
        public void MissingClosingTagInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1,1,1, 
                @"\xt Gen. 1:8", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing closing \xt* tag in reference.");
            Assert.AreEqual(@"\xt Gen. 1:8", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \xt tags in the introduction.
        ///
        [TestMethod]
        public void MissingAllTagsInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1,1,1, 
                @"Luke 1:12", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing all \xt tags in reference.");
            Assert.AreEqual(@"Luke 1:12", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a long name in the introduction.
        ///
        [TestMethod]
        public void LongNameReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Genesis 1:7\xt*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Long name is used in reference.");
            Assert.AreEqual(@"\xt Genesis 1:7\xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the introduction.
        ///
        [TestMethod]
        public void AbbreviatedNameReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Ex. 5:1\xt*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Abbreviated name is used in reference.");
            Assert.AreEqual(@"\xt Ex. 5:1\xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ior tags in the introduction.
        ///
        [TestMethod]
        public void IorTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ior Exodus 3:1-4\ior*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"An \ior tag surrounding a reference outside of ToC area.");
            Assert.AreEqual(@"\ior Exodus 3:1-4\ior*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the introduction.
        ///
        [TestMethod]
        public void PlusXtTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\+xt Num. 10:1\+xt*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"\+xt Num. 10:1\+xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \fr tags in the introduction.
        ///
        [TestMethod]
        public void FrTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fr \xt Num. 3:5\xt*\fr*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"\fr \xt Num. 3:5\xt*\fr*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the introduction.
        ///
        [TestMethod]
        public void FtTaggedReferenceInIntroduction()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ft \xt Deut. 3:2\xt*\ft*", 1, 1,
                PartContext.Introductions);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"\ft \xt Deut. 3:2\xt*\ft*", resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Main Text section of the text.

        /// <summary>
        /// A test for a wrong verse separator malformation reference in the table of contents.
        /// <summary>
        [TestMethod]
        public void MalformedMainTextReference()
        {
            // Arrange

            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"(\ior Gen. 1;3 \ior*)", 1, 1,
                PartContext.Outlines);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count, "Reference is malformed.");
            Assert.AreEqual(@"(\ior Gen. 1;3 \ior*)", resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \xt tag in the main text.
        ///
        [TestMethod]
        public void MissingOpeningTagInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"Deut. 1:3\xt*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing opening \xt tag in reference.");
            Assert.AreEqual(@"Deut. 1:3\xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \xt* tag in the main text.
        ///
        [TestMethod]
        public void MissingClosingTagInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Deut. 3:1", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing closing \xt* tag in reference.");
            Assert.AreEqual(@"\xt Deut. 3:1", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \ior tags in the main text.
        ///
        [TestMethod]
        public void MissingAllTagsInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"Num. 11:1", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing all \xt tags in reference.");
            Assert.AreEqual(@"Num. 11:1", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a short name in the main text.
        ///
        [TestMethod]
        public void LongNameReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @" \xt Leviticus 1:4 \xt*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Long name is used in reference.");
            Assert.AreEqual(@" \xt Leviticus 1:4 \xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the main text.
        ///
        [TestMethod]
        public void AbbreviatedNameReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\xt Lev 2:4 \xt*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Abbreviated name is used in reference.");
            Assert.AreEqual(@"\xt Lev 2:4 \xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ior tags in the main text.
        ///
        [TestMethod]
        public void IorTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ior Genesis 5:1\ior*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"An \ior tag surrounding a reference outside of table of contents area.");
            Assert.AreEqual(@"\ior Genesis 5:1\ior*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by +\xt tags in the main text.
        ///
        [TestMethod]
        public void PlusXtTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @" \+xt Num. 3:1\+xt*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A +\xt tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@" \+xt Num. 3:1\+xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \ft tags in the main text.
        ///
        [TestMethod]
        public void FtTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\ft \+xt Gen. 1:1\+xt* \ft*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \ft tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"\ft \+xt Gen. 1:1\+xt* \ft*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the main text.
        ///
        [TestMethod]
        public void FrTaggedReferenceInMainText()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fr \xt Num. 3:5\xt*\fr*", 1, 1,
                PartContext.MainText);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \fr tag surrounding a reference outside of footnotes area.");
            Assert.AreEqual(@"\fr \xt Num. 3:5\xt*\fr*", resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Footnotes/References section of the text.

        /// <summary>
        /// A test for a wrong verse separator malformation reference in the footnotes/references area.
        /// <summary>
        [TestMethod]
        public void MalformedFootnoteReferenceCheck()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \xt 7:1, 4\xt*\f*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);


            // Assert
            Assert.AreEqual(1, resultList.Count, "Reference is malformed.");
            Assert.AreEqual(@"\fp \xt 7:1, 4\xt*\f*)", resultList[0].MatchText);

        }

        /// <summary>
        /// A test case for a reference missing an opening \+xt tag in footnotes/references area.
        ///
        [TestMethod]
        public void MissingOpeningTagInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp Gen. 1:1\+xt*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing opening \+xt tag in reference.");
            Assert.AreEqual(@"\fp Gen. 1:1\+xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing an closing \+xt* tag in the footnotes/references area.
        ///
        [TestMethod]
        public void MissingClosingTagInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Luke 3:4 \f*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing closing \+xt* tag in reference.");
            Assert.AreEqual(@"\fp \+xt Luke 3:4 \f*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for missing all \+xt \ft \fr tags in the footnotes/references area.
        ///
        [TestMethod]
        public void MissingAllTagsInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp Gen. 1:1\f*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Missing all \+xt \ft \fr tags in reference.");
            Assert.AreEqual(@"\fp Gen. 1:1\f*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using a long name in the footnotes/references area.
        ///
        [TestMethod]
        public void LongNameReferenceInFootnoteRreference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Numbers 5:1\+xt*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Long name is used in reference.");
            Assert.AreEqual(@"\fp \+xt Numbers 5:1\+xt*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference using an abbreviated name in the footnotes/references area.
        ///
        [TestMethod]
        public void AbbreviatedNameReferenceInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \+xt Nu 8:8\+xt*\f*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"Abbreviated name is used in reference.");
            Assert.AreEqual(@"\fp \+xt Nu 8:8\+xt*\f*", resultList[0].MatchText);
        }

        /// <summary>
        /// A test case for a reference surround by \xt tags in the footnotes/references area.
        ///
        [TestMethod]
        public void XtTaggedReferenceInFootnoteReference()
        {
            // Arrange
            var resultList = new List<ResultItem>();
            // Describes location and nature of the text being checked
            // Note: "PartContext" tells the check what it's looking at.
            var partData = VersePart.Create(1, 1, 1,
                @"\fp \xt Luke 3:3\xt*\f*", 1, 1,
                PartContext.NoteOrReference);

            // Executes the check
            _referenceCheck.CheckText(partData, resultList);

            // Assert
            Assert.AreEqual(1, resultList.Count, @"A \xt tag surrounding a reference in table of footnotes/references area.");
            Assert.AreEqual(@"\fp \xt Luke 3:3\xt*\f*", resultList[0].MatchText);
        }

        /// The following unit tests all pertain to the Ignore list section of the text.
        
        /// <summary>
        /// A test case for a reference that is in the ignore list that should be ignored
        /// 
        [TestMethod]
        public void ReferenceInIgnoreList()
        {
            // Arrange
            var ignoreList = new List<IgnoreList>();

            // 
        }
    }
}
