using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace translation_validation_framework.data
{
    interface IFilter
    {
        public event EventHandler<CheckResult> ResultHandler;

        public void RunFilter();

        public CheckResult GetLastResult();
    }
}
