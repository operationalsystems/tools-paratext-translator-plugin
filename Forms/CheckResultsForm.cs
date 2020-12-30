using System;
using System.Windows.Forms;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
        // Whether to show results which have been denied
        private bool _showDenied = false;

        /// <summary>
        /// Default contstructor
        /// </summary>
        public CheckResultsForm()
        {
            InitializeComponent();
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
