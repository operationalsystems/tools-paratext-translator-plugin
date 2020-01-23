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
            => Map((fromVerse, toVerse) => new VerseRange(fromVerse, toVerse),
                    Tok(UnsignedInt(10))
                        .Before(AnyTok(referenceSeparators.VerseRangeSeparators)),
                    Tok(UnsignedInt(10)))
                .Labelled("paired verse range");

        protected static Parser<char, VerseRange> VerseRange(
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(PairedVerseRange(referenceSeparators)),
                    Try(SingletonVerse()))
                .Labelled("verse range");

        protected static Parser<char, IList<VerseRange>> VerseRanges(
            ReferenceSeparators referenceSeparators)
            => VerseRange(referenceSeparators)
                .Separated(AnyTok(referenceSeparators.VerseSequenceSeparators))
                .Select<IList<VerseRange>>(values => values.ToImmutableList())
                .Labelled("verse ranges");

        protected static Parser<char, BookOrChapterRange> SingletonBookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => Map((chapterNum, verseRanges) => new BookOrChapterRange(chapterNum, verseRanges),
                Tok(UnsignedInt(10))
                    .Before(AnyTok(referenceSeparators.ChapterAndVerseSeparators)),
                VerseRanges(referenceSeparators))
                .Labelled("singleton book or chapter range");

        protected static Parser<char, BookOrChapterRange> PairedBookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => Map((fromBookOrChapter, toBookOrChapter) => new BookOrChapterRange(
                        fromBookOrChapter.FromChapter, fromBookOrChapter.FromVerseRanges,
                        toBookOrChapter.FromChapter, toBookOrChapter.FromVerseRanges),
                    SingletonBookOrChapterRange(referenceSeparators)
                        .Before(AnyTok(referenceSeparators.BookOrChapterRangeSeparators)),
                    SingletonBookOrChapterRange(referenceSeparators))
                .Labelled("paired book or chapter range");

        protected static Parser<char, BookReferenceName> BookReferenceName(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => AnyTok(projectManager.BookNamesByAllNames.Keys)
                .Select(value => new BookReferenceName(value,
                    projectManager.BookNamesByAllNames[value.Trim().ToLower()]))
                .Labelled("book reference name");

        protected static Parser<char, BookOrChapterRange> BookOrChapterRange(
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(PairedBookOrChapterRange(referenceSeparators)),
                    Try(SingletonBookOrChapterRange(referenceSeparators)))
                .Labelled("book or chapter range");

        protected static Parser<char, BookVerseReference> LocalBookVerseReference(
            ReferenceSeparators referenceSeparators)
            => BookOrChapterRange(referenceSeparators)
                .Select(value => new BookVerseReference(null, value))
            .Labelled("local book verse reference");

        protected static Parser<char, BookVerseReference> OtherBookVerseReference(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((referenceName, bookOrChapterRange) => new BookVerseReference(referenceName, bookOrChapterRange),
                    BookReferenceName(projectManager, referenceSeparators),
                    BookOrChapterRange(referenceSeparators))
                .Labelled("other (non-local) book verse reference");

        protected static Parser<char, ScriptureReference> ScriptureReference(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => OneOf(
                    Try(OtherBookVerseReference(projectManager, referenceSeparators)
                        .Separated(AnyTok(referenceSeparators.BookSequenceSeparators))
                        .Select<ScriptureReference>(values => new ScriptureReference(values.ToImmutableList()))),
                    Try(LocalBookVerseReference(referenceSeparators)
                        .Select<ScriptureReference>(value => new ScriptureReference(
                            new List<BookVerseReference>() { value }.ToImmutableList()))))
                .Labelled("scripture reference");

        protected static Parser<char, string> OpeningScriptureReferenceTag()
            => Tok(Char('\\')
                    .Then(AnyCharExcept(WhitespaceChars)
                        .AtLeastOnceString()))
                .Labelled("opening reference tag");

        protected static Parser<char, string> ClosingScriptureReferenceTag()
            => Tok(Char('\\')
                    .Then(AnyCharExcept(WhitespaceAndStarChars)
                        .AtLeastOnceString())
                    .Before(Char('*')))
            .Labelled("closing reference tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithNoTag(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => ScriptureReference(projectManager, referenceSeparators)
                .Select(scriptureReference =>
                    new ScriptureReferenceWrapper(null, scriptureReference, null))
            .Labelled("scripture reference with no tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((openingTag, scriptureReference) =>
                    new ScriptureReferenceWrapper(openingTag, scriptureReference, null),
                OpeningScriptureReferenceTag(),
                ScriptureReference(projectManager, referenceSeparators))
            .Labelled("scripture reference with opening tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(null, scriptureReference, closingTag),
                ScriptureReference(projectManager, referenceSeparators),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with closing tag");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => Map((openingTag, scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(openingTag, scriptureReference, closingTag),
                OpeningScriptureReferenceTag(),
                ScriptureReference(projectManager, referenceSeparators),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with both tags");

        protected static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper(
            ProjectManager projectManager,
            ReferenceSeparators referenceSeparators)
            => OneOf(Try(ScriptureReferenceWithBothTags(projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithClosingTag(projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithOpeningTag(projectManager, referenceSeparators)),
                    Try(ScriptureReferenceWithNoTag(projectManager, referenceSeparators)))
            .Labelled("scripture reference wrapper");

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceBuilder(ProjectManager projectManager)
        {
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(ProjectManager));
            var tempProjectParserList = new List<Parser<char, ScriptureReferenceWrapper>>();

            var denormalizedSeparators = new ReferenceSeparators(ProjectManager, false);
            if (denormalizedSeparators.IsUsable)
            {
                tempProjectParserList.Add(
                    ScriptureReferenceWrapper(ProjectManager, denormalizedSeparators));
            }

            if (denormalizedSeparators.IsAnyDuplicates)
            {
                var normalizedSeparators = new ReferenceSeparators(ProjectManager, true);
                if (normalizedSeparators.IsUsable)
                {
                    tempProjectParserList.Add(
                        ScriptureReferenceWrapper(ProjectManager, normalizedSeparators));
                }
            }

            ProjectParsers = tempProjectParserList.ToImmutableList();
            StandardParsers = new List<Parser<char, ScriptureReferenceWrapper>>()
            {
                ScriptureReferenceWrapper(ProjectManager,
                    new ReferenceSeparators(
                        new string[] {";"},
                        new string[] {"&"},
                        new string[] {"-"},
                        new string[] {":"},
                        new string[] {"-"},
                        new string[] {":"},
                        false))
            }.ToImmutableList();
        }

        public bool TryParseScriptureReference(
            string inputText,
            out ScriptureReferenceWrapper outputWrapper)
        {
            foreach (var parserItem in ProjectParsers
                .Concat(StandardParsers))
            {
                var parseResult = parserItem.Parse(inputText);
                if (parseResult.Success)
                {
                    outputWrapper = parseResult.Value;
                    return true;
                }
            }

            outputWrapper = null;
            return false;
        }

        /// <summary>
        /// Manages normalized (de-conflicted) or full separator sets.
        ///
        /// At least a few projects have higher-level separators that are the same as
        /// lower-level ones (e.g., lists of books and lists of verses, both of which
        /// include ";" for "spaNVI15").
        ///
        /// Keeping these in place makes for un-navigable grammar, since most of the
        /// reference fields are indistinguishable integers without dubious classification
        /// (e.g., book number ranges) so this step prevents higher-level separators
        /// (e.g., lists of books) from being usable as lower-level ones
        /// (e.g., lists of verses).
        ///
        /// This is ok as we ultimately want a standard, parseable reference format and
        /// any reference actually containing these inversions will at least be caught as
        /// invalid by the checker and corrected or ignored.
        ///
        /// This may be an example of a position-independent grammar (e.g., programming
        /// language field modifiers) and at least Pidgin can support that, but it looks
        /// like that will allow truly undesirable combinations in exchange).
        /// 
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
                var prevSeparators = new HashSet<string>();
                var isAnyDuplicates = false;

                BookSequenceSeparators = FilterSeparators(
                    bookSequenceSeparators,
                    prevSeparators, isNormalized, ref isAnyDuplicates);
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
}
