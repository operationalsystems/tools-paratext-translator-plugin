using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddInSideViews;
using TvpMain.Data;

namespace TvpMain.Filter
{
    class FilterResult
    {
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private readonly ConcurrentQueue<ResultItem> _resultItems;

        public ConcurrentQueue<ResultItem> ResultItems => _resultItems;
        public void RunFilter()
        {
            try
            {
                CheckResult result = new CheckResult(this._host, this._activeProjectName);
                string versificationName = _host.GetProjectVersificationName(this._activeProjectName);
                // Output result items to a text stream
                // Do some string manipulation in order to get the error isolated
                // Store that result into an array list

                // Create an arbitrary word list
                ArrayList testList = new ArrayList();

                // Adding words to the Test Word List
                testList.Add("God");
                testList.Add("I");
                testList.Add("the");

                // compare those results against the created arbitrary word list
                // update result items object and update UI
            }
            catch (Exception ex)
            {

            }
        }
        //public CheckResult GetLastResult()
        //{
        //return this.lastResult;
        //}
    }
}
