using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pidgin;
using TvpMain.Project;

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
                    new VerseRange(fromVerse, toVerse), Tok(Parser.UnsignedInt(10)), AnyTok(referenceSeparators.VerseRangeSeparators), Tok(Parser.UnsignedInt(10)))
                .Labelled("paired verse range");

        public static Parser<char, VerseRange> VerseRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedVerseRange(referenceSeparators)),
                    Parser.Try(SingletonVerse()))
                .Labelled("verse range");

        public static Parser<char, IList<VerseRange>> VerseRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            =>
                VerseRange(referenceSeparators)
                    .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyTok(referenceSeparators.VerseSequenceSeparators))
                    .Select<IList<VerseRange>>(values => values.ToImmutableList())
                    .Labelled("verse ranges");

        public static Parser<char, BookOrChapterRange> SingletonBookOrChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((chapterNum, chapterSeparator, verseRanges) =>
                    new BookOrChapterRange(chapterNum, verseRanges), Tok(Parser.UnsignedInt(10)), AnyTok(referenceSeparators.ChapterAndVerseSeparators), VerseRangeSequence(referenceSeparators))
                .Labelled("singleton book or chapter range");

        public static Parser<char, BookOrChapterRange> PairedBookOrChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((fromBookOrChapter, bookSeparator, toBookOrChapter) => new BookOrChapterRange(
                    fromBookOrChapter.FromChapter, fromBookOrChapter.FromVerseRanges,
                    toBookOrChapter.FromChapter, toBookOrChapter.FromVerseRanges), SingletonBookOrChapterRange(referenceSeparators), AnyTok(referenceSeparators.BookOrChapterRangeSeparators), SingletonBookOrChapterRange(referenceSeparators))
                .Labelled("paired book or chapter range");

        public static Parser<char, BookReferenceName> BookReferenceName(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            =>
                Tok(Parser.LetterOrDigit.AtLeastOnceString()
                    .Where(inputText => inputText.Any(Char.IsLetter))
                    .Select(inputText =>
                        projectManager.BookNamesByAllNames.TryGetValue(inputText.Trim().ToLower(), out var nameItem)
                            ? new BookReferenceName(inputText, nameItem)
                            : new BookReferenceName(inputText, null))
                    .Labelled("book reference name"));

        public static Parser<char, BookOrChapterRange> BookOrChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedBookOrChapterRange(referenceSeparators)),
                    Parser.Try(SingletonBookOrChapterRange(referenceSeparators)))
                .Labelled("book or chapter range");

        public static Parser<char, IList<BookOrChapterRange>> BookOrChapterRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            =>
                BookOrChapterRange(referenceSeparators)
                    .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyTok(referenceSeparators.ChapterSequenceSeparators))
                    .Select<IList<BookOrChapterRange>>(values => values.ToImmutableList())
                    .Labelled("book or chapter ranges");

        public static Parser<char, BookVerseReference> LocalBookVerseReference(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(BookOrChapterRangeSequence(referenceSeparators)
                        .Select(value => new BookVerseReference(null, value))),
                    Parser.Try(VerseRangeSequence(referenceSeparators)
                        .Select(value => new BookVerseReference(null,
                            Enumerable.Repeat(new BookOrChapterRange(-1, value), 1).ToImmutableList()))))
                .Labelled("local book verse reference");

        public static Parser<char, BookVerseReference> OtherBookVerseReference(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((referenceName, bookOrChapterRange, optionalSeparator) =>
                    new BookVerseReference(referenceName, bookOrChapterRange), BookReferenceName(projectManager, referenceSeparators), BookOrChapterRangeSequence(referenceSeparators), AnyTok(referenceSeparators.BookSequenceSeparators).Optional())
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
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(
                    Parser.Try(OtherBookVerseReferenceSequence(projectManager, referenceSeparators)
                        .Select<ScriptureReference>(values => new ScriptureReference(values))),
                    Parser.Try(LocalBookVerseReference(referenceSeparators)
                        .Select<ScriptureReference>(value => new ScriptureReference(
                            Enumerable.Repeat(value, 1).ToImmutableList()))))
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
            ScriptureReferenceSeparators referenceSeparators)
            =>
                ScriptureReference(projectManager, referenceSeparators)
                    .Select(scriptureReference =>
                        new ScriptureReferenceWrapper(parserType, null, scriptureReference, null))
                    .Labelled("scripture reference with no tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((openingTag, scriptureReference) =>
                    new ScriptureReferenceWrapper(parserType, openingTag, scriptureReference, null), OpeningScriptureReferenceTag(), ScriptureReference(projectManager, referenceSeparators))
                .Labelled("scripture reference with opening tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, null, scriptureReference, closingTag), ScriptureReference(projectManager, referenceSeparators), ClosingScriptureReferenceTag())
                .Labelled("scripture reference with closing tag");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((openingTag, scriptureReference, closingTag) =>
                    new ScriptureReferenceWrapper(parserType, openingTag, scriptureReference, closingTag), OpeningScriptureReferenceTag(), ScriptureReference(projectManager, referenceSeparators), ClosingScriptureReferenceTag())
                .Labelled("scripture reference with both tags");

        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(ScriptureReferenceWithBothTags(parserType, projectManager, referenceSeparators)),
                    Parser.Try(ScriptureReferenceWithClosingTag(parserType, projectManager, referenceSeparators)),
                    Parser.Try(ScriptureReferenceWithOpeningTag(parserType, projectManager, referenceSeparators)),
                    Parser.Try(ScriptureReferenceWithNoTag(parserType, projectManager, referenceSeparators)))
                .Labelled("scripture reference wrapper");
    }
}
