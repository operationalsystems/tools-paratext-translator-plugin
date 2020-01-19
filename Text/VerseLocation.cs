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
    /// Specific verse location.
    /// </summary>
    public class VerseLocation
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
        /// Coordinate in Paratext-specific format.
        /// </summary>
        [Ignore]
        public int VerseCoordinate => BookUtil.BcvToRef(BookNum, ChapterNum, VerseNum);

        /// <summary>
        /// B/C/V summary text.
        /// </summary>
        [Ignore]
        public string VerseCoordinateText =>
            BookUtil.BookIdsByNum.TryGetValue(BookNum, out var bookId)
                ? $"{bookId.BookCode + " " + ChapterNum + ":" + VerseNum}"
                : $"{"#" + BookNum + " " + ChapterNum + ":" + VerseNum}";

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookNum">Book number.</param>
        /// <param name="chapterNum">Chapter number.</param>
        /// <param name="verseNum">Verse number.</param>
        public VerseLocation(int bookNum, int chapterNum, int verseNum)
        {
            BookNum = bookNum;
            ChapterNum = chapterNum;
            VerseNum = verseNum;
        }
        /// <summary>
        /// Typed equality method.
        /// </summary>
        /// <param name="other">Other verse location (required).</param>
        /// <returns>True if equal, false otherwise</returns>
        protected bool Equals(VerseLocation other)
        {
            return BookNum == other.BookNum
                   && ChapterNum == other.ChapterNum
                   && VerseNum == other.VerseNum;
        }

        /// <summary>
        /// Standard equality method.
        /// </summary>
        /// <param name="obj">Other object (optional, may be null).</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VerseLocation)obj);
        }

        /// <summary>
        /// Standard hash code method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BookNum;
                hashCode = (hashCode * 397) ^ ChapterNum;
                hashCode = (hashCode * 397) ^ VerseNum;
                return hashCode;
            }
        }

        /// <summary>
        /// Typed equality operator.
        /// </summary>
        /// <param name="left">Left verse location.</param>
        /// <param name="right">Right verse location.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(VerseLocation left, VerseLocation right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Standard equality operator.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator !=(VerseLocation left, VerseLocation right)
        {
            return !Equals(left, right);
        }
    }
}
