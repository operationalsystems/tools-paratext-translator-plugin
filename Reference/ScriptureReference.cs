using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TvpMain.Project;

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

        public ScriptureReferenceWrapper(
            ParserType parserType,
            LocalReferenceMode referenceMode,
            string openingTag,
            ScriptureReference scriptureReference,
            string closingTag)
        {
            ParserType = parserType;
            ReferenceMode = referenceMode;
            OpeningTag = openingTag;
            ScriptureReference = scriptureReference ?? throw new ArgumentNullException(nameof(scriptureReference));
            ClosingTag = closingTag;
        }

        public long Score => (OpeningTag == null ? 0L : 1000L)
                          + (ScriptureReference.Score * 10L)
                          + (ClosingTag == null ? 0L : 1000L);

        protected bool Equals(ScriptureReferenceWrapper other)
        {
            return OpeningTag == other.OpeningTag && Equals(ScriptureReference, other.ScriptureReference) && ClosingTag == other.ClosingTag;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScriptureReferenceWrapper)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (OpeningTag != null ? OpeningTag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ScriptureReference != null ? ScriptureReference.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ClosingTag != null ? ClosingTag.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ScriptureReferenceWrapper left, ScriptureReferenceWrapper right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScriptureReferenceWrapper left, ScriptureReferenceWrapper right)
        {
            return !Equals(left, right);
        }

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

        public ScriptureReference(IList<BookVerseReference> bookReferences)
        {
            BookReferences = bookReferences ?? throw new ArgumentNullException(nameof(bookReferences)); ;
        }

        public long Score
        {
            get { return BookReferences.Sum(value => value.Score) * 10L; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected bool Equals(ScriptureReference other)
        {
            return BookReferences.Equals(other.BookReferences);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScriptureReference)obj);
        }

        public override int GetHashCode()
        {
            return BookReferences.GetHashCode();
        }

        public static bool operator ==(ScriptureReference left, ScriptureReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ScriptureReference left, ScriptureReference right)
        {
            return !Equals(left, right);
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

        public BookReferenceName(string nameText, BookNameItem nameItem)
        {
            NameText = nameText;
            NameItem = nameItem;
        }

        public long Score =>
            (NameText == null ? 0L : 1L)
            + (NameItem == null ? 0L : 10L);

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected bool Equals(BookReferenceName other)
        {
            return NameText == other.NameText && Equals(NameItem, other.NameItem);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BookReferenceName)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((NameText != null ? NameText.GetHashCode() : 0) * 397) ^ (NameItem != null ? NameItem.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BookReferenceName left, BookReferenceName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BookReferenceName left, BookReferenceName right)
        {
            return !Equals(left, right);
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

        public BookVerseReference(BookReferenceName bookReferenceName, IList<ChapterRange> chapterRanges)
        {
            BookReferenceName = bookReferenceName;
            ChapterRanges = chapterRanges ?? throw new ArgumentNullException(nameof(chapterRanges));
        }

        public long Score
        {
            get
            {
                return (BookReferenceName == null ? 0L : BookReferenceName.Score * 10L)
                        + (ChapterRanges.Sum(value => value.Score) * 10L);
            }
        }

        protected bool Equals(BookVerseReference other)
        {
            return Equals(BookReferenceName, other.BookReferenceName) && Equals(ChapterRanges, other.ChapterRanges);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BookVerseReference)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((BookReferenceName != null ? BookReferenceName.GetHashCode() : 0) * 397) ^ (ChapterRanges != null ? ChapterRanges.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BookVerseReference left, BookVerseReference right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BookVerseReference left, BookVerseReference right)
        {
            return !Equals(left, right);
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

        public ChapterRange(int chapter, IList<VerseRange> verseRanges) :
            this(chapter, verseRanges, -1, null)
        { }

        public ChapterRange(
            int fromChapter, IList<VerseRange> fromVerseRanges,
            int chapter, IList<VerseRange> verseRanges)
        {
            FromChapter = fromChapter;
            FromVerseRanges = fromVerseRanges;

            ToChapter = chapter;
            ToVerseRanges = verseRanges;

            if (!IsFromChapter && !IsToChapter
                && !IsFromVerseRanges && !IsToVerseRanges)
            {
                throw new ArgumentException("Invalid chapter range (empty).");
            }
        }

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

        protected bool Equals(ChapterRange other)
        {
            return FromChapter == other.FromChapter && Equals(FromVerseRanges, other.FromVerseRanges) && ToChapter == other.ToChapter && Equals(ToVerseRanges, other.ToVerseRanges);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ChapterRange)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FromChapter;
                hashCode = (hashCode * 397) ^ (FromVerseRanges != null ? FromVerseRanges.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ToChapter;
                hashCode = (hashCode * 397) ^ (ToVerseRanges != null ? ToVerseRanges.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ChapterRange left, ChapterRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ChapterRange left, ChapterRange right)
        {
            return !Equals(left, right);
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

        public VerseRange(int verse)
            : this(verse, -1) { }

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

        public long Score => IsSingleton ? 1L : 2L;

        protected bool Equals(VerseRange other)
        {
            return FromVerse == other.FromVerse
                   && ToVerse == other.ToVerse;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VerseRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (FromVerse * 397) ^ ToVerse;
            }
        }

        public static bool operator ==(VerseRange left, VerseRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VerseRange left, VerseRange right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
