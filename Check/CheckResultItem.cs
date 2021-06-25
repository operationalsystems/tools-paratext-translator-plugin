using Newtonsoft.Json;
using System;
using TvpMain.Text;
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
        /// TODO: Update this to account for the new possibilities
        /// </summary>
        [JsonProperty]
        public CheckResultState ResultState { get; set; }

        /// <summary>
        /// The project name for where the check result was found.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The book number, 1-based index where the check result refers.
        /// For checks that have project scope, this value will be -1.
        /// </summary>
        public int Book { get; set; }

        /// <summary>
        /// The chapter number, 1-based index where the check result refers.
        /// For checks that have project or book scope, this value will be -1.
        /// </summary>
        public int Chapter { get; set; }

        /// <summary>
        /// The verse number, 1-based index where the check result refers.
        /// For checks that do not have verse scope, this value will be -1.
        /// </summary>
        public int Verse { get; set; }

        /// <summary>
        /// This is the reference text that was searched
        /// </summary>
        public string Reference { get; set; }

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
            resultTypeCode)
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
            string matchText,
            int matchStart,
            string fixText,
            CheckType checkType,
            int resultTypeCode,
            CheckResultState resultState = CheckResultState.Found)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
            MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
            MatchStart = matchStart;
            FixText = fixText;
            CheckType = checkType;
            ResultTypeCode = resultTypeCode;
            ResultState = resultState;
        }

        /// <summary>
        /// Constructor that allows for passing in optional information about where the 
        /// result was found by project name, book, chapter, verse
        /// </summary>
        /// <param name="description">Description text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        /// <param name="matchStart">Start of match text, relative to verse start (0-based).</param>
        /// <param name="checkType">Check type (i.e., source).</param>
        /// <param name="resultTypeCode">Result type code (i.e., a discrete error sub-type).</param>
        /// <param name="projectName">The project name for where the check result was found.</param>
        /// <param name="book">The project name for where the check result was found.</param>
        /// <param name="chapter">The project name for where the check result was found.</param>
        /// <param name="verse">The project name for where the check result was found.</param>
        public CheckResultItem(string description,
            string matchText, int matchStart, CheckType checkType,
            int resultTypeCode,
            string projectName = "",
            int book = -1,
            int chapter = -1,
            int verse = -1)
        : this(description,
            matchText, matchStart,
            "", checkType,
            resultTypeCode)
        {
            ProjectName = projectName;
            Book = book;
            Chapter = chapter;
            Verse = verse;
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
            if (obj.GetType() != GetType()) return false;
            return Equals((CheckResultItem)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (MainConsts.HASH_PRIME) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (MatchText != null ? MatchText.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ MatchStart;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (FixText != null ? FixText.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (int)CheckType;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ ResultTypeCode;
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

        /// <summary>
        /// Compare two result items by location alone, allowing for
        /// possibly incomplete book, chapter, or verse elements.
        /// </summary>
        /// <param name="leftItem">Left result item to compare.</param>
        /// <param name="rightItem">Right result item to compare.</param>
        /// <returns></returns>
        public static int CompareByLocation(CheckResultItem leftItem, CheckResultItem rightItem)
        {
            return BookUtil.BcvToRef(
                    leftItem.Book < 1 ? 1 : leftItem.Book,
                    leftItem.Chapter < 1 ? 1 : leftItem.Chapter,
                    leftItem.Verse < 1 ? 1 : leftItem.Verse)
                .CompareTo(BookUtil.BcvToRef(
                    rightItem.Book < 1 ? 1 : rightItem.Book,
                    rightItem.Chapter < 1 ? 1 : rightItem.Chapter,
                    rightItem.Verse < 1 ? 1 : rightItem.Verse));
        }
    }
}
