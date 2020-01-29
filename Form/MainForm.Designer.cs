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
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.areaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.currentProjectAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentBookAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentChapterAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mainTextAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.introductionsAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outlinesAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesAndReferencesAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.missingSentencePunctuationCheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referencesCheckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordListFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biblicaTermsFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreListFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.punctuationMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.entireVerseFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIgnoredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referencesMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.hideLooseMatchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.punctuationTab = new System.Windows.Forms.TabPage();
            this.referencesTab = new System.Windows.Forms.TabPage();
            this.referencesOuterSplitContainer = new System.Windows.Forms.SplitContainer();
            this.referencesListView = new System.Windows.Forms.DataGridView();
            this.referencesListViewReferenceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesListViewCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesInnerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.referencesTextBox = new System.Windows.Forms.RichTextBox();
            this.referencesActionsGridView = new System.Windows.Forms.DataGridView();
            this.referencesActionsExceptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesActionsProblemColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesActionsSuggestionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesActionsAcceptColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.referencesActionsIgnoreColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.resultWorker = new System.ComponentModel.BackgroundWorker();
            this.hideIncorrectNameStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideTagShouldntExistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMissingTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideIncorrectTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMalformedTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideBadReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.punctuationTab.SuspendLayout();
            this.referencesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.referencesOuterSplitContainer)).BeginInit();
            this.referencesOuterSplitContainer.Panel1.SuspendLayout();
            this.referencesOuterSplitContainer.Panel2.SuspendLayout();
            this.referencesOuterSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.referencesListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.referencesInnerSplitContainer)).BeginInit();
            this.referencesInnerSplitContainer.Panel1.SuspendLayout();
            this.referencesInnerSplitContainer.Panel2.SuspendLayout();
            this.referencesInnerSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.referencesActionsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRunChecks
            // 
            this.btnRunChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunChecks.Location = new System.Drawing.Point(1061, 719);
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
            this.dgvCheckResults.Location = new System.Drawing.Point(0, 0);
            this.dgvCheckResults.Name = "dgvCheckResults";
            this.dgvCheckResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvCheckResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCheckResults.Size = new System.Drawing.Size(1204, 647);
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
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToIgnoreList,
            this.removeFromIgnoreList});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(214, 48);
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
            this.mainMenu.Size = new System.Drawing.Size(1211, 27);
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
            this.currentChapterAreaMenuItem,
            this.toolStripSeparator2,
            this.mainTextAreaMenuItem,
            this.introductionsAreaMenuItem,
            this.outlinesAreaMenuItem,
            this.notesAndReferencesAreaMenuItem});
            this.areaMenu.Name = "areaMenu";
            this.areaMenu.Size = new System.Drawing.Size(43, 23);
            this.areaMenu.Text = "&Area";
            // 
            // currentProjectAreaMenuItem
            // 
            this.currentProjectAreaMenuItem.Checked = true;
            this.currentProjectAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.currentProjectAreaMenuItem.Name = "currentProjectAreaMenuItem";
            this.currentProjectAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.currentProjectAreaMenuItem.Text = "Current &Project";
            this.currentProjectAreaMenuItem.Click += new System.EventHandler(this.OnCurrentProjectAreaMenuItemClick);
            // 
            // currentBookAreaMenuItem
            // 
            this.currentBookAreaMenuItem.Name = "currentBookAreaMenuItem";
            this.currentBookAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.currentBookAreaMenuItem.Text = "Current &Book";
            this.currentBookAreaMenuItem.Click += new System.EventHandler(this.OnCurrentBookAreaMenuItemClick);
            // 
            // currentChapterAreaMenuItem
            // 
            this.currentChapterAreaMenuItem.Name = "currentChapterAreaMenuItem";
            this.currentChapterAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.currentChapterAreaMenuItem.Text = "Current &Chapter";
            this.currentChapterAreaMenuItem.Click += new System.EventHandler(this.OnCurrentChapterAreaMenuItemClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // mainTextAreaMenuItem
            // 
            this.mainTextAreaMenuItem.Checked = true;
            this.mainTextAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mainTextAreaMenuItem.Name = "mainTextAreaMenuItem";
            this.mainTextAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.mainTextAreaMenuItem.Text = "&Main Text";
            this.mainTextAreaMenuItem.Click += new System.EventHandler(this.OnMainTextAreaMenuItemClick);
            // 
            // introductionsAreaMenuItem
            // 
            this.introductionsAreaMenuItem.Checked = true;
            this.introductionsAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.introductionsAreaMenuItem.Name = "introductionsAreaMenuItem";
            this.introductionsAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.introductionsAreaMenuItem.Text = "&Introductions";
            this.introductionsAreaMenuItem.Click += new System.EventHandler(this.OnIntroductionsAreaMenuItemClick);
            // 
            // outlinesAreaMenuItem
            // 
            this.outlinesAreaMenuItem.Checked = true;
            this.outlinesAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outlinesAreaMenuItem.Name = "outlinesAreaMenuItem";
            this.outlinesAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.outlinesAreaMenuItem.Text = "&Outlines";
            this.outlinesAreaMenuItem.Click += new System.EventHandler(this.OnOutlinesAreaMenuItemClick);
            // 
            // notesAndReferencesAreaMenuItem
            // 
            this.notesAndReferencesAreaMenuItem.Checked = true;
            this.notesAndReferencesAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.notesAndReferencesAreaMenuItem.Name = "notesAndReferencesAreaMenuItem";
            this.notesAndReferencesAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.notesAndReferencesAreaMenuItem.Text = "&Notes && References";
            this.notesAndReferencesAreaMenuItem.Click += new System.EventHandler(this.OnNotesAndReferencesAreaMenuItemClick);
            // 
            // checkMenu
            // 
            this.checkMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.missingSentencePunctuationCheckMenuItem,
            this.referencesCheckMenuItem});
            this.checkMenu.Name = "checkMenu";
            this.checkMenu.Size = new System.Drawing.Size(57, 23);
            this.checkMenu.Text = "&Checks";
            // 
            // missingSentencePunctuationCheckMenuItem
            // 
            this.missingSentencePunctuationCheckMenuItem.Checked = true;
            this.missingSentencePunctuationCheckMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.missingSentencePunctuationCheckMenuItem.Name = "missingSentencePunctuationCheckMenuItem";
            this.missingSentencePunctuationCheckMenuItem.Size = new System.Drawing.Size(234, 22);
            this.missingSentencePunctuationCheckMenuItem.Text = "&Missing Sentence Punctuation";
            this.missingSentencePunctuationCheckMenuItem.Click += new System.EventHandler(this.missingSentencePunctuationCheckMenuItem_Click);
            // 
            // referencesCheckMenuItem
            // 
            this.referencesCheckMenuItem.Name = "referencesCheckMenuItem";
            this.referencesCheckMenuItem.Size = new System.Drawing.Size(234, 22);
            this.referencesCheckMenuItem.Text = "Reference Checks";
            this.referencesCheckMenuItem.Click += new System.EventHandler(this.referencesToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordListFiltersMenuItem,
            this.biblicaTermsFiltersMenuItem,
            this.ignoreListFiltersMenuItem,
            this.punctuationMenuSeparator,
            this.entireVerseFiltersMenuItem,
            this.showIgnoredToolStripMenuItem,
            this.referencesMenuSeparator,
            this.hideLooseMatchesToolStripMenuItem,
            this.hideIncorrectNameStyleToolStripMenuItem,
            this.hideTagShouldntExistToolStripMenuItem,
            this.hideMissingTagToolStripMenuItem,
            this.hideIncorrectTagToolStripMenuItem,
            this.hideMalformedTagToolStripMenuItem,
            this.hideBadReferencesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.toolsToolStripMenuItem.Text = "&Filters";
            // 
            // wordListFiltersMenuItem
            // 
            this.wordListFiltersMenuItem.Name = "wordListFiltersMenuItem";
            this.wordListFiltersMenuItem.Size = new System.Drawing.Size(217, 22);
            this.wordListFiltersMenuItem.Text = "&Word List";
            this.wordListFiltersMenuItem.Click += new System.EventHandler(this.OnWordListFilterToolMenuClick);
            // 
            // biblicaTermsFiltersMenuItem
            // 
            this.biblicaTermsFiltersMenuItem.Name = "biblicaTermsFiltersMenuItem";
            this.biblicaTermsFiltersMenuItem.Size = new System.Drawing.Size(217, 22);
            this.biblicaTermsFiltersMenuItem.Text = "&Biblical Terms";
            this.biblicaTermsFiltersMenuItem.Click += new System.EventHandler(this.OnBiblicalTermsFilterToolMenuClick);
            // 
            // ignoreListFiltersMenuItem
            // 
            this.ignoreListFiltersMenuItem.Name = "ignoreListFiltersMenuItem";
            this.ignoreListFiltersMenuItem.Size = new System.Drawing.Size(217, 22);
            this.ignoreListFiltersMenuItem.Text = "&Ignore List";
            this.ignoreListFiltersMenuItem.Click += new System.EventHandler(this.IgnoreListToolStripMenuItem_Click);
            // 
            // punctuationMenuSeparator
            // 
            this.punctuationMenuSeparator.Name = "punctuationMenuSeparator";
            this.punctuationMenuSeparator.Size = new System.Drawing.Size(214, 6);
            // 
            // entireVerseFiltersMenuItem
            // 
            this.entireVerseFiltersMenuItem.Name = "entireVerseFiltersMenuItem";
            this.entireVerseFiltersMenuItem.Size = new System.Drawing.Size(217, 22);
            this.entireVerseFiltersMenuItem.Text = "Entire &Verse";
            this.entireVerseFiltersMenuItem.Click += new System.EventHandler(this.OnEntireVerseFiltersMenuClick);
            // 
            // showIgnoredToolStripMenuItem
            // 
            this.showIgnoredToolStripMenuItem.Name = "showIgnoredToolStripMenuItem";
            this.showIgnoredToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.showIgnoredToolStripMenuItem.Text = "Show Ignored";
            this.showIgnoredToolStripMenuItem.Click += new System.EventHandler(this.showIgnoredToolStripMenuItem_Click);
            // 
            // referencesMenuSeparator
            // 
            this.referencesMenuSeparator.Name = "referencesMenuSeparator";
            this.referencesMenuSeparator.Size = new System.Drawing.Size(214, 6);
            // 
            // hideLooseMatchesToolStripMenuItem
            // 
            this.hideLooseMatchesToolStripMenuItem.Name = "hideLooseMatchesToolStripMenuItem";
            this.hideLooseMatchesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideLooseMatchesToolStripMenuItem.Text = "Hide Loose Matches";
            this.hideLooseMatchesToolStripMenuItem.Click += new System.EventHandler(this.hideLooseMatchesToolStripMenuItem_Click);
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
            this.btnShowIgnoreList.Location = new System.Drawing.Point(17, 719);
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
            this.btnClose.Location = new System.Drawing.Point(1142, 719);
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
            this.statusLabel.Location = new System.Drawing.Point(98, 724);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 13);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "No violations.";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.punctuationTab);
            this.tabControl.Controls.Add(this.referencesTab);
            this.tabControl.Location = new System.Drawing.Point(13, 40);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1208, 673);
            this.tabControl.TabIndex = 7;
            // 
            // punctuationTab
            // 
            this.punctuationTab.Controls.Add(this.dgvCheckResults);
            this.punctuationTab.Location = new System.Drawing.Point(4, 22);
            this.punctuationTab.Name = "punctuationTab";
            this.punctuationTab.Padding = new System.Windows.Forms.Padding(3);
            this.punctuationTab.Size = new System.Drawing.Size(1200, 647);
            this.punctuationTab.TabIndex = 0;
            this.punctuationTab.Text = "Punctuation";
            this.punctuationTab.UseVisualStyleBackColor = true;
            // 
            // referencesTab
            // 
            this.referencesTab.Controls.Add(this.referencesOuterSplitContainer);
            this.referencesTab.Location = new System.Drawing.Point(4, 22);
            this.referencesTab.Name = "referencesTab";
            this.referencesTab.Padding = new System.Windows.Forms.Padding(3);
            this.referencesTab.Size = new System.Drawing.Size(1200, 647);
            this.referencesTab.TabIndex = 1;
            this.referencesTab.Text = "References";
            this.referencesTab.UseVisualStyleBackColor = true;
            // 
            // referencesOuterSplitContainer
            // 
            this.referencesOuterSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesOuterSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.referencesOuterSplitContainer.Name = "referencesOuterSplitContainer";
            // 
            // referencesOuterSplitContainer.Panel1
            // 
            this.referencesOuterSplitContainer.Panel1.Controls.Add(this.referencesListView);
            // 
            // referencesOuterSplitContainer.Panel2
            // 
            this.referencesOuterSplitContainer.Panel2.Controls.Add(this.referencesInnerSplitContainer);
            this.referencesOuterSplitContainer.Size = new System.Drawing.Size(1200, 647);
            this.referencesOuterSplitContainer.SplitterDistance = 251;
            this.referencesOuterSplitContainer.TabIndex = 4;
            // 
            // referencesListView
            // 
            this.referencesListView.AllowUserToAddRows = false;
            this.referencesListView.AllowUserToDeleteRows = false;
            this.referencesListView.AllowUserToResizeRows = false;
            this.referencesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesListView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.referencesListView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.referencesListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.referencesListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.referencesListViewReferenceColumn,
            this.referencesListViewCountColumn});
            this.referencesListView.Location = new System.Drawing.Point(0, 0);
            this.referencesListView.MultiSelect = false;
            this.referencesListView.Name = "referencesListView";
            this.referencesListView.ReadOnly = true;
            this.referencesListView.RowHeadersVisible = false;
            this.referencesListView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.referencesListView.Size = new System.Drawing.Size(249, 647);
            this.referencesListView.TabIndex = 0;
            this.referencesListView.SelectionChanged += new System.EventHandler(this.referencesListView_SelectionChanged);
            // 
            // referencesListViewReferenceColumn
            // 
            this.referencesListViewReferenceColumn.HeaderText = "Reference";
            this.referencesListViewReferenceColumn.Name = "referencesListViewReferenceColumn";
            this.referencesListViewReferenceColumn.ReadOnly = true;
            // 
            // referencesListViewCountColumn
            // 
            this.referencesListViewCountColumn.HeaderText = "# Exceptions";
            this.referencesListViewCountColumn.Name = "referencesListViewCountColumn";
            this.referencesListViewCountColumn.ReadOnly = true;
            // 
            // referencesInnerSplitContainer
            // 
            this.referencesInnerSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesInnerSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.referencesInnerSplitContainer.Name = "referencesInnerSplitContainer";
            // 
            // referencesInnerSplitContainer.Panel1
            // 
            this.referencesInnerSplitContainer.Panel1.Controls.Add(this.referencesTextBox);
            // 
            // referencesInnerSplitContainer.Panel2
            // 
            this.referencesInnerSplitContainer.Panel2.Controls.Add(this.referencesActionsGridView);
            this.referencesInnerSplitContainer.Size = new System.Drawing.Size(946, 647);
            this.referencesInnerSplitContainer.SplitterDistance = 487;
            this.referencesInnerSplitContainer.TabIndex = 0;
            // 
            // referencesTextBox
            // 
            this.referencesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.referencesTextBox.Location = new System.Drawing.Point(0, 0);
            this.referencesTextBox.Name = "referencesTextBox";
            this.referencesTextBox.ReadOnly = true;
            this.referencesTextBox.Size = new System.Drawing.Size(485, 647);
            this.referencesTextBox.TabIndex = 3;
            this.referencesTextBox.Text = "";
            // 
            // referencesActionsGridView
            // 
            this.referencesActionsGridView.AllowUserToAddRows = false;
            this.referencesActionsGridView.AllowUserToDeleteRows = false;
            this.referencesActionsGridView.AllowUserToResizeRows = false;
            this.referencesActionsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesActionsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.referencesActionsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.referencesActionsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.referencesActionsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.referencesActionsExceptionColumn,
            this.referencesActionsProblemColumn,
            this.referencesActionsSuggestionColumn,
            this.referencesActionsAcceptColumn,
            this.referencesActionsIgnoreColumn});
            this.referencesActionsGridView.Location = new System.Drawing.Point(0, 0);
            this.referencesActionsGridView.MultiSelect = false;
            this.referencesActionsGridView.Name = "referencesActionsGridView";
            this.referencesActionsGridView.ReadOnly = true;
            this.referencesActionsGridView.RowHeadersVisible = false;
            this.referencesActionsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.referencesActionsGridView.Size = new System.Drawing.Size(455, 647);
            this.referencesActionsGridView.TabIndex = 0;
            this.referencesActionsGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.referencesActionsGridView_CellClick);
            this.referencesActionsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.referencesActionsGridView_CellContentClick);
            this.referencesActionsGridView.SelectionChanged += new System.EventHandler(this.referencesActionsGridView_SelectionChanged);
            // 
            // referencesActionsExceptionColumn
            // 
            this.referencesActionsExceptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.referencesActionsExceptionColumn.FillWeight = 50F;
            this.referencesActionsExceptionColumn.HeaderText = "Exception";
            this.referencesActionsExceptionColumn.Name = "referencesActionsExceptionColumn";
            this.referencesActionsExceptionColumn.ReadOnly = true;
            this.referencesActionsExceptionColumn.Width = 79;
            // 
            // referencesActionsProblemColumn
            // 
            this.referencesActionsProblemColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.referencesActionsProblemColumn.HeaderText = "Issue";
            this.referencesActionsProblemColumn.Name = "referencesActionsProblemColumn";
            this.referencesActionsProblemColumn.ReadOnly = true;
            this.referencesActionsProblemColumn.Width = 57;
            // 
            // referencesActionsSuggestionColumn
            // 
            this.referencesActionsSuggestionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.referencesActionsSuggestionColumn.HeaderText = "Suggested Fix";
            this.referencesActionsSuggestionColumn.Name = "referencesActionsSuggestionColumn";
            this.referencesActionsSuggestionColumn.ReadOnly = true;
            this.referencesActionsSuggestionColumn.Width = 99;
            // 
            // referencesActionsAcceptColumn
            // 
            this.referencesActionsAcceptColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.referencesActionsAcceptColumn.HeaderText = "Actions";
            this.referencesActionsAcceptColumn.Name = "referencesActionsAcceptColumn";
            this.referencesActionsAcceptColumn.ReadOnly = true;
            this.referencesActionsAcceptColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.referencesActionsAcceptColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.referencesActionsAcceptColumn.ToolTipText = "Future capability";
            this.referencesActionsAcceptColumn.Width = 48;
            // 
            // referencesActionsIgnoreColumn
            // 
            this.referencesActionsIgnoreColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.referencesActionsIgnoreColumn.HeaderText = "";
            this.referencesActionsIgnoreColumn.Name = "referencesActionsIgnoreColumn";
            this.referencesActionsIgnoreColumn.ReadOnly = true;
            this.referencesActionsIgnoreColumn.Text = "Ignore";
            this.referencesActionsIgnoreColumn.ToolTipText = "Ignore this exception in the future.";
            this.referencesActionsIgnoreColumn.Width = 5;
            // 
            // hideIncorrectNameStyleToolStripMenuItem
            // 
            this.hideIncorrectNameStyleToolStripMenuItem.Name = "hideIncorrectNameStyleToolStripMenuItem";
            this.hideIncorrectNameStyleToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideIncorrectNameStyleToolStripMenuItem.Text = "Hide Incorrect Name Style";
            this.hideIncorrectNameStyleToolStripMenuItem.Click += new System.EventHandler(this.hideIncorrectNameStyleToolStripMenuItem_Click);
            // 
            // hideTagShouldntExistToolStripMenuItem
            // 
            this.hideTagShouldntExistToolStripMenuItem.Name = "hideTagShouldntExistToolStripMenuItem";
            this.hideTagShouldntExistToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideTagShouldntExistToolStripMenuItem.Text = "Hide Tag Shouldn\'t Exist";
            this.hideTagShouldntExistToolStripMenuItem.Click += new System.EventHandler(this.hideTagShouldntExistToolStripMenuItem_Click);
            // 
            // hideMissingTagToolStripMenuItem
            // 
            this.hideMissingTagToolStripMenuItem.Name = "hideMissingTagToolStripMenuItem";
            this.hideMissingTagToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideMissingTagToolStripMenuItem.Text = "Hide Missing Tag";
            this.hideMissingTagToolStripMenuItem.Click += new System.EventHandler(this.hideMissingTagToolStripMenuItem_Click);
            // 
            // hideIncorrectTagToolStripMenuItem
            // 
            this.hideIncorrectTagToolStripMenuItem.Name = "hideIncorrectTagToolStripMenuItem";
            this.hideIncorrectTagToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideIncorrectTagToolStripMenuItem.Text = "Hide Incorrect Tag";
            this.hideIncorrectTagToolStripMenuItem.Click += new System.EventHandler(this.hideIncorrectTagToolStripMenuItem_Click);
            // 
            // hideMalformedTagToolStripMenuItem
            // 
            this.hideMalformedTagToolStripMenuItem.Name = "hideMalformedTagToolStripMenuItem";
            this.hideMalformedTagToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideMalformedTagToolStripMenuItem.Text = "Hide Malformed Tag";
            this.hideMalformedTagToolStripMenuItem.Click += new System.EventHandler(this.hideMalformedTagToolStripMenuItem_Click);
            // 
            // hideBadReferencesToolStripMenuItem
            // 
            this.hideBadReferencesToolStripMenuItem.Name = "hideBadReferencesToolStripMenuItem";
            this.hideBadReferencesToolStripMenuItem.Size = new System.Drawing.Size(217, 22);
            this.hideBadReferencesToolStripMenuItem.Text = "Hide Bad References";
            this.hideBadReferencesToolStripMenuItem.Click += new System.EventHandler(this.hideBadReferencesToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1231, 755);
            this.ControlBox = false;
            this.Controls.Add(this.btnShowIgnoreList);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnClose);
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
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.punctuationTab.ResumeLayout(false);
            this.referencesTab.ResumeLayout(false);
            this.referencesOuterSplitContainer.Panel1.ResumeLayout(false);
            this.referencesOuterSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.referencesOuterSplitContainer)).EndInit();
            this.referencesOuterSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.referencesListView)).EndInit();
            this.referencesInnerSplitContainer.Panel1.ResumeLayout(false);
            this.referencesInnerSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.referencesInnerSplitContainer)).EndInit();
            this.referencesInnerSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.referencesActionsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem missingSentencePunctuationCheckMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem bcvViewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem errorViewMenuItem;
        private System.Windows.Forms.Button btnShowIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem wordListFiltersMenuItem;
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
        private System.Windows.Forms.ToolStripSeparator punctuationMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem entireVerseFiltersMenuItem;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Match;
        private System.Windows.Forms.DataGridViewTextBoxColumn Verse;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem removeFromIgnoreList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mainTextAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notesAndReferencesAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem biblicaTermsFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem introductionsAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outlinesAreaMenuItem;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage punctuationTab;
        private System.Windows.Forms.TabPage referencesTab;
        private System.Windows.Forms.ToolStripMenuItem referencesCheckMenuItem;
        private System.Windows.Forms.RichTextBox referencesTextBox;
        private System.Windows.Forms.SplitContainer referencesOuterSplitContainer;
        private System.Windows.Forms.SplitContainer referencesInnerSplitContainer;
        private System.Windows.Forms.DataGridView referencesActionsGridView;
        private System.Windows.Forms.DataGridView referencesListView;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesListViewReferenceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesListViewCountColumn;
        private System.Windows.Forms.ToolStripMenuItem showIgnoredToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator referencesMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem hideLooseMatchesToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesActionsExceptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesActionsProblemColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesActionsSuggestionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn referencesActionsAcceptColumn;
        private System.Windows.Forms.DataGridViewButtonColumn referencesActionsIgnoreColumn;
        private System.ComponentModel.BackgroundWorker resultWorker;
        private System.Windows.Forms.ToolStripMenuItem hideIncorrectNameStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideTagShouldntExistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMissingTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideIncorrectTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMalformedTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideBadReferencesToolStripMenuItem;
    }
}