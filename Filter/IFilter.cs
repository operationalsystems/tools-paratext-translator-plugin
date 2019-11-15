using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    interface IFilter
    {
        public void RunFilter();

        public CheckResult GetLastResult();

        public event EventHandler<CheckResult> ResultHandler;
    }
}
