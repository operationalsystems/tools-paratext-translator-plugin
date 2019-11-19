using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    public class IgnoreListTextFilter : AbstractTextFilter
    {
        private readonly HashSet<String> _caseSensitiveWords;
        private readonly HashSet<String> _caseInsensitiveWords;
        private readonly List<String> _caseSensitivePhrases;
        private readonly List<String> _caseInsensitivePhrases;

        protected override ISet<string> CaseSensitiveWords => _caseSensitiveWords;

        protected override ISet<string> CaseInsensitiveWords => _caseInsensitiveWords;

        protected override IList<string> CaseSensitivePhrases => _caseSensitivePhrases;

        protected override IList<string> CaseInsensitivePhrases => _caseInsensitivePhrases;

        public IgnoreListTextFilter()
        {
            _caseSensitiveWords = new HashSet<string>();
            _caseInsensitiveWords = new HashSet<string>();
            _caseSensitivePhrases = new List<string>();
            _caseInsensitivePhrases = new List<string>();
        }

        public void SetIgnoreListItems(IList<IgnoreListItem> inputItems)
        {
            _caseSensitiveWords.Clear();
            _caseInsensitiveWords.Clear();
            _caseSensitivePhrases.Clear();
            _caseInsensitivePhrases.Clear();

            foreach (IgnoreListItem listItem in inputItems)
            {
                if (listItem.IsPhrase)
                {
                    if (listItem.IsIgnoreCase)
                    {
                        _caseInsensitivePhrases.Add(listItem.CaseInsensitiveItemText);
                    }
                    else
                    {
                        _caseSensitivePhrases.Add(listItem.CaseSensitiveItemText);
                    }
                }
                else
                {
                    if (listItem.IsIgnoreCase)
                    {
                        _caseInsensitiveWords.Add(listItem.CaseInsensitiveItemText);
                    }
                    else
                    {
                        _caseSensitiveWords.Add(listItem.CaseSensitiveItemText);
                    }
                }
            }
        }
    }
}
