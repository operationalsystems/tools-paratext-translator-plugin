using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using AddInSideViews;
using CsvHelper;
using TvpMain.Check;
using TvpMain.Text;
using TvpMain.Result;
using TvpMain.Filter;
using TvpMain.Project;
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
        private readonly TextCheckRunner _textCheckRunner;

        /// <summary>
        /// List of all checks to be performed.
        /// </summary>
        private readonly IEnumerable<ITextCheck> _allChecks;

        /// <summary>
        /// Ignore list filter.
        /// </summary>
        private readonly IgnoreListTextFilter _ignoreFilter = new IgnoreListTextFilter();

        /// <summary>
        /// Project terms (word list) filter.
        /// </summary>
        private readonly KeyTermsTextFilter _wordListFilter = new KeyTermsTextFilter();

        /// <summary>
        /// Factory terms (biblical) filter.
        /// </summary>
        private readonly KeyTermsTextFilter _biblicalTermFilter = new KeyTermsTextFilter();

        /// <summary>
        /// Reusable progress form.
        /// </summary>
        private readonly ProgressForm _progressForm;

        /// <summary>
        /// Last check result (could be null).
        /// </summary>
        private CheckResults _lastResult;

        /// <summary>
        /// All result items from last result (defaults to empty).
        /// </summary>
        private IList<ResultItem> _allResultItems = Enumerable.Empty<ResultItem>().ToList();

        /// <summary>
        /// Result items from last result, post-filtering (defaults to empty).
        /// </summary>
        private IList<ResultItem> _filteredResultItems;

        /// <summary>
        /// Check contexts, per menu items.
        /// </summary>
        private readonly ISet<PartContext> _checkContexts = new HashSet<PartContext>();

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private readonly ProjectManager _projectManager;

        /// <summary>
        /// Current check area.
        /// </summary>
        private CheckArea _checkArea;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="host">Paratext host interface (required).</param>
        /// <param name="activeProjectName">Active project name (required).</param>
        public MainForm(IHost host, string activeProjectName)
        {
            InitializeComponent();
            Text = $"Translation Validations - Project: \"{activeProjectName}\"";

            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));

            _progressForm = new ProgressForm();
            _progressForm.Cancelled += OnProgressFormCancelled;

            _projectManager = new ProjectManager(host, activeProjectName);
            _textCheckRunner = new TextCheckRunner(_host, _activeProjectName, _projectManager);

            _allChecks = new List<ITextCheck>()
            {
                new MissingSentencePunctuationCheck(_projectManager),
                new ScriptureReferenceCheck(_projectManager)
            };

            _textCheckRunner.CheckUpdated += OnCheckUpdated;

            searchMenuTextBox.TextChanged += OnSearchTextChanged;
            contextMenu.Opening += OnContextMenuOpening;

            _filteredResultItems = _allResultItems;

            UpdateCheckArea();
            UpdateCheckContexts();

            // Background worker to build the biblical term list filter at startup
            // (takes a few seconds, so should not hold up the UI).
            termWorker.DoWork += OnSetupWorkerDoWork;
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
        private void OnSetupWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                lock (_wordListFilter)
                {
                    var projectTerms = _host.GetProjectKeyTerms(
                        _activeProjectName,
                        _host.GetProjectLanguageId(_activeProjectName, "translation validation"));
                    if (projectTerms != null
                        && projectTerms.Count > 0)
                    {
                        _wordListFilter.SetKeyTerms(projectTerms);
                    }
                }

                lock (_biblicalTermFilter)
                {
                    var factoryTerms = _host.GetFactoryKeyTerms(
                        _host.GetProjectKeyTermListType(_activeProjectName),
                        _host.GetProjectLanguageId(_activeProjectName, "translation validation"));
                    if (factoryTerms != null
                        && factoryTerms.Count > 0)
                    {
                        _biblicalTermFilter.SetKeyTerms(factoryTerms);
                    }
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
            _textCheckRunner.CancelChecks();

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
            statusLabel.Text = CheckResults.GetSummaryText(_filteredResultItems);

            foreach (var resultItem in _filteredResultItems)
            {
                dgvCheckResults.Rows.Add(
                    $"{resultItem.PartData.VerseData.VerseLocation.VerseCoordinateText}",
                    $"{resultItem.MatchText}",
                    $"{resultItem.CheckText}",
                    $"{resultItem.ErrorText}");
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
                var isEntireVerse = entireVerseFiltersMenuItem.Checked;

                if (ignoreListFiltersMenuItem.Checked
                    && !_ignoreFilter.IsEmpty)
                {
                    _filteredResultItems = _filteredResultItems.Where(
                        resultItem => !_ignoreFilter.FilterText(isEntireVerse
                        ? resultItem.CheckText : resultItem.MatchText)).ToList();
                }

                // lock in case the background worker hasn't finished yet
                lock (_wordListFilter)
                {
                    if (wordListFiltersMenuItem.Checked
                    && !_wordListFilter.IsEmpty)
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                            resultItem => !_wordListFilter.FilterText(isEntireVerse
                        ? resultItem.CheckText : resultItem.MatchText)).ToList();
                    }
                    if (biblicaTermsFiltersMenuItem.Checked
                        && !_biblicalTermFilter.IsEmpty)
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                            resultItem => !_biblicalTermFilter.FilterText(isEntireVerse
                                ? resultItem.CheckText : resultItem.MatchText)).ToList();
                    }
                }

                var searchText = searchMenuTextBox.TextBox.Text.Trim();
                if (searchText.Length > 0)
                {
                    // upcase chars in search text = 
                    // case-sensitive match, otherwise case-insensitive
                    if (searchText.Any(char.IsUpper))
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.CheckText.Contains(searchText)
                                || resultItem.ErrorText.Contains(searchText))).ToList();
                    }
                    else
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.CheckText.ToLower().Contains(searchText)
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
            if (_checkContexts.Count < 1)
            {
                MessageBox.Show(
                    "Can't run check without a context.\r\n\r\nSelect \"Main Text\", \"Notes\", or both from \"Area\" menu.",
                    "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            dgvCheckResults.Rows.Clear();
            try
            {
                ShowProgress();

                if (_textCheckRunner.RunChecks(_checkArea, _allChecks, _checkContexts, out var nextResults))
                {
                    _lastResult ??= nextResults;
                }
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
            var ignoreListForm = new IgnoreListForm();
            ignoreListForm.IgnoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);

            ignoreListForm.ShowDialog(this);
            var ignoreList = ignoreListForm.IgnoreList;

            HostUtil.Instance.PutIgnoreList(_activeProjectName, ignoreList);
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
            }
        }

        /// <summary>
        /// Word list filter menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnWordListFilterToolMenuClick(object sender, EventArgs e)
        {
            wordListFiltersMenuItem.Checked = !wordListFiltersMenuItem.Checked;
            wordListFiltersMenuItem.CheckState = wordListFiltersMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateMainTable();
        }

        /// <summary>
        /// Biblical term filter menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnBiblicalTermsFilterToolMenuClick(object sender, EventArgs e)
        {
            biblicaTermsFiltersMenuItem.Checked = !biblicaTermsFiltersMenuItem.Checked;
            biblicaTermsFiltersMenuItem.CheckState = biblicaTermsFiltersMenuItem.Checked
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
        private void OnCurrentProjectAreaMenuItemClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentProjectAreaMenuItem.CheckState = CheckState.Checked;
            currentProjectAreaMenuItem.Checked = true;

            UpdateCheckArea();
        }

        /// <summary>
        /// Current book area menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCurrentBookAreaMenuItemClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentBookAreaMenuItem.CheckState = CheckState.Checked;
            currentBookAreaMenuItem.Checked = true;

            UpdateCheckArea();
        }

        /// <summary>
        /// Current chapter area menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCurrentChapterAreaMenuItemClick(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            currentChapterAreaMenuItem.CheckState = CheckState.Checked;
            currentChapterAreaMenuItem.Checked = true;

            UpdateCheckArea();
        }

        /// <summary>
        /// Main text area menu click handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnMainTextAreaMenuItemClick(object sender, EventArgs e)
        {
            mainTextAreaMenuItem.Checked = !mainTextAreaMenuItem.Checked;
            mainTextAreaMenuItem.CheckState = mainTextAreaMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateCheckContexts();
        }

        private void OnIntroductionsAreaMenuItemClick(object sender, EventArgs e)
        {
            introductionsAreaMenuItem.Checked = !introductionsAreaMenuItem.Checked;
            introductionsAreaMenuItem.CheckState = introductionsAreaMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateCheckContexts();
        }

        private void OnOutlinesAreaMenuItemClick(object sender, EventArgs e)
        {
            outlinesAreaMenuItem.Checked = !outlinesAreaMenuItem.Checked;
            outlinesAreaMenuItem.CheckState = outlinesAreaMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateCheckContexts();
        }

        /// <summary>
        /// Notes area menu click handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnNotesAndReferencesAreaMenuItemClick(object sender, EventArgs e)
        {
            notesAndReferencesAreaMenuItem.Checked = !notesAndReferencesAreaMenuItem.Checked;
            notesAndReferencesAreaMenuItem.CheckState = notesAndReferencesAreaMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            UpdateCheckContexts();
        }

        /// <summary>
        /// Builds check context set on ctor or menu item change.
        /// </summary>
        private void UpdateCheckContexts()
        {
            _checkContexts.Clear();
            if (mainTextAreaMenuItem.Checked)
            {
                _checkContexts.Add(PartContext.MainText);
            }
            if (introductionsAreaMenuItem.Checked)
            {
                _checkContexts.Add(PartContext.Introductions);
            }
            if (outlinesAreaMenuItem.Checked)
            {
                _checkContexts.Add(PartContext.Outlines);
            }
            if (notesAndReferencesAreaMenuItem.Checked)
            {
                _checkContexts.Add(PartContext.NoteOrReference);
            }
        }

        /// <summary>
        /// Updates check area.
        /// </summary>
        private void UpdateCheckArea()
        {
            if (currentBookAreaMenuItem.Checked)
            {
                _checkArea = CheckArea.CurrentBook;
            }
            else if (currentChapterAreaMenuItem.Checked)
            {
                _checkArea = CheckArea.CurrentChapter;
            }
            else
            {
                _checkArea = CheckArea.CurrentProject;
            }
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
            using var saveFile = new SaveFileDialog();

            saveFile.FileName = "";
            saveFile.Title = "Save CSV file...";
            saveFile.InitialDirectory = GetFolderPath(SpecialFolder.MyDocuments);
            saveFile.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFile.DefaultExt = "csv";
            saveFile.AddExtension = true;
            saveFile.OverwritePrompt = true;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using var outputStream = saveFile.OpenFile();
                    using var streamWriter = new StreamWriter(outputStream);
                    using var csvWriter = new CsvWriter(streamWriter);

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
        private void OnContextMenuOpening(object sender, CancelEventArgs e)
        {
            var isEnabled = (dgvCheckResults.SelectedRows != null
                             && dgvCheckResults.SelectedRows.Count > 0);

            addToIgnoreList.Enabled = isEnabled;
            removeFromIgnoreList.Enabled = isEnabled;

            if (isEnabled)
            {
                var selectedCount = dgvCheckResults.SelectedRows.Count;

                addToIgnoreList.Text = $"Add {selectedCount:N0} selected to Ignore List...";
                removeFromIgnoreList.Text = $"Remove {selectedCount:N0} selected from Ignore List...";
            }
            else
            {
                addToIgnoreList.Text = "Add to Ignore List...";
                removeFromIgnoreList.Text = "Remove from Ignore List...";
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
            var selectedMatches = matchesToAdd.Count;

            var ignoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);
            foreach (var ignoreItem in ignoreList)
            {
                matchesToAdd.Remove(ignoreItem.CaseSensitiveText);
            }

            if (matchesToAdd.Count > 0)
            {
                foreach (var matchItem in matchesToAdd)
                {
                    ignoreList.Add(new IgnoreListItem(matchItem, false));
                }
                HostUtil.Instance.PutIgnoreList(_activeProjectName, ignoreList);

                var presentItems = (selectedMatches - matchesToAdd.Count);
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
            var selectedMatches = matchesToRemove.Count;

            var oldIgnoreList = HostUtil.Instance.GetIgnoreList(_activeProjectName);
            IList<IgnoreListItem> newIgnoreList = oldIgnoreList.Where(ignoreItem =>
                !matchesToRemove.Contains(ignoreItem.CaseSensitiveText)).ToList();

            var removedItems = (oldIgnoreList.Count - newIgnoreList.Count);
            if (removedItems > 0)
            {
                HostUtil.Instance.PutIgnoreList(_activeProjectName, newIgnoreList);
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
