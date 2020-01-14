using System.Collections.Generic;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    public class ScriptureReferenceCheck : ITextCheck
    {
        /// <summary>
        /// Implements verse check for scripture references.
        /// </summary>
        /// <param name="textLocation">Text location (required).</param>
        /// <param name="inputText">Input text.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckVerse(TextLocation textLocation, string inputText, IList<ResultItem> checkResults)
        {
            // do nothing, at this time.
        }
    }
}
