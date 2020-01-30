using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using TvpMain.Check;
using TvpMain.Project;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Reference
{
    /// <summary>
    /// Scripture reference check.
    /// </summary>
    public class ScriptureReferenceCheck : ITextCheck
    {
        /// <summary>
        /// All part contexts, for iteration.
        /// </summary>
        private static readonly IEnumerable<PartContext> AllContexts
            = Enum.GetValues(typeof(PartContext)).Cast<PartContext>();

        /// <summary>
        /// Text titles for different part contexts.
        /// </summary>
        private static readonly IDictionary<PartContext, string> ContextTitles
            = new Dictionary<PartContext, string>()
            {
                {PartContext.Introductions, "introduction"},
                {PartContext.Outlines, "outline"},
                {PartContext.MainText, "main"},
                {PartContext.NoteOrReference, "note or reference"}
            }.ToImmutableDictionary();

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

        /// <inheritdoc />
        public CheckType CheckType => CheckType.ScriptureReference;

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="versePart">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(
            VersePart versePart,
            ICollection<ResultItem> checkResults)
        {
            // keep track of identified matches so we don't hit them again
            var matchedParts = new HashSet<Tuple<int, int>>();
            foreach (var regexItem in _projectManager.TargetReferenceRegexes)
            {
                foreach (Match matchItem in regexItem.Matches(versePart.PartText))
                {
                    if (matchedParts.Add(new Tuple<int, int>(
                        versePart.PartLocation.PartStart,
                        versePart.PartLocation.PartLength)))
                    {
                        CheckVersePart(versePart, matchItem, checkResults);
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
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckVersePart(
            VersePart inputPart,
            Capture inputMatch,
            ICollection<ResultItem> outputResults)
        {
            var result = false;
            var matchText = inputMatch.Value.Trim();
            var matchStart = inputPart.PartLocation.PartStart + inputMatch.Index;

            // check we can parse (loose match)
            if (_referenceBuilder.TryParseScriptureReference(
                matchText, out var foundWrapper))
            {
                result = CheckFoundReference(
                    inputPart, matchText,
                    matchStart, foundWrapper,
                    outputResults);
            }
            else
            {
                result = true;
                outputResults.Add(new ResultItem(inputPart,
                    $"Invalid reference at position {matchStart} (can't parse).",
                    matchText, matchStart, null,
                    CheckType.ScriptureReference, (int)ScriptureReferenceErrorType.MalformedTag));
            }

            return result;
        }

        /// <summary>
        /// Check a parsed reference for correctness.
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="inputStart">Input text position, from verse start (0-based).</param>
        /// <param name="inputReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckFoundReference(
            VersePart inputPart,
            string inputText,
            int inputStart,
            ScriptureReferenceWrapper inputReference,
            ICollection<ResultItem> outputResults)
        {
            // check for unknown books
            var result = false;
            var unknownBooks = string.Join(", ",
                inputReference.ScriptureReference.BookReferences
                    .Where(referenceItem => !referenceItem.IsLocalReference)
                    .Select(referenceItem => referenceItem.BookReferenceName)
                    .Where(rangeItem => !rangeItem.IsKnownBook)
                    .Select(rangeItem => rangeItem.NameText));
            if (!string.IsNullOrWhiteSpace(unknownBooks))
            {
                result = true;
                outputResults.Add(new ResultItem(inputPart,
                    $"Invalid book name(s) at position {inputStart}: {unknownBooks}.",
                    inputText, inputStart,
                    null, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.BadReference));
            }

            // check format (tight match)
            var standardFormat = _referenceBuilder.FormatStandardReference(
                inputPart.PartLocation.PartContext,
                inputReference);

            // Same as standard = done
            if (!inputText.Equals(standardFormat))
            {
                // check whether only difference is spacing
                string messageText = null;
                var typeCode = 0;

                // check for spacing-only miss
                if (inputText.EqualsIgnoringWhitespace(standardFormat))
                {
                    messageText = $"Non-standard reference spacing at position {inputStart}.";
                    typeCode = (int)ScriptureReferenceErrorType.LooseFormatting;
                }
                else
                {
                    // check other possible styles for this reference
                    var otherFormats =
                        AllContexts.Where(contextItem =>
                            contextItem != inputPart.PartLocation.PartContext)
                        .Select(contextItem =>
                            _referenceBuilder.FormatStandardReference(
                                contextItem,
                                inputReference))
                        .Select(formatItem =>
                            formatItem.RemoveWhitespace())
                        .ToImmutableHashSet();

                    // check with whitespace removed, as wrong name type
                    // is more important than a whitespace miss
                    var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
                    if (otherFormats.Contains(inputText.RemoveWhitespace()))
                    {
                        messageText = $"Non-standard book name style for {contextTitle} text at position {inputStart}.";
                        typeCode = (int)ScriptureReferenceErrorType.IncorrectNameStyle;
                    }
                    else
                    {
                        messageText = $"Non-standard reference content for {contextTitle} text at position {inputStart}.";
                        typeCode = (int)ScriptureReferenceErrorType.BadReference;
                    }
                }

                result = true;
                outputResults.Add(new ResultItem(inputPart, messageText,
                    inputText, inputStart,
                    standardFormat, CheckType.ScriptureReference,
                    typeCode));
            }
            return result;
        }
    }
}
