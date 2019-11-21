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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnRunChecks = new System.Windows.Forms.Button();
            this.dgvCheckResults = new System.Windows.Forms.DataGridView();
            this.bcv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Match = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Verse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.areaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.currentProjectAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentBookAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentChapterAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.punctuationCheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biblicalTermsFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreListFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.entireVerseFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.bcvViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.matchViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verseViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.errorViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchMenuTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchLabelMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowIgnoreList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.termWorker = new System.ComponentModel.BackgroundWorker();
            this.statusLabel = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRunChecks
            // 
            this.btnRunChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunChecks.Location = new System.Drawing.Point(542, 453);
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
            this.dgvCheckResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCheckResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCheckResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dgvCheckResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bcv,
            this.Match,
            this.Verse,
            this.error});
            this.dgvCheckResults.ContextMenuStrip = this.contextMenu;
            this.dgvCheckResults.Location = new System.Drawing.Point(10, 40);
            this.dgvCheckResults.Name = "dgvCheckResults";
            this.dgvCheckResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvCheckResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCheckResults.Size = new System.Drawing.Size(688, 407);
            this.dgvCheckResults.TabIndex = 2;
            // 
            // bcv
            // 
            this.bcv.FillWeight = 10F;
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
            this.Verse.FillWeight = 35F;
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
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.areaMenu,
            this.checkMenu,
            this.toolsToolStripMenuItem,
            this.viewMenu,
            this.searchMenuTextBox,
            this.searchLabelMenu});
            this.mainMenu.Location = new System.Drawing.Point(10, 10);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(688, 27);
            this.mainMenu.TabIndex = 3;
            this.mainMenu.Text = "mainMenu";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 23);
            this.fileMenu.Text = "&File";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveMenuItem.Image")));
            this.saveMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveMenuItem.Text = "&Save...";
            this.saveMenuItem.Click += new System.EventHandler(this.OnFileSaveMenuClick);
            // 
            // areaMenu
            // 
            this.areaMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentProjectAreaMenuItem,
            this.currentBookAreaMenuItem,
            this.currentChapterAreaMenuItem});
            this.areaMenu.Name = "areaMenu";
            this.areaMenu.Size = new System.Drawing.Size(43, 23);
            this.areaMenu.Text = "&Area";
            // 
            // currentProjectAreaMenuItem
            // 
            this.currentProjectAreaMenuItem.Checked = true;
            this.currentProjectAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.currentProjectAreaMenuItem.Name = "currentProjectAreaMenuItem";
            this.currentProjectAreaMenuItem.Size = new System.Drawing.Size(159, 22);
            this.currentProjectAreaMenuItem.Text = "Current &Project";
            this.currentProjectAreaMenuItem.Click += new System.EventHandler(this.OnCurrentProjectAreaMenuClick);
            // 
            // currentBookAreaMenuItem
            // 
            this.currentBookAreaMenuItem.Name = "currentBookAreaMenuItem";
            this.currentBookAreaMenuItem.Size = new System.Drawing.Size(159, 22);
            this.currentBookAreaMenuItem.Text = "Current &Book";
            this.currentBookAreaMenuItem.Click += new System.EventHandler(this.OnCurrentBookAreaMenuClick);
            // 
            // currentChapterAreaMenuItem
            // 
            this.currentChapterAreaMenuItem.Name = "currentChapterAreaMenuItem";
            this.currentChapterAreaMenuItem.Size = new System.Drawing.Size(159, 22);
            this.currentChapterAreaMenuItem.Text = "Current &Chapter";
            this.currentChapterAreaMenuItem.Click += new System.EventHandler(this.OnCurrentChapterAreaMenuClick);
            // 
            // checkMenu
            // 
            this.checkMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.punctuationCheckMenuItem});
            this.checkMenu.Name = "checkMenu";
            this.checkMenu.Size = new System.Drawing.Size(57, 23);
            this.checkMenu.Text = "&Checks";
            // 
            // punctuationCheckMenuItem
            // 
            this.punctuationCheckMenuItem.Checked = true;
            this.punctuationCheckMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.punctuationCheckMenuItem.Enabled = false;
            this.punctuationCheckMenuItem.Name = "punctuationCheckMenuItem";
            this.punctuationCheckMenuItem.Size = new System.Drawing.Size(139, 22);
            this.punctuationCheckMenuItem.Text = "Punctuation";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.biblicalTermsFiltersMenuItem,
            this.ignoreListFiltersMenuItem,
            this.toolStripSeparator1,
            this.entireVerseFiltersMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.toolsToolStripMenuItem.Text = "&Filters";
            // 
            // biblicalTermsFiltersMenuItem
            // 
            this.biblicalTermsFiltersMenuItem.Name = "biblicalTermsFiltersMenuItem";
            this.biblicalTermsFiltersMenuItem.Size = new System.Drawing.Size(146, 22);
            this.biblicalTermsFiltersMenuItem.Text = "&Biblical Terms";
            this.biblicalTermsFiltersMenuItem.Click += new System.EventHandler(this.OnBiblicalTermListToolMenuClick);
            // 
            // ignoreListFiltersMenuItem
            // 
            this.ignoreListFiltersMenuItem.Name = "ignoreListFiltersMenuItem";
            this.ignoreListFiltersMenuItem.Size = new System.Drawing.Size(146, 22);
            this.ignoreListFiltersMenuItem.Text = "&Ignore List";
            this.ignoreListFiltersMenuItem.Click += new System.EventHandler(this.IgnoreListToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // entireVerseFiltersMenuItem
            // 
            this.entireVerseFiltersMenuItem.Name = "entireVerseFiltersMenuItem";
            this.entireVerseFiltersMenuItem.Size = new System.Drawing.Size(146, 22);
            this.entireVerseFiltersMenuItem.Text = "Entire &Verse";
            this.entireVerseFiltersMenuItem.Click += new System.EventHandler(this.OnEntireVerseFiltersMenuClick);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bcvViewMenuItem,
            this.matchViewMenuItem,
            this.verseViewMenuItem,
            this.errorViewMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 23);
            this.viewMenu.Text = "&View";
            // 
            // bcvViewMenuItem
            // 
            this.bcvViewMenuItem.Checked = true;
            this.bcvViewMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bcvViewMenuItem.Name = "bcvViewMenuItem";
            this.bcvViewMenuItem.Size = new System.Drawing.Size(108, 22);
            this.bcvViewMenuItem.Text = "BCV";
            this.bcvViewMenuItem.Click += new System.EventHandler(this.OnBcvViewMenuClick);
            // 
            // matchViewMenuItem
            // 
            this.matchViewMenuItem.Checked = true;
            this.matchViewMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.matchViewMenuItem.Name = "matchViewMenuItem";
            this.matchViewMenuItem.Size = new System.Drawing.Size(108, 22);
            this.matchViewMenuItem.Text = "Match";
            this.matchViewMenuItem.Click += new System.EventHandler(this.OnMatchViewMenuItem_Click);
            // 
            // verseViewMenuItem
            // 
            this.verseViewMenuItem.Checked = true;
            this.verseViewMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.verseViewMenuItem.Name = "verseViewMenuItem";
            this.verseViewMenuItem.Size = new System.Drawing.Size(108, 22);
            this.verseViewMenuItem.Text = "Verse";
            this.verseViewMenuItem.Click += new System.EventHandler(this.OnVerseViewMenuClicked);
            // 
            // errorViewMenuItem
            // 
            this.errorViewMenuItem.Checked = true;
            this.errorViewMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.errorViewMenuItem.Name = "errorViewMenuItem";
            this.errorViewMenuItem.Size = new System.Drawing.Size(108, 22);
            this.errorViewMenuItem.Text = "Error";
            this.errorViewMenuItem.Click += new System.EventHandler(this.OnErrorViewMenuClicked);
            // 
            // searchMenuTextBox
            // 
            this.searchMenuTextBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchMenuTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.searchMenuTextBox.Name = "searchMenuTextBox";
            this.searchMenuTextBox.Size = new System.Drawing.Size(200, 23);
            // 
            // searchLabelMenu
            // 
            this.searchLabelMenu.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchLabelMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.searchLabelMenu.Name = "searchLabelMenu";
            this.searchLabelMenu.Size = new System.Drawing.Size(57, 23);
            this.searchLabelMenu.Text = "Search:";
            // 
            // btnShowIgnoreList
            // 
            this.btnShowIgnoreList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnShowIgnoreList.Location = new System.Drawing.Point(10, 453);
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
            this.btnClose.Location = new System.Drawing.Point(623, 453);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.OnCloseButtonClick);
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(91, 458);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 13);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "No violations.";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToIgnoreList,
            this.removeFromIgnoreList});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(214, 70);
            // 
            // addToIgnoreList
            // 
            this.addToIgnoreList.Name = "addToIgnoreList";
            this.addToIgnoreList.Size = new System.Drawing.Size(213, 22);
            this.addToIgnoreList.Text = "&Add to Ignore List...";
            this.addToIgnoreList.Click += new System.EventHandler(this.OnAddToIgnoreListMenuClick);
            // 
            // removeFromIgnoreList
            // 
            this.removeFromIgnoreList.Name = "removeFromIgnoreList";
            this.removeFromIgnoreList.Size = new System.Drawing.Size(213, 22);
            this.removeFromIgnoreList.Text = "&Remove from Ignore List...";
            this.removeFromIgnoreList.Click += new System.EventHandler(this.OnRemoveFromIgnoreListMenuClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(708, 489);
            this.ControlBox = false;
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.dgvCheckResults);
            this.Controls.Add(this.btnShowIgnoreList);
            this.Controls.Add(this.btnRunChecks);
            this.Controls.Add(this.mainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Translation Validations...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTest_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRunChecks;
        private System.Windows.Forms.DataGridView dgvCheckResults;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkMenu;
        private System.Windows.Forms.ToolStripMenuItem punctuationCheckMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem bcvViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorViewMenuItem;
        private System.Windows.Forms.Button btnShowIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem biblicalTermsFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreListFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verseViewMenuItem;
        private System.Windows.Forms.Button btnClose;
        private System.ComponentModel.BackgroundWorker termWorker;
        private System.Windows.Forms.ToolStripMenuItem matchViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchLabelMenu;
        private System.Windows.Forms.ToolStripTextBox searchMenuTextBox;
        private System.Windows.Forms.ToolStripMenuItem areaMenu;
        private System.Windows.Forms.ToolStripMenuItem currentProjectAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentBookAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentChapterAreaMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem entireVerseFiltersMenuItem;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Match;
        private System.Windows.Forms.DataGridViewTextBoxColumn Verse;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem removeFromIgnoreList;
    }
}