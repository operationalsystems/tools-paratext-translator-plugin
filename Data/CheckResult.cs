using AddInSideViews;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using TvpMain.Data;

/*
 * A class to handle results from validation checks.
 */
namespace TvpMain.Data
{
    public class CheckResult
    {
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private readonly ConcurrentQueue<ResultItem> _resultItems;
        private IScrExtractor _scrExtractor;

        public IHost Host => _host;
        public string ActiveProjectName => _activeProjectName;
        public ConcurrentQueue<ResultItem> ResultItems => _resultItems;


        public CheckResult(IHost host, string activeProjectName)
        {
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
            _resultItems = new ConcurrentQueue<ResultItem>();
        }


        public string SummaryText
        {
            get
            {
                return GetSummaryText(new List<ResultItem>(_resultItems));
            }
        }

        public IScrExtractor Extractor
        {
            get
            {
                lock (this)
                {
                    if (_scrExtractor == null)
                    {
                        _scrExtractor = _host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM);
                    }
                    return _scrExtractor;
                }
            }
        }

        public static string GetSummaryText(IList<ResultItem> resultItems)
        {
            StringBuilder stringBuilder = new StringBuilder();
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
                foreach (ResultItem resultItem in resultItems)
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