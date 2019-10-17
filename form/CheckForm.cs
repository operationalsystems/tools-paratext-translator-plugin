using AddInSideViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using translation_validation_framework.form;
using translation_validation_framework.util;

/*
 * This is the main form for the Translation Validation Plugin and will be the base for the UI work.
 */
namespace translation_validation_framework
{
    public partial class FormTest : Form
    {
        private readonly TranslationValidationPlugin plugin;

        private readonly IHost host;

        private readonly string activeProjectName;

        private readonly ProgressForm frmProgress;

        private readonly PuctuationCheck1 chkPunctuation1;

        
        public FormTest(TranslationValidationPlugin plugin, IHost host, string activeProjectName)
        {
            try
            {
                /*
                * Controls the flow of the Progress form and Result handler.
                */
                InitializeComponent();

                this.plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
                this.host = host ?? throw new ArgumentNullException(nameof(host));
                this.activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));

                this.frmProgress = new ProgressForm();
                this.chkPunctuation1 = new PuctuationCheck1(this.plugin, this.host, this.activeProjectName);
                this.chkPunctuation1.ProgressHandler += ChkPunctuation1_ProgressHandler;
                this.chkPunctuation1.ResultHandler += ChkPunctuation1_ResultHandler;
            }
            catch (Exception ex)
            {
                ErrorUtil.ReportError(ex);
            }

        }
        /*
         * Launches the Progress form after run has been clicked.
         */
        private void ChkPunctuation1_ProgressHandler(object sender, int currBookNum)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    this.frmProgress.setCurrBookNum(currBookNum);
                    this.frmProgress.Activate();

                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ErrorUtil.ReportError(ex);
                }
            });
        }

        /*
         * Populates after the main Validation form after the Progress form has finished.
         */
        private void ChkPunctuation1_ResultHandler(object sender, CheckResult chkResult)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    this.HideProgress();

                    string text = "";
                    foreach (ResultItem item in chkResult.ResultItems)
                    {
                        text += item.ToString();
                        text += Environment.NewLine;

                        this.dataGridView1.Rows.Add(new string[] { $"{MainConsts.BOOK_NAMES[item.BookNum - 1] + " " + item.ChapterNum + ":" + item.VerseNum}", $"{item.ErrorText}" });
  
                    }

                    this.SetText(text);
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ErrorUtil.ReportError(ex);
                }
            });
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void SetText(string inputText)
        {
            this.txtData.Text = inputText;
        }

        private void ShowProgress()
        {
            this.Enabled = false;
            this.frmProgress.Show(this);

            Application.DoEvents();
        }

        private void HideProgress()
        {
            this.frmProgress.Hide();
            this.Enabled = true;
            this.Activate();

            Application.DoEvents();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowProgress();
                this.chkPunctuation1.RunCheck();
            }
            catch (Exception ex)
            {
                ErrorUtil.ReportError(ex);
            }
        }

        private void FormTest_Load(object sender, EventArgs e)
        {

        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var ignoreList = new IgnoreList();
            ignoreList.Show();
        }

        private void PunctuationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
