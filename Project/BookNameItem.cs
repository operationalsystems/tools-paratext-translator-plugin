/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using TvpMain.Util;

namespace TvpMain.Project
{
    /// <summary>
    /// Model class for project-specific book names (abbreviation, short, and long).
    /// </summary>
    public class BookNameItem
    {
        /// <summary>
        /// Book code.
        /// </summary>
        public string BookCode { get; }

        /// <summary>
        /// Book number (1-based).
        /// </summary>
        public int BookNum { get; }

        /// <summary>
        /// Language-specific abbreviation text (may be null/empty).
        /// </summary>
        public string Abbreviation { get; }

        /// <summary>
        /// True if abbreviation is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsAbbreviation { get; }

        /// <summary>
        /// Language-specific short name text (may be null/empty).
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// True if short name is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsShortName { get; }

        /// <summary>
        /// Language-specific long name text (may be null/empty).
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// True if long name is not null, empty, or whitespace-only.
        /// </summary>
        public bool IsLongName { get; }

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
            BookCode = bookCode ?? "";
            BookNum = bookNum;

            Abbreviation = abbreviation?.Trim();
            IsAbbreviation = !string.IsNullOrWhiteSpace(Abbreviation);

            ShortName = shortName?.Trim();
            IsShortName = !string.IsNullOrWhiteSpace(ShortName);

            LongName = longName?.Trim();
            IsLongName = !string.IsNullOrWhiteSpace(LongName);
        }

        /// <summary>
        /// Gets name string based on what's available, according to a specified precedence.
        /// </summary>
        /// <param name="inputTypes">Book name type precedence (required).</param>
        /// <returns>First available name string or code string, whichever is first.</returns>
        public string GetAvailableName(params BookNameType[] inputTypes)
        {
            if (inputTypes == null)
            {
                HostUtil.Instance.LogLine("Instance types is null, responding with default book code", true);
                return BookCode;
            }

            foreach (var nameType in inputTypes)
            {
                switch (nameType)
                {
                    case BookNameType.Abbreviation:
                        if (IsAbbreviation)
                        {
                            return Abbreviation;
                        }
                        break;
                    case BookNameType.ShortName:
                        if (IsShortName)
                        {
                            return ShortName;
                        }
                        break;
                    case BookNameType.LongName:
                        if (IsLongName)
                        {
                            return LongName;
                        }
                        break;
                    default:
                        return BookCode;
                }
            }
            return BookCode;
        }

        /// <summary>
        /// Override of the default so that we can display the name
        /// </summary>
        /// <returns>The best name to display for the book in the list.</returns>
        public override string ToString()
        {
            string displayName = GetAvailableName(BookNameType.LongName, BookNameType.ShortName, BookNameType.Abbreviation);
            return Truncate(displayName, MainConsts.MAX_BOOK_NAME_DISPLAY_LENGTH);
        }

        /// <summary>
        /// In some cases the default length is very-very long, need to truncate for display purposes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns>A shorter, truncated version of a long name.</returns>
        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
