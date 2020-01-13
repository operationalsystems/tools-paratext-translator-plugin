using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Result;

namespace TvpMain.Filter
{
    /// <summary>
    /// Text filter for biblical terms.
    /// </summary>
    public class KeyTermsTextFilter : AbstractTextFilter
    {
        /// <summary>
        /// Case-sensitive word set.
        /// </summary>
        private readonly HashSet<string> _caseSensitiveWords;

        /// <summary>
        /// Case-insensitive word set.
        /// </summary>
        private readonly HashSet<string> _caseInsensitiveWords;

        /// <summary>
        /// Case-sensitive phrase list.
        /// </summary>
        private readonly List<string> _caseSensitivePhrases;

        /// <summary>
        /// Case-insensitive phrase list.
        /// </summary>
        private readonly List<string> _caseInsensitivePhrases;

        /// <summary>
        /// Case-sensitive word set.
        /// </summary>
        public override ISet<string> CaseSensitiveWords => _caseSensitiveWords;

        /// <summary>
        /// Case-insensitive word set.
        /// </summary>
        public override ISet<string> CaseInsensitiveWords => _caseInsensitiveWords;

        /// <summary>
        /// Case-insensitive phrase list.
        /// </summary>
        public override IList<string> CaseSensitivePhrases => _caseSensitivePhrases;

        /// <summary>
        /// Case-sensitive word set.
        /// </summary>
        public override IList<string> CaseInsensitivePhrases => _caseInsensitivePhrases;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public KeyTermsTextFilter()
        {
            _caseSensitiveWords = new HashSet<string>();
            _caseInsensitiveWords = new HashSet<string>();
            _caseSensitivePhrases = new List<string>();
            _caseInsensitivePhrases = new List<string>();
        }

        /// <summary>
        /// Sets up filter with biblical terms, dividing into words and phrases according to whitespace.
        /// 
        /// Assumes all terms are case-sensitive.
        /// </summary>
        /// <param name="inputItems">Input terms (required).</param>
        public void SetKeyTerms(IList<IKeyTerm> inputItems)
        {
            _caseSensitiveWords.Clear();
            _caseInsensitiveWords.Clear();
            _caseSensitivePhrases.Clear();
            _caseInsensitivePhrases.Clear();

            ISet<string> caseSensitivePhraseSet = new HashSet<string>();
            foreach (var listItem in inputItems)
            {
                var termText = listItem.Term.Trim();
                if (termText.Any(char.IsWhiteSpace))
                {
                    caseSensitivePhraseSet.Add(termText);
                }
                else
                {
                    _caseSensitiveWords.Add(termText);
                }
            }

            _caseSensitivePhrases.AddRange(caseSensitivePhraseSet);
        }
    }
}
