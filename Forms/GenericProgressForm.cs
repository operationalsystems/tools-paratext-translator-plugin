using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TvpMain.Forms
{
    /// <summary>
    /// A generic progress form that allows for setting the title of the dialog. This is meant to not have
    /// any iteraction, just a spinning progress bar.
    /// </summary>
    public partial class GenericProgressForm : Form
    {
        /// <summary>
        /// Constructor for setting the title of the dialog.
        /// </summary>
        /// <param name="title"></param>
        public GenericProgressForm(string title)
        {
            InitializeComponent();
            this.Text = title;
        }
    }
}
