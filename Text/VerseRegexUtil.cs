using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Verse Regex-related utilities.
    /// </summary>
    public static class VerseRegexUtil
    {
        /// <summary>
        /// Standard book name regex text (i.e., at least one letter surrounded by letters or digits).
        /// </summary>
        public const string STANDARD_BOOK_NAME_REGEX_TEXT = @"\w*\p{L}\w*";

        /// <summary>
        /// Standard punctuation regex text.
        /// </summary>
        public const string STANDARD_PUNCTUATION_REGEX_TEXT = @"[ \t\p{P}]";

        /// <summary>
        /// Possible reference-wrapping tags, anywhere in the text.
        /// </summary>
        public static readonly ISet<string> TargetReferencePairedTags =
            new HashSet<string>()
            {
                "xt", "+xt", "ior"
            }.ToImmutableHashSet();

        /// <summary>
        /// Regexes to catch _possible_ target references in arbitrary text
        /// with typical English punctuation for error checking.
        /// </summary>
        public static readonly IList<Regex> StandardTargetReferenceRegexes =
            new List<Regex>()
            {
                CreateTargetReferenceGroupRegex(
                    TargetReferencePairedTags.Select(Regex.Escape),
                    STANDARD_BOOK_NAME_REGEX_TEXT.ToSingletonEnumerable(),
                    STANDARD_PUNCTUATION_REGEX_TEXT.ToSingletonEnumerable())
            }.ToImmutableList();

        /// <summary>
        /// Creates regex to catch _possible_ target references in arbitrary text
        /// with groups of specified book identifiers and punctuation for error checking.
        ///
        /// Note: Escape and trim any potential, un-intentional argument meta-characters
        /// (e.g., from book names or settings config files).
        /// </summary>
        /// <param name="tagParts">Enclosing tag regex sub-elements (e.g., "xt"; required).</param>
        /// <param name="bookParts">Book regex sub-elements (e.g., a non-capturing group w/ORed elements; required).</param>
        /// <param name="punctuationParts">Punctuation regex sub-elements (required).</param>
        /// <returns>Compiled, case-insensitive regex.</returns>
        public static Regex CreateTargetReferenceGroupRegex(
            IEnumerable<string> tagParts,
            IEnumerable<string> bookParts,
            IEnumerable<string> punctuationParts)
        {
            return CreateTargetReferenceRegex(
                $"(?:{string.Join("|", tagParts)})",
                $"(?:{string.Join("|", bookParts)})",
                $"(?:{string.Join("|", punctuationParts)})");
        }

        /// <summary>
        /// Creates regex to catch _possible_ target references in arbitrary text
        /// with specified book identifiers and punctuation for error checking.
        /// 
        /// Note: Escape and trim any potential, un-intentional argument meta-characters
        /// (e.g., from book names or settings config files).
        /// </summary>
        /// <param name="tagPart">Enclosing tag regex sub-element (e.g., "xt"; required).</param>
        /// <param name="bookPart">Book regex sub-element (e.g., a non-capturing group w/ORed elements; required).</param>
        /// <param name="punctuationPart">Punctuation regex sub-element (required).</param>
        /// <returns>Compiled, case-insensitive regex.</returns>
        public static Regex CreateTargetReferenceRegex(
            string tagPart,
            string bookPart,
            string punctuationPart)
        {
            return new Regex($@"(?:\\\S*{tagPart}\s*)?(?:(?:\\)?(?:{bookPart})?\s*[0-9]+(?:\s*{punctuationPart}\s*[0-9]+)+)(?:\s*{punctuationPart}\s*(?:\\)?(?:{bookPart})?\s*[0-9]+(?:\s*{punctuationPart}\s*[0-9]+)+)*(?:\s*\\\S*{tagPart}\*?)?",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
