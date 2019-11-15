using System;

/*
 * Class to track all result items necessary for the Translation Validation Plugin
 */
namespace TvpMain.Data
{
    public class ResultItem
    {
        private readonly int _bookNum;
        private readonly int _chapterNum;
        private readonly int _verseNum;
        private readonly string _errorText;
        private readonly string _verseText;

        public int BookNum => _bookNum;
        public int ChapterNum => _chapterNum;
        public int VerseNum => _verseNum;
        public string ErrorText => _errorText;
        public string VerseText => _verseText;

        public ResultItem(int bookNum, int chapterNum, int verseNum, string errorText, string verseText)
        {
            this._bookNum = bookNum;
            this._chapterNum = chapterNum;
            this._verseNum = verseNum;
            this._verseText = verseText;
            this._errorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
        }
        /*
         * Comma separated output of the error in the open Project TVP Main form text box from the Punctuation check run.
         */
        public override string ToString()
        {
            return $"{_bookNum},{_chapterNum},{_verseNum} = {_errorText}";
        }

        public int Coordinate => _bookNum * 1000000 + _chapterNum * 1000 + _verseNum;
    }
}
