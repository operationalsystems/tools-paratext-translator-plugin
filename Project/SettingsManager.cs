using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using AddInSideViews;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Project
{
    public class SettingsManager
    {
        /// <summary>
        /// Regex for splitting separator settings.
        /// </summary>
        private static readonly Regex SeparatorRegex = new Regex("\\|",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _activeProjectName;

        /// <summary>
        /// Readable chapter and verse separators.
        /// </summary>
        public IList<string> ChapterOrVerseSeparators { get; private set; }

        /// <summary>
        /// Readable verse range separators.
        /// </summary>
        public IList<string> VerseRangeSeparators { get; private set; }

        /// <summary>
        /// Readable verse sequence separators.
        /// </summary>
        public IList<string> VerseSequenceSeparators { get; private set; }

        /// <summary>
        /// Readable chapter or book range separators.
        /// </summary>
        public IList<string> BookOrChapterRangeSeparators { get; private set; }

        /// <summary>
        /// Readable book sequence separators.
        /// </summary>
        public IList<string> BookSequenceSeparators { get; private set; }

        /// <summary>
        /// Readable chapter sequence separators.
        /// </summary>
        public IList<string> ChapterSequenceSeparators { get; private set; }

        /// <summary>
        /// Extra reference prefixes or suffix options.
        /// </summary>
        public IList<string> ReferencePrefixesOrSuffixes { get; private set; }

        /// <summary>
        /// Final reference punctuation options.
        /// </summary>
        public IList<string> FinalReferencePunctuation { get; private set; }

        /// <summary>
        /// Ordinal present books list (by 0-based book number).
        /// </summary>
        public IList<bool> PresentBookFlags { get; private set; }

        /// <summary>
        /// Present book number set (1-based).
        /// </summary>
        public ISet<int> PresentBookNums { get; private set; }

        /// <summary>
        /// Minimum present book number (1-based; -1 = none).
        /// </summary>
        public int MinPresentBookNum { get; private set; }

        /// <summary>
        /// Maximum present book number (1-based; -1 = none).
        /// </summary>
        public int MaxPresentBookNum { get; private set; }

        /// <summary>
        /// Book names map, keyed by book number (1-based).
        /// </summary>
        public IDictionary<int, BookNameItem> BookNames { get; private set; }

        /// <summary>
        /// Project file manager.
        /// </summary>
        private readonly FileManager _fileManager;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public SettingsManager(IHost host, string activeProjectName)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName
                                 ?? throw new ArgumentNullException(nameof(activeProjectName));

            _fileManager = new FileManager(_host, _activeProjectName);

            ReadBooksPresent();
            ReadSeparators();
            ReadBookNames();
        }

        /// <summary>
        /// Read present books array from settings.
        /// </summary>
        private void ReadBooksPresent()
        {
            var settingText = _host.GetProjectSetting(_activeProjectName, "BooksPresent");
            if (string.IsNullOrWhiteSpace(settingText))
            {
                PresentBookFlags = Enumerable.Empty<bool>()
                    .ToImmutableList();
                PresentBookNums = Enumerable.Empty<int>()
                    .ToImmutableHashSet();
                MinPresentBookNum = -1;
                MaxPresentBookNum = -1;
            }
            else
            {
                PresentBookFlags =
                        settingText.Trim()
                            .Select(charItem => charItem == '1')
                            .ToImmutableList();

                var currBookNum = 0;
                PresentBookNums =
                    PresentBookFlags
                        .Select(bookItem =>
                        {
                            currBookNum++;
                            return currBookNum;
                        })
                        .Where(bookNum =>
                            PresentBookFlags[bookNum - 1])
                        .ToImmutableHashSet();

                var tempMinPresentBookNum = int.MaxValue;
                var tempMaxPresentBookNum = int.MinValue;

                PresentBookNums.ToImmutableList()
                    .ForEach(bookNum =>
                    {
                        tempMinPresentBookNum = Math.Min(tempMinPresentBookNum, bookNum);
                        tempMaxPresentBookNum = Math.Max(tempMaxPresentBookNum, bookNum);
                    });

                MinPresentBookNum = tempMinPresentBookNum;
                MaxPresentBookNum = tempMaxPresentBookNum;
            }
        }

        /// <summary>
        /// Read separator lists from settings.
        /// </summary>
        private void ReadSeparators()
        {
            ChapterOrVerseSeparators =
                GetSeparatorSetting("ChapterVerseSeparator")
                    .ToImmutableList();

            VerseRangeSeparators =
                GetSeparatorSetting("RangeIndicator")
                    .ToImmutableList();

            VerseSequenceSeparators =
                GetSeparatorSetting("SequenceIndicator")
                    .ToImmutableList();

            BookOrChapterRangeSeparators =
                GetSeparatorSetting("ChapterRangeSeparator")
                    .ToImmutableList();

            BookSequenceSeparators =
                GetSeparatorSetting("BookSequenceSeparator")
                    .ToImmutableList();

            ChapterSequenceSeparators =
                GetSeparatorSetting("ChapterNumberSeparator")
                    .ToImmutableList();

            ReferencePrefixesOrSuffixes =
                GetSeparatorSetting("ReferenceExtraMaterial")
                    .ToImmutableList();

            FinalReferencePunctuation =
                GetSeparatorSetting("ReferenceFinalPunctuation")
                    .ToImmutableList();
        }

        /// <summary>
        /// Reads book names from XML file.
        /// </summary>
        private void ReadBookNames()
        {
            var tempBookNames = new Dictionary<int, BookNameItem>();

            if (_fileManager.TryGetBookNamesFile(out var fileStream))
            {
                using (fileStream)
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(fileStream);

                    // look for nodes
                    var nodeList = xmlDoc.SelectNodes("/BookNames/book");
                    if (nodeList != null)
                    {
                        // iterate nodes
                        foreach (XmlNode nodeItem in nodeList)
                        {
                            // no attributes or no/unusuable code = unusable
                            var codeAttrib = nodeItem.Attributes?["code"]?.Value?.Trim();
                            if (string.IsNullOrWhiteSpace(codeAttrib)
                                || !BookUtil.BookIdsByCode.TryGetValue(codeAttrib, out var bookId))
                            {
                                continue;
                            }

                            var abbrAttrib = nodeItem.Attributes["abbr"]?.Value?.Trim();
                            var shortAttrib = nodeItem.Attributes["short"]?.Value?.Trim();
                            var longAttrib = nodeItem.Attributes["long"]?.Value?.Trim();

                            // no value attribs = unusable
                            if (string.IsNullOrWhiteSpace(abbrAttrib)
                                && string.IsNullOrWhiteSpace(shortAttrib)
                                && string.IsNullOrWhiteSpace(longAttrib))
                            {
                                continue;
                            }

                            tempBookNames[bookId.BookNum]
                                = new BookNameItem(
                                    codeAttrib,
                                    bookId.BookNum,
                                    abbrAttrib,
                                    shortAttrib,
                                    longAttrib);
                        }
                    }
                }
            }

            BookNames = tempBookNames.ToImmutableDictionary();
        }

        /// <summary>
        /// Retrieves a separator-type setting (i.e., delimited by pipe characters).
        ///
        /// All result elements are whitespace trimmed.
        /// </summary>
        /// <param name="settingKey">Settings key (required).</param>
        /// <returns>List of setting values if found, empty list otherwise.</returns>
        private IEnumerable<string> GetSeparatorSetting(string settingKey)
        {
            var settingValue = _host.GetProjectSetting(_activeProjectName, settingKey);
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                return Enumerable.Empty<string>()
                    .ToImmutableList();
            }
            else
            {
                return SeparatorRegex.Split(settingValue)
                    .Where(listItem => !string.IsNullOrWhiteSpace(listItem))
                    .ToImmutableList();
            }
        }

        /// <summary>
        /// Determines whether a book is present in the given project.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <returns>True if book is present in project, false otherwise.</returns>
        public bool IsBookPresent(int bookNum)
        {
            return (bookNum >= 1 && bookNum <= PresentBookFlags.Count)
                   && PresentBookFlags[bookNum - 1];
        }
    }
}
