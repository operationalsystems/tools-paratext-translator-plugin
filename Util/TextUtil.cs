using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Util
{
    /// <summary>
    /// Standard text utilities and extension methods (string, string builder, and related).
    /// </summary>
    public static class TextUtil
    {
        /// <summary>
        /// Append to a string builder, adding a space first if
        /// there is already content and the input is non-null, non-empty.
        /// </summary>
        /// <param name="thisBuilder">This string builder (provided).</param>
        /// <param name="inputData">Data to append (optional, may be null).</param>
        public static void AppendWithSpace<T>(this StringBuilder thisBuilder, T inputData)
        {
            var inputText = inputData?.ToString();
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                if (thisBuilder.Length > 0)
                {
                    thisBuilder.Append(" ");
                }

                thisBuilder.Append(inputData);
            }
        }

        /// <summary>
        /// Indicates whether this and another string are equivalent,
        /// excluding whitespace.
        /// </summary>
        /// <param name="thisText">This string (provided).</param>
        /// <param name="otherText">String to compare (required).</param>
        /// <returns>True if strings are equal excluding whitespace, false otherwise.</returns>
        public static bool EqualsIgnoringWhitespace(this string thisText, string otherText) =>
            thisText.Where(value => !char.IsWhiteSpace(value))
                .SequenceEqual(otherText.Where(value => !char.IsWhiteSpace(value)));

        /// <summary>
        /// Returns string copy with all whitespace removed.
        /// </summary>
        /// <param name="thisText">This string (provided).</param>
        /// <returns>Copy of this string with all whitespace removed.</returns>
        public static string RemoveWhitespace(this string thisText) =>
            string.Concat(thisText.Where(value => !char.IsWhiteSpace(value)));

        /// <summary>
        /// Find the lowest index (position) of any supplied items,
        /// optionally trimming input and ignoring case.
        /// </summary>
        /// <typeparam name="T">Input type (provided).</typeparam>
        /// <param name="thisText">This text (provided).</param>
        /// <param name="isIgnoreCase">True to ignore case, false otherwise.</param>
        /// <param name="isTrimInput">True to trim input, false otherwise.</param>
        /// <param name="inputItems">Input items to evaluate.</param>
        /// <returns>Lowest index (position) of any supplied items if found, -1 otherwise.</returns>
        public static int MinIndexOf<T>(
            this string thisText,
            bool isIgnoreCase,
            bool isTrimInput,
            IEnumerable<T> inputItems) =>
            inputItems
                .Select(inputItem => isTrimInput
                    ? inputItem.ToString().Trim()
                    : inputItem.ToString())
                .Select(inputItem => thisText.IndexOf(inputItem, isIgnoreCase
                        ? StringComparison.InvariantCultureIgnoreCase
                        : StringComparison.InvariantCulture))
                .Where(inputIndex => inputIndex >= 0) // exclude missing items
                .DefaultIfEmpty(-1) // default to -1, if nothing found
                .Min(); // find lowest, regardless
    }
}
