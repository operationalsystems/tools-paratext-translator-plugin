using System;
using TvpMain.Data;

namespace TvpMain.Check
{
    interface ITextCheck
    {
        public event EventHandler<CheckResult> ResultHandler;

        public event EventHandler<int> ProgressHandler;

        public void RunCheck();
        public void CancelCheck();

        public CheckResult GetLastResult();
    }
}
