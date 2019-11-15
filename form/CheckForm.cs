using AddInSideViews;
using System;
using System.Windows.Forms;
using System.Windows.Threading;
using TvpMain.Check;
using TvpMain.Data;
using TvpMain.Form;
using TvpMain.Util;

/*
 * This is the main form for the Translation Validation Plugin and will be the base for the UI work.
 */
namespace TvpMain.Form
{
    public partial class CheckForm : System.Windows.Forms.Form
    {
        private readonly TranslationValidationPlugin _plugin;
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private readonly ProgressForm _progressForm;
        private readonly PunctuationCheck1 _punctuationCheck;

        private ToolStripMenuItem _wordListMenuItem;
        private ToolStripMenuItem _ignoreListMenuItem;

        public CheckForm(TranslationValidationPlugin plugin, IHost host, string activeProjectName)
        {
            try
            {
                /*
                * Controls the flow of the Progress form and Result handler.
                */
                InitializeComponent();

                this._plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
                this._host = host ?? throw new ArgumentNullException(nameof(host));
                this._activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));

                this._progressForm = new ProgressForm();
                this._punctuationCheck = new PunctuationCheck1(this._plugin, this._host, this._activeProjectName);
                this._punctuationCheck.ProgressHandler += OnCheckProgress;
                this._punctuationCheck.ResultHandler += OnCheckResult;
            }
            catch (Exception ex)
            {
                ErrorUtil.ReportError(ex);
            }

        }

        internal void CancelCheck()
        {
            this.dataGridView1.Rows.Clear();
            this._punctuationCheck.CancelCheck();

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    this.HideProgress();
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    ErrorUtil.ReportError(ex);
                }
            });
        }

        /*
         * Launches the Progress form after run has been clicked.
         */
        private void OnCheckProgress(object sender, int currBookNum)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    this._progressForm.SetCurrBookNum(currBookNum);
                    this._progressForm.Activate();

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
        private void OnCheckResult(object sender, CheckResult chkResult)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                try
                {
                    this.HideProgress();

                    foreach (ResultItem item in (chkResult.ResultItems))
                    {

                        this.dataGridView1.Rows.Add(new string[] { $"{MainConsts.BOOK_NAMES[item.BookNum - 1] + " " + item.ChapterNum + ":" + item.VerseNum}", $"{item.VerseText}"
                        });
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
            this._progressForm.Show(this);

            Application.DoEvents();
        }

        private void HideProgress()
        {
            this._progressForm.Hide();
            this.Enabled = true;
            this._progressForm.ResetForm();
            this.Activate();

            Application.DoEvents();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Clear();
            try
            {
                this.ShowProgress();
                this._punctuationCheck.RunCheck();
            }
            catch (Exception ex)
            {
                ErrorUtil.ReportError(ex);
            }
        }

        private void FormTest_Load(object sender, EventArgs e)
        {
            _wordListMenuItem = this.biblicalWordListToolStripMenuItem;
            _ignoreListMenuItem = this.ignoreListToolStripMenuItem;
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

        private void BiblicalWordListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_wordListMenuItem.CheckState == CheckState.Checked)
            {
                _wordListMenuItem.CheckState = CheckState.Unchecked;
                _wordListMenuItem.Checked = false;

                MessageBox.Show("Biblical Word List is unselected.");
            }
            else
            {
                _wordListMenuItem.CheckState = CheckState.Checked;
                _wordListMenuItem.Checked = true;

                MessageBox.Show("Biblical Word List filter is selected.");
            }
        }

        private void IgnoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_ignoreListMenuItem.CheckState == CheckState.Checked)
            {
                _ignoreListMenuItem.CheckState = CheckState.Unchecked;
                _ignoreListMenuItem.Checked = false;

                MessageBox.Show("Ignore List filter is unselected.");
            }
            else
            {
                _ignoreListMenuItem.CheckState = CheckState.Checked;
                _ignoreListMenuItem.Checked = true;

                MessageBox.Show("Ignore List filter is selected.");
            }
        }

        private void PunctuationToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void BcvToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ErrorToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void NotesToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ActionToolStripMenuItem_Click(object sender, EventArgs e)
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
