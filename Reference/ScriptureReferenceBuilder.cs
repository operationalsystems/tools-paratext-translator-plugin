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

        public ProjectManager ProjectManager { get; }

        public IList<string> BookSequenceSeparators { get; private set; }

        public IList<string> ChapterSequenceSeparators { get; private set; }

        public IList<string> BookOrChapterRangeSeparators { get; private set; }

        public IList<string> ChapterAndVerseSeparators { get; private set; }

        public IList<string> VerseSequenceSeparators { get; private set; }

        public IList<string> VerseRangeSeparators { get; private set; }

        public Parser<char, T> Tok<T>(Parser<char, T> token)
            => Try(token).Before(SkipWhitespaces);

        public Parser<char, string> Tok(string token)
            => Tok(CIString(token));

        public Parser<char, string> AnyTok(IEnumerable<string> values)
        {
            return OneOf(values.Select(Tok)
                .ToImmutableArray());
        }

        public Parser<char, VerseRange> SingletonVerse()
            => Tok(UnsignedInt(10)
                .Select<VerseRange>(value => new VerseRange(value))
                .Labelled("singleton verse range"));

        public Parser<char, VerseRange> PairedVerseRange()
            => Map((fromVerse, toVerse) => new VerseRange(fromVerse, toVerse),
                    Tok(UnsignedInt(10)).Before(AnyTok(VerseRangeSeparators)),
                    Tok(UnsignedInt(10)))
                .Labelled("paired verse range");

        public Parser<char, VerseRange> VerseRange()
            => OneOf(Try(PairedVerseRange()),
                    Try(SingletonVerse()))
                .Labelled("verse range");

        public Parser<char, IList<VerseRange>> VerseRanges()
            => VerseRange()
                .Separated(AnyTok(VerseSequenceSeparators))
                .Select<IList<VerseRange>>(values => values.ToImmutableList())
                .Labelled("verse ranges");

        public Parser<char, BookOrChapterRange> SingletonBookOrChapterRange()
            => Map((chapterNum, verseRanges) => new BookOrChapterRange(chapterNum, verseRanges),
                Tok(UnsignedInt(10)).Before(AnyTok(ChapterAndVerseSeparators)),
                VerseRanges())
                .Labelled("singleton book or chapter range");

        public Parser<char, BookOrChapterRange> PairedBookOrChapterRange()
            => Map((fromBookOrChapter, toBookOrChapter) => new BookOrChapterRange(
                        fromBookOrChapter.FromChapter, fromBookOrChapter.FromVerseRanges,
                        toBookOrChapter.FromChapter, toBookOrChapter.FromVerseRanges),
                    SingletonBookOrChapterRange().Before(AnyTok(BookOrChapterRangeSeparators)),
                    SingletonBookOrChapterRange())
                .Labelled("paired book or chapter range");

        public Parser<char, BookReferenceName> BookReferenceName()
            => AnyTok(ProjectManager.BookNamesByAllNames.Keys)
                .Select(value => new BookReferenceName(value, ProjectManager.BookNamesByAllNames[value.Trim().ToLower()]))
                .Labelled("book reference name");

        public Parser<char, BookOrChapterRange> BookOrChapterRange()
            => OneOf(Try(PairedBookOrChapterRange()),
                    Try(SingletonBookOrChapterRange()))
                .Labelled("book or chapter range");

        public Parser<char, BookVerseReference> LocalBookVerseReference()
            => BookOrChapterRange()
                .Select(value => new BookVerseReference(null, value))
            .Labelled("local book verse reference");

        public Parser<char, BookVerseReference> OtherBookVerseReference()
            => Map((referenceName, bookOrChapterRange) => new BookVerseReference(referenceName, bookOrChapterRange),
                    BookReferenceName(),
                    BookOrChapterRange())
                .Labelled("other (non-local) book verse reference");

        public Parser<char, BookVerseReference> BookVerseReference()
            => OneOf(Try(OtherBookVerseReference()),
                Try(LocalBookVerseReference()))
            .Labelled("book verse reference");

        public Parser<char, ScriptureReference> ScriptureReference()
            => BookVerseReference()
                .Separated(AnyTok(BookSequenceSeparators))
                .Select<ScriptureReference>(values => new ScriptureReference(values.ToImmutableList()))
                .Labelled("scripture reference");

        public Parser<char, string> OpeningScriptureReferenceTag()
            => Tok(Map((part1, tagName) => tagName,
                String(@"\"),
                AnyCharExcept(WhitespaceChars)
                    .AtLeastOnceString()))
                .Labelled("opening reference tag");

        public Parser<char, string> ClosingScriptureReferenceTag()
            => Tok(Map((part1, tagName, part4) => tagName,
                String(@"\"),
                AnyCharExcept(WhitespaceChars)
                    .AtLeastOnceString(),
                String("*")))
            .Labelled("closing reference tag");

        public Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithNoTag()
            => ScriptureReference()
                .Select(scriptureReference =>
                    new ScriptureReferenceWrapper(null, scriptureReference, null))
            .Labelled("scripture reference with no tag");

        public Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag()
            => Map((openingTag, scriptureReference) =>
                    new ScriptureReferenceWrapper(openingTag, scriptureReference, null),
                OpeningScriptureReferenceTag(),
                ScriptureReference())
            .Labelled("scripture reference with opening tag");

        public Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag()
            => Map((scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(null, scriptureReference, closingTag),
                ScriptureReference(),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with closing tag");

        public Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags()
            => Map((openingTag, scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(openingTag, scriptureReference, closingTag),
                OpeningScriptureReferenceTag(),
                ScriptureReference(),
                ClosingScriptureReferenceTag())
            .Labelled("scripture reference with both tags");

        public Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper()
            => OneOf(Try(ScriptureReferenceWithBothTags()),
                    Try(ScriptureReferenceWithClosingTag()),
                    Try(ScriptureReferenceWithOpeningTag()),
                    Try(ScriptureReferenceWithNoTag()))
            .Labelled("scripture reference wrapper");

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceBuilder(ProjectManager projectManager)
        {
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(ProjectManager));

            BuildProjectSeparators();
        }

        /// <summary>
        /// Builds de-conflicted separators.
        ///
        /// At least a few projects have higher-level separators that are the same as
        /// lower-level ones (e.g., lists of books and lists of verses, both of which
        /// include ";" for "spaNVI15").
        ///
        /// Keeping these in place makes for un-navigable grammar, since most of the
        /// fields are indistinguishable integers so this step prevents higher-level
        /// separators (e.g., lists of books) from being usable as lower-level ones
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
        public void BuildProjectSeparators()
        {
            var excludedSeparators = new HashSet<string>();

            var tempBookSequenceSeparators =
                ProjectManager.BookSequenceSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempBookSequenceSeparators.ForEach(value => excludedSeparators.Add(value));
            BookSequenceSeparators = tempBookSequenceSeparators;

            var tempChapterSequenceSeparators =
                ProjectManager.ChapterSequenceSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempChapterSequenceSeparators.ForEach(value => excludedSeparators.Add(value));
            ChapterSequenceSeparators = tempChapterSequenceSeparators;

            var tempBookOrChapterRangeSeparators =
                ProjectManager.BookOrChapterRangeSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempBookOrChapterRangeSeparators.ForEach(value => excludedSeparators.Add(value));
            BookOrChapterRangeSeparators = tempBookOrChapterRangeSeparators;

            var tempChapterAndVerseSeparators =
                ProjectManager.ChapterAndVerseSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempChapterAndVerseSeparators.ForEach(value => excludedSeparators.Add(value));
            ChapterAndVerseSeparators = tempChapterAndVerseSeparators;

            var tempVerseSequenceSeparators =
                ProjectManager.VerseSequenceSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempVerseSequenceSeparators.ForEach(value => excludedSeparators.Add(value));
            VerseSequenceSeparators = tempVerseSequenceSeparators;

            var tempVerseRangeSeparators =
                ProjectManager.VerseRangeSeparators
                    .Select(value => value.Trim().ToLower())
                    .Where(value => !excludedSeparators.Contains(value))
                    .ToImmutableList();
            tempVerseRangeSeparators.ForEach(value => excludedSeparators.Add(value));
            VerseRangeSeparators = tempVerseRangeSeparators;
        }

        public Parser<char, ScriptureReferenceWrapper> CreateParser()
        {
            return ScriptureReferenceWrapper();
        }
    }
}
