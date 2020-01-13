using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AddInSideViews;
using TvpMain.Result;
using TvpMain.Filter;

namespace TvpMain.Text
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
            RegexOptions.Multiline);

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="textLocation">Text location (required).</param>
        /// <param name="inputText">Input text.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckVerse(TextLocation textLocation, string inputText, IList<ResultItem> checkResults)
        {
            foreach (Match matchItem in CheckRegex.Matches(inputText))
            {
                checkResults.Add
                (new ResultItem(textLocation,
                    $"Punctuation check failure at position {matchItem.Index}.",
                    inputText, matchItem.Value, null,
                    CheckType.MissingSentencePunctuation));
            }
        }
    }
}
