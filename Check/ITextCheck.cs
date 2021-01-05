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
        /// Type discriminator, for filtering results.
        /// </summary>
        CheckType CheckType { get; }

        /// <summary>
        /// Check implementation.
        /// </summary>
        /// <param name="versePart">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        void CheckText(VersePart versePart, ICollection<ResultItem> checkResults);
    }
}
