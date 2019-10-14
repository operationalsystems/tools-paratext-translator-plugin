using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using translation_validation_framework.util;

/*
 * Form to provide progress of Translation Validation Checks.
 */
namespace translation_validation_framework.form
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        public void SetTitle(string titleText)
        {
            this.lblTitle.Text = titleText;
        }
        /*
         * Show progress of books being checked against the validation check(s) being used.
         */
        public void setCurrBookNum(int bookNum)
        {
            this.pbrStatus.Maximum = MainConsts.MAX_BOOK_NUM;
            this.pbrStatus.Value = bookNum;

            this.lblTitle.Text = $"Checked book #{bookNum} of {MainConsts.MAX_BOOK_NUM}...";
        }
    }
}
