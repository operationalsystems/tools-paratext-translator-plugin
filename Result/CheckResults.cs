/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace TvpMain.Result
{
    /// <summary>
    /// Check result aggregator.
    /// </summary>
    public class CheckResults
    {
        /// <summary>
        /// This is a container class for holding the results from the searches.
        /// This class also allows for reproting about the results list itself.
        /// </summary>
        public ConcurrentQueue<ResultItem> ResultItems { get; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public CheckResults()
        {
            ResultItems = new ConcurrentQueue<ResultItem>();
        }

        /// <summary>
        /// Summary text from the collection of result items.
        /// </summary>
        public string SummaryText => GetSummaryText(new List<ResultItem>(ResultItems));

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
                    bookSet.Add(resultItem.VersePart.ProjectVerse.VerseLocation.BookNum);
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