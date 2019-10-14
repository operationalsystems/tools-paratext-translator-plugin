using AddInSideViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Class to track all result items necessary for the Translation Validation Plugin
 */
namespace translation_validation_framework
{
    public class ResultItem
    {
        private readonly int bookNum;

        private readonly int chapterNum;

        private readonly int verseNum;

        private readonly string errorText;

        public int BookNum => bookNum;

        public int ChapterNum => chapterNum;

        public int VerseNum => verseNum;

        public string ErrorText => errorText;

        public ResultItem(int bookNum, int chapterNum, int verseNum, string errorText)
        {
            this.bookNum = bookNum;
            this.chapterNum = chapterNum;
            this.verseNum = verseNum;
            this.errorText = errorText ?? throw new ArgumentNullException(nameof(errorText));
        }
        /*
         * Comma separated output of the error in the open Project TVP Main form text box from the Punctuation check run.
         */
        public override string ToString()
        {
            return $"{bookNum},{chapterNum},{verseNum} = {errorText}";
        }

        public int Coordinate => (this.bookNum * 1000000) + (this.chapterNum * 1000) + this.verseNum;
    }
}
