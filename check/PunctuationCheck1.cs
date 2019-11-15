using AddInSideViews;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using translation_validation_framework.util;

/*
 * The first validation check in the Translation Validation Plugin framework.
 */
namespace translation_validation_framework
{
    class PunctuationCheck1 : ITextCheck
    {
        private static readonly Regex checkRegex;
        private readonly TranslationValidationPlugin plugin;
        private readonly IHost host;
        private readonly string activeProjectName;
        private CheckResult lastResult;
        public event EventHandler<CheckResult> ResultHandler;
        public event EventHandler<int> ProgressHandler;
        private CancellationTokenSource tokenSource;
        private int currProgress;

        public int CurrProgress { get => currProgress; }

        /*
         * Regex to check for improper capitlization following non-final punctuation (E.g. <text>, And <text>).
         */
        static PunctuationCheck1()
        {
            PunctuationCheck1.checkRegex = new Regex("(?<=[;,]([\"'](\\s[\"'])*)?(\\\\f([^\\\\]|\\\\(?!f\\*))*?\\\\f\\*)*(\\s*\\\\\\w+)+(\\s*\\\\v\\s\\S+)?\\s+(\\\\x([^\\\\]|\\\\(?!x\\*))*?\\\\x\\*)?)[A-Z]\\w+",
                RegexOptions.Multiline);
        }

        public PunctuationCheck1(TranslationValidationPlugin plugin, IHost host, string activeProjectName)
        {
            this.plugin = plugin;
            this.host = host;
            this.activeProjectName = activeProjectName;
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
                this.subTotal = 0;
            }
        }

        public void CancelCheck()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
        }

        public void RunCheck()
        {
            try
            {
                CheckResult result = new CheckResult(this.host, this.activeProjectName);

                string versificationName = host.GetProjectVersificationName(this.activeProjectName);
                this.currProgress = 0;

                tokenSource = new CancellationTokenSource();

                // Provides multiple threads in order to increase the speed of the Puncutation check.
                // Use type parameter to make subtotal a long, not an int
                Parallel.For<ExtractorState>(1, (MainConsts.MAX_BOOK_NUM + 1),
                    new ParallelOptions { MaxDegreeOfParallelism = MainConsts.MAX_CHECK_THREADS, CancellationToken = tokenSource.Token },
                    () => new ExtractorState(this.host.GetScriptureExtractor(this.activeProjectName, ExtractorType.USFM)),
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
                          int lastChapterNum = this.host.GetLastChapter(bookNum, versificationName);
                          for (int chapterNum = 1; chapterNum <= lastChapterNum; chapterNum++)
                          {
                              currChapterNum = chapterNum;
                              int lastVerseNum = this.host.GetLastVerse(bookNum, chapterNum, versificationName);

                              for (int verseNum = 1; verseNum <= lastVerseNum; verseNum++)
                              {
                                  currVerseNum = verseNum;
                                  int coord = bookNum * 1000000;
                                  coord += chapterNum * 1000;
                                  coord += verseNum;

                                  string verseText = extractorState.Extractor.Extract(coord, coord);

                                  foreach (Match matchItem in checkRegex.Matches(extractorState.Extractor.Extract(coord, coord)))
                                  {

                                      ResultItem resultItem = new ResultItem(bookNum, chapterNum, verseNum, 
                                          $"Punctuation check failure at position {matchItem.Index}.", verseText);
                                      result.ResultItems.Enqueue(resultItem);

                                      // this.host.WriteLineToLog(this.plugin, resultItem.ToString());
                                  }

                                  if (tokenSource.IsCancellationRequested)
                                  {
                                      return extractorState;
                                  }
                                  this.ProgressHandler?.Invoke(this, this.currProgress);
                              }
                          }


                          extractorState.SubTotal++;

                          lock (this)
                          {
                              this.currProgress++;
                              //this.ProgressHandler?.Invoke(this, this.currProgress);

                              if (this.currProgress >= MainConsts.MAX_BOOK_NUM)
                              {
                                  this.lastResult = result;
                                  this.ResultHandler?.Invoke(this, this.lastResult);
                              }
                          }
                      }
                      catch (Exception ex)
                      {
                          ErrorUtil.reportError($"Location: {currBookNum}.{currChapterNum}.{currVerseNum}", ex);
                      }

                      return extractorState;
                  },
                    (extractorState) =>
                    {
                        // ignore, at this time
                    });
            }
            catch (OperationCanceledException e)
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
            return this.lastResult;
        }

    }


}
