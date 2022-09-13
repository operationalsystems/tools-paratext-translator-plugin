﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using PtxUtils;

namespace TvpMain.Util
{
    /// <summary>
    /// Standard text utilities and extension methods (string, string builder, and related).
    /// </summary>
    public static class TextUtil
    {
        /// <summary>
        /// Parenthesis chars for trimming.
        /// </summary>
        private static readonly char[] ParenthesisChars
            = "({[]})".ToCharArray();

        /// <summary>
        /// Characters safe for file names (not paths).
        /// </summary>
        private static readonly ISet<char> InvalidFileChars
            = Path.GetInvalidFileNameChars().ToImmutableHashSet();

        /// <summary>
        /// Append to a string builder, adding a space first if
        /// there is already content and the input is non-null, non-empty.
        /// </summary>
        /// <param name="thisBuilder">This string builder (provided).</param>
        /// <param name="inputData">Data to append (optional, may be null).</param>
        public static void AppendWithSpace<T>(this StringBuilder thisBuilder, T inputData)
        {
            var inputText = inputData?.ToString();
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return;
            }
            if (thisBuilder.Length > 0)
            {
                thisBuilder.Append(" ");
            }
            thisBuilder.Append(inputData);
        }

        /// <summary>
        /// Returns string copy with all whitespace removed.
        /// </summary>
        /// <param name="thisText">This string (provided).</param>
        /// <returns>Copy of this string with all whitespace removed.</returns>
        public static string RemoveWhitespace(this string thisText) =>
            string.Concat(thisText.Where(value => !char.IsWhiteSpace(value)));

        /// <summary>
        /// Returns string copy with all punctuation removed.
        /// </summary>
        /// <param name="thisText">This string (provided).</param>
        /// <returns>Copy of this string with all punctuation removed.</returns>
        public static string RemovePunctuation(this string thisText) =>
            string.Concat(thisText.Where(value => !char.IsPunctuation(value)));

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

        /// <summary>
        /// Return singular or plural text, depending on count of items in enumerable.
        /// </summary>
        /// <typeparam name="T">Input type (provided).</typeparam>
        /// <param name="inputItems">Input items (provided).</param>
        /// <param name="singularText">Singular text (required).</param>
        /// <param name="pluralText">Plural text (required).</param>
        public static string SingularOrPlural<T>(
            this IEnumerable<T> inputItems,
            string singularText,
            string pluralText) =>
            inputItems.Count() == 1
                ? singularText
                : pluralText;

        /// <summary>
        /// Provide a "nice" delimited list, with an interior and final delimiter.
        /// </summary>
        /// <typeparam name="T">Input type (provided).</typeparam>
        /// <param name="inputItems">Input items (provided).</param>
        /// <param name="interiorDelimiter">Interior delimiter, without integral spacing (e.g., ",").</param>
        /// <param name="finalDelimiter">Final delimiter, without integral spacing (e.g., "or").</param>
        /// <returns>Delimited list text.</returns>
        public static string ToNiceList<T>(
            this IEnumerable<T> inputItems,
            string interiorDelimiter,
            string finalDelimiter)
        {
            IList<string> inputList = inputItems
                .Select(listItem => listItem.ToString())
                .ToImmutableList();
            return inputList.Count switch
            {
                0 => string.Empty,
                1 => inputList[0],
                2 => string.Concat(inputList[0], " ",
                    finalDelimiter, " ",
                    inputList[1]),
                _ => string.Concat(
                    string.Join(string.Concat(interiorDelimiter, " "),
                        inputList.ToArray(), 0, inputList.Count - 1),
                    interiorDelimiter, " ",
                    finalDelimiter, " ",
                    inputList.LastOrDefault())
            };
        }

        /// <summary>
        /// Trims whitespace, parenthesis, and whitespace within parenthesis.
        /// </summary>
        /// <param name="thisText">This text (provided).</param>
        /// <returns>Trimmed text.</returns>
        public static string TrimWhitespaceAndParenthesis(this string thisText) =>
            thisText.Trim()
                .Trim(ParenthesisChars)
                .Trim();

        /// <summary>
        /// Converts a string to a usable filename.
        ///
        /// Algorithm:
        /// 1. Convert input to title case
        /// 2. Process each character:
        /// 2.a. Letters & numbers = unchanged
        /// 2.b. Whitespace = removed
        /// 2.c. Other = _
        /// </summary>
        /// <param name="thisText">This text (provided).</param>
        /// <returns>Converted text.</returns>
        public static string ConvertToFileName(this string thisText)
        {
            return string.Concat(thisText
                .ConvertToTitleCase()
                .Select(foundChar =>
                    char.IsWhiteSpace(foundChar)
                    ? '\0'
                    : (InvalidFileChars.Contains(foundChar) ? '_' : foundChar))
                .Where(foundChar => foundChar != '\0'));
        }
    }
}
