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
        private readonly string _matchText;

        public int BookNum => _bookNum;
        public int ChapterNum => _chapterNum;
        public int VerseNum => _verseNum;
        public string ErrorText => _errorText;
        public string VerseText => _verseText;
        public string MatchText => _matchText;

        public ResultItem(int bookNum, int chapterNum, int verseNum, string errorText, string verseText, string matchText)
        {
            this._bookNum = bookNum;
            this._chapterNum = chapterNum;
            this._verseNum = verseNum;
            this._errorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
            this._verseText = verseText ?? throw new ArgumentNullException(nameof(verseText));
            this._matchText = matchText ?? throw new ArgumentNullException(nameof(matchText));
        }

        public int Coordinate => _bookNum * 1000000 + _chapterNum * 1000 + _verseNum;
    }
}
