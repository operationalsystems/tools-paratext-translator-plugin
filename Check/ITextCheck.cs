using System;
using TvpMain.Data;

namespace TvpMain.Check
{
    public interface ITextCheck
    {
        public event EventHandler<CheckUpdatedArgs> CheckUpdated;
        public CheckResult RunCheck(CheckArea checkScope);
        public void CancelCheck();
    }
}
