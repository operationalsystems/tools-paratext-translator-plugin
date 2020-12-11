using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpMain.Forms
{
    public partial class CheckEditor : Form
    {
        // keep track if there were changes made
        private bool _dirty = false;

        private ICheckManager _checkManager;

        private CheckAndFixItem _checkAndFixItem;

        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        GenericProgressForm _progressForm;

        public CheckEditor()
        {
            InitializeComponent();

            _checkManager = new CheckManager();
        }

        /// <summary>
        /// On dialog load, set to 'new' state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEditor_Load(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        /// <summary>
        /// Set to "new" state, a brand new check/fix item to edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // prevent overwriting changes unless explict
            if(_dirty)
            {
                DialogResult dialogResult = MessageBox.Show("You have unsaved changes, are you sure you wish to proceed?", "Verify", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.No)
                {
                    return;
                }

            }

            _checkAndFixItem = new CheckAndFixItem();
            _checkAndFixItem.Id = Guid.NewGuid().ToString();
            updateUI();

            checkFixIdLabel.Text = _checkAndFixItem.Id;

            scopeCombo.SelectedItem = "CHAPTER";
            _dirty = false;

        }

        /// <summary>
        /// Open a file for editing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _checkManager.GetLocalRepoDirectory();
            openFileDialog.Filter = "check/fix files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using var fileStream = openFileDialog.OpenFile();
                _checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(fileStream);
                updateUI();
            }
        }

        /// <summary>
        /// Save the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dirty)
            {
                updateCheckAndFix();

                if(String.IsNullOrEmpty(_checkAndFixItem.Name.Trim()) ||
                    String.IsNullOrEmpty(_checkAndFixItem.Version.Trim()) ||
                    String.IsNullOrEmpty(_checkAndFixItem.DefaultItemDescription.Trim()) ||
                    (String.IsNullOrEmpty(_checkAndFixItem.CheckRegex.Trim()) && String.IsNullOrEmpty(_checkAndFixItem.CheckScript.Trim()))
                    )
                {
                    MessageBox.Show("Name, Version, Default Description, and either the Check Regex or the Check Script, must be entered.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                _checkManager.SaveCheckAndFixItem(_checkAndFixItem);

                _dirty = false;
            }
        }

        /// <summary>
        /// Exit this dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dirty)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to exit without saving?", "Exit?", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
            } else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Save and Publish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void publishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to save and publish this check/fix?", "Save and Publish?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                saveToolStripMenuItem_Click(sender, e);

                _progressForm = new GenericProgressForm("Publishing check/fix item...");
                _progressForm.Show(this);

                publishWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Worker for doing publish updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void publishWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _checkManager.SynchronizeInstalledChecks();
            List<CheckAndFixItem> remoteChecks = _checkManager.GetInstalledCheckAndFixItems();

            var found = false;

            foreach (CheckAndFixItem checkAndFixItem in remoteChecks)
            {
                if (checkAndFixItem.Name.Equals(_checkAndFixItem.Name) && checkAndFixItem.Version.Equals(_checkAndFixItem.Version))
                {
                    found = true;
                }
            }

            if (found)
            {
                MessageBox.Show("This version of the Check/Fix already exists in the repository, you must increment the version before trying to publish.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                _checkManager.PublishCheckAndFixItem(_checkAndFixItem);
            }
        }

        /// <summary>
        /// For when the worker is complete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void publishWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressForm.Close();
        }

        /// <summary>
        /// Update the UI when a CFitem is loaded
        /// </summary>
        private void updateUI()
        {
            checkFixIdLabel.Text = _checkAndFixItem.Id ?? "";
            checkFixNameTextBox.Text = _checkAndFixItem.Name ?? "";
            versionTextBox.Text = _checkAndFixItem.Version ?? "";
            scopeCombo.SelectedItem = _checkAndFixItem.Scope.ToString();
            defaultDescTextBox.Text = _checkAndFixItem.DefaultItemDescription ?? "";
            languagesTextBox.Text = _checkAndFixItem.Languages == null ? "" : string.Join(", ", _checkAndFixItem.Languages);
            tagsTextBox.Text = _checkAndFixItem.Tags == null ? "" :string.Join(", ", _checkAndFixItem.Tags);
            descriptionTextBox.Text = _checkAndFixItem.Description;

            checkFindRegExTextBox.Text = _checkAndFixItem.CheckRegex ?? "";
            fixRegExTextBox.Text = _checkAndFixItem.FixRegex ?? "";
            scriptTextBox.Text = _checkAndFixItem.CheckScript == null ? "" : _checkAndFixItem.CheckScript.Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Update the CFitem from the UI before saves
        /// </summary>
        private void updateCheckAndFix()
        {
            try
            {
                _checkAndFixItem.Name = checkFixNameTextBox.Text;
                _checkAndFixItem.Version = versionTextBox.Text;
                _checkAndFixItem.Scope = (CheckAndFixItem.CheckScope)scopeCombo.SelectedIndex;
                _checkAndFixItem.DefaultItemDescription = defaultDescTextBox.Text;
                _checkAndFixItem.Languages = languagesTextBox.Text.Trim().Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
                _checkAndFixItem.Tags = tagsTextBox.Text.Trim().Split(',').Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray();
                _checkAndFixItem.Description = descriptionTextBox.Text;

                _checkAndFixItem.CheckRegex = checkFindRegExTextBox.Text;
                _checkAndFixItem.FixRegex = fixRegExTextBox.Text;
                _checkAndFixItem.CheckScript = scriptTextBox.Text;
            } catch
            {
                MessageBox.Show("Name, Version, Default Description, and either the Check Regex or the Check Script, must be entered.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// Keep track of changes and mark the CFitem dirty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void content_TextChanged(object sender, EventArgs e)
        {
            _dirty = true;
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkFindRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression to find issues. This value may be empty if it relies on the Javascript to perform its modifications.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression replacement pattern, using $1 type replacement values from the groupings found in the check find regex.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scriptTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Javascript that can be called after the two regular expressions are run, if they are defined." + Environment.NewLine);
            helpTextBox.AppendText("This script MUST implement the function checkAndFix(checkResultItems). The CheckResultItems are the results" +
                " found in the regular expression pass.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkFixIdLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The automatically assigned unique identifier.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkFixNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The name of your check/fix.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void versionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The version of the check/fix. Increment each time you publish an update. Use semantic versioning scheme: https://semver.org/");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scopeCombo_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The text scope to run this check/fix at; PROJECT, BOOK, CHAPTER, VERSE" + Environment.NewLine);
            helpTextBox.AppendText("Leave defaulted to CHAPTER if unsure.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void defaultDescTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("This is the default description associated with matched results.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void languagesTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Languages associated with this check/fix. Separate lanugages by comma." + Environment.NewLine);
            helpTextBox.AppendText("Use language codes found in projects like eng-US, zh, ja, etc.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tagsTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText( "Enter Tags associated with this check/fix. Separate tags by comma." + Environment.NewLine);
            helpTextBox.AppendText("Currently supported tags: RTL = right to left only languages.");
        }

        /// <summary>
        /// Update the help text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void descriptionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.Text = "Enter the full description for this check/fix.";
        }
    }
}
