using System.Collections.Generic;
using TvpMain.Result;

namespace TvpMain.Filter
{
    /// <summary>
    /// Text filter for an ignore list.
    /// </summary>
    public class IgnoreListTextFilter : AbstractTextFilter
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
        /// Case-sensitive phrase list.
        /// </summary>
        public override IList<string> CaseSensitivePhrases => _caseSensitivePhrases;

        /// <summary>
        /// Case-insensitive word set.
        /// </summary>
        public override IList<string> CaseInsensitivePhrases => _caseInsensitivePhrases;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public IgnoreListTextFilter()
        {
            _caseSensitiveWords = new HashSet<string>();
            _caseInsensitiveWords = new HashSet<string>();
            _caseSensitivePhrases = new List<string>();
            _caseInsensitivePhrases = new List<string>();
        }

        /// <summary>
        /// Sets up filter withg ignore list items, dividing into words and phrases according to whitespace.
        /// </summary>
        /// <param name="inputItems">Input items (required).</param>
        public void SetIgnoreList(IList<IgnoreListItem> inputItems)
        {
            _caseSensitiveWords.Clear();
            _caseInsensitiveWords.Clear();
            _caseSensitivePhrases.Clear();
            _caseInsensitivePhrases.Clear();

            ISet<string> caseSensitivePhraseSet = new HashSet<string>();
            ISet<string> caseInsensitivePhraseSet = new HashSet<string>();

            foreach (var listItem in inputItems)
            {
                if (listItem.IsPhrase())
                {
                    if (listItem.IsIgnoreCase)
                    {
                        caseInsensitivePhraseSet.Add(listItem.CaseInsensitiveText());
                    }
                    else
                    {
                        caseSensitivePhraseSet.Add(listItem.CaseSensitiveText);
                    }
                }
                else
                {
                    if (listItem.IsIgnoreCase)
                    {
                        _caseInsensitiveWords.Add(listItem.CaseInsensitiveText());
                    }
                    else
                    {
                        _caseSensitiveWords.Add(listItem.CaseSensitiveText);
                    }
                }
            }

            _caseSensitivePhrases.AddRange(caseSensitivePhraseSet);
            _caseInsensitivePhrases.AddRange(caseInsensitivePhraseSet);
        }
    }
}
