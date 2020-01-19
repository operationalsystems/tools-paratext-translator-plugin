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
        /// <param name="partData">Verse part data, including original verse, location, etc.</param>
        /// <param name="checkResults">Result items list to populate.</param>
        void CheckText(PartData partData, ICollection<ResultItem> checkResults);
    }
}
