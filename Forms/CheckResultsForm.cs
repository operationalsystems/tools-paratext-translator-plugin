using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Import;
using TvpMain.Text;
using TvpMain.Util;
using static TvpMain.Check.CheckAndFixItem;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
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
        /// Project content import manager.
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
        /// Check updated event handler.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Reusable progress form.
        /// </summary>
        private readonly ProgressForm ProgressForm;

        /// <summary>
        /// The basic constructor.
        /// </summary>
        /// <param name="host">The Paratext Host object.</param>
        /// <param name="checksToRun">The list of checks to run against the content.</param>
        /// <param name="checkRunContext">The context of what Bible content we're checking against.</param>
        /// <param name="checkRunner">The <c>CheckAndFixRunner</c> that will execute the checks against supplied content.</param>
        /// <param name="importManager">The <c>ImportManager</c> used to read in the scripture text.</param>
        public CheckResultsForm(
            IHost host,
            List<CheckAndFixItem> checksToRun,
            CheckRunContext checkRunContext,
            CheckAndFixRunner checkRunner,
            ImportManager importManager)
        {
            // validate inputs
            Host = host ?? throw new ArgumentNullException(nameof(host));
            ChecksToRun = checksToRun ?? throw new ArgumentNullException(nameof(checksToRun));
            CheckRunContext = checkRunContext ?? throw new ArgumentNullException(nameof(checkRunContext));
            CheckRunner = checkRunner ?? throw new ArgumentNullException(nameof(checkRunner));
            ImportManager = importManager ?? throw new ArgumentNullException(nameof(importManager));
            CheckRunContext.Validate();

            ProgressForm = new ProgressForm();
            ProgressForm.Cancelled += OnProgressFormCancelled;

            CheckUpdated += OnCheckUpdated;

            // initialize the components
            InitializeComponent();

            // Enable the visibility of this form
            this.Show();
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
                RunBookCtr++;
                CheckUpdated?.Invoke(this,
                    new CheckUpdatedArgs(CheckRunContext.Books.Count(), RunBookCtr, updateBookNum));
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
            if (ChecksToRun.Count <= 0)
            {
                MessageBox.Show(
                    "No checks provided.",
                    "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Hide();
                return;
            }

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
                // Show the progress bar as we kick off our work.
                ShowProgress();

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
                }

                if (RunCancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
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
        /// <param name="bcv">The current Book, Chapter, and Verse location (BCV).</param>
        private void ExecuteChecksAndStoreResults(string content, List<CheckAndFixItem> checksToRun, string bcv)
        {
            // check the content with every specified check
            checksToRun.ForEach(check =>
            {
                var results = CheckRunner.ExecCheckAndFix(content, check);

                // TODO set the BCV for each result

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

                var projectSb = new StringBuilder();
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
                                projectSb.AppendLine(verseText);
                                bookSb.AppendLine(verseText);
                                chapterSb.AppendLine(verseText);

                                // check the verse with verse checks
                                ExecuteChecksAndStoreResults(verseText, ChecksByScope[CheckScope.VERSE], "TODO");
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
                        ExecuteChecksAndStoreResults(chapterSb.ToString(), ChecksByScope[CheckScope.CHAPTER], "TODO");
                    }

                    // check the book with book checks
                    ExecuteChecksAndStoreResults(bookSb.ToString(), ChecksByScope[CheckScope.BOOK], "TODO");
                    ExecuteChecksAndStoreResults(projectSb.ToString(), ChecksByScope[CheckScope.PROJECT], "TODO");

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
            foreach (KeyValuePair<CheckAndFixItem, List<CheckResultItem>> result in CheckResults)
            {
                checksDataGridView.Rows.Add(new object[] {
                    false,                                      // selected
                    result.Key.Name.Trim(),                     // category
                    result.Key.Description.Trim(),              // description
                    result.Value.Count                          // count
                });
            }

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
    }
}
