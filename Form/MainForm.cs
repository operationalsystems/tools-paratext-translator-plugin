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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Paratext.Data;
using TvpMain.Check;
using TvpMain.Filter;
using TvpMain.Project;
using TvpMain.Punctuation;
using TvpMain.Reference;
using TvpMain.Result;
using TvpMain.Export;
using TvpMain.Import;
using TvpMain.Text;
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
        /// Column index for ignore button.
        /// </summary>
        private const int IGNORE_BUTTON_COLUMN = 2;

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
        /// List with only punctuation check to be performed.
        /// </summary>
        private readonly IEnumerable<ITextCheck> _punctuationCheck;

        /// <summary>
        /// List with only references check to be performed.
        /// </summary>
        private readonly IEnumerable<ITextCheck> _referenceCheck;

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
        /// Project export manager.
        /// </summary>
        private ExportManager _exportManager;

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
            _resultManager = new ResultManager(host, activeProjectName);
            _resultManager.ScheduleLoadBooks(_projectManager.PresentBookNums);

            _allChecks = new List<ITextCheck>()
            {
                new MissingSentencePunctuationCheck(_projectManager),
                new ScriptureReferenceCheck(_projectManager)
            };

            _referenceCheck = new List<ITextCheck>()
            {
                new ScriptureReferenceCheck(_projectManager)
            };

            _punctuationCheck = new List<ITextCheck>()
            {
                new MissingSentencePunctuationCheck(_projectManager)
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
        /// Update the references check UI if it's active instead of the default punctuation UI
        /// </summary>
        private void DoPrimaryUpdate()
        {
            if (referencesCheckMenuItem.Checked)
            {
                UpdateReferencesCheckUi();
            }
            else
            {
                UpdateMainTable();
            }
        }

        /// <summary>
        /// Search text field handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnSearchTextChanged(object sender, EventArgs e)
        {
            DoPrimaryUpdate();
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
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
            }
            finally
            {
                _filterSetupEvent.Signal();
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
                _exportManager = new ExportManager(_host, _activeProjectName,
                    _projectManager, _resultManager);

                _textCheckRunner = new TextCheckRunner(_host, _activeProjectName,
                    _projectManager, _importManager, _resultManager);
                _textCheckRunner.CheckUpdated += OnCheckUpdated;
            }
            catch (Exception ex)
            {
                HostUtil.Instance.ReportError(ex);
            }
            finally
            {
                _runnerSetupEvent.Signal();
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
                    var rowIndex = dgvCheckResults.Rows.Add(
                        $"{resultItem.VerseLocation.VerseCoordinateText}",
                        $"{resultItem.MatchText}",
                        $"{resultItem.VersePart.PartText}",
                        $"{resultItem.ErrorText}");
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

                if (ignoreListFiltersMenuItem.Checked
                    && !_ignoreFilter.IsEmpty)
                {
                    _filteredResultItems = _filteredResultItems.Where(
                        resultItem => !_ignoreFilter.FilterText(isEntireVerse
                        ? resultItem.VersePart.PartText : resultItem.MatchText)).ToList();
                }

                // block in case the background worker hasn't finished yet
                _filterSetupEvent.Wait();

                if (wordListFiltersMenuItem.Checked
                && !_wordListFilter.IsEmpty)
                {
                    _filteredResultItems = _filteredResultItems.Where(
                        resultItem => !_wordListFilter.FilterText(isEntireVerse
                    ? resultItem.VersePart.PartText : resultItem.MatchText)).ToList();
                }
                if (biblicaTermsFiltersMenuItem.Checked
                    && !_biblicalTermFilter.IsEmpty)
                {
                    _filteredResultItems = _filteredResultItems.Where(
                        resultItem => !_biblicalTermFilter.FilterText(isEntireVerse
                            ? resultItem.VersePart.PartText : resultItem.MatchText)).ToList();
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
                                || resultItem.ErrorText.Contains(searchText))).ToList();
                    }
                    else
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VersePart.PartText.ToLower().Contains(searchText)
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

                // by default only run the punctuation check
                var checksToRun = _punctuationCheck;

                // only run the references check if that's the mode
                if (referencesCheckMenuItem.Checked)
                {
                    checksToRun = _referenceCheck;
                }

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

                    DoPrimaryUpdate();
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

            DoPrimaryUpdate();
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

            DoPrimaryUpdate();
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

            DoPrimaryUpdate();
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

            DoPrimaryUpdate();
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

            DoPrimaryUpdate();
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
        /// Project export menu item handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnFileExportProjectMenuItemClick(object sender, EventArgs e)
        {
            using var folderBrowser = new FolderBrowserDialog();

            folderBrowser.Description = "Select destination folder...";
            folderBrowser.SelectedPath = _projectManager.FileManager.ProjectDir.FullName;
            folderBrowser.ShowNewFolderButton = true;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _exportManager.ExportProject(
                        new DirectoryInfo(folderBrowser.SelectedPath),
                        true);
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError($"Can't write export project: {folderBrowser.SelectedPath}", false, ex);
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
                DoPrimaryUpdate();
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
                DoPrimaryUpdate();
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
            // set up the tab control, make the header super small... so as not to be there
            tabControl.Multiline = true;
            tabControl.Appearance = TabAppearance.Buttons;
            tabControl.ItemSize = new System.Drawing.Size(0, 1);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.TabStop = false;

            referencesTextBox.SelectionBackColor = Color.Yellow;

            referencesListView.RowHeadersVisible = false;
            referencesActionsGridView.RowHeadersVisible = false;

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

            referencesListView.SortCompare += (o, args) =>
            {
                if (args.Column.Index < 0
                    || args.Column.Index > 1)
                {
                    return;
                }

                var firstResults = (referencesListView.Rows[args.RowIndex1].Tag as IList<ResultItem>);
                var secondResults = (referencesListView.Rows[args.RowIndex2].Tag as IList<ResultItem>);

                args.SortResult = args.Column.Index switch
                {
                    0 => firstResults[0]
                        .VerseLocation.VerseCoordinate.CompareTo(
                            secondResults[0].VerseLocation.VerseCoordinate),
                    1 => firstResults.Count.CompareTo(
                        secondResults.Count),
                    _ => args.SortResult
                };
                args.Handled = true;
            };
            referencesListView.Sort(
                referencesListView.Columns[0],
                ListSortDirection.Ascending);

            ShowScriptureReferencesCheckControls(false);
        }

        /// <summary>
        /// Switches b/t Scripture Reference check controls or punctuation controls
        /// </summary>
        private void ShowScriptureReferencesCheckControls(bool show)
        {
            missingSentencePunctuationCheckMenuItem.Checked = !show;
            missingSentencePunctuationCheckMenuItem.CheckState = show ? CheckState.Unchecked : CheckState.Checked;

            referencesCheckMenuItem.Checked = show;
            referencesCheckMenuItem.CheckState = show ? CheckState.Checked : CheckState.Unchecked;

            if (show)
            {
                // switch to reference check tab
                tabControl.SelectedTab = referencesTab;
            }
            else
            {
                tabControl.SelectedTab = punctuationTab;
            }

            // show/hide punctuation view menu
            viewMenu.Visible = !show;

            // hide punctuation filter menu items
            punctuationMenuSeparator.Visible = !show;
            wordListFiltersMenuItem.Visible = !show;
            ignoreListFiltersMenuItem.Visible = !show;
            biblicaTermsFiltersMenuItem.Visible = !show;
            entireVerseFiltersMenuItem.Visible = !show;
            statusLabel.Visible = !show;

            // show references filter menu items
            referencesMenuSeparator.Visible = show;
            showIgnoredToolStripMenuItem.Visible = show;
            hideLooseMatchesToolStripMenuItem.Visible = show;
            hideBadReferencesToolStripMenuItem.Visible = show;
            hideMalformedTagToolStripMenuItem.Visible = show;
            hideIncorrectTagToolStripMenuItem.Visible = show;
            hideMissingTagToolStripMenuItem.Visible = show;
            hideTagShouldntExistToolStripMenuItem.Visible = show;
            hideIncorrectNameStyleToolStripMenuItem.Visible = show;

            // hide ignore list button
            btnShowIgnoreList.Visible = !show;
        }

        /// <summary>
        /// Handles selection of controls based on menu selection
        /// </summary>
        private void OnReferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
            ShowScriptureReferencesCheckControls(true);
        }

        /// <summary>
        /// Handles selection of controls based on menu selection
        /// </summary>
        private void OnMissingSentencePunctuationCheckMenuItemClick(object sender, EventArgs e)
        {
            ShowScriptureReferencesCheckControls(false);
        }

        /// <summary>
        /// Kicks off updating the scripture reference controls update with the latest results
        /// </summary>
        private void UpdateReferencesCheckUi()
        {
            var selectedVerse = GetSelectedGridRow(referencesListView)?.Cells[0].Value.ToString();
            FilterReferencesCheckResults();

            referencesTextBox.Text = "";
            referencesActionsGridView.Rows.Clear();

            referencesListView.Enabled = false;

            try
            {
                referencesListView.Rows.Clear();

                // for each verse location in the map, update the left panel, the list of verses
                foreach (var verseGroup in _filteredResultItems
                    .GroupBy(resultItem => resultItem.VerseLocation))
                {
                    var localList = verseGroup.ToImmutableList();
                    var rowIndex = referencesListView.Rows.Add(
                        $"{verseGroup.Key.VerseCoordinateText}",
                        $"{localList.Count}");

                    // keep the list of exceptions with the row for use in the right-hand UI
                    referencesListView.Rows[rowIndex].Tag = localList;
                }

                // select previously-selected item (or) first
                SortGridAndSetSelectedRow(referencesListView, selectedVerse);
            }
            finally
            {
                referencesListView.Enabled = true;
            }

            // update the right portion of the UI, the text and list of exceptions
            UpdateReferencesUiRight();
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
        /// Apply filtering of the results
        /// </summary>
        private void FilterReferencesCheckResults()
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

                // filter ignored
                if (!showIgnoredToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultState != ResultState.Ignored).ToList();
                }

                // filter out loose matches 
                if (hideLooseMatchesToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.LooseFormatting).ToList();
                }

                // filter out bad references
                if (hideBadReferencesToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.BadReference).ToList();
                }

                // filter out incorrect tag
                if (hideIncorrectTagToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.IncorrectTag).ToList();
                }

                // filter out malformed tag
                if (hideMalformedTagToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.MalformedTag).ToList();
                }

                // filter out missing tag
                if (hideMissingTagToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.MissingTag).ToList();
                }

                // filter out tag shouldn't exist
                if (hideTagShouldntExistToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.TagShouldNotExist).ToList();
                }

                // filter out incorrect name style
                if (hideIncorrectNameStyleToolStripMenuItem.Checked)
                {
                    _filteredResultItems = _filteredResultItems.Where(resultItem => resultItem.ResultTypeCode != (int)ScriptureReferenceErrorType.IncorrectNameStyle).ToList();
                }

                var searchText = searchMenuTextBox.TextBox.Text.Trim();
                if (searchText.Length > 0)
                {
                    // upper-case chars in search text = 
                    // case-sensitive match, otherwise case-insensitive
                    if (searchText.Any(char.IsUpper))
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VerseLocation.VerseCoordinateText.Contains(searchText)
                                || resultItem.ErrorText.Contains(searchText))).ToList();
                    }
                    else
                    {
                        _filteredResultItems = _filteredResultItems.Where(
                                resultItem => (resultItem.VerseLocation.VerseCoordinateText.ToLower().Contains(searchText)
                                || resultItem.ErrorText.ToLower().Contains(searchText))).ToList();
                    }
                }
            }
        }

        /// <summary>
        /// Update the exception list and text when the selection changes in the main verse list
        /// </summary>
        private void OnReferencesListViewSelectionChanged(object sender, EventArgs e)
        {
            UpdateReferencesUiRight();
        }

        /// <summary>
        /// Updates the verse level exception list and text, with highlighting
        /// </summary>
        private void UpdateReferencesUiRight()
        {
            Debug.WriteLine("updateReferencesUIRight");
            var selectedRow = GetSelectedGridRow(referencesListView);

            if (selectedRow != null)
            {
                if (selectedRow.Tag is IList<ResultItem> localList
                    && localList.Any())
                {
                    referencesTextBox.Text = localList[0].VersePart.ProjectVerse.VerseText;
                    Debug.WriteLine("updateReferencesUIRight - Text Set");

                    referencesActionsGridView.Enabled = false;

                    try
                    {
                        referencesActionsGridView.Rows.Clear();

                        foreach (var resultItem in localList)
                        {
                            var rowNum = referencesActionsGridView.Rows.Add(
                                $"{(ScriptureReferenceErrorType)resultItem.ResultTypeCode}",
                                "Accept",
                                resultItem.ResultState == ResultState.Ignored
                                    ? "Un-Ignore"
                                    : "Ignore"
                            );

                            Debug.WriteLine("updateReferencesUIRight - Setting Tag resultItem for " + rowNum);

                            // keep the result item with the row of exceptions for future reference
                            referencesActionsGridView.Rows[rowNum].Tag = resultItem;
                        }

                        if (referencesActionsGridView.Rows.Count > 0)
                        {
                            referencesActionsGridView.Rows[0].Selected = true;
                        }
                    }
                    finally
                    {
                        referencesActionsGridView.Enabled = true;
                    }
                }
            }

            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// When something happens in the action grid, make sure the text and highlighting is up-to-date
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReferencesActionsGridViewSelectionChanged(object sender, EventArgs e)
        {
            // highlight the text based on new selection
            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// Toggles the value of the Ignore/Un-Ignore button while saving the state of the result state flag on the associated result item
        /// </summary>
        private void OnReferencesActionsGridViewCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("referencesActionsGridView_CellContentClick");

            if (e.ColumnIndex == IGNORE_BUTTON_COLUMN)
            {
                var resultItem = (ResultItem)referencesActionsGridView.Rows[e.RowIndex].Tag;

                if (resultItem != null)
                {
                    // set ignored state
                    if (resultItem.ResultState == ResultState.Ignored)
                    {
                        resultItem.ResultState = ResultState.Found;
                        referencesActionsGridView.Rows[e.RowIndex].Cells[IGNORE_BUTTON_COLUMN].Value = "Ignore";
                    }
                    else
                    {
                        resultItem.ResultState = ResultState.Ignored;
                        referencesActionsGridView.Rows[e.RowIndex].Cells[IGNORE_BUTTON_COLUMN].Value = "Un-Ignore";
                    }

                    _resultManager.SetVerseResult(resultItem);

                    // update ui so that the change is reflected by re-running the filter
                    DoPrimaryUpdate();
                }
            }

            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// Update highlighting on click
        /// </summary>
        private void OnReferencesActionsGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("referencesActionsGridView_CellClick");

            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// To capture arrow key navigation through list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReferencesActionsGridViewKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("referencesActionsGridView_KeyPress");

            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// To make sure highlighting occurs when the text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReferencesTextBoxTextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("referencesTextBox_TextChanged");

            UpdateExceptionDetailsAndHighlightForActiveResultItem();
        }

        /// <summary>
        /// Highlights the selected result item problem area in the text box.
        /// Update the exception details group box.
        /// </summary>
        private void UpdateExceptionDetailsAndHighlightForActiveResultItem()
        {
            Debug.WriteLine("highlightActiveResultItem");

            if (referencesActionsGridView.CurrentRow == null)
            {
                Debug.WriteLine("highlightActiveResultItem - CurrentRow NULL");

                if (referencesActionsGridView.Rows.Count > 0)
                {
                    Debug.WriteLine("highlightActiveResultItem - Setting Selection");

                    if (referencesActionsGridView.Rows.Count > 0)
                    {
                        referencesActionsGridView.Rows[0].Selected = true;
                    }
                }
            }
            else
            {
                if (referencesActionsGridView.CurrentRow.Tag != null)
                {
                    var resultItem = (ResultItem)referencesActionsGridView.CurrentRow.Tag;
                    if (resultItem != null)
                    {
                        referencesTextBox.SelectAll();
                        referencesTextBox.SelectionBackColor = Color.White;

                        referencesTextBox.SelectionStart = MinusPrecedingChars(
                            resultItem.VersePart.ProjectVerse.VerseText,
                            resultItem.MatchStart,
                            '\r');
                        referencesTextBox.SelectionLength = resultItem.MatchLength;
                        referencesTextBox.SelectionBackColor = Color.Yellow;

                        Debug.WriteLine("highlightActiveResultItem - Selection Set: " + resultItem.MatchStart);

                        issueTextBox.Text = resultItem.MatchText;
                        suggestedFixTextBox.Text = resultItem.SuggestionText;
                        descriptionTextBox.Text = resultItem.ErrorText;

                        referencesTextBox.Refresh();
                    }
                    else
                    {
                        Debug.WriteLine("highlightActiveResultItem - result item NULL - Setting Selection");

                        if (referencesActionsGridView.Rows.Count > 0)
                        {
                            referencesActionsGridView.Rows[0].Selected = true;
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("highlightActiveResultItem - CurrentRow.Tag NULL");

                    if (referencesActionsGridView.Rows.Count > 0)
                    {
                        referencesActionsGridView.Rows[0].Selected = true;
                    }
                }
            }
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
            var searchPosition = inputText.IndexOf(searchChar);
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
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideLooseMatchesToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideLooseMatchesToolStripMenuItem.Checked = !hideLooseMatchesToolStripMenuItem.Checked;
            hideLooseMatchesToolStripMenuItem.CheckState = hideLooseMatchesToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnShowIgnoredToolStripMenuItemClick(object sender, EventArgs e)
        {
            showIgnoredToolStripMenuItem.Checked = !showIgnoredToolStripMenuItem.Checked;
            showIgnoredToolStripMenuItem.CheckState = showIgnoredToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideIncorrectNameStyleToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideIncorrectNameStyleToolStripMenuItem.Checked = !hideIncorrectNameStyleToolStripMenuItem.Checked;
            hideIncorrectNameStyleToolStripMenuItem.CheckState = hideIncorrectNameStyleToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideTagShouldntExistToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideTagShouldntExistToolStripMenuItem.Checked = !hideTagShouldntExistToolStripMenuItem.Checked;
            hideTagShouldntExistToolStripMenuItem.CheckState = hideTagShouldntExistToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideMissingTagToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideMissingTagToolStripMenuItem.Checked = !hideMissingTagToolStripMenuItem.Checked;
            hideMissingTagToolStripMenuItem.CheckState = hideMissingTagToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideIncorrectTagToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideIncorrectTagToolStripMenuItem.Checked = !hideIncorrectTagToolStripMenuItem.Checked;
            hideIncorrectTagToolStripMenuItem.CheckState = hideIncorrectTagToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideMalformedTagToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideMalformedTagToolStripMenuItem.Checked = !hideMalformedTagToolStripMenuItem.Checked;
            hideMalformedTagToolStripMenuItem.CheckState = hideMalformedTagToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }

        /// <summary>
        /// Filter checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHideBadReferencesToolStripMenuItemClick(object sender, EventArgs e)
        {
            hideBadReferencesToolStripMenuItem.Checked = !hideBadReferencesToolStripMenuItem.Checked;
            hideBadReferencesToolStripMenuItem.CheckState = hideBadReferencesToolStripMenuItem.Checked
                ? CheckState.Checked : CheckState.Unchecked;

            DoPrimaryUpdate();
        }
    }
}
