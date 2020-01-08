using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Result;

namespace TvpMain.Filter
{
    /// <summary>
    /// Abstract parts of a general-purpose text filter, managing filtration with case sensitive/insenstive words and phrases.
    /// 
    /// Enables word-only checks using sets (much better performance), even with phrase matches and 
    /// mixed case sensitive/insenstive checks without repetitive (e.g.) ToLower() calls.
    /// </summary>
    public abstract class AbstractTextFilter : ITextFilter
    {
        /// <summary>
        /// Set of case-sensitive words to check against.
        /// </summary>
        public abstract ISet<string> CaseSensitiveWords { get; }

        /// <summary>
        /// Set of case-insensitive (lowercase) words to check against.
        /// </summary>
        public abstract ISet<string> CaseInsensitiveWords { get; }

        /// <summary>
        /// List of case-sensitive phrases to check against.
        /// </summary>
        public abstract IList<string> CaseSensitivePhrases { get; }

        /// <summary>
        /// List of case-insensitive phrases to check against.
        /// </summary>
        public abstract IList<string> CaseInsensitivePhrases { get; }

        /// <summary>
        /// Main filter implementation.
        /// </summary>
        /// <param name="inputText">Text to be checked for filtering (required).</param>
        /// <returns>True if filter matches, false otherwise.</returns>
        public bool FilterText(string inputText)
        {
            inputText = inputText.Trim();

            var caseSensitiveWords = CaseSensitiveWords;
            var caseInsensitiveWords = CaseInsensitiveWords;

            // for words checks: break input text into words, check each word against 
            // case sensitive/insenstive sets.
            if (caseSensitiveWords.Count > 0
                || caseInsensitiveWords.Count > 0)
            {
                var textParts = inputText.Split(null)
                    .Select(partItem => partItem.Trim())
                    .Where(partItem => !partItem.StartsWith("\\"));

                if (caseSensitiveWords.Count > 0
                    && textParts.Any(textItem => caseSensitiveWords.Contains(textItem)))
                {
                    return true;
                }

                if (caseInsensitiveWords.Count > 0
                    && textParts.Any(partItem => caseInsensitiveWords.Contains(partItem.ToLower())))
                {
                    return true;
                }
            }

            // for phrases: iterate case sensitive/insenstive phrases and check if
            // within (entire) input text. Input text obviously isn't a phrase if 
            // it doesn't contain whitespace.
            if (inputText.Any(char.IsWhiteSpace))
            {
                var caseSensitivePhrases = CaseSensitivePhrases;
                var caseInsensitivePhrases = CaseInsensitivePhrases;

                if (caseSensitivePhrases.Count > 0
                    || caseInsensitivePhrases.Count > 0)
                {
                    if (caseSensitivePhrases.Count > 0
                        && caseSensitivePhrases.Any(phraseItem => inputText.Contains(phraseItem)))
                    {
                        return true;
                    }

                    if (caseInsensitivePhrases.Count > 0)
                    {
                        var lowerVerseText = inputText.ToLower();
                        if (caseInsensitivePhrases.Any(phraseItem => lowerVerseText.Contains(phraseItem)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// True if no constituent sets/lists have contents, false otherwise.
        /// </summary>
        public bool IsEmpty =>
            (CaseSensitiveWords.Count < 1
             && CaseInsensitiveWords.Count < 1
             && CaseSensitivePhrases.Count < 1
             && CaseInsensitivePhrases.Count < 1);
    }
}
