using System.Collections.Generic;
using System.Text.RegularExpressions;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    /// <summary>
    /// Regex-based punctuation check.
    /// </summary>
    public class MissingSentencePunctuationCheck : ITextCheck
    {
        /// <summary>
        /// Regex to check for improper capitalization following non-final punctuation (E.g. <text>, And <text>).
        /// </summary>
        private static readonly Regex CheckRegex =
            new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(VersePart partData, ICollection<ResultItem> checkResults)
        {
            foreach (Match matchItem in CheckRegex.Matches(partData.PartText))
            {
                checkResults.Add
                (new ResultItem(partData,
                    $"Punctuation check failure at position {matchItem.Index}.",
                    partData.PartText, matchItem.Value, null,
                    CheckType.MissingSentencePunctuation));
            }
        }
    }
}
