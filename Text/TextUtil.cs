using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    /// Bible book-related utilities.
    /// </summary>
    public class TextUtil
    {
        /// <summary>
        /// List of note and reference regexes.
        /// </summary>
        private static readonly IList<Regex> NoteAndReferenceRegexes
            = new List<Regex>()
            {
                CreateNoteOrReferenceRegex("f"),
                CreateNoteOrReferenceRegex("x"),
                CreateNoteOrReferenceRegex("ef"),
                CreateNoteOrReferenceRegex("ex"),
                CreateNoteOrReferenceRegex("fe")
            }.ToImmutableList();

        /// <summary>
        /// List of line-oriented intro regexes.
        /// </summary>
        private static readonly IList<Regex> IntroRegexes
            = new List<Regex>()
            {
                CreateLineRegex("i", true),
                CreateLineRegex("usfm", false),
                CreateLineRegex("sts", false),
                CreateLineRegex("rem", false),
                CreateLineRegex("h", false),
            }.ToImmutableList();

        /// <summary>
        /// List of line-oriented TOC regexes.
        /// </summary>
        private static readonly IList<Regex> TocRegexes
            = new List<Regex>()
            {
                CreateLineRegex("toc", true)
            }.ToImmutableList();

        /// <summary>
        /// All regexes that find text we _don't_ want in extracted "main" text,
        /// needed because main scripture extractor will co-mingle everything.
        /// </summary>
        private static readonly IList<Regex> NonMainTextRegexes
            = IntroRegexes.Concat(TocRegexes)
                .ToImmutableList();

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

        static TextUtil()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            using var inputStream = executingAssembly.GetManifestResourceStream("TvpMain.Resources.book-ids-1.csv");
            using var streamReader = new StreamReader(inputStream);
            using var csvReader = new CsvReader(streamReader);

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

        /// <summary>
        /// Creates a note or reference regex from a tag name (note required interior non-space char).
        /// </summary>
        /// <param name="tagName">Tag name (required).</param>
        /// <returns></returns>
        private static Regex CreateNoteOrReferenceRegex(string tagName)
        {
            return new Regex($@"\\{tagName}\s*[\S](?:\s(?:(?!\\{tagName}).)*|\s*)\\{tagName}\*", RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates tag pair regex from a tag name.
        /// </summary>
        /// <param name="tagName">Tag name (required).</param>
        /// <returns></returns>
        private static Regex CreateTagPairRegex(string tagName)
        {
            return new Regex($@"\\{tagName}(?:\s(?:(?!\\{tagName}).)*|\s*)\\{tagName}\*", RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates a whole-line regex from a tag name (e.g., titles and tocs).
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="isPartial">True to match partial tags, false otherwise.</param>
        /// <returns></returns>
        private static Regex CreateLineRegex(string tagName, bool isPartial)
        {
            return new Regex(isPartial ? $@"^\s*\\{tagName}.*\r?$" : $@"^\s*\\{tagName}(?:(?!\\{tagName}).)*\r?$",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Finds parts of main text from co-mingled content.
        /// 
        /// Needed because while scripture extractors returns several contexts co-mingled,
        /// (e.g., main text, notes) we need them separated for context-sensitive checks.
        /// </summary>
        /// <param name="inputText">Input text containing mixed content (required).</param>
        /// <param name="mainParts"></param>
        /// <param name="introParts"></param>
        /// <param name="tocParts"></param>
        /// <returns></returns>
        public static bool FindMainParts(string inputText,
            ICollection<string> mainParts,
            ICollection<string> introParts,
            ICollection<string> tocParts)
        {
            var isFound = FindTextParts(inputText, NonMainTextRegexes, true, mainParts);
            isFound = FindTextParts(inputText, IntroRegexes, false, introParts) || isFound;
            isFound = FindTextParts(inputText, TocRegexes, false, tocParts) || isFound;

            return isFound;
        }

        /// <summary>
        /// Find note or reference parts within mixed content.
        ///
        /// Needed because while scripture extractors returns several contexts co-mingled,
        /// (e.g., main text, notes) we need them separated for context-sensitive checks.
        /// </summary>
        /// <param name="inputText">Input text containing main, note, or reference content (required).</param>
        /// <param name="noteParts">Destination collection for found note and reference parts.</param>
        /// <returns>True if applicable content found, false otherwise.</returns>
        public static bool FindNoteOrReferenceParts(string inputText, ICollection<string> noteParts)
        {
            return FindTextParts(inputText, NoteAndReferenceRegexes, false, noteParts);
        }

        /// <summary>
        /// Find specific sub-elements within mixed content.
        /// 
        /// Needed because while scripture extractors returns several contexts co-mingled,
        /// (e.g., main text, notes) we need them separated for context-sensitive checks.
        /// </summary>
        /// <param name="inputText">Input text containing mixed content (required).</param>
        /// <param name="includeRegexes">Regexes to search for (required).</param>
        /// <param name="isNegative">True to find parts _not) matching regexes, false to find matching.</param>
        /// <param name="foundParts">Destination collection for found note and reference parts.</param>
        /// <returns>True if applicable content found, false otherwise.</returns>
        private static bool FindTextParts(
            string inputText, IEnumerable<Regex> includeRegexes,
            bool isNegative, ICollection<string> foundParts)
        {
            // create mask of note text
            var workBuilder = new StringBuilder(inputText, inputText.Length);
            var isFound = false;

            foreach (var noteRegex in includeRegexes)
            {
                foreach (Match matchItem in noteRegex.Matches(inputText))
                {
                    for (var ctr = matchItem.Index;
                        ctr < (matchItem.Index + matchItem.Length);
                        ctr++)
                    {
                        workBuilder[ctr] = '\0';
                        isFound = true;
                    }
                }
            }

            // filter out non-masked text (i.e., main text)
            if (isFound)
            {
                var outputBuilder = new StringBuilder();
                var isNewLine = false;
                var isAdded = false;

                for (var ctr = 0;
                    ctr < workBuilder.Length;
                    ctr++)
                {
                    if ((isNegative && workBuilder[ctr] != '\0')
                        || (!isNegative && workBuilder[ctr] == '\0'))
                    {
                        if (isNewLine)
                        {
                            if (outputBuilder.Length > 0)
                            {
                                foundParts.Add(outputBuilder.ToString());
                                isAdded = true;

                                outputBuilder.Clear();
                            }

                            isNewLine = false;
                        }
                        outputBuilder.Append(inputText[ctr]);
                    }
                    else
                    {
                        isNewLine = true;
                    }
                }

                if (isNewLine
                    && outputBuilder.Length > 0)
                {
                    foundParts.Add(outputBuilder.ToString());
                    isAdded = true;

                    outputBuilder.Clear();
                }

                return isAdded;
            }
            else
            {
                if (isNegative)
                {
                    foundParts.Add(inputText);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
