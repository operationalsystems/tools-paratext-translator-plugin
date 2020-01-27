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
        public VersePart PartData { get; }

        /// <summary>
        /// Error message describing the result
        /// </summary>
        public string ErrorText { get; }

        /// <summary>
        /// Exact text triggering the result.
        /// </summary>
        public string MatchText { get; }

        /// <summary>
        /// Suggested replacement text (may be null).
        /// </summary>
        public string SuggestionText { get; }

        /// <summary>
        /// Check type (i.e., source).
        /// </summary>
        public CheckType CheckType { get; }

        /// <summary>
        /// Result type (i.e., severity).
        /// </summary>
        public ResultType ResultType { get; }

        /// <summary>
        /// User-controlled result state.
        /// </summary>
        public ResultState ResultState { get; set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc. (required).</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="suggestionText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultType">Result type (i.e., severity).</param>
        public ResultItem(VersePart partData, string errorText,
            string matchText, string suggestionText,
            CheckType checkType, ResultType resultType)
        : this(partData, errorText,
            matchText, suggestionText,
            checkType, resultType,
            ResultState.Found)
        { }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc. (required).</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="suggestionText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultType">Result type (i.e., severity).</param>
        /// <param name="resultState">User-controlled result state.</param>
        public ResultItem(VersePart partData, string errorText,
        string matchText, string suggestionText,
        CheckType checkType, ResultType resultType,
        ResultState resultState)
        {
            this.PartData = partData ?? throw new ArgumentNullException(nameof(partData));
            this.ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
            this.MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
            this.SuggestionText = suggestionText;
            this.CheckType = checkType;
            this.ResultType = resultType;
            this.ResultState = resultState;
        }
    }
}
