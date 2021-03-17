using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TvpMain.Check;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TvpMain.Import;
using TvpMain.Project;
using TvpMain.Text;
using TvpMain.Util;
using static TvpMain.Check.CheckAndFixItem;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
        // Deny Button Text
        readonly string DENY_BUTTON_TEXT = "Deny";
        readonly string UNDENY_BUTTON_TEXT = "Un-Deny";

        // A list of <c>CheckResultItem</c>s which have been denied
        private List<int> _denied;

        /// This index indicates to the progress form that the project scope checks are being run.
        /// </summary>
        public const int ALL_PROJECTS_INDEX = -1;

        /// <summary>
        /// This index indicates a scope for BCV is not applicable.
        /// </summary>
        public const int SCOPE_NOT_APPLICABLE = -1;

        /// <summary>
        /// This index indicates no row is selected
        /// </summary>
        public const int DEFAULT_ROW_NOT_SELECTED = -1;

        /// <summary>
        /// Lock for updating check results dictionary.
        /// </summary>
        private readonly object CheckResultslock = new object();

        /// <summary>
        /// Cancellation token source.
        /// </summary>
        private CancellationTokenSource RunCancellationTokenSource;

        /// <summary>
        /// Current run's task semaphore.
        /// </summary>
        private SemaphoreSlim RunSemaphore;

        /// <summary>
        /// The Paratext plugin host.
        /// </summary>
        private IHost Host { get; set; }

        /// <summary>
        /// Active project name.
        /// </summary>
        private string ActiveProjectName { get; set; }

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager ProjectManager { get; set; }

        /// <summary>
        /// A collection of the <c>CheckAndFixItem</c>s to run against the content.
        /// </summary>
        private List<CheckAndFixItem> ChecksToRun { get; set; }

        /// <summary>
        /// The checks organized by check scope.
        /// </summary>
        private Dictionary<CheckScope, List<CheckAndFixItem>> ChecksByScope { get; set; }

        /// <summary>
        /// The context of what we're checking against.
        /// </summary>
        private CheckRunContext CheckRunContext { get; set; }

        /// <summary>
        /// The Check and Fix executor.
        /// </summary>
        private CheckAndFixRunner CheckRunner { get; set; }

        /// <summary>
        /// The <c>ImportManager</c> used to read in the scripture text.
        /// </summary>
        private ImportManager ImportManager { get; set; }

        /// <summary>
        /// Progress of current run (1-based indexing).
        /// </summary>
        private int RunBookCtr;

        /// <summary>
        /// The collection of <c>CheckResultItem</c>s mapped by the associated <c>CheckAndFixItem</c>.
        /// </summary>
        Dictionary<CheckAndFixItem, List<CheckResultItem>> CheckResults { get; set; } = new Dictionary<CheckAndFixItem, List<CheckResultItem>>();

        /// <summary>
        /// The collection for holding project content in the correct book order.
        /// </summary>
        ConcurrentDictionary<int, StringBuilder> ProjectSb { get; set; } = new ConcurrentDictionary<int, StringBuilder>();

        /// <summary>
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Reusable progress form.
        /// </summary>
        private readonly ProgressForm ProgressForm;

        /// <summary>
        /// The list of selected books to check
        /// </summary>
        private BookNameItem[] SelectedBooks { get; set; }

        /// <summary>
        /// This form displays information about the checks which have been run against a project
        /// as well as options for viewing and fixing any found errors.
        /// </summary>
        /// <param name="host">The Paratext Host object.</param>
        /// <param name="activeProjectName">The name of the current project</param>
        /// <param name="projectManager">The project manager, to get to book information</param>
        /// <param name="selectedBooks">The selected books when the filter was run</param
        /// <param name="checksToRun">The list of checks to run against the content.</param>
        /// <param name="checkRunContext">The context of what Bible content we're checking against.</param>
        /// <param name="checkRunner">The <c>CheckAndFixRunner</c> that will execute the checks against supplied content.</param>
        public CheckResultsForm(
            IHost host,
            string activeProjectName,
            ProjectManager projectManager,
            BookNameItem[] selectedBooks,
            List<CheckAndFixItem> checksToRun,
            CheckRunContext checkRunContext)
        {
            // validate inputs
            Host = host ?? throw new ArgumentNullException(nameof(host));
            if (activeProjectName == null || activeProjectName.Length < 1)
            {
                throw new ArgumentNullException(nameof(activeProjectName));
            }
            ActiveProjectName = activeProjectName;
            ProjectManager = projectManager ?? throw new ArgumentNullException(nameof(projectManager));
            SelectedBooks = selectedBooks ?? throw new ArgumentNullException(nameof(selectedBooks));
            ChecksToRun = checksToRun ?? throw new ArgumentNullException(nameof(checksToRun));
            CheckRunContext = checkRunContext ?? throw new ArgumentNullException(nameof(checkRunContext));
            
            ProgressForm = initializeProgressForm();
            CheckUpdated += OnCheckUpdated;

            // initialize the components
            InitializeComponent();
        }

        /// <summary>
        /// Creates a <c>ProgressForm</c> with default values
        /// </summary>
        private ProgressForm initializeProgressForm()
        {
            ProgressForm progressForm = new ProgressForm();
            progressForm.StartPosition = FormStartPosition.CenterParent;
            progressForm.Cancelled += OnProgressFormCancelled;
            return progressForm;
        }

        /// <summary>
        /// Set the default set of books, all of them
        /// </summary>
        private void setSelectedBooks()
        {
            string selectedBooksString = BookSelection.stringFromSelectedBooks(SelectedBooks);
            bookFilterTextBox.Text = selectedBooksString;
        }

        /// <summary>
        /// This method loads the denied <c>CheckResultItem</c>s from the project.
        /// </summary>
        private void LoadDeniedResults()
        {
            _denied = Util.HostUtil.Instance.GetProjectDeniedResults(ActiveProjectName);
        }

        /// <summary>
        /// This method saves the denied <c>CheckResultItem</c>s to the project.
        /// </summary>
        private void SaveDeniedResults()
        {
            Util.HostUtil.Instance.PutProjectDeniedResults(ActiveProjectName, _denied);
        }

        /// <summary>
        /// This function will cancel the execution of the checks.
        /// </summary>
        public void CancelChecks()
        {
            RunCancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// This function notifies event listeners that a book has been updated.
        /// </summary>
        /// <param name="updateBookNum">The number of the book that finished being checked.</param>
        private void OnCheckUpdated(int updateBookNum)
        {
            lock (CheckResultslock)
            {
                // we increased the max number of books to account for the project. We pass -1 as the book num to represent the project.
                RunBookCtr++;
                CheckUpdated?.Invoke(this,
                    new CheckUpdatedArgs(CheckRunContext.Books.Count() + 1, RunBookCtr, updateBookNum));
            }
        }

        /// <summary>
        /// Punctuation check update forwarder.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="updatedArgs">Check update details.</param>
        private void OnCheckUpdated(object sender, CheckUpdatedArgs updatedArgs)
        {
            ProgressForm.OnCheckUpdated(updatedArgs);
        }

        /// <summary>
        /// This function will lay the foundations for running the checks, executing them asynchronously, and monitoring them.
        /// </summary>
        public void RunChecks()
        {
            CheckRunContext.Validate();
            if (ChecksToRun.Count <= 0)
            {
                MessageBox.Show(
                    "No checks provided.",
                    "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Hide();
                return;
            }

            // Show the progress bar as we kick off our work.
            ShowProgress();

            // clear the previous results
            CheckResults.Clear();

            // Pre-filter the checks and fixes so we don't have to do it repeatedly later.
            ChecksByScope = new Dictionary<CheckScope, List<CheckAndFixItem>>()
                {
                    { CheckScope.PROJECT, new List<CheckAndFixItem>() },
                    { CheckScope.BOOK, new List<CheckAndFixItem>() },
                    { CheckScope.CHAPTER, new List<CheckAndFixItem>() },
                    { CheckScope.VERSE, new List<CheckAndFixItem>() },
                };

            ChecksToRun.Aggregate(ChecksByScope, (acc, check) =>
            {
                acc[check.Scope].Add(check);
                return acc;
            });

            // set up semaphore and cancellation token to control execution and termination
            RecycleRunEntities(true);

            // content builders for performing scoped checks.
            var books = CheckRunContext.Books;

            // set up semaphore for parallelism control, results set, and task list
            var taskList = new List<Task>();
            taskList.AddRange(books.Select(book => RunBookCheckTask(book.BookNum)));

            try
            {

                var workThread = new Thread(() =>
                {
                    try
                    {
                        Task.WaitAll(taskList.ToArray(), RunCancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore (can occur w/cancel).
                    }
                    catch (Exception ex)
                    {
                        var messageText =
                            $"Error: Can't check project: \"{CheckRunContext.Project}\" (error: {ex.Message}).";

                        var _runEx = new TextCheckException(messageText, ex);
                        HostUtil.Instance.ReportError(messageText, _runEx);
                    }
                })
                { IsBackground = true };
                workThread.Start();

                // busy-wait until helper thread is done,
                // keeping the UI responsive w/DoEvents()
                while (workThread.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(MainConsts.CHECK_EVENTS_DELAY_IN_MSEC);

                    if (RunCancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }
                }

                // once all the books are checked, let's check the accumulated project content (ordered by book).
                var sortedKeys = ProjectSb.Keys.ToList();
                sortedKeys.Sort();

                var finalSb = new StringBuilder();
                foreach (var key in sortedKeys)
                {
                    finalSb.Append(ProjectSb[key].ToString());
                }

                ExecuteChecksAndStoreResults(finalSb.ToString(), ChecksByScope[CheckScope.PROJECT], SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);
                OnCheckUpdated(ALL_PROJECTS_INDEX);
            }
            finally
            {
                HideProgress();
            }

            // populate the checks results table
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// This function will execute the specified checks against the provided content and store the results.
        /// </summary>
        /// <param name="content">The content to check against.</param>
        /// <param name="checksToRun">The list of checks to assess the content.</param>
        /// <param name="book">The current book (1-based).</param>
        /// <param name="chapter">The current chapter (1-based).</param>
        /// <param name="verse">The current verse (1-based).</param>
        private void ExecuteChecksAndStoreResults(string content, List<CheckAndFixItem> checksToRun, int book, int chapter, int verse)
        {
            // check the content with every specified check
            checksToRun.ForEach(check =>
            {
                List<CheckResultItem> results = CheckRunner.ExecCheckAndFix(content, check);

                foreach(CheckResultItem item in results)
                {
                    item.Book = book;
                    item.Chapter = chapter;
                    item.Verse = verse;
                    item.Reference = content;
                }

                lock (CheckResultslock)
                {
                    // add or append results
                    if (CheckResults.ContainsKey(check))
                    {
                        CheckResults[check].AddRange(results);
                    }
                    else
                    {
                        CheckResults.Add(check, results);
                    }
                }
            });
        }

        /// <summary>
        /// This function creates and runs a check against a given book number.
        /// </summary>
        /// <param name="inputBookNum">Book number (1-based).</param>
        private Task RunBookCheckTask(
            int inputBookNum)
        {
            return Task.Run(() =>
            {
                // wait to get started
                RunSemaphore.Wait();

                if (!ProjectSb.TryAdd(inputBookNum, new StringBuilder()))
                {
                    throw new ArgumentException($"There's already an entry for book {inputBookNum} in {nameof(ProjectSb)}");
                }
                var bookSb = new StringBuilder();

                // track where we are for error reporting
                var currBookNum = inputBookNum;
                var currChapterNum = 0;
                var currVerseNum = 0;

                try
                {
                    // get utility items
                    var versificationName = Host.GetProjectVersificationName(CheckRunContext.Project);

                    // needed to track empty chapters
                    var emptyVerseCtr = 0;

                    // determine chapter range using check area and user's location in Paratext
                    var minChapter = (CheckRunContext.CheckScope == CheckScope.CHAPTER)
                            ? CheckRunContext.Chapters.Min()
                            : 1;
                    var maxChapter = (CheckRunContext.CheckScope == CheckScope.CHAPTER)
                        ? CheckRunContext.Chapters.Max()
                        : Host.GetLastChapter(currBookNum, versificationName);

                    // iterate chapters
                    for (var chapterNum = minChapter;
                            chapterNum <= maxChapter;
                            chapterNum++)
                    {
                        var chapterSb = new StringBuilder();
                        currChapterNum = chapterNum;

                        // iterate verses
                        var lastVerseNum = Host.GetLastVerse(currBookNum, chapterNum, versificationName);
                        for (var verseNum = 0; // verse 0 = intro text
                            verseNum <= lastVerseNum;
                            verseNum++)
                        {
                            currVerseNum = verseNum;

                            try
                            {
                                // Check if the parallel.ForEach has been cancelled. The verse is the smallest entitity granularity
                                RunCancellationTokenSource.Token.ThrowIfCancellationRequested();

                                var verseLocation = new VerseLocation(currBookNum, chapterNum, verseNum);
                                var verseText = ImportManager.Extract(verseLocation);

                                // empty text = consecutive check, in case we're looking at an empty chapter
                                if (string.IsNullOrWhiteSpace(verseText))
                                {
                                    emptyVerseCtr++;
                                    if (emptyVerseCtr > MainConsts.MAX_CONSECUTIVE_EMPTY_VERSES)
                                    {
                                        break; // no beginning text = empty chapter (skip)
                                    }
                                }
                                // else, take apart text
                                emptyVerseCtr = 0;

                                var verseData = new ProjectVerse(verseLocation, verseText);

                                // build the scoped strings
                                ProjectSb[inputBookNum].AppendLine(verseText);
                                bookSb.AppendLine(verseText);
                                chapterSb.AppendLine(verseText);

                                // check the verse with verse checks
                                ExecuteChecksAndStoreResults(verseText, ChecksByScope[CheckScope.VERSE], currBookNum, currChapterNum, currVerseNum);
                            }
                            catch (ArgumentException)
                            {
                                // arg exceptions occur when verses are missing, 
                                // which they can be for given translations (ignore and move on)
                                // TODO spit out warning
                                continue;
                            }
                        }

                        // check the chapter with chapter checks
                        ExecuteChecksAndStoreResults(chapterSb.ToString(), ChecksByScope[CheckScope.CHAPTER], currBookNum, currChapterNum, SCOPE_NOT_APPLICABLE);
                    }

                    // check the book with book checks
                    ExecuteChecksAndStoreResults(bookSb.ToString(), ChecksByScope[CheckScope.BOOK], currBookNum, SCOPE_NOT_APPLICABLE, SCOPE_NOT_APPLICABLE);

                    OnCheckUpdated(inputBookNum);
                }
                catch (OperationCanceledException)
                {
                    // A cancel occurred. We can ignore.
                }
                catch (Exception ex)
                {
                    var messageText =
                        $"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{CheckRunContext.Project}\" (error: {ex.Message}).";

                    HostUtil.Instance.ReportError(messageText, ex);
                }
                finally
                {
                    RunSemaphore.Release();
                }
            });
        }

        /// <inheritdoc />
        public new virtual void Dispose()
        {
            RecycleRunEntities(false);
            base.Dispose();
        }

        /// <summary>
        /// Populate the results into the UI results data grid.
        /// </summary>
        protected void PopulateChecksDataGridView()
        {
            int currentSelectedRowIndex = 0;

            if (checksDataGridView.CurrentCell != null)
            {
                currentSelectedRowIndex = checksDataGridView.SelectedRows[0].Index;
            }

            checksDataGridView.Rows.Clear();

            foreach (KeyValuePair<CheckAndFixItem, List<CheckResultItem>> result in CheckResults)
            {
                if (IfCheckFixItemIsNotFiltered(result.Key))
                {
                    List<CheckResultItem> filteredResultItems = FilterResults(result.Value);

                    int rowIndex = checksDataGridView.Rows.Add(new object[] {
                        false,                                      // selected
                        result.Key.Name.Trim(),                     // category
                        result.Key.Description.Trim(),              // description
                        filteredResultItems.Count                   // count
                    });
                    checksDataGridView.Rows[rowIndex].Tag = new KeyValuePair<CheckAndFixItem,List<CheckResultItem>>(result.Key, filteredResultItems);
                }
            }

            // ensure that there are rows to select
            if(checksDataGridView.Rows != null && checksDataGridView.Rows.Count > 0 && checksDataGridView.Rows[0] != null)
            {
                checksDataGridView.Rows[currentSelectedRowIndex].Selected = true;
            }

            // Send in the currently selected row. This is needed because this value is incorrect from the control
            // itself until the UI thread has actually updated the control and the event has been triggered.
            // However, we can't rely on that event to update the issues list because it dosn't trigger in every case.
            PopulateIssuesDataGridView(currentSelectedRowIndex);
            
        }

        /// <summary>
        /// Checks if the text filter matches anything in the name or description of the checkFixItem
        /// </summary>
        /// <param name="checkAndFixItem">The item to be checked for matches</param>
        /// <returns>If the item has a match and is not filtered out</returns>
        private bool IfCheckFixItemIsNotFiltered(CheckAndFixItem checkAndFixItem)
        {
            if(String.IsNullOrEmpty(filterTextBox.Text)) {
                return true;
            }
            return checkAndFixItem.Name.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                checkAndFixItem.Description.IndexOf(filterTextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Checks the given result items to see if the result is from one of the selected books
        /// </summary>
        /// <param name="results">The results to review</param>
        /// <returns>A filtered set of results that match for the selected books</returns>
        protected List<CheckResultItem> FilterResults(List<CheckResultItem> results)
        {
            List<CheckResultItem> filteredItems = new List<CheckResultItem>();

            foreach ( CheckResultItem item in results)
            {
                foreach(BookNameItem bookNameItem in SelectedBooks)
                {
                    if(bookNameItem.BookNum == item.Book)
                    {
                        if (ShowDeniedCheckbox.Checked)
                        {
                            filteredItems.Add(item);
                        } else
                        {
                            if( _denied.Contains(item.GetHashCode()))
                            {
                                item.ResultState = CheckResultState.Ignored;
                            } else
                            {
                                item.ResultState = CheckResultState.Found;
                                filteredItems.Add(item);
                            }
                        }
                        continue;
                    }
                }
            }

            return filteredItems;
        }

        /// <summary>
        /// The function will close the results dialog.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            CancelChecks();
        }

        /// <summary>
        /// Recycle (dispose and optionally re-create) run semaphore and cancellation token.
        /// </summary>
        /// <param name="isCreateNew">True to create a new one, false to only dispose any existing one.</param>
        private void RecycleRunEntities(bool isCreateNew)
        {
            RunSemaphore?.Dispose();
            RunSemaphore = isCreateNew
                ? new SemaphoreSlim(MainConsts.MAX_CHECK_THREADS)
                : null;

            RunCancellationTokenSource?.Dispose();
            RunCancellationTokenSource = isCreateNew
                ? new CancellationTokenSource()
                : null;
        }

        /// <summary>
        /// Show progress form and reset contents.
        /// </summary>
        private void ShowProgress()
        {
            ProgressForm.ResetForm();

            Enabled = false;
            ProgressForm.Show(this);
        }

        /// <summary>
        /// Hide progress form and reactivate this form.
        /// </summary>
        private void HideProgress()
        {
            ProgressForm.Hide();

            Enabled = true;
            Activate();
        }

        /// <summary>
        /// The progress form cancelled observer.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void OnProgressFormCancelled(object sender, EventArgs e)
        {
            CancelChecks();

            HideProgress();
        }

        /// <summary>
        /// The event handler for clicking the License button. This function will start the License form.
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void LicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormUtil.StartLicenseForm();
        }

        /// <summary>
        /// When we want to clear the check filter
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void clearCheckFilterButton_Click(object sender, EventArgs e)
        {
            filterTextBox.Text = "";
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// Set the books filter back to ALL
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void bookFilterClearButton_Click(object sender, EventArgs e)
        {
            // set the default set of books, all of them
            SelectedBooks = ProjectManager.BookNamesByNum.Values.ToArray();
            string selectedBooksString = BookSelection.stringFromSelectedBooks(SelectedBooks);
            bookFilterTextBox.Text = selectedBooksString;
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// Select, for filtering, a set of books
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void selectBooksButton_Click(object sender, EventArgs e)
        {
            // bring up book selection dialog, use current selection to initialize
            using (var form = new BookSelection(ProjectManager, SelectedBooks))
            {
                form.StartPosition = FormStartPosition.CenterParent;

                var result = form.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    // update which books were selected
                    SelectedBooks = form.GetSelected();
                    string selectedBooksString = BookSelection.stringFromSelectedBooks(SelectedBooks);
                    bookFilterTextBox.Text = selectedBooksString;

                    PopulateChecksDataGridView();
                }
            }
        }

        /// <summary>
        /// This method handles a click event on the "Deny"/"Un-Deny" button.
        /// </summary>
        /// <param name="sender">The "Deny" button</param>
        /// <param name="e">The event information</param>
        private void Deny_Click(object sender, EventArgs e)
        {
            var item = (CheckResultItem)issuesDataGridView.CurrentRow.Tag;

            if (item.ResultState == CheckResultState.Found)
            {
                item.ResultState = CheckResultState.Ignored;
                if (!_denied.Contains(item.GetHashCode()))
                {
                    _denied.Add(item.GetHashCode());
                }
            } else if(item.ResultState == CheckResultState.Ignored)
            {
                item.ResultState = CheckResultState.Found;
                _denied.Remove(item.GetHashCode());
            }

            SaveDeniedResults();
            PopulateChecksDataGridView();
            UpdateDenyButton();
        }

        /// <summary>
        /// This method handles checking/unchecking the "Show Denied" checkbox.
        /// </summary>
        /// <param name="sender">The "Show Denied" checkbox</param>
        /// <param name="e">The event information</param>
        private void ShowDenied_CheckedChanged(object sender, EventArgs e)
        {
            CheckState.Checked.Equals(ShowDeniedCheckbox.CheckState);
            PopulateChecksDataGridView();
        }

        /// When the filter text changes, trigger filtering
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void filterTextBox_TextChanged(object sender, EventArgs e)
        {
            PopulateChecksDataGridView();
        }

        /// <summary>
        /// When the selection on this item changes, update the dependant list
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checksDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // only do updates here when the control is focused. Updates to the UI happen in other ways too.
            if (checksDataGridView.Focused)
            {
                PopulateIssuesDataGridView();
            }
        }

        /// <summary>
        /// Update the list of issues in the issues list
        /// </summary>
        private void PopulateIssuesDataGridView(int rowIndexOverride = DEFAULT_ROW_NOT_SELECTED)
        {
            // update list of issues
            if (checksDataGridView.SelectedRows != null && checksDataGridView.SelectedRows.Count > 0 && checksDataGridView.SelectedRows[0] != null && checksDataGridView.SelectedRows[0].Tag != null)
            {
                // use selected rows[0] instead of current row since they aren't the same thing. Since the control is limited to only a single selection, this is the value we want.
                int currentSelectedRowIndex = rowIndexOverride != DEFAULT_ROW_NOT_SELECTED ? rowIndexOverride : checksDataGridView.SelectedRows[0].Index;
                KeyValuePair<CheckAndFixItem, List<CheckResultItem>> result = (KeyValuePair<CheckAndFixItem, List<CheckResultItem>>)checksDataGridView.Rows[currentSelectedRowIndex].Tag;

                issuesDataGridView.Rows.Clear();

                foreach (CheckResultItem item in result.Value)
                {
                    var verseLocation = new VerseLocation(item.Book, item.Chapter, item.Verse);

                    int rowIndex = issuesDataGridView.Rows.Add(new object[]
                    {
                        getStatusIcon(item.ResultState),
                        verseLocation.toString(),
                        item.MatchText
                    });
                    issuesDataGridView.Rows[rowIndex].Tag = item;
                }

                if (issuesDataGridView.Rows != null && issuesDataGridView.Rows.Count > 0 && issuesDataGridView.Rows[0] != null)
                {
                    issuesDataGridView.Rows[0].Selected = true;
                }
                PopulateMatchFixTextBoxes();
            }
            UpdateDenyButton();
        }

        /// <summary>
        /// Returns the appropriate icon bitmap for the status of the issue
        /// </summary>
        /// <param name="resultState">The current issue result state</param>
        /// <returns>a bitmap to be used in the column</returns>
        private Icon getStatusIcon(CheckResultState resultState)
        {
            switch(resultState)
            {
                case CheckResultState.Ignored:
                    return Properties.Resources.x_mark_16;
                case CheckResultState.Fixed:
                    return Properties.Resources.checkmark_16;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Navigate the project view to the issue location
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void issuesDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (issuesDataGridView.Rows != null && issuesDataGridView.Rows.Count > 0 && issuesDataGridView.CurrentRow != null && issuesDataGridView.CurrentRow.Tag != null)
            {
                var item = (CheckResultItem)issuesDataGridView.CurrentRow.Tag;

                // navigate project to BCV
                HostUtil.Instance.GotoBcvInGui(ActiveProjectName, item.Book == -1 ? 0 : item.Book, item.Chapter == -1 ? 0 : item.Chapter, item.Verse == -1 ? 0 : item.Verse);
            }
        }

        /// <summary>
        /// Update the match and fix texts and the deny button based on new selection
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void issuesDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // only do updates here when the control is focused. Updates to the UI happen in other ways too.
            if (issuesDataGridView.Focused)
            {
                // update match/fix text boxes
                PopulateMatchFixTextBoxes();
            }

            UpdateDenyButton();
        }
        
        /// <summary>
        /// Update the text on the deny button based on the currently selected row
        /// </summary>
        private void UpdateDenyButton()
        {
            if (issuesDataGridView.Rows != null && issuesDataGridView.Rows.Count > 0 && issuesDataGridView.CurrentRow != null && issuesDataGridView.CurrentRow.Tag != null)
            {
                var item = (CheckResultItem)issuesDataGridView.CurrentRow.Tag;
                switch (item.ResultState)
                {
                    case CheckResultState.Ignored:
                        DenyButton.Text = UNDENY_BUTTON_TEXT;
                        break;
                    case CheckResultState.Found:
                    default:
                        DenyButton.Text = DENY_BUTTON_TEXT;
                        break;
                }
            }
        }

        /// <summary>
        /// Fill in the match/fix text boxes based on current selections
        /// </summary>
        private void PopulateMatchFixTextBoxes()
        {
            if(issuesDataGridView.Rows != null && issuesDataGridView.Rows.Count > 0 && issuesDataGridView.CurrentRow != null && issuesDataGridView.CurrentRow.Tag != null)
            {
                var item = (CheckResultItem)issuesDataGridView.CurrentRow.Tag;

                // update match text
                matchTextBox.Text = item.Reference;

                matchTextBox.SelectAll();
                matchTextBox.SelectionBackColor = Color.White;

                matchTextBox.SelectionStart = MinusPrecedingChars(
                    item.Reference,
                    item.MatchStart,
                    '\r');
                matchTextBox.SelectionLength = item.MatchLength;
                matchTextBox.SelectionBackColor = Color.Yellow;
                matchTextBox.ScrollToCaret();

                matchTextBox.Refresh();

                // update fix text
                if (!String.IsNullOrEmpty(item.FixText))
                {
                    StringBuilder stringBuilder = new StringBuilder(item.Reference);
                    stringBuilder.Remove(item.MatchStart, item.MatchLength);
                    stringBuilder.Insert(item.MatchStart, item.FixText);

                    var fixReference = stringBuilder.ToString();

                    fixTextBox.Text = fixReference;

                    fixTextBox.SelectAll();
                    fixTextBox.SelectionBackColor = Color.White;

                    fixTextBox.SelectionStart = MinusPrecedingChars(
                        fixReference,
                        item.MatchStart,
                        '\r');
                    fixTextBox.SelectionLength = item.MatchLength;
                    fixTextBox.SelectionBackColor = Color.LightGreen;
                    fixTextBox.ScrollToCaret();

                    fixTextBox.Refresh();
                } else
                {
                    fixTextBox.Text = "";
                    fixTextBox.Refresh();
                }
                
            } else
            {
                matchTextBox.Text = "";
                matchTextBox.Refresh();

                fixTextBox.Text = "";
                fixTextBox.Refresh();
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

        private void CheckResultsForm_Shown(object sender, EventArgs e)
        {
            CheckRunner ??= new CheckAndFixRunner();
            ImportManager ??= new ImportManager(Host, ActiveProjectName);

            // set the status column so that empty doesn't show unfound image
            ((DataGridViewImageColumn)issuesDataGridView.Columns["statusIconColumn"]).DefaultCellStyle.NullValue = null;
            LoadDeniedResults();
            setSelectedBooks();

            RunChecks();
        }
    }

    /// <summary>
    /// Basic exception for text check errors.
    /// </summary>
    public class TextCheckException : ApplicationException, ISerializable
    {
        /// <summary>
        /// Basic ctor.
        /// </summary>
        /// <param name="messageText">Message text (optional, may be null).</param>
        /// <param name="causeEx">Cause exception (optional, may be null).</param>
        public TextCheckException(string messageText, Exception causeEx)
            : base(messageText, causeEx)
        {
        }
    }
}
