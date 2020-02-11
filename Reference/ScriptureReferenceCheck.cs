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
        /// Content tags (i.e., in the entire part) that we should ensure aren't in the wrong context.
        /// </summary>
        private static readonly IDictionary<PartContext, ISet<string>> ContextContentTags
            = new Dictionary<PartContext, ISet<string>>()
            {
                {
                    PartContext.NoteOrReference,
                    new HashSet<string>()
                    {
                        "fr", "ft"
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

            result = CheckForIncorrectContentTags(
                         inputPart, matchText,
                         matchStart, parsedReference,
                         outputResults) || result;

            result = CheckForIncorrectReferenceTags(
                         inputPart, matchText,
                         matchStart, parsedReference,
                         outputResults) || result;

            result = CheckForMissingOrMalformedTags(
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
        private static bool CheckForBadReference(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            // check for unknown books
            var unknownBooks = parsedReference.ScriptureReference.BookReferences
                    .Where(referenceItem => !referenceItem.IsLocalReference)
                    .Select(referenceItem => referenceItem.BookReferenceName)
                    .Where(rangeItem => !rangeItem.IsKnownBook)
                    .Select(rangeItem => rangeItem.NameText)
                    .Distinct()
                    .ToImmutableList();
            if (!unknownBooks.Any())
            {
                return false;
            }

            var unknownBooksText = string.Join(", ", unknownBooks);
            var nameLabel = unknownBooks.SingularOrPlural("name", "names");
            outputResults.Add(new ResultItem(inputPart,
                $"Invalid book {nameLabel} at position {matchStart}: {unknownBooksText}.",
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
        /// Check for incorrect content tags (e.g., \fr outside of a footnote area).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private static bool CheckForIncorrectContentTags(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            var badTags = FindContentTags(
                    inputPart.PartText, AllContextContentTags)
                .ToHashSet();
            if (badTags.Count < 1)
            {
                return false;
            }
            if (ContextContentTags.TryGetValue(inputPart.PartLocation.PartContext, out var goodTags))
            {
                badTags.RemoveWhere(tagName =>
                    goodTags.Contains(tagName));
                if (badTags.Count < 1)
                {
                    return false;
                }
            }

            var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
            var tagList = badTags.Select(tagItem => string.Concat(@"\", tagItem))
                .ToNiceList(",", "and");
            var tagLabel = badTags.SingularOrPlural("tag", "tags");
            outputResults.Add(new ResultItem(inputPart,
                $@"Incorrect use of {tagList} {tagLabel} in {contextTitle} text at position {matchStart}.",
                matchText, matchStart,
                null, CheckType.ScriptureReference,
                (int)ScriptureReferenceErrorType.IncorrectTag));

            return true;
        }

        /// <summary>
        /// Check for incorrect paired tags (e.g., \ior outside of an outline area).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private static bool CheckForIncorrectReferenceTags(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            // deal with missing or malformed tags elsewhere
            if (!parsedReference.IsOpeningTag
                && !parsedReference.IsClosingTag)
            {
                return false;
            }
            // get good tags for this context
            if (!ContextPairedTags.TryGetValue(inputPart.PartLocation.PartContext, out var goodTags)
                || goodTags.Count < 1)
            {
                return false;
            }
            // missing tags or matches = no error
            if ((!parsedReference.IsOpeningTag || goodTags.Contains(parsedReference.OpeningTag))
                && (!parsedReference.IsClosingTag || goodTags.Contains(parsedReference.ClosingTag)))
            {
                return false;
            }
            // otherwise, build message
            var badTags = new HashSet<string>();
            if (parsedReference.IsOpeningTag)
            {
                badTags.Add(parsedReference.OpeningTag);
            }
            if (parsedReference.IsClosingTag)
            {
                badTags.Add(parsedReference.ClosingTag);
            }

            var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
            var tagList1 = badTags.Select(tagItem => string.Concat(@"\", tagItem))
                .ToNiceList(",", "and");
            var tagLabel1 = badTags.SingularOrPlural("tag", "tags");
            var tagList2 = goodTags.Select(tagItem => string.Concat(@"\", tagItem))
                .ToNiceList(",", "or");
            outputResults.Add(new ResultItem(inputPart,
                $@"Incorrect use of {tagList1} {tagLabel1} in {contextTitle} text at position {matchStart} (expecting paired {tagList2} tags).",
                matchText, matchStart,
                null, CheckType.ScriptureReference,
                (int)ScriptureReferenceErrorType.IncorrectTag));

            return true;
        }

        /// <summary>
        /// Check for missing or malformed tags (e.g., \fp only at the beginning).
        /// </summary>
        /// <param name="inputPart">Input verse part (required).</param>
        /// <param name="matchText">Input text (required).</param>
        /// <param name="matchStart">Input text position, from verse start (0-based).</param>
        /// <param name="parsedReference">Input scripture reference (required).</param>
        /// <param name="outputResults">Output result destination (required).</param>
        /// <returns>True if results added, false otherwise.</returns>
        private static bool CheckForMissingOrMalformedTags(
            VersePart inputPart,
            string matchText,
            int matchStart,
            ScriptureReferenceWrapper parsedReference,
            ICollection<ResultItem> outputResults)
        {
            // get good tags for this context
            if (!ContextPairedTags.TryGetValue(inputPart.PartLocation.PartContext, out var goodTags)
                || goodTags.Count < 1)
            {
                return false;
            }
            // mismatched (malformed)
            if (!Equals(parsedReference.OpeningTag,
                parsedReference.ClosingTag))
            {
                var badTags = new HashSet<string>();
                if (parsedReference.IsOpeningTag)
                {
                    badTags.Add(parsedReference.OpeningTag);
                }
                if (parsedReference.IsClosingTag)
                {
                    badTags.Add(parsedReference.ClosingTag);
                }

                var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
                var tagList1 = badTags.Select(tagItem => string.Concat(@"\", tagItem))
                    .ToNiceList(",", "and");
                var tagLabel1 = badTags.SingularOrPlural("tag", "tags");
                var tagList2 = goodTags.Select(tagItem => string.Concat(@"\", tagItem))
                    .ToNiceList(",", "or");
                outputResults.Add(new ResultItem(inputPart,
                    $@"Malformed {tagList1} {tagLabel1} in {contextTitle} text at position {matchStart} (expecting paired {tagList2} tags).",
                    matchText, matchStart,
                    null, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.MalformedTag));
            }

            // we have missing (or) wrong but equal tags
            if ((!parsedReference.IsOpeningTag || !goodTags.Contains(parsedReference.OpeningTag))
                && (!parsedReference.IsClosingTag || !goodTags.Contains(parsedReference.ClosingTag)))
            {
                // missing tags
                if (IgnoreMissingPairedTagsContexts.Contains(inputPart.PartLocation.PartContext))
                {
                    return false;
                }

                var contextTitle = ContextTitles[inputPart.PartLocation.PartContext];
                var tagList = goodTags.Select(tagItem => string.Concat(@"\", tagItem))
                    .ToNiceList(",", "or");
                var tagLabel = goodTags.SingularOrPlural("tag", "tags");
                outputResults.Add(new ResultItem(inputPart,
                    $@"Missing {tagList} {tagLabel} in {contextTitle} text at position {matchStart}.",
                    matchText, matchStart,
                    null, CheckType.ScriptureReference,
                    (int)ScriptureReferenceErrorType.MissingTag));
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
    }
}
