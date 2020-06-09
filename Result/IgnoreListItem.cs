using Newtonsoft.Json;
using System;
using System.Linq;

namespace TvpMain.Result
{
    /// <summary>
    /// Model class for ignore list entries.
    /// 
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class IgnoreListItem
    {
        /// <summary>
        /// Case-sensitive version of text.
        /// </summary>
        [JsonProperty]
        public string CaseSensitiveText { get; private set; }

        /// <summary>
        /// True if item is meant to be case-insensitive (i.e., ignores case), false otherwise.
        /// </summary>
        [JsonProperty]
        public bool IsIgnoreCase { get; private set; }

        /// <summary>
        /// True if case-sensitive text contains any whitespace.
        /// </summary>
        public bool IsPhrase()
        {
            return CaseSensitiveText.Any(char.IsWhiteSpace);
        }

        /// <summary>
        /// Provide a case-insensitive version of the text (lower case).
        /// </summary>
        public string CaseInsensitiveText()
        {
            return CaseSensitiveText.ToLower();
        }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="itemText">Entry text (required).</param>
        /// <param name="isIgnoreCase">True if item is to be case-insensitive, false otherwise.</param>
        public IgnoreListItem(string itemText, bool isIgnoreCase)
        {
            CaseSensitiveText = itemText ?? throw new ArgumentNullException(nameof(itemText));
            IsIgnoreCase = isIgnoreCase;
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private IgnoreListItem()
        {
        }
    }
}
