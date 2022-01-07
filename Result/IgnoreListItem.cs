/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Newtonsoft.Json;
using System;
using System.Linq;

namespace TvpMain.Result
{
    /// <summary>
    /// Model class for ignore list entries.
    /// 
    /// Note "private set" fields enable JSON serialization
    /// while maintaining runtime immutability.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class IgnoreListItem
    {
        /// <summary>
        /// Case-sensitive version of text.
        /// </summary>
        [JsonProperty]
        public string CaseSensitiveText { get; private set; }

        /// <summary>
        /// True if item is meant to be case-insensitive (i.e., ignores case), false otherwise.
        /// </summary>
        [JsonProperty]
        public bool IsIgnoreCase { get; private set; }

        /// <summary>
        /// True if case-sensitive text contains any whitespace.
        /// </summary>
        public bool IsPhrase()
        {
            return CaseSensitiveText.Any(char.IsWhiteSpace);
        }

        /// <summary>
        /// Provide a case-insensitive version of the text (lower case).
        /// </summary>
        public string CaseInsensitiveText()
        {
            return CaseSensitiveText.ToLower();
        }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="itemText">Entry text (required).</param>
        /// <param name="isIgnoreCase">True if item is to be case-insensitive, false otherwise.</param>
        public IgnoreListItem(string itemText, bool isIgnoreCase)
        {
            CaseSensitiveText = itemText ?? throw new ArgumentNullException(nameof(itemText));
            IsIgnoreCase = isIgnoreCase;
        }

        /// <summary>
        /// Private ctor for serialization.
        /// </summary>
        [JsonConstructor]
        private IgnoreListItem()
        {
        }
    }
}
