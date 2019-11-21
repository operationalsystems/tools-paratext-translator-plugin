using CsvHelper.Configuration.Attributes;
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
        [Index(0)]
        public string CaseSensitiveText { get; set; }

        [JsonProperty]
        [Index(1)]
        [Default(false)]
        public bool IsIgnoreCase { get; set; }

        [Ignore]
        public bool IsPhrase { get => CaseSensitiveText.Any(Char.IsWhiteSpace); }

        [Ignore]
        public string CaseInsensitiveText { get => CaseSensitiveText.ToLower(); }

        public IgnoreListItem(string itemText, bool isIgnoreCase)
        {
            CaseSensitiveText = itemText ?? throw new ArgumentNullException(nameof(itemText));
            IsIgnoreCase = isIgnoreCase;
        }

        [JsonConstructor]
        private IgnoreListItem()
        {
        }
    }
}
