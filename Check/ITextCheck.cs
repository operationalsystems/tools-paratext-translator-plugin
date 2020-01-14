using System.Collections.Generic;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    /// <summary>
    /// General-purpose per-verse text check.
    /// </summary>
    public interface ITextCheck
    {
        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="textLocation">Text location (required).</param>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="checkResults">Result items list to populate.</param>
        void CheckVerse(TextLocation textLocation, string inputText, IList<ResultItem> checkResults);
    }
}
