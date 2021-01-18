
namespace TvpMain.Forms
{
    partial class CheckEditor
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
            this.idLabel = new System.Windows.Forms.Label();
            this.checkFixIdLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.checkFixNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.jsEditor = new ScintillaNET.Scintilla();
            this.label5 = new System.Windows.Forms.Label();
            this.fixRegExTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkFindRegExTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.scopeCombo = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.versionTextBox = new System.Windows.Forms.MaskedTextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.descriptionTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tagsTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.languagesTextBox = new System.Windows.Forms.TextBox();
            this.defaultDescTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.helpTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishWorker = new System.ComponentModel.BackgroundWorker();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(6, 20);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(21, 13);
            this.idLabel.TabIndex = 0;
            this.idLabel.Text = "ID:";
            // 
            // checkFixIdLabel
            // 
            this.checkFixIdLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkFixIdLabel.Location = new System.Drawing.Point(33, 16);
            this.checkFixIdLabel.Name = "checkFixIdLabel";
            this.checkFixIdLabel.Size = new System.Drawing.Size(406, 20);
            this.checkFixIdLabel.TabIndex = 1;
            this.checkFixIdLabel.Text = "label1";
            this.checkFixIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.checkFixIdLabel.MouseEnter += new System.EventHandler(this.checkFixIdLabel_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Check/Fix Name:";
            // 
            // checkFixNameTextBox
            // 
            this.checkFixNameTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.checkFixNameTextBox.Location = new System.Drawing.Point(145, 40);
            this.checkFixNameTextBox.Name = "checkFixNameTextBox";
            this.checkFixNameTextBox.Size = new System.Drawing.Size(294, 20);
            this.checkFixNameTextBox.TabIndex = 3;
            this.checkFixNameTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.checkFixNameTextBox.MouseEnter += new System.EventHandler(this.checkFixNameTextBox_MouseEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.jsEditor);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.fixRegExTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.checkFindRegExTextBox);
            this.groupBox2.Location = new System.Drawing.Point(13, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1164, 685);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // jsEditor
            // 
            this.jsEditor.Lexer = ScintillaNET.Lexer.Cpp;
            this.jsEditor.Location = new System.Drawing.Point(9, 92);
            this.jsEditor.Name = "jsEditor";
            this.jsEditor.Size = new System.Drawing.Size(1147, 575);
            this.jsEditor.TabIndex = 6;
            this.jsEditor.CharAdded += new System.EventHandler<ScintillaNET.CharAddedEventArgs>(this.jsEditor_CharAdded);
            this.jsEditor.TextChanged += new System.EventHandler(this.jsEditor_TextChanged);
            this.jsEditor.MouseEnter += new System.EventHandler(this.jsEditor_MouseEnter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "JavaScript";
            // 
            // fixRegExTextBox
            // 
            this.fixRegExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fixRegExTextBox.Location = new System.Drawing.Point(116, 43);
            this.fixRegExTextBox.Name = "fixRegExTextBox";
            this.fixRegExTextBox.Size = new System.Drawing.Size(1040, 20);
            this.fixRegExTextBox.TabIndex = 3;
            this.fixRegExTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.fixRegExTextBox.MouseEnter += new System.EventHandler(this.FixRegExTextBox_MouseEnter);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Fix Replace RegEx:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Check Find RegEx:";
            // 
            // checkFindRegExTextBox
            // 
            this.checkFindRegExTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkFindRegExTextBox.Location = new System.Drawing.Point(116, 16);
            this.checkFindRegExTextBox.Name = "checkFindRegExTextBox";
            this.checkFindRegExTextBox.Size = new System.Drawing.Size(1040, 20);
            this.checkFindRegExTextBox.TabIndex = 0;
            this.checkFindRegExTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.checkFindRegExTextBox.MouseEnter += new System.EventHandler(this.checkFindRegExTextBox_MouseEnter);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Version:";
            // 
            // scopeCombo
            // 
            this.scopeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.scopeCombo.FormattingEnabled = true;
            this.scopeCombo.Items.AddRange(new object[] {
            "PROJECT",
            "BOOK",
            "CHAPTER",
            "VERSE"});
            this.scopeCombo.Location = new System.Drawing.Point(145, 92);
            this.scopeCombo.Name = "scopeCombo";
            this.scopeCombo.Size = new System.Drawing.Size(294, 21);
            this.scopeCombo.TabIndex = 6;
            this.scopeCombo.MouseEnter += new System.EventHandler(this.scopeCombo_MouseEnter);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.versionTextBox);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.descriptionTextBox);
            this.groupBox3.Controls.Add(this.checkFixNameTextBox);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tagsTextBox);
            this.groupBox3.Controls.Add(this.idLabel);
            this.groupBox3.Controls.Add(this.checkFixIdLabel);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.languagesTextBox);
            this.groupBox3.Controls.Add(this.defaultDescTextBox);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.scopeCombo);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(1183, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(445, 685);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            // 
            // versionTextBox
            // 
            this.versionTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.versionTextBox.Location = new System.Drawing.Point(145, 66);
            this.versionTextBox.Mask = "0.0.0.0";
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Size = new System.Drawing.Size(294, 20);
            this.versionTextBox.TabIndex = 16;
            this.versionTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.versionTextBox.MouseEnter += new System.EventHandler(this.versionTextBox_MouseEnter);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 197);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Description";
            // 
            // descriptionTextBox
            // 
            this.descriptionTextBox.AcceptsReturn = true;
            this.descriptionTextBox.AcceptsTab = true;
            this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionTextBox.Location = new System.Drawing.Point(6, 224);
            this.descriptionTextBox.Multiline = true;
            this.descriptionTextBox.Name = "descriptionTextBox";
            this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.descriptionTextBox.Size = new System.Drawing.Size(430, 455);
            this.descriptionTextBox.TabIndex = 14;
            this.descriptionTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.descriptionTextBox.MouseEnter += new System.EventHandler(this.descriptionTextBox_MouseEnter);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 174);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Tags:";
            // 
            // tagsTextBox
            // 
            this.tagsTextBox.Location = new System.Drawing.Point(145, 171);
            this.tagsTextBox.Name = "tagsTextBox";
            this.tagsTextBox.Size = new System.Drawing.Size(294, 20);
            this.tagsTextBox.TabIndex = 12;
            this.tagsTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.tagsTextBox.MouseEnter += new System.EventHandler(this.tagsTextBox_MouseEnter);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 148);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Languages:";
            // 
            // languagesTextBox
            // 
            this.languagesTextBox.Location = new System.Drawing.Point(145, 145);
            this.languagesTextBox.Name = "languagesTextBox";
            this.languagesTextBox.Size = new System.Drawing.Size(294, 20);
            this.languagesTextBox.TabIndex = 10;
            this.languagesTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.languagesTextBox.MouseEnter += new System.EventHandler(this.languagesTextBox_MouseEnter);
            // 
            // defaultDescTextBox
            // 
            this.defaultDescTextBox.BackColor = System.Drawing.Color.LightYellow;
            this.defaultDescTextBox.Location = new System.Drawing.Point(145, 119);
            this.defaultDescTextBox.Name = "defaultDescTextBox";
            this.defaultDescTextBox.Size = new System.Drawing.Size(294, 20);
            this.defaultDescTextBox.TabIndex = 9;
            this.defaultDescTextBox.TextChanged += new System.EventHandler(this.content_TextChanged);
            this.defaultDescTextBox.MouseEnter += new System.EventHandler(this.defaultDescTextBox_MouseEnter);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(133, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Default Result Description:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Scope:";
            // 
            // helpTextBox
            // 
            this.helpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextBox.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.helpTextBox.Location = new System.Drawing.Point(12, 718);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.Size = new System.Drawing.Size(1616, 74);
            this.helpTextBox.TabIndex = 8;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.publishToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1640, 24);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // publishToolStripMenuItem
            // 
            this.publishToolStripMenuItem.Name = "publishToolStripMenuItem";
            this.publishToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
            this.publishToolStripMenuItem.Text = "Save and &Publish";
            this.publishToolStripMenuItem.Click += new System.EventHandler(this.publishToolStripMenuItem_Click);
            // 
            // publishWorker
            // 
            this.publishWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.publishWorker_DoWork);
            this.publishWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.publishWorker_RunWorkerCompleted);
            // 
            // CheckEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1640, 804);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckEditor";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Check/Fix Editor & Publisher";
            this.Load += new System.EventHandler(this.CheckEditor_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label checkFixIdLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox checkFixNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox checkFindRegExTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fixRegExTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox scopeCombo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox defaultDescTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tagsTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox languagesTextBox;
        private System.Windows.Forms.TextBox helpTextBox;
        private System.Windows.Forms.TextBox descriptionTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.MaskedTextBox versionTextBox;
        private System.Windows.Forms.ToolStripMenuItem publishToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker publishWorker;
        private ScintillaNET.Scintilla jsEditor;
    }
}