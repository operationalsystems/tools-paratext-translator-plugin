namespace TvpMain.Export
{
    /// <summary>
    /// Args for export update events.
    /// </summary>
    public class ExportUpdatedArgs
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
        public ExportUpdatedArgs(int totalBooks, int bookCtr, int bookNum)
        {
            this.TotalBooks = totalBooks;
            this.BookCtr = bookCtr;
            this.BookNum = bookNum;
        }
    }
}
