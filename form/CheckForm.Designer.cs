namespace translation_validation_framework
{
    partial class FormTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.runCheck = new System.Windows.Forms.Button();
            this.txtData = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bcv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chapter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.verse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // runCheck
            // 
            this.runCheck.Location = new System.Drawing.Point(713, 415);
            this.runCheck.Name = "runCheck";
            this.runCheck.Size = new System.Drawing.Size(75, 23);
            this.runCheck.TabIndex = 1;
            this.runCheck.Text = "Run";
            this.runCheck.UseVisualStyleBackColor = true;
            this.runCheck.Click += new System.EventHandler(this.Run_Click);
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(12, 12);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(776, 189);
            this.txtData.TabIndex = 0;
            this.txtData.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bcv,
            this.chapter,
            this.verse,
            this.error});
            this.dataGridView1.Location = new System.Drawing.Point(12, 207);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(776, 202);
            this.dataGridView1.TabIndex = 2;
            // 
            // bcv
            // 
            this.bcv.HeaderText = "Book";
            this.bcv.Name = "bcv";
            // 
            // chapter
            // 
            this.chapter.HeaderText = "Chapter";
            this.chapter.Name = "chapter";
            // 
            // verse
            // 
            this.verse.HeaderText = "Verse";
            this.verse.Name = "verse";
            // 
            // error
            // 
            this.error.HeaderText = "Error";
            this.error.Name = "error";
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(800, 446);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.runCheck);
            this.Controls.Add(this.txtData);
            this.Name = "FormTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormTest";
            this.Load += new System.EventHandler(this.FormTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button runCheck;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcv;
        private System.Windows.Forms.DataGridViewTextBoxColumn chapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn verse;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
    }
}