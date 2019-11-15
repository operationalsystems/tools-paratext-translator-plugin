using System;
using System.Text;
using System.Windows.Forms;
using TvpMain.Util;

/*
 * Form to provide progress of Translation Validation Checks.
 */
namespace TvpMain.Form
{
    public partial class ProgressForm : System.Windows.Forms.Form
    {
        private readonly DateTime _startTime;

        /// <summary>
        /// Cancel event handler, for use by workflow.
        /// </summary>
        public event EventHandler Cancelled;

        public ProgressForm()
        {
            InitializeComponent();
            _startTime = DateTime.Now;
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
            Application.DoEvents();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            SetTitle("Cancelling Validation.  Please Wait...");
            Application.DoEvents();

            Cancelled?.Invoke(sender, e);
        }

        public void ResetForm()
        {
            SetTitle("Running Validation...");
            this.pbrStatus.Value = 0;
        }

        /// <summary>
        /// Helper method that reports time as "MM:SS" text form TimeSpan, showing the ":" only on odd seconds.
        /// </summary>
        /// <param name="timeSpan">Input time span (required).</param>
        /// <returns>Reported time text.</returns>
        private string GetElapsedTime(TimeSpan timeSpan)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Format("{0:D2}", (int)timeSpan.TotalMinutes));
            stringBuilder.Append(((timeSpan.Seconds % 2) == 0) ? " " : ":");
            stringBuilder.Append(string.Format("{0:D2}", timeSpan.Seconds));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Cancel event forwarder.
        /// </summary>
        /// <param name="sender">Event source (button).</param>
        /// <param name="e">Event args.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled?.Invoke(sender, e);
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            lblElapsedTime.Text = GetElapsedTime(DateTime.Now.Subtract(_startTime));
        }
    }
}
