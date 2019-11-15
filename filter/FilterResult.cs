using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddInSideViews;

namespace translation_validation_framework.data
{
    class FilterResult
    {
        private readonly ConcurrentQueue<ResultItem> resultItems;
        private readonly IHost host;
        private readonly string activeProjectName;

        public ConcurrentQueue<ResultItem> ResultItems => resultItems;
        public void RunFilter()
        {
            try
            {
                CheckResult result = new CheckResult(this.host, this.activeProjectName);
                string versificationName = host.GetProjectVersificationName(this.activeProjectName);
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
