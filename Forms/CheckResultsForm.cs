using AddInSideViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Import;
using TvpMain.Project;
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
        /// The Paratext plugin host.
        /// </summary>
        private IHost Host { get; set; }

        /// <summary>
        /// A collection of the <c>CheckAndFixItem</c>s to run against the content.
        /// </summary>
        private List<CheckAndFixItem> ChecksToRun { get; set; }

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
        /// The collection of <c>CheckResultItem</c>s mapped by the associated <c>CheckAndFixItem</c>.
        /// </summary>
        Dictionary<CheckAndFixItem, List<CheckResultItem>> CheckResults { get; set; } = new Dictionary<CheckAndFixItem, List<CheckResultItem>>();

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

            // initialize the components
            InitializeComponent();

            // run the provided tests
            RunChecks();
        }

        protected void RunChecks()
        {
            // clear the previous results
            CheckResults.Clear();

            // Pre-filter the checks and fixes so we don't have to do it repeatedly later.
            var checksByScope = new Dictionary<CheckScope, List<CheckAndFixItem>>()
                {
                    { CheckScope.PROJECT, new List<CheckAndFixItem>() },
                    { CheckScope.BOOK, new List<CheckAndFixItem>() },
                    { CheckScope.CHAPTER, new List<CheckAndFixItem>() },
                    { CheckScope.VERSE, new List<CheckAndFixItem>() },
                };

            ChecksToRun.Aggregate(checksByScope, (acc, check) =>
            {
                acc[check.Scope].Add(check);
                return acc;
            });

            // content builders for performing scoped checks.
            // TODO remove parallism to support project SB... or... read in differently.
            var projectSb = new StringBuilder();

            var books = CheckRunContext.Books;

            // iterate books
            foreach (var book in books)
            {
                {
                    var bookSb = new StringBuilder();

                    // track where we are for error reporting
                    var currBookNum = book.BookNum;
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
                                    RunChecks(verseText, checksByScope[CheckScope.VERSE], "TODO");
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
                            RunChecks(chapterSb.ToString(), checksByScope[CheckScope.CHAPTER], "TODO");
                        }

                        // check the book with book checks
                        RunChecks(bookSb.ToString(), checksByScope[CheckScope.BOOK], "TODO");
                    }
                    catch (Exception ex)
                    {
                        var messageText =
                            $"Error: Can't check location: {currBookNum}.{currChapterNum}.{currVerseNum} in project: \"{CheckRunContext.Project}\" (error: {ex.Message}).";

                        HostUtil.Instance.ReportError(messageText, ex);
                    }
                }

                // check the book with book checks
                RunChecks(projectSb.ToString(), checksByScope[CheckScope.PROJECT], "TODO");
            }

            // populate the checks results table
            PopulateChecksDataGridView();
        }

        private void RunChecks(string content, List<CheckAndFixItem> checksToRun, string bcv)
        {
            // check the content with every specified check
            checksToRun.ForEach(check => {
                var results = CheckRunner.ExecCheckAndFix(content, check);

                // TODO set the BCV for each result

                // add or append results
                if (CheckResults.ContainsKey(check))
                {
                    CheckResults[check].AddRange(results);
                } 
                else
                {
                    CheckResults.Add(check, results);
                }
            });
        }

        protected void PopulateChecksDataGridView()
        {
            foreach (KeyValuePair<CheckAndFixItem, List<CheckResultItem>> result in CheckResults)
            {
                checksDataGridView.Rows.Add(new object[] { 
                    false,                                      // selected
                    result.Key.DefaultItemDescription.Trim(),   // category
                    result.Key.Description.Trim(),              // description
                    result.Value.Count                          // count
                });
            }

        }

        /// <summary>
        /// This method is called when the form is cancelled.
        /// </summary>
        public void OnCancel()
        {
            this.Close();
        }

        /// <summary>
        /// Close the dialog
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void cancel_Click(object sender, EventArgs e)
        {
            OnCancel();
        }
    }
}
