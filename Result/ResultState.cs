using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Result
{
    /// <summary>
    /// Indicates the state of a given check result.
    /// </summary>
    public enum ResultState
    {
        Found,
        Ignored,
        ToBeFixed,
        Fixed
    }
}
