using AddInSideViews;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using TvpMain.Check;
using TvpMain.Data;
using TvpMain.Filter;
using TvpMain.Form;
using TvpMain.Util;

/*
 * This is the main form for the Translation Validation Plugin and will be the base for the UI work.
 */
namespace TvpMain.Form
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private readonly RegexPunctuationCheck1 _punctuationCheck;
        private readonly IgnoreListTextFilter _ignoreFilter;
        private ProgressForm _progressForm;

        private ToolStripMenuItem _wordListMenuItem;
        private ToolStripMenuItem _ignoreListMenuItem;

        private CheckResult _lastResult;

        private IList<ResultItem> _allResultItems;
        private IList<ResultItem> _filteredResultItems;

        public MainForm(IHost host, string activeProjectName)
        {
            try
            {
                /*
                * Controls the flow of the Progress form and Result handler.
                */
                InitializeComponent();

                _host = host ?? throw new ArgumentNullException(nameof(host));
                _activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));

                _progressForm = new ProgressForm();
                _progressForm.Cancelled += OnProgressFormCancelled;

                _ignoreFilter = new IgnoreListTextFilter();
                _punctuationCheck = new RegexPunctuationCheck1(_host, _activeProjectName);
                _punctuationCheck.CheckUpdated += OnCheckProgress;
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
            }

        }

        private void OnProgressFormCancelled(object sender, EventArgs e)
        {
            dgvCheckResults.Rows.Clear();
            _punctuationCheck.CancelCheck();

            HideProgress();
        }

        /*
         * Launches the Progress form after run has been clicked.
         */
        private void OnCheckProgress(object sender, int currBookNum)
        {
            _progressForm.SetCurrBookNum(currBookNum);
        }

        private void UpdateMainTable()
        {
            FilterResults();

            dgvCheckResults.Rows.Clear();
            foreach (ResultItem item in _filteredResultItems)
            {
                dgvCheckResults.Rows.Add(new string[] {
                            $"{MainConsts.BOOK_NAMES[item.BookNum - 1] + " " + item.ChapterNum + ":" + item.VerseNum}",
                            $"{item.MatchText}",
                            $"{item.VerseText}",
                            $"{item.ErrorText}"
                        });
            }
        }

        private void FilterResults()
        {
            if (_allResultItems == null)
            {
                _filteredResultItems = Enumerable.Empty<ResultItem>().ToList();
            }
            else
            {
                if (ignoreListToolStripMenuItem.Checked
                    && !_ignoreFilter.IsEmpty)
                {
                    _filteredResultItems = _allResultItems.Where(
                        resultItem => !_ignoreFilter.FilterText(resultItem)).ToList();
                }
                else
                {
                    _filteredResultItems = _allResultItems;
                }
            }
        }

        private void ShowProgress()
        {
            _progressForm.ResetForm();

            Enabled = false;
            _progressForm.Show(this);
        }

        private void HideProgress()
        {
            _progressForm.Hide();

            Enabled = true;
            Activate();
        }

        private void OnRunChecks(object sender, EventArgs e)
        {
            dgvCheckResults.Rows.Clear();
            try
            {
                ShowProgress();
                _lastResult = _punctuationCheck.RunCheck() ?? _lastResult;

                HideProgress();
                if (_lastResult != null)
                {
                    _allResultItems = new List<ResultItem>(_lastResult.ResultItems);
                    UpdateMainTable();
                }
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            _wordListMenuItem = biblicalWordListToolStripMenuItem;
            _ignoreListMenuItem = ignoreListToolStripMenuItem;
        }

        private void OnClickIgnoreList(object sender, EventArgs e)
        {
            IgnoreListForm ignoreListForm = new IgnoreListForm();
            ignoreListForm.IgnoreListItems = HostUtil.Instance.GetIgnoreListItems(_activeProjectName);

            ignoreListForm.ShowDialog(this);
            IList<IgnoreListItem> ignoreListItems = ignoreListForm.IgnoreListItems;

            HostUtil.Instance.PutIgnoreListItems(_activeProjectName, ignoreListItems);
            _ignoreFilter.SetIgnoreListItems(ignoreListItems);

            UpdateMainTable();
        }

        private void FormTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (MessageBox.Show(this, "Are you sure you want to quit?",
                                     "Notice...", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
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
            _wordListMenuItem.Checked = !_wordListMenuItem.Checked;
            _wordListMenuItem.CheckState = _wordListMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }

        private void IgnoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ignoreListMenuItem.Checked = !_ignoreListMenuItem.Checked;
            _ignoreListMenuItem.CheckState = _ignoreListMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
