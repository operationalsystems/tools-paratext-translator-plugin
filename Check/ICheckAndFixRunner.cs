using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    interface ICheckAndFixRunner
    {
        public List<CheckResultItem> ExecCheckAndFix(string text, CheckAndFixItem checkAndFixItem );
    }
}
