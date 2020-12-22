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
using TvpMain.Project;
using TvpMain.Util;

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

        // TODO possibly make this a constructor argument.
        private readonly CheckAndFixRunner checkRunner = new CheckAndFixRunner();

        /// <summary>
        /// The collection of <c>CheckResultItem</c>s mapped by the associated <c>CheckAndFixItem</c>.
        /// </summary>
        Dictionary <CheckAndFixItem, List<CheckResultItem>> CheckResults { get; set; } = new Dictionary<CheckAndFixItem, List<CheckResultItem>>();

        public CheckResultsForm(IHost host, List<CheckAndFixItem> checksToRun, CheckRunContext checkRunContext)
        {
            // validate inputs
            Host = host ?? throw new ArgumentNullException(nameof(host));
            ChecksToRun = checksToRun ?? throw new ArgumentNullException(nameof(checksToRun));
            CheckRunContext = checkRunContext ?? throw new ArgumentNullException(nameof(checkRunContext));
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

            // TODO get the Paratext Settings to determine the root location of the paratext projects
            var projectsRootFolder = HostUtil.Instance.GetParatextProjectsDirectory(CheckRunContext.Project);
            var specifiedProjectFolder = Path.Combine(projectsRootFolder, CheckRunContext.Project);
            var projectSettings = ParatextProjectHelper.GetProjectSettings(specifiedProjectFolder);
            var temp2 = projectSettings.BookFileName(CheckRunContext.Books[0].BookNum);
            
            // TODO Grab the prefix and postfix from settings. Or get filename from Paratext utility

            // run each of the specified checks
            ChecksToRun.ForEach(caf =>
            {
                var books = CheckRunContext.Books;
                switch (CheckRunContext.CheckScope)
                {
                    case CheckAndFixItem.CheckScope.BOOK:
                    case CheckAndFixItem.CheckScope.CHAPTER:
                        var results = books.Select<BookNameItem, List<CheckResultItem>>((book) =>
                        {
                            var bookFilename = projectSettings.BookFileName(CheckRunContext.Books[0].BookNum); 
                            string contents = File.ReadAllText(Path.Combine(specifiedProjectFolder, bookFilename));

                            // TODO filter content if we're handling chapters
                            if (CheckRunContext.CheckScope.Equals(CheckAndFixItem.CheckScope.CHAPTER))
                            {
                                Regex.Matches(contents, "//", RegexOptions.Multiline & RegexOptions.ECMAScript);
                            }

                            return checkRunner.ExecCheckAndFix(contents, caf);
                        });
                        CheckResults.Add(caf, results.SelectMany(l => l).Distinct().ToList());
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });

            // populate the checks results table
            PopulateChecksDataGridView();
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
