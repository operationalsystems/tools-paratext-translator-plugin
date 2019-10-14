using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/*
 * A class to handle errors for the Translation Validation Plugin.
 */
namespace translation_validation_framework
{
    public class ErrorUtil
    {
        static private TranslationValidationPlugin translationValidationPlugin;

        static private IHost host;

        public static TranslationValidationPlugin TranslationValidationPlugin { get => translationValidationPlugin; set => translationValidationPlugin = value; }
        public static IHost Host { get => host; set => host = value; }

        public static void ReportError(Exception ex)
        {
            reportError(null, ex);
        }

        public static void reportError(string prefix, Exception ex)
        {
            string text = (prefix ?? "") + Environment.NewLine
                + ex.Message + Environment.NewLine
                + ex.StackTrace + Environment.NewLine;

            MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            host.WriteLineToLog(translationValidationPlugin, $"Error: {text}");
        }
    }
}
