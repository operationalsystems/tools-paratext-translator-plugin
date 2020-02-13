using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AddInSideViews;
using TvpMain.Check;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Export
{
    /// <summary>
    /// Saves Paratext project content to a target folder or back to the project.
    /// </summary>
    public class ExportManager : IDisposable
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
        /// Project settings manager.
        /// </summary>
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Check results manager.
        /// </summary>
        private readonly ResultManager _resultManager;

        /// <summary>
        /// Current run's task semaphore.
        /// </summary>
        private SemaphoreSlim _runSemaphore;

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource _runTokenSource;

        /// <summary>
        /// Target directory for run.
        /// </summary>
        private DirectoryInfo _runTargetDir;

        /// <summary>
        /// Current run's earliest exception.
        /// </summary>
        private Exception _runEx;

        /// <summary>
        /// Progress of current run.
        /// </summary>
        private int _runBookCtr;

        /// <summary>
        /// Total number of books in run.
        /// </summary>
        private int _runTotalBooks;

        /// <summary>
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<ExportUpdatedArgs> ExportUpdated;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="projectManager">Project settings manager (required).</param>
        /// <param name="resultManager">Check result manager (required).</param>
        public ExportManager(IHost host, string activeProjectName,
            ProjectManager projectManager,
            ResultManager resultManager)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            _resultManager = resultManager
                                  ?? throw new ArgumentNullException(nameof(resultManager));
        }

        /// <summary>
        /// Export project as USFM to a target directory, replacing parts of verses
        /// with check result suggestions that haven't been ignored.
        /// </summary>
        /// <param name="targetDir">Target directory (required).</param>
        /// <param name="isChangesOnly">True to only export books with result suggestions that haven't been ignored, false to export all books.</param>
        public bool ExportProject(
            DirectoryInfo targetDir,
            bool isChangesOnly)
        {
            _runTargetDir = targetDir;
            _runEx = null;
            _runBookCtr = 0;

            // set up semaphore and cancellation token to control execution and termination
            RecycleRunSemaphore(true);
            RecycleRunTokenSource(true);

            var bookNums = _projectManager.PresentBookNums.ToList();
            _runTotalBooks = bookNums.Count;

            _resultManager.TryGetBookResults(null, null, true,
                bookNums, out var bookResults);

            bookNums.Sort(); // sort to visually run in ~expected order
            var taskList = bookNums
                .Select(foundNum =>
                {
                    if (bookResults.TryGetValue(foundNum, out var foundItems)
                        && foundItems.Any())
                    {
                        return RunBookTask(foundNum, foundItems);
                    }
                    else if (!isChangesOnly)
                    {
                        return RunBookTask(foundNum, Enumerable.Empty<ResultItem>());
                    }

                    return null;
                })
                .Where(foundTask =>
                    foundTask != null);

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
                            $"Error: Can't export project: \"{_activeProjectName}\" to: {targetDir.FullName} (error: {ex.Message}).";

                        _runEx ??= new TextCheckException(messageText, ex);
                        HostUtil.Instance.ReportError(messageText, ex);
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

            // return whether run completed normally
            return !_runTokenSource.IsCancellationRequested;
        }

        /// <summary>
        /// Creates and runs an export task for a given book number.
        /// </summary>
        /// <param name="inputBookNum">Book number (1-based).</param>
        /// <param name="inputItems">Input items to include in output files (required).</param>
        /// <returns></returns>
        private Task RunBookTask(
            int inputBookNum,
            IEnumerable<ResultItem> inputItems)
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
                    // collate result items for quick reference
                    var verseItems = inputItems
                        .GroupBy(foundItem => foundItem.VerseLocation)
                        .ToImmutableDictionary(
                            groupItem => groupItem.Key,
                            groupItem => groupItem.ToImmutableList());

                    // get temp output file
                    var fileName = GetUsfmFileName(inputBookNum);
                    var tempDir = new DirectoryInfo(Path.GetTempPath());

                    using (var tempStream = _projectManager.FileManager.GetOutputFile(
                            tempDir, fileName, true, true))
                    {
                        using var tempWriter = new StreamWriter(tempStream);

                        // get utility items
                        var versificationName = _host.GetProjectVersificationName(_activeProjectName);
                        var scriptureExtractor = _host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM);

                        scriptureExtractor.IncludeNotes = true;
                        var emptyVerseCtr = 0;

                        // iterate chapters
                        var lastChapterNum = _host.GetLastChapter(inputBookNum, versificationName);
                        for (var chapterNum = 1;
                            chapterNum <= lastChapterNum;
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

                                var checkRef = BookUtil.BcvToRef(inputBookNum, chapterNum, verseNum);

                                try
                                {
                                    var verseText = scriptureExtractor.Extract(checkRef, checkRef);

                                    // empty text = consecutive check, in case we're looking at an empty chapter
                                    if (string.IsNullOrWhiteSpace(verseText))
                                    {
                                        emptyVerseCtr++;
                                        if (emptyVerseCtr > MainConsts.MAX_CONSECUTIVE_EMPTY_VERSES)
                                        {
                                            break; // no beginning text = empty chapter (skip)
                                        }
                                    } // else, replace suggestions and 
                                    else
                                    {
                                        emptyVerseCtr = 0;
                                        var verseData = ProjectVerse.Create(
                                            inputBookNum, chapterNum, verseNum,
                                            verseText);

                                        tempWriter.Write(verseItems.TryGetValue(verseData.VerseLocation,
                                            out var foundItems)
                                            ? ReplaceSuggestionText(verseData.VerseText, foundItems)
                                            : verseData.VerseText);
                                        tempWriter.WriteLine();
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

                        OnExportUpdated(inputBookNum);
                    }

                    using (var tempStream = _projectManager.FileManager.GetInputFile(tempDir, fileName))
                    {
                        using var outputStream = _projectManager.FileManager.GetOutputFile(
                            _runTargetDir, fileName, true, true);
                        tempStream.CopyTo(outputStream);
                        outputStream.Flush();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ignore (can occur w/cancel).
                }
                catch (Exception ex)
                {
                    var messageText =
                        $"Error: Can't export location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{_activeProjectName}\" (error: {ex.Message}).";

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
        /// Replace verse content with suggestion text.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="inputItems">Input result items (required).</param>
        /// <returns></returns>
        public string ReplaceSuggestionText(string inputText, IEnumerable<ResultItem> inputItems)
        {
            // filter to just usable items
            var inputItemsList = inputItems
                .Where(inputItem =>
                    inputItem.ResultState != ResultState.Ignored
                    && inputItem.SuggestionText != null)
                .ToImmutableList();

            // no items = done
            if (inputItemsList.IsEmpty)
            {
                return inputText;
            }

            // now check for out of bounds and missing items
            if (inputItemsList.Any(inputItem =>
                (inputItem.MatchStart + inputItem.MatchLength) > inputText.Length
                || !inputText.Contains(inputItem.MatchText)))
            {
                throw new ArgumentException("check match not found; verse has probably changed since last check");
            }

            // iterate and replace
            var workBuilder = new StringBuilder(inputText);
            var offsetCtr = 0;

            foreach (var inputItem in inputItemsList)
            {
                var matchStart = inputItem.MatchStart + offsetCtr;

                workBuilder.Remove(matchStart, inputItem.MatchLength);
                workBuilder.Insert(matchStart, inputItem.SuggestionText);

                offsetCtr += (inputItem.SuggestionText.Length - inputItem.MatchText.Length);
            }

            return workBuilder.ToString();
        }

        /// <summary>
        /// Get USFM file name for a given book number.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <returns>USFM file name.</returns>
        private string GetUsfmFileName(int bookNum)
        {
            if (BookUtil.BookIdsByNum.TryGetValue(bookNum, out var idItem))
            {
                return $"{idItem.UsfmBookNum}{idItem.BookCode}{_activeProjectName}.SFM";
            }
            else
            {
                throw new ArgumentException($"invalid book number: {bookNum}");
            }
        }

        /// <summary>
        /// Distributes update events.
        /// </summary>
        /// <param name="updateBookNum">Book number that's completed.</param>
        private void OnExportUpdated(int updateBookNum)
        {
            lock (_progressLock)
            {
                _runBookCtr++;
                ExportUpdated?.Invoke(this,
                    new ExportUpdatedArgs(_runTotalBooks, _runBookCtr, updateBookNum));
            }
        }

        /// <summary>
        /// Cancels a running export.
        /// </summary>
        public void CancelExport()
        {
            _runTokenSource?.Cancel();
        }

        /// <summary>
        /// Recycle (dispose and optionally re-create) run semaphore.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunSemaphore(bool isCreateNew)
        {
            _runSemaphore?.Dispose();
            _runSemaphore = isCreateNew
                ? new SemaphoreSlim(MainConsts.MAX_EXPORT_THREADS)
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

        /// <inheritdoc />
        public virtual void Dispose()
        {
            RecycleRunSemaphore(false);
            RecycleRunTokenSource(false);
        }
    }
}
