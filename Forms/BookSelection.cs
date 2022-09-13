/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Project;
using TvpMain.Text;

namespace TvpMain.Forms
{
    /// <summary>
    /// This form allows for selecting sets of books from the current project.
    /// It assumes Old and New testament are present.
    /// </summary>
    public partial class BookSelection : Form
    {
        private const int OT_COUNT = 39;
        private const int CANON_BOOK_COUNT = 66;
        private ProjectManager _projectManager;

        /// <summary>
        /// Initial Ctor for use when no books are selected already.
        /// Pass in the project manager so we can get the actual books in the project, 
        /// including extra material.
        /// </summary>
        /// <param name="projectManager"></param>
        public BookSelection(ProjectManager projectManager)
        {
            InitializeComponent();
            _projectManager = projectManager;

            bookList.Items.AddRange(projectManager.BookNamesByNum.Values.ToArray());
        }

        /// <summary>
        /// Used to specify already selected books, like when reselecting.
        /// </summary>
        /// <param name="projectManager"></param>
        /// <param name="selectedBooks"></param>
        public BookSelection(ProjectManager projectManager, BookNameItem[] selectedBooks)
        {
            InitializeComponent();
            _projectManager = projectManager;

            bookList.Items.AddRange(projectManager.BookNamesByNum.Values.Select(item => item.BookCode).ToArray());

            foreach (BookNameItem bookNameItem in selectedBooks)
            {
                int idx = bookList.FindString(bookNameItem.BookCode);
                bookList.SetSelected(idx, true);
            }
        }

        /// <summary>
        /// Get the list of the selected books
        /// </summary>
        /// <returns>The list of selected books by <see cref="BookNameItem"/></returns>
        public BookNameItem[] GetSelected()
        {
            // Return a BookNameItem based on the BookCode strings passed in from the BookSelection
            return bookList.SelectedItems.Cast<String>().Select(item => 
            _projectManager.BookNamesByNum[BookUtil.BookIdsByCode[item].BookNum]).ToArray();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BookSelection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Select all the books in the list
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void allBooksButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        /// <summary>
        /// Add all OT books to selected set
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>

        private void otButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < OT_COUNT; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        /// <summary>
        /// Add all NT books to selected set
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void ntButton_Click(object sender, EventArgs e)
        {
            for (int i = OT_COUNT; i < CANON_BOOK_COUNT; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        /// <summary>
        /// Add all extra material to selected  set
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void extraButton_Click(object sender, EventArgs e)
        {
            for (int i = CANON_BOOK_COUNT; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        /// <summary>
        /// Deselect all books to start from scratch
        /// </summary>
        /// <param name="sender">The control that sent this event</param>
        /// <param name="e">The event information that triggered this call</param>
        private void deselectButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, false);
            }
        }

        /// <summary>
        /// Used to display the list of books selected. If there are more than 4 then truncate in the middle.
        /// </summary>
        /// <param name="selectedBooks">This allows for passing in the currently selected books, 
        /// in case this dialog has already been used previously, 
        /// to prefill the dialog with
        /// the currently selected items.</param>
        /// <returns>A string created by the list of <see cref="BookNameItem"/>s, if greater than 4, ellipsized.</returns>
        public static string stringFromSelectedBooks(BookNameItem[] selectedBooks)
        {
            string names = "";

            if (selectedBooks.Length > 5)
            {
                names = selectedBooks[0].BookCode.ToString() 
                    + ", " + selectedBooks[1].BookCode.ToString()  
                    + ", " + selectedBooks[2].BookCode.ToString()  
                    + ", " + selectedBooks[3].BookCode.ToString()
                    + ", ..., " 
                    + selectedBooks[selectedBooks.Length - 1].BookCode.ToString();
            }
            else
            {
                names = string.Join(", ", Array.ConvertAll<BookNameItem, string>(selectedBooks, bni => bni.BookCode.ToString()));
            }

            return names;
        }

    }
}
