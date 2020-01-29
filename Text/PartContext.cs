using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Text
{
    /// <summary>
    /// Context of checked text.
    /// </summary>
    public enum PartContext
    {
        MainText, // Main verse text (i.e., not obviously tagged other than with "\p"; expected in verse 1+).
        Outlines, // Outlines (i.e., \io- paragraph tags; expected in verse 0)
        Introductions, // Introductions (i.e., non-outline \i- paragraph tags; expected in verse 0)
        NoteOrReference // Notes or references (i.e., \f, \x, and related \e- character tags; expected in verse 1+)
    }
}
