using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AddInSideViews;
using TvpMain.Data;
using TvpMain.Filter;

namespace TvpMain.Check
{
    /// <summary>
    /// Regex-based punctuation check.
    /// </summary>
    public class MissingSentencePunctuationCheck : AbstractTextCheck
    {
        /// <summary>
        /// Regex to check for improper capitalization following non-final punctuation (E.g. <text>, And <text>).
        /// </summary>
        private static readonly Regex CheckRegex;

        /// <summary>
        /// Static initializer.
        /// </summary>
        static MissingSentencePunctuationCheck()
        {
            CheckRegex = new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
                RegexOptions.Multiline);
        }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public MissingSentencePunctuationCheck(IHost host, string activeProjectName)
            : base(host, activeProjectName)
        {
        }

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="bookNum">Book number.</param>
        /// <param name="chapterNum">Chapter number.</param>
        /// <param name="verseNum">Verse number.</param>
        /// <param name="verseText">Verse text.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        protected override void CheckVerse(int bookNum, int chapterNum, int verseNum, string verseText, IList<ResultItem> checkResults)
        {
            foreach (Match matchItem in CheckRegex.Matches(verseText))
            {
                checkResults.Add(new ResultItem(bookNum, chapterNum, verseNum,
                    $"Punctuation check failure at position {matchItem.Index}.",
                    verseText, matchItem.Value));
            }
        }
    }
}
