using Pidgin;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TvpMain.Project;
using TvpMain.Util;

namespace TvpMain.Reference
{
    /// <summary>
    /// Holds static methods for scripture reference parser generation via Pidgin,
    /// a C# parser library.
    /// </summary>
    public static class ScriptureReferenceParser
    {
        public static readonly IList<char> WhitespaceChars =
            new List<char>()
            {
                ' ','\t', '\r', '\n'
            }.ToImmutableList();

        public static readonly IList<char> WhitespaceAndStarChars = WhitespaceChars.Concat(new List<char>()
        {
            '*'
        }).ToImmutableList();

        public static Parser<char, T> Tok<T>(Parser<char, T> token)
            => Parser.Try(token).Before(Parser.SkipWhitespaces);

        public static Parser<char, string> Tok(string token)
            =>
                Tok(Parser.CIString(token));

        public static Parser<char, string> AnyTok(IEnumerable<string> values)
        {
            return Parser.OneOf(values.Select(Tok)
                .ToImmutableArray());
        }

        public static Parser<char, VerseRange> SingletonVerse()
            =>
                Tok(Parser.UnsignedInt(10)
                        .Select<VerseRange>(value => new VerseRange(value))
                    .Labelled("singleton verse range"));

        public static Parser<char, VerseRange> PairedVerseRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((fromVerse, verseSeparator, toVerse) =>
                    new VerseRange(fromVerse.FromVerse, toVerse.FromVerse),
                    SingletonVerse(),
                    AnyTok(referenceSeparators.VerseRangeSeparators),
                    SingletonVerse())
                .Labelled("paired verse range");

        public static Parser<char, VerseRange> VerseRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedVerseRange(referenceSeparators)),
                    Parser.Try(SingletonVerse()))
                .Labelled("verse range");

        public static Parser<char, IList<VerseRange>> VerseRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            => VerseRange(referenceSeparators)
                    .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyTok(referenceSeparators.VerseSequenceSeparators))
                    .Select<IList<VerseRange>>(values => values.ToImmutableList())
                    .Labelled("verse range sequence");

        public static Parser<char, ChapterRange> SingletonChapterRangeWithVerses(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((chapterNum, chapterSeparator, verseRanges) =>
                        new ChapterRange(chapterNum, verseRanges),
                    Tok(Parser.UnsignedInt(10)),
                    AnyTok(referenceSeparators.ChapterAndVerseSeparators),
                    VerseRangeSequence(referenceSeparators))
                .Labelled("singleton chapter range with verses");

        public static Parser<char, ChapterRange> SingletonChapterRangeWithoutVerses(
            ScriptureReferenceSeparators referenceSeparators)
            => Tok(Parser.UnsignedInt(10))
                    .Select(value => new ChapterRange(value, null))
                .Labelled("singleton chapter range without verses");

        public static Parser<char, ChapterRange> SingletonChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(SingletonChapterRangeWithVerses(referenceSeparators)),
                    Parser.Try(SingletonChapterRangeWithoutVerses(referenceSeparators)))
                .Labelled("singleton chapter range");

        public static Parser<char, ChapterRange> PairedChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((fromChapter, bookSeparator, toChapter) => new ChapterRange(
                    fromChapter.FromChapter, fromChapter.FromVerseRanges,
                    toChapter.FromChapter, toChapter.FromVerseRanges),
                    SingletonChapterRange(referenceSeparators),
                    AnyTok(referenceSeparators.BookOrChapterRangeSeparators),
                    SingletonChapterRange(referenceSeparators))
                .Labelled("paired chapter range");

        public static Parser<char, BookReferenceName> BookReferenceName(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            =>
                Tok(Parser.LetterOrDigit.AtLeastOnceString()
                    .Where(inputText => inputText.Any(char.IsLetter))
                    .Select(inputText =>
                        projectManager.BookNamesByAllNames.TryGetValue(inputText.Trim().ToLower(), out var nameItem)
                            ? new BookReferenceName(inputText, nameItem)
                            : new BookReferenceName(inputText, null))
                    .Labelled("book reference name"));

        public static Parser<char, ChapterRange> ChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedChapterRange(referenceSeparators)),
                    Parser.Try(SingletonChapterRange(referenceSeparators)))
                .Labelled("chapter range");

        public static Parser<char, IList<ChapterRange>> ChapterRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            => ChapterRange(referenceSeparators)
                    .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyTok(referenceSeparators.ChapterSequenceSeparators))
                    .Select<IList<ChapterRange>>(values => values.ToImmutableList())
                    .Labelled("chapter range sequence");

        public static Parser<char, BookVerseReference> LocalBookVerseReference(
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => (referenceMode == LocalReferenceMode.ChapterRangeSequence)
                ? Parser.OneOf(
                        Parser.Try(ChapterRangeSequence(referenceSeparators)
                            .Select(value => new BookVerseReference(null, value))),
                        Parser.Try(VerseRangeSequence(referenceSeparators)
                            .Select(value => new BookVerseReference(null,
                                new ChapterRange(-1, value).ToSingletonList()))))
                    .Labelled("local book verse reference (chapter > verse)")
                : Parser.OneOf(
                        Parser.Try(VerseRangeSequence(referenceSeparators)
                            .Select(value => new BookVerseReference(null,
                                new ChapterRange(-1, value).ToSingletonList()))),
                        Parser.Try(ChapterRangeSequence(referenceSeparators)
                            .Select(value => new BookVerseReference(null, value))))
                    .Labelled("local book verse reference (verse > chapter)");


        public static Parser<char, BookVerseReference> OtherBookVerseReference(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((referenceName, chapterRange, optionalSeparator) =>
                    new BookVerseReference(referenceName, chapterRange),
                    BookReferenceName(projectManager, referenceSeparators),
                    ChapterRangeSequence(referenceSeparators),
                    AnyTok(referenceSeparators.BookSequenceSeparators).Optional())
                .Labelled("other (non-local) book verse reference");

        public static Parser<char, IList<BookVerseReference>> OtherBookVerseReferenceSequence(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            =>
                OtherBookVerseReference(projectManager, referenceSeparators)
                    .AtLeastOnce()
                    .Select<IList<BookVerseReference>>(values => values.ToImmutableList())
                    .Labelled("other (non-local) book verse references");

        public static Parser<char, ScriptureReference> ScriptureReference(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.OneOf(
                    Parser.Try(OtherBookVerseReferenceSequence(projectManager, referenceSeparators)
                        .Select<ScriptureReference>(values => new ScriptureReference(values))),
                    Parser.Try(LocalBookVerseReference(referenceSeparators, referenceMode)
                        .Select<ScriptureReference>(value => new ScriptureReference(
                            value.ToSingletonList()))))
                .Labelled("scripture reference");

        public static Parser<char, string> OpeningScriptureReferenceTag()
            =>
                Tok(Parser.Char('\\')
                    .Then(Parser.AnyCharExcept(WhitespaceChars)
                        .AtLeastOnceString())
                    .Labelled("opening reference tag"));

        public static Parser<char, string> ClosingScriptureReferenceTag()
            =>
                Tok(Parser.Char('\\')
                    .Then(Parser.AnyCharExcept(WhitespaceAndStarChars)
                        .AtLeastOnceString())
                    .Before(Parser.Char('*'))
                    .Labelled("closing reference tag"));

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithNoTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            =>
                ScriptureReference(projectManager, referenceSeparators, referenceMode)
                    .Select(scriptureReference =>
                        new ScriptureReferenceWrapper(parserType, referenceMode, null, scriptureReference, null))
                    .Labelled("scripture reference with no tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((openingTag, scriptureReference) =>
                    new ScriptureReferenceWrapper(parserType, referenceMode, openingTag, scriptureReference, null),
                    OpeningScriptureReferenceTag(),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode))
                .Labelled("scripture reference with opening tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, referenceMode, null, scriptureReference, closingTag),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode),
                    ClosingScriptureReferenceTag())
                .Labelled("scripture reference with closing tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((openingTag, scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, referenceMode, openingTag, scriptureReference, closingTag),
                    OpeningScriptureReferenceTag(),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode),
                    ClosingScriptureReferenceTag())
                .Labelled("scripture reference with both tags");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.OneOf(Parser.Try(ScriptureReferenceWithBothTags(parserType, projectManager, referenceSeparators, referenceMode)),
                    Parser.Try(ScriptureReferenceWithClosingTag(parserType, projectManager, referenceSeparators, referenceMode)),
                    Parser.Try(ScriptureReferenceWithOpeningTag(parserType, projectManager, referenceSeparators, referenceMode)),
                    Parser.Try(ScriptureReferenceWithNoTag(parserType, projectManager, referenceSeparators, referenceMode)))
                .Labelled("scripture reference wrapper");
    }

    /// <summary>
    /// Which sequence type to favor for local references,
    /// based on filtered input.
    /// </summary>
    public enum LocalReferenceMode
    {
        VerseRangeSequence,
        ChapterRangeSequence
    }
}
