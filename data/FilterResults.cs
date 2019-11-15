using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TvpMain.Data;

/*
 * This class will handle the filter options in order to allow results to be as accurate as possible.
 */
namespace TvpMain.Data
{
    public class Filter
    {
        private readonly ConcurrentQueue<ResultItem> resultItems;

        public ConcurrentQueue<ResultItem> ResultItems => resultItems;
        // Put result items into a string
        // Do some string manipulation in order to get the error isolated
        // Store that result into an array list
        // compare those results against the created arbitrary word list
        // update result items object and update UI
    }
}
