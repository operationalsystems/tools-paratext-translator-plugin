using AddInSideViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Data;

/*
 * A class to handle errors for the Translation Validation Plugin.
 */
namespace TvpMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class HostUtil
    {
        private static readonly HostUtil _instance = new HostUtil();

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
            string messageText = (prefixText ?? "Error: Please contact support.")
                + Environment.NewLine + Environment.NewLine
                + "Details: " + ex.ToString() + Environment.NewLine;
            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogLine($"Error: {messageText}", true);
        }

        public void LogLine(String inputText, bool isError)
        {
            (isError ? Console.Error : Console.Out).WriteLine(inputText);
            if (_host != null)
            {
                _host.WriteLineToLog(_translationValidationPlugin, inputText);
            }
        }

        public IList<IgnoreListItem> GetIgnoreListItems(string activeProjectName)
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

        public void PutIgnoreListItems(string activeProjectName, IList<IgnoreListItem> outputItems)
        {
            _host.PutPlugInData(_translationValidationPlugin,
                activeProjectName, MainConsts.IGNORE_LIST_ITEMS_ID,
                JsonConvert.SerializeObject(outputItems));
        }
    }
}
