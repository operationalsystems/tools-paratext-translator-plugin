using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AddInSideViews;
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
    public class TextCheckRunner
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
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// Project settings manager.
        /// </summary>
        private SettingsManager _settingsManager;

        /// <summary>
        /// Current book number at run start (1-based).
        /// </summary>
        private int _runBookNum;

        /// <summary>
        /// Current chapter number at run start (1-based).
        /// </summary>
        private int _runChapterNum;

        /// <summary>
        /// Current verse number at run start (0-based; 0 = intro, 1+ = verses).
        /// </summary>
        private int _runVerseNum;

        /// <summary>
        /// Total number of books in run.
        /// </summary>
        private int _runTotalBooks;

        /// <summary>
        /// Progress of current run.
        /// </summary>
        private int _runBookCtr;

        /// <summary>
        /// Current run's area.
        /// </summary>
        private CheckArea _runArea;

        /// <summary>
        /// Current run's check list.
        /// </summary>
        private IList<ITextCheck> _runChecks;

        /// <summary>
        /// Current run's contexts.
        /// </summary>
        private ISet<TextContext> _runContexts;

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
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="settingsManager">Settings manager (required).</param>
        public TextCheckRunner(IHost host, string activeProjectName, SettingsManager settingsManager)
        {
            this._host = host
                         ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName
                                      ?? throw new ArgumentNullException(nameof(activeProjectName));
            this._settingsManager = settingsManager
                                    ?? throw new ArgumentNullException(nameof(settingsManager));
        }

        /// <summary>
        /// Cancels a running check.
        /// </summary>
        public void CancelChecks()
        {
            _tokenSource?.Cancel();
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
        /// <param name="outputResults">Output results (populated if run completes normally, null otherwise).</param>
        /// <returns>True if run completes normally, false otherwise.</returns>
        public bool RunChecks(CheckArea inputArea,
            IEnumerable<ITextCheck> inputChecks,
            IEnumerable<TextContext> inputContexts,
            out CheckResults outputResults)
        {
            // create run-based utilities
            _runArea = inputArea;
            _runChecks = inputChecks.ToImmutableList();
            _runContexts = inputContexts.ToImmutableHashSet();
            _runResults = new CheckResults();
            _runEx = null;
            _runBookCtr = 0;

            var versificationName = _host.GetProjectVersificationName(_activeProjectName);

            // set up semaphore and cancellation token to control execution and termination
            _runSemaphore = new SemaphoreSlim(MainConsts.MAX_CHECK_THREADS);
            _tokenSource = new CancellationTokenSource();

            // get user's location in Paratext
            TextUtil.RefToBcv(_host.GetCurrentRef(versificationName),
                out _runBookNum, out _runChapterNum, out _runVerseNum);

            // set up semaphore for parallelism control, results set, and task list
            var taskList = new List<Task>();
            if (_runArea == CheckArea.CurrentProject)
            {
                var bookNums = _settingsManager.PresentBookNums.ToList();
                _runTotalBooks = bookNums.Count;

                bookNums.Sort(); // sort to visually run in ~expected order
                taskList.AddRange(bookNums.Select(RunBookTask));
            }
            else
            {
                _runTotalBooks = 1;
                taskList.Add(RunBookTask(_runBookNum));
            }

            var workThread = new Thread(() =>
                {
                    try
                    {
                        Task.WaitAll(taskList.ToArray(), _tokenSource.Token);
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
                        HostUtil.Instance.ReportError(messageText, ex);
                    }
                })
            { IsBackground = true };
            workThread.Start();

            // busy-wait until helper thread is done,
            // keeping the UI responsive w/DoEvents()
            const int threadSleepInMs = (int)(1000f / (float)MainConsts.CHECK_EVENTS_UPDATE_RATE_IN_FPS);
            while (workThread.IsAlive)
            {
                Application.DoEvents();
                Thread.Sleep(threadSleepInMs);
            }

            // populate output
            if (_tokenSource.IsCancellationRequested)
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
        /// Creates and runs a check task for a given book number.
        /// </summary>
        /// <param name="inputBookNum">Book number (1-based).</param>
        /// <returns></returns>
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
                    var mainExtractor = _host.GetScriptureExtractor(
                        _activeProjectName, ExtractorType.USFM);
                    var noteExtractor = _host.GetScriptureExtractor(
                        _activeProjectName, ExtractorType.USFM);
                    noteExtractor.IncludeNotes = true;

                    var checkList = _runChecks.ToList();
                    string[] splitArray = { "" };
                    var emptyVerseCtr = 0;

                    // determine chapter range using check area and user's location in Paratext
                    var minChapter = (_runArea != CheckArea.CurrentChapter)
                        ? 1
                        : _runChapterNum;
                    var maxChapter = (_runArea != CheckArea.CurrentChapter)
                        ? _host.GetLastChapter(inputBookNum, versificationName)
                        : _runChapterNum;

                    var mainParts = new List<string>();
                    var introParts = new List<string>();
                    var tocParts = new List<string>();
                    var noteParts = new List<string>();

                    var resultItems = new List<ResultItem>();
                    for (var chapterNum = minChapter;
                        chapterNum <= maxChapter;
                        chapterNum++)
                    {
                        currChapterNum = chapterNum;
                        if (_tokenSource.IsCancellationRequested)
                        {
                            return;
                        }

                        var lastVerseNum = _host.GetLastVerse(inputBookNum, chapterNum, versificationName);

                        for (var verseNum = 0;
                            verseNum <= lastVerseNum;
                            verseNum++)
                        {
                            currVerseNum = verseNum;
                            if (_tokenSource.IsCancellationRequested)
                            {
                                return;
                            }

                            var checkRef = TextUtil.BcvToRef(inputBookNum, chapterNum, verseNum);
                            resultItems.Clear();

                            string mainText = null;
                            if (_runContexts.Contains(TextContext.MainText))
                            {
                                try
                                {
                                    mainText = mainExtractor.Extract(checkRef, checkRef);

                                    // check main text, if present
                                    if (!string.IsNullOrWhiteSpace(mainText))
                                    {
                                        mainParts.Clear();
                                        tocParts.Clear();
                                        introParts.Clear();

                                        if (TextUtil.FindMainParts(mainText, mainParts, introParts, tocParts))
                                        {
                                            if (mainParts.Count > 0)
                                            {
                                                // run checks on main text
                                                RunBookChecks(
                                                    new TextLocation(inputBookNum, chapterNum, verseNum,
                                                        TextContext.MainText),
                                                    mainParts,
                                                    resultItems);
                                            }
                                            if (introParts.Count > 0)
                                            {
                                                // run checks on intro lines
                                                RunBookChecks(
                                                    new TextLocation(inputBookNum, chapterNum, verseNum,
                                                        TextContext.Introduction),
                                                    introParts,
                                                    resultItems);
                                            }
                                            if (tocParts.Count > 0)
                                            {
                                                // run checks on TOC lines
                                                RunBookChecks(
                                                    new TextLocation(inputBookNum, chapterNum, verseNum,
                                                        TextContext.TableOfContents),
                                                    introParts,
                                                    resultItems);
                                            }
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

                            string noteText = null;
                            if (_runContexts.Contains(TextContext.NoteOrReference))
                            {
                                try
                                {
                                    noteText = noteExtractor.Extract(checkRef, checkRef);

                                    // check main+note text, if present (and) different from main text
                                    if (!string.IsNullOrWhiteSpace(noteText)
                                        && (string.IsNullOrWhiteSpace(mainText) || mainText != noteText))
                                    {
                                        // filter out non-note text
                                        noteParts.Clear();
                                        if (TextUtil.FindNoteOrReferenceParts(noteText, noteParts))
                                        {
                                            // run checks on notes or references
                                            RunBookChecks(
                                                new TextLocation(inputBookNum, chapterNum, verseNum,
                                                    TextContext.NoteOrReference),
                                                noteParts,
                                                resultItems);
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

                            if (string.IsNullOrWhiteSpace(mainText)
                                && string.IsNullOrWhiteSpace(noteText))
                            {
                                emptyVerseCtr++;
                                if (emptyVerseCtr > MainConsts.MAX_CONSECUTIVE_EMPTY_VERSES)
                                {
                                    break; // no beginning text = empty chapter (skip)
                                }
                                else
                                {
                                    continue; // else, next verse
                                }
                            }
                            else
                            {
                                emptyVerseCtr = 0;
                            }

                            resultItems.ForEach(resultItem =>
                                _runResults.ResultItems.Enqueue(resultItem));
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

        /// <summary>
        /// Applies the current run's book checks to specific input.
        /// </summary>
        /// <param name="inputLocation">Input location (required).</param>
        /// <param name="inputParts">Input parts (required).</param>
        /// <param name="outputItems">Output result items (required).</param>
        private void RunBookChecks(
            TextLocation inputLocation,
            IEnumerable<string> inputParts,
            ICollection<ResultItem> outputItems)
        {
            foreach (var partItem in inputParts
                .Where(partItem => !string.IsNullOrWhiteSpace(partItem))
                .Select(partItem => partItem.Trim()))
            {
                foreach (var checkItem in _runChecks)
                {
                    checkItem.CheckVerse(inputLocation, partItem, outputItems);
                }
            }
        }
    }

    /// <summary>
    /// Basic exception for text check errors.
    /// </summary>
    public class TextCheckException : ApplicationException
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
    }
}
