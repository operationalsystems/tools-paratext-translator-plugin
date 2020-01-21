using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using TvpMain.Result;
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
        /// Map of book codes to numbers (1-based).
        /// </summary>
        public static readonly IDictionary<string, BookIdItem> BookIdsByCode;

        /// <summary>
        /// Map of book numbers (1-based) to codes.
        /// </summary>
        public static readonly IDictionary<int, BookIdItem> BookIdsByNum;

        static BookUtil()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            using var inputStream = executingAssembly.GetManifestResourceStream("TvpMain.Resources.book-ids-1.csv");
            using var streamReader = new StreamReader(inputStream);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            csvReader.Configuration.HasHeaderRecord = false;
            csvReader.Configuration.IgnoreBlankLines = true;
            csvReader.Configuration.TrimOptions = TrimOptions.Trim;
            csvReader.Configuration.MissingFieldFound = null;

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
