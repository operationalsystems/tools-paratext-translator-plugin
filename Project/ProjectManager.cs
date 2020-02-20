using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using TvpMain.Result;
using TvpMain.Export;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Project
{
    /// <summary>
    /// Provides important project setting and metadata access.
    /// </summary>
    public class ProjectManager
    {
        /// <summary>
        /// Regex for splitting separator settings.
        /// </summary>
        public static readonly Regex SeparatorRegex = new Regex("\\|",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Paratext host interface.
        /// </summary>
        public IHost Host { get; }

        /// <summary>
        /// Active project name.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// Project file manager.
        /// </summary>
        public FileManager FileManager { get; }

        /// <summary>
        /// Readable chapter and verse separators.
        /// </summary>
        public IList<string> ChapterAndVerseSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) chapter and verse separator.
        /// </summary>
        public string StandardChapterAndVerseSeparator { get; private set; }

        /// <summary>
        /// Readable verse range separators.
        /// </summary>
        public IList<string> VerseRangeSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) verse range separator.
        /// </summary>
        public string StandardVerseRangeSeparator { get; private set; }

        /// <summary>
        /// Readable verse sequence separators.
        /// </summary>
        public IList<string> VerseSequenceSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) verse sequence separator.
        /// </summary>
        public string StandardVerseSequenceSeparator { get; private set; }

        /// <summary>
        /// Readable chapter or book range separators.
        /// </summary>
        public IList<string> BookOrChapterRangeSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) book or chapter range separator.
        /// </summary>
        public string StandardBookOrChapterRangeSeparator { get; private set; }

        /// <summary>
        /// Readable book sequence separators.
        /// </summary>
        public IList<string> BookSequenceSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) book sequence separator.
        /// </summary>
        public string StandardBookSequenceSeparator { get; private set; }

        /// <summary>
        /// Readable chapter sequence separators.
        /// </summary>
        public IList<string> ChapterSequenceSeparators { get; private set; }

        /// <summary>
        /// Standard (the first, currently) chapter sequence separator.
        /// </summary>
        public string StandardChapterSequenceSeparator { get; private set; }

        /// <summary>
        /// Extra reference prefixes or suffix options.
        /// </summary>
        public IList<string> ReferencePrefixesOrSuffixes { get; private set; }

        /// <summary>
        /// Standard (the first, currently) reference prefixes or suffix.
        /// </summary>
        public string StandardReferencePrefixesOrSuffix { get; private set; }

        /// <summary>
        /// Final reference punctuation options.
        /// </summary>
        public IList<string> FinalReferencePunctuation { get; private set; }

        /// <summary>
        /// Standard (the first, currently) final reference punctuation.
        /// </summary>
        public string StandardFinalReferencePunctuation { get; private set; }

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
        /// Cross reference (\xt tag) book name type.
        /// </summary>
        public BookNameType TargetNameType { get; private set; }

        /// <summary>
        /// Parallel passage book name type.
        /// </summary>
        public BookNameType PassageNameType { get; private set; }

        /// <summary>
        /// Book names map, keyed by book number (1-based).
        /// </summary>
        public virtual IDictionary<int, BookNameItem> BookNamesByNum { get; private set; }

        /// <summary>
        /// Book names map, keyed by all book names in lower case (i.e., code, abbreviation, short, long).
        /// </summary>
        public virtual IDictionary<string, BookNameItem> BookNamesByAllNames { get; private set; }

        /// <summary>
        /// Regexes to catch _possible_ target references in arbitrary text
        /// with typical project- and language-specific and English punctuation for error checking.
        /// </summary>
        public IList<Regex> TargetReferenceRegexes { get; private set; }

        /// <summary>
        /// Basic ctor, taking minimum args and creating major support objects
        /// (e.g., FileManager).
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public ProjectManager(
            IHost host, string activeProjectName)
        : this(host, activeProjectName,
            new FileManager(host, activeProjectName))
        { }

        /// <summary>
        /// Secondary ctor, taking all args including support objects.
        ///
        /// Note: Expected to be used for testing.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="fileManager">File manager (required).</param>
        public ProjectManager(
            IHost host, string activeProjectName,
            FileManager fileManager)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            ProjectName = activeProjectName
                          ?? throw new ArgumentNullException(nameof(activeProjectName));
            FileManager = fileManager
                          ?? throw new ArgumentNullException(nameof(fileManager));

            ReadBooksPresent();
            ReadSeparators();
            ReadBookNames();
            CreateRegexes();
        }

        /// <summary>
        /// Read present books array from settings.
        /// </summary>
        private void ReadBooksPresent()
        {
            var settingText = Host.GetProjectSetting(ProjectName, "BooksPresent");
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
            ChapterAndVerseSeparators =
                GetSeparatorSetting("ChapterVerseSeparator")
                    .ToImmutableList();
            StandardChapterAndVerseSeparator = ChapterAndVerseSeparators.Count > 0
                ? ChapterAndVerseSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_CHAPTER_AND_VERSE_SEPARATOR;

            VerseRangeSeparators =
                GetSeparatorSetting("RangeIndicator")
                    .ToImmutableList();
            StandardVerseRangeSeparator = VerseRangeSeparators.Count > 0
                ? VerseRangeSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_VERSE_RANGE_SEPARATOR;

            VerseSequenceSeparators =
                GetSeparatorSetting("SequenceIndicator")
                    .ToImmutableList();
            StandardVerseSequenceSeparator = VerseSequenceSeparators.Count > 0
                ? VerseSequenceSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_VERSE_SEQUENCE_SEPARATOR;

            BookOrChapterRangeSeparators =
                GetSeparatorSetting("ChapterRangeSeparator")
                    .ToImmutableList();
            StandardBookOrChapterRangeSeparator = BookOrChapterRangeSeparators.Count > 0
                ? BookOrChapterRangeSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_BOOK_OR_CHAPTER_RANGE_SEPARATOR;

            BookSequenceSeparators =
                GetSeparatorSetting("BookSequenceSeparator")
                    .ToImmutableList();
            StandardBookSequenceSeparator = BookSequenceSeparators.Count > 0
                ? BookSequenceSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_BOOK_SEQUENCE_SEPARATOR;

            ChapterSequenceSeparators =
                GetSeparatorSetting("ChapterNumberSeparator")
                    .ToImmutableList();
            StandardChapterSequenceSeparator = ChapterSequenceSeparators.Count > 0
                ? ChapterSequenceSeparators[0].Trim() : MainConsts.DEFAULT_REFERENCE_CHAPTER_SEQUENCE_SEPARATOR;

            ReferencePrefixesOrSuffixes =
                GetSeparatorSetting("ReferenceExtraMaterial")
                    .ToImmutableList();
            StandardReferencePrefixesOrSuffix = ReferencePrefixesOrSuffixes.Count > 0
                ? ReferencePrefixesOrSuffixes[0].Trim() : MainConsts.DEFAULT_REFERENCE_PREFIX_OR_SUFFIX;

            FinalReferencePunctuation =
                GetSeparatorSetting("ReferenceFinalPunctuation")
                    .ToImmutableList();
            StandardFinalReferencePunctuation = FinalReferencePunctuation.Count > 0
                ? FinalReferencePunctuation[0].Trim() : MainConsts.DEFAULT_REFERENCE_FINAL_PUNCTUATION;
        }

        /// <summary>
        /// Reads book names from XML file.
        /// </summary>
        private void ReadBookNames()
        {
            var tempBookNamesByNum = new Dictionary<int, BookNameItem>();
            if (FileManager.TryGetBookNamesFile(out var fileStream))
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

                            tempBookNamesByNum[bookId.BookNum]
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
            BookNamesByNum = tempBookNamesByNum.ToImmutableDictionary();

            var tempBookNamesByAllNames = new Dictionary<string, BookNameItem>();
            foreach (var nameItem in BookNamesByNum.Values)
            {
                tempBookNamesByAllNames[nameItem.BookCode.ToLower()] = nameItem;
                if (nameItem.IsAbbreviation)
                {
                    tempBookNamesByAllNames[nameItem.Abbreviation.ToLower()] = nameItem;
                }
                if (nameItem.IsShortName)
                {
                    tempBookNamesByAllNames[nameItem.ShortName.ToLower()] = nameItem;
                }
                if (nameItem.IsLongName)
                {
                    tempBookNamesByAllNames[nameItem.LongName.ToLower()] = nameItem;
                }
            }
            BookNamesByAllNames = tempBookNamesByAllNames.ToImmutableDictionary();

            // additional name-related settings
            TargetNameType = GetBookNameTypeSetting("BookSourceForMarkerXt", BookNameType.Abbreviation);
            PassageNameType = GetBookNameTypeSetting("BookSourceForMarkerR", BookNameType.ShortName);
        }

        /// <summary>
        /// Creates project-specific regexes.
        /// </summary>
        private void CreateRegexes()
        {
            var punctuationParts =
                ChapterAndVerseSeparators
                    .Concat(VerseRangeSeparators)
                    .Concat(VerseSequenceSeparators)
                    .Concat(BookOrChapterRangeSeparators)
                    .Concat(BookSequenceSeparators)
                    .Concat(ChapterSequenceSeparators)
                    .Select(punctuationItem => punctuationItem.Trim())
                    .Distinct()
                    .Select(Regex.Escape);

            var abbrevBookNames = BookNamesByNum.Values
                .Where(nameItem => nameItem.IsAbbreviation)
                .Select(nameItem => nameItem.Abbreviation);
            var shortBookNames = BookNamesByNum.Values
                .Where(nameItem => nameItem.IsShortName)
                .Select(nameItem => nameItem.ShortName);
            var longBookNames = BookNamesByNum.Values
                .Where(nameItem => nameItem.IsLongName)
                .Select(nameItem => nameItem.LongName);
            var allBookNames = abbrevBookNames
                .Concat(shortBookNames)
                .Concat(longBookNames)
                .Distinct()
                .Select(Regex.Escape);

            TargetReferenceRegexes = new List<Regex>()
                {
                    VerseRegexUtil.CreateTargetReferenceGroupRegex(
                        VerseRegexUtil.TargetReferencePairedTags.Select(Regex.Escape),
                        allBookNames
                            .Concat(VerseRegexUtil.STANDARD_BOOK_NAME_REGEX_TEXT.ToSingletonEnumerable()),
                        punctuationParts),
                }.Concat(VerseRegexUtil.StandardTargetReferenceRegexes)
                .ToImmutableList();
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
            var settingValue = Host.GetProjectSetting(ProjectName, settingKey);
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
        /// Extract a book name type from a project setting.
        /// </summary>
        /// <param name="settingKey">Setting key (required).</param>
        /// <param name="defaultType">Default book name type, if not found.</param>
        /// <returns>Book name type if found, default type otherwise.</returns>
        private BookNameType GetBookNameTypeSetting(string settingKey, BookNameType defaultType)
        {
            var settingValue = Host.GetProjectSetting(ProjectName, settingKey);
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                return defaultType;
            }
            else
            {
                var workSettingValue = settingKey.Trim().ToLower();
                switch (workSettingValue)
                {
                    case "abbreviation":
                        return BookNameType.Abbreviation;

                    case "shortname":
                        return BookNameType.ShortName;

                    case "longname":
                        return BookNameType.LongName;

                    default:
                        return defaultType;
                }
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

    /// <summary>
    /// Types of book names for (e.g.) target references.
    /// </summary>
    public enum BookNameType
    {
        Abbreviation,
        ShortName,
        LongName
    }
}
