using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace TvpMain.Result
{
    /// <summary>
    /// Model class for ignore list entries.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class IgnoreListItem
    {
        /// <summary>
        /// Case-sensitive version of text.
        /// </summary>
        [JsonProperty]
        [Index(0)]
        public string CaseSensitiveText { get; }

        /// <summary>
        /// True if item is meant to be case-insensitive (i.e., ignores case), false otherwise.
        /// </summary>
        [JsonProperty]
        [Index(1)]
        [Default(false)]
        public bool IsIgnoreCase { get; }

        /// <summary>
        /// True if case-sensitive text contains any whitespace.
        /// </summary>
        [Ignore]
        public bool IsPhrase => CaseSensitiveText.Any(char.IsWhiteSpace);

        /// <summary>
        /// Provide a case-insensitive version of the text (lower case).
        /// </summary>
        [Ignore]
        public string CaseInsensitiveText => CaseSensitiveText.ToLower();

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
