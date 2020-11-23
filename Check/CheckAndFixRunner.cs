using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvpMain.Check
{
    public class CheckAndFixRunner : ICheckAndFixRunner
    {
        

        public List<CheckResultItem> ExecCheckAndFix(string text, CheckAndFixItem checkAndFixItem)
        {
            using (var engine = new V8ScriptEngine())
            {

            }
            return new List<CheckResultItem>();
        }
    }
}
