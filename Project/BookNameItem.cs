using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Project
{
    public class BookNameItem
    {
        /// <summary>
        /// Book code.
        /// </summary>
        public string BookCode { get; private set; }

        /// <summary>
        /// Book number (1-based).
        /// </summary>
        public int BookNum { get; private set; }

        /// <summary>
        /// Language-specific abbreviation text (may be null/empty).
        /// </summary>
        public string Abbreviation { get; private set; }

        /// <summary>
        /// True if abbreviation is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsAbbreviation { get; private set; }

        /// <summary>
        /// Language-specific short name text (may be null/empty).
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        /// True if short name is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsShortName { get; private set; }

        /// <summary>
        /// Language-specific long name text (may be null/empty).
        /// </summary>
        public string LongName { get; private set; }

        /// <summary>
        /// True if long name is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsLongName { get; private set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookCode">Book code (required).</param>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <param name="abbreviation">Language-specific abbreviation (optional, may be null/empty).</param>
        /// <param name="shortName">Language-specific short name (optional, may be null/empty).</param>
        /// <param name="longName">Language-specific long name (optional, may be null/empty).</param>
        public BookNameItem(
            string bookCode, int bookNum,
            string abbreviation, string shortName,
            string longName)
        {
            BookCode = bookCode;
            BookNum = bookNum;

            Abbreviation = abbreviation;
            IsAbbreviation = !string.IsNullOrWhiteSpace(Abbreviation);

            ShortName = shortName;
            IsShortName = !string.IsNullOrWhiteSpace(ShortName);

            LongName = longName;
            IsLongName = !string.IsNullOrWhiteSpace(LongName);
        }
    }
}
