using AddInSideViews;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Filter;
using TvpMain.Import;
using TvpMain.Project;
using TvpMain.Properties;
using TvpMain.Punctuation;
using TvpMain.Reference;
using TvpMain.Result;
using TvpMain.Text;
using TvpMain.Util;
using static System.Environment;

namespace TvpMain.Forms
{
    /// <summary>
    /// Main translation validation form.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Column index for ignore button.
        /// </summary>
        private const int IGNORE_BUTTON_COLUMN = 6;

        /// <summary>
        /// Semaphore for filter objects, created and managed in the background.
        /// </summary>
        private readonly CountdownEvent _filterSetupEvent = new CountdownEvent(1);

        /// <summary>
        /// Semaphore for check runner-related objects, created and managed in the background.
        /// </summary>
        private readonly CountdownEvent _runnerSetupEvent = new CountdownEvent(1);

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
        private TextCheckRunner _textCheckRunner;

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
        /// Provides access to results.
        /// </summary>
        private readonly ResultManager _resultManager;

        /// <summary>
        /// Project import manager.
        /// </summary>
        private ImportManager _importManager;

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
            Copyright.Text = MainConsts.COPYRIGHT;

            _host = host ?? throw new ArgumentNullException(nameof(host));
            _activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));

            _projectManager = new ProjectManager(_host, _activeProjectName);
            _resultManager = new ResultManager(_host, _activeProjectName);
            _resultManager.ScheduleLoadBooks(_projectManager.PresentBookNums);

            _progressForm = new ProgressForm();
            _progressForm.Cancelled += OnProgressFormCancelled;

            _allChecks = new List<ITextCheck>()
            {
                new MissingSentencePunctuationCheck(_projectManager),
                new ScriptureReferenceCheck(_projectManager)
            };

            searchMenuTextBox.TextChanged += OnSearchTextChanged;
            contextMenu.Opening += OnContextMenuOpening;

            _filteredResultItems = _allResultItems;

            UpdateCheckArea();
            UpdateCheckContexts();

            // Background worker to build the biblical term list filter at startup
            filterSetupWorker.DoWork += OnFilterSetupWorkerDoWork;
            filterSetupWorker.RunWorkerAsync();

            // Background worker to fire up ParatextData
            runnerSetupWorker.DoWork += OnRunnerSetupWorkerDoWork;
            runnerSetupWorker.RunWorkerAsync();
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
        private void OnFilterSetupWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var projectTerms = _host.GetProjectKeyTerms(
                    _activeProjectName,
                    _host.GetProjectLanguageId(_activeProjectName, "translation validation"));
                if (projectTerms != null
                    && projectTerms.Count > 0)
                {
                    _wordListFilter.SetKeyTerms(projectTerms);
                }

                var factoryTerms = _host.GetFactoryKeyTerms(
                    _host.GetProjectKeyTermListType(_activeProjectName),
                    _host.GetProjectLanguageId(_activeProjectName, "translation validation"));
                if (factoryTerms != null
                    && factoryTerms.Count > 0)
                {
                    _biblicalTermFilter.SetKeyTerms(factoryTerms);
                }

                _filterSetupEvent.Signal();
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
                Close();
            }
        }

        /// <summary>
        /// Initializes ParatextData libraries.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnRunnerSetupWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _importManager = new ImportManager(_host, _activeProjectName);

                _textCheckRunner = new TextCheckRunner(_host, _activeProjectName,
                    _projectManager, _importManager, _resultManager);
                _textCheckRunner.CheckUpdated += OnCheckUpdated;

                _runnerSetupEvent.Signal();
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
                Close();
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

            var selectedVerse = GetSelectedGridRow(dgvCheckResults)?.Cells[0].Value.ToString();
            statusLabel.Text = CheckResults.GetSummaryText(_filteredResultItems);

            dgvCheckResults.Enabled = false;

            try
            {
                dgvCheckResults.Rows.Clear();

                foreach (var resultItem in _filteredResultItems)
                {
                    // BCV
                    // Verse (hidden)
                    // Category
                    // Match
                    // Description
                    // Actions - Accept (hidden)
                    // Actions - Ignore

                    var rowIndex = dgvCheckResults.Rows.Add(
                        $"{resultItem.VerseLocation.VerseCoordinateText}",
                        $"{resultItem.VersePart.PartText}",
                        $"{(ScriptureReferenceErrorType)resultItem.ResultTypeCode}",
                        $"{resultItem.MatchText}",
                        $"{resultItem.ErrorText}",
                        "Accept",
                        resultItem.ResultState == ResultState.Ignored ? "Un-Ignore" : "Ignore");

                    dgvCheckResults.Rows[rowIndex].HeaderCell.Value =
                        $"{dgvCheckResults.Rows.Count:N0}";
                    dgvCheckResults.Rows[rowIndex].Tag = resultItem;
                }

                // select previously-selected item (or) first
                SortGridAndSetSelectedRow(dgvCheckResults, selectedVerse);
            }
            finally
            {
                dgvCheckResults.Enabled = true;
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

                if (ignoreListFiltersMenuItem.Checked && !_ignoreFilter.IsEmpty)
                {
                    doFilter(_ignoreFilter, isEntireVerse);
                }

                // block in case the background worker hasn't finished yet
                _filterSetupEvent.Wait();

                if (wordListFiltersMenuItem.Checked && !_wordListFilter.IsEmpty)
                {
                    doFilter(_wordListFilter, isEntireVerse);
                }
                if (biblicaTermsFiltersMenuItem.Checked && !_biblicalTermFilter.IsEmpty)
                {
                    doFilter(_biblicalTermFilter, isEntireVerse);
                }

                // filter ignored
                if (!showIgnoredToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultState != ResultState.Ignored).ToList();
                }

                // filter out loose matches 
                if (hideLooseMatchesToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.LooseFormatting);
                }

                // filter out bad references
                if (hideBadReferencesToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.BadReference);
                }

                // filter out incorrect tag
                if (hideIncorrectTagToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.IncorrectTag);
                }

                // filter out malformed tag
                if (hideMalformedTagToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.MalformedTag);
                }

                // filter out missing tag
                if (hideMissingTagToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.MissingTag);
                }

                // filter out tag shouldn't exist
                if (hideTagShouldntExistToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.TagShouldNotExist);
                }

                // filter out incorrect name style
                if (hideIncorrectNameStyleToolStripMenuItem.Checked)
                {
                    doResultTypeCodeFilter(ScriptureReferenceErrorType.IncorrectNameStyle);
                }

                var searchText = searchMenuTextBox.TextBox.Text.Trim();
                if (searchText.Length > 0)
                {
                    // upcase chars in search text = 
                    // case-sensitive match, otherwise case-insensitive
                    if (searchText.Any(char.IsUpper))
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VersePart.PartText.Contains(searchText)
                                || resultItem.VersePart.ProjectVerse.VerseText.Contains(searchText)
                                || resultItem.MatchText.Contains(searchText)
                                || resultItem.ErrorText.Contains(searchText))).ToList();
                    }
                    else
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VersePart.PartText.ToLower().Contains(searchText)
                                || resultItem.VersePart.ProjectVerse.VerseText.Contains(searchText)
                                || resultItem.MatchText.Contains(searchText)
                                || resultItem.ErrorText.ToLower().Contains(searchText))).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Utility function that updates the _filteredResultItems list based on the given filter and scope
        /// </summary>
        /// <param name="textFilter"></param>
        /// <param name="isEntireVerse"></param>
        private void doFilter(AbstractTextFilter textFilter, Boolean isEntireVerse)
        {
            _filteredResultItems = _filteredResultItems.Where(
                       resultItem => !textFilter.FilterText(isEntireVerse
                   ? resultItem.VersePart.PartText : resultItem.MatchText)).ToList();
        }

        /// <summary>
        /// Utility function that updates the _filteredResultItems list based on the type of error to filter out
        /// </summary>
        /// <param name="scriptureReferenceErrorType"></param>
        private void doResultTypeCodeFilter(ScriptureReferenceErrorType scriptureReferenceErrorType)
        {
            _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)scriptureReferenceErrorType).ToList();
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

                // by default only run the punctuation check
                var checksToRun = _allChecks;

                try
                {
                    // block in case the background worker hasn't finished yet
                    _runnerSetupEvent.Wait();

                    if (_textCheckRunner.RunChecks(
                        _checkArea, checksToRun,
                        _checkContexts, true,
                        out var nextResults))
                    {
                        _allResultItems = nextResults.ResultItems.ToImmutableList();
                    }

                    UpdateMainTable();
                }
                finally
                {
                    HideProgress();
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
        /// File save menu item handler.
        /// 
        /// Saves current results as CSV file.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnFileSaveResultsMenuClick(object sender, EventArgs e)
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
                    using var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture);

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

        /// <summary>
        /// Set up for Scripture Reference check.
        /// 
        /// By default, only enable for punctuation check and hide all the scripture reference check components.
        /// </summary>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            referencesTextBox.SelectionBackColor = Color.Yellow;

            dgvCheckResults.SortCompare += (o, args) =>
            {
                if (args.Column.Index != 0)
                {
                    return;
                }

                var firstResult = (dgvCheckResults.Rows[args.RowIndex1].Tag as ResultItem);
                var secondResult = (dgvCheckResults.Rows[args.RowIndex2].Tag as ResultItem);

                args.SortResult =
                    firstResult.VerseLocation.VerseCoordinate.CompareTo(
                        secondResult.VerseLocation.VerseCoordinate);
                args.Handled = true;
            };
            dgvCheckResults.Sort(
                dgvCheckResults.Columns[0],
                ListSortDirection.Ascending);
        }

        /// <summary>
        /// Gets current grid row, checking first the selected list then the current
        /// row (rows may be _current_ but not _selected_ in data grids).
        /// </summary>
        /// <returns>Current reference list row if found, null otherwise.</returns>
        private DataGridViewRow GetSelectedGridRow(DataGridView gridView)
        {
            if (gridView.RowCount < 1)
            {
                return null;
            }
            else if (gridView.SelectedRows.Count > 0)
            {
                return gridView.SelectedRows[0]
                       ?? gridView.CurrentRow;
            }
            return gridView.CurrentRow;
        }

        /// <summary>
        /// Sorts a grid as needed, scrolls to a row in data grid with supplied BCV text
        /// and selects it, as needed.
        /// 
        /// Used after repopulating the list, as (a) this will cause grids to scroll randomly
        /// and (b) sorting will not be automatic after programmatic changes to row collections.
        /// 
        /// Assumes first column of data grid is BCV text.
        /// </summary>
        /// <param name="gridView">Data grid to work with (required).</param>
        /// <param name="selectedVerse">Selected verse text (optional, may be null; null selects first row).</param>
        private void SortGridAndSetSelectedRow(DataGridView gridView, string selectedVerse)
        {
            // check for nothing to do 
            var maxRows = gridView.RowCount;
            if (maxRows < 1)
            {
                return;
            }

            // sort first, as that changes row order
            if (gridView.SortedColumn != null
                && gridView.SortOrder != SortOrder.None)
            {
                gridView.Sort(
                    gridView.SortedColumn,
                    gridView.SortOrder == SortOrder.Ascending
                        ? ListSortDirection.Ascending
                        : ListSortDirection.Descending);
            }

            // check to see if we need to do anything
            var currSelectedRow = GetSelectedGridRow(gridView);
            if (currSelectedRow != null
                && currSelectedRow.Cells[0].Value.ToString()
                    .Equals(selectedVerse))
            {
                return;
            }

            // default to first row, then find BCV
            var nextSelectedRow = gridView.Rows[0];
            if (selectedVerse != null)
            {
                for (var rowCtr = 0;
                    rowCtr < maxRows;
                    rowCtr++)
                {
                    var rowItem = gridView.Rows[rowCtr];
                    if (!rowItem.Cells[0].Value.ToString()
                        .Equals(selectedVerse))
                    {
                        continue;
                    }

                    nextSelectedRow = rowItem;
                    break;
                }
            }

            // scroll to found row, as needed and select
            if (!nextSelectedRow.Displayed)
            {
                gridView.FirstDisplayedScrollingRowIndex = nextSelectedRow.Index;
            }
            nextSelectedRow.Selected = true;
        }

        /// <summary>
        /// Subtract the occurences of a search char from before an input position.
        ///
        /// Used to deal with rich text control's filtering out '\r' from input text.
        /// </summary>
        /// <param name="inputText">Text to search (required).</param>
        /// <param name="inputPosition">Position to search up to (0-based).</param>
        /// <param name="searchChar">Character to search for.</param>
        /// <returns>Input position minus occurences of search char before it.</returns>
        private static int MinusPrecedingChars(string inputText, int inputPosition, char searchChar)
        {
            var searchPosition = 0;
            if (inputText != null && inputText.Length > 0)
            {
                searchPosition = inputText.IndexOf(searchChar);
            }

            var charCtr = 0;

            while (searchPosition >= 0
                   && searchPosition < inputPosition)
            {
                charCtr++;
                searchPosition = inputText.IndexOf(searchChar, searchPosition + 1);
            }

            return Math.Max(inputPosition - charCtr, 0);
        }

        /// <summary>
        /// Menu checkbox handling 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuItemClick(object sender, EventArgs e)
        {
            UpdateMenuItemState((ToolStripMenuItem)sender);

            UpdateMainTable();
        }

        /// <summary>
        /// Menu checkbox handling for text area selections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuItemClickAndClearArea(object sender, EventArgs e)
        {
            ClearAreaMenuItems();

            UpdateMenuItemState((ToolStripMenuItem)sender);

            UpdateCheckArea();
        }

        /// <summary>
        /// Menu checkbox handling for text context selections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMenuItemClickAndCheckContexts(object sender, EventArgs e)
        {
            UpdateMenuItemState((ToolStripMenuItem)sender);

            UpdateCheckContexts();
        }

        /// <summary>
        /// Helper to set the state of the menu checkboxes
        /// </summary>
        /// <param name="menuItem"></param>
        private void UpdateMenuItemState(ToolStripMenuItem menuItem)
        {
            menuItem.Checked = !menuItem.Checked;
            menuItem.CheckState = menuItem.Checked ? CheckState.Checked : CheckState.Unchecked;
        }

        /// <summary>
        /// Handle Ignore button clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCheckResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("dgvCheckResults_CellContentClick");

            if (e.ColumnIndex == IGNORE_BUTTON_COLUMN)
            {
                //var resultItem = (ResultItem)referencesActionsGridView.Rows[e.RowIndex].Tag;

                if (dgvCheckResults.CurrentRow != null && dgvCheckResults.CurrentRow.Tag != null)
                {

                    var resultItem = (ResultItem)dgvCheckResults.CurrentRow.Tag;

                    if (resultItem != null)
                    {
                        if (e.RowIndex >= 0)
                        {
                            // set ignored state
                            if (resultItem.ResultState == ResultState.Ignored)
                            {
                                resultItem.ResultState = ResultState.Found;
                                dgvCheckResults.Rows[e.RowIndex].Cells[IGNORE_BUTTON_COLUMN].Value = "Ignore";
                            }
                            else
                            {
                                resultItem.ResultState = ResultState.Ignored;
                                dgvCheckResults.Rows[e.RowIndex].Cells[IGNORE_BUTTON_COLUMN].Value = "Un-Ignore";
                            }
                        }

                        _resultManager.SetVerseResult(resultItem);

                        // update ui so that the change is reflected by re-running the filter
                        UpdateMainTable();
                    }
                }
            }

            UpdateReferenceBox();
        }

        /// <summary>
        /// Handle updates of the reference box when cell is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCheckResults_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            UpdateReferenceBox();
        }

        /// <summary>
        /// Handle updates of the reference box when arrow keys are used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCheckResults_KeyPress(object sender, KeyPressEventArgs e)
        {
            UpdateReferenceBox();
        }

        /// <summary>
        /// Handle updates of the reference box when selection changes are made
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCheckResults_SelectionChanged(object sender, EventArgs e)
        {
            UpdateReferenceBox();
        }

        /// <summary>
        /// Update the verse reference box
        /// </summary>
        private void UpdateReferenceBox()
        {
            if (dgvCheckResults.CurrentRow.Tag != null)
            {
                var resultItem = (ResultItem)dgvCheckResults.CurrentRow.Tag;

                if (resultItem != null)
                {
                    referencesTextBox.Text = resultItem.VersePart.ProjectVerse.VerseText;
                    Debug.WriteLine("updateReferencesUIRight - Text Set");

                    referencesTextBox.SelectAll();
                    referencesTextBox.SelectionBackColor = Color.White;

                    referencesTextBox.SelectionStart = MinusPrecedingChars(
                        resultItem.VersePart.ProjectVerse.VerseText,
                        resultItem.MatchStart,
                        '\r');
                    referencesTextBox.SelectionLength = resultItem.MatchLength;
                    referencesTextBox.SelectionBackColor = Color.Yellow;

                    Debug.WriteLine("highlightActiveResultItem - Selection Set: " + resultItem.MatchStart);

                    referencesTextBox.Refresh();
                }

            }

        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string formTitle = $"{pluginName} - End User License Agreement";

            LicenseForm eulaForm = new LicenseForm();
            eulaForm.FormType = LicenseForm.FormTypes.Info;
            eulaForm.FormTitle = formTitle;
            eulaForm.LicenseText = Resources.TVP_EULA;
            eulaForm.OnDismiss = () => eulaForm.Close();
            eulaForm.Show();
        }

        private void checkManagerTestInterfaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckManagerTestInterface checkManagerTestInterface = new CheckManagerTestInterface(_activeProjectName);
            checkManagerTestInterface.Show();
        }

        private void checkResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form checkResults = new CheckResultsForm(_host, _activeProjectName);
            checkResults.ShowDialog();
        }

        private void projectCheckSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form settingsForm = new ProjectCheckSettingsTestForm(_activeProjectName);
            settingsForm.ShowDialog();
        }
    }
}
