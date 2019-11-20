using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    public class CheckUpdatedArgs
    {
        private int _currPos;

        private int _maxPos;

        public int CurrPos { get => _currPos; }
        public int MaxPos { get => _maxPos; }

        public CheckUpdatedArgs(int currPos, int maxPos)
        {
            this._currPos = currPos;
            this._maxPos = maxPos;
        }
    }
}
