/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Reflection;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Bible text-related utilities.
    /// </summary>
    public class BookUtil
    {
        /// <summary>
        /// Book id list, from resource file.
        /// </summary>
        public static readonly IList<BookIdItem> BookIdList;

        /// <summary>
        /// Map of book codes to IDs.
        /// </summary>
        public static readonly IDictionary<string, BookIdItem> BookIdsByCode;

        /// <summary>
        /// Map of book numbers (1-based) to IDs.
        /// </summary>
        public static readonly IDictionary<int, BookIdItem> BookIdsByNum;

        static BookUtil()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            using var inputStream = executingAssembly.GetManifestResourceStream("TvpMain.Resources.book-ids-1.csv");
            using var streamReader = new StreamReader(inputStream);

            var config = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = false,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null
            };

            using var csvReader = new CsvReader(streamReader, config);

            BookIdList = csvReader.GetRecords<BookIdItem>().ToImmutableList();
            BookIdsByCode = BookIdList.ToImmutableDictionary(idItem => idItem.BookCode);
            BookIdsByNum = BookIdList.ToImmutableDictionary(idItem => idItem.BookNum);
        }

        /// <summary>
        /// Converts a Paratext coordinate reference to specific book, chapter, and verse.
        /// </summary>
        /// <param name="inputRef">Input coordinate reference (BBBCCCVVV).</param>
        /// <param name="outputBook">Output book number (1-based).</param>
        /// <param name="outputChapter">Output chapter number (1-1000; Max varies by book & versification).</param>
        /// <param name="outputVerse">Output verse number (1-1000; Max varies by chapter & versification).</param>
        public static void RefToBcv(int inputRef, out int outputBook, out int outputChapter, out int outputVerse)
        {
            outputBook = (inputRef / MainConsts.BookRefMultiplier);
            outputChapter = (inputRef / MainConsts.ChapRefMultiplier) % MainConsts.RefPartRange;
            outputVerse = inputRef % MainConsts.RefPartRange;
        }


        /// <summary>
        /// Converts specific book, chapter, and verse to a Paratext coordinate reference.
        /// </summary>
        /// <param name="inputBook">Input book number (1-based).</param>
        /// <param name="inputChapter">Input chapter number (1-1000; Max varies by book & versification).</param>
        /// <param name="inputVerse">Input verse number (1-1000; Max varies by chapter & versification).</param>
        /// <returns>Output coordinate reference (BBBCCCVVV).</returns>
        public static int BcvToRef(int inputBook, int inputChapter, int inputVerse)
        {
            return (inputBook * MainConsts.BookRefMultiplier)
                   + (inputChapter * MainConsts.ChapRefMultiplier)
                   + inputVerse;
        }
    }
}
