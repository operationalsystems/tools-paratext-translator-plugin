using AddInSideViews;
using System;
using System.Windows.Forms;

/*
 * A class to handle errors for the Translation Validation Plugin.
 */
namespace TvpMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class ErrorUtil
    {
        /// <summary>
        /// Global reference to plugin, to route logging.
        /// </summary>
        static private TranslationValidationPlugin translationValidationPlugin;

        /// <summary>
        /// Global reference to host interface, providing Paratext services including logging.
        /// </summary>
        static private IHost host;

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public static TranslationValidationPlugin TranslationValidationPlugin { set => translationValidationPlugin = value; }

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public static IHost Host { set => host = value; }

        /// <summary>
        /// Reports exception to log and message box w/o prefix text.
        /// </summary>
        /// <param name="ex"></param>
        public static void ReportError(Exception ex)
        {
            ReportError(null, ex);
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="ex">Exception (required).</param>
        public static void ReportError(string prefixText, Exception ex)
        {
            string messageText = (prefixText ?? "Error: Please contact support.")
                + Environment.NewLine + Environment.NewLine
                + "Details: " + ex.ToString() + Environment.NewLine;
            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (host != null)
            {
                host.WriteLineToLog(translationValidationPlugin, $"Error: {messageText}");
            }
        }
    }
}
