/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
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
        BadReference, // Book name or chapter number incorrect or doesn't exist
        Unknown
    }
}
