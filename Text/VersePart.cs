using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// A specific part of a verse.
    ///
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class VersePart
    {

        /// <summary>
        /// Source verse text.
        /// </summary>
        [JsonProperty(propertyName: "ParatextVerse")]
        public ProjectVerse ProjectVerse { get; private set; }

        /// <summary>
        /// Part location.
        /// </summary>
        [JsonProperty]
        public PartLocation PartLocation { get; private set; }

        /// <summary>
        /// Part text within verse.
        /// </summary>
        [JsonProperty]
        public string PartText { get; private set; }

        /// <summary>
        /// B/C/V summary text, including part range.
        /// </summary>
        [Ignore]
        public string PartCoordinateText =>
            $"{ProjectVerse.VerseLocation.VerseCoordinateText + "." + PartLocation.PartRangeText}";

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectVerse">Source verse data (required).</param>
        /// <param name="partText">Part text within verse.</param>
        /// <param name="partLocation"></param>
        public VersePart(ProjectVerse projectVerse, PartLocation partLocation, string partText)
        {
            ProjectVerse = projectVerse ?? throw new ArgumentNullException(nameof(projectVerse));
            PartLocation = partLocation ?? throw new ArgumentNullException(nameof(partLocation));
            PartText = partText ?? throw new ArgumentNullException(nameof(partText));
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private VersePart()
        {
        }

        /// <summary>
        /// Helper factory method that creates a part consisting of part of a verse.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <param name="chapterNum">Chapter number (1-based).</param>
        /// <param name="verseNum">Verse number (generally 1-based; 0 = any intro).</param>
        /// <param name="verseText">Verse text (required).</param>
        /// <param name="partStart">Part start within verse text (0-based).</param>
        /// <param name="partLength">Part length within verse text.</param>
        /// <param name="partContext">Part context (e.g., main text, footnote or reference).</param>
        /// <returns>Created part data.</returns>
        public static VersePart Create(
            int bookNum, int chapterNum, int verseNum, string verseText,
            int partStart, int partLength, PartContext partContext)
        {
            return new VersePart(
                ProjectVerse.Create(bookNum, chapterNum, verseNum, verseText),
                new PartLocation(partStart, partLength, partContext),
                verseText.Substring(partStart, partLength));
        }

        /// <summary>
        /// Helper factory method that creates a part consisting of an entire verse.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <param name="chapterNum">Chapter number (1-based).</param>
        /// <param name="verseNum">Verse number (generally 1-based; 0 = any intro).</param>
        /// <param name="verseText">Verse text (required).</param>
        /// <param name="partContext">Part context (e.g., main text, footnote or reference).</param>
        /// <returns>Created part data.</returns>
        public static VersePart Create(
            int bookNum, int chapterNum, int verseNum, string verseText,
            PartContext partContext)
        {
            return new VersePart(
                ProjectVerse.Create(bookNum, chapterNum, verseNum, verseText),
                new PartLocation(0, verseText.Length, partContext),
                verseText);
        }

        /// <summary>
        /// Typed equality method.
        /// </summary>
        /// <param name="other">Other part data (required).</param>
        /// <returns>True if equal, false otherwise</returns>
        protected bool Equals(VersePart other)
        {
            return Equals(ProjectVerse, other.ProjectVerse)
                   && Equals(PartLocation, other.PartLocation)
                   && PartText == other.PartText;
        }

        /// <summary>
        /// Standard equality method.
        /// </summary>
        /// <param name="obj">Other object (optional, may be null).</param>
        /// <returns>True if equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VersePart)obj);
        }

        /// <summary>
        /// Standard hash code method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ProjectVerse != null ? ProjectVerse.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (PartLocation != null ? PartLocation.GetHashCode() : 0);
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (PartText != null ? PartText.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        /// Typed equality operator.
        /// </summary>
        /// <param name="left">Left part data.</param>
        /// <param name="right">Right part data.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(VersePart left, VersePart right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Standard equality operator.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator !=(VersePart left, VersePart right)
        {
            return !Equals(left, right);
        }
    }
}
