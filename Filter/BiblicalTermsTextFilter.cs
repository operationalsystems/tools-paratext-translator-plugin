using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    public class BiblicalTermsTextFilter : AbstractTextFilter
    {
        private readonly HashSet<String> _caseSensitiveWords;
        private readonly HashSet<String> _caseInsensitiveWords;
        private readonly List<String> _caseSensitivePhrases;
        private readonly List<String> _caseInsensitivePhrases;

        protected override ISet<string> CaseSensitiveWords => _caseSensitiveWords;

        protected override ISet<string> CaseInsensitiveWords => _caseInsensitiveWords;

        protected override IList<string> CaseSensitivePhrases => _caseSensitivePhrases;

        protected override IList<string> CaseInsensitivePhrases => _caseInsensitivePhrases;

        public BiblicalTermsTextFilter()
        {
            _caseSensitiveWords = new HashSet<string>();
            _caseInsensitiveWords = new HashSet<string>();
            _caseSensitivePhrases = new List<string>();
            _caseInsensitivePhrases = new List<string>();
        }

        public void SetKeyTerms(IList<IKeyTerm> inputItems)
        {
            _caseSensitiveWords.Clear();
            _caseInsensitiveWords.Clear();
            _caseSensitivePhrases.Clear();
            _caseInsensitivePhrases.Clear();

            foreach (IKeyTerm listItem in inputItems)
            {
                string termText = listItem.Term.Trim();
                if (termText.Any(Char.IsWhiteSpace))
                {
                    _caseSensitivePhrases.Add(termText);
                }
                else
                {
                    _caseSensitiveWords.Add(termText);
                }
            }
        }
    }
}
