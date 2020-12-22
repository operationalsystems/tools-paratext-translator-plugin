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
