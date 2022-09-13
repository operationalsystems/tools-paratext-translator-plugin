/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Specific verse location.
    ///
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VerseLocation : IComparable<VerseLocation>
    {

        /// <summary>
        /// Book number of result (1-based).
        /// </summary>
        [JsonProperty]
        public int BookNum { get; private set; }

        /// <summary>
        /// Chapter number of result (1-based).
        /// </summary>
        [JsonProperty]
        public int ChapterNum { get; private set; }

        /// <summary>
        /// Verse number of result (generally 1-based, 0=any intro).
        /// </summary>
        [JsonProperty]
        public int VerseNum { get; private set; }

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
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private VerseLocation()
        {
        }

        /// <summary>
        /// Similar to VerseCoordinateText, but handles -1 values at book, chapter, or verse
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // no book number = "project"
            if (BookNum == -1)
            {
                return "Project";
            }

            // try retrieving the book ID, if we've gotten this far
            BookUtil.BookIdsByNum.TryGetValue(BookNum, out var bookId);

            // no chapter = only the book
            if (ChapterNum == -1)
            {
                return bookId != null
                    ? $"{bookId.BookCode}"
                    : $"{"#" + BookNum}";
            }

            // no verse = only the book and chapter
            if (VerseNum == -1)
            {
                return bookId != null
                    ? $"{bookId.BookCode + " " + ChapterNum}"
                    : $"{"#" + BookNum + " " + ChapterNum}";
            }

            // else = book, chapter, and verse
            return bookId != null
                ? $"{bookId.BookCode + " " + ChapterNum + ":" + VerseNum}"
                : $"{"#" + BookNum + " " + ChapterNum + ":" + VerseNum}";
        }

        /// <summary>
        /// Create project-specific reference text.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        /// <returns></returns>
        public string ToProjectString(ProjectManager projectManager)
        {
            // no book number = project name
            if (BookNum == -1)
            {
                return projectManager.ProjectName;
            }

            // first, try getting a project-specific name
            string bookText = null;
            if (projectManager.BookNamesByNum.TryGetValue(BookNum, out var bookName))
            {
                bookText = bookName.GetAvailableName(
                    BookNameType.Abbreviation,
                    BookNameType.ShortName,
                    BookNameType.LongName);
            }

            // second, try the standard book codes
            if (bookText == null)
            {
                if (BookUtil.BookIdsByNum.TryGetValue(BookNum, out var bookId))
                {
                    bookText = bookId.BookCode;
                }
            }

            // no chapter = only the book
            if (ChapterNum == -1)
            {
                return !string.IsNullOrWhiteSpace(bookText)
                    ? $"{bookText}"
                    : $"{"#" + BookNum}";
            }

            // no verse = only the book and chapter
            if (VerseNum == -1)
            {
                return !string.IsNullOrWhiteSpace(bookText)
                    ? $"{bookText + " " + ChapterNum}"
                    : $"{"#" + BookNum + " " + ChapterNum}";
            }

            // else = book, chapter, and verse
            return !string.IsNullOrWhiteSpace(bookText)
                ? $"{bookText + " " + ChapterNum + ":" + VerseNum}"
                : $"{"#" + BookNum + " " + ChapterNum + ":" + VerseNum}";

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
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ ChapterNum;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ VerseNum;
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

        /// <inheritdoc />
        public int CompareTo(VerseLocation other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var bookNumComparison = BookNum.CompareTo(other.BookNum);
            if (bookNumComparison != 0) return bookNumComparison;
            var chapterNumComparison = ChapterNum.CompareTo(other.ChapterNum);
            if (chapterNumComparison != 0) return chapterNumComparison;
            return VerseNum.CompareTo(other.VerseNum);
        }
    }
}
