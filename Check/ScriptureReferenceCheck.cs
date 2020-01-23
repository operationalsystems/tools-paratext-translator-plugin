using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using TvpMain.Project;
using TvpMain.Reference;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    public class ScriptureReferenceCheck : ITextCheck
    {
        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Scripture reference builder.
        /// </summary>
        private readonly ScriptureReferenceBuilder _referenceBuilder;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="projectManager">Project manager (required).</param>
        public ScriptureReferenceCheck(ProjectManager projectManager)
        {
            _projectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            _referenceBuilder = new ScriptureReferenceBuilder(projectManager);
        }

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(
            VersePart partData,
            ICollection<ResultItem> checkResults)
        {
            // keep track of identified parts so we don't hit them again
            var matchedParts = new HashSet<VersePart>();
            foreach (var regexItem in _projectManager.TargetReferenceRegexes)
            {
                foreach (Match matchItem in regexItem.Matches(partData.PartText))
                {
                    var matchedPart = new VersePart(partData.VerseData,
                        new PartLocation(partData.PartLocation.PartStart + matchItem.Index,
                            matchItem.Length,
                            partData.PartLocation.PartContext),
                        matchItem.Value);
                    if (!matchedParts.Contains(matchedPart)
                        && CheckVersePart(matchedPart, matchItem, checkResults))
                    {
                        matchedParts.Add(matchedPart);
                    }
                }
            }
        }

        /// <summary>
        /// Perform reference checks on a specific verse part identified as
        /// reference text using regexes. 
        /// </summary>
        /// <param name="inputPart">Verse part (required).</param>
        /// <param name="inputMatch">Regex match (required).</param>
        /// <param name="outputResults">Result item list to add checks to.</param>
        /// <returns>True if any result items added, false otherwise.</returns>
        private bool CheckVersePart(VersePart inputPart,
            Capture inputMatch,
            ICollection<ResultItem> outputResults)
        {
            var result = false;

            // check we can parse (loose match)
            var workMatch = inputMatch.Value.Trim();
            if (_referenceBuilder.TryParseScriptureReference(
                workMatch,
                out var foundWrapper))
            {
                // check for unknown books
                var unknownBooks = string.Join(", ",
                foundWrapper.ScriptureReference.BookReferences
                            .Where(referenceItem => !referenceItem.IsLocalReference)
                            .Select(referenceItem => referenceItem.BookReferenceName)
                            .Where(rangeItem => !rangeItem.IsKnownBook)
                            .Select(rangeItem => rangeItem.NameText));
                if (!string.IsNullOrWhiteSpace(unknownBooks))
                {
                    result = true;
                    outputResults.Add(new ResultItem(inputPart,
                        $"Unknown book name(s) at position {inputMatch.Index}: {unknownBooks}.",
                        inputPart.PartText, inputMatch.Value, "Verify reference and re-run checks.",
                        CheckType.ScriptureReference));
                }

                // check format (tight match)
                var standardFormat = _referenceBuilder.FormatStandardReference(
                    inputPart.PartLocation.PartContext,
                    foundWrapper);

                if (!workMatch.Equals(standardFormat))
                {
                    // check whether only difference is spacing
                    var messageText = workMatch.Where(value => !char.IsWhiteSpace(value))
                        .Equals(standardFormat.Where(value => !char.IsWhiteSpace(value)))
                        ? $"Non-standard reference spacing at position {inputMatch.Index}."
                        : $"Non-standard reference content at position {inputMatch.Index}.";

                    result = true;
                    outputResults.Add(new ResultItem(inputPart, messageText,
                        inputPart.PartText, inputMatch.Value, standardFormat,
                        CheckType.ScriptureReference));
                }
            }
            else
            {
                result = true;
                outputResults.Add(new ResultItem(inputPart,
                    $"Invalid reference at position {inputMatch.Index} (can't parse).",
                    inputPart.PartText, inputMatch.Value, null,
                    CheckType.ScriptureReference));
            }

            return result;
        }
    }
}
