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
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(
            VersePart partData,
            ICollection<ResultItem> checkResults)
        {
            foreach (Match matchItem in CheckRegex.Matches(partData.PartText))
            {
                checkResults.Add
                (new ResultItem(partData,
                    $"Punctuation check failure at position {matchItem.Index}.",
                    matchItem.Value, matchItem.Index, null,
                    CheckType.MissingSentencePunctuation,
                    (int)PunctuationErrorType.MissingSentenceEnd));
            }
        }
    }
}
