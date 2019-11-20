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
        private readonly BiblicalTermsTextFilter _termFilter;
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
                _termFilter = new BiblicalTermsTextFilter();
                _punctuationCheck = new RegexPunctuationCheck1(_host, _activeProjectName);
                _punctuationCheck.CheckUpdated += OnCheckUpdated;

                searchMenuTextBox.TextChanged += OnSearchTextChanged;

                termWorker.DoWork += OnTermWorkerDoWork;
                termWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
            }
        }

        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            UpdateMainTable();
        }

        private void OnTermWorkerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                lock (_termFilter)
                {
                    _termFilter.SetKeyTerms(_host.GetProjectKeyTerms(_activeProjectName,
                        _host.GetProjectLanguageId(_activeProjectName, "translation validation")));
                }
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
        private void OnCheckUpdated(object sender, CheckUpdatedArgs updatedArgs)
        {
            _progressForm.OnCheckUpdated(updatedArgs);
        }

        private void UpdateMainTable()
        {
            FilterResults();

            dgvCheckResults.Rows.Clear();
            statusLabel.Text = CheckResult.GetSummaryText(_filteredResultItems);

            foreach (ResultItem item in _filteredResultItems)
            {
                dgvCheckResults.Rows.Add(new string[] {
                            $"{MainConsts.BOOK_NAMES[item.BookNum - 1] + " " + item.ChapterNum + ":" + item.VerseNum}",
                            $"{item.MatchText}",
                            $"{item.VerseText}",
                            $"{item.ErrorText}"
                        });
                dgvCheckResults.Rows[(dgvCheckResults.Rows.Count - 1)].HeaderCell.Value =
                    $"{dgvCheckResults.Rows.Count:N0}";
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
                _filteredResultItems = _allResultItems;
                bool isEntireVerse = entireVerseFiltersMenuItem.Checked;

                if (ignoreListFiltersMenuItem.Checked
                    && !_ignoreFilter.IsEmpty)
                {
                    _filteredResultItems = _filteredResultItems.Where(
                        resultItem => !_ignoreFilter.FilterText(isEntireVerse
                        ? resultItem.VerseText : resultItem.MatchText)).ToList();
                }

                lock (_termFilter)
                {
                    if (_wordListMenuItem.Checked
                    && !_termFilter.IsEmpty)
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                            resultItem => !_termFilter.FilterText(isEntireVerse
                        ? resultItem.VerseText : resultItem.MatchText)).ToList();
                    }
                }

                string searchText = searchMenuTextBox.TextBox.Text.Trim();
                if (searchText.Length > 0)
                {
                    if (searchText.Any(Char.IsUpper))
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VerseText.Contains(searchText)
                                || resultItem.ErrorText.Contains(searchText))).ToList();
                    }
                    else
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VerseText.ToLower().Contains(searchText)
                                || resultItem.ErrorText.ToLower().Contains(searchText))).ToList();
                    }
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

                CheckArea checkArea = CheckArea.CurrentProject;
                if (currentBookAreaMenuItem.Checked)
                {
                    checkArea = CheckArea.CurrentBook;
                }
                else if (currentChapterAreaMenuItem.Checked)
                {
                    checkArea = CheckArea.CurrentChapter;
                }

                _lastResult = _punctuationCheck.RunCheck(checkArea) ?? _lastResult;
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
            _wordListMenuItem = biblicalTermsFiltersMenuItem;
            _ignoreListMenuItem = ignoreListFiltersMenuItem;
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

        private void OnBcvViewMenuClick(object sender, EventArgs e)
        {
            bcvViewMenuItem.Checked = !bcvViewMenuItem.Checked;
            bcvViewMenuItem.CheckState = bcvViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[0].Visible = bcvViewMenuItem.Checked;
        }

        private void OnErrorViewMenuClicked(object sender, EventArgs e)
        {
            errorViewMenuItem.Checked = !errorViewMenuItem.Checked;
            errorViewMenuItem.CheckState = errorViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[3].Visible = errorViewMenuItem.Checked;
        }

        private void OnVerseViewMenuClicked(object sender, EventArgs e)
        {
            verseViewMenuItem.Checked = !verseViewMenuItem.Checked;
            verseViewMenuItem.CheckState = verseViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[2].Visible = verseViewMenuItem.Checked;
        }

        private void OnMatchViewMenuItem_Click(object sender, EventArgs e)
        {
            matchViewMenuItem.Checked = !matchViewMenuItem.Checked;
            matchViewMenuItem.CheckState = matchViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[1].Visible = matchViewMenuItem.Checked;
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private void ClearAreaMenuItems()
        {
            currentProjectAreaMenuItem.CheckState = CheckState.Unchecked;
            currentProjectAreaMenuItem.Checked = false;

            currentBookAreaMenuItem.CheckState = CheckState.Unchecked;
            currentBookAreaMenuItem.Checked = false;

            currentChapterAreaMenuItem.CheckState = CheckState.Unchecked;
            currentChapterAreaMenuItem.Checked = false;
        }

        private void OnCurrentProjectAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentProjectAreaMenuItem.CheckState = CheckState.Checked;
            currentProjectAreaMenuItem.Checked = true;
        }

        private void OnCurrentBookAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentBookAreaMenuItem.CheckState = CheckState.Checked;
            currentBookAreaMenuItem.Checked = true;
        }

        private void OnCurrentChapterAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentChapterAreaMenuItem.CheckState = CheckState.Checked;
            currentChapterAreaMenuItem.Checked = true;
        }

        private void OnEntireVerseFiltersMenuClick(object sender, EventArgs e)
        {
            entireVerseFiltersMenuItem.Checked = !entireVerseFiltersMenuItem.Checked;
            entireVerseFiltersMenuItem.CheckState = entireVerseFiltersMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }
    }
}
