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

namespace translation_validation_framework
{
    /// <summary>
    /// This is the Capitalization Validation Check for finding instances of miscapitalizaed words following non-final punctuation.
    /// </summary>
    [AddIn("Translation Validation Plugin", Description = "Framework that will hold validation checks for translated text.", Version = "1.0", Publisher = "Biblica")]
    [QualificationData(PluginMetaDataKeys.menuText, "Translation Validation Plugin")]
    [QualificationData(PluginMetaDataKeys.insertAfterMenuName, "Tools|")]
    [QualificationData(PluginMetaDataKeys.multipleInstances, CreateInstanceRule.always)]
    public class TranslationValidationPlugin : IParatextAddIn
    {
        public const string pluginName = "Capitalizatoin Validation Check";

        private FormTest Form;
        /// <summary>
        /// Creates a "Capitalizatoin Validation Check" accessible while there is an active project.
        /// </summary>
        public void Run(IHost host, string activeProjectName)
        {
            lock(this)
            {
                ScriptExtractor(host, activeProjectName);
                
            }
            
        }

        public void ScriptExtractor(IHost host, string activeProjectName)
        {
            FormTest formTest = new FormTest();

            IScrExtractor extractor = host.GetScriptureExtractor(activeProjectName, ExtractorType.USFM);
            string versificationName = host.GetProjectVersificationName(activeProjectName);
            int currentRef = host.GetCurrentRef(versificationName);
            string text = "";
            for (int i = 1; i <= 111; i++)
            {
                int coord = i * 1000000;
                coord += 1000;
                coord += host.GetLastVerse(i, 1, versificationName);
                text += "#" + i.ToString();
                text += Environment.NewLine;
                text += extractor.Extract(coord, coord);
                text += Environment.NewLine;
            }
            //int book = currentref / 1000000;
            //int chapter = (currentref / 1000) % 1000;
            //int bookandchapter = (currentref / 1000) * 1000;
            //int startofchap = bookandchapter + 1;
            //int endofchap = bookandchapter + host.getlastverse(book, chapter, versificationname);
            
            //string text = extractor.extract(startofchap, endofchap);

            ////loop over all books, chapters and verses
            ////test each verse against the regex
            ////save errors to a list
            ////output the location (bcv), error, note, action
            //for (int i = 0; i <= currentref; i++)
            //{ 
                
                
            //}
            formTest.SetText(text);
            formTest.ShowDialog();
        }

        public class RunCheck
        {
            TranslationValidationPlugin tvp = new TranslationValidationPlugin();
        }


        public void RequestShutdown()
        {
            // Paratext will shutdown the plugin when it is closed.
            lock(this)
            {
                if (Form != null)
                {
                    Form.Close();
                }
            }
        }

        public Dictionary<string, IPluginDataFileMergeInfo> DataFileKeySpecifications
        {
            get { return null; }
        }

        static void Main(string[] args)
        {
            Form test = new translation_validation_framework.FormTest();
            test.ShowDialog();
        }
    }
}