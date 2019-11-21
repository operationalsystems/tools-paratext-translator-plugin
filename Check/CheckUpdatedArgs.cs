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
        private int _currPos;

        /// <summary>
        /// Max book position in check (1 for single-book check, 66 for all books, etc.).
        /// </summary>
        private int _maxPos;

        /// <summary>
        /// Current book count in check.
        /// </summary>
        public int CurrPos { get => _currPos; }

        /// <summary>
        /// Max book position in check (1 for single-book check, 66 for all books, etc.).
        /// </summary>
        public int MaxPos { get => _maxPos; }

        /// <summary>
        /// Basic ctor;
        /// </summary>
        /// <param name="currPos">Current book count in check.</param>
        /// <param name="maxPos">Max book position in check.</param>
        public CheckUpdatedArgs(int currPos, int maxPos)
        {
            this._currPos = currPos;
            this._maxPos = maxPos;
        }
    }
}
