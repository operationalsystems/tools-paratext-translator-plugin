using AddInSideViews;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TvpMain.Data;
using TvpMain.Util;

/*
 * The first validation check in the Translation Validation Plugin framework.
 */
namespace TvpMain.Check
{
    class PunctuationCheck1 : ITextCheck
    {
        private static readonly Regex _checkRegex;
        private readonly TranslationValidationPlugin _plugin;
        private readonly IHost _host;
        private readonly string _activeProjectName;
        private CheckResult _lastResult;
        private CancellationTokenSource _tokenSource;
        private int _currProgress;

        public event EventHandler<CheckResult> ResultHandler;
        public event EventHandler<int> ProgressHandler;

        public int CurrProgress { get => _currProgress; }

        /*
         * Regex to check for improper capitlization following non-final punctuation (E.g. <text>, And <text>).
         */
        static PunctuationCheck1()
        {
            _checkRegex = new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
                RegexOptions.Multiline);
        }

        public PunctuationCheck1(TranslationValidationPlugin plugin, IHost host, string activeProjectName)
        {
            this._plugin = plugin;
            this._host = host;
            this._activeProjectName = activeProjectName;
        }

        /*
         * 
         */
        private class ExtractorState
        {
            private readonly IScrExtractor extractor;
            private int subTotal;

            public IScrExtractor Extractor { get => extractor; }
            public int SubTotal { get => subTotal; set => subTotal = value; }

            public ExtractorState(IScrExtractor extractor)
            {
                this.extractor = extractor ?? throw new ArgumentNullException(nameof(extractor));
                subTotal = 0;
            }
        }

        public void CancelCheck()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
        }

        public void RunCheck()
        {
            try
            {
                CheckResult result = new CheckResult(_host, _activeProjectName);

                string versificationName = _host.GetProjectVersificationName(_activeProjectName);
                _currProgress = 0;

                _tokenSource = new CancellationTokenSource();

                // Provides multiple threads in order to increase the speed of the Puncutation check.
                // Use type parameter to make subtotal a long, not an int
                Parallel.For(1, MainConsts.MAX_BOOK_NUM + 1,
                    new ParallelOptions { MaxDegreeOfParallelism = MainConsts.MAX_CHECK_THREADS, CancellationToken = _tokenSource.Token },
                    () => new ExtractorState(_host.GetScriptureExtractor(_activeProjectName, ExtractorType.USFM)),
                    (bookNum, loopState, extractorState) =>
                  {
                      int currBookNum = bookNum;
                      int currChapterNum = 0;
                      int currVerseNum = 0;

                      try
                      {
                          /*
                           * Handles looping over the entire Bible and storing the results to be indexed.
                           */
                          int lastChapterNum = _host.GetLastChapter(bookNum, versificationName);
                          for (int chapterNum = 1; chapterNum <= lastChapterNum; chapterNum++)
                          {
                              currChapterNum = chapterNum;
                              int lastVerseNum = _host.GetLastVerse(bookNum, chapterNum, versificationName);

                              for (int verseNum = 1; verseNum <= lastVerseNum; verseNum++)
                              {
                                  currVerseNum = verseNum;
                                  int coord = bookNum * 1000000;
                                  coord += chapterNum * 1000;
                                  coord += verseNum;

                                  string verseText = extractorState.Extractor.Extract(coord, coord);

                                  foreach (Match matchItem in _checkRegex.Matches(extractorState.Extractor.Extract(coord, coord)))
                                  {

                                      ResultItem resultItem = new ResultItem(bookNum, chapterNum, verseNum,
                                          $"Punctuation check failure at position {matchItem.Index}.", verseText);
                                      result.ResultItems.Enqueue(resultItem);

                                      // this.host.WriteLineToLog(this.plugin, resultItem.ToString());
                                  }

                                  if (_tokenSource.IsCancellationRequested)
                                  {
                                      return extractorState;
                                  }
                                  ProgressHandler?.Invoke(this, _currProgress);
                              }
                          }


                          extractorState.SubTotal++;

                          lock (this)
                          {
                              _currProgress++;
                              //this.ProgressHandler?.Invoke(this, this.currProgress);

                              if (_currProgress >= MainConsts.MAX_BOOK_NUM)
                              {
                                  _lastResult = result;
                                  ResultHandler?.Invoke(this, _lastResult);
                              }
                          }
                      }
                      catch (Exception ex)
                      {
                          ErrorUtil.ReportError($"Location: {currBookNum}.{currChapterNum}.{currVerseNum}", ex);
                      }

                      return extractorState;
                  },
                    (extractorState) =>
                    {
                        // ignore, at this time
                    });
            }
            catch (OperationCanceledException ex)
            {
                // Eating this exception for now.
            }
            catch (Exception ex)
            {
                ErrorUtil.ReportError(ex);
            }
        }

        public CheckResult GetLastResult()
        {
            return _lastResult;
        }

    }


}
