using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Filter
{
    public abstract class AbstractTextFilter : ITextFilter
    {
        protected abstract ISet<string> CaseSensitiveWords { get; }
        protected abstract ISet<string> CaseInsensitiveWords { get; }
        protected abstract IList<string> CaseSensitivePhrases { get; }
        protected abstract IList<string> CaseInsensitivePhrases { get; }

        public bool FilterText(int bookNum, int chapterNum, int verseNum, string inputText)
        {
            ISet<string> caseSensitiveWords = CaseSensitiveWords;
            ISet<string> caseInsensitiveWords = CaseInsensitiveWords;

            if (caseSensitiveWords.Count > 0
                || caseInsensitiveWords.Count > 0)
            {
                IEnumerable<string> verseParts = inputText.Split(null)
                    .Select(partItem => partItem.Trim())
                    .Where(partItem => !partItem.StartsWith("\\"));

                if (caseSensitiveWords.Count > 0
                    && verseParts.Any(partItem => caseSensitiveWords.Contains(partItem)))
                {
                    return true;
                }

                if (caseInsensitiveWords.Count > 0
                    && verseParts.Any(partItem => caseInsensitiveWords.Contains(partItem.ToLower())))
                {
                    return true;
                }
            }

            IList<string> caseSensitivePhrases = CaseSensitivePhrases;
            IList<string> caseInsensitivePhrases = CaseSensitivePhrases;

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

            return false;
        }
    }
}
