using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <summary>
        /// A collection of the <c>CheckAndFixItem</c>s to run against the content.
        /// </summary>
        private List<CheckAndFixItem> ChecksToRun { get; set; }

        /// <summary>
        /// The collection of <c>CheckResultItem</c>s mapped by the associated <c>CheckAndFixItem</c>.
        /// </summary>
        Dictionary<CheckAndFixItem, List<CheckResultItem>> CheckResults { get; set; }

        public CheckResultsForm(List<CheckAndFixItem> checksToRun)
        {
            // validate inputs
            ChecksToRun = checksToRun ?? throw new ArgumentNullException(nameof(checksToRun));

            // initialize the components
            InitializeComponent();

            // run the provided tests

        }

        /// <summary>
        /// This method is called when the form is cancelled.
        /// </summary>
        public void OnCancel()
        {
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            OnCancel();
        }
    }
}
