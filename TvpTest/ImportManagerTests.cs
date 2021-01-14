using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using TvpMain.Check;
using TvpMain.Reference;
using TvpMain.Text;

namespace TvpTest
{
    [TestClass]
    public class ImportManagerTests : AbstractCheckTests
    {
        /// <summary>
        /// Verse delimiter.
        /// </summary>
        protected static readonly char[] VerseDelimiter = " ".ToCharArray();

        /// <summary>
        /// Per-test context, provided by MsTest framework.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Verse dictionary.
        /// </summary>
        protected IDictionary<VerseLocation, string> VerseDictionary;

        /// <summary>
        /// Verses retrieved during a test.
        /// </summary>
        protected ISet<VerseLocation> ReadVerses;

        /// <summary>
        /// Reference result items expected from input.
        /// </summary>
        protected IList<string> ReferenceResultLines;

        /// <summary>
        /// Test contexts.
        /// </summary>
        protected readonly ISet<PartContext> TestContexts = new HashSet<PartContext>()
        {
            PartContext.Introductions,
            PartContext.MainText,
            PartContext.NoteOrReference,
            PartContext.Outlines
        }.ToImmutableHashSet();

        /// <summary>
        /// Test setup for verse lines.
        /// </summary>
        [TestInitialize]
        [DeploymentItem(@"Resources\test-usfm-1.txt", "Resources")]
        [DeploymentItem(@"Resources\reference-results-1.txt", "Resources")]
        public void TestSetup()
        {
            base.AbstractTestSetup(TestContext);

            VerseDictionary = GetUsfmVerses(@"Resources\test-usfm-1.txt");
            ReadVerses = new HashSet<VerseLocation>();

            // all expected verses have been read
            Assert.AreEqual(434, VerseDictionary.Count,
                "Unexpected verse count in test input.");

            // extractor setup
            MockImportManager.Setup(extractorItem =>
                    extractorItem.Extract(It.IsAny<VerseLocation>()))
                .Returns<VerseLocation>((verseLocation) =>
                {
                    // missing verses are ok; input is not comprehensive
                    if (!VerseDictionary.TryGetValue(verseLocation, out var verseText))
                    {
                        return string.Empty;
                    }
                    lock (ReadVerses) // checks are multi-threaded
                    {
                        Assert.IsTrue(ReadVerses.Add(verseLocation),
                            $"Duplicate verse requested from test input: {verseLocation.VerseCoordinateText}");
                    }
                    return verseText;
                });
        }

        /// <summary>
        /// Simple USFM parser that extracts verses from a complete USFM file.
        /// </summary>
        /// <param name="filePath">USFM file path (required).</param>
        /// <returns>Dictionary of verse locations to verse text.</returns>
        private static IDictionary<VerseLocation, string> GetUsfmVerses(string filePath)
        {
            var nextChapterNum = 1; // all books start at chapter 1
            var nextVerseNum = 0; // verse 0 is intro & other start matter

            var verseBuilder = new StringBuilder();
            var result = new Dictionary<VerseLocation, string>();

            foreach (var lineText in File.ReadAllLines(filePath))
            {
                var workText = lineText.Trim();

                if (workText.StartsWith(@"\c ")
                    || workText.StartsWith(@"\v "))
                {
                    var workParts = workText.Split(VerseDelimiter, 3);
                    var workNum = int.Parse(workParts[1].Trim());

                    if (workText.StartsWith(@"\c "))
                    {
                        nextChapterNum = workNum;
                        nextVerseNum = 0;
                    }
                    else if (workText.StartsWith(@"\v "))
                    {
                        var verseLocation = new VerseLocation(TEST_BOOK_NUM, nextChapterNum, nextVerseNum);
                        Assert.IsFalse(result.ContainsKey(verseLocation),
                            $"Duplicate verse location in test input: {verseLocation.VerseCoordinateText}");

                        result[verseLocation] = verseBuilder.ToString();
                        verseBuilder.Clear();

                        nextVerseNum = workNum;
                    }
                }

                verseBuilder.AppendLine(lineText);
            }

            result[new VerseLocation(TEST_BOOK_NUM, nextChapterNum, nextVerseNum)]
                = verseBuilder.ToString();
            return result;
        }
    }
}
