using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

namespace TvpMain.Filter
{
    public interface ITextFilter
    {
        public bool FilterText(ResultItem inputItem);

        public bool IsEmpty { get; }
    }
}
