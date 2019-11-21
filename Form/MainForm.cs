using AddInSideViews;
using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Threading;
using TvpMain.Check;
using TvpMain.Data;
using TvpMain.Filter;
using TvpMain.Form;
using TvpMain.Util;
using static System.Environment;

namespace TvpMain.Form
{
    /// <summary>
    /// Main translation validation form.
    /// </summary>
    public partial class MainForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private readonly string _activeProjectName;

        /// <summary>
        /// Punctuation check.
        /// 
        /// Only check currently implemented. This will grow to an aggregate model of some kind
        /// (maybe even just a list of checks, run in series or parallel). 
        /// </summary>
        private readonly RegexPunctuationCheck1 _punctuationCheck;

        /// <summary>
        /// Ignore list filter.
        /// </summary>
        private readonly IgnoreListTextFilter _ignoreFilter;

        /// <summary>
        /// Biblical terms filter.
        /// </summary>
        private readonly BiblicalTermsTextFilter _termFilter;

        /// <summary>
        /// Reusable progress form.
        /// </summary>
        private readonly ProgressForm _progressForm;

        /// <summary>
        /// Last check result (could be null).
        /// </summary>
        private CheckResult _lastResult;

        /// <summary>
        /// All result items from last result (defaults to empty).
        /// </summary>
        private IList<ResultItem> _allResultItems;

        /// <summary>
        /// Result items from last result, post-filtering (defaults to empty).
        /// </summary>
        private IList<ResultItem> _filteredResultItems;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public MainForm(IHost host, string activeProjectName)
        {
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
            contextMenu.Opening += OnContextMenuOpening;

            _allResultItems = Enumerable.Empty<ResultItem>().ToList();
            _filteredResultItems = _allResultItems;

            // Background worker to build the biblical term list filter at startup
            // (takes a few seconds, so should not hold up the UI).
            termWorker.DoWork += OnTermWorkerDoWork;
            termWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Search text field handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            UpdateMainTable();
        }

        /// <summary>
        /// Background worker handler to build biblical terms list filter.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnTermWorkerDoWork(object sender, DoWorkEventArgs e)
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

        /// <summary>
        /// Progress form cancelled observer.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnProgressFormCancelled(object sender, EventArgs e)
        {
            dgvCheckResults.Rows.Clear();
            _punctuationCheck.CancelCheck();

            HideProgress();
        }

        /// <summary>
        /// Punctuation check update forwarder.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="updatedArgs">Check update details.</param>
        private void OnCheckUpdated(object sender, CheckUpdatedArgs updatedArgs)
        {
            _progressForm.OnCheckUpdated(updatedArgs);
        }

        /// <summary>
        /// Re-filter and update main table with filtered results.
        /// </summary>
        private void UpdateMainTable()
        {
            FilterResults();

            dgvCheckResults.Rows.Clear();
            statusLabel.Text = CheckResult.GetSummaryText(_filteredResultItems);

            foreach (ResultItem resultItem in _filteredResultItems)
            {
                dgvCheckResults.Rows.Add(new string[] {
                            $"{resultItem.BcvText}",
                            $"{resultItem.MatchText}",
                            $"{resultItem.VerseText}",
                            $"{resultItem.ErrorText}"
                        });
                dgvCheckResults.Rows[(dgvCheckResults.Rows.Count - 1)].HeaderCell.Value =
                    $"{dgvCheckResults.Rows.Count:N0}";
                dgvCheckResults.Rows[(dgvCheckResults.Rows.Count - 1)].Tag = resultItem;
            }
        }

        /// <summary>
        /// Filter results vs selected filters and any search text.
        /// </summary>
        private void FilterResults()
        {
            // no results = empty
            if (_allResultItems == null)
            {
                _allResultItems = Enumerable.Empty<ResultItem>().ToList();
                _filteredResultItems = _allResultItems;
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

                // lock in case the background worker hasn't finished yet
                lock (_termFilter)
                {
                    if (biblicalTermsFiltersMenuItem.Checked
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
                    // upcase chars in search text = 
                    // case-sensitive match, otherwise case-insensitive
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

        /// <summary>
        /// Show progress form and reset contents.
        /// </summary>
        private void ShowProgress()
        {
            _progressForm.ResetForm();

            Enabled = false;
            _progressForm.Show(this);
        }

        /// <summary>
        /// Hide progress form and reactivate this form.
        /// </summary>
        private void HideProgress()
        {
            _progressForm.Hide();

            Enabled = true;
            Activate();
        }

        /// <summary>
        /// Handle run check button click.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
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

        /// <summary>
        /// Ignore list button click handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnClickIgnoreList(object sender, EventArgs e)
        {
            IgnoreListForm ignoreListForm = new IgnoreListForm();
            ignoreListForm.IgnoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);

            ignoreListForm.ShowDialog(this);
            IList<IgnoreListItem> ignoreList = ignoreListForm.IgnoreList;

            HostUtil.Instance.PutIgnoreListItems(_activeProjectName, ignoreList);
            _ignoreFilter.SetIgnoreList(ignoreList);

            UpdateMainTable();
        }

        /// <summary>
        /// Form closing handler (asks for confirmation).
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (used to cancel close).</param>
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

        /// <summary>
        /// Biblical term filter menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnBiblicalTermListToolMenuClick(object sender, EventArgs e)
        {
            biblicalTermsFiltersMenuItem.Checked = !biblicalTermsFiltersMenuItem.Checked;
            biblicalTermsFiltersMenuItem.CheckState = biblicalTermsFiltersMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }

        /// <summary>
        /// Ignore list filter menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void IgnoreListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ignoreListFiltersMenuItem.Checked = !ignoreListFiltersMenuItem.Checked;
            ignoreListFiltersMenuItem.CheckState = ignoreListFiltersMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }

        /// <summary>
        /// BCV column view menu handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnBcvViewMenuClick(object sender, EventArgs e)
        {
            bcvViewMenuItem.Checked = !bcvViewMenuItem.Checked;
            bcvViewMenuItem.CheckState = bcvViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[0].Visible = bcvViewMenuItem.Checked;
        }

        /// <summary>
        /// Error column view menu handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnErrorViewMenuClicked(object sender, EventArgs e)
        {
            errorViewMenuItem.Checked = !errorViewMenuItem.Checked;
            errorViewMenuItem.CheckState = errorViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[3].Visible = errorViewMenuItem.Checked;
        }

        /// <summary>
        /// Verse column view menu handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnVerseViewMenuClicked(object sender, EventArgs e)
        {
            verseViewMenuItem.Checked = !verseViewMenuItem.Checked;
            verseViewMenuItem.CheckState = verseViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[2].Visible = verseViewMenuItem.Checked;
        }

        /// <summary>
        /// Match column view menu handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnMatchViewMenuItem_Click(object sender, EventArgs e)
        {
            matchViewMenuItem.Checked = !matchViewMenuItem.Checked;
            matchViewMenuItem.CheckState = matchViewMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            dgvCheckResults.Columns[1].Visible = matchViewMenuItem.Checked;
        }

        /// <summary>
        /// Close button button handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Utility method to clear all area options (simplifies mutual exclusivity).
        /// </summary>
        private void ClearAreaMenuItems()
        {
            currentProjectAreaMenuItem.CheckState = CheckState.Unchecked;
            currentProjectAreaMenuItem.Checked = false;

            currentBookAreaMenuItem.CheckState = CheckState.Unchecked;
            currentBookAreaMenuItem.Checked = false;

            currentChapterAreaMenuItem.CheckState = CheckState.Unchecked;
            currentChapterAreaMenuItem.Checked = false;
        }

        /// <summary>
        /// Current project area menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCurrentProjectAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentProjectAreaMenuItem.CheckState = CheckState.Checked;
            currentProjectAreaMenuItem.Checked = true;
        }

        /// <summary>
        /// Current book area menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCurrentBookAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentBookAreaMenuItem.CheckState = CheckState.Checked;
            currentBookAreaMenuItem.Checked = true;
        }

        /// <summary>
        /// Current chapter area menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCurrentChapterAreaMenuClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentChapterAreaMenuItem.CheckState = CheckState.Checked;
            currentChapterAreaMenuItem.Checked = true;
        }

        /// <summary>
        /// Entire verse menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnEntireVerseFiltersMenuClick(object sender, EventArgs e)
        {
            entireVerseFiltersMenuItem.Checked = !entireVerseFiltersMenuItem.Checked;
            entireVerseFiltersMenuItem.CheckState = entireVerseFiltersMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }

        /// <summary>
        /// File save menu item handler.
        /// 
        /// Saves current results as CSV file.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnFileSaveMenuClick(object sender, EventArgs e)
        {
            using SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.FileName = $"";
            saveFile.Title = "Save CSV file...";
            saveFile.InitialDirectory = Environment.GetFolderPath(SpecialFolder.MyDocuments);
            saveFile.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFile.DefaultExt = "csv";
            saveFile.AddExtension = true;
            saveFile.OverwritePrompt = true;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using Stream outputStream = saveFile.OpenFile();
                    using StreamWriter streamWriter = new StreamWriter(outputStream);
                    using CsvWriter csvWriter = new CsvWriter(streamWriter);

                    csvWriter.WriteRecords(_filteredResultItems);
                    csvWriter.Flush();
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError($"Can't write CSV file: {saveFile.FileName}", false, ex);
                }
            }
        }

        /// <summary>
        /// Context menu pre-open handler, to set text.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool isEnabled = (dgvCheckResults.SelectedRows != null
                                && dgvCheckResults.SelectedRows.Count > 0);

            addToIgnoreList.Enabled = isEnabled;
            removeFromIgnoreList.Enabled = isEnabled;

            if (isEnabled)
            {
                int selectedCount = dgvCheckResults.SelectedRows.Count;

                addToIgnoreList.Text = $"Add {selectedCount:N0} selected to Ignore List...";
                removeFromIgnoreList.Text = $"Remove {selectedCount:N0} selected from Ignore List...";
            }
            else
            {
                addToIgnoreList.Text = $"Add to Ignore List...";
                removeFromIgnoreList.Text = $"Remove from Ignore List...";
            }
        }

        /// <summary>
        /// Add to ignore list context menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnAddToIgnoreListMenuClick(object sender, EventArgs e)
        {
            ISet<string> matchesToAdd = new HashSet<string>();
            foreach (DataGridViewRow rowItem in dgvCheckResults.SelectedRows)
            {
                matchesToAdd.Add((rowItem.Tag as ResultItem).MatchText);
            }
            int selectedMatches = matchesToAdd.Count;

            IList<IgnoreListItem> ignoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);
            foreach (IgnoreListItem ignoreItem in ignoreList)
            {
                matchesToAdd.Remove(ignoreItem.CaseSensitiveText);
            }

            if (matchesToAdd.Count > 0)
            {
                foreach (string matchItem in matchesToAdd)
                {
                    ignoreList.Add(new IgnoreListItem(matchItem, false));
                }
                HostUtil.Instance.PutIgnoreListItems(_activeProjectName, ignoreList);

                int presentItems = (selectedMatches - matchesToAdd.Count);
                if (presentItems == 0)
                {
                    MessageBox.Show(matchesToAdd.Count > 1
                        ? $"Added {matchesToAdd.Count:N0} unique matches to ignore list (all new)."
                        : $"Added {matchesToAdd.Count:N0} unique match to ignore list.");
                }
                else
                {
                    MessageBox.Show(matchesToAdd.Count > 1
                        ? $"Added {matchesToAdd.Count:N0} unique matches to ignore list ({presentItems:N0} already present)."
                        : $"Added {matchesToAdd.Count:N0} unique match to ignore list ({presentItems:N0} already present).");
                }

                _ignoreFilter.SetIgnoreList(ignoreList);
                UpdateMainTable();
            }
            else
            {
                MessageBox.Show("All selected matches already added to ignore list.");
            }
        }

        /// <summary>
        /// Remove from ignore list context menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnRemoveFromIgnoreListMenuClick(object sender, EventArgs e)
        {
            ISet<string> matchesToRemove = new HashSet<string>();
            foreach (DataGridViewRow rowItem in dgvCheckResults.SelectedRows)
            {
                matchesToRemove.Add((rowItem.Tag as ResultItem).MatchText);
            }
            int selectedMatches = matchesToRemove.Count;

            IList<IgnoreListItem> oldIgnoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);
            IList<IgnoreListItem> newIgnoreList = oldIgnoreList.Where(ignoreItem =>
                !matchesToRemove.Contains(ignoreItem.CaseSensitiveText)).ToList();

            int removedItems = (oldIgnoreList.Count - newIgnoreList.Count);
            if (removedItems > 0)
            {
                HostUtil.Instance.PutIgnoreListItems(_activeProjectName, newIgnoreList);
                MessageBox.Show(removedItems > 1
                    ? $"Removed {removedItems:N0} matching items from ignore list."
                    : $"Removed {removedItems:N0} matching item from ignore list.");

                _ignoreFilter.SetIgnoreList(oldIgnoreList);
                UpdateMainTable();
            }
            else
            {
                MessageBox.Show("All selected matches already added to ignore list.");
            }
        }
    }
}
