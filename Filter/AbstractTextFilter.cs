using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    public abstract class AbstractTextFilter : ITextFilter
    {

        protected abstract ISet<string> CaseSensitiveWords { get; }
        protected abstract ISet<string> CaseInsensitiveWords { get; }
        protected abstract IList<string> CaseSensitivePhrases { get; }
        protected abstract IList<string> CaseInsensitivePhrases { get; }

        public bool FilterText(String inputText)
        {
            inputText = inputText.Trim();

            ISet<string> caseSensitiveWords = CaseSensitiveWords;
            ISet<string> caseInsensitiveWords = CaseInsensitiveWords;

            if (caseSensitiveWords.Count > 0
                || caseInsensitiveWords.Count > 0)
            {
                IEnumerable<string> textParts = inputText.Split(null)
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

            if (inputText.Any(Char.IsWhiteSpace))
            {
                IList<string> caseSensitivePhrases = CaseSensitivePhrases;
                IList<string> caseInsensitivePhrases = CaseInsensitivePhrases;

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
                        string lowerVerseText = inputText.ToLower();
                        if (caseInsensitivePhrases.Any(phraseItem => lowerVerseText.Contains(phraseItem)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool IsEmpty
        {
            get
            {
                return (CaseSensitiveWords.Count < 1
                        && CaseInsensitiveWords.Count < 1
                        && CaseSensitivePhrases.Count < 1
                        && CaseInsensitivePhrases.Count < 1);
            }
        }
    }
}
