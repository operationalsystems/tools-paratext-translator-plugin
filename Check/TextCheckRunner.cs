/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Import;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Check
{
    /// <summary>
    /// Parallelizes concrete text checks across one or more books.
    /// 
    /// This is preferable because extracting verses process boundaries and is therefore
    /// CPU and I/O intensive, so for best results we want (n) checks performed on each
    /// verse, vs each check extracting all verses (n) times. 
    /// </summary>
    public class TextCheckRunner : IDisposable
    {
        /// <summary>
        /// Lock for publishing check progress.
        /// </summary>
        private readonly object _progressLock = new object();

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
        private CancellationTokenSource _runTokenSource;

        /// <summary>
        /// Project settings manager.
        /// </summary>
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Project content import manager.
        /// </summary>
        private readonly ImportManager _importManager;

        /// <summary>
        /// Check result manager.
        /// </summary>
        private readonly ResultManager _resultManager;

        /// <summary>
        /// Current book number at run start (1-based indexing).
        /// </summary>
        private int _runBookNum;

        /// <summary>
        /// Current chapter number at run start (1-based indexing).
        /// </summary>
        private int _runChapterNum;

        /// <summary>
        /// Current verse number at run start (0-based; 0 = intro, 1+ = verses).
        /// </summary>
        private int _runVerseNum;

        /// <summary>
        /// Total number of books in run (1-based indexing).
        /// </summary>
        private int _runTotalBooks;

        /// <summary>
        /// Progress of current run (1-based indexing).
        /// </summary>
        private int _runBookCtr;

        /// <summary>
        /// Current run's area.
        /// </summary>
        private CheckArea _runArea;

        /// <summary>
        /// Current run's list of checks.
        /// </summary>
        private IList<ITextCheck> _runChecks;

        /// <summary>
        /// Current run's list of check types.
        /// </summary>
        private ISet<CheckType> _runCheckTypes;

        /// <summary>
        /// Current run's contexts.
        /// </summary>
        private ISet<PartContext> _runContexts;

        /// <summary>
        /// Current run's task semaphore.
        /// </summary>
        private SemaphoreSlim _runSemaphore;

        /// <summary>
        /// Current run's result collection.
        /// </summary>
        private CheckResults _runResults;

        /// <summary>
        /// Current run's earliest exception.
        /// </summary>
        private Exception _runEx;

        /// <summary>
        /// True to merge and save results with previous,
        /// false to only return current results.
        /// </summary>
        private bool _runSaveResults;

        /// <summary>
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="projectManager">Settings manager (required).</param>
        /// <param name="importManager">Project content import manager (required).</param>
        /// <param name="resultManager">Check result manager (required).</param>
        public TextCheckRunner(IHost host, string activeProjectName,
            ProjectManager projectManager,
            ImportManager importManager,
            ResultManager resultManager)
        {
            this._host = host
                         ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName
                                      ?? throw new ArgumentNullException(nameof(activeProjectName));
            this._projectManager = projectManager
                                   ?? throw new ArgumentNullException(nameof(projectManager));
            this._importManager = importManager
                                   ?? throw new ArgumentNullException(nameof(importManager));
            this._resultManager = resultManager
                                  ?? throw new ArgumentNullException(nameof(resultManager));
        }

        /// <summary>
        /// Cancels a running check.
        /// </summary>
        public void CancelChecks()
        {
            _runTokenSource?.Cancel();
        }

        private void OnCheckUpdated(int updateBookNum)
        {
            lock (_progressLock)
            {
                _runBookCtr++;
                CheckUpdated?.Invoke(this,
                    new CheckUpdatedArgs(_runTotalBooks, _runBookCtr, updateBookNum));
            }
        }

        /// <summary>
        /// Runs a check.
        /// </summary>
        /// <param name="inputArea">Check area (i.e., scope; required).</param>
        /// <param name="inputChecks">List of checks to execute (required).</param>
        /// <param name="inputContexts">Text contexts (>0 required).</param>
        /// <param name="isSaveResults">True to merge and save results with previous, false to only return current results.</param>
        /// <param name="outputResults">Output results (populated if run completes normally, null otherwise).</param>
        /// <returns>True if run completes normally, false otherwise.</returns>
        public bool RunChecks(CheckArea inputArea,
            IEnumerable<ITextCheck> inputChecks,
            IEnumerable<PartContext> inputContexts,
            bool isSaveResults,
            int overrideBookNum,
            out CheckResults outputResults)
        {
            if (inputChecks == null)
            {
                HostUtil.Instance.ReportError("Input checks for running checks is null", new TextCheckException("Input checks for running checks is null"));
                outputResults = null;
                return false;
            }
            if (inputContexts == null)
            {
                HostUtil.Instance.ReportError("Input contexts for running checks is null", new TextCheckException("Input contexts for running checks is null"));
                outputResults = null;
                return false;
            }

            // create run-based utilities
            _runArea = inputArea;
            _runChecks = inputChecks.ToImmutableList();
            _runCheckTypes = _runChecks.Select(value => value.CheckType)
                .ToImmutableHashSet();
            _runContexts = inputContexts.ToImmutableHashSet();
            _runSaveResults = isSaveResults;
            _runResults = new CheckResults();
            _runEx = null;
            _runBookCtr = 0;

            var versificationName = _host.GetProjectVersificationName(_activeProjectName);

            // set up semaphore and cancellation token to control execution and termination
            RecycleRunSemaphore(true);
            RecycleRunTokenSource(true);

            // get user's location in Paratext
            BookUtil.RefToBcv(_host.GetCurrentRef(versificationName),
                out _runBookNum, out _runChapterNum, out _runVerseNum);

            // set up semaphore for parallelism control, results set, and task list
            var taskList = new List<Task>();
            if (_runArea == CheckArea.CurrentProject)
            {
                var bookNums = _projectManager.PresentBookNums.ToList();
                _runTotalBooks = bookNums.Count;

                bookNums.Sort(); // sort to visually run in ~expected order
                taskList.AddRange(bookNums.Select(RunBookTask));
            }
            else
            {
                _runTotalBooks = 1;
                if(overrideBookNum != -1)
                {
                    _runBookNum = overrideBookNum;
                }
                taskList.Add(RunBookTask(_runBookNum));
            }

            var workThread = new Thread(() =>
                {
                    try
                    {
                        Task.WaitAll(taskList.ToArray(), _runTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore (can occur w/cancel).
                    }
                    catch (Exception ex)
                    {
                        var messageText =
                            $"Error: Can't check area: {inputArea} in project: \"{_activeProjectName}\" (error: {ex.Message}).";

                        _runEx ??= new TextCheckException(messageText, ex);
                        HostUtil.Instance.ReportError(messageText, _runEx);
                    }
                })
            { IsBackground = true };
            workThread.Start();

            // busy-wait until helper thread is done,
            // keeping the UI responsive w/DoEvents()
            while (workThread.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(MainConsts.CHECK_EVENTS_DELAY_IN_MSEC);
            }

            // populate output
            if (_runTokenSource.IsCancellationRequested)
            {
                outputResults = null;
                return false;
            }
            else
            {
                outputResults = _runResults;
                return true;
            }
        }

        /// <summary>
        /// Recycle (dispose and optionally re-create) run semaphore.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunSemaphore(bool isCreateNew)
        {
            _runSemaphore?.Dispose();
            _runSemaphore = isCreateNew
                ? new SemaphoreSlim(MainConsts.MAX_CHECK_THREADS)
                : null;
        }


        /// <summary>
        /// Recycle (dispose and optionally re-create) run cancellation token.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunTokenSource(bool isCreateNew)
        {
            _runTokenSource?.Dispose();
            _runTokenSource = isCreateNew
                ? new CancellationTokenSource()
                : null;
        }

        /// <summary>
        /// Creates and runs a check task for a given book number.
        /// </summary>
        /// <param name="inputBookNum">Book number (1-based).</param>
        private Task RunBookTask(
            int inputBookNum)
        {
            return Task.Run(() =>
            {
                // wait to get started
                _runSemaphore.Wait();

                // track where we are for error reporting
                var currBookNum = inputBookNum;
                var currChapterNum = 0;
                var currVerseNum = 0;

                try
                {
                    // get utility items
                    var versificationName = _host.GetProjectVersificationName(_activeProjectName);

                    var checkList = _runChecks.ToList();
                    var emptyVerseCtr = 0;

                    var resultItems = new List<ResultItem>();
                    var foundParts = new List<VersePart>();

                    // determine chapter range using check area and user's location in Paratext
                    var minChapter = (_runArea == CheckArea.CurrentChapter)
                        ? _runChapterNum
                        : 1;
                    var maxChapter = (_runArea == CheckArea.CurrentChapter)
                        ? _runChapterNum
                        : _host.GetLastChapter(inputBookNum, versificationName);

                    // iterate chapters
                    for (var chapterNum = minChapter;
                        chapterNum <= maxChapter;
                        chapterNum++)
                    {
                        currChapterNum = chapterNum;
                        if (_runTokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        // find verse number range & iterate
                        var lastVerseNum = _host.GetLastVerse(inputBookNum, chapterNum, versificationName);
                        for (var verseNum = 0; // verse 0 = intro text
                            verseNum <= lastVerseNum;
                            verseNum++)
                        {
                            currVerseNum = verseNum;
                            if (_runTokenSource.IsCancellationRequested)
                            {
                                return;
                            }

                            resultItems.Clear();

                            try
                            {
                                var verseLocation = new VerseLocation(inputBookNum, chapterNum, verseNum);
                                var verseText = _importManager.Extract(verseLocation);

                                // empty text = consecutive check, in case we're looking at an empty chapter
                                if (string.IsNullOrWhiteSpace(verseText))
                                {
                                    emptyVerseCtr++;
                                    if (emptyVerseCtr > MainConsts.MAX_CONSECUTIVE_EMPTY_VERSES)
                                    {
                                        break; // no beginning text = empty chapter (skip)
                                    }
                                } // else, take apart text
                                else
                                {
                                    emptyVerseCtr = 0;

                                    // clear out part lists & get ready to create new ones
                                    foundParts.Clear();
                                    var verseData = new ProjectVerse(verseLocation, verseText);

                                    // find verse parts
                                    if (VerseRegexUtil.FindVerseParts(
                                        verseData, _runContexts, foundParts))
                                    {
                                        // run checks on each part
                                        foreach (var partItem in foundParts
                                            .Where(partItem => !string.IsNullOrWhiteSpace(partItem.PartText)))
                                        {
                                            foreach (var checkItem in _runChecks)
                                            {
                                                checkItem.CheckText(partItem, resultItems);
                                            }
                                        }
                                    }

                                    IList<ResultItem> outputItems = resultItems;
                                    if (_runSaveResults)
                                    {
                                        _resultManager.SetVerseResults(
                                            _runCheckTypes, _runContexts,
                                            verseData.VerseLocation, resultItems,
                                            out outputItems);
                                    }
                                    foreach (var outputItem in outputItems)
                                    {
                                        _runResults.ResultItems.Enqueue(outputItem);
                                    }
                                }
                            }
                            catch (ArgumentException)
                            {
                                // arg exceptions occur when verses are missing, 
                                // which they can be for given translations (ignore and move on)
                                continue;
                            }
                        }
                    }

                    OnCheckUpdated(inputBookNum);
                }
                catch (OperationCanceledException)
                {
                    // Ignore (can occur w/cancel).
                }
                catch (Exception ex)
                {
                    var messageText =
                        $"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{_activeProjectName}\" (error: {ex.Message}).";

                    _runEx ??= new TextCheckException(messageText, ex);
                    HostUtil.Instance.ReportError(messageText, ex);
                }
                finally
                {
                    _runSemaphore.Release();
                }
            });
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            RecycleRunSemaphore(false);
            RecycleRunTokenSource(false);
        }
    }

    /// <summary>
    /// Basic exception for text check errors.
    /// </summary>
    public class TextCheckException : ApplicationException, ISerializable
    {
        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="messageText">Message text (optional, may be null).</param>
        /// <param name="causeEx">Cause exception (optional, may be null).</param>
        public TextCheckException(string messageText, Exception causeEx)
            : base(messageText, causeEx)
        {
        }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="messageText">Message text (optional, may be null).</param>
        public TextCheckException(string messageText)
            : base(messageText)
        {
        }
    }
}
