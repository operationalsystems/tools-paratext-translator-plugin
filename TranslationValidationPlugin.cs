using AddInSideViews;
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using TvpMain.Forms;
using TvpMain.Util;

/*
 * This is the main framework for the Translation Validation Plugin
 */
namespace TvpMain
{
    /// <summary>
    /// Translation validation plugin entry point.
    /// </summary>
    [AddIn("Translation Validation Plugin", Description = "Provides validation checks for translated text.", Version = "2.0.0.5", Publisher = "Biblica")]
    [QualificationData(PluginMetaDataKeys.menuText, "Translation-Validation")]
    [QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|")]
    [QualificationData(PluginMetaDataKeys.enableWhen, WhenToEnable.anyProjectActive)]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.always)]
    public class TranslationValidationPlugin : IParatextAddIn2
    {
        /// <summary>
        /// Main entry method.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public void Run(IHost host, string activeProjectName)
        {
            if (activeProjectName == null || activeProjectName.Length == 0)
            {
                Util.HostUtil.Instance.LogLine("Active project name is invalid, throwing exception, can't operate.", true);
                throw new ArgumentNullException(nameof(activeProjectName));
            }

            lock (this)
            {

                HostUtil.Instance.Host = host;
                HostUtil.Instance.TranslationValidationPlugin = this;
#if DEBUG
                // Provided because plugins are separate processes that may only be attached to,
                // once instantiated (can't run Paratext and automatically attach, as with shared libraries).
                MessageBox.Show($"Attach debugger now to PID {Process.GetCurrentProcess().Id}, if needed!",
                    "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
                HostUtil.Instance.InitParatextData(false);

                try
                {
                    var uiThread = new Thread(() =>
                    {
                        try
                        {
                            Application.EnableVisualStyles();
                            Application.Run(new RunChecks(host, activeProjectName));
                        }
                        catch (Exception ex)
                        { 
                            HostUtil.Instance.ReportError(null, false, ex);
                        }
                        finally
                        {
                            Environment.Exit(0);
                        }
                    })
                    { IsBackground = false };

                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.Start();
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError(null, false, ex);
                }
            }

        }

        /// <summary>
        /// Shutdown request method.
        /// </summary>
        public void RequestShutdown()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Re-activation method (no-op, for now).
        /// </summary>
        /// <param name="activeProjectName">Active project name (ignored).</param>
        public void Activate(string activeProjectName)
        {
            // ignore, for now
        }

        /// <summary>
        /// Get the default data key file specs
        /// </summary>
        public Dictionary<string, IPluginDataFileMergeInfo> _dataFileKeySpecifications = new Dictionary<string, IPluginDataFileMergeInfo>
        {
            [MainConsts.CHECK_SETTINGS_DATA_ID] = new PluginDataFileMergeInfo(
                new MergeLevel("DefaultCheckIds", ".")
            ),
            [MainConsts.DENIED_RESULTS_DATA_ID] = new PluginDataFileMergeInfo(
                new MergeLevel("ArrayOfInt", ".")
            )
        };

        /// <summary>
        /// Data file key spec accessor.
        /// </summary>
        public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
        {
            get
            {
                return _dataFileKeySpecifications;
            }
        }
    }
}