namespace TvpMain.Form
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnRunChecks = new System.Windows.Forms.Button();
            this.dgvCheckResults = new System.Windows.Forms.DataGridView();
            this.bcv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Match = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Verse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mstMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.punctuationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bCVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biblicalWordListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowIgnoreList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.termWorker = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).BeginInit();
            this.mstMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRunChecks
            // 
            this.btnRunChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunChecks.Location = new System.Drawing.Point(659, 478);
            this.btnRunChecks.Name = "btnRunChecks";
            this.btnRunChecks.Size = new System.Drawing.Size(75, 23);
            this.btnRunChecks.TabIndex = 1;
            this.btnRunChecks.Text = "&Run";
            this.btnRunChecks.UseVisualStyleBackColor = true;
            this.btnRunChecks.Click += new System.EventHandler(this.OnRunChecks);
            // 
            // dgvCheckResults
            // 
            this.dgvCheckResults.AllowUserToAddRows = false;
            this.dgvCheckResults.AllowUserToDeleteRows = false;
            this.dgvCheckResults.AllowUserToOrderColumns = true;
            this.dgvCheckResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCheckResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCheckResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvCheckResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bcv,
            this.Match,
            this.Verse,
            this.error,
            this.notes});
            this.dgvCheckResults.Location = new System.Drawing.Point(10, 37);
            this.dgvCheckResults.MultiSelect = false;
            this.dgvCheckResults.Name = "dgvCheckResults";
            this.dgvCheckResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCheckResults.Size = new System.Drawing.Size(805, 435);
            this.dgvCheckResults.TabIndex = 2;
            // 
            // bcv
            // 
            this.bcv.FillWeight = 15F;
            this.bcv.HeaderText = "BCV";
            this.bcv.MinimumWidth = 60;
            this.bcv.Name = "bcv";
            this.bcv.ReadOnly = true;
            // 
            // Match
            // 
            this.Match.FillWeight = 15F;
            this.Match.HeaderText = "Match";
            this.Match.MinimumWidth = 60;
            this.Match.Name = "Match";
            this.Match.ReadOnly = true;
            // 
            // Verse
            // 
            this.Verse.FillWeight = 30F;
            this.Verse.HeaderText = "Verse";
            this.Verse.MinimumWidth = 120;
            this.Verse.Name = "Verse";
            this.Verse.ReadOnly = true;
            // 
            // error
            // 
            this.error.FillWeight = 30F;
            this.error.HeaderText = "Error";
            this.error.MinimumWidth = 120;
            this.error.Name = "error";
            this.error.ReadOnly = true;
            // 
            // notes
            // 
            this.notes.FillWeight = 10F;
            this.notes.HeaderText = "Notes";
            this.notes.MinimumWidth = 40;
            this.notes.Name = "notes";
            // 
            // mstMain
            // 
            this.mstMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewsToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.mstMain.Location = new System.Drawing.Point(10, 10);
            this.mstMain.Name = "mstMain";
            this.mstMain.Size = new System.Drawing.Size(805, 24);
            this.mstMain.TabIndex = 3;
            this.mstMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(135, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(135, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.punctuationToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(112, 20);
            this.editToolStripMenuItem.Text = "Validation Checks";
            // 
            // punctuationToolStripMenuItem
            // 
            this.punctuationToolStripMenuItem.Name = "punctuationToolStripMenuItem";
            this.punctuationToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.punctuationToolStripMenuItem.Text = "Punctuation";
            this.punctuationToolStripMenuItem.Click += new System.EventHandler(this.PunctuationToolStripMenuItem_Click);
            // 
            // viewsToolStripMenuItem
            // 
            this.viewsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bCVToolStripMenuItem,
            this.errorToolStripMenuItem,
            this.notesToolStripMenuItem,
            this.actionToolStripMenuItem});
            this.viewsToolStripMenuItem.Name = "viewsToolStripMenuItem";
            this.viewsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.viewsToolStripMenuItem.Text = "Views";
            // 
            // bCVToolStripMenuItem
            // 
            this.bCVToolStripMenuItem.Name = "bCVToolStripMenuItem";
            this.bCVToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.bCVToolStripMenuItem.Text = "BCV";
            this.bCVToolStripMenuItem.Click += new System.EventHandler(this.BcvToolStripMenuItem_Click);
            // 
            // errorToolStripMenuItem
            // 
            this.errorToolStripMenuItem.Name = "errorToolStripMenuItem";
            this.errorToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.errorToolStripMenuItem.Text = "Error";
            this.errorToolStripMenuItem.Click += new System.EventHandler(this.ErrorToolStripMenuItem_Click);
            // 
            // notesToolStripMenuItem
            // 
            this.notesToolStripMenuItem.Name = "notesToolStripMenuItem";
            this.notesToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.notesToolStripMenuItem.Text = "Notes";
            this.notesToolStripMenuItem.Click += new System.EventHandler(this.NotesToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.actionToolStripMenuItem.Text = "Action";
            this.actionToolStripMenuItem.Click += new System.EventHandler(this.ActionToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.biblicalWordListToolStripMenuItem,
            this.ignoreListToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.toolsToolStripMenuItem.Text = "Filters";
            // 
            // biblicalWordListToolStripMenuItem
            // 
            this.biblicalWordListToolStripMenuItem.Name = "biblicalWordListToolStripMenuItem";
            this.biblicalWordListToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.biblicalWordListToolStripMenuItem.Text = "Biblical Term List";
            this.biblicalWordListToolStripMenuItem.Click += new System.EventHandler(this.BiblicalWordListToolStripMenuItem_Click);
            // 
            // ignoreListToolStripMenuItem
            // 
            this.ignoreListToolStripMenuItem.Name = "ignoreListToolStripMenuItem";
            this.ignoreListToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ignoreListToolStripMenuItem.Text = "Ignore List";
            this.ignoreListToolStripMenuItem.Click += new System.EventHandler(this.IgnoreListToolStripMenuItem_Click);
            // 
            // btnShowIgnoreList
            // 
            this.btnShowIgnoreList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowIgnoreList.Location = new System.Drawing.Point(10, 478);
            this.btnShowIgnoreList.Name = "btnShowIgnoreList";
            this.btnShowIgnoreList.Size = new System.Drawing.Size(75, 23);
            this.btnShowIgnoreList.TabIndex = 4;
            this.btnShowIgnoreList.Text = "&Ignore List...";
            this.btnShowIgnoreList.UseVisualStyleBackColor = true;
            this.btnShowIgnoreList.Click += new System.EventHandler(this.OnClickIgnoreList);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(740, 478);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(825, 514);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvCheckResults);
            this.Controls.Add(this.btnShowIgnoreList);
            this.Controls.Add(this.btnRunChecks);
            this.Controls.Add(this.mstMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mstMain;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Translation Validations...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTest_FormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).EndInit();
            this.mstMain.ResumeLayout(false);
            this.mstMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRunChecks;
        private System.Windows.Forms.DataGridView dgvCheckResults;
        private System.Windows.Forms.MenuStrip mstMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem punctuationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bCVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.Button btnShowIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem biblicalWordListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notesToolStripMenuItem;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Match;
        private System.Windows.Forms.DataGridViewTextBoxColumn Verse;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.DataGridViewTextBoxColumn notes;
        private System.ComponentModel.BackgroundWorker termWorker;
    }
}