using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form displays information about the checks which have been run against a project
    /// as well as options for viewing and fixing any found errors.
    /// </summary>
    public partial class CheckResultsForm : Form
    {
        /// <summary>
        /// Default contstructor
        /// </summary>
        public CheckResultsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when the form is cancelled.
        /// </summary>
        public void OnCancel()
        {
            this.Close();
        }

        /// <summary>
        /// This method handles a click event on the "Save" button.
        /// </summary>
        /// <param name="sender">The "Save" button</param>
        /// <param name="e">The event information</param>
        private void Save_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method handles a click event on the "Cancel" button.
        /// </summary>
        /// <param name="sender">The "Cancel" button</param>
        /// <param name="e">The event information</param>
        private void Cancel_Click(object sender, EventArgs e)
        {
            OnCancel();
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

        }

        
    }
}
