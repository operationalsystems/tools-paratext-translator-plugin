using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using Pidgin.Expression;
using Pidgin;
using TvpMain.Project;
using TvpMain.Text;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace TvpMain.Reference
{
    /// <summary>
    /// Grammar utility methods.
    /// </summary>
    public class ScriptureReferenceBuilder
    {
        public static readonly IList<char> WhitespaceChars =
            new List<char>()
            {
                ' ','\t', '\r', '\n'
            }.ToImmutableList();

        public static readonly IList<char> WhitespaceAndStarChars =
            WhitespaceChars.Concat(new List<char>()
            {
                '*'
            }).ToImmutableList();

        public ProjectManager ProjectManager { get; }

        private IList<Parser<char, ScriptureReferenceWrapper>> ProjectParsers { get; }

        private IList<Parser<char, ScriptureReferenceWrapper>> StandardParsers { get; }

        protected static Parser<char, T> Tok<T>(Parser<char, T> token)
            => Try(token).Before(SkipWhitespaces);

        protected static Parser<char, string> Tok(string token)
            => Tok(CIString(token));

        protected static Parser<char, string> AnyTok(IEnumerable<string> values)
        {
            return OneOf(values.Select(Tok)
                .ToImmutableArray());
        }

        protected static Parser<char, VerseRange> SingletonVerse()
            => Tok(UnsignedInt(10)
                .Select<VerseRange>(value => new VerseRange(value))
                .Labelled("singleton verse range"));

        protected static Parser<char, VerseRange> PairedVerseRange(
            ReferenceSeparators referenceSeparators)
            => Map((fromVerse, verseSeparator, toVerse) =>
                        new VerseRange(fromVerse, toVerse),
                    Tok(UnsignedInt(10)),
                    AnyTok(referenceSeparators.VerseRangeSeparators),
                    Tok(UnsignedInt(10)))
                .Labelled("paired verse range");

        protected static Parser<char, VerseRange> VerseRange(
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(PairedVerseRange(referenceSeparators)),
                    Try(SingletonVerse()))
                .Labelled("verse range");

        protected static Parser<char, IList<VerseRange>> VerseRangeSequence(
            ReferenceSeparators referenceSeparators)
            => VerseRange(referenceSeparators)
                .SeparatedAndOptionallyTerminated(AnyTok(referenceSeparators.VerseSequenceSeparators))
                .Select<IList<VerseRange>>(values => values.ToImmutableList())
                .Labelled("verse ranges");

        protected static Parser<char, BookOrChapterRange> SingletonBookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => Map((chapterNum, chapterSeparator, verseRanges) =>
                        new BookOrChapterRange(chapterNum, verseRanges),
                Tok(UnsignedInt(10)),
                AnyTok(referenceSeparators.ChapterAndVerseSeparators),
                VerseRangeSequence(referenceSeparators))
                .Labelled("singleton book or chapter range");

        protected static Parser<char, BookOrChapterRange> PairedBookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => Map((fromBookOrChapter, bookSeparator, toBookOrChapter) => new BookOrChapterRange(
                        fromBookOrChapter.FromChapter, fromBookOrChapter.FromVerseRanges,
                        toBookOrChapter.FromChapter, toBookOrChapter.FromVerseRanges),
                        SingletonBookOrChapterRange(referenceSeparators),
                        AnyTok(referenceSeparators.BookOrChapterRangeSeparators),
                        SingletonBookOrChapterRange(referenceSeparators))
                .Labelled("paired book or chapter range");

        protected static Parser<char, BookReferenceName> BookReferenceName(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Tok(LetterOrDigit.AtLeastOnceString()
                    .Where(inputText => inputText.Any(char.IsLetter))
                    .Select(inputText =>
                        projectManager.BookNamesByAllNames.TryGetValue(inputText.Trim().ToLower(), out var nameItem)
                            ? new BookReferenceName(inputText, nameItem)
                            : new BookReferenceName(inputText, null))
                .Labelled("book reference name"));

        protected static Parser<char, BookOrChapterRange> BookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(PairedBookOrChapterRange(referenceSeparators)),
                    Try(SingletonBookOrChapterRange(referenceSeparators)))
                .Labelled("book or chapter range");

        protected static Parser<char, IList<BookOrChapterRange>> BookOrChapterRangeSequence(
            ReferenceSeparators referenceSeparators)
            => BookOrChapterRange(referenceSeparators)
                .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyTok(referenceSeparators.ChapterSequenceSeparators))
                .Select<IList<BookOrChapterRange>>(values => values.ToImmutableList())
                .Labelled("book or chapter ranges");

        protected static Parser<char, BookVerseReference> LocalBookVerseReference(
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(BookOrChapterRangeSequence(referenceSeparators)
                    .Select(value => new BookVerseReference(null, value))),
                Try(VerseRangeSequence(referenceSeparators)
                    .Select(value => new BookVerseReference(null,
                       Enumerable.Repeat(new BookOrChapterRange(-1, value), 1).ToImmutableList()))))
            .Labelled("local book verse reference");
        protected static Parser<char, BookVerseReference> OtherBookVerseReference(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((referenceName, bookOrChapterRange, optionalSeparator) =>
                        new BookVerseReference(referenceName, bookOrChapterRange),
                    BookReferenceName(projectManager, referenceSeparators),
                    BookOrChapterRangeSequence(referenceSeparators),
                    AnyTok(referenceSeparators.BookSequenceSeparators).Optional())
                .Labelled("other (non-local) book verse reference");

        protected static Parser<char, IList<BookVerseReference>> OtherBookVerseReferenceSequence(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => OtherBookVerseReference(projectManager, referenceSeparators)
                .AtLeastOnce()
                .Select<IList<BookVerseReference>>(values => values.ToImmutableList())
                .Labelled("other (non-local) book verse references");

        protected static Parser<char, ScriptureReference> ScriptureReference(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => OneOf(
                Try(OtherBookVerseReferenceSequence(projectManager, referenceSeparators)
                        .Select<ScriptureReference>(values => new ScriptureReference(values))),
                Try(LocalBookVerseReference(referenceSeparators)
                    .Select<ScriptureReference>(value => new ScriptureReference(
                        Enumerable.Repeat(value, 1).ToImmutableList()))))
                .Labelled("scripture reference");

        protected static Parser<char, string> OpeningScriptureReferenceTag()
            => Tok(Char('\\')
                    .Then(AnyCharExcept(WhitespaceChars)
                        .AtLeastOnceString())
                .Labelled("opening reference tag"));

        protected static Parser<char, string> ClosingScriptureReferenceTag()
            => Tok(Char('\\')
                    .Then(AnyCharExcept(WhitespaceAndStarChars)
                        .AtLeastOnceString())
                    .Before(Char('*'))
            .Labelled("closing reference tag"));

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithNoTag(
            ParserType parserType,
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => ScriptureReference(projectManager, referenceSeparators)
                .Select(scriptureReference =>
                    new ScriptureReferenceWrapper(parserType, null, scriptureReference, null))
            .Labelled("scripture reference with no tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag(
            ParserType parserType,
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((openingTag, scriptureReference) =>
                    new ScriptureReferenceWrapper(parserType, openingTag, scriptureReference, null),
                OpeningScriptureReferenceTag(),
                ScriptureReference(projectManager, referenceSeparators))
            .Labelled("scripture reference with opening tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag(
            ParserType parserType,
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, null, scriptureReference, closingTag),
                ScriptureReference(projectManager, referenceSeparators),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with closing tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags(
            ParserType parserType,
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((openingTag, scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, openingTag, scriptureReference, closingTag),
                OpeningScriptureReferenceTag(),
                ScriptureReference(projectManager, referenceSeparators),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with both tags");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper(
            ParserType parserType,
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(ScriptureReferenceWithBothTags(parserType, projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithClosingTag(parserType, projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithOpeningTag(parserType, projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithNoTag(parserType, projectManager, referenceSeparators)))
            .Labelled("scripture reference wrapper");

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceBuilder(ProjectManager projectManager)
        {
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(ProjectManager));
            var tempProjectParserList = new List<Parser<char, ScriptureReferenceWrapper>>();

            var projectSeparators = new ReferenceSeparators(ProjectManager, false);
            if (projectSeparators.IsUsable)
            {
                tempProjectParserList.Add(
                    ScriptureReferenceWrapper(ParserType.Project, ProjectManager, projectSeparators));
            }

            if (projectSeparators.IsAnyDuplicates)
            {
                var normalizedSeparators = new ReferenceSeparators(ProjectManager, true);
                if (normalizedSeparators.IsUsable)
                {
                    tempProjectParserList.Add(
                        ScriptureReferenceWrapper(ParserType.Normalized, ProjectManager, normalizedSeparators));
                }
            }

            ProjectParsers = tempProjectParserList.ToImmutableList();
            StandardParsers = new List<Parser<char, ScriptureReferenceWrapper>>()
            {
                ScriptureReferenceWrapper(ParserType.Standard, ProjectManager,
                    new ReferenceSeparators(
                        new string[] {";"},
                        new string[] {";"},
                        new string[] {"-"},
                        new string[] {":"},
                        new string[] {"-"},
                        new string[] {":"},
                        false))
            }.ToImmutableList();
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
            ScriptureReferenceWrapper bestWrapper = null;

            foreach (var parserItem in ProjectParsers
                .Concat(StandardParsers))
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
        /// Format standard reference text, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputWrapper">Input scripture wrapper (required).</param>
        /// <returns>Formatted reference text.</returns>
        public string FormatStandardReference(PartContext inputContext,
                                                ScriptureReferenceWrapper inputWrapper)
        {
            var resultBuilder = new StringBuilder();
            var openingTag = inputWrapper.OpeningTag;
            var closingTag = inputWrapper.ClosingTag;

            // correct xt and ior tags, if not paired
            if (!string.IsNullOrWhiteSpace(openingTag)
                || !string.IsNullOrWhiteSpace(closingTag))
            {
                var firstTag = (openingTag ?? closingTag).Trim().ToLower();
                if (firstTag.Contains("xt")
                    || firstTag.Contains("ior"))
                {
                    openingTag = firstTag;
                    closingTag = string.Concat(firstTag, "*");
                }
            }

            if (!string.IsNullOrWhiteSpace(openingTag))
            {
                resultBuilder.AppendWithSpace($@"\{openingTag}");
            }

            for (var ctr1 = 0;
                    ctr1 < inputWrapper.ScriptureReference.BookReferences.Count;
                    ctr1++)
            {
                var bookVerseItem = inputWrapper.ScriptureReference.BookReferences[ctr1];
                if (ctr1 > 0)
                {
                    resultBuilder.Append(ProjectManager.BookSequenceSeparators[0].Trim());
                }

                if (!bookVerseItem.IsLocalReference)
                {
                    resultBuilder.AppendWithSpace(
                        FormatBookName(inputContext, bookVerseItem.BookReferenceName));
                }

                for (var ctr2 = 0;
                    ctr2 < bookVerseItem.BookOrChapterRanges.Count;
                    ctr2++)
                {
                    var bookOrChapterRange = bookVerseItem.BookOrChapterRanges[ctr2];
                    if (ctr2 > 0)
                    {
                        resultBuilder.Append(ProjectManager.ChapterSequenceSeparators[0].Trim());
                    }

                    resultBuilder.AppendWithSpace(
                        FormatBookOrChapterRange(inputContext, bookOrChapterRange));
                }
            }

            if (!string.IsNullOrWhiteSpace(closingTag))
            {
                resultBuilder.AppendWithSpace($@"\{closingTag}");
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
                return inputContext == PartContext.Introductions
                    ? inputName.NameItem.LongName
                    : inputName.NameItem.ShortName;
            }
            else
            {
                return inputName.NameText;
            }
        }

        /// <summary>
        /// Formats book or chapter range text, based on text context.
        /// </summary>
        /// <param name="inputContext">Input context (required).</param>
        /// <param name="inputRange">Input book or chapter range (required).</param>
        /// <returns></returns>
        private string FormatBookOrChapterRange(
            PartContext inputContext,
            BookOrChapterRange inputRange)
        {
            var resultBuilder = new StringBuilder();
            if (inputRange.IsFromChapter)
            {
                resultBuilder.Append(inputRange.FromChapter);
                resultBuilder.Append(ProjectManager.ChapterAndVerseSeparators[0].Trim());
            }

            for (var ctr = 0;
                ctr < inputRange.FromVerseRanges.Count;
                ctr++)
            {
                var verseRange = inputRange.FromVerseRanges[ctr];
                if (ctr > 0)
                {
                    resultBuilder.Append(ProjectManager.VerseSequenceSeparators[0].Trim());
                }

                resultBuilder.Append(verseRange.FromVerse);
                if (verseRange.IsSingleton) continue;

                resultBuilder.Append(ProjectManager.VerseRangeSeparators[0].Trim());
                resultBuilder.Append(verseRange.ToVerse);
            }

            if (inputRange.IsSingleton)
            {
                return resultBuilder.ToString();
            }

            resultBuilder.Append(ProjectManager.BookOrChapterRangeSeparators[0].Trim());
            if (inputRange.IsToChapter)
            {
                resultBuilder.Append(inputRange.ToChapter);
                resultBuilder.Append(ProjectManager.ChapterAndVerseSeparators[0].Trim());
            }

            for (var ctr = 0;
                ctr < inputRange.ToVerseRanges.Count;
                ctr++)
            {
                var verseRange = inputRange.ToVerseRanges[ctr];
                if (ctr > 0)
                {
                    resultBuilder.Append(ProjectManager.VerseSequenceSeparators[0].Trim());
                }

                resultBuilder.Append(verseRange.FromVerse);
                if (verseRange.IsSingleton) continue;

                resultBuilder.Append(ProjectManager.VerseRangeSeparators[0].Trim());
                resultBuilder.Append(verseRange.ToVerse);
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        /// Manages a full or normalized (de-conflicted) separator set.
        ///
        /// At least a few projects have higher-level separators that are the same as
        /// lower-level ones (e.g., lists of books and lists of verses, both of which
        /// include ";" for "spaNVI15").
        ///
        /// Keeping these in place makes for un-navigable grammar, since most of the
        /// reference fields are indistinguishable integers without dubious classification
        /// (e.g., book number ranges) so this step limits higher-level separators
        /// (e.g., lists of books) being accepted as lower-level ones (e.g., lists of verses).
        ///
        /// This is ok as we ultimately want a standard, parseable reference format and
        /// any reference actually containing these inversions will at least be caught as
        /// invalid by the checker and corrected or ignored.
        ///
        /// The next evolution of this should include context-sensitivity to filter separators
        /// based on what's encountered within the expression.
        /// </summary>
        protected class ReferenceSeparators
        {
            public IList<string> BookSequenceSeparators { get; }

            public IList<string> ChapterSequenceSeparators { get; }

            public IList<string> BookOrChapterRangeSeparators { get; }

            public IList<string> ChapterAndVerseSeparators { get; }

            public IList<string> VerseSequenceSeparators { get; }

            public IList<string> VerseRangeSeparators { get; }

            public bool IsAnyDuplicates { get; }

            public bool IsUsable =>
                BookSequenceSeparators.Count > 0
                && ChapterSequenceSeparators.Count > 0
                && BookOrChapterRangeSeparators.Count > 0
                && ChapterAndVerseSeparators.Count > 0
                && VerseSequenceSeparators.Count > 0
                && VerseRangeSeparators.Count > 0;

            public ReferenceSeparators(
                ProjectManager projectManager,
                bool isNormalized)
            : this(projectManager.BookSequenceSeparators,
                projectManager.ChapterSequenceSeparators,
                projectManager.BookOrChapterRangeSeparators,
                projectManager.ChapterAndVerseSeparators,
                projectManager.VerseSequenceSeparators,
                projectManager.VerseRangeSeparators,
                isNormalized)
            { }

            public ReferenceSeparators(
                IEnumerable<string> bookSequenceSeparators,
                IEnumerable<string> chapterSequenceSeparators,
                IEnumerable<string> bookOrChapterRangeSeparators,
                IEnumerable<string> chapterAndVerseSeparators,
                IEnumerable<string> verseSequenceSeparators,
                IEnumerable<string> verseRangeSeparators,
                bool isNormalized)
            {
                // Book sequence separators aren't literally used to separate 
                // collections by this parser as they're at the top level, so
                // they need not be used to mask other separators.
                BookSequenceSeparators = bookSequenceSeparators.ToImmutableList();

                var prevSeparators = new HashSet<string>();
                var isAnyDuplicates = false;

                ChapterSequenceSeparators = FilterSeparators(
                    chapterSequenceSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);
                BookOrChapterRangeSeparators = FilterSeparators(
                    bookOrChapterRangeSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);
                ChapterAndVerseSeparators = FilterSeparators(
                    chapterAndVerseSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);
                VerseSequenceSeparators = FilterSeparators(
                    verseSequenceSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);
                VerseRangeSeparators = FilterSeparators(
                    verseRangeSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);

                IsAnyDuplicates = isAnyDuplicates;
            }

            private static IList<string> FilterSeparators(
                IEnumerable<string> inputSeparators, ISet<string> prevSeparators,
                bool isNormalized, ref bool isAnyDuplicate)
            {
                var isListDuplicates = false;
                var result = inputSeparators
                        .Select(value => value.Trim().ToLower())
                        .Where(value =>
                        {
                            var isDuplicate = !prevSeparators.Add(value);
                            isListDuplicates = isListDuplicates || isDuplicate;

                            if (isNormalized)
                            {
                                return !isDuplicate;
                            }
                            else
                            {
                                return true;
                            }
                        })
                        .ToImmutableList();
                result.ForEach(value => prevSeparators.Add(value));
                isAnyDuplicate = isAnyDuplicate || isListDuplicates;

                return result;
            }
        }
    }

    /// <summary>
    /// Helper methods for string builders.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append to a string builder, adding a space first if
        /// there is already content and the input is non-null, non-empty.
        /// </summary>
        /// <param name="inputBuilder"></param>
        /// <param name="inputData"></param>
        public static void AppendWithSpace<T>(this StringBuilder inputBuilder, T inputData)
        {
            var inputText = inputData?.ToString();
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                if (inputBuilder.Length > 0)
                {
                    inputBuilder.Append(" ");
                }
                inputBuilder.Append(inputData);
            }
        }
    }
}
