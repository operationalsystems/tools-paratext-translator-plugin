using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using AddInSideViews;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TvpMain.Check;
using TvpMain.Data;
using TvpMain.Util;

namespace TvpTest
{
    /// <summary>
    /// Basic tests for project, book, and chapter-scope text checks.
    /// </summary>
    [TestClass]
    public class CheckTests
    {
        /// <summary>
        /// Multiplier for book numbers in BCV-style references.
        /// </summary>
        public static readonly int TEST_BOOK_REF_MULTIPLIER = 1000000;

        /// <summary>
        /// Multiplier for chapter numbers in BCV-style references.
        /// </summary>
        public static readonly int TEST_CHAP_REF_MULTIPLIER = 1000;

        /// <summary>
        /// Range ref parts (i.e., chapters, verses).
        /// </summary>
        public static readonly int TEST_REF_PART_RANGE = 1000;

        /// <summary>
        /// Test project name.
        /// </summary>
        private const string TEST_PROJECT_NAME = "testProjectName";

        /// <summary>
        /// Test versification name.
        /// </summary>
        private const string TEST_VERSIFICATION_NAME = "testVersificationName";

        /// <summary>
        /// Max book number.
        /// </summary>
        private const int TEST_MAX_BOOK_NUM = 66;

        /// <summary>
        /// Test book number, for book-scale tests.
        /// </summary>
        private const int TEST_BOOK_NUM = 10;

        /// <summary>
        /// Test chapter number, for chapter-scale tests.
        /// </summary>
        private const int TEST_CHAPTER_NUM = 10;

        /// <summary>
        /// Test delay in miliseconds, for cancellation tests.
        /// </summary>
        private const int TEST_DELAY_IN_MS = 500;

        /// <summary>
        /// Time in seconds to wait for cancellation to complete.
        /// </summary>
        private const int TEST_CANCEL_TIMEOUT_IN_SEC = 5;

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
        /// Expected set of refs (BCV coordinates) for a given test.
        /// </summary>
        private ISet<int> _expectedRefs;

        /// <summary>
        /// True to delay verse extraction, false otherwise (for cancellation tests).
        /// </summary>
        private bool _isVersesDelayed;

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

            // host setup
            _mockHost.Setup(hostItem => hostItem.GetScriptureExtractor(TEST_PROJECT_NAME, ExtractorType.USFM))
                .Returns(_mockExtractor.Object);
            _mockHost.Setup(hostItem => hostItem.GetProjectVersificationName(TEST_PROJECT_NAME))
                .Returns(TEST_VERSIFICATION_NAME);
            _mockHost.Setup(hostItem => hostItem.GetCurrentRef(TEST_VERSIFICATION_NAME))
                .Returns<string>((versificationName) =>
                GetVerseRef(TEST_BOOK_NUM, TEST_CHAPTER_NUM, 1));
            _mockHost.Setup(hostItem => hostItem.GetLastChapter(It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, string>((bookNum, versificationName) => bookNum + 11);
            _mockHost.Setup(hostItem => hostItem.GetLastVerse(It.IsAny<int>(), It.IsAny<int>(), TEST_VERSIFICATION_NAME))
                .Returns<int, int, string>((bookNum, chapterNum, versificationName) => chapterNum + 111);

            // extractor setup
            _mockExtractor.Setup(extractorItem => extractorItem.Extract(It.IsAny<int>(), It.IsAny<int>()))
                .Callback(() =>
                {
                    if (_isVersesDelayed)
                    {
                        Thread.Sleep(TEST_DELAY_IN_MS);
                    }
                })
                .Returns<int, int>((fromRef, toRef) =>
                {
                    Assert.AreEqual(fromRef, toRef, ">1 verse requested."); // always request one verse at a time
                    return GetVerseText(fromRef);
                });
        }

        /// <summary>
        /// Turn independant BCV valus into a coordinate.
        /// </summary>
        /// <param name="bookNum">Paratext book number (1-66).</param>
        /// <param name="chapterNum">Paratext chapter number (book- and versification-specific).</param>
        /// <param name="verseNum">Paratext verse number (chapter- and versification-specific).</param>
        /// <returns></returns>
        static private int GetVerseRef(int bookNum, int chapterNum, int verseNum)
        {
            int verseRef = bookNum * TEST_BOOK_REF_MULTIPLIER;
            verseRef += chapterNum * TEST_CHAP_REF_MULTIPLIER;
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
            for (int bookNum = 1; bookNum <= TEST_MAX_BOOK_NUM; bookNum++)
            {
                for (int chapterNum = 1; chapterNum <= 11 + bookNum; chapterNum++)
                {
                    for (int verseNum = 1; verseNum <= 111 + chapterNum; verseNum++)
                    {
                        _expectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }

            ITextCheck textCheck = new MissingSentencePunctuationCheck1(_mockHost.Object, TEST_PROJECT_NAME);

            // execute
            CheckResult checkResult = textCheck.RunCheck(CheckArea.CurrentProject);

            // assert
            Assert.AreEqual(0, _expectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(1454, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests that a check can scan a specific book (only a specific bookt, but all chapters and verses).
        /// </summary>
        [TestMethod]
        public void TestBookOnlyPunctuationCheck()
        {
            // setup
            for (int chapterNum = 1; chapterNum <= 11 + TEST_BOOK_NUM; chapterNum++)
            {
                for (int verseNum = 1; verseNum <= 111 + chapterNum; verseNum++)
                {
                    _expectedRefs.Add(GetVerseRef(TEST_BOOK_NUM, chapterNum, verseNum));
                }
            }

            ITextCheck textCheck = new MissingSentencePunctuationCheck1(_mockHost.Object, TEST_PROJECT_NAME);

            // execute
            CheckResult checkResult = textCheck.RunCheck(CheckArea.CurrentBook);

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
            for (int verseNum = 1; verseNum <= 111 + TEST_CHAPTER_NUM; verseNum++)
            {
                _expectedRefs.Add(GetVerseRef(TEST_BOOK_NUM, TEST_CHAPTER_NUM, verseNum));
            }

            ITextCheck textCheck = new MissingSentencePunctuationCheck1(_mockHost.Object, TEST_PROJECT_NAME);

            // execute
            CheckResult checkResult = textCheck.RunCheck(CheckArea.CurrentChapter);

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
            for (int bookNum = 1; bookNum <= TEST_MAX_BOOK_NUM; bookNum++)
            {
                for (int chapterNum = 1; chapterNum <= 11 + bookNum; chapterNum++)
                {
                    for (int verseNum = 1; verseNum <= 111 + chapterNum; verseNum++)
                    {
                        _expectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }
            _isVersesDelayed = true;

            // setup
            ITextCheck textCheck = new MissingSentencePunctuationCheck1(_mockHost.Object, TEST_PROJECT_NAME);

            // start check in background thread, then cancel from test thread.
            Thread workThread = new Thread(() =>
                    textCheck.RunCheck(CheckArea.CurrentProject));
            workThread.IsBackground = true;
            workThread.Start();

            // wait a sec, then cancel
            Thread.Sleep(2000);
            Assert.IsTrue(workThread.IsAlive); // worker thread is still alive

            // cancel and wait for worker thread to be done
            textCheck.CancelCheck();
            workThread.Join(TimeSpan.FromSeconds(TEST_CANCEL_TIMEOUT_IN_SEC));

            // assert
            Assert.IsFalse(workThread.IsAlive); // worker thread is dead
            Assert.AreNotEqual(0, _expectedRefs.Count); // there are still verses to process (i.e., cancel stopped things early)
        }
    }
}
