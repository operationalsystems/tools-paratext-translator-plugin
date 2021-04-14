using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TvpCustomControls
{
    /**
     * This RichTextBox supports Unicode characters that may otherwise fail in a RichTextBox.
     * Once .Net's RichTextBox supports characters used in translations like `arNAV12` and `bnBCV19`, this override may no longer be necessary.
     */
    public class RichTextBox50 : RichTextBox
    {
        // Override the CreateParams getter so that this class uses the "RICHEDIT50W" RichText Control
        protected override CreateParams CreateParams
        {
            get
            {
                var createParams = base.CreateParams;

                // Only override the class if the appropriate DLL is available
                if (LoadLibrary("msftedit.dll") != IntPtr.Zero) createParams.ClassName = "RICHEDIT50W";

                return createParams;
            }
        }

        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string fileName);
    }
}