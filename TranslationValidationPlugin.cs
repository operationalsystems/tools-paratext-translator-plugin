using AddInSideViews;
using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using TvpMain.Form;
using TvpMain.Util;

/*
 * This is the main framework for the Translation Validation Plugin
 */
namespace TvpMain
{
    /*
     * Positions the launch for the Translation Validation Plugin in the Main Tools drop down in Paratext.
     */
    [AddIn("Translation Validation Plugin", Description = "Validation checks for translated text.", Version = "1.0", Publisher = "Biblica")]
    [QualificationData(PluginMetaDataKeys.menuText, "Translation Validation Plugin")]
    [QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|")]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.always)]
    public class TranslationValidationPlugin : IParatextAddIn2
    {
        public void Run(IHost host, string activeProjectName)
        {
            lock (this)
            {

                ErrorUtil.Host = host;
                ErrorUtil.TranslationValidationPlugin = this;

                try
                {
                    Application.EnableVisualStyles();
                    Thread uiThread = new Thread(() =>
                    {
                        CheckForm checkForm = new CheckForm(this, host, activeProjectName);
                        Application.Run(checkForm);

                        Environment.Exit(0);
                    });

                    uiThread.IsBackground = false;
                    uiThread.SetApartmentState(ApartmentState.STA);
                    uiThread.Start();
                }
                catch (Exception ex)
                {
                    ErrorUtil.ReportError(ex);
                    throw;
                }
            }

        }

        public void RequestShutdown()
        {
            Environment.Exit(0);
        }

        public void Activate(string activeProjectName)
        {
            // ignore, for now
        }

        public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
        {
            get { return null; }
        }
    }
}