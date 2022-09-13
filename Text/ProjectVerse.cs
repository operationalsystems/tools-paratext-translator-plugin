/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Newtonsoft.Json;
using System;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Paratext verse data.
    ///
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ProjectVerse
    {

        /// <summary>
        /// Verse location.
        /// </summary>
        [JsonProperty]
        public VerseLocation VerseLocation { get; private set; }

        /// <summary>
        /// Verse text, containing any combination of main text, footnotes or references, etc.
        /// </summary>
        [JsonProperty]
        public string VerseText { get; private set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="verseLocation">Verse location (required).</param>
        /// <param name="verseText">Verse text (required).</param>
        public ProjectVerse(VerseLocation verseLocation, string verseText)
        {
            VerseLocation = verseLocation ?? throw new ArgumentNullException(nameof(verseLocation));
            VerseText = verseText ?? throw new ArgumentNullException(nameof(verseText));
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private ProjectVerse()
        {
        }

        /// <summary>
        /// Helper factory method.
        /// </summary>
        /// <param name="bookNum">Book number (1-based).</param>
        /// <param name="chapterNum">Chapter number (1-based).</param>
        /// <param name="verseNum">Verse number (generally 1-based; 0 = any intro).</param>
        /// <param name="verseText">Verse text (required).</param>
        /// <returns>Created part data.</returns>
        public static ProjectVerse Create(int bookNum, int chapterNum, int verseNum, string verseText)
        {
            return new ProjectVerse(
                    new VerseLocation(bookNum, chapterNum, verseNum),
                    verseText);
        }

        /// <summary>
        /// Typed equality method.
        /// </summary>
        /// <param name="other">Other verse data (required).</param>
        /// <returns>True if equal, false otherwise</returns>
        protected bool Equals(ProjectVerse other)
        {
            return Equals(VerseLocation, other.VerseLocation)
                   && VerseText == other.VerseText;
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
            return Equals((ProjectVerse)obj);
        }

        /// <summary>
        /// Standard hash code method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((VerseLocation != null
                            ? VerseLocation.GetHashCode() : 0) * MainConsts.HASH_PRIME)
                       ^ (VerseText != null ? VerseText.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Typed equality operator.
        /// </summary>
        /// <param name="left">Left verse data.</param>
        /// <param name="right">Right verse data.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(ProjectVerse left, ProjectVerse right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Standard equality operator.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator !=(ProjectVerse left, ProjectVerse right)
        {
            return !Equals(left, right);
        }
    }
}
