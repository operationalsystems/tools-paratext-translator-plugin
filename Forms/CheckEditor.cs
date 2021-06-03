using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpMain.Forms
{
    /// <summary>
    /// Allows for editing of CFitems
    /// </summary>
    public partial class CheckEditor : Form
    {
        // Keep track when changes are made in the UI.
        private bool _dirty = false;

        private ICheckManager _checkManager;

        private CheckAndFixItem _checkAndFixItem;

        /// <summary>
        /// Max number of characters in a line number
        /// </summary>
        private int maxLineNumberCharLength = 5;

        /// <summary>
        /// a set of JavaScript keywords
        /// </summary>
        private string jsKeywords = "break case catch class const continue debugger default delete do else export extends finally " +
                "for function if import in instanceof new return super switch this throw try typeof var void while with yield " +
                "enum implements interface let package private protected public static yield await abstract boolean byte char " +
                "double final float goto int long native short synchronized throws transient volatile";

        /// <summary>
        /// Simple progress bar form for when the checks are being synchronized
        /// </summary>
        GenericProgressForm _progressForm;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckEditor()
        {
            InitializeComponent();

            _checkManager = new CheckManager();
        }

        public CheckEditor(FileInfo checkAndFixFile)
        {
            InitializeComponent();

            _checkManager = new CheckManager();
            using (var fileStream = checkAndFixFile.OpenRead())
            {
                _checkAndFixItem = CheckAndFixItem.LoadFromXmlContent(fileStream);
            }
        }

        /// <summary>
        /// On dialog load, set to 'new' state
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void CheckEditor_Load(object sender, EventArgs e)
        {
            if (_checkAndFixItem == null)
            {
                newToolStripMenuItem_Click(sender, e);
            }
            updateUI();
            _dirty = false;
            setScintillaRecipe();
        }

        /// <summary>
        /// Set the form to a "new" state; a brand new check/fix item to edit
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // prevent overwriting changes unless explicit
            if (_dirty)
            {
                DialogResult dialogResult = MessageBox.Show("You have unsaved changes, are you sure you wish to proceed?", "Verify", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.No)
                {
                    return;
                }

            }

            _checkAndFixItem = new CheckAndFixItem();
            _checkAndFixItem.CheckScript = @"function checkAndFix(checkResultItems) {
  return checkResultItems
}
";

            _checkAndFixItem.Id = Guid.NewGuid().ToString();
            updateUI();

            checkFixIdLabel.Text = _checkAndFixItem.Id;

            scopeCombo.SelectedItem = "VERSE";
            _dirty = false;

        }

        /// <summary>
        /// Open a check/fix file for editing
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
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
        /// Save the file that represents the check/fix
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dirty)
            {
                updateCheckAndFix();

                if (String.IsNullOrEmpty(_checkAndFixItem.Name.Trim()) ||
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
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Save and Publish the check/fix
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
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
        /// Worker for doing publish updates asynchronously
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
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
        /// Callback for when the async worker is complete
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
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
            tagsTextBox.Text = _checkAndFixItem.Tags == null ? "" : string.Join(", ", _checkAndFixItem.Tags);
            descriptionTextBox.Text = _checkAndFixItem.Description;

            checkFindRegExTextBox.Text = _checkAndFixItem.CheckRegex ?? "";
            fixRegExTextBox.Text = _checkAndFixItem.FixRegex ?? "";
            jsEditor.Text = _checkAndFixItem.CheckScript == null ? "" : _checkAndFixItem.CheckScript.Replace("\n", Environment.NewLine);
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
                _checkAndFixItem.CheckScript = jsEditor.Text;
            }
            catch
            {
                MessageBox.Show("Name, Version, Default Description, and either the Check Regex or the Check Script, must be entered.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// Keep track of changes and mark the form dirty
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void content_TextChanged(object sender, EventArgs e)
        {
            _dirty = true;
        }

        /// <summary>
        /// Update the help text for the regex control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checkFindRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression to find issues. This value may be empty if the check/fix relies on JavaScript to perform modifications.");
        }

        /// <summary>
        /// Update the help text for the fix regex control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void FixRegExTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The regular expression replacement pattern, using $1 type replacement values from the groupings found in the check find regex.");
        }

        /// <summary>
        /// Update the help text for the check/fix id label control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checkFixIdLabel_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The automatically assigned unique identifier.");
        }

        /// <summary>
        /// Update the help text for the check/fix name text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void checkFixNameTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The name of your check/fix.");
        }

        /// <summary>
        /// Update the help text for the version text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void versionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The version of the check/fix. Increment each time you publish an update. Use semantic versioning scheme: https://semver.org/");
        }

        /// <summary>
        /// Update the help text for the scope combo box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void scopeCombo_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("The text scope to run this check/fix at; PROJECT, BOOK, CHAPTER, VERSE" + Environment.NewLine);
            helpTextBox.AppendText("Leave defaulted to VERSE if unsure.");
        }

        /// <summary>
        /// Update the help text for the check/fix description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void defaultDescTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("This is the default description associated with matched results.");
        }

        /// <summary>
        /// Update the help text for the languages text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void languagesTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Languages associated with this check/fix. Separate lanugages by comma." + Environment.NewLine);
            helpTextBox.AppendText("Use language codes found in projects like eng-US, zh, ja, etc.");
        }

        /// <summary>
        /// Update the help text for the tags text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void tagsTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("Enter Tags associated with this check/fix. Separate tags by comma." + Environment.NewLine);
            helpTextBox.AppendText("Currently supported tags: RTL = right to left only languages.");
        }

        /// <summary>
        /// Update the help text for the description text box
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void descriptionTextBox_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.Text = "Enter the full description for this check/fix.";
        }

        /// <summary>
        /// Creates and sets the style information and keywords for JavaScript
        /// </summary>
        private void setScintillaRecipe()
        {

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            jsEditor.StyleResetDefault();
            jsEditor.Styles[Style.Default].Font = "Consolas";
            jsEditor.Styles[Style.Default].Size = 10;
            jsEditor.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            jsEditor.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
            jsEditor.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
            jsEditor.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
            jsEditor.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
            jsEditor.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
            jsEditor.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
            jsEditor.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
            jsEditor.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
            jsEditor.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
            jsEditor.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
            jsEditor.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
            jsEditor.Lexer = Lexer.Cpp;

            jsEditor.SetKeywords(0, this.jsKeywords);
        }

        /// <summary>
        /// On text change, look to increase the size of the margin to handle full line numbers
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void jsEditor_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = jsEditor.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            jsEditor.Margins[0].Width = jsEditor.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;
        }

        /// <summary>
        /// While characters are being added, allow for hints for keywords
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void jsEditor_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = jsEditor.CurrentPosition;
            var wordStartPos = jsEditor.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                if (!jsEditor.AutoCActive)
                    jsEditor.AutoCShow(lenEntered, this.jsKeywords);
            }
        }

        /// <summary>
        /// Update the help text for the JavaScript control
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void jsEditor_MouseEnter(object sender, EventArgs e)
        {
            helpTextBox.Clear();
            helpTextBox.AppendText("JavaScript that can be called after the two regular expressions are run, if they are defined." + Environment.NewLine);
            helpTextBox.AppendText("This script MUST implement the function checkAndFix(checkResultItems). The CheckResultItems are the results" +
                " found in the regular expression pass.");
        }

        /// <summary>
        /// Handles closing of the form via the "X" button
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void onFormClose_Click(object sender, FormClosingEventArgs e)
        {
            if (_dirty)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you wish to exit without saving?", "Exit?", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
