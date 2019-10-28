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

        private ToolStripMenuItem biblicalWordListMenuItem;
        private ToolStripMenuItem ignoreListMenuItem;


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

                    foreach (ResultItem item in chkResult.ResultItems)
                    {
                        this.dataGridView1.Rows.Add(new string[] { $"{MainConsts.BOOK_NAMES[item.BookNum - 1] + " " + item.ChapterNum + ":" + item.VerseNum}", $"{item.ErrorText}" });
                    }

                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ErrorUtil.ReportError(ex);
                }
            });
        }

        private void ShowProgress()
        {
            this.Enabled = false;
            this.frmProgress.Show(this);

            //this.frmProgress.Visible = false;
            //StartForm(frmProgress);

            Application.DoEvents();
        }

        void StartForm(ProgressForm form)
        {
            DialogResult result = form.ShowDialog();
            if (result == DialogResult.Cancel)
                MessageBox.Show("Operation has been cancelled");
            
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
            biblicalWordListMenuItem = this.biblicalWordListToolStripMenuItem;
            ignoreListMenuItem = this.ignoreListToolStripMenuItem;
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var ignoreList = new IgnoreList();
            ignoreList.Show();
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show(this,
                                    "Are you sure you want to close this plugin?",
                                     "Close Plugin",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {

                // ---- *)  if No keep the application alive 
                //----  *)  else close the application
                case DialogResult.No:
                    e.Cancel = true;
                    break;
                default:
                    break;
            }
        }

        private void FormTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void biblicalWordListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (biblicalWordListMenuItem.CheckState == CheckState.Checked)
            {
                biblicalWordListMenuItem.CheckState = CheckState.Unchecked;
                biblicalWordListMenuItem.Checked = false;

                MessageBox.Show("Biblical Word List is unselected.");
            }
            else
            {
                biblicalWordListMenuItem.CheckState = CheckState.Checked;
                biblicalWordListMenuItem.Checked = true;

                MessageBox.Show("Biblical Word List filter is selected.");
            }
        }

        private void ignoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ignoreListMenuItem.CheckState == CheckState.Checked)
            {
                ignoreListMenuItem.CheckState = CheckState.Unchecked;
                ignoreListMenuItem.Checked = false;

                MessageBox.Show("Ignore List filter is unselected.");
            }
            else
            {
                ignoreListMenuItem.CheckState = CheckState.Checked;
                ignoreListMenuItem.Checked = true;

                MessageBox.Show("Ignore List filter is selected.");
            }
        }

        private void punctuationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (punctuationToolStripMenuItem.CheckState == CheckState.Checked)
            {
                punctuationToolStripMenuItem.CheckState = CheckState.Unchecked;
                punctuationToolStripMenuItem.Checked = false;

                MessageBox.Show("Punctuation Check unselected.");
            }
            else
            {
                punctuationToolStripMenuItem.CheckState = CheckState.Checked;
                punctuationToolStripMenuItem.Checked = true;

                MessageBox.Show("Punctuation Check selected.");
            }
        }

        private void bcvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bCVToolStripMenuItem.CheckState == CheckState.Checked)
            {
                bCVToolStripMenuItem.CheckState = CheckState.Unchecked;
                bCVToolStripMenuItem.Checked = false;

                MessageBox.Show("BCV column is hidden.");
            }
            else
            {
                bCVToolStripMenuItem.CheckState = CheckState.Checked;
                bCVToolStripMenuItem.Checked = true;

                MessageBox.Show("BCV is shown.");
            }
        }

        private void errorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (errorToolStripMenuItem.CheckState == CheckState.Checked)
            {
                errorToolStripMenuItem.CheckState = CheckState.Unchecked;
                errorToolStripMenuItem.Checked = false;

                MessageBox.Show("Error column is hidden.");
            }
            else
            {
                errorToolStripMenuItem.CheckState = CheckState.Checked;
                errorToolStripMenuItem.Checked = true;

                MessageBox.Show("Error column is shown.");
            }
        }

        private void notesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (notesToolStripMenuItem.CheckState == CheckState.Checked)
            {
                notesToolStripMenuItem.CheckState = CheckState.Unchecked;
                notesToolStripMenuItem.Checked = false;

                MessageBox.Show("Notes column is hidden.");
            }
            else
            {
                notesToolStripMenuItem.CheckState = CheckState.Checked;
                notesToolStripMenuItem.Checked = true;

                MessageBox.Show("Notes column is shown.");
            }
        }

        private void actionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actionToolStripMenuItem.CheckState == CheckState.Checked)
            {
                actionToolStripMenuItem.CheckState = CheckState.Unchecked;
                actionToolStripMenuItem.Checked = false;

                MessageBox.Show("Actions column is hidden.");
            }
            else
            {
                actionToolStripMenuItem.CheckState = CheckState.Checked;
                actionToolStripMenuItem.Checked = true;

                MessageBox.Show("Actions column is shown.");
            }
        }

    }
}
