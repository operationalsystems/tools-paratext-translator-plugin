using AddInSideViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Data;

/// <summary>
/// Global error-handling and other maintenance capabilities.
/// </summary>
namespace TvpMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class HostUtil
    {
        /// <summary>
        /// Thread-safe singleton pattern.
        /// </summary>
        private static readonly HostUtil _instance = new HostUtil();

        /// <summary>
        /// Thread-safe singleton accessor.
        /// </summary>
        public static HostUtil Instance
        {
            get => _instance;
        }

        /// <summary>
        /// Global reference to plugin, to route logging.
        /// </summary>
        private TranslationValidationPlugin _translationValidationPlugin;

        /// <summary>
        /// Global reference to host interface, providing Paratext services including logging.
        /// </summary>
        private IHost _host;

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public TranslationValidationPlugin TranslationValidationPlugin { set => _translationValidationPlugin = value; }

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public IHost Host { set => _host = value; }

        /// <summary>
        /// Reports exception to log and message box w/o prefix text.
        /// </summary>
        /// <param name="ex"></param>
        public void ReportError(Exception ex)
        {
            ReportError(null, ex);
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="ex">Exception (required).</param>
        public void ReportError(string prefixText, Exception ex)
        {
            ReportError(prefixText, true, ex);
        }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="includeStackTrace">True to include stack trace, false otherwise.</param>
        /// <param name="ex">Exception (required).</param>
        public void ReportError(string prefixText, bool includeStackTrace, Exception ex)
        {
            string messageText = null;
            if (includeStackTrace)
            {
                messageText = (prefixText ?? "Error: Please contact support.")
                    + Environment.NewLine + Environment.NewLine
                    + "Details: " + ex.ToString() + Environment.NewLine;
            }
            else
            {
                messageText = (prefixText ?? "Error: Please contact support")
                    + $" (Details: {ex.Message}).";
            }
            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogLine(messageText, true);
        }

        /// <summary>
        /// Log text to Paratext's app log and the console.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="isError">Error flag.</param>
        public void LogLine(String inputText, bool isError)
        {
            (isError ? Console.Error : Console.Out).WriteLine(inputText);
            if (_host != null)
            {
                _host.WriteLineToLog(_translationValidationPlugin, inputText);
            }
        }

        /// <summary>
        /// Retrieve the ignore list from the host's plugin data storage.
        /// </summary>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <returns>Ignore list.</returns>
        public IList<IgnoreListItem> GetIgnoreList(string activeProjectName)
        {
            string inputData =
                    _host.GetPlugInData(_translationValidationPlugin,
                activeProjectName, MainConsts.IGNORE_LIST_ITEMS_ID);
            if (inputData == null)
            {
                return Enumerable.Empty<IgnoreListItem>().ToList();
            }
            else
            {
                return JsonConvert.DeserializeObject<List<IgnoreListItem>>(inputData);
            }
        }

        /// <summary>
        /// Stores the ignore list to the host's plugin data storage.
        /// </summary>
        /// <param name="activeProjectName">Active project name (required).</param>
        /// <param name="outputItems">Ignore list.</param>
        public void PutIgnoreList(string activeProjectName, IList<IgnoreListItem> outputItems)
        {
            _host.PutPlugInData(_translationValidationPlugin,
                activeProjectName, MainConsts.IGNORE_LIST_ITEMS_ID,
                JsonConvert.SerializeObject(outputItems));
        }

        /// <summary>
        /// Converts a Paratext coordinate reference to specific book, chapter, and verse.
        /// </summary>
        /// <param name="inputRef">Input coordinate reference (BBBCCCVVV).</param>
        /// <param name="outputBook">Output book number (1-66).</param>
        /// <param name="outputChapter">Output chapter number (1-1000; Max varies by book & versification).</param>
        /// <param name="outputVerse">Output verse number (1-1000; Max varies by chapter & versification).</param>
        static public void RefToBCV(int inputRef, out int outputBook, out int outputChapter, out int outputVerse)
        {
            outputBook = (inputRef / MainConsts.BOOK_REF_MULTIPLIER);
            outputChapter = (inputRef / MainConsts.CHAP_REF_MULTIPLIER) % MainConsts.REF_PART_RANGE;
            outputVerse = inputRef % MainConsts.REF_PART_RANGE;
        }


        /// <summary>
        /// Converts specific book, chapter, and verse to a Paratext coordinate reference.
        /// </summary>
        /// <param name="inputBook">Input book number (1-66).</param>
        /// <param name="inputChapter">Input chapter number (1-1000; Max varies by book & versification).</param>
        /// <param name="inputVerse">Input verse number (1-1000; Max varies by chapter & versification).</param>
        /// <returns>Output coordinate reference (BBBCCCVVV).</returns>
        static public int BcvToRef(int inputBook, int inputChapter, int inputVerse)
        {
            return (inputBook * MainConsts.BOOK_REF_MULTIPLIER)
                + (inputChapter * MainConsts.CHAP_REF_MULTIPLIER)
                + inputVerse;
        }
    }
}
