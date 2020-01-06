using AddInSideViews;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TvpMain.Data;

namespace TvpMain.Data
{
    /// <summary>
    /// Check result aggregator.
    /// </summary>
    public class CheckResult
    {
        /// <summary>
        /// Collection of result items.
        /// </summary>
        private readonly ConcurrentQueue<ResultItem> _resultItems;

        /// <summary>
        /// Collection of result items.
        /// </summary>
        public ConcurrentQueue<ResultItem> ResultItems => _resultItems;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public CheckResult()
        {
            _resultItems = new ConcurrentQueue<ResultItem>();
        }

        /// <summary>
        /// Summary text from the collection of result items.
        /// </summary>
        public string SummaryText => GetSummaryText(new List<ResultItem>(_resultItems));

        /// <summary>
        /// Gets summary text from a collection of result items.
        /// </summary>
        /// <param name="resultItems">Result items (required).</param>
        /// <returns>Summary text.</returns>
        public static string GetSummaryText(IList<ResultItem> resultItems)
        {
            var stringBuilder = new StringBuilder();
            if (resultItems.Count < 1)
            {
                stringBuilder.Append("No violations.");
            }
            else
            {
                stringBuilder.Append(resultItems.Count > 1
                    ? $"{resultItems.Count:N0} violations"
                    : $"{resultItems.Count:N0} violation");

                ISet<int> bookSet = new HashSet<int>();
                ISet<string> matchSet = new HashSet<string>();
                foreach (var resultItem in resultItems)
                {
                    bookSet.Add(resultItem.BookNum);
                    matchSet.Add(resultItem.MatchText);
                }

                stringBuilder.Append(bookSet.Count > 1
                    ? $" in {bookSet.Count:N0} books"
                    : $" in {bookSet.Count:N0} book");
                stringBuilder.Append(matchSet.Count > 1
                    ? $" with {matchSet.Count:N0} unique matches."
                    : $" with {matchSet.Count:N0} unique match.");
            }
            return stringBuilder.ToString();
        }
    }
}