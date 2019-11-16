using System;
using TvpMain.Data;

namespace TvpMain.Check
{
    public interface ITextCheck
    {
        public event EventHandler<CheckResult> CheckCompleted;

        public event EventHandler<int> CheckUpdated;

        public void RunCheck();
        public void CancelCheck();

        public CheckResult LastResult { get; }
    }
}
