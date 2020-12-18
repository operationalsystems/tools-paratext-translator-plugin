using AddInSideViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;
using TvpMain.Project;
using TvpMain.Properties;
using TvpMain.Text;
using TvpMain.Util;

namespace TvpMain.Forms
{
    public partial class RunChecks : Form
    {
        /// <summary>
        /// Paratext host interface.
        /// </summary>
        private readonly IHost _host;

        /// <summary>
        /// Active project name.
        /// </summary>
        private string _activeProjectName;

        /// <summary>
        /// Provides project setting & metadata access.
        /// </summary>
        private ProjectManager _projectManager;

        /// <summary>
        /// This project's default checks
        /// </summary>
        private ProjectCheckSettings _projectCheckSettings;

        /// <summary>
        /// Storing the default book
        /// </summary>
        private int _defaultCurrentBook;

        /// <summary>
        /// The list of selected books to check
        /// </summary>
        private BookNameItem[] _selectedBooks;

        /// <summary>
        /// Access to the checks themselves
        /// </summary>
        ICheckManager _checkManager;

        /// <summary>
        /// The list of remote checks
        /// </summary>
        List<CheckAndFixItem> _remoteChecks;

        /// <summary>
        /// The list of local checks, can't be set as defaults
        /// </summary>
        List<CheckAndFixItem> _localChecks;

        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        GenericProgressForm _progressForm;

        /// <summary>
        /// Standard constructor for kicking off main plugin dialog
        /// </summary>
        /// <param name="host"></param>
        /// <param name="activeProjectName"></param>
        public RunChecks(IHost host, string activeProjectName)
        {
            InitializeComponent();

            _progressForm = new GenericProgressForm("Synchronizing Check/Fixes");
            _checkManager = new CheckManager();

            // set up the needed service dependencies
            _host = host ?? throw new ArgumentNullException(nameof(host));

            setActiveProject(activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName)));
        }

        /// <summary>
        /// Set up to support selecting a different project when we enable that. Right now the Plugin API
        /// does not allow for getting a list of projects.
        /// </summary>
        /// <param name="activeProjectName"></param>
        private void setActiveProject(string activeProjectName)
        {
            _activeProjectName = activeProjectName;
            _projectManager = new ProjectManager(_host, _activeProjectName);
            _projectCheckSettings = HostUtil.Instance.GetProjectCheckSettings(_activeProjectName);

            resetToProjectDefaults();
            setCurrentBookDefaults();
            checksList.ClearSelection();
        }

        /// <summary>
        /// On Load method for the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunChecks_Load(object sender, EventArgs e)
        {
            // the project name text, will eventually be the selected current project from the list of projects
            projectNameText.Text = _activeProjectName;

            // sets up for just the current book by default
            setCurrentBook();

            // disable the ability to save the project check defaults if not an admin
            if(!HostUtil.Instance.isCurrentUserAdmin(_activeProjectName))
            {
                setDefaultsToSelected.Hide();
            }

            // sets the chapter lengths and such for the current book
            setCurrentBookDefaults();

            // set the copyright text
            Copyright.Text = MainConsts.COPYRIGHT;

            // start the sync for the check/fixes
            _progressForm.Show(this);
            loadingWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Async method for synchronizing the check/fixes for the project and selecting the defaults
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadingWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // sync with repo
                _checkManager.SynchronizeInstalledChecks();
            } catch
            {
                // in case the user is off-line
                MessageBox.Show("Could not synchronize with check/fix repo. You may only run checks with locally available set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Close the progress form when complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            checksList.Invoke(new MethodInvoker(delegate { this.updateChecksList(); }));
            _progressForm.Close();
        }

        /// <summary>
        /// After doing async download of latest checks, update the list (must be run in main thread)
        /// </summary>
        private void updateChecksList()
        {
            try
            {
                // load all the checks into the list
                _remoteChecks = _checkManager.GetInstalledCheckAndFixItems();
                _localChecks = _checkManager.GetSavedCheckAndFixItems();

                checksList.Enabled = false;
                checksList.Rows.Clear();

                foreach (var item in _remoteChecks)
                {
                    var rowIndex = checksList.Rows.Add(
                        false,
                        item.Name,
                        item.Version,
                        item.Languages != null ? String.Join(", ", item.Languages) : "All",
                        item.Tags != null ? String.Join(", ", item.Tags) : "",
                        item.Id
                        );

                    if (!isCheckAvailableForProject(item))
                    {
                        checksList.Rows[rowIndex].DefaultCellStyle.BackColor = SystemColors.Control;
                        checksList.Rows[rowIndex].DefaultCellStyle.ForeColor = SystemColors.GrayText;
                    }

                    checksList.Rows[rowIndex].Tag = item;
                }

                foreach (var item in _localChecks)
                {
                    var rowIndex = checksList.Rows.Add(
                        false,
                        "(Local) " + item.Name,
                        item.Version,
                        item.Languages != null ? String.Join(", ", item.Languages) : "All",
                        item.Tags != null ? String.Join(", ", item.Tags) : "",
                        item.Id
                        );

                    if (!isCheckAvailableForProject(item))
                    {
                        checksList.Rows[rowIndex].DefaultCellStyle.BackColor = SystemColors.Control;
                        checksList.Rows[rowIndex].DefaultCellStyle.ForeColor = SystemColors.GrayText;
                    }

                    checksList.Rows[rowIndex].Tag = item;
                }
            }
            finally
            {
                checksList.Enabled = true;
            }

            // deselect the first row
            checksList.ClearSelection();

            // set the default checks for the project
            resetToProjectDefaults();
        }

        /// <summary>
        /// Sets the defaults for the current book, including the name and chapter counts
        /// </summary>
        private void setCurrentBookDefaults()
        {
            int _runBookNum, _runChapterNum, _runVerseNum;

            var versificationName = _host.GetProjectVersificationName(_activeProjectName);
            BookUtil.RefToBcv(_host.GetCurrentRef(versificationName),
                out _runBookNum, out _runChapterNum, out _runVerseNum);

            _defaultCurrentBook = _runBookNum;

            var lastChapter = _host.GetLastChapter(_runBookNum, versificationName);

            fromChapterDropDown.Items.Clear();
            toChapterDropDown.Items.Clear();

            // add items for all the chapters
            for (int i = 0; i < lastChapter; i++)
            {
                fromChapterDropDown.Items.Add(i.ToString());
                toChapterDropDown.Items.Add(i.ToString());
            }

            // set the chapter indexes
            fromChapterDropDown.SelectedIndex = 0;
            toChapterDropDown.SelectedIndex = lastChapter - 1;

            // by default, use the current book
            currentBookRadioButton.Checked = true;

            // set the current book name
            currentBookText.Text = _projectManager.BookNamesByNum[_runBookNum].GetAvailableName(BookNameType.LongName, BookNameType.ShortName, BookNameType.Abbreviation);
            _selectedBooks = new BookNameItem[] { _projectManager.BookNamesByNum[_runBookNum] };

        }

        /// <summary>
        /// Quit the dialog - File -> Exit from menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Start the check/fix editor from the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckEditor checkEditor = new CheckEditor();
            checkEditor.Show(this);

        }

        /// <summary>
        /// Default, show the EULA for the project.
        /// License from menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string formTitle = $"{pluginName} - End User License Agreement";

            LicenseForm eulaForm = new LicenseForm
            {
                FormType = LicenseForm.FormTypes.Info,
                FormTitle = formTitle,
                LicenseText = Resources.TVP_EULA
            };
            eulaForm.OnDismiss = () => eulaForm.Close();
            eulaForm.Show();
        }

        /// <summary>
        /// This function is to handle when the "Run Checks" button is clicked. We pass the checks and notion of what to check to the CheckResultsForm to process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runChecksButton_Click(object sender, EventArgs e)
        {
            // grab the selected checks
            var selectedChecks = GetSelectedChecks();

            // grab the check run context
            var checkContext = GetCheckRunContext();

            // pass the checks and specification of what to check to the CheckResultsForm to perform the necessary search with.
            var checkResultsForm = new CheckResultsForm(selectedChecks, checkContext);
            checkResultsForm.Show();
        }

        /// <summary>
        /// This function will return the <c>CheckAndFixItem</c>s that are selected in the Run Checks list.
        /// </summary>
        /// <returns>The selected <c>CheckAndFixItem</c>s</returns>
        private List<CheckAndFixItem> GetSelectedChecks()
        {
            var selectedChecks = new List<CheckAndFixItem>();

            // grab the selected checks
            foreach (DataGridViewRow row in checksList.Rows)
            {
                CheckAndFixItem item = (CheckAndFixItem) row.Tag;
                if ((bool)row.Cells[0].Value)
                {
                    selectedChecks.Add(item);
                }
            }

            return selectedChecks;
        }

        /// <summary>
        /// This function create and return the <c>CheckRunContext</c> of what's being checked against.
        /// </summary>
        /// <returns>The <c>CheckRunContext</c> of what's being checked against.</returns>
        private CheckRunContext GetCheckRunContext()
        {
            // initialize the context with the project name.
            var checkRunContext = new CheckRunContext()
            {
                Project = _activeProjectName,

            };

            // track the selected books
            checkRunContext.Books = (BookNameItem[])_selectedBooks.Clone();

            if (currentBookRadioButton.Checked)
            {
                checkRunContext.CheckScope = CheckAndFixItem.CheckScope.CHAPTER;

                // track the specified chapters
                checkRunContext.Chapters = new List<int>();

                var chapterStart = int.Parse(fromChapterDropDown.Text);
                var chapterEnd = int.Parse(toChapterDropDown.Text);

                // flip the values, if the end is larger than the start
                if (chapterStart > chapterEnd)
                {
                    var temp = chapterStart;
                    chapterStart = chapterEnd;
                    chapterEnd = temp;
                }

                // add the chapters to check
                for (int i = chapterStart; i <= chapterEnd; i++)
                {
                    checkRunContext.Chapters.Add(i);
                }
            }
            else
            {
                checkRunContext.CheckScope = CheckAndFixItem.CheckScope.BOOK;
            }

            return checkRunContext;
        }

        /// <summary>
        /// Select which books, if not using the default single book, to seach through
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseBooksButton_Click(object sender, EventArgs e)
        {
            // bring up book selection dialog, use current selection to initialize
            using (var form = new BookSelection(_projectManager, _selectedBooks))
            {
                form.StartPosition = FormStartPosition.CenterParent;

                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    // update which books were selected
                    _selectedBooks = form.GetSelected();
                    string selectedBooksString = stringFromSelectedBooks(_selectedBooks);
                    chooseBooksText.Text = selectedBooksString;
                }
            }

            // set up UI
            setChooseBooks();
        }

        /// <summary>
        /// Used to display the list of books selected. If there are more than 4 then truncate in the middle.
        /// </summary>
        /// <param name="selectedBooks"></param>
        /// <returns></returns>
        private string stringFromSelectedBooks(BookNameItem[] selectedBooks)
        {
            string names = "";

            if (selectedBooks.Length > 4)
            {
                names = selectedBooks[0].ToString() + ", " + selectedBooks[1].ToString() + ", ..., " + selectedBooks[selectedBooks.Length - 1].ToString();
            }
            else
            {
                names = string.Join(", ", Array.ConvertAll<BookNameItem, string>(selectedBooks, bni => bni.ToString()));
            }

            return names;
        }

        /// <summary>
        /// Quit the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Allow for the selection dialog to pop up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseBooksRadioButton_Click(object sender, EventArgs e)
        {
            setChooseBooks();
            /// If switching from just one, show the selection dialog. Don't do that again if switching back-and-forth
            if (_selectedBooks.Length < 2)
            {
                chooseBooksButton_Click(sender, e);
            }
        }

        /// <summary>
        /// Allow for switching back to just the current book
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentBookRadioButton_Click(object sender, EventArgs e)
        {
            setCurrentBook();
        }

        /// <summary>
        /// Update the UI
        /// </summary>
        private void setChooseBooks()
        {
            currentBookRadioButton.Checked = false;
            chooseBooksRadioButton.Checked = true;
            fromChapterDropDown.Enabled = false;
            toChapterDropDown.Enabled = false;
        }

        /// <summary>
        /// Update the UI, and reset selected books list
        /// </summary>
        private void setCurrentBook()
        {
            currentBookRadioButton.Checked = true;
            chooseBooksRadioButton.Checked = false;
            fromChapterDropDown.Enabled = true;
            toChapterDropDown.Enabled = true;

            _selectedBooks = new BookNameItem[] { _projectManager.BookNamesByNum[_defaultCurrentBook] };
        }

        /// <summary>
        /// Update the project default check/fixes, saving to project file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setDefaultsToSelected_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to set the default checks/fixes for this project? ", "Verify Change", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in checksList.Rows)
                {
                    CheckAndFixItem item = (CheckAndFixItem)checksList.Rows[row.Index].Tag;
                    if ((bool)row.Cells[0].Value)
                    {
                        if (!_projectCheckSettings.DefaultCheckIds.Contains(item.Id))
                        {
                            // do not allow local only to be in the defaults list
                            if (!_localChecks.Contains(item))
                            {
                                _projectCheckSettings.DefaultCheckIds.Add(item.Id);
                            }
                        }
                    }
                    else
                    {
                        if (_projectCheckSettings.DefaultCheckIds.Contains(item.Id))
                        {
                            _projectCheckSettings.DefaultCheckIds.Remove(item.Id);
                        }
                    }
                }

                // save
                Util.HostUtil.Instance.PutProjectCheckSettings(_activeProjectName, _projectCheckSettings);
            }
        }

        /// <summary>
        /// Update the list of selected checks to the project defaults
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetToProjectDefaults_Click(object sender, EventArgs e)
        {
            resetToProjectDefaults();
        }

        /// <summary>
        ///  Actually does the work. In separate method in case there are other reasons to call this (there are).
        /// </summary>
        private void resetToProjectDefaults()
        {
            foreach (DataGridViewRow row in checksList.Rows)
            {
                CheckAndFixItem item = (CheckAndFixItem) row.Tag;
                var isDefault = _projectCheckSettings.DefaultCheckIds.Contains(item.Id);

                row.Cells[0].Value = isDefault;
            }
        }

        /// <summary>
        /// Change the selected value for the check, if it's to be in the set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checksList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                CheckAndFixItem item = (CheckAndFixItem)checksList.Rows[e.RowIndex].Tag;

                if (isCheckAvailableForProject(item))
                {
                    DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)
                    checksList.Rows[e.RowIndex].Cells[0];
                    cell.Value = !(bool)cell.Value;
                    checksList.Rows[e.RowIndex].Selected = false;
                }
            }
        }

        /// <summary>
        /// Utility method to determine if the specified check can be run on this project.
        ///  Will filter out based on language.
        ///  Will filter out based on Tags, add additional tag support here.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Boolean isCheckAvailableForProject(CheckAndFixItem item)
        {
            var languageId = _host.GetProjectLanguageId(_activeProjectName, "translation validation").ToUpper();
            var projectRTL = _host.GetProjectRtoL(_activeProjectName);

            // filter based on language
            var languageEnabled = item.Languages == null
                       || (item.Languages != null && item.Languages.Length == 0)
                       || (item.Languages != null && item.Languages.Length > 0 && item.Languages.Contains(languageId, StringComparer.OrdinalIgnoreCase));

            // filter based on Tags

            // RTL Tag support
            var itemRTL = item.Tags != null && item.Tags.Contains("RTL");

            var rtlEnabled = (projectRTL && itemRTL)
                || (!projectRTL && !itemRTL);

            Debug.WriteLine("Project Language: " + languageId);
            Debug.WriteLine("Project RTL: " + projectRTL);
            Debug.WriteLine("Item RTL: " + rtlEnabled);

            return languageEnabled && rtlEnabled;
        }

        // 
        // The following methods are for updating the instructions at the bottom of the page 
        // as the mouse is moved around the dialog
        // 

        /// <summary>
        /// For handling updating the instructions as the mouse moves around the table of available
        /// checks. Will note if a check is disabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checksList_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                CheckAndFixItem item = (CheckAndFixItem)checksList.Rows[e.RowIndex].Tag;

                helpTextBox.Clear();

                if (!isCheckAvailableForProject(item))
                {
                    helpTextBox.AppendText("NOTE: This check/fix is not selectedable for this project" + Environment.NewLine + Environment.NewLine);
                }

                helpTextBox.AppendText("Check/Fix: " + item.Name + Environment.NewLine);
                helpTextBox.AppendText(item.Description);

                checksList.Rows[e.RowIndex].Selected = false;
            }
        }

        /// <summary>
        /// Update help text for choosing multiple books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseBooksRadioButton_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = "Select the set of books to be checked.";
        }

        /// <summary>
        /// Update help text for choosing multiple books
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chooseBooksText_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = "The set of books chosen to check.";
        }

        /// <summary>
        /// Update help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void currentBookRadioButton_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = "Check the current book";
        }

        /// <summary>
        /// Update help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fromChapterDropDown_MouseEnter(object sender, EventArgs e)
        {
            if (fromChapterDropDown.Enabled)
            {
                helpTextBox.Text = "Select the starting chapter in the current book to begin the check.";
            }
        }

        /// <summary>
        /// Update help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toChapterDropDown_MouseEnter(object sender, EventArgs e)
        {
            if (fromChapterDropDown.Enabled)
            {
                helpTextBox.Text = "Select the ending chapter in the current book to finish checking.";
            }
        }

        /// <summary>
        /// Update help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resetToProjectDefaults_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = "Sets the selected checks/fixes back to the project defaults, " +
                "or if there are no defaults, deselects all.";
        }

        /// <summary>
        /// Update help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setDefaultsToSelected_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Text = "Saves the currently selected checks/fixes as the default set " +
                "for this project. This may only be performed by accounts with the sufficient privileges.";
        }
    }
}
