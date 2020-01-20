using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TvpMain.Text
{
    /// <summary>
    /// Regex-related utilities.
    /// </summary>
    public class VerseUtil
    {
        /// <summary>
        /// List of note and reference regexes.
        /// </summary>
        private static readonly IList<Regex> NoteOrReferenceRegexes
            = new List<Regex>()
            {
                CreateNoteOrReferenceRegex("f"),
                CreateNoteOrReferenceRegex("x"),
                CreateNoteOrReferenceRegex("ef"),
                CreateNoteOrReferenceRegex("ex"),
                CreateNoteOrReferenceRegex("fe"),
                CreatePairedTagRegex("ior"),
                CreatePairedTagRegex("rq")
            }.ToImmutableList();

        /// <summary>
        /// List of line-oriented intro regexes.
        /// </summary>
        private static readonly IList<Regex> IntroductionRegexes
            = new List<Regex>()
            {
                CreateLineTagGroupRegex(
                    "usfm", "sts", "rem", "h", "toc[0-9]+",
                    "toca[0-9]+", "imt[0-9]+", "is[0-9]+", "ip", "ipi",
                    "im", "imi", "ipq", "imq", "ipr",
                    "iq[0-9]+", "ib", "ili[0-9]+", "iex", "imte[0-9]+",
                    "ie")
            }.ToImmutableList();

        /// <summary>
        /// List of line-oriented TOC regexes.
        /// </summary>
        private static readonly IList<Regex> OutlineRegexes
            = new List<Regex>()
            {
                CreateLineTagGroupRegex(
                    "iot", "io[0-9]+")
            }.ToImmutableList();

        /// <summary>
        /// All regexes that find text we _don't_ want in extracted "main" text,
        /// needed because main scripture extractor will co-mingle everything.
        /// </summary>
        private static readonly IList<Regex> NonMainTextRegexes
            = NoteOrReferenceRegexes
                .Concat(IntroductionRegexes)
                .Concat(OutlineRegexes)
                .ToImmutableList();

        /// <summary>
        /// Context mapping list.
        /// </summary>
        private static readonly IDictionary<PartContext, ContextMappingItem> ContextMappings =
            new List<ContextMappingItem>() {
                new ContextMappingItem(PartContext.MainText, NonMainTextRegexes, true),
                new ContextMappingItem(PartContext.Introductions, IntroductionRegexes, false),
                new ContextMappingItem(PartContext.Outlines, OutlineRegexes, false),
                new ContextMappingItem(PartContext.NoteOrReference, NoteOrReferenceRegexes, false),
            }.ToImmutableDictionary(mappingItem => mappingItem.PartContext);

        /// <summary>
        /// Creates a note or reference regex from a tag name (note extra required interior non-space char).
        ///
        /// Note: This regex is set up to terminate either at a legitimate closing tag or the presence of
        /// another opening tag _without_ capturing that opening in order for checks to work on the correct
        /// range of content.
        ///
        /// This comes from the following in the USFM spec: The individual elements which make up the
        /// footnote or cross reference content are defined as character level markers, which means that they
        /// each define a beginning and a corresponding end marker. The Paratext translation editor will
        /// interpret the presence of a new marker as an implicit closure of any preceding character level
        /// marker. For this reason a majority of translation projects over the years have adopted the
        /// approach of authoring footnote or cross reference content without supplying the explicit end
        /// marker, since it reduces the volume of markup found within the notes.
        ///
        /// https://ubsicap.github.io/usfm/about/syntax.html#endmarkers-in-footnotes-and-cross-references
        /// </summary>
        /// <param name="tagName">Tag name (required).</param>
        /// <returns></returns>
        private static Regex CreateNoteOrReferenceRegex(string tagName)
        {
            return new Regex($@"\\{tagName}\s+[\S]\s+(?:(?:(?!\\{tagName}).)*|\s*)(?:\\{tagName}\*)?",
                RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates tag pair regex from a tag name.
        ///
        /// Note: This regex is set up to terminate either at a legitimate closing tag or the presence of
        /// another opening tag _without_ capturing that opening in order for checks to work on the correct
        /// range of content.
        ///
        /// This comes from the following in the USFM spec: The individual elements which make up the
        /// footnote or cross reference content are defined as character level markers, which means that they
        /// each define a beginning and a corresponding end marker. The Paratext translation editor will
        /// interpret the presence of a new marker as an implicit closure of any preceding character level
        /// marker. For this reason a majority of translation projects over the years have adopted the
        /// approach of authoring footnote or cross reference content without supplying the explicit end
        /// marker, since it reduces the volume of markup found within the notes.
        ///
        /// https://ubsicap.github.io/usfm/about/syntax.html#endmarkers-in-footnotes-and-cross-references
        /// </summary>
        /// <param name="tagName">Tag name (required).</param>
        /// <returns>Compiled, single-line regex.</returns>
        private static Regex CreatePairedTagRegex(string tagName)
        {
            return new Regex($@"\\{tagName}(?:(?:(?!\\{tagName}).)*|\s*)(?:\\{tagName}\*)?",
                RegexOptions.Singleline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates a whole-line regex matching any of a list of tag names.
        /// </summary>
        /// <param name="tagNames">Tag names to match (required).</param>
        /// <returns>Compiled, multi-line regex.</returns>
        private static Regex CreateLineTagGroupRegex(params string[] tagNames)
        {
            return CreateLineTagRegex($"(?:{string.Join("|", tagNames)})");
        }

        /// <summary>
        /// Creates a whole-line regex from a tag name (e.g., titles and tocs).
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns>Compiled, multi-line regex.</returns>
        private static Regex CreateLineTagRegex(string tagName)
        {
            return new Regex($@"^\s*\\{tagName}(?:\s.*)?\r?$",
                RegexOptions.Multiline | RegexOptions.Compiled);
        }

        /// <summary>
        /// Finds parts of main text from co-mingled content.
        /// 
        /// Needed because while scripture extractors returns several contexts co-mingled,
        /// (e.g., main text, notes) we need them separated for context-sensitive checks.
        /// </summary>
        /// <param name="inputVerse">Input verse containing mixed content (required).</param>
        /// <param name="inputContexts">Set of allowable contexts to search for (required).</param>
        /// <param name="outputParts">Map of text contexts to lists of found parts (required).</param>
        /// <returns>True if any parts found, false otherwise.</returns>
        public static bool FindVerseParts(
            ProjectVerse inputVerse,
            ISet<PartContext> inputContexts,
            ICollection<VersePart> outputParts)
        {
            var isFound = false;
            foreach (var contextItem in inputContexts)
            {
                if (ContextMappings.TryGetValue(contextItem, out var mappingItem))
                {
                    isFound = FindContextParts(inputVerse, mappingItem.ContextRegexes,
                                  mappingItem.IsNegative, contextItem, outputParts) || isFound;
                }
            }

            return isFound;
        }

        /// <summary>
        /// Find specific sub-elements within mixed content.
        /// 
        /// Needed because while scripture extractors returns several contexts co-mingled,
        /// (e.g., main text, notes) we need them separated for context-sensitive checks.
        /// </summary>
        /// <param name="inputVerse">Input verse data containing mixed content (required).</param>
        /// <param name="includeRegexes">Regexes to search for (required).</param>
        /// <param name="isNegative">True to find parts _not_ matching regexes, false to find matching.</param>
        /// <param name="outputContext">Context for newly-created parts.</param>
        /// <param name="outputParts">Destination collection for found note and reference parts.</param>
        /// <returns>True if applicable content found, false otherwise.</returns>
        private static bool FindContextParts(
            ProjectVerse inputVerse, IEnumerable<Regex> includeRegexes,
            bool isNegative, PartContext outputContext,
            ICollection<VersePart> outputParts)
        {
            // create mask from text matching regexes
            var inputText = inputVerse.VerseText;
            var workBuilder = new StringBuilder(inputText, inputText.Length);
            var isFound = false;

            foreach (var noteRegex in includeRegexes)
            {
                foreach (Match matchItem in noteRegex.Matches(inputText))
                {
                    for (var ctr = matchItem.Index;
                        ctr < (matchItem.Index + matchItem.Length);
                        ctr++)
                    {
                        workBuilder[ctr] = '\0';
                        isFound = true;
                    }
                }
            }

            // filter out masked or non-masked text (per negative flag)
            // to find result parts
            if (isFound)
            {
                var outputBuilder = new StringBuilder();
                var isNewLine = false;
                var isAdded = false;
                var partStart = 0;

                for (var ctr = 0;
                    ctr < workBuilder.Length;
                    ctr++)
                {
                    // every useful fraction of the mask
                    // delimits a result part, breaking up composite input
                    if ((isNegative && workBuilder[ctr] != '\0')
                        || (!isNegative && workBuilder[ctr] == '\0'))
                    {
                        if (isNewLine)
                        {
                            if (outputBuilder.Length > 0)
                            {
                                var partText = outputBuilder.ToString();
                                outputBuilder.Clear();

                                // empty parts are of no interest...
                                if (!string.IsNullOrWhiteSpace(partText))
                                {
                                    // ...but preserve spacing if non-empty
                                    outputParts.Add(new VersePart(inputVerse,
                                        new PartLocation(partStart, partText.Length, outputContext),
                                        partText));
                                    isAdded = true;
                                }
                            }

                            partStart = ctr;
                            isNewLine = false;
                        }

                        outputBuilder.Append(inputText[ctr]);
                    }
                    else
                    {
                        isNewLine = true;
                    }
                }

                // handle any last part
                if (outputBuilder.Length > 0)
                {
                    var partText = outputBuilder.ToString();
                    outputBuilder.Clear();

                    // empty parts are of no interest...
                    if (!string.IsNullOrWhiteSpace(partText))
                    {
                        // ...but preserve spacing if non-empty
                        outputParts.Add(new VersePart(inputVerse,
                            new PartLocation(partStart, partText.Length, outputContext),
                            partText));
                        isAdded = true;
                    }
                }

                return isAdded;
            }
            else // entire input is masked
            {
                if (isNegative)
                {
                    outputParts.Add(new VersePart(inputVerse,
                        new PartLocation(0, inputText.Length, outputContext),
                        inputText));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Context mapping item.
        /// </summary>
        private class ContextMappingItem
        {
            /// <summary>
            /// Text context (e.g., main text).
            /// </summary>
            public PartContext PartContext { get; }

            /// <summary>
            /// Regexes identifying context.
            /// </summary>
            public IList<Regex> ContextRegexes { get; }

            /// <summary>
            /// True if negative match, false if positive.
            /// </summary>
            public bool IsNegative { get; }

            /// <summary>
            /// Basic ctor.
            /// </summary>
            /// <param name="partContext">Text context (e.g., main text).</param>
            /// <param name="contextRegexes">Regexes identifying context (required).</param>
            /// <param name="isNegative">True if negative match, false if positive.</param>
            public ContextMappingItem(
               PartContext partContext,
               IList<Regex> contextRegexes,
               bool isNegative)
            {
                PartContext = partContext;
                ContextRegexes = contextRegexes ?? throw new ArgumentNullException(nameof(contextRegexes));
                this.IsNegative = isNegative;
            }
        }
    }
}
