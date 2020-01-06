using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    /// <summary>
    /// Args for check update events.
    /// </summary>
    public class CheckUpdatedArgs
    {
        /// <summary>
        /// Current book count in check.
        /// </summary>
        public int CurrBookNum { get; }

        /// <summary>
        /// Max book position in check (1 for single-book check, 66 for all books, etc.).
        /// </summary>
        public int MaxBookNum { get; }

        /// <summary>
        /// Basic ctor;
        /// </summary>
        /// <param name="currBookNum">Current book count in check.</param>
        /// <param name="maxBookNum">Max book position in check.</param>
        public CheckUpdatedArgs(int currBookNum, int maxBookNum)
        {
            this.CurrBookNum = currBookNum;
            this.MaxBookNum = maxBookNum;
        }
    }
}
