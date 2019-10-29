using AddInSideViews;
using System;
using System.Collections.Concurrent;

/*
 * A class to handle results from validation checks.
 */
namespace translation_validation_framework
{
    public class CheckResult
    {
        private readonly IHost host;

        private readonly string activeProjectName;

        private readonly ConcurrentQueue<ResultItem> resultItems;

        public IHost Host => host;
        public string ActiveProjectName => activeProjectName;
        public ConcurrentQueue<ResultItem> ResultItems => resultItems;

        private IScrExtractor extractor;

        public CheckResult(IHost host, string activeProjectName)
        {
            this.host = host ?? throw new ArgumentNullException(nameof(host));
            this.activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
            this.resultItems = new ConcurrentQueue<ResultItem>();
        }

        public IScrExtractor Extractor
        {
            get
            {
                lock (this)
                {
                    if (this.extractor == null)
                    {
                        this.extractor = this.host.GetScriptureExtractor(this.activeProjectName, ExtractorType.USFM);
                    }
                    return this.extractor;
                }
            }
        }
    }
}