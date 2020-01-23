using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Project;
using TvpMain.Text;

namespace TvpMain.Reference
{
    public class ScriptureReferenceWrapper
    {
        public string OpeningTag { get; }

        public bool IsOpeningTag => !string.IsNullOrWhiteSpace(OpeningTag);

        public ScriptureReference ScriptureReference { get; }

        public string ClosingTag { get; }

        public bool IsClosingTag => !string.IsNullOrWhiteSpace(ClosingTag);

        public ScriptureReferenceWrapper(
            string openingTag,
            ScriptureReference scriptureReference,
            string closingTag)
        {
            OpeningTag = openingTag;
            ScriptureReference = scriptureReference;
            ClosingTag = closingTag;
        }

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
            return $"{nameof(OpeningTag)}: {OpeningTag}, {nameof(IsOpeningTag)}: {IsOpeningTag}, {nameof(ScriptureReference)}: {ScriptureReference}, {nameof(ClosingTag)}: {ClosingTag}, {nameof(IsClosingTag)}: {IsClosingTag}";
        }
    }

    public class ScriptureReference
    {
        public IList<BookVerseReference> BookReferences { get; }

        public ScriptureReference(IList<BookVerseReference> bookReferences)
        {
            BookReferences = bookReferences;
        }

        public override string ToString()
        {
            return $"{nameof(BookReferences)}: {BookReferences}";
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

    public class BookReferenceName
    {
        public string NameText { get; }

        public BookNameItem NameItem { get; }

        public BookReferenceName(string nameText, BookNameItem nameItem)
        {
            NameText = nameText;
            NameItem = nameItem;
        }

        public override string ToString()
        {
            return $"{nameof(NameText)}: {NameText}, {nameof(NameItem)}: {NameItem}";
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

    public class BookVerseReference
    {
        public BookReferenceName BookReferenceName { get; }

        public bool IsLocalReference => BookReferenceName == null;

        public BookOrChapterRange BookOrChapterRange { get; }

        public BookVerseReference(BookReferenceName bookReferenceName, BookOrChapterRange bookOrChapterRanges)
        {
            BookReferenceName = bookReferenceName;
            BookOrChapterRange = bookOrChapterRanges ?? throw new ArgumentNullException(nameof(bookOrChapterRanges));
        }

        protected bool Equals(BookVerseReference other)
        {
            return Equals(BookReferenceName, other.BookReferenceName) && Equals(BookOrChapterRange, other.BookOrChapterRange);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BookVerseReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((BookReferenceName != null ? BookReferenceName.GetHashCode() : 0) * 397) ^ (BookOrChapterRange != null ? BookOrChapterRange.GetHashCode() : 0);
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

        public override string ToString()
        {
            return $"{nameof(BookReferenceName)}: {BookReferenceName}, {nameof(IsLocalReference)}: {IsLocalReference}, {nameof(BookOrChapterRange)}: {BookOrChapterRange}";
        }
    }

    public class BookOrChapterRange
    {
        public int FromChapter { get; }
        public IList<VerseRange> FromVerseRanges { get; }
        public int ToChapter { get; }
        public IList<VerseRange> ToVerseRanges { get; }

        public bool IsSingleton => FromVerseRanges == null
                                   || ToVerseRanges == null;

        public BookOrChapterRange(int chapter, IList<VerseRange> verseRanges) :
            this(chapter, verseRanges, -1, null)
        { }

        public BookOrChapterRange(int fromChapter, IList<VerseRange> fromVerseRanges, int chapter, IList<VerseRange> verseRanges)
        {
            FromChapter = fromChapter;
            FromVerseRanges = fromVerseRanges;

            ToChapter = chapter;
            ToVerseRanges = verseRanges;

            if (FromVerseRanges == null
                && ToVerseRanges == null)
            {
                throw new ArgumentNullException(nameof(FromVerseRanges));
            }
            else if (FromVerseRanges == null
                     || ToVerseRanges == null)
            {
                if (FromVerseRanges == null)
                {
                    FromVerseRanges = ToVerseRanges;
                    FromChapter = ToChapter;

                    ToVerseRanges = null;
                    ToChapter = -1;
                }
            }
        }

        protected bool Equals(BookOrChapterRange other)
        {
            return FromChapter == other.FromChapter && Equals(FromVerseRanges, other.FromVerseRanges) && ToChapter == other.ToChapter && Equals(ToVerseRanges, other.ToVerseRanges);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((BookOrChapterRange)obj);
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

        public static bool operator ==(BookOrChapterRange left, BookOrChapterRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BookOrChapterRange left, BookOrChapterRange right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(FromChapter)}: {FromChapter}, {nameof(FromVerseRanges)}: {FromVerseRanges}, {nameof(ToChapter)}: {ToChapter}, {nameof(ToVerseRanges)}: {ToVerseRanges}, {nameof(IsSingleton)}: {IsSingleton}";
        }
    }

    public class VerseRange
    {
        public int FromVerse { get; }

        public int ToVerse { get; }

        public bool IsSingleton => FromVerse == ToVerse;

        public VerseRange(int verse)
            : this(verse, verse) { }

        public VerseRange(int fromVerse, int toVerse)
        {
            FromVerse = fromVerse;
            ToVerse = toVerse;
        }

        protected bool Equals(VerseRange other)
        {
            return FromVerse == other.FromVerse && ToVerse == other.ToVerse;
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

        public override string ToString()
        {
            return $"{nameof(FromVerse)}: {FromVerse}, {nameof(ToVerse)}: {ToVerse}, {nameof(IsSingleton)}: {IsSingleton}";
        }
    }
}
