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
    public partial class BookSelection : Form
    {
        private ProjectManager _projectManager;

        public BookSelection(ProjectManager projectManager)
        {
            InitializeComponent();
            _projectManager = projectManager;

            bookList.Items.AddRange(projectManager.BookNamesByNum.Values.ToArray());
        }

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

        public BookNameItem[] GetSelected() {
            
            return bookList.SelectedItems.Cast<BookNameItem>().ToArray();
        }  

        public BookSelection()
        {
            InitializeComponent();
        }

        private void allBooksButton_Click(object sender, EventArgs e)
        {
            for( int i = 0; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        private void otButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 39; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        private void ntButton_Click(object sender, EventArgs e)
        {
            for (int i = 39; i < 66; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        private void extraButton_Click(object sender, EventArgs e)
        {
            for (int i = 66; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, true);
            }
        }

        private void deselectButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < bookList.Items.Count; i++)
            {
                bookList.SetSelected(i, false);
            }
        }
    }
}
