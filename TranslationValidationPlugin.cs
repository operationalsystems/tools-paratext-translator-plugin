using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Threading;
using AddInSideViews;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;

/*
 * This is the main framework for the Translation Validation Plugin
 */
namespace translation_validation_framework
{
    /*
     * Positions the launch for the Translation Validation Plugin in the Main Tools drop down in Paratext.
     */
    [AddIn("Translation Validation Plugin", Description = "Framework that will hold validation checks for translated text.", Version = "1.0", Publisher = "Biblica")]
    [QualificationData(PluginMetaDataKeys.menuText, "Translation Validation Plugin")]
    [QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|")]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.always)]
    public class TranslationValidationPlugin : IParatextAddIn2
    {
        public const string pluginName = "Capitalizatoin Validation Check";

        private CheckForm frmCheck;

        public void Run(IHost host, string activeProjectName)
        {
            lock (this)
            {

                ErrorUtil.Host = host;
                ErrorUtil.TranslationValidationPlugin = this;

                try
                {
                    Application.EnableVisualStyles();
                    Thread mainUIThread = new Thread(() =>
                    {
                        if (frmCheck == null)
                        {
                            frmCheck = new CheckForm(this, host, activeProjectName);
                        }
                        frmCheck.ShowDialog();
                        Environment.Exit(0);
                    });

                    mainUIThread.Name = pluginName;
                    mainUIThread.IsBackground = false;
                    mainUIThread.SetApartmentState(ApartmentState.STA);
                    mainUIThread.Start();

                    Console.Error.WriteLine("BEEP!");
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
            // Paratext will shutdown the plugin when it is closed.
            lock (this)
            {
                if (frmCheck != null)
                {
                    frmCheck.Close();
                }
            }
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