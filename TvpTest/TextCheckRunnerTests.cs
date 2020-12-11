using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using TvpMain.Check;
using TvpMain.Punctuation;
using TvpMain.Text;

namespace TvpTest
{
    /// <summary>
    /// Basic tests for project, book, and chapter-scope text checks.
    /// </summary>
    [TestClass]
    public class TextCheckRunnerTests : AbstractCheckTests
    {
        /// <summary>
        /// Per-test context, provided by MsTest framework.
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// Max book number.
        /// </summary>
        protected const int TEST_MAX_BOOK_NUM = 66;

        /// <summary>
        /// Test delay in milliseconds, for cancellation tests.
        /// </summary>
        protected const int TEST_DELAY_IN_MS = 500;

        /// <summary>
        /// Time in seconds to wait for cancellation to complete.
        /// </summary>
        protected const int TEST_CANCEL_TIMEOUT_IN_SEC = 5;

        /// <summary>
        /// Test verses loaded from resource file.
        /// </summary>
        protected string[] VerseLines;

        /// <summary>
        /// Expected set of refs (BCV coordinates) for a given test.
        /// </summary>
        protected ISet<int> ExpectedRefs;

        /// <summary>
        /// True to delay verse extraction, false otherwise (for cancellation tests).
        /// </summary>
        protected bool AreVersesDelayed;

        /// <summary>
        /// Test contexts.
        /// </summary>
        protected readonly ISet<PartContext> TestContexts = new HashSet<PartContext>()
        {
            PartContext.MainText
        }.ToImmutableHashSet();

        /// <summary>
        /// Test setup for verse lines and main mocks.
        /// </summary>
        [TestInitialize]
        [DeploymentItem(@"Resources\test-verses-1.txt", "Resources")]
        public void TestSetup()
        {
            base.AbstractTestSetup(TestContext);

            VerseLines = File.ReadAllLines(@"Resources\test-verses-1.txt");
            ExpectedRefs = new HashSet<int>();
            AreVersesDelayed = false;

            // extractor setup
            MockImportManager.Setup(extractorItem =>
                    extractorItem.Extract(It.IsAny<VerseLocation>()))
                .Callback(() =>
                {
                    if (AreVersesDelayed)
                    {
                        Thread.Sleep(TEST_DELAY_IN_MS);
                    }
                })
                .Returns<VerseLocation>((verseLocation) =>
                    GetVerseText(verseLocation.VerseCoordinate));
        }

        /// <summary>
        /// Extract verse text from test verses and remove from expected set.
        /// </summary>
        /// <param name="verseRef">BCV coordinate.</param>
        /// <returns>Verse text.</returns>
        protected string GetVerseText(int verseRef)
        {
            // checks are currently multi-threaded by nature
            lock (ExpectedRefs)
            {
                Assert.IsTrue(ExpectedRefs.Remove(verseRef), "Unexpected verse requested.");
            }
            return VerseLines[verseRef % VerseLines.Length];
        }

        /// <summary>
        /// Tests that a check can scan an entire project (all books, chapters, and verses).
        /// </summary>
        [TestMethod]
        public void TestWholeProjectPunctuationCheck()
        {
            // setup
            for (var bookNum = 1; bookNum <= TEST_MAX_BOOK_NUM; bookNum++)
            {
                for (var chapterNum = 1; chapterNum <= MinTestBooks + bookNum; chapterNum++)
                {
                    for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                    {
                        ExpectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }

            // execute
            var checkRunner = new TextCheckRunner(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockProjectManager.Object,
                MockImportManager.Object,
                MockResultManager.Object);
            checkRunner.RunChecks(
               CheckArea.CurrentProject,
               new List<ITextCheck>()
               {
                   new MissingSentencePunctuationCheck(MockProjectManager.Object)
               }, TestContexts,
               false,
               out var checkResult);

            // assert
            Assert.AreEqual(0, ExpectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(1459, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests that a check can scan a specific book (only a specific book, but all chapters and verses).
        /// </summary>
        [TestMethod]
        public void TestBookOnlyPunctuationCheck()
        {
            // setup
            for (var chapterNum = 1; chapterNum <= MinTestBooks + TEST_BOOK_NUM; chapterNum++)
            {
                for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                {
                    ExpectedRefs.Add(GetVerseRef(TEST_BOOK_NUM, chapterNum, verseNum));
                }
            }

            var checkRunner = new TextCheckRunner(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockProjectManager.Object,
                MockImportManager.Object,
                MockResultManager.Object);
            checkRunner.RunChecks(
                CheckArea.CurrentBook,
                new List<ITextCheck>()
                {
                    new MissingSentencePunctuationCheck(MockProjectManager.Object)
                }, TestContexts,
                false,
                out var checkResult);

            // assert
            Assert.AreEqual(0, ExpectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(8, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests a check can scan a specific chapter (only a specific book and chapters, but all verses).
        /// </summary>
        [TestMethod]
        public void TestChapterOnlyPunctuationCheck()
        {
            // setup
            for (var verseNum = 0; verseNum <= MinTestVerses + TEST_CHAPTER_NUM; verseNum++)
            {
                ExpectedRefs.Add(GetVerseRef(TEST_BOOK_NUM, TEST_CHAPTER_NUM, verseNum));
            }

            var checkRunner = new TextCheckRunner(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockProjectManager.Object,
                MockImportManager.Object,
                MockResultManager.Object);
            checkRunner.RunChecks(
                CheckArea.CurrentChapter,
                new List<ITextCheck>()
                {
                    new MissingSentencePunctuationCheck(MockProjectManager.Object)
                }, TestContexts,
                false,
                out var checkResult);

            // assert
            Assert.AreEqual(0, ExpectedRefs.Count); // all expected verses have been read
            Assert.AreEqual(0, checkResult.ResultItems.Count); // expected number of violations found
        }

        /// <summary>
        /// Tests a check may be cancelled, once started and will not completed.
        /// </summary>
        [TestMethod]
        public void TestCancelledPunctuationCheck()
        {
            for (var bookNum = 1; bookNum <= TEST_MAX_BOOK_NUM; bookNum++)
            {
                for (var chapterNum = 1; chapterNum <= MinTestBooks + bookNum; chapterNum++)
                {
                    for (var verseNum = 0; verseNum <= MinTestVerses + chapterNum; verseNum++)
                    {
                        ExpectedRefs.Add(GetVerseRef(bookNum, chapterNum, verseNum));
                    }
                }
            }
            AreVersesDelayed = true;

            // setup
            var checkRunner = new TextCheckRunner(
                MockHost.Object,
                TEST_PROJECT_NAME,
                MockProjectManager.Object,
                MockImportManager.Object,
                MockResultManager.Object);

            // start check in background thread, then cancel from test thread.
            var workThread = new Thread(() =>
                    checkRunner.RunChecks(
                        CheckArea.CurrentProject,
                        new List<ITextCheck>()
                        {
                            new MissingSentencePunctuationCheck(MockProjectManager.Object)
                        }, TestContexts,
                        false,
                        out var checkResult))
            { IsBackground = true };
            workThread.Start();

            // wait a sec, then cancel
            Thread.Sleep(2000);
            Assert.IsTrue(workThread.IsAlive); // worker thread is still alive

            // cancel and wait for worker thread to be done
            checkRunner.CancelChecks();
            workThread.Join(TimeSpan.FromSeconds(TEST_CANCEL_TIMEOUT_IN_SEC));

            // assert
            Assert.IsFalse(workThread.IsAlive); // worker thread is dead
            Assert.AreNotEqual(0, ExpectedRefs.Count); // there are still verses to process (i.e., cancel stopped things early)
        }
    }
}
