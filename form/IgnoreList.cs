using System;
using System.Windows.Forms;

namespace TvpMain.Form
{
    public partial class IgnoreList : System.Windows.Forms.Form
    {
        public IgnoreList()
        {
            InitializeComponent();
            CreateDummyData();
        }

        private void CreateDummyData()
        {
            this.dataGridView1.Rows.Add("MAT, 9, 15", "(text), I", "Add to ignore list");
            this.dataGridView1.Rows.Add("FOO, 9, 15", "foo, Bar", "This is an error");
            this.dataGridView1.Rows.Add("GEN 10, 2", "(text), God", "Add to ignore list");
            this.dataGridView1.Rows.Add("GEN, 2, 8", "(text); Adam", "Add to ignore list");
            this.dataGridView1.Rows.Add("MAT, 10, 3", "(text); I", "Add to ignore list");
            this.dataGridView1.Rows.Add("MAT, 15, 10", "(text); In", "This is an error");
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
