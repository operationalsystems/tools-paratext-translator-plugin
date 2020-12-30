using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TvpMain.Check;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
        // Deny Button Text
        readonly string DENY_BUTTON = "Deny";
        readonly string UNDENY_BUTTON = "Un-Deny";

        // A list of <c>CheckResultItem</c>s which have been denied
        private List<int> _denied { get; set; }

        // Whether to show results which have been denied
        private bool _showDenied = false;

        /// <summary>
        /// Default contstructor
        /// </summary>
        public CheckResultsForm()
        {
            InitializeComponent();

            LoadDeniedResults();
            UpdateDenyButton();
        }

        /// <summary>
        /// This method updates the text and state of the "Deny" button depending on what (if any) <c>CheckResultItem</c> is selected.
        /// </summary>
        private void UpdateDenyButton()
        {
            CheckResultItem selectedResult = GetSelectedResult();
            if (selectedResult != null)
            {
                Deny.Text = _denied.Contains(selectedResult.GetHashCode()) ? UNDENY_BUTTON : DENY_BUTTON;
                Deny.Enabled = true;
            }
            else
            {
                Deny.Text = DENY_BUTTON;
                Deny.Enabled = false;
            }
        }

        /// <summary>
        /// This method gets the currently-selected <c>CheckResultItem</c>.
        /// </summary>
        /// <returns></returns>
        private CheckResultItem GetSelectedResult()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method loads the denied <c>CheckResultItem</c>s from the project.
        /// </summary>
        private void LoadDeniedResults()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method saves the denied <c>CheckResultItem</c>s to the project.
        /// </summary>
        private void SaveDeniedResults()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called when the form is closed.
        /// </summary>
        public void OnClose()
        {
            this.Close();
        }

        /// <summary>
        /// This method handles a click event on the "Close" button.
        /// </summary>
        /// <param name="sender">The "Close" button</param>
        /// <param name="e">The event information</param>
        private void Close_Click(object sender, EventArgs e)
        {
            OnClose();
        }

        /// <summary>
        /// This method handles a click event on the "Deny" button.
        /// </summary>
        /// <param name="sender">The "Deny" button</param>
        /// <param name="e">The event information</param>
        private void Deny_Click(object sender, EventArgs e)
        {
            CheckResultItem selectedResult = GetSelectedResult();
            if (selectedResult == null) return;

            int selectedResultHashCode = selectedResult.GetHashCode();
            if (_denied.Contains(selectedResultHashCode))
            {
                _denied.Remove(selectedResultHashCode);
            }
            else
            {
                _denied.Add(selectedResultHashCode);
            }

            SaveDeniedResults();
        }

        /// <summary>
        /// This method handles checking/unchecking the "Show Denied" checkbox.
        /// </summary>
        /// <param name="sender">The "Show Denied" checkbox</param>
        /// <param name="e">The event information</param>
        private void ShowDenied_CheckedChanged(object sender, EventArgs e)
        {
            _showDenied = CheckState.Checked.Equals(ShowDenied.CheckState);
        }
    }
}
