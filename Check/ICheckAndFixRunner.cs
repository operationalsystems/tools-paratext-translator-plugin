using System.Collections.Generic;

namespace TvpMain.Check
{
    interface ICheckAndFixRunner
    {
        public List<CheckResultItem> ExecCheckAndFix(string inputText, CheckAndFixItem checkAndFixItem);
    }
}
