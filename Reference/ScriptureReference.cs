using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TvpMain.Project;
using TvpMain.Text;

namespace TvpMain.Reference
{
    /// <summary>
    /// Origin parser type for a scripture reference wrapper.
    /// </summary>
    public enum ParserType
    {
        Project,
        Normalized,
        Standard
    }

    /// <summary>
    /// Container for a scripture reference with an optional open and closing tag.
    ///
    /// See scripture reference and lower-level docs for details.
    ///
    /// Top-level scripture reference parser result object.
    /// </summary>
    public class ScriptureReferenceWrapper
    {
        public string OpeningTag { get; }

        public bool IsOpeningTag => !string.IsNullOrWhiteSpace(OpeningTag);

        public ScriptureReference ScriptureReference { get; }

        public string ClosingTag { get; }

        public bool IsClosingTag => !string.IsNullOrWhiteSpace(ClosingTag);

        public ParserType ParserType { get; }

        public LocalReferenceMode ReferenceMode { get; }


        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="parserType">Parser type, to aid with debugging (e.g., project, normalized).</param>
        /// <param name="referenceMode">Local reference mode, to aid with debugging (e.g., book-first, verse-first).</param>
        /// <param name="openingTag">Opening tag, if present (optional, may be null).</param>
        /// <param name="scriptureReference">Scripture reference object (required).</param>
        /// <param name="closingTag">Closing tag, if present (optional, may be null).</param>
        public ScriptureReferenceWrapper(
            ParserType parserType,
            LocalReferenceMode referenceMode,
            string openingTag,
            ScriptureReference scriptureReference,
            string closingTag)
        {
            ParserType = parserType;
            ReferenceMode = referenceMode;
            OpeningTag = openingTag != null
                         && VerseRegexUtil.TargetReferencePairedTags.Contains(openingTag.ToLower())
                ? openingTag
                : null;
            ScriptureReference = scriptureReference ?? throw new ArgumentNullException(nameof(scriptureReference));
            ClosingTag = closingTag != null
                         && VerseRegexUtil.TargetReferencePairedTags.Contains(closingTag.ToLower())
                ? closingTag
                : null;
        }

        /// <summary>
        /// Quick utility method for giving a hash value to the reference.
        /// </summary>
        public long Score => (OpeningTag == null ? 0L : 1000L)
                          + (ScriptureReference.Score * 10L)
                          + (ClosingTag == null ? 0L : 1000L);

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A complete scripture reference, including one or more book references.
    ///
    /// Each of the following is a complete scripture reference, consisting of
    /// one or more book references. Actual references will be project- and
    /// language-specific (i.e., "Gn" for "Genesis" in "spaNVI15").
    /// 
    /// E.g., single, non-local book references:
    /// Mat 1:23
    /// Mat 1:1-3
    /// Mat 1:1,3
    /// Mat 1:2–3:4
    ///
    /// Multiple, non-local book references:
    /// Mat 1:2–3:4; 5:6–7:8; Luk 10:20–30:40; 50:60–70:80
    /// Mat 1:2–3:4; 5:6–7:8; Mrk 10:20–30:40; 50:60–70:80,83,85,87
    /// Mat 1:2–3:4; 5:6–7:8; Luk 10:20–30:40; 50:60–70:80; Mrk 11:21–31:41; 51:61–71:81-83,85,87
    ///
    /// Single, local book reference:
    /// 1:2; 3:4
    /// </summary>
    public class ScriptureReference
    {
        public IList<BookVerseReference> BookReferences { get; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookReferences">Constituent book references (required).</param>
        public ScriptureReference(IList<BookVerseReference> bookReferences)
        {
            BookReferences = bookReferences ?? throw new ArgumentNullException(nameof(bookReferences)); ;
        }

        /// <summary>
        /// Creates a hashed score value to compare results
        /// </summary>
        public long Score
        {
            get { return BookReferences.Sum(value => value.Score) * 10L; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// The name element of a book reference, including what was entered and
    /// a related book name from the project configuration, if found.
    ///
    /// E.g., in these book references:
    /// Mat 1:2; Luk 3:4
    ///
    /// ..."Mat" and "Luk" are name elements.
    /// </summary>
    public class BookReferenceName
    {
        public string NameText { get; }

        public BookNameItem NameItem { get; }

        public bool IsKnownBook => NameItem != null;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="nameText">Name text, if found (optional, may be null).</param>
        /// <param name="nameItem">Project-specific name item, if found (optional, may be null).</param>
        public BookReferenceName(string nameText, BookNameItem nameItem)
        {
            NameText = nameText;
            NameItem = nameItem;
        }

        /// <summary>
        /// Creates a hashed value to score results against
        /// </summary>
        public long Score =>
            (NameText == null ? 0L : 1L)
            + (NameItem == null ? 0L : 10L);

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A complete book reference, consisting of the name and chapter and verse elements.
    ///
    /// E.g., in this book reference:
    /// Mat 1:2–3:4
    ///
    /// ..."Mat" is the name element, "1:2–3:4" are the chapter and verse elements.
    /// </summary>
    public class BookVerseReference
    {
        public BookReferenceName BookReferenceName { get; }

        public bool IsLocalReference => BookReferenceName == null;

        public IList<ChapterRange> ChapterRanges { get; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="bookReferenceName">Book name element, if found (optional, may be null; null = local-only reference).</param>
        /// <param name="chapterRanges">Chapter ranges (required).</param>
        public BookVerseReference(BookReferenceName bookReferenceName, IList<ChapterRange> chapterRanges)
        {
            BookReferenceName = bookReferenceName;
            ChapterRanges = chapterRanges ?? throw new ArgumentNullException(nameof(chapterRanges));
        }

        /// <summary>
        /// Quick utility method for giving hash value to reference and ranges
        /// </summary>
        public long Score
        {
            get
            {
                return (BookReferenceName?.Score * 10L ?? 0L)
                        + (ChapterRanges.Sum(value => value.Score) * 10L);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// A book or chapter range, consisting of one or two chapters,
    /// each including individual or ranges and sequences of verses.
    ///
    /// E.g., in these book references:
    /// Mat 1:23
    /// Mat 1:1-3
    /// Mat 1:1,3
    /// Mat 1:2–3:4
    /// Mat 1:2–3:4; 5:6–7:8
    ///
    /// ...These are singleton book ranges:
    /// 1:23
    /// 1:1-3
    /// 1:1,3
    ///
    /// ...These are paired book ranges:
    /// 1:2–3:4
    /// 1:2–3:4; 5:6–7:8
    ///
    /// ...The last example has two paired ranges, contained at the
    /// book verse reference level.
    /// 
    /// </summary>
    public class ChapterRange
    {
        public int FromChapter { get; }
        public IList<VerseRange> FromVerseRanges { get; }
        public int ToChapter { get; }
        public IList<VerseRange> ToVerseRanges { get; }

        public bool IsSingleton => !IsToChapter;

        public bool IsFromChapter => FromChapter > 0;

        public bool IsToChapter => ToChapter > 0;

        public bool IsFromVerseRanges => FromVerseRanges != null;

        public bool IsToVerseRanges => ToVerseRanges != null;

        /// <summary>
        /// Basic ctor for singleton range (i.e., a single chapter).
        /// </summary>
        /// <param name="chapter">Chapter number (1-based).</param>
        /// <param name="verseRanges">Verse ranges, if found (optional, may be null).</param>
        public ChapterRange(int chapter, IList<VerseRange> verseRanges) :
            this(chapter, verseRanges, -1, null)
        { }

        /// <summary>
        /// Basic ctor for paired range (i.e., from-to).
        /// </summary>
        /// <param name="fromChapter">From chapter number (1-based).</param>
        /// <param name="fromVerseRanges">From verse ranges, if found (optional, may be null).</param>
        /// <param name="toChapter">To chapter number (1-based).</param>
        /// <param name="toVerseRanges">To verse ranges, if found (optional, may be null).</param>
        public ChapterRange(
            int fromChapter, IList<VerseRange> fromVerseRanges,
            int toChapter, IList<VerseRange> toVerseRanges)
        {
            FromChapter = fromChapter;
            FromVerseRanges = fromVerseRanges;

            ToChapter = toChapter;
            ToVerseRanges = toVerseRanges;

            if (!IsFromChapter && !IsToChapter
                && !IsFromVerseRanges && !IsToVerseRanges)
            {
                throw new ArgumentException("Invalid chapter range (empty).");
            }
        }

        /// <summary>
        /// Quick utility method for giving hash value to verse ranges
        /// </summary>
        public long Score
        {
            get
            {
                return (IsFromChapter ? 10L : 0L)
                       + (FromVerseRanges?.Sum(value => value.Score) * 10L ?? 0L)
                       + (IsToChapter ? 10L : 0L)
                       + (ToVerseRanges?.Sum(value => value.Score) * 10L ?? 0L);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// One or a range of verses.
    ///
    /// Considered a singleton if both from and to verse numbers are equal.
    ///
    /// E.g., in the following book references:
    /// Mat 1:23
    /// Mat 1:1-3
    /// Mat 1:1,3
    /// Mat 1:2–3:4
    ///
    /// ...These are verse ranges:
    /// 23 (singleton)
    /// 1-3 (range)
    /// 1,3 (two singletons, contained in a single book or chapter range)
    /// 2,4 (two singletons, contained in the "from" and "to" verse ranges
    /// of a single book or chapter range)
    ///  
    /// </summary>
    public class VerseRange
    {
        public int FromVerse { get; }

        public int ToVerse { get; }

        public bool IsSingleton => !IsToVerse;

        public bool IsFromVerse => FromVerse >= 0;

        public bool IsToVerse => ToVerse >= 0;

        /// <summary>
        /// Basic ctor for singleton range (i.e., one verse).
        /// </summary>
        /// <param name="verse">Verse number (0-based).</param>
        public VerseRange(int verse)
            : this(verse, -1) { }

        /// <summary>
        /// Basic ctor for paired verses (i.e., from-to).
        /// </summary>
        /// <param name="fromVerse">From verse number (0-based).</param>
        /// <param name="toVerse">To verse number (0-based).</param>
        public VerseRange(int fromVerse, int toVerse)
        {
            FromVerse = fromVerse;
            ToVerse = toVerse;

            if (!IsFromVerse
                && !IsToVerse)
            {
                throw new ArgumentException("Invalid verse range (empty).");
            }
        }

        /// <summary>
        ///  Quick utility method for giving hash value to verse range
        /// </summary>
        public long Score => IsSingleton ? 1L : 2L;

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
