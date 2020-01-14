using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvpMain.Project;
using TvpMain.Text;
using TvpMain.Result;
using TvpMain.Util;

namespace TvpTest
{
    /// <summary>
    /// Basic tests for project, book, and chapter-scope text checks.
    /// </summary>
    [TestClass]
    public class CheckRunnerTests
    {
        /// <summary>
        /// Multiplier for book numbers in BCV-style references.
        /// </summary>
        public static readonly int TestBookRefMultiplier = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        public static readonly int TestChapRefMultiplier = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        public static readonly int TestRefPartRange = 1000;

        /// <summary>
        /// Minimum number of books in test datasets.
        /// </summary>
        public static readonly int MinTestBooks = 11;

        /// <summary>
        /// Minimum number of verses in test datasets.
        /// </summary>
        public static readonly int MinTestVerses = 111;

        /// <summary>
        /// Test project name.
        /// </summary>
        private const string TestProjectName = "testProjectName";

        /// <summary>
        /// Test versification name.
        /// </summary>
        private const string TestVersificationName = "testVersificationName";

        /// <summary>
        /// Books present setting for test project.
        /// </summary>
        private const string TestBooksPresentSetting =
            "111111111111111111111111111111111111111111111111111111111111111111000000000000000000000000000000000000000000000000000000000";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        private const string ChapterVerseSeparatorSetting = ":";

        /// <summary>
        /// Chapter verse separator setting.
        /// </summary>
        private const string RangeIndicatorSetting = "-";

        /// <summary>
        /// Sequence indicator setting.
        /// </summary>
        private const string SequenceIndicatorSetting = ",|;| («terrenal»),";

        /// <summary>
        /// Chapter range separator setting.
        /// </summary>
        private const string ChapterRangeSeparatorSetting = "–| al |—|b—";

        /// <summary>
        /// Book sequence separator setting.
        /// </summary>
        private const string BookSequenceSeparatorSetting = "; ";

        /// <summary>
        /// Chapter number separator setting.
        /// </summary>
        private const string ChapterNumberSeparatorSetting = "; | y | ( ";

        /// <summary>
        /// Reference extra material setting.
        /// </summary>
        private const string ReferenceExtraMaterialSetting = "a|Salmos |capítulos |capítulo |cap. |Cap. | –| Tít.-50 –|cf.|) ";

        /// <summary>
        /// Reference final punctuation setting.
        /// </summary>
        private const string ReferenceFinalPunctuationSetting = "";

        /// <summary>
        /// Max book number.
        /// </summary>
        private const int TestMaxBookNum = 66;

        /// <summary>
        /// Test book number, for book-scale tests.
        /// </summary>
        private const int TestBookNum = 10;

        /// <summary>
        /// Test chapter number, for chapter-scale tests.
        /// </summary>
        private const int TestChapterNum = 10;

        /// <summary>
        /// Test delay in milliseconds, for cancellation tests.
        /// </summary>
        private const int TestDelayInMs = 500;

        /// <summary>
        /// Time in seconds to wait for cancellation to complete.
        /// </summary>
        private const int TestCancelTimeoutInSec = 5;

        /// <summary>
        /// Test verses loaded from resource file.
        /// </summary>
        private string[] _verseLines;

        /// <summary>
        /// Mock Paratext scripture extractor.
        /// </summary>
        private Mock<IScrExtractor> _mockExtractor;

        /// <summary>
        /// Mock Paratext scripture host.
        /// </summary>
        private Mock<IHost> _mockHost;

        /// <summary>
        /// Mock settings manager.
        /// </summary>
        private Mock<SettingsManager> _mockSettingsManager;

        /// <summary>
        /// Expected set of refs (BCV coordinates) for a given test.
        /// </summary>
        private ISet<int> _expectedRefs;

        /// <summary>
        /// True to delay verse extraction, false otherwise (for cancellation tests).
        /// </summary>
        private bool _isVersesDelayed;

        /// <summary>
        /// Test contexts.
        /// </summary>
        private readonly ISet<TextContext> _testContexts = new HashSet<TextContext>()
        {
            TextContext.MainText
        };

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        [TestInitialize]
        [DeploymentItem(@"Resources\test-verses.txt", "Resources")]
        public void TestSetup()
        {
            _verseLines = File.ReadAllLines(@"Resources\test-verses.txt");
            _expectedRefs = new HashSet<int>();
            _isVersesDelayed = false;

            _mockHost = new Mock<IHost>(MockBehavior.Strict);
            _mockExtractor = new Mock<IScrExtractor>(MockBehavior.Strict);
            _mockSettingsManager = new Mock<SettingsManager>(MockBehavior.Strict,
                _mockHost.Object,
                TestProjectName);

            // host setup
            _mockHost.Setup(hostItem => hostItem.GetScriptureExtractor(TestProjectName, ExtractorType.USFM))
                .Returns(_mockExtractor.Object);
            _mockHost.Setup(hostItem => hostItem.GetProjectVersificationName(TestProjectName))
                .Returns(TestVersificationName);
            _mockHost.Setup(hostItem => hostItem.GetCurrentRef(TestVersificationName))
                .Returns<string>((versificationName) =>
                GetVerseRef(TestBookNum, TestChapterNum, 1));
            _mockHost.Setup(hostItem => hostItem.GetLastChapter(It.IsAny<int>(), TestVersificationName))
                .Returns<int, string>((bookNum, versificationName) => bookNum + MinTestBooks);
            _mockHost.Setup(hostItem => hostItem.GetLastVerse(It.IsAny<int>(), It.IsAny<int>(), TestVersificationName))
                .Returns<int, int, string>((bookNum, chapterNum, versificationName) => chapterNum + MinTestVerses);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "BooksPresent"))
                .Returns<string, string>((projectName, settingsKey) => TestBooksPresentSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "ChapterVerseSeparator"))
                .Returns<string, string>((projectName, settingsKey) => ChapterVerseSeparatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "RangeIndicator"))
                .Returns<string, string>((projectName, settingsKey) => RangeIndicatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "SequenceIndicator"))
                .Returns<string, string>((projectName, settingsKey) => SequenceIndicatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "ChapterRangeSeparator"))
                .Returns<string, string>((projectName, settingsKey) => ChapterRangeSeparatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "BookSequenceSeparator"))
                .Returns<string, string>((projectName, settingsKey) => BookSequenceSeparatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "ChapterNumberSeparator"))
                .Returns<string, string>((projectName, settingsKey) => ChapterNumberSeparatorSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "ReferenceExtraMaterial"))
                .Returns<string, string>((projectName, settingsKey) => ReferenceExtraMaterialSetting);
            _mockHost.Setup(hostItem => hostItem.GetProjectSetting(TestProjectName, "ReferenceFinalPunctuation"))
                .Returns<string, string>((projectName, settingsKey) => ReferenceFinalPunctuationSetting);

            // extractor setup
            _mockExtractor.Setup(extractorItem => extractorItem.Extract(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() =>
                {
                    if (_isVersesDelayed)
                    {
                        Thread.Sleep(TestDelayInMs);
                    }
                })
                .Returns<int, int>((fromRef, toRef) =>
                {
                    Assert.AreEqual(fromRef, toRef, ">1 verse requested."); // always request one verse at a time
                    return GetVerseText(fromRef);
                });
            _mockExtractor.SetupAllProperties();
        }

        /// <summary>
        /// Turn independent BCV values into a coordinate.
        /// </summary>
        /// <param name="bookNum">Paratext book number (1-66).</param>
        /// <param name="chapterNum">Paratext chapter number (book- and versification-specific).</param>
        /// <param name="verseNum">Paratext verse number (chapter- and versification-specific).</param>
        /// <returns></returns>
        private static int GetVerseRef(int bookNum, int chapterNum, int verseNum)
        {
            var verseRef = bookNum * TestBookRefMultiplier;
            verseRef += chapterNum * TestChapRefMultiplier;
            verseRef += verseNum;
            return verseRef;
        }

        /// <summary>
        /// Extract verse text from test verses and remove from expected set.
        /// </summary>
        /// <param name="verseRef">BCV coordinate.</param>
        /// <returns>Verse text.</returns>
        private string GetVerseText(int verseRef)
        {
            // checks are currently multi-threaded by nature
            lock (_expectedRefs)
            {
                Assert.IsTrue(_expectedRefs.Remove(verseRef), "Unexpected verse requested.");
            }
            return _verseLines[verseRef % _verseLines.Length];
        }

        /// <summary>
        /// Tests that a check can scan an entire project (all books, chapters, and verses).
        /// </summary>
        [TestMethod]
        public void TestWholeProjectPunctuationCheck()
        {
            // setup
            for (var bookNum = 1; bookNum <= TestMaxBookNum; bookNum++)
            {
                for (var chapterNum = 1; chapterNum <= MinTestBooks + bookNum; chapterNum++)
                {
                    for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                    {
                        _expectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }

            // execute
            var checkRunner = new TextCheckRunner(
                _mockHost.Object,
                TestProjectName,
                _mockSettingsManager.Object);
            checkRunner.RunChecks(
               CheckArea.CurrentProject,
               new List<ITextCheck>()
               {
                   new MissingSentencePunctuationCheck()
               }, _testContexts,
               out var checkResult);

            // assert
            Assert.AreEqual(0, _expectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(1459, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests that a check can scan a specific book (only a specific book, but all chapters and verses).
        /// </summary>
        [TestMethod]
        public void TestBookOnlyPunctuationCheck()
        {
            // setup
            for (var chapterNum = 1; chapterNum <= MinTestBooks + TestBookNum; chapterNum++)
            {
                for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                {
                    _expectedRefs.Add(GetVerseRef(TestBookNum, chapterNum, verseNum));
                }
            }

            var checkRunner = new TextCheckRunner(
                _mockHost.Object,
                TestProjectName,
                _mockSettingsManager.Object);
            checkRunner.RunChecks(
                CheckArea.CurrentBook,
                new List<ITextCheck>()
                {
                    new MissingSentencePunctuationCheck()
                }, _testContexts,
                out var checkResult);

            // assert
            Assert.AreEqual(0, _expectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(8, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests a check can scan a specific chapter (only a specific book and chapters, but all verses).
        /// </summary>
        [TestMethod]
        public void TestChapterOnlyPunctuationCheck()
        {
            // setup
            for (var verseNum = 0; verseNum <= MinTestVerses + TestChapterNum; verseNum++)
            {
                _expectedRefs.Add(GetVerseRef(TestBookNum, TestChapterNum, verseNum));
            }

            var checkRunner = new TextCheckRunner(
                _mockHost.Object,
                TestProjectName,
                _mockSettingsManager.Object);
            checkRunner.RunChecks(
                CheckArea.CurrentChapter,
                new List<ITextCheck>()
                {
                    new MissingSentencePunctuationCheck()
                }, _testContexts,
                out var checkResult);

            // assert
            Assert.AreEqual(0, _expectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(0, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests a check may be cancelled, once started and will not completed.
        /// </summary>
        [TestMethod]
        public void TestCancelledPunctuationCheck()
        {
            for (var bookNum = 1; bookNum <= TestMaxBookNum; bookNum++)
            {
                for (var chapterNum = 1; chapterNum <= MinTestBooks + bookNum; chapterNum++)
                {
                    for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                    {
                        _expectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }
            _isVersesDelayed = true;

            // setup
            var checkRunner = new TextCheckRunner(
                _mockHost.Object,
                TestProjectName,
                _mockSettingsManager.Object);

            // start check in background thread, then cancel from test thread.
            var workThread = new Thread(() =>
                    checkRunner.RunChecks(
                        CheckArea.CurrentProject,
                        new List<ITextCheck>()
                        {
                            new MissingSentencePunctuationCheck()
                        }, _testContexts,
                        out var checkResult))
            { IsBackground = true };
            workThread.Start();

            // wait a sec, then cancel
            Thread.Sleep(2000);
            Assert.IsTrue(workThread.IsAlive); // worker thread is still alive

            // cancel and wait for worker thread to be done
            checkRunner.CancelChecks();
            workThread.Join(TimeSpan.FromSeconds(TestCancelTimeoutInSec));

            // assert
            Assert.IsFalse(workThread.IsAlive); // worker thread is dead
            Assert.AreNotEqual(0, _expectedRefs.Count); // there are still verses to process (i.e., cancel stopped things early)
        }
    }
}
