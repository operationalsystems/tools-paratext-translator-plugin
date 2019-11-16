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
    }
}
