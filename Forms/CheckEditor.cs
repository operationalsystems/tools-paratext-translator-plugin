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

        private void CheckEditor_Load(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _checkAndFixItem = new CheckAndFixItem();
            _checkAndFixItem.Id = Guid.NewGuid().ToString();

            checkFixIdLabel.Text = _checkAndFixItem.Id;

            scopeCombo.SelectedItem = "CHAPTER";

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = _checkManager.GetLocalRepoDirectory();
            openFileDialog.Filter = "check/fix files (*.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileStream = openFileDialog.OpenFile();
                _checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(fileStream);
                updateUI();
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dirty)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to exit without saving?", "Exit?", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
            }
        }

        private void publishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to publish this check/fix?", "Exit?", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                updateCheckAndFix();
                _checkManager.PublishCheckAndFixItem(_checkAndFixItem);
            }
        }

        private void updateUI()
        {
            checkFixIdLabel.Text = _checkAndFixItem.Id;
            checkFixNameTextBox.Text = _checkAndFixItem.Name;
            versionTextBox.Text = _checkAndFixItem.Version;
            scopeCombo.SelectedItem = _checkAndFixItem.Scope.ToString();
            defaultDescTextBox.Text = _checkAndFixItem.DefaultItemDescription;
            languagesTextBox.Text = string.Join(", ", _checkAndFixItem.Languages);
            tagsTextBox.Text = string.Join(", ", _checkAndFixItem.Tags);
            descriptionTextBox.Text = _checkAndFixItem.Description;

            checkFindRegExTextBox.Text = _checkAndFixItem.CheckRegex;
            fixRegExTextBox.Text = _checkAndFixItem.FixRegex;
            scriptTextBox.Text = _checkAndFixItem.CheckScript.Replace("\n", Environment.NewLine);
        }

        private void updateCheckAndFix()
        {
            _checkAndFixItem.Name = checkFixNameTextBox.Text;
            _checkAndFixItem.Version = versionTextBox.Text;
            _checkAndFixItem.Scope = (CheckAndFixItem.CheckScope)scopeCombo.SelectedIndex;
            _checkAndFixItem.DefaultItemDescription = defaultDescTextBox.Text;
            _checkAndFixItem.Languages = languagesTextBox.Text.Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            _checkAndFixItem.Tags = tagsTextBox.Text.Split(',').Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
            _checkAndFixItem.Description = descriptionTextBox.Text;

            _checkAndFixItem.CheckRegex = checkFindRegExTextBox.Text;
            _checkAndFixItem.FixRegex = fixRegExTextBox.Text;
            _checkAndFixItem.CheckScript = scriptTextBox.Text;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updateCheckAndFix();

            string filename = _checkAndFixItem.Name.Replace(" ", "") + '-' + _checkAndFixItem.Version + ".xml";
            string filePath = Path.Combine(_checkManager.GetLocalRepoDirectory(), filename);

            _checkAndFixItem.SaveToXmlFile(filePath);
        }

        private void checkFindRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression to find issues. This value may be empty if it relies on the Javascript to perform its modifications.");
        }

        private void FixRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression replacement pattern, using $1 type replacement values from the groupings found in the check find regex.");
        }

        private void scriptTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Javascript that can be called after the two regular expressions are run, if they are defined." + Environment.NewLine);
            helpTextBox.AppendText("This script MUST implement the function checkAndFix(checkResultItems). The CheckResultItems are the results" +
                " found in the regular expression pass.");
        }

        private void checkFixIdLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The automatically assigned unique identifier.");
        }

        private void checkFixNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The name of your check/fix.");
        }

        private void versionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The version of the check/fix. Increment each time you publish an update. Use semantic versioning scheme: https://semver.org/");
        }

        private void scopeCombo_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The text scope to run this check/fix at; PROJECT, BOOK, CHAPTER, VERSE" + Environment.NewLine);
            helpTextBox.AppendText("Leave defaulted to CHAPTER if unsure.");
        }

        private void defaultDescTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("This is the default description associated with matched results.");
        }

        private void languagesTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Languages associated with this check/fix. Separate lanugages by comma." + Environment.NewLine);
            helpTextBox.AppendText("Use language codes found in projects like eng-US, zh, ja, etc.");
        }

        private void tagsTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText( "Enter Tags associated with this check/fix. Separate tags by comma." + Environment.NewLine);
            helpTextBox.AppendText("Currently supported tags: RTL = right to left only languages.");
        }

        private void descriptionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.Text = "Enter the full description for this check/fix.";
        }

    }
}
