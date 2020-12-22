using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.Project;

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

            bookList.Items.AddRange(projectManager.BookNamesByNum.Values.ToArray());

            foreach( BookNameItem bookNameItem in selectedBooks) {
                int idx = bookList.FindString(bookNameItem.ToString());
                bookList.SetSelected(idx, true);
            }
        }

        /// <summary>
        /// Get the list of the selected books
        /// </summary>
        /// <returns>The list of selected books by <see cref="BookNameItem"/></returns>
        public BookNameItem[] GetSelected() {
            
            return bookList.SelectedItems.Cast<BookNameItem>().ToArray();
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
            for( int i = 0; i < bookList.Items.Count; i++)
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
    }
}
