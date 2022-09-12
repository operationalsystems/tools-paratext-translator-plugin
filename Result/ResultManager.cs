/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
    ///
    /// Disposable to ensure task shutdown.
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
        /// Book numbers to load at next opportunity.
        /// </summary>
        private readonly ISet<int> _bookNumsToLoad;

        /// <summary>
        /// Current cancellation token for scheduled load operation (may be null).
        /// </summary>
        private CancellationTokenSource _loadTokenSource;

        /// <summary>
        /// Book numbers to save at next opportunity.
        /// </summary>
        private readonly ISet<int> _bookNumsToSave;

        /// <summary>
        /// Current cancellation token for scheduled save operation (may be null).
        /// </summary>
        private CancellationTokenSource _saveTokenSource;

        /// <summary>
        /// Dictionary of result items, indexed by verse location.
        /// </summary>
        private readonly IDictionary<VerseLocation, IList<ResultItem>> _resultItems;

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
            _bookNumsToLoad = new HashSet<int>();

            _resultItems = new SortedDictionary<VerseLocation, IList<ResultItem>>();
            _resultLock = new object();
        }

        /// <summary>
        /// Sets (merges) result items with existing ones for a given verse
        /// and schedules the book-level results to be subsequently persisted.
        /// 
        /// This is designed to support capabilities generating result item
        /// sets for _entire_ verses, such as TextCheckRunner.
        /// 
        /// Merge criteria:
        /// - New result items not matching an existing one will be added
        /// to the list for the verse
        /// - New result items matching an existing one except for result
        /// state (e.g., "ignored") will be dropped in favor of the existing one
        /// - Existing result items matching the supplied check types and
        /// contexts but _not_ matching a new result item will be dropped
        /// 
        /// Note: inputTypes and inputContexts represent types and contexts
        /// to be replaced for the verse. If inputItems is empty, all existing
        /// result items from these contexts will be removed.
        /// </summary>
        /// <param name="inputTypes">Check types to replace results for (required).</param>
        /// <param name="inputContexts">Part contexts to replace results for (required).</param>
        /// <param name="inputLocation">Verse location to replace results for (required).</param>
        /// <param name="inputItems">New result items to set (merge; required).</param>
        /// <param name="outputItems">Merged list of items for this verse (provided).</param>
        /// <returns>True if there have been any changes or a verse removed (if empty), false otherwise.</returns>
        public bool SetVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            VerseLocation inputLocation,
            IEnumerable<ResultItem> inputItems,
            out IList<ResultItem> outputItems)
        {
            var result = false;
            var inputList = inputItems.ToImmutableList();

            lock (_resultLock)
            {
                if (inputList.Any())
                {
                    if (_resultItems.TryGetValue(inputLocation,
                        out var foundItems))
                    {
                        var toSet = new List<ResultItem>();

                        // Add new items that don't match existing ones
                        // (and) keep existing ones that match new ones
                        // (e.g., previous results that have been ignored).
                        toSet.AddRange(inputList.Select(inputItem =>
                            foundItems.FirstOrDefault(foundItem =>
                                Equals(inputItem, foundItem)) ?? inputItem));

                        // Also keep existing results that don't match
                        // supplied check types and contexts
                        toSet.AddRange(foundItems.Where(foundItem =>
                            !(inputTypes.Contains(foundItem.CheckType)
                             && inputContexts.Contains(foundItem.PartLocation.PartContext))));

                        outputItems = toSet.ToImmutableList();
                    }
                    else
                    {
                        outputItems = inputList;
                    }
                }
                else // empty input = remove any existing items for verse
                {
                    if (_resultItems.TryGetValue(inputLocation,
                        out var foundItems))
                    {
                        // Keep existing results that don't match
                        // supplied check types and contexts
                        outputItems = foundItems.Where(foundItem =>
                                !(inputTypes.Contains(foundItem.CheckType)
                                  && inputContexts.Contains(foundItem.PartLocation.PartContext)))
                            .ToImmutableList();
                    }
                    else
                    {
                        outputItems = Enumerable.Empty<ResultItem>()
                            .ToImmutableList();
                    }
                }

                // any results? add or update verse in map
                if (outputItems.Any())
                {
                    _resultItems[inputLocation] = outputItems;
                    result = true;
                }
                else // no results? remove verse from map
                {
                    result = _resultItems.Remove(inputLocation);
                }

                // save if there have been notable changes
                if (result)
                {
                    ScheduleSaveBooks(inputLocation.BookNum
                        .ToSingletonEnumerable());
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
                ScheduleSaveBooks(inputItem.VerseLocation.BookNum
                    .ToSingletonEnumerable());
            }
            return result;
        }

        /// <summary>
        /// Gets verse results for given location, if present.
        /// </summary>
        /// <param name="inputTypes">Check types to include (optional, may be null; null = all).</param>
        /// <param name="inputContexts">Part contexts to include  (optional, may be null; null = all).</param>
        /// <param name="isChangesOnly">True to only include result items (a) not ignored and (b) including suggested changes.</param>
        /// <param name="inputLocation">Verse to retrieve for (required).</param>
        /// <param name="outputItems">Output items to populate (provided).</param>
        /// <returns>True if any items found, false otherwise.</returns>
        public bool TryGetVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            bool isChangesOnly,
            VerseLocation inputLocation, out IList<ResultItem> outputItems)
        {
            outputItems = null;
            lock (_resultLock)
            {
                if (_resultItems.TryGetValue(inputLocation, out var foundItems))
                {
                    outputItems = FilterResultItems(inputTypes, inputContexts, isChangesOnly, foundItems)
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
        /// <param name="isChangesOnly">True to only include result items (a) not ignored and (b) including suggested changes.</param>
        /// <returns>Found verse results, if any.</returns>
        public IList<ResultItem> GetAllVerseResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            bool isChangesOnly)
        {
            lock (_resultLock)
            {
                return _resultItems
                    .Values
                    .SelectMany(listItem =>
                        FilterResultItems(inputTypes, inputContexts, isChangesOnly, listItem))
                    .ToImmutableList();
            }
        }

        /// <summary>
        /// Schedules given book numbers for persistence in the near future,
        /// cancelling any previously-pending save.
        /// </summary>
        /// <param name="bookNums">Book numbers to save (1-based).</param>
        public void ScheduleSaveBooks(IEnumerable<int> bookNums)
        {
            lock (_resultLock)
            {
                foreach (var bookNum in bookNums)
                {
                    _bookNumsToSave.Add(bookNum);
                }

                CancelSaveBooks(true);
                var currToken = _saveTokenSource.Token;

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
        /// Try to retrieve in-memory results organized by book number.
        /// </summary>
        /// <param name="inputTypes">Check types to include (optional, may be null; null = all).</param>
        /// <param name="inputContexts">Part contexts to include  (optional, may be null; null = all).</param>
        /// <param name="isChangesOnly">True to only include result items (a) not ignored and (b) including suggested changes.</param>
        /// <param name="inputBooks">Input book numbers (required).</param>
        /// <param name="outputResults">Output result dictionary (required).</param>
        /// <returns>True if any results retrieved, false otherwise.</returns>
        public bool TryGetBookResults(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            bool isChangesOnly,
            IEnumerable<int> inputBooks,
            out IDictionary<int, IEnumerable<ResultItem>> outputResults)
        {
            var inputBooksSet = inputBooks.ToImmutableHashSet();
            if (inputBooksSet.Count < 1)
            {
                outputResults = ImmutableDictionary.Create<int, IEnumerable<ResultItem>>();
                return false;
            }

            lock (_resultLock)
            {
                outputResults = _resultItems
                    .Where(pairItem =>
                        inputBooksSet.Contains(pairItem.Key.BookNum))
                    .GroupBy(pairItem => pairItem.Key.BookNum)
                    .ToImmutableDictionary(groupItem =>
                        groupItem.Key, groupItem =>
                        groupItem.SelectMany(pairItem =>
                            FilterResultItems(inputTypes, inputContexts, isChangesOnly, pairItem.Value)));
            }

            return outputResults.Count > 0;
        }

        /// <summary>
        /// Filters a result items using typical criteria.
        /// </summary>
        /// <param name="inputTypes">Check types to include (optional, may be null; null = all).</param>
        /// <param name="inputContexts">Part contexts to include  (optional, may be null; null = all).</param>
        /// <param name="isChangesOnly">True to only include result items (a) not ignored and (b) including suggested changes.</param>
        /// <param name="inputItems">Input items to filter (required).</param>
        /// <returns>Input items if no meaningful criteria, filtered items otherwise.</returns>
        private static IEnumerable<ResultItem> FilterResultItems(
            IEnumerable<CheckType> inputTypes,
            IEnumerable<PartContext> inputContexts,
            bool isChangesOnly,
            IEnumerable<ResultItem> inputItems)
        {
            var isAnyCriteria = inputTypes != null
                                || inputContexts != null
                                || isChangesOnly;
            return isAnyCriteria
                ? inputItems
                    .Where(foundItem => inputTypes == null
                            || inputTypes.Contains(foundItem.CheckType))
                    .Where(foundItem => inputContexts == null
                            || inputContexts.Contains(foundItem.PartLocation.PartContext))
                    .Where(foundItem =>
                        foundItem.ResultState != ResultState.Ignored
                        && foundItem.SuggestionText != null)
                : inputItems;
        }

        /// <summary>
        /// Save books for a provided set of book numbers.
        /// </summary>
        /// <param name="inputBooks">Book numbers (1-based).</param>
        public void SaveBooks(IEnumerable<int> inputBooks)
        {
            lock (_resultLock)
            {
                if (!TryGetBookResults(null, null, false,
                    inputBooks, out var booksToSave))
                {
                    return;
                }

                foreach (var pairItem in booksToSave)
                {
                    if (BookUtil.BookIdsByNum.TryGetValue(pairItem.Key, out var bookId))
                    {
                        HostUtil.Instance.PutResultItems(_projectName, bookId.BookCode, pairItem.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Schedules given book numbers for loading in the near future,
        /// cancelling any previously-pending load.
        ///
        /// Note: There is no versioning in this persistence scheme, so while loading
        /// will reconcile with any result items already in memory, newly-loaded items
        /// will be treated as updates. Therefore, loading should be done only at startup
        /// or when coherency with persisted items may be otherwise guaranteed.
        /// </summary>
        /// <param name="bookNums">Book numbers to load (1-based).</param>
        public void ScheduleLoadBooks(IEnumerable<int> bookNums)
        {
            lock (_resultLock)
            {
                foreach (var bookNum in bookNums)
                {
                    _bookNumsToLoad.Add(bookNum);
                }

                CancelLoadBooks(true);
                var currToken = _loadTokenSource.Token;

                // call does _not_ block
                Task.Delay(TimeSpan.FromSeconds(MainConsts.RESULT_ITEM_LOAD_DELAY_IN_SEC), currToken)
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
                                if (_bookNumsToLoad.Count <= 0)
                                {
                                    return;
                                }
                                LoadBooks(_bookNumsToLoad);
                                _bookNumsToLoad.Clear();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            // Ignore (can occur w/cancel).
                        }
                        catch (Exception ex)
                        {
                            HostUtil.Instance.ReportError(
                                "Can't load result items", true, ex);
                        }
                    }, currToken);
            }
        }

        /// <summary>
        /// Load books from a provided set of book numbers.
        /// </summary>
        /// <param name="inputBooks">Book numbers (1-based).</param>
        public void LoadBooks(IEnumerable<int> inputBooks)
        {
            ISet<int> inputBooksSet = inputBooks.ToImmutableHashSet();
            if (inputBooksSet.Count < 1)
            {
                return;
            }
            lock (_resultLock)
            {
                foreach (var bookNum in inputBooksSet)
                {
                    if (!BookUtil.BookIdsByNum.TryGetValue(bookNum, out var bookId))
                    {
                        continue;
                    }

                    foreach (var foundItem in
                        HostUtil.Instance.GetResultItems(_projectName, bookId.BookCode))
                    {
                        SetVerseResult(foundItem);
                    }
                }
            }
        }

        /// <summary>
        /// Cancel any pending book save,
        /// recycling cancellation token.
        /// </summary>
        /// <param name="isCreateNewToken">True to create a new cancellation token afterwards, false otherwise.</param>
        private void CancelSaveBooks(bool isCreateNewToken)
        {
            lock (_resultLock)
            {
                if (_saveTokenSource != null)
                {
                    try
                    {
                        _saveTokenSource.Cancel();
                    }
                    finally
                    {
                        _saveTokenSource.Dispose();
                    }
                }
                _saveTokenSource = isCreateNewToken
                    ? new CancellationTokenSource()
                    : null;
            }
        }

        /// <summary>
        /// Cancel any pending book load,
        /// recycling cancellation token.
        /// </summary>
        /// <param name="isCreateNewToken">True to create a new cancellation token afterwards, false otherwise.</param>
        private void CancelLoadBooks(bool isCreateNewToken)
        {
            lock (_resultLock)
            {
                if (_loadTokenSource != null)
                {
                    try
                    {
                        _loadTokenSource.Cancel();
                    }
                    finally
                    {
                        _loadTokenSource.Dispose();
                    }
                }
                _loadTokenSource = isCreateNewToken
                    ? new CancellationTokenSource()
                    : null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (_resultLock)
            {
                CancelSaveBooks(false);
                CancelLoadBooks(false);
            }
        }
    }
}
