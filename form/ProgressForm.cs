using System;
using System.Windows.Forms;
using translation_validation_framework.util;

/*
 * Form to provide progress of Translation Validation Checks.
 */
namespace translation_validation_framework.form
{
    public partial class ProgressForm : Form
    {
        private readonly CheckForm checkForm;

        public ProgressForm(CheckForm checkForm)
        {
            this.checkForm = checkForm;
            InitializeComponent();
        }


        public void SetTitle(string titleText)
        {
            this.lblTitle.Text = titleText;
        }
        /*
         * Show progress of books being checked against the validation check(s) being used.
         */
        public void SetCurrBookNum(int bookNum)
        {
            this.pbrStatus.Maximum = MainConsts.MAX_BOOK_NUM;
            this.pbrStatus.Value = bookNum;

            this.lblTitle.Text = $"Checked book #{bookNum} of {MainConsts.MAX_BOOK_NUM}...";
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SetTitle("Cancelling Validation.  Please Wait...");
            checkForm.CancelCheck();
            Application.DoEvents();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }

        public void ResetForm()
        {
            SetTitle("Running Validation...");
            this.pbrStatus.Value = 0;
        }
    }
}
