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
    public class RegexPunctuationCheck1 : AbstractTextCheck
    {
        private static readonly Regex _checkRegex;

        /*
         * Regex to check for improper capitlization following non-final punctuation (E.g. <text>, And <text>).
         */
        static RegexPunctuationCheck1()
        {
            _checkRegex = new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
                RegexOptions.Multiline);
        }

        public RegexPunctuationCheck1(IHost host, string activeProjectName)
            : base(host, activeProjectName)
        {
        }

        protected override void CheckVerse(int bookNum, int chapterNum, int verseNum, string verseText, IList<ResultItem> checkResults)
        {
            foreach (Match matchItem in _checkRegex.Matches(verseText))
            {
                checkResults.Add(new ResultItem(bookNum, chapterNum, verseNum,
                    $"Punctuation check failure at position {matchItem.Index}.",
                    verseText, matchItem.Value));
            }
        }
    }
}
