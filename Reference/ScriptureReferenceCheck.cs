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
        /// Tags exclusive to specific context content.
        /// </summary>
        private static readonly IDictionary<PartContext, ISet<string>> ContextContentTags
            = new Dictionary<PartContext, ISet<string>>()
            {
                {
                    PartContext.Introductions,
                    new HashSet<string>()
                    {
                        "xt"
                    }.ToImmutableHashSet()
                },
                {
                    PartContext.Outlines,
                    new HashSet<string>()
                    {
                        "ior"
                    }.ToImmutableHashSet()
                },
                {
                    PartContext.MainText,
                    new HashSet<string>()
                    {
                        "xt"
                    }.ToImmutableHashSet()
                },
                {
                    PartContext.NoteOrReference,
                    new HashSet<string>()
                    {
                        "fr", "ft", "xt", "+xt"
                    }.ToImmutableHashSet()
                }
            }.ToImmutableDictionary();

        /// <summary>
        /// All distinct tags exclusive to any context content.
        /// </summary>
        private static readonly ISet<string> AllContextContentTags =
            ContextContentTags.Values.SelectMany(listItem => listItem)
                .ToImmutableHashSet();

        /// <summary>
        /// Paired tags exclusive to specific contexts.
        /// </summary>
        private static readonly IDictionary<PartContext, ISet<string>> ContextPairedTags
            = new Dictionary<PartContext, ISet<string>>()
            {
                {
                    PartContext.Introductions,
                    new HashSet<string>()
                    {
                        "xt"
                    }
                    .ToImmutableHashSet()
                },
                {
                    PartContext.Outlines,
                    new HashSet<string>()
                    {
                        "ior"
                    }.ToImmutableHashSet()
                },
                {
                    PartContext.MainText,
                    new HashSet<string>()
                    {
                        "xt"
                    }
                    .ToImmutableHashSet()
                },
                {
                    PartContext.NoteOrReference,
                    new HashSet<string>()
                    {
                        "xt", "+xt"
                    }.ToImmutableHashSet()
                }
            }.ToImmutableDictionary();

        /// <summary>
        /// All distinct tags paired tags paired in any contexts.
        /// </summary>
        private static readonly ISet<string> AllContextPairedTags =
            ContextPairedTags.Values.SelectMany(listItem => listItem)
                .ToImmutableHashSet();

        /// <summary>
        /// Contexts to ignore missing paired tags.
        /// </summary>
        private static readonly ISet<PartContext> IgnoreMissingPairedTagsContexts =
            new HashSet<PartContext>()
            {
                PartContext.Introductions
            }.ToImmutableHashSet();

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
                matchText.TrimWhitespaceAndParenthesis(), out var foundWrapper))
            {
                result = CheckReference(
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
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckReference(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            var result = CheckForBadReference(
                inputPart, matchText,
                matchStart, parsedReference,
                outputResults);

            result = CheckReferenceFormat(
                         inputPart, matchText,
                         matchStart, parsedReference,
                         outputResults) || result;

            result = CheckForIncorrectTag(
                         inputPart, matchText,
                         matchStart, parsedReference,
                         outputResults) || result;

            result = CheckForMalformedOrMissingTag(
                         inputPart, matchText,
                         matchStart, parsedReference,
                         outputResults) || result;

            return result;
        }

        /// <summary>
        /// Check for unknown book names (e.g., mis-spelling).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckForBadReference(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            // check for unknown books
            var unknownBooks = string.Join(", ",
                parsedReference.ScriptureReference.BookReferences
                    .Where(referenceItem => !referenceItem.IsLocalReference)
                    .Select(referenceItem => referenceItem.BookReferenceName)
                    .Where(rangeItem => !rangeItem.IsKnownBook)
                    .Select(rangeItem => rangeItem.NameText));

            if (string.IsNullOrWhiteSpace(unknownBooks))
            {
                return false;
            }

            var nameLabel = unknownBooks.SingularOrPlural("name", "names");
            outputResults.Add(new ResultItem(inputPart,
                $"Invalid book {nameLabel} at position {matchStart}: {unknownBooks}.",
                matchText, matchStart,
                null, CheckType.ScriptureReference,
                (int)ScriptureReferenceErrorType.BadReference));
            return true;
        }

        /// <summary>
        /// Check for incorrect spacing or book naming (e.g., abbreviation vs short).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckReferenceFormat(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            // check format (tight match)
            var standardText = _referenceBuilder.FormatStandardReference(
                inputPart.PartLocation.PartContext,
                parsedReference);

            // Same as standard = done
            if (matchText.Equals(standardText))
            {
                return false;
            }

            // prep for rest of checks
            var matchNormalized1 = matchText.RemoveWhitespace();
            var standardNormalized1 = standardText.RemoveWhitespace();

            // check whether only difference is spacing
            if (matchNormalized1.Equals(standardNormalized1))
            {
                outputResults.Add(new ResultItem(inputPart,
                    $"Non-standard reference spacing at position {matchStart}.",
                    matchText, matchStart,
                    standardText, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.LooseFormatting));
                return true;
            }

            // prep for rest of checks
            var matchNormalized2 = matchNormalized1.ToLower();

            // create all possible styles for this reference
            var allNormalized2 =
                AllContexts.Select(contextItem =>
                        _referenceBuilder.FormatStandardReference(
                            contextItem,
                            parsedReference))
                    .Select(formatItem =>
                        formatItem.RemoveWhitespace().ToLower())
                    .ToImmutableHashSet();

            // check for name style and case miss by looking for
            // normalized, tag-less versions in match
            if (allNormalized2.Contains(matchNormalized2))
            {
                var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
                outputResults.Add(new ResultItem(inputPart,
                    $"Non-standard book name style or casing for {contextTitle} text at position {matchStart}.",
                    matchText, matchStart,
                    standardText, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.IncorrectNameStyle));
                return true;
            }

            // create all possible styles without punctuation or whitespace
            var matchNormalized3 = matchNormalized2.RemovePunctuation();
            var allNormalized3 =
                allNormalized2.Select(formatItem =>
                        formatItem.RemovePunctuation())
                    .ToImmutableHashSet();

            // check for punctuation miss by looking for
            // normalized, tag-less versions in match
            if (!allNormalized3.Contains(matchNormalized3))
            {
                return false;
            }

            outputResults.Add(new ResultItem(inputPart,
                $"Non-standard reference punctuation at position {matchStart}.",
                matchText, matchStart,
                standardText, CheckType.ScriptureReference,
                (int)ScriptureReferenceErrorType.LooseFormatting));
            return true;

        }

        /// <summary>
        /// Check for incorrect tag naming (e.g., \fr outside of a footnote area).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckForIncorrectTag(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
            var badTags = FindContentTags(
                    inputPart.PartText, AllContextContentTags)
                .ToHashSet();
            if (badTags.Count < 1)
            {
                return false;
            }

            var goodTags = ContextContentTags[inputPart.PartLocation.PartContext];
            badTags.RemoveWhere(
                tagName => goodTags.Contains(tagName));
            if (badTags.Count < 1)
            {
                return false;
            }

            var tagList = badTags.Select(tagItem => string.Concat(@"\", tagItem))
                .NiceListOf(",", "and");
            var tagLabel = badTags.SingularOrPlural("tag", "tags");
            outputResults.Add(new ResultItem(inputPart,
                $@"Incorrect use of {tagList} {tagLabel} in {contextTitle} text at position {matchStart}.",
                matchText, matchStart,
                null, CheckType.ScriptureReference,
                (int)ScriptureReferenceErrorType.IncorrectTag));

            return true;
        }

        /// <summary>
        /// Check for malformed tag (e.g., \fp only at the beginning).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private bool CheckForMalformedOrMissingTag(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
            var workText = inputPart.PartText.TrimWhitespaceAndParenthesis();

            var contextTags = ContextPairedTags[inputPart.PartLocation.PartContext];
            if (contextTags.Count < 1)
            {
                return false;
            }

            var startOrEndTag = FindStartOrEndTags(workText, contextTags);
            if (startOrEndTag == null)
            {
                // ignore missing (but not malformed) tags in specific contexts
                if (IgnoreMissingPairedTagsContexts.Contains(inputPart.PartLocation.PartContext))
                {
                    return false;
                }
                else // otherwise, we have a problem
                {
                    var tagList = contextTags.Select(tagItem => string.Concat(@"\", tagItem))
                        .NiceListOf(",", "or");
                    var tagLabel = contextTags.SingularOrPlural("tag", "tags");
                    outputResults.Add(new ResultItem(inputPart,
                        $@"Missing paired {tagList} {tagLabel} in {contextTitle} text at position {matchStart}.",
                        matchText, matchStart,
                        null, CheckType.ScriptureReference,
                        (int)ScriptureReferenceErrorType.MissingTag));
                    return true;
                }
            }

            var startAndEndTag = FindStartAndEndTags(workText, contextTags);
            if (!Equals(startOrEndTag, startAndEndTag))
            {
                var tagList = contextTags.Select(tagItem => string.Concat(@"\", tagItem))
                    .NiceListOf(",", "or");
                var tagLabel = contextTags.SingularOrPlural("tag", "tags");
                outputResults.Add(new ResultItem(inputPart,
                    $@"Malformed \{startOrEndTag} tag in {contextTitle} text at position {matchStart} (expecting paired {tagList} {tagLabel}).",
                    matchText, matchStart,
                    null, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.MalformedTag));
                return true;
            }

            return true;
        }

        /// <summary>
        /// Checks whether supplied text contains any of the supplied tags.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="tagNames">Tag names to search for, without leading backslashes or trailing stars (required).</param>
        /// <returns>True if input starts or ends with any of the tags.</returns>
        private static IList<string> FindContentTags(string inputText, IEnumerable<string> tagNames)
        {
            return tagNames.Where(tagName =>
                inputText.IndexOf($@"\{tagName}", StringComparison.InvariantCultureIgnoreCase) >= 0
                || inputText.IndexOf($@"\{tagName}*", StringComparison.InvariantCultureIgnoreCase) >= 0)
                .ToImmutableList();
        }

        /// <summary>
        /// Finds the first of the supplied tags that begins _and_ ends the input text.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="tagNames">Tag names to search for, without leading backslashes or trailing stars (required).</param>
        /// <returns>True if input starts or ends with any of the tags.</returns>
        private static string FindStartAndEndTags(string inputText, IEnumerable<string> tagNames)
        {
            var bestPosition = -1;
            string bestTag = null;

            foreach (var tagName in tagNames)
            {
                var foundStart = inputText.IndexOf($@"\{tagName}", StringComparison.InvariantCultureIgnoreCase);
                var foundEnd = inputText.IndexOf($@"\{tagName}*", StringComparison.InvariantCultureIgnoreCase);

                if (foundStart >= 0 && foundEnd >= 0 &&
                    foundStart < foundEnd)
                {
                    var foundPosition = Math.Min(foundStart, foundEnd);
                    if (bestPosition == -1
                        || bestPosition < foundPosition)
                    {
                        bestPosition = foundPosition;
                        bestTag = tagName;
                    }
                }
            }

            return bestTag;
        }

        /// <summary>
        /// Finds the first of the supplied tags that begins _or_ ends the input text.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="tagNames">Tag names to search for, without leading backslashes or trailing stars (required).</param>
        /// <returns>True if input starts or ends with any of the tags.</returns>
        private static string FindStartOrEndTags(string inputText, IEnumerable<string> tagNames)
        {
            var bestPosition = -1;
            string bestTag = null;

            foreach (var tagName in tagNames)
            {
                var foundStart = inputText.IndexOf($@"\{tagName}", StringComparison.InvariantCultureIgnoreCase);
                var foundEnd = inputText.IndexOf($@"\{tagName}*", StringComparison.InvariantCultureIgnoreCase);

                var foundPosition = foundStart;
                if (foundStart < 0 && foundEnd >= 0)
                {
                    foundPosition = foundEnd;
                }
                else if (foundEnd < 0 && foundStart >= 0)
                {
                    foundPosition = foundStart;
                }
                else if (foundStart >= 0 && foundEnd >= 0
                         && foundStart < foundEnd)
                {
                    foundPosition = foundStart;
                }

                if (foundPosition >= 0
                    && (bestPosition < 0 || bestPosition < foundPosition))
                {
                    bestPosition = foundPosition;
                    bestTag = tagName;
                }
            }

            return bestTag;
        }

        /// <summary>
        /// Finds the first of the supplied tags that begins _and_ ends the input text.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="tagNames">Tag names to search for, without leading backslashes or trailing stars (required).</param>
        /// <returns>True if input starts or ends with any of the tags.</returns>
        private static string StartsAndEndsWithTags(string inputText, IEnumerable<string> tagNames)
        {
            return tagNames
                .FirstOrDefault(tagName =>
                    inputText.StartsWith($@"\{tagName}",
                        StringComparison.InvariantCultureIgnoreCase)
                    && inputText.EndsWith($@"\{tagName}*",
                        StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
