using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Specific location for a check.
    /// </summary>
    public class TextLocation
    {
        /// <summary>
        /// Book number of result (1-based).
        /// </summary>
        public int BookNum { get; }

        /// <summary>
        /// Chapter number of result (1-based).
        /// </summary>
        public int ChapterNum { get; }

        /// <summary>
        /// Verse number of result (generally 1-based, 0=any intro).
        /// </summary>
        public int VerseNum { get; }

        /// <summary>
        /// Text context.
        /// </summary>
        public TextContext TextContext { get; }

        /// <summary>
        /// Coordinate in Paratext-specific format.
        /// </summary>
        [Ignore]
        public int Coordinate => TextUtil.BcvToRef(BookNum, ChapterNum, VerseNum);

        /// <summary>
        /// B/C/V summary text.
        /// </summary>
        [Ignore]
        public string CoordinateText =>
            TextUtil.BookIdsByNum.TryGetValue(BookNum, out var bookId)
                ? $"{bookId.BookCode + " " + ChapterNum + ":" + VerseNum}"
                : $"{"#" + BookNum + " " + ChapterNum + ":" + VerseNum}";

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookNum">Book number.</param>
        /// <param name="chapterNum">Chapter number.</param>
        /// <param name="verseNum">Verse number.</param>
        /// <param name="textContext">Text context.</param>
        public TextLocation(int bookNum, int chapterNum, int verseNum, TextContext textContext)
        {
            BookNum = bookNum;
            ChapterNum = chapterNum;
            VerseNum = verseNum;
            TextContext = textContext;
        }
    }
}
