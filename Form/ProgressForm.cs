using System;
using System.Text;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.Util;

/*
 * Form to provide progress of Translation Validation Checks.
 */
namespace TvpMain.Form
{
    /// <summary>
    /// Validation progress form.
    /// </summary>
    public partial class ProgressForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Start time for timer label.
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// Last book number processed in current run.
        /// </summary>
        private int _lastBookNum;

        /// <summary>
        /// Max book number in current run.
        /// </summary>
        private int _maxBookNum;

        /// <summary>
        /// Cancel event handler, for use by workflow.
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public ProgressForm()
        {
            InitializeComponent();
            ResetForm();
        }

        /// <summary>
        /// On notification validation updates.
        /// </summary>
        /// <param name="updatedArgs"></param>
        public void OnCheckUpdated(CheckUpdatedArgs updatedArgs)
        {
            lock (this)
            {
                _lastBookNum = updatedArgs.CurrPos;
                _maxBookNum = updatedArgs.MaxPos;
            }
        }

        /// <summary>
        /// Cancellation click handler.
        /// </summary>
        /// <param name="sender">Sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnCancelClick(object sender, EventArgs e)
        {
            pbrStatus.Value = pbrStatus.Minimum;
            pbrStatus.Style = ProgressBarStyle.Marquee;

            lblTitle.Text = $"Cancelling Validation...";
            Cancelled?.Invoke(sender, e);
        }

        public void ResetForm()
        {
            _startTime = DateTime.Now;
            ResetFormContents();
        }

        private void ResetFormContents()
        {
            pbrStatus.Value = pbrStatus.Minimum;
            pbrStatus.Style = ProgressBarStyle.Marquee;

            lblTitle.Text = $"Running Validation...";
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
        /// <param name="e">Event args (ignored).</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled?.Invoke(sender, e);
        }

        /// <summary>
        /// Timer update method that synchronizes progress bar, label text, and elapsed time label.
        /// </summary>
        /// <param name="sender">Event source (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnTimerUpdate(object sender, EventArgs e)
        {
            lblElapsedTime.Text = GetElapsedTime(DateTime.Now.Subtract(_startTime));
            lock (this)
            {
                if (_lastBookNum != pbrStatus.Value
                    || _maxBookNum != pbrStatus.Maximum)
                {
                    pbrStatus.Maximum = _maxBookNum;

                    if (_lastBookNum > pbrStatus.Maximum
                        || _lastBookNum <= pbrStatus.Minimum)
                    {
                        ResetFormContents();
                    }
                    else
                    {
                        pbrStatus.Value = _lastBookNum;
                        pbrStatus.Style = ProgressBarStyle.Continuous;

                        lblTitle.Text = $"Checked book #{_lastBookNum} of {_maxBookNum}...";
                    }

                    Activate();
                }
            }
        }
    }
}
