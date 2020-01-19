using System.Collections.Generic;
using TvpMain.Result;
using TvpMain.Text;

namespace TvpMain.Check
{
    public class ScriptureReferenceCheck : ITextCheck
    {
        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        public void CheckText(VersePart partData, ICollection<ResultItem> checkResults)
        {
            // do nothing, at this time.
        }
    }
}
