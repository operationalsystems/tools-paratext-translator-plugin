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
        /// <returns></returns>
        public static bool EqualsIgnoringWhitespace(this string thisText, string otherText) =>
            thisText.Where(value => !char.IsWhiteSpace(value))
                .SequenceEqual(otherText.Where(value => !char.IsWhiteSpace(value)));
    }
}
