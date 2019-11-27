using System;
using TvpMain.Data;

namespace TvpMain.Check
{
    /// <summary>
    /// General-purpose text check that's expected to be long-running.
    /// 
    /// This will probably evolve to be an aggregate of multiple, specific checks 
    /// that may be selectively enabled or disabled or decomposed in some way. 
    /// 
    /// Reason being, extracting verses is CPU and I/O intensive because it crosses 
    /// the process boundary, so for best results we want (n) checks performed on each
    /// verse, vs each check extracting all verses (n) times. 
    /// </summary>
    public interface ITextCheck
    {
        /// <summary>
        /// Event handler for updates.
        /// </summary>
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;

        /// <summary>
        /// Start the check.
        /// 
        /// Expected to busy-wait on the calling thread with a FPS-based periodic sleep
        /// and DoEvents() call to keep the programming model simple and the UI responsive.
        /// </summary>
        /// <param name="checkScope">Check scope (project, book, chapter; required).</param>
        /// <returns>Check result.</returns>
        public CheckResult RunCheck(CheckArea checkScope);

        /// <summary>
        /// Cancels an in-progress run.
        /// </summary>
        public void CancelCheck();
    }
}
