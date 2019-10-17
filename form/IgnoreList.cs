using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace translation_validation_framework.form
{
    public partial class IgnoreList : Form
    {
        public IgnoreList()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.Rows.Add("MAT, 9, 15", "(text), I", "Add to ignore list");
            this.dataGridView1.Rows.Add("FOO, 9, 15", "foo, Bar", "This is an error");
            this.dataGridView1.Rows.Add("GEN 10, 2", "(text), God", "Add to ignore list");
            this.dataGridView1.Rows.Add("GEN, 2, 8", "(text); Adam", "Add to ignore list");
            this.dataGridView1.Rows.Add("MAT, 10, 3", "(text); I", "Add to ignore list");
            this.dataGridView1.Rows.Add("MAT, 15, 10", "(text); In", "This is an error");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
