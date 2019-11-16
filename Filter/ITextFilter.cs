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
        public bool FilterText(int bookNum, int chapterNum, int verseNum, string inputText);
    }
}
