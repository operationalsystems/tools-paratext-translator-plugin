using System;
using TvpMain.Data;

namespace TvpMain.Check
{
    public interface ITextCheck
    {
        public event EventHandler<int> CheckUpdated;
        public CheckResult RunCheck();
        public void CancelCheck();
    }
}
