using System;
using System.Windows.Forms;

namespace TvpMain.Form
{
    public partial class IgnoreListForm : System.Windows.Forms.Form
    {
        public IgnoreListForm()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
