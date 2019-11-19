using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class IgnoreListItem
    {
        [JsonProperty]
        public string CaseSensitiveItemText { get; set; }

        [JsonProperty]
        public bool IsIgnoreCase { get; set; }

        public IgnoreListItem(string itemText, bool isIgnoreCase)
        {
            CaseSensitiveItemText = itemText ?? throw new ArgumentNullException(nameof(itemText));
            IsIgnoreCase = isIgnoreCase;
        }

        [JsonConstructor]
        private IgnoreListItem()
        {
        }

        public bool IsPhrase { get => CaseSensitiveItemText.Any(Char.IsWhiteSpace); }
        public string CaseInsensitiveItemText { get => CaseSensitiveItemText.ToLower(); }
    }
}
