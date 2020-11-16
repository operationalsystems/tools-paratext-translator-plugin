using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    class CheckAndFixRunner : ICheckAndFixRunner
    {
        public List<CheckResultItem> ExecCheckAndFix(string text, CheckAndFixItem checkAndFixItem)
        {
            return new List<CheckResultItem>();
        }
    }
}
