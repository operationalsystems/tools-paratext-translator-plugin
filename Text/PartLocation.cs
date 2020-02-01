using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace TvpMain.Text
{
    /// <summary>
    /// Location of a specific part of a verse.
    ///
    /// Note "private set" fields support JSON serialization
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
                hashCode = (hashCode * 397) ^ PartLength;
                hashCode = (hashCode * 397) ^ (int)PartContext;
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
