using AddInSideViews;
using System;
using System.Collections.Concurrent;
using TvpMain.Data;

/*
 * A class to handle results from validation checks.
 */
namespace TvpMain.Data
{
    public class CheckResult
    {
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private readonly ConcurrentQueue<ResultItem> _resultItems;
        private IScrExtractor _scrExtractor;

        public IHost Host => _host;
        public string ActiveProjectName => _activeProjectName;
        public ConcurrentQueue<ResultItem> ResultItems => _resultItems;


        public CheckResult(IHost host, string activeProjectName)
        {
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._activeProjectName = activeProjectName ?? throw new ArgumentNullException(nameof(activeProjectName));
            _resultItems = new ConcurrentQueue<ResultItem>();
        }

        public IScrExtractor Extractor
        {
            get
            {
                lock (this)
                {
                    if (_scrExtractor == null)
                    {
                        _scrExtractor = _host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM);
                    }
                    return _scrExtractor;
                }
            }
        }
    }
}