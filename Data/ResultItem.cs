using CsvHelper.Configuration.Attributes;
using System;
using TvpMain.Util;

namespace TvpMain.Data
{
    /// <summary>
    /// Model class for check result items.
    /// </summary>
    public class ResultItem
    {
        /// <summary>
        /// Book number of result.
        /// </summary>
        public int BookNum { get; }

        /// <summary>
        /// Chapter number of result.
        /// </summary>
        public int ChapterNum { get; }

        /// <summary>
        /// Verse number of result.
        /// </summary>
        public int VerseNum { get; }

        /// <summary>
        /// B/C/V summary text.
        /// </summary>
        public string BcvText => $"{ MainConsts.ShortBookNames[BookNum - 1] + " " + ChapterNum + ":" + VerseNum}";

        /// <summary>
        /// Exact text triggering the result.
        /// </summary>
        public string MatchText { get; }

        /// <summary>
        /// Complete verse containing the result.
        /// </summary>
        public string VerseText { get; }

        /// <summary>
        /// Error message describing the result
        /// </summary>
        public string ErrorText { get; }

        /// <summary>
        /// Coordinate in Paratext-specific format.
        /// </summary>
        [Ignore]
        public int Coordinate => HostUtil.BcvToRef(BookNum, ChapterNum, VerseNum);

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookNum">Book number.</param>
        /// <param name="chapterNum">Chapter number.</param>
        /// <param name="verseNum">Verse number.</param>
        /// <param name="errorText">Error text (required).</param>
        /// <param name="verseText">Verse text (required).</param>
        /// <param name="matchText">Match text (required).</param>
        public ResultItem(int bookNum, int chapterNum, int verseNum, string errorText, string verseText, string matchText)
        {
            this.BookNum = bookNum;
            this.ChapterNum = chapterNum;
            this.VerseNum = verseNum;
            this.ErrorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
            this.VerseText = verseText ?? throw new ArgumentNullException(nameof(verseText));
            this.MatchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
        }
    }
}
