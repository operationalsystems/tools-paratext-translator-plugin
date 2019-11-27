using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    /// <summary>
    /// General-purpose text filter interface created for ignore list and biblical term filters.
    /// </summary>
    public interface ITextFilter
    {
        /// <summary>
        /// Main filter method.
        /// </summary>
        /// <param name="inputText">Text to be checked for filtering (required).</param>
        /// <returns>True if filter matches, false otherwise.</returns>
        public bool FilterText(String inputText);

        /// <summary>
        /// True if filter is currently a no-op (i.e., an empty ignore list), false otherwise.
        /// </summary>
        public bool IsEmpty { get; }
    }
}
