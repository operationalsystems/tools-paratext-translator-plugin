using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TvpMain.Check;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Result
{
    /// <summary>
    /// Manages access to results for a given project,
    /// including saving and loading to the project file area.
    /// </summary>
    public class ResultManager : IDisposable
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _projectName;

        /// <summary>
        /// Lock for in-memory sets and maps.
        /// </summary>
        private readonly object _resultLock;

        /// <summary>
        /// Book numbers to save at next opportunity.
        /// </summary>
        private readonly ISet<int> _bookNumsToSave;

        /// <summary>
        /// Dictionary of result items, indexed by verse location.
        /// </summary>
        private readonly IDictionary<VerseLocation, IList<ResultItem>> _resultItems;

        /// <summary>
        /// Current cancellation token for scheduled save operation (may be null).
        /// </summary>
        private CancellationTokenSource _currTokenSource;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="projectName">Active project name (required).</param>
        public ResultManager(IHost host, string projectName)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _projectName = projectName
                           ?? throw new ArgumentNullException(nameof(projectName));

            _bookNumsToSave = new HashSet<int>();
            _resultItems = new SortedDictionary<VerseLocation, IList<ResultItem>>();
            _resultLock = new object();
        }

        /// <summary>
        /// Sets (merges) result items with existing ones for a given verse
        /// and schedules the book-level results to be subsequently persisted.
        /// 
        /// This is designed to support capabilities generating result item
        /// sets for entire verses, such as TextCheckRunner.
        /// 
        /// Merge criteria:
        /// - New result items not matching an existing one will be added
        /// to the list for the verse
        /// - New result items matching an existing one except for result
        /// state (e.g., ignored) will be dropped in favor of the existing one
        /// - Existing result items matching the supplied check types and
        /// contexts and not matching a new result item will be dropped
        /// 
        /// Note: inputTypes and inputContexts represent types and contexts
        /// to be replaced for the verse. If inputItems is empty, all existing
        /// result items from these contexts will be replaced.
        /// </summary>
        /// <param name="inputTypes">Check types to replace results for (required).</param>
        /// <param name="inputContexts">Part contexts to replace results for (required).</param>
        /// <param name="inputLocation">Verse location to replace results for (required).</param>
        /// <param name="inputItems">New result items to set (merge; required).</param>
        /// <param name="outputItems">Merged list of items for this verse (provided).</param>
        /// <returns>True if input items are the first items for the verse (if non-empty) or has removed a verse (if empty), false otherwise.</returns>
        public bool SetVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            VerseLocation inputLocation,
            IEnumerable<ResultItem> inputItems,
            out IList<ResultItem> outputItems)
        {
            var result = false;
            lock (_resultLock)
            {
                if (inputItems.Any())
                {
                    var toSet = new List<ResultItem>();
                    if (_resultItems.TryGetValue(inputLocation,
                        out var foundItems))
                    {
                        // Add new items that don't match existing ones
                        // (and) keep existing ones that match new ones
                        // (e.g., previous results that have been ignored).
                        toSet.AddRange(inputItems.Select(inputItem =>
                            foundItems.FirstOrDefault(foundItem =>
                                Equals(inputItem, foundItem)) ?? inputItem));

                        // Also keep existing results that don't match
                        // supplied check types and contexts
                        toSet.AddRange(foundItems.Where(foundItem =>
                            !inputTypes.Contains(foundItem.CheckType)
                            || !inputContexts.Contains(foundItem.PartLocation.PartContext)));
                    }
                    else
                    {
                        result = true;
                        toSet.AddRange(inputItems);
                    }

                    _resultItems[inputLocation] = toSet;
                    outputItems = toSet.ToImmutableList();

                    ScheduleSaveBooks(inputLocation.BookNum);
                }
                else // empty input = remove any existing items for verse
                {
                    result = _resultItems.Remove(inputLocation);
                    outputItems = Enumerable.Empty<ResultItem>().ToImmutableList();

                    if (result)
                    {
                        ScheduleSaveBooks(inputLocation.BookNum);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Add or replace one verse result item, typically to change its state
        /// (e.g., to ignore it).
        /// </summary>
        /// <param name="inputItem">Input item (required).</param>
        /// <returns>True if this is the first item for the verse, false otherwise.</returns>
        public bool SetVerseResult(ResultItem inputItem)
        {
            var result = false;
            lock (_resultLock)
            {
                var toSet = new List<ResultItem>();
                if (_resultItems.TryGetValue(inputItem.VerseLocation,
                    out var foundItems))
                {
                    toSet.Add(inputItem);
                    toSet.AddRange(foundItems.Where(foundItem =>
                        !Equals(foundItem, inputItem)));
                }
                else
                {
                    result = true;
                    toSet.Add(inputItem);
                }
                _resultItems[inputItem.VerseLocation] = toSet;
                ScheduleSaveBooks(inputItem.VerseLocation.BookNum);
            }
            return result;
        }

        /// <summary>
        /// Gets verse results for given location, if present.
        /// </summary>
        /// <param name="inputTypes">Check types to include (optional, may be null; null = all).</param>
        /// <param name="inputContexts">Part contexts to include  (optional, may be null; null = all).</param>
        /// <param name="inputLocation">Verse to retrieve for (required).</param>
        /// <param name="outputItems">Output items to populate (provided).</param>
        /// <returns>True if any items found, false otherwise.</returns>
        public bool TryGetVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            VerseLocation inputLocation, out IList<ResultItem> outputItems)
        {
            outputItems = null;
            lock (_resultLock)
            {
                if (_resultItems.TryGetValue(inputLocation, out var foundItems))
                {
                    outputItems = foundItems
                        .Where(foundItem => inputTypes == null
                            || inputTypes.Contains(foundItem.CheckType))
                        .Where(foundItem => inputContexts == null
                            || inputContexts.Contains(foundItem.PartLocation.PartContext))
                        .ToImmutableList();
                }
            }
            return (outputItems != null);
        }

        /// <summary>
        /// Gets all verse results, for all locations.
        /// </summary>
        /// <param name="inputTypes">Check types to include (optional, may be null; null = all).</param>
        /// <param name="inputContexts">Part contexts to include  (optional, may be null; null = all).</param>
        /// <returns>Found verse results, if any.</returns>
        public IList<ResultItem> GetAllVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts)
        {
            lock (_resultLock)
            {
                return _resultItems
                    .Values
                    .SelectMany(listItem => listItem)
                    .Where(foundItem => inputTypes == null
                        || inputTypes.Contains(foundItem.CheckType))
                    .Where(foundItem => inputContexts == null
                        || inputContexts.Contains(foundItem.PartLocation.PartContext))
                    .ToImmutableList();
            }
        }

        /// <summary>
        /// Schedules a given book number for persistence in the near future.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        private void ScheduleSaveBooks(int bookNum)
        {
            lock (_resultLock)
            {
                _bookNumsToSave.Add(bookNum);

                CancelSaveBooks();
                var currToken = _currTokenSource.Token;

                // call does _not_ block
                Task.Delay(TimeSpan.FromSeconds(MainConsts.RESULT_ITEM_SAVE_DELAY_IN_SEC), currToken)
                    .ContinueWith(taskItem =>
                    {
                        try
                        {
                            if (currToken.IsCancellationRequested)
                            {
                                return;
                            }
                            lock (_resultLock)
                            {
                                if (_bookNumsToSave.Count <= 0)
                                {
                                    return;
                                }
                                SaveBooks(_bookNumsToSave);
                                _bookNumsToSave.Clear();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Ignore (can occur w/cancel).
                        }
                        catch (Exception ex)
                        {
                            HostUtil.Instance.ReportError(
                                "Can't save result items", true, ex);
                        }
                    }, currToken);
            }
        }

        /// <summary>
        /// Save books for a provided set of book numbers.
        /// </summary>
        /// <param name="inputBooks">Book numbers (1-based).</param>
        public void SaveBooks(ISet<int> inputBooks)
        {
            if (inputBooks.Count < 1)
            {
                return;
            }
            IDictionary<int, IEnumerable<ResultItem>> booksToSave = null;
            lock (_resultLock)
            {
                booksToSave = _resultItems
                    .Where(pairItem =>
                        inputBooks.Contains(pairItem.Key.BookNum))
                    .GroupBy(pairItem => pairItem.Key.BookNum)
                    .ToImmutableDictionary(groupItem =>
                        groupItem.Key,
                groupItem =>
                                groupItem.SelectMany(pairItem =>
                                    pairItem.Value));
            }

            foreach (var pairItem in booksToSave)
            {
                if (BookUtil.BookIdsByNum.TryGetValue(pairItem.Key, out var bookId))
                {
                    HostUtil.Instance.PutResultItems(_projectName, bookId.BookCode, pairItem.Value);
                }
            }
        }

        /// <summary>
        /// Load books from a provided set of book numbers.
        /// </summary>
        /// <param name="inputBooks">Book numbers (1-based).</param>
        public void LoadBooks(ISet<int> inputBooks)
        {
            if (inputBooks.Count < 1)
            {
                return;
            }
            lock (_resultLock)
            {
                foreach (var bookNum in inputBooks)
                {
                    if (BookUtil.BookIdsByNum.TryGetValue(bookNum, out var bookId))
                    {
                        foreach (var foundItem in
                            HostUtil.Instance.GetResultItems(_projectName, bookId.BookCode))
                        {
                            SetVerseResult(foundItem);
                        }
                    }
                }
            }
        }

        private void CancelSaveBooks()
        {
            lock (_resultLock)
            {
                if (_currTokenSource != null)
                {
                    try
                    {
                        _currTokenSource.Cancel();
                    }
                    finally
                    {
                        _currTokenSource.Dispose();
                    }
                }
                _currTokenSource = new CancellationTokenSource();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            CancelSaveBooks();
        }
    }
}
