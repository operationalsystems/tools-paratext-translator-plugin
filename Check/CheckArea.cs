using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    /// <summary>
    /// Check area (i.e., scope).
    /// </summary>
    public enum CheckArea
    {
        CurrentProject, // user's entire current project 
        CurrentBook, // user's current book
        CurrentChapter // user's current chapter
    }
}
