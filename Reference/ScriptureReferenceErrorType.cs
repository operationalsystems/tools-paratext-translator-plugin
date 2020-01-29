using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Reference
{
    /// <summary>
    /// Scripture reference error sub-types,
    /// used for ResultTypeCode field in ResultItem.
    /// </summary>
    public enum ScriptureReferenceErrorType
    {
        LooseFormatting, // Spacing or punctuation misses
        IncorrectNameStyle, // Name style inappropriate for context (e.g., abbrev when it should be long).
        TagShouldNotExist, // for when there shouldn't be a tag
                           // (found in section that should not
                           // have tags (the other ignore list -- overloaded term) )
        MissingTag, // Reference should have tag, but does not (e.g., in body footnote)
        IncorrectTag, // Wrong tag for context (e.g., \xt in footnote instead of \+xt)
        MalformedTag, // Missing slash(es), unmatched tag pair, etc.
        BadReference // Book name or chapter number incorrect or doesn't exist
    }
}
