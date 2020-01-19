using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Result
{
    /// <summary>
    /// Model class for check result items.
    /// </summary>
    public class ResultItem
    {
        /// <summary>
        /// Verse part data, including original verse, location, etc.
        /// </summary>
        public PartData PartData { get; }

        /// <summary>
        /// Error message describing the result
        /// </summary>
        public string ErrorText { get; }

        /// <summary>
        /// Complete input containing the whatever was found.
        /// </summary>
        public string CheckText { get; }

        /// <summary>
        /// Exact text triggering the result.
        /// </summary>
        public string MatchText { get; }

        /// <summary>
        /// Suggestion text (may be null).
        /// </summary>
        public string SuggestionText { get; }

        /// <summary>
        /// Check type.
        /// </summary>
        public CheckType CheckType { get; }

        /// <summary>
        /// Internal check state.
        /// </summary>
        private ResultState _checkState;

        /// <summary>
        /// External check state.
        /// </summary>
        public ResultState CheckState
        {
            get => _checkState;
            set
            {
                _checkState = value;
                _checkHistory.Add(new KeyValuePair<DateTime, ResultState>(DateTime.UtcNow, _checkState));
            }
        }

        /// <summary>
        /// Internal check history.
        /// </summary>
        private readonly IList<KeyValuePair<DateTime, ResultState>> _checkHistory =
            new List<KeyValuePair<DateTime, ResultState>>();

        /// <summary>
        /// External check history.
        /// </summary>
        public IEnumerable<KeyValuePair<DateTime, ResultState>> CheckHistory => _checkHistory;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc. (required).</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="checkText">Input text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="suggestionText">Suggestion text (optional, may be null).</param>
        /// <param name="checkType">Check type.</param>
        public ResultItem(PartData partData, string errorText,
            string checkText, string matchText,
            string suggestionText, CheckType checkType)
        {
            this.PartData = partData ?? throw new ArgumentNullException(nameof(partData));
            this.ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
            this.CheckText = checkText ?? throw new ArgumentNullException(nameof(checkText));
            this.MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
            this.SuggestionText = suggestionText;

            this.CheckType = checkType;
            this.CheckState = ResultState.Found;
        }
    }
}
