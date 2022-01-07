/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using TvpMain.Util;

namespace TvpMain.Text
{
    /// <summary>
    /// Location of a specific part of a verse.
    ///
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PartLocation
    {

        /// <summary>
        /// Text context.
        /// </summary>
        [JsonProperty]
        public PartContext PartContext { get; private set; }

        /// <summary>
        /// Start of the part, relative to verse start (0-based).
        /// </summary>
        [JsonProperty]
        public int PartStart { get; private set; }

        /// <summary>
        /// Length of the part.
        /// </summary>
        [JsonProperty]
        public int PartLength { get; private set; }

        /// <summary>
        /// Inclusive part range (e.g., "1-4" = characters 1 through 4).
        /// </summary>
        [Ignore]
        public string PartRangeText =>
            $"{(PartStart + 1) + "-" + (PartStart + PartLength)}";

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="partStart">Part start, within verse (1-based).</param>
        /// <param name="partLength">Part length, within verse.</param>
        /// <param name="partContext">Text context of part (e.g., main text, footnote or reference).</param>
        public PartLocation(int partStart, int partLength, PartContext partContext)
        {
            PartStart = partStart;
            PartLength = partLength;
            PartContext = partContext;
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private PartLocation()
        {
        }

        /// <summary>
        /// Typed equality method.
        /// </summary>
        /// <param name="other">Other part location (required).</param>
        /// <returns>True if equal, false otherwise</returns>
        protected bool Equals(PartLocation other)
        {
            return PartStart == other.PartStart
                   && PartLength == other.PartLength
                   && PartContext == other.PartContext;
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
            return Equals((PartLocation)obj);
        }

        /// <summary>
        /// Standard hash code method.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = PartStart;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ PartLength;
                hashCode = (hashCode * MainConsts.HASH_PRIME) ^ (int)PartContext;
                return hashCode;
            }
        }

        /// <summary>
        /// Typed equality operator.
        /// </summary>
        /// <param name="left">Left part location.</param>
        /// <param name="right">Right part location.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator ==(PartLocation left, PartLocation right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Standard equality operator.
        /// </summary>
        /// <param name="left">Left object.</param>
        /// <param name="right">Right object.</param>
        /// <returns>True if equal, false otherwise.</returns>
        public static bool operator !=(PartLocation left, PartLocation right)
        {
            return !Equals(left, right);
        }
    }
}
