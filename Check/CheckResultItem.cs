using System;
using Newtonsoft.Json;
using TvpMain.Result;
using TvpMain.Util;

namespace TvpMain.Check
{
    /// <summary>
    /// Model class for check result items.
    ///
    /// Immutable except for ResultState, which is meant to be
    /// user-editable. Accordingly, all fields are incorporated
    /// into equality checks _except_ for ResultState.
    ///
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]

    public class CheckResultItem
    {
        /// <summary>
        /// Message describing the result
        /// </summary>
        [JsonProperty]
        public string Description { get; private set; }

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
        public string FixText { get; set; }

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
        /// TODO: Update this to account for the new possiblities
        /// </summary>
        [JsonProperty]
        public ResultState ResultState { get; set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="description">Description text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        public CheckResultItem(string description,
            string matchText, int matchStart, CheckType checkType,
            int resultTypeCode)
        : this(description,
            matchText, matchStart,
            "", checkType,
            resultTypeCode, ResultState.Found)
        { }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="description">Description text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="fixText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        public CheckResultItem(string description,
            string matchText, int matchStart,
            string fixText, CheckType checkType,
            int resultTypeCode)
        : this(description,
            matchText, matchStart,
            fixText, checkType,
            resultTypeCode, ResultState.Found)
        { }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="description">Description text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="fixText">Suggested replacement text (optional, may be null).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        /// <param name="resultState">User-controlled result state.</param>
        public CheckResultItem(string description,
            string matchText, int matchStart,
            string fixText, CheckType checkType,
            int resultTypeCode, ResultState resultState)
        {
            this.Description = description ?? throw new ArgumentNullException(nameof(description));
            this.MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
            this.MatchStart = matchStart;
            this.FixText = fixText;
            this.CheckType = checkType;
            this.ResultTypeCode = resultTypeCode;
            this.ResultState = resultState;
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private CheckResultItem()
        {
        }

        protected bool Equals(CheckResultItem other)
        {
            return Description == other.Description
                   && MatchText == other.MatchText
                   && MatchStart == other.MatchStart
                   && FixText == other.FixText
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
                var hashCode = ( MainConsts.HASH_PRIME) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (MatchText != null ? MatchText.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ MatchStart;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (FixText != null ? FixText.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (int)CheckType;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ ResultTypeCode;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (int)ResultState;
                return hashCode;
            }
        }

        public static bool operator ==(CheckResultItem left, CheckResultItem right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CheckResultItem left, CheckResultItem right)
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
