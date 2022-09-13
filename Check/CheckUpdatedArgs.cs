/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace TvpMain.Check
{
    /// <summary>
    /// Args for check update events.
    /// </summary>
    public class CheckUpdatedArgs
    {
        /// <summary>
        /// Max book position in check (1 for single-book check, 66 for all books, etc.).
        /// </summary>
        public int TotalBooks { get; }

        /// <summary>
        /// Current book count in check.
        /// </summary>
        public int BookCtr { get; }

        /// <summary>
        /// Current book number in update (1-based).
        /// </summary>
        public int BookNum { get; }

        /// <summary>
        /// Basic ctor;
        /// </summary>
        /// <param name="bookCtr">Current book count in check.</param>
        /// <param name="totalBooks">Max book position in check.</param>
        public CheckUpdatedArgs(int totalBooks, int bookCtr, int bookNum)
        {
            this.TotalBooks = totalBooks > 0 ? totalBooks : 0;
            this.BookCtr = bookCtr > 0 ? bookCtr : 0;
            this.BookNum = bookNum > 0 ? bookNum : 0;
        }
    }
}
