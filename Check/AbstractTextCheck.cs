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

/*
 * The first validation check in the Translation Validation Plugin framework.
 */
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
        public AbstractTextCheck(IHost host, string activeProjectName)
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
            private readonly IScrExtractor _scrExtractor;

            /// <summary>
            /// Progress count.
            /// </summary>
            private int _subTotal;

            /// <summary>
            /// Scripture extractor.
            /// </summary>
            public IScrExtractor ScrExtractor { get => _scrExtractor; }

            /// <summary>
            /// Progress count.
            /// </summary>
            public int SubTotal { get => _subTotal; set => _subTotal = value; }

            /// <summary>
            /// Basic ctor.
            /// </summary>
            /// <param name="extractor">Scripture extractor.</param>
            public ExtractorState(IScrExtractor extractor)
            {
                this._scrExtractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
                _subTotal = 0;
            }
        }

        /// <summary>
        /// Cancels a running check.
        /// </summary>
        public void CancelCheck()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Runs a check.
        /// </summary>
        /// <param name="checkArea">Check area (i.e., scope; required).</param>
        /// <returns>Check result.</returns>
        public CheckResult RunCheck(CheckArea checkArea)
        {
            CheckResult checkResult = new CheckResult();
            string versificationName = _host.GetProjectVersificationName(_activeProjectName);

            int currProgress = 0;
            _tokenSource = new CancellationTokenSource();

            // get user's location in Paratext
            int userRef = _host.GetCurrentRef(versificationName);
            int refBook = (userRef / 1000000);
            int refChapter = (userRef / 1000) % 1000;
            int refVerse = userRef % 1000;

            // determine book range using checkarea and user's location in Paratext
            int minBook = (checkArea == CheckArea.CurrentProject)
                ? 1 : refBook;
            int maxBook = (checkArea == CheckArea.CurrentProject)
                ? (MainConsts.MAX_BOOK_NUM + 1) : (refBook + 1);
            int numBooks = (maxBook - minBook);

            // wrap in a single thread to coordinate TPL (parallelized) for-loop 
            // so calling thread can busy-wait and keep the UI responsive w/DoEvents().
            Thread workThread = new Thread(() =>
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
                          int minChapter = (checkArea != CheckArea.CurrentChapter)
                              ? 1 : refChapter;
                          int maxChapter = (checkArea != CheckArea.CurrentChapter)
                              ? _host.GetLastChapter(bookNum, versificationName) : refChapter;

                          int currBookNum = bookNum;
                          int currChapterNum = 0;
                          int currVerseNum = 0;

                          try
                          {
                              IList<ResultItem> resultItems = new List<ResultItem>();
                              for (int chapterNum = minChapter; chapterNum <= maxChapter; chapterNum++)
                              {
                                  currChapterNum = chapterNum;
                                  int lastVerseNum = _host.GetLastVerse(bookNum, chapterNum, versificationName);

                                  for (int verseNum = 1; verseNum <= lastVerseNum; verseNum++)
                                  {
                                      if (_tokenSource.IsCancellationRequested)
                                      {
                                          return extractorState;
                                      }

                                      currVerseNum = verseNum;
                                      int checkRef = bookNum * 1000000;
                                      checkRef += chapterNum * 1000;
                                      checkRef += verseNum;

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
                                          || verseText.Trim().Count() < 1)
                                      {
                                          continue;
                                      }

                                      resultItems.Clear();
                                      CheckVerse(bookNum, chapterNum, verseNum, verseText, resultItems);

                                      foreach (ResultItem resultItem in resultItems)
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
                              HostUtil.Instance.ReportError($"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{_activeProjectName}\"", ex);
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
            });
            workThread.IsBackground = true;
            workThread.Start();

            // busy-wait until helper thead is done,
            // keeping the UI responsive w/DoEvents()
            int threadSleepInMs = (int)(1000f / (float)MainConsts.CHECK_EVENTS_UPDATE_RATE_IN_FPS);
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
