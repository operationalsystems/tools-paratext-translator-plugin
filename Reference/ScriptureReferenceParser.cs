/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
    ///
    /// The methods in this class (a) construct parser objects from settings
    /// embodied by support classes such as ProjectManager and ScriptureReferenceSeparators,
    /// which in turn (b) are used by ScriptureReferenceBuilder to parse reference text into
    /// domain objects, rooted in ScriptureReferenceWrapper.
    ///
    /// Other domain objects created by these parsers and included within
    /// ScriptureReferenceWrapper objects include:
    /// - ScriptureReference
    /// - BookVerseReference
    /// - BookReferenceName
    /// - ChapterRange
    /// - VerseRange
    ///
    /// The top-level parser creation method is ScriptureReferenceWrapper(), near the top
    /// of this file and the only public method.
    ///
    /// Pidgin is a fluent API, a design with particular idioms described here:
    /// - https://en.wikipedia.org/wiki/Fluent_interface
    ///  
    /// From the Pidgin docs:
    ///
    /// Pidgin is a parser combinator library, a lightweight, high-level, 
    /// declarative tool for constructing parsers. Parsers written with parser 
    /// combinators look like a high-level specification of a language's 
    /// grammar, but they're expressed within a general-purpose programming 
    /// language and require no special tools to produce executable code. 
    /// Parser combinators are more powerful than regular expressions - they 
    /// can parse a larger class of languages - but simpler and easier to use 
    /// than parser generators like ANTLR.
    /// 
    /// Pidgin's core type, Parser<TToken, T>, represents a procedure which 
    /// consumes an input stream of TTokens, and may either fail with a 
    /// parsing error or produce a T as output. 
    /// 
    /// Parser combinators’ power comes from their composability. The library 
    /// comprises a small number of building blocks, which you can put 
    /// together in rich and varied ways to build a parser which does what you 
    /// need. The library’s level of abstraction is a good fit for 
    /// small-to-medium sized parsing tasks: it’s not as high-level as a 
    /// full-blown parser generator like Antlr, but it’s much simpler to 
    /// integrate.
    /// 
    /// Relevant links:
    /// - https://github.com/benjamin-hodgson/Pidgin
    /// - https://www.benjamin.pizza/Pidgin/v2.0.0/api/Pidgin.html
    /// - https://www.benjamin.pizza/posts/2019-01-26-announcing-pidgin-v2.0.html
    /// </summary>
    public static class ScriptureReferenceParser
    {
        /// <summary>
        /// List of whitespace chars to use as terminators.
        /// </summary>
        private static readonly IList<char> WhitespaceChars =
            new List<char>()
            {
                ' ', '\t', '\r', '\n'
            }.ToImmutableList();

        /// <summary>
        /// List of whitespace chars plus the star (*) char to use as end tag discriminators.
        /// </summary>
        private static readonly IList<char> WhitespaceAndStarChars =
            WhitespaceChars.Concat(new List<char>()
            {
                '*'
            }).ToImmutableList();

        /// <summary>
        /// Creates top-level ScriptureReferenceWrapper parser using settings objects.
        /// </summary>
        /// <param name="parserType">Parser type, to help with debugging parsed results.</param>
        /// <param name="projectManager">Project manager, for project-specific settings.</param>
        /// <param name="referenceSeparators">Project-specific separators relevant for this parser</param>
        /// <param name="referenceMode">Mode to use for local-only references (i.e., when book names don't define a hierarchy).</param>
        /// <returns>Created parser, ready to use.</returns>
        public static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWrapper(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.OneOf(
                    Parser.Try(ScriptureReferenceWithBothTags(parserType, projectManager, referenceSeparators,
                        referenceMode)),
                    Parser.Try(ScriptureReferenceWithClosingTag(parserType, projectManager, referenceSeparators,
                        referenceMode)),
                    Parser.Try(ScriptureReferenceWithOpeningTag(parserType, projectManager, referenceSeparators,
                        referenceMode)),
                    Parser.Try(ScriptureReferenceWithNoTag(parserType, projectManager, referenceSeparators,
                        referenceMode)))
                .Labelled("scripture reference wrapper");

        /// <summary>
        /// Creates lowest-level, whitespace-preceding token parser.
        /// </summary>
        /// <typeparam name="T">Output type (provided).</typeparam>
        /// <param name="token">Content parser to attempt (required).</param>
        /// <returns>Created parser.</returns>
        private static Parser<char, T> OneToken<T>(Parser<char, T> token)
            => Parser.Try(token).Before(Parser.SkipWhitespaces);

        /// <summary>
        /// Creates lowest-level, whitespace-preceding and case-insensitive string parser.
        /// </summary>
        /// <param name="inputToken">String to look for (required).</param>
        /// <returns>Created token.</returns>
        private static Parser<char, string> OneToken(string inputToken)
            => OneToken(Parser.CIString(inputToken));

        /// <summary>
        /// Creates a parser that attempts any of a supplied list of values,
        /// ignoring trailing whitespace.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Created parser.</returns>
        private static Parser<char, string> AnyToken(IEnumerable<string> values)
        {
            return Parser.OneOf(values.Select(OneToken)
                .ToImmutableArray());
        }

        /// <summary>
        /// Parser for finding a single verse
        /// </summary>
        private static Parser<char, VerseRange> SingletonVerse()
            =>
                OneToken(Parser.UnsignedInt(10)
                    .Select<VerseRange>(value => new VerseRange(value))
                    .Labelled("singleton verse range"));

        /// <summary>
        /// Creates a parser fro finding general references
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, VerseRange> PairedVerseRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((fromVerse, verseSeparator, toVerse) =>
                        new VerseRange(fromVerse.FromVerse, toVerse.FromVerse),
                    SingletonVerse(),
                    AnyToken(referenceSeparators.VerseRangeSeparators),
                    SingletonVerse())
                .Labelled("paired verse range");

        /// <summary>
        /// Creates a parser for finding verse ranges ex. 1-2
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, VerseRange> VerseRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedVerseRange(referenceSeparators)),
                    Parser.Try(SingletonVerse()))
                .Labelled("verse range");

        /// <summary>
        /// Creates a parser for finding verse range sequences ex. 1-2,3,3-4
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, IList<VerseRange>> VerseRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            => VerseRange(referenceSeparators)
                .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyToken(referenceSeparators.VerseSequenceSeparators))
                .Select<IList<VerseRange>>(values => values.ToImmutableList())
                .Labelled("verse range sequence");

        /// <summary>
        /// Creates a parser for finding chapters in verse ranges ex: 3:1-2,3,5:23,6:2-30
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, ChapterRange> SingletonChapterRangeWithVerses(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((chapterNum, chapterSeparator, verseRanges) =>
                        new ChapterRange(chapterNum, verseRanges),
                    OneToken(Parser.UnsignedInt(10)),
                    AnyToken(referenceSeparators.ChapterAndVerseSeparators),
                    VerseRangeSequence(referenceSeparators))
                .Labelled("singleton chapter range with verses");

        /// <summary>
        /// Creates a parser for finding chapter ranges that don't have verses associated
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, ChapterRange> SingletonChapterRangeWithoutVerses(
            ScriptureReferenceSeparators referenceSeparators)
            => OneToken(Parser.UnsignedInt(10))
                .Select(value => new ChapterRange(value, null))
                .Labelled("singleton chapter range without verses");

        /// <summary>
        /// Creates a parser for finding verse/chapter reference separators
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, ChapterRange> SingletonChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(SingletonChapterRangeWithVerses(referenceSeparators)),
                    Parser.Try(SingletonChapterRangeWithoutVerses(referenceSeparators)))
                .Labelled("singleton chapter range");

        /// <summary>
        /// Creates a parser for finding paired chapter ranges 2,3:34,4-8 , but that aren't verses
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, ChapterRange> PairedChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((fromChapter, bookSeparator, toChapter) => new ChapterRange(
                        fromChapter.FromChapter, fromChapter.FromVerseRanges,
                        toChapter.FromChapter, toChapter.FromVerseRanges),
                    SingletonChapterRange(referenceSeparators),
                    AnyToken(referenceSeparators.BookOrChapterRangeSeparators),
                    SingletonChapterRange(referenceSeparators))
                .Labelled("paired chapter range");

        /// <summary>
        /// Creates a parser for finding book names
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, BookReferenceName> BookReferenceName(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            =>
                Parser.OneOf(Parser.Try(OneToken(Parser.LetterOrDigit.AtLeastOnceString()
                    .Before(Parser.Char('.')))),
                        Parser.Try(OneToken(Parser.LetterOrDigit.AtLeastOnceString())))
                    .Where(inputText => inputText.Any(char.IsLetter))
                    .Select(inputText =>
                        projectManager.BookNamesByAllNames.TryGetValue(inputText.Trim().ToLower(), out var nameItem)
                            ? new BookReferenceName(inputText, nameItem)
                            : new BookReferenceName(inputText, null))
                    .Labelled("book reference name");

        /// <summary>
        /// Creates a parser for finding chapter ranges
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, ChapterRange> ChapterRange(
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.OneOf(Parser.Try(PairedChapterRange(referenceSeparators)),
                    Parser.Try(SingletonChapterRange(referenceSeparators)))
                .Labelled("chapter range");

        /// <summary>
        ///  Creates a parser for finding chapter range sequences
        /// </summary>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, IList<ChapterRange>> ChapterRangeSequence(
            ScriptureReferenceSeparators referenceSeparators)
            => ChapterRange(referenceSeparators)
                .SeparatedAndOptionallyTerminatedAtLeastOnce(AnyToken(referenceSeparators.ChapterSequenceSeparators))
                .Select<IList<ChapterRange>>(values => values.ToImmutableList())
                .Labelled("chapter range sequence");

        /// <summary>
        /// Creates a parser for finding verses in the local book instead of outside the local book
        /// </summary>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, BookVerseReference> LocalBookVerseReference(
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

        /// <summary>
        /// Creates a parser for finding verse references for verses outside the current book
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, BookVerseReference> OtherBookVerseReference(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            => Parser.Map((referenceName, chapterRange, optionalSeparator) =>
                        new BookVerseReference(referenceName, chapterRange),
                    BookReferenceName(projectManager, referenceSeparators),
                    ChapterRangeSequence(referenceSeparators),
                    AnyToken(referenceSeparators.BookSequenceSeparators).Optional())
                .Labelled("other (non-local) book verse reference");

        /// <summary>
        /// Creates a parser for finding verse references outside current book
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        private static Parser<char, IList<BookVerseReference>> OtherBookVerseReferenceSequence(
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators)
            =>
                OtherBookVerseReference(projectManager, referenceSeparators)
                    .AtLeastOnce()
                    .Select<IList<BookVerseReference>>(values => values.ToImmutableList())
                    .Labelled("other (non-local) book verse references");

        /// <summary>
        /// Creates a parser for finding general scripture references using other parsers in sequence
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, ScriptureReference> ScriptureReference(
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

        /// <summary>
        /// Creates a parser for finding opening reference tags
        /// </summary>
        private static Parser<char, string> OpeningScriptureReferenceTag()
            =>
                OneToken(Parser.Char('\\')
                    .Then(Parser.AnyCharExcept(WhitespaceChars)
                        .AtLeastOnceString())
                    .Labelled("opening reference tag"));

        /// <summary>
        /// Creates a parser for finding closing reference tags
        /// </summary>
        private static Parser<char, string> ClosingScriptureReferenceTag()
            =>
                OneToken(Parser.Char('\\')
                    .Then(Parser.AnyCharExcept(WhitespaceAndStarChars)
                        .AtLeastOnceString())
                    .Before(Parser.Char('*'))
                    .Labelled("closing reference tag"));

        /// <summary>
        /// Creates a parser for finding references that don't have tags
        /// </summary>
        /// <param name="parserType"></param>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithNoTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            =>
                ScriptureReference(projectManager, referenceSeparators, referenceMode)
                    .Select(scriptureReference =>
                        new ScriptureReferenceWrapper(parserType, referenceMode, null, scriptureReference, null))
                    .Labelled("scripture reference with no tag");

        /// <summary>
        /// Creates a parser for finding references that have opening tags, but possibly no closing tag
        /// </summary>
        /// <param name="parserType"></param>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithOpeningTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((openingTag, scriptureReference) =>
                        new ScriptureReferenceWrapper(parserType, referenceMode, openingTag, scriptureReference, null),
                    OpeningScriptureReferenceTag(),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode))
                .Labelled("scripture reference with opening tag");

        /// <summary>
        /// Creates a parser for finding references that have a closing tag, but no opening tag
        /// </summary>
        /// <param name="parserType"></param>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithClosingTag(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((scriptureReference, closingTag) =>
                        new ScriptureReferenceWrapper(parserType, referenceMode, null, scriptureReference, closingTag),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode),
                    ClosingScriptureReferenceTag())
                .Labelled("scripture reference with closing tag");

        /// <summary>
        /// Creates a parser that find references that have both tags in place
        /// </summary>
        /// <param name="parserType"></param>
        /// <param name="projectManager"></param>
        /// <param name="referenceSeparators"></param>
        /// <param name="referenceMode"></param>
        private static Parser<char, ScriptureReferenceWrapper> ScriptureReferenceWithBothTags(
            ParserType parserType,
            ProjectManager projectManager,
            ScriptureReferenceSeparators referenceSeparators,
            LocalReferenceMode referenceMode)
            => Parser.Map((openingTag, scriptureReference, closingTag) =>
                        new ScriptureReferenceWrapper(parserType, referenceMode, openingTag, scriptureReference,
                            closingTag),
                    OpeningScriptureReferenceTag(),
                    ScriptureReference(projectManager, referenceSeparators, referenceMode),
                    ClosingScriptureReferenceTag())
                .Labelled("scripture reference with both tags");
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
