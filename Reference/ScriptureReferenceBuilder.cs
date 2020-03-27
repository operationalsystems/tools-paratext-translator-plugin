using Pidgin;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using TvpMain.Project;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Reference
{
    /// <summary>
    /// Grammar utility methods.
    /// </summary>
    public class ScriptureReferenceBuilder
    {
        /// <summary>
        /// Project manager with needed settings and metadata.
        /// </summary>
        public ProjectManager ProjectManager { get; }

        /// <summary>
        /// Project-specific parsers.
        /// </summary>
        private IDictionary<LocalReferenceMode, IList<Parser<char, ScriptureReferenceWrapper>>> ProjectParsers { get; }

        /// <summary>
        /// Standard (backup) parsers.
        /// </summary>
        private IDictionary<LocalReferenceMode, IList<Parser<char, ScriptureReferenceWrapper>>> StandardParsers { get; }

        /// <summary>
        /// Part contexts that require target-specific book name formats.
        /// </summary>
        private static readonly ISet<PartContext> TargetNameContexts =
            new HashSet<PartContext>()
            {
                PartContext.MainText,
                PartContext.NoteOrReference
            }.ToImmutableHashSet();

        /// <summary>
        /// Separators relevant for local references consisting only of chapter range sequences.
        /// </summary>
        private readonly IList<string> _chapterReferenceModeSeparators;

        /// <summary>
        /// Separators relevant for local references consisting only of verse range sequences.
        /// </summary>
        private readonly IList<string> _verseReferenceModeSeparators;

        /// <summary>
        /// Basic ctor.
        ///
        /// Creates project-specific and standard parsers.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceBuilder(ProjectManager projectManager)
        {
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(ProjectManager));

            var tempProjectParsers = new Dictionary<LocalReferenceMode, IList<Parser<char, ScriptureReferenceWrapper>>>();
            var tempStandardParsers = new Dictionary<LocalReferenceMode, IList<Parser<char, ScriptureReferenceWrapper>>>();

            foreach (var referenceMode in
                Enum.GetValues(typeof(LocalReferenceMode)).Cast<LocalReferenceMode>())
            {
                var tempProjectParserList = new List<Parser<char, ScriptureReferenceWrapper>>();
                var projectSeparators = new ScriptureReferenceSeparators(ProjectManager, false);

                if (projectSeparators.IsUsable)
                {
                    tempProjectParserList.Add(
                        ScriptureReferenceParser.ScriptureReferenceWrapper(
                            ParserType.Project,
                            ProjectManager,
                            projectSeparators,
                            referenceMode));
                }

                if (projectSeparators.IsAnyDuplicates)
                {
                    var normalizedSeparators = new ScriptureReferenceSeparators(ProjectManager, true);
                    if (normalizedSeparators.IsUsable)
                    {
                        tempProjectParserList.Add(
                            ScriptureReferenceParser.ScriptureReferenceWrapper(
                            ParserType.Normalized,
                            ProjectManager,
                            normalizedSeparators,
                            referenceMode));
                    }
                }

                tempProjectParsers[referenceMode] = tempProjectParserList.ToImmutableList();
                tempStandardParsers[referenceMode] =
                    new List<Parser<char, ScriptureReferenceWrapper>>()
                {
                    ScriptureReferenceParser.ScriptureReferenceWrapper(
                        ParserType.Standard,
                        ProjectManager,
                        new ScriptureReferenceSeparators(
                            MainConsts.DEFAULT_REFERENCE_BOOK_SEQUENCE_SEPARATOR.ToSingletonEnumerable(),
                            MainConsts.DEFAULT_REFERENCE_CHAPTER_SEQUENCE_SEPARATOR.ToSingletonEnumerable(),
                            MainConsts.DEFAULT_REFERENCE_BOOK_OR_CHAPTER_RANGE_SEPARATOR.ToSingletonEnumerable(),
                            MainConsts.DEFAULT_REFERENCE_CHAPTER_AND_VERSE_SEPARATOR.ToSingletonEnumerable(),
                            MainConsts.DEFAULT_REFERENCE_VERSE_SEQUENCE_SEPARATOR.ToSingletonEnumerable(),
                            MainConsts.DEFAULT_REFERENCE_VERSE_RANGE_SEPARATOR.ToSingletonEnumerable(),
                            false),
                        referenceMode)
                }.ToImmutableList();

            }

            ProjectParsers = tempProjectParsers.ToImmutableDictionary();
            StandardParsers = tempStandardParsers.ToImmutableDictionary();

            _chapterReferenceModeSeparators =
                ProjectManager.BookOrChapterRangeSeparators
                    .Concat(ProjectManager.ChapterSequenceSeparators)
                    .Concat(ProjectManager.ChapterAndVerseSeparators)
                    .Where(separatorItem => !string.IsNullOrWhiteSpace(separatorItem))
                    .Select(separatorItem => separatorItem.Trim())
                    .Distinct()
                    .ToImmutableList();
            _verseReferenceModeSeparators =
                ProjectManager.VerseRangeSeparators
                    .Concat(ProjectManager.VerseSequenceSeparators)
                    .Where(separatorItem => !string.IsNullOrWhiteSpace(separatorItem))
                    .Select(separatorItem => separatorItem.Trim())
                    .Distinct()
                    .ToImmutableList();
        }

        /// <summary>
        /// Attempts to parse input text into a scripture reference.
        ///
        /// Projects support multiple separators for each level of a scripture reference
        /// (e.g., book and chapter sequences, verse ranges) and some allow the same
        /// separators at different levels (e.g., "spaNVA15" allows ";" for book, chapter,
        /// and verse sequences), meaning the same input could yield multiple results
        /// of very different value with the same, context-free grammar.
        ///
        /// E.g., 
        /// Mat 1:2–3:4; 5:6–7:8
        ///
        /// ...Is intuitively two chapter and verse ranges because the expression is internally
        /// consistent, but should the project allow ";" as both a chapter and verse range and
        /// verse sequence separator (as "spaNVA15"), a parser could interpret this as:
        ///
        /// c1v2 through c3v4 (and) c5v6 through c7v8 (correct)
        /// c1v2 through c3v4 (and) v5 (end parse) (incorrect, but consistent)
        ///
        /// This gets more involved with examples such as this, which are also valid:
        /// Mat 1:2–3:4; 5:6–7:8; Luk 2:3–4:5; 6:7–8:9
        ///
        /// ...this may be two chapter and verse ranges in both Mat and Luk, respectively
        /// _or_ one such range for both Mat and Luk interleaved with local (relative)
        /// chapter and verse ranges specific to wherever the reference appears.
        ///
        /// To address this, we apply multiple separator schemes to every input including
        /// the project-specified one (first), a normalized one where higher-level separators
        /// mask lower-level ones, and a standard one reflecting common English usage.
        ///
        /// Whichever scheme provides the highest-value result (i.e., the most book, chapter, and
        /// verse ranges, in that order of precedence) becomes the result of this method.
        /// </summary>
        /// <param name="inputText">Text to parse (required).</param>
        /// <param name="outputWrapper">Parsed scripture reference wrapper, if created (may be null).</param>
        /// <returns>True if parse successful, false otherwise.</returns>
        public bool TryParseScriptureReference(
            string inputText,
            out ScriptureReferenceWrapper outputWrapper)
        {
            outputWrapper = null;
            ScriptureReferenceWrapper bestWrapper = null;

            // figure whether chapter or verse separators come first,
            // to help choose the right parsers for local references
            var minChapterSeparator = inputText.MinIndexOf(
                true, false,
                _chapterReferenceModeSeparators);
            var minVerseSeparator = inputText.MinIndexOf(
                true, false,
                _verseReferenceModeSeparators);

            // select whether to emphasize chapter or verse parsing
            // in local references (i.e., without book names)
            var referenceMode = LocalReferenceMode.ChapterRangeSequence;
            if (minChapterSeparator >= 0 && minVerseSeparator >= 0)
            {
                referenceMode = (minVerseSeparator < minChapterSeparator)
                    ? LocalReferenceMode.VerseRangeSequence
                    : LocalReferenceMode.ChapterRangeSequence;
            }
            else if (minVerseSeparator >= 0)
            {
                referenceMode = LocalReferenceMode.VerseRangeSequence;
            }

            // iterate parsers and retain the highest-value result
            foreach (var parserItem in ProjectParsers[referenceMode]
                        .Concat(StandardParsers[referenceMode]))
            {
                var parseResult = parserItem.Parse(inputText);
                if (parseResult.Success)
                {
                    if (bestWrapper == null
                        || bestWrapper.Score < parseResult.Value.Score)
                    {
                        bestWrapper = parseResult.Value;
                    }
                }
            }

            outputWrapper = bestWrapper;
            return outputWrapper != null;
        }

        /// <summary>
        /// Format standard reference text including any tags, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputWrapper">Input scripture wrapper (required).</param>
        /// <returns>Formatted reference text.</returns>
        public string FormatStandardReference(
            PartContext inputContext,
            ScriptureReferenceWrapper inputWrapper)
        {
            return FormatStandardReference(
                inputContext, inputWrapper,
                false, null);
        }

        /// <summary>
        /// Format standard reference text including any tags, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputWrapper">Input scripture wrapper (required).</param>
        /// <param name="isSuppressTags">True to suppress tags and only include reference content.</param>
        /// <returns>Formatted reference text.</returns>
        public string FormatStandardReference(
            PartContext inputContext,
            ScriptureReferenceWrapper inputWrapper,
            bool isSuppressTags)
        {
            return FormatStandardReference(
                inputContext, inputWrapper,
                isSuppressTags, null);
        }

        /// <summary>
        /// Format standard reference text including any tags, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputWrapper">Input scripture wrapper (required).</param>
        /// <param name="overrideTag">Tag overriding tags present on scripture wrapper (optional; may be null).</param>
        /// <returns>Formatted reference text.</returns>
        public string FormatStandardReference(
            PartContext inputContext,
            ScriptureReferenceWrapper inputWrapper,
            string overrideTag)
        {
            return FormatStandardReference(
                inputContext, inputWrapper,
                false, overrideTag);
        }

        /// <summary>
        /// Format standard reference text, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputWrapper">Input scripture wrapper (required).</param>
        /// <param name="isSuppressTags">True to suppress tags and only include reference content.</param>
        /// <param name="overrideTag">Tag overriding tags present on scripture wrapper (optional; may be null).</param>
        /// <returns>Formatted reference text.</returns>
        public string FormatStandardReference(
            PartContext inputContext,
            ScriptureReferenceWrapper inputWrapper,
            bool isSuppressTags,
            string overrideTag)
        {
            var resultBuilder = new StringBuilder();
            var openingTag = inputWrapper.OpeningTag;
            var closingTag = inputWrapper.ClosingTag;

            // correct xt and ior tags, if not paired
            if (!string.IsNullOrWhiteSpace(openingTag)
                || !string.IsNullOrWhiteSpace(closingTag))
            {
                var firstTag = (openingTag ?? closingTag).Trim().ToLower();
                if (VerseRegexUtil.TargetReferencePairedTags.Contains(firstTag))
                {
                    openingTag = firstTag;
                    closingTag = string.Concat(firstTag, "*");
                }
            }

            // prepend tag, if present
            if (!isSuppressTags
                && (!string.IsNullOrWhiteSpace(overrideTag)
                || !string.IsNullOrWhiteSpace(openingTag)))
            {
                resultBuilder.AppendWithSpace($@"\{overrideTag ?? openingTag}");
            }

            // iterate book references, each including chapter and verse references
            for (var ctr1 = 0;
                ctr1 < inputWrapper.ScriptureReference.BookReferences.Count;
                ctr1++)
            {
                var bookVerseItem = inputWrapper.ScriptureReference.BookReferences[ctr1];
                if (ctr1 > 0)
                {
                    resultBuilder.Append(ProjectManager.StandardBookSequenceSeparator);
                }

                if (!bookVerseItem.IsLocalReference)
                {
                    resultBuilder.AppendWithSpace(
                        FormatBookName(inputContext, bookVerseItem.BookReferenceName));
                }

                // iterate chapter ranges, each including verse references
                for (var ctr2 = 0;
                    ctr2 < bookVerseItem.ChapterRanges.Count;
                    ctr2++)
                {
                    var bookOrChapterRange = bookVerseItem.ChapterRanges[ctr2];
                    if (ctr2 > 0)
                    {
                        resultBuilder.Append(ProjectManager.StandardChapterSequenceSeparator);
                    }

                    resultBuilder.AppendWithSpace(
                        FormatBookOrChapterRange(inputContext, bookOrChapterRange));
                }
            }

            if (!isSuppressTags
                && (!string.IsNullOrWhiteSpace(overrideTag)
                || !string.IsNullOrWhiteSpace(closingTag)))
            {
                resultBuilder.Append($@"\{overrideTag ?? closingTag}");
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Format book name from reference, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputName">Input name object (required).</param>
        private string FormatBookName(
            PartContext inputContext,
            BookReferenceName inputName)
        {
            if (inputName.IsKnownBook)
            {
                var nameType = TargetNameContexts.Contains(inputContext)
                    ? ProjectManager.TargetNameType
                    : ProjectManager.PassageNameType;
                return nameType switch
                {
                    BookNameType.Abbreviation =>
                    inputName.NameItem.GetAvailableName(BookNameType.Abbreviation,
                        BookNameType.ShortName, BookNameType.LongName),
                    BookNameType.ShortName =>
                    inputName.NameItem.GetAvailableName(BookNameType.ShortName,
                        BookNameType.Abbreviation, BookNameType.LongName),
                    BookNameType.LongName =>
                    inputName.NameItem.GetAvailableName(BookNameType.LongName,
                        BookNameType.Abbreviation, BookNameType.ShortName),
                    _ => inputName.NameItem.BookCode
                };
            }
            else
            {
                return inputName.NameText;
            }
        }

        /// <summary>
        /// Formats book or chapter range text, based on text context.
        /// For rebuilding an edited verse string based on formatting rules. 
        /// Different separators are used at different types of or sections of text.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputRange">Input book or chapter range (required).</param>
        /// <returns></returns>
        private string FormatBookOrChapterRange(
            PartContext inputContext,
            ChapterRange inputRange)
        {
            var resultBuilder = new StringBuilder();
            if (inputRange.IsFromChapter)
            {
                resultBuilder.Append(inputRange.FromChapter);
            }

            if (inputRange.IsFromVerseRanges)
            {
                if (inputRange.IsFromChapter)
                {
                    resultBuilder.Append(ProjectManager.StandardChapterAndVerseSeparator);
                }
                for (var ctr = 0;
                    ctr < inputRange.FromVerseRanges.Count;
                    ctr++)
                {
                    var verseRange = inputRange.FromVerseRanges[ctr];
                    if (ctr > 0)
                    {
                        resultBuilder.Append(ProjectManager.StandardVerseSequenceSeparator);
                    }

                    resultBuilder.Append(verseRange.FromVerse);
                    if (verseRange.IsSingleton) continue;

                    resultBuilder.Append(ProjectManager.StandardVerseRangeSeparator);
                    resultBuilder.Append(verseRange.ToVerse);
                }
            }

            if (inputRange.IsSingleton)
            {
                return resultBuilder.ToString();
            }

            resultBuilder.Append(ProjectManager.StandardBookOrChapterRangeSeparator);

            if (inputRange.IsToChapter)
            {
                resultBuilder.Append(inputRange.ToChapter);
            }

            if (inputRange.IsToVerseRanges)
            {
                if (inputRange.IsToChapter)
                {
                    resultBuilder.Append(ProjectManager.StandardChapterAndVerseSeparator);
                }

                for (var ctr = 0;
                    ctr < inputRange.ToVerseRanges.Count;
                    ctr++)
                {
                    var verseRange = inputRange.ToVerseRanges[ctr];
                    if (ctr > 0)
                    {
                        resultBuilder.Append(ProjectManager.StandardVerseSequenceSeparator);
                    }

                    resultBuilder.Append(verseRange.FromVerse);
                    if (verseRange.IsSingleton) continue;

                    resultBuilder.Append(ProjectManager.StandardVerseRangeSeparator);
                    resultBuilder.Append(verseRange.ToVerse);
                }
            }

            return resultBuilder.ToString();
        }
    }
}
