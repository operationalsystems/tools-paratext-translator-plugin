using AddInSideViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Data;
using TvpMain.Filter;
using TvpMain.Util;

namespace TvpMain.Check
{
    /// <summary>
    /// Abstract implementation that parallelizes the concrete check across
    /// one or more books.
    /// </summary>
    public abstract class AbstractTextCheck : ITextCheck
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _activeProjectName;

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="activeProjectName"></param>
        protected AbstractTextCheck(IHost host, string activeProjectName)
        {
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
        }

        /// <summary>
        /// Internal class that keeps track of a scripture extractor and progress count.
        /// </summary>
        private class ExtractorState
        {
            /// <summary>
            /// Scripture extractor.
            /// </summary>
            public IScrExtractor ScrExtractor { get; }

            /// <summary>
            /// Progress count.
            /// </summary>
            public int SubTotal { get; set; }

            /// <summary>
            /// Basic ctor.
            /// </summary>
            /// <param name="extractor">Scripture extractor.</param>
            public ExtractorState(IScrExtractor extractor)
            {
                this.ScrExtractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
                SubTotal = 0;
            }
        }

        /// <summary>
        /// Cancels a running check.
        /// </summary>
        public void CancelCheck()
        {
            _tokenSource?.Cancel();
        }

        /// <summary>
        /// Runs a check.
        /// </summary>
        /// <param name="checkArea">Check area (i.e., scope; required).</param>
        /// <returns>Check result.</returns>
        public CheckResult RunCheck(CheckArea checkArea)
        {
            var checkResult = new CheckResult();
            var versificationName = _host.GetProjectVersificationName(_activeProjectName);

            var currProgress = 0;
            _tokenSource = new CancellationTokenSource();

            // get user's location in Paratext
            HostUtil.RefToBcv(_host.GetCurrentRef(versificationName),
                out var refBook, out var refChapter, out var refVerse);

            // determine book range using check area and user's location in Paratext
            var minBook = (checkArea == CheckArea.CurrentProject)
                ? 1 : refBook;
            var maxBook = (checkArea == CheckArea.CurrentProject)
                ? (MainConsts.MAX_BOOK_NUM + 1) : (refBook + 1);
            var numBooks = (maxBook - minBook);

            // wrap in a single thread to coordinate TPL (parallelized) for-loop 
            // so calling thread can busy-wait and keep the UI responsive w/DoEvents().
            var workThread = new Thread(() =>
            {
                try
                {
                    // Paralellizes the checks. We're not using (e.g.) backgroundworker because we want >1 thread 
                    // to do this this efficiently across the whole Bible, but also want to constrain the number of threads.
                    Parallel.For(minBook, maxBook,
                        new ParallelOptions
                        {
                            MaxDegreeOfParallelism = MainConsts.MAX_CHECK_THREADS,
                            CancellationToken = _tokenSource.Token
                        },
                        () => new ExtractorState(_host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM)),
                        (bookNum, loopState, extractorState) =>
                        {
                            // determine chapter range using checkarea and user's location in Paratext
                            var minChapter = (checkArea != CheckArea.CurrentChapter)
                                ? 1
                                : refChapter;
                            var maxChapter = (checkArea != CheckArea.CurrentChapter)
                                ? _host.GetLastChapter(bookNum, versificationName)
                                : refChapter;

                            var currBookNum = bookNum;
                            var currChapterNum = 0;
                            var currVerseNum = 0;

                            try
                            {
                                IList<ResultItem> resultItems = new List<ResultItem>();
                                for (var chapterNum = minChapter; chapterNum <= maxChapter; chapterNum++)
                                {
                                    currChapterNum = chapterNum;
                                    var lastVerseNum = _host.GetLastVerse(bookNum, chapterNum, versificationName);

                                    for (var verseNum = 1; verseNum <= lastVerseNum; verseNum++)
                                    {
                                        if (_tokenSource.IsCancellationRequested)
                                        {
                                            return extractorState;
                                        }

                                        currVerseNum = verseNum;

                                        var checkRef = HostUtil.BcvToRef(bookNum, chapterNum, verseNum);
                                        string verseText = null;

                                        try
                                        {
                                            verseText = extractorState.ScrExtractor.Extract(checkRef, checkRef);
                                        }
                                        catch (ArgumentException)
                                        {
                                            // arg exceptions occur when verses are missing, 
                                            // which they can be for given translations (ignore and move on)
                                            continue;
                                        }

                                        if (verseText == null
                                            || !verseText.Trim().Any())
                                        {
                                            continue;
                                        }

                                        resultItems.Clear();
                                        CheckVerse(bookNum, chapterNum, verseNum, verseText, resultItems);

                                        foreach (var resultItem in resultItems)
                                        {
                                            checkResult.ResultItems.Enqueue(resultItem);
                                        }
                                    }
                                }

                                extractorState.SubTotal++;

                                lock (this)
                                {
                                    currProgress++;
                                    CheckUpdated?.Invoke(this, (checkArea == CheckArea.CurrentProject)
                                        ? new CheckUpdatedArgs(currProgress, numBooks)
                                        : new CheckUpdatedArgs(currProgress, 1));
                                }
                            }
                            catch (ThreadAbortException)
                            {
                                // Ignore (can occur w/cancel).
                            }
                            catch (Exception ex)
                            {
                                HostUtil.Instance.ReportError(
                                    $"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{_activeProjectName}\"",
                                    ex);
                            }

                            return extractorState;
                        },
                        (extractorState) =>
                        {
                            // ignore, at this time
                        });
                }
                catch (ThreadAbortException)
                {
                    // Ignore (can occur w/cancel).
                }
                catch (OperationCanceledException)
                {
                    // Ignore (can occur w/cancel).
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError($"Error: Can't check project: \"{_activeProjectName}\"", ex);
                }
            })
            { IsBackground = true };
            workThread.Start();

            // busy-wait until helper thead is done,
            // keeping the UI responsive w/DoEvents()
            var threadSleepInMs = (int)(1000f / (float)MainConsts.CHECK_EVENTS_UPDATE_RATE_IN_FPS);
            while (workThread.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(threadSleepInMs);
            }

            return _tokenSource.IsCancellationRequested
                ? null : checkResult;
        }

        /// <summary>
        /// Abstract check method.
        /// </summary>
        /// <param name="bookNum">Book number.</param>
        /// <param name="chapterNum">Chapter number.</param>
        /// <param name="verseNum">Verse number.</param>
        /// <param name="verseText">Verse text.</param>
        /// <param name="resultItems">Result items list to populate.</param>
        protected abstract void CheckVerse(int bookNum, int chapterNum, int verseNum, string verseText, IList<ResultItem> resultItems);
    }
}
