/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TvpMain.Check;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Punctuation
{
    /// <summary>
    /// Regex-based punctuation check.
    /// </summary>
    public class MissingSentencePunctuationCheck : ITextCheck
    {
        /// <summary>
        /// Regex to check for improper capitalization following non-final punctuation (E.g. <text>, And <text>).
        /// </summary>
        private static readonly Regex CheckRegex =
            new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager _projectManager;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public MissingSentencePunctuationCheck(ProjectManager projectManager)
        {
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
        }

        /// <inheritdoc />
        public CheckType CheckType => CheckType.MissingSentencePunctuation;

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="versePart">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(
            VersePart versePart,
            ICollection<ResultItem> checkResults)
        {
            foreach (Match matchItem in CheckRegex.Matches(versePart.PartText))
            {
                checkResults.Add
                (new ResultItem(versePart,
                    $"Punctuation check failure.",
                    matchItem.Value, matchItem.Index, null,
                    CheckType.MissingSentencePunctuation,
                    (int)PunctuationErrorType.MissingSentenceEnd));
            }
        }
    }
}
