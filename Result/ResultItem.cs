using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using TvpMain.Check;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Result
{
    /// <summary>
    /// Model class for check result items.
    ///
    /// Immutable except for ResultState, which is meant to be
    /// user-editable. Accordingly, all fields are incorporated
    /// into equality checks _except_ for ResultState.
    ///
    /// Note "private set" fields support JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ResultItem
    {
        /// <summary>
        /// Verse part data, including original verse, location, etc.
        /// </summary>
        [JsonProperty]
        public VersePart VersePart { get; private set; }

        /// <summary>
        /// Gets the verse location (book, chapter, verse).
        /// </summary>
        public VerseLocation VerseLocation =>
            VersePart.ParatextVerse.VerseLocation;

        /// <summary>
        /// Gets the Part location (start, length w/in verse).
        /// </summary>
        public PartLocation PartLocation =>
            VersePart.PartLocation;

        /// <summary>
        /// Error message describing the result
        /// </summary>
        [JsonProperty]
        public string ErrorText { get; private set; }

        /// <summary>
        /// Exact text triggering the result.
        /// </summary>
        [JsonProperty]
        public string MatchText { get; private set; }

        /// <summary>
        /// Start of match text, relative to verse start (0-based).
        /// </summary>
        [JsonProperty]
        public int MatchStart { get; private set; }

        /// <summary>
        /// Length of match text.
        /// </summary>
        public int MatchLength => MatchText.Length;

        /// <summary>
        /// Suggested replacement text (may be null).
        /// </summary>
        [JsonProperty]
        public string SuggestionText { get; private set; }

        /// <summary>
        /// Check type (i.e., source).
        /// </summary>
        [JsonProperty]
        public CheckType CheckType { get; private set; }

        /// <summary>
        /// Result type code (i.e., a discrete error sub-type).
        /// </summary>
        [JsonProperty]
        public int ResultTypeCode { get; private set; }

        /// <summary>
        /// User-controlled result state.
        /// </summary>
        [JsonProperty]
        public ResultState ResultState { get; set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc. (required).</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="suggestionText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        public ResultItem(VersePart partData, string errorText,
            string matchText, int matchStart,
            string suggestionText, CheckType checkType,
            int resultTypeCode)
        : this(partData, errorText,
            matchText, matchStart,
            suggestionText, checkType,
            resultTypeCode, ResultState.Found)
        { }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc. (required).</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="suggestionText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        /// <param name="resultState">User-controlled result state.</param>
        public ResultItem(VersePart partData, string errorText,
            string matchText, int matchStart,
            string suggestionText, CheckType checkType,
            int resultTypeCode, ResultState resultState)
        {
            this.VersePart = partData ?? throw new ArgumentNullException(nameof(partData));
            this.ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
            this.MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
            this.MatchStart = matchStart;
            this.SuggestionText = suggestionText;
            this.CheckType = checkType;
            this.ResultTypeCode = resultTypeCode;
            this.ResultState = resultState;
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private ResultItem()
        {
        }

        protected bool Equals(ResultItem other)
        {
            return Equals(VersePart, other.VersePart)
                   && ErrorText == other.ErrorText
                   && MatchText == other.MatchText
                   && MatchStart == other.MatchStart
                   && SuggestionText == other.SuggestionText
                   && CheckType == other.CheckType
                   && ResultTypeCode == other.ResultTypeCode;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResultItem)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (VersePart != null ? VersePart.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ErrorText != null ? ErrorText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MatchText != null ? MatchText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ MatchStart;
                hashCode = (hashCode * 397) ^ (SuggestionText != null ? SuggestionText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)CheckType;
                hashCode = (hashCode * 397) ^ ResultTypeCode;
                return hashCode;
            }
        }

        public static bool operator ==(ResultItem left, ResultItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ResultItem left, ResultItem right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
