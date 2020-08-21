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
            _resultManager?.Dispose();
            _textCheckRunner?.Dispose();

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnRunChecks = new System.Windows.Forms.Button();
            this.dgvCheckResults = new System.Windows.Forms.DataGridView();
            this.bcv = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Verse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.categoryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Match = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionsAcceptColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.actionsIgnoreColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromIgnoreList = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.areaMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.currentProjectAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentBookAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentChapterAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mainTextAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.introductionsAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.outlinesAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notesAndReferencesAreaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordListFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.biblicaTermsFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ignoreListFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.punctuationMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.entireVerseFiltersMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showIgnoredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.referencesMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.hideLooseMatchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideIncorrectNameStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideTagShouldntExistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMissingTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideIncorrectTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideMalformedTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideBadReferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchMenuTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchLabelMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.btnShowIgnoreList = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.filterSetupWorker = new System.ComponentModel.BackgroundWorker();
            this.statusLabel = new System.Windows.Forms.Label();
            this.referencesTextBox = new System.Windows.Forms.RichTextBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.directoryEntry1 = new System.DirectoryServices.DirectoryEntry();
            this.runnerSetupWorker = new System.ComponentModel.BackgroundWorker();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).BeginInit();
            this.contextMenu.SuspendLayout();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnRunChecks
            // 
            this.btnRunChecks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunChecks.Location = new System.Drawing.Point(686, 531);
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
            this.dgvCheckResults.AllowUserToResizeRows = false;
            this.dgvCheckResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCheckResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCheckResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCheckResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCheckResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCheckResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.bcv,
            this.Verse,
            this.categoryColumn,
            this.Match,
            this.error,
            this.actionsAcceptColumn,
            this.actionsIgnoreColumn});
            this.dgvCheckResults.ContextMenuStrip = this.contextMenu;
            this.dgvCheckResults.Location = new System.Drawing.Point(3, 3);
            this.dgvCheckResults.MultiSelect = false;
            this.dgvCheckResults.Name = "dgvCheckResults";
            this.dgvCheckResults.ReadOnly = true;
            this.dgvCheckResults.RowHeadersVisible = false;
            this.dgvCheckResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.dgvCheckResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCheckResults.Size = new System.Drawing.Size(821, 228);
            this.dgvCheckResults.TabIndex = 2;
            this.dgvCheckResults.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCheckResults_CellClick);
            this.dgvCheckResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCheckResults_CellContentClick);
            this.dgvCheckResults.SelectionChanged += new System.EventHandler(this.dgvCheckResults_SelectionChanged);
            this.dgvCheckResults.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.dgvCheckResults_KeyPress);
            // 
            // bcv
            // 
            this.bcv.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.bcv.FillWeight = 10F;
            this.bcv.HeaderText = "BCV";
            this.bcv.MinimumWidth = 60;
            this.bcv.Name = "bcv";
            this.bcv.ReadOnly = true;
            this.bcv.Width = 60;
            // 
            // Verse
            // 
            this.Verse.FillWeight = 35F;
            this.Verse.HeaderText = "Verse";
            this.Verse.MinimumWidth = 120;
            this.Verse.Name = "Verse";
            this.Verse.ReadOnly = true;
            this.Verse.Visible = false;
            // 
            // categoryColumn
            // 
            this.categoryColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.categoryColumn.FillWeight = 20F;
            this.categoryColumn.HeaderText = "Category";
            this.categoryColumn.Name = "categoryColumn";
            this.categoryColumn.ReadOnly = true;
            // 
            // Match
            // 
            this.Match.FillWeight = 15F;
            this.Match.HeaderText = "Match";
            this.Match.MinimumWidth = 60;
            this.Match.Name = "Match";
            this.Match.ReadOnly = true;
            // 
            // error
            // 
            this.error.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.error.HeaderText = "Description";
            this.error.MinimumWidth = 120;
            this.error.Name = "error";
            this.error.ReadOnly = true;
            // 
            // actionsAcceptColumn
            // 
            this.actionsAcceptColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.actionsAcceptColumn.HeaderText = "Actions";
            this.actionsAcceptColumn.MinimumWidth = 50;
            this.actionsAcceptColumn.Name = "actionsAcceptColumn";
            this.actionsAcceptColumn.ReadOnly = true;
            this.actionsAcceptColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.actionsAcceptColumn.Visible = false;
            // 
            // actionsIgnoreColumn
            // 
            this.actionsIgnoreColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.actionsIgnoreColumn.HeaderText = "Action";
            this.actionsIgnoreColumn.MinimumWidth = 70;
            this.actionsIgnoreColumn.Name = "actionsIgnoreColumn";
            this.actionsIgnoreColumn.ReadOnly = true;
            this.actionsIgnoreColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.actionsIgnoreColumn.Text = "Un-Ignore";
            this.actionsIgnoreColumn.ToolTipText = "Ignore this exception in the future.";
            this.actionsIgnoreColumn.Width = 70;
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
            this.toolsToolStripMenuItem,
            this.searchMenuTextBox,
            this.searchLabelMenu,
            this.helpToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(10, 10);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(836, 27);
            this.mainMenu.TabIndex = 3;
            this.mainMenu.Text = "mainMenu";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveResultsMenuItem});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 23);
            this.fileMenu.Text = "&File";
            // 
            // saveResultsMenuItem
            // 
            this.saveResultsMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveResultsMenuItem.Image")));
            this.saveResultsMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveResultsMenuItem.Name = "saveResultsMenuItem";
            this.saveResultsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveResultsMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveResultsMenuItem.Text = "&Save Results...";
            this.saveResultsMenuItem.Click += new System.EventHandler(this.OnFileSaveResultsMenuClick);
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
            this.currentProjectAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndClearArea);
            // 
            // currentBookAreaMenuItem
            // 
            this.currentBookAreaMenuItem.Name = "currentBookAreaMenuItem";
            this.currentBookAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.currentBookAreaMenuItem.Text = "Current &Book";
            this.currentBookAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndClearArea);
            // 
            // currentChapterAreaMenuItem
            // 
            this.currentChapterAreaMenuItem.Name = "currentChapterAreaMenuItem";
            this.currentChapterAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.currentChapterAreaMenuItem.Text = "Current &Chapter";
            this.currentChapterAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndClearArea);
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
            this.mainTextAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndCheckContexts);
            // 
            // introductionsAreaMenuItem
            // 
            this.introductionsAreaMenuItem.Checked = true;
            this.introductionsAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.introductionsAreaMenuItem.Name = "introductionsAreaMenuItem";
            this.introductionsAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.introductionsAreaMenuItem.Text = "&Introductions";
            this.introductionsAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndCheckContexts);
            // 
            // outlinesAreaMenuItem
            // 
            this.outlinesAreaMenuItem.Checked = true;
            this.outlinesAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outlinesAreaMenuItem.Name = "outlinesAreaMenuItem";
            this.outlinesAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.outlinesAreaMenuItem.Text = "&Outlines";
            this.outlinesAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndCheckContexts);
            // 
            // notesAndReferencesAreaMenuItem
            // 
            this.notesAndReferencesAreaMenuItem.Checked = true;
            this.notesAndReferencesAreaMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.notesAndReferencesAreaMenuItem.Name = "notesAndReferencesAreaMenuItem";
            this.notesAndReferencesAreaMenuItem.Size = new System.Drawing.Size(178, 22);
            this.notesAndReferencesAreaMenuItem.Text = "&Notes && References";
            this.notesAndReferencesAreaMenuItem.Click += new System.EventHandler(this.OnMenuItemClickAndCheckContexts);
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
            this.wordListFiltersMenuItem.Size = new System.Drawing.Size(242, 22);
            this.wordListFiltersMenuItem.Text = "Hide &Word List Matches";
            this.wordListFiltersMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // biblicaTermsFiltersMenuItem
            // 
            this.biblicaTermsFiltersMenuItem.Name = "biblicaTermsFiltersMenuItem";
            this.biblicaTermsFiltersMenuItem.Size = new System.Drawing.Size(242, 22);
            this.biblicaTermsFiltersMenuItem.Text = "Hide &Biblical Term Matches";
            this.biblicaTermsFiltersMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // ignoreListFiltersMenuItem
            // 
            this.ignoreListFiltersMenuItem.Name = "ignoreListFiltersMenuItem";
            this.ignoreListFiltersMenuItem.Size = new System.Drawing.Size(242, 22);
            this.ignoreListFiltersMenuItem.Text = "Hide &Ignored Word List";
            this.ignoreListFiltersMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // punctuationMenuSeparator
            // 
            this.punctuationMenuSeparator.Name = "punctuationMenuSeparator";
            this.punctuationMenuSeparator.Size = new System.Drawing.Size(239, 6);
            // 
            // entireVerseFiltersMenuItem
            // 
            this.entireVerseFiltersMenuItem.Name = "entireVerseFiltersMenuItem";
            this.entireVerseFiltersMenuItem.Size = new System.Drawing.Size(242, 22);
            this.entireVerseFiltersMenuItem.Text = "Show Only Entire &Verse Matches";
            this.entireVerseFiltersMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // showIgnoredToolStripMenuItem
            // 
            this.showIgnoredToolStripMenuItem.Name = "showIgnoredToolStripMenuItem";
            this.showIgnoredToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.showIgnoredToolStripMenuItem.Text = "Show Ignored &Exceptions";
            this.showIgnoredToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // referencesMenuSeparator
            // 
            this.referencesMenuSeparator.Name = "referencesMenuSeparator";
            this.referencesMenuSeparator.Size = new System.Drawing.Size(239, 6);
            // 
            // hideLooseMatchesToolStripMenuItem
            // 
            this.hideLooseMatchesToolStripMenuItem.Name = "hideLooseMatchesToolStripMenuItem";
            this.hideLooseMatchesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideLooseMatchesToolStripMenuItem.Text = "Hide &Loose Matches";
            this.hideLooseMatchesToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideIncorrectNameStyleToolStripMenuItem
            // 
            this.hideIncorrectNameStyleToolStripMenuItem.Name = "hideIncorrectNameStyleToolStripMenuItem";
            this.hideIncorrectNameStyleToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideIncorrectNameStyleToolStripMenuItem.Text = "Hide Incorrect &Name Style";
            this.hideIncorrectNameStyleToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideTagShouldntExistToolStripMenuItem
            // 
            this.hideTagShouldntExistToolStripMenuItem.Name = "hideTagShouldntExistToolStripMenuItem";
            this.hideTagShouldntExistToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideTagShouldntExistToolStripMenuItem.Text = "Hide &Tag Shouldn\'t Exist";
            this.hideTagShouldntExistToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideMissingTagToolStripMenuItem
            // 
            this.hideMissingTagToolStripMenuItem.Name = "hideMissingTagToolStripMenuItem";
            this.hideMissingTagToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideMissingTagToolStripMenuItem.Text = "Hide &Missing Tag";
            this.hideMissingTagToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideIncorrectTagToolStripMenuItem
            // 
            this.hideIncorrectTagToolStripMenuItem.Name = "hideIncorrectTagToolStripMenuItem";
            this.hideIncorrectTagToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideIncorrectTagToolStripMenuItem.Text = "Hide In&correct Tag";
            this.hideIncorrectTagToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideMalformedTagToolStripMenuItem
            // 
            this.hideMalformedTagToolStripMenuItem.Name = "hideMalformedTagToolStripMenuItem";
            this.hideMalformedTagToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideMalformedTagToolStripMenuItem.Text = "&Hide Malformed Tag";
            this.hideMalformedTagToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
            // 
            // hideBadReferencesToolStripMenuItem
            // 
            this.hideBadReferencesToolStripMenuItem.Name = "hideBadReferencesToolStripMenuItem";
            this.hideBadReferencesToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.hideBadReferencesToolStripMenuItem.Text = "Hide Bad &References";
            this.hideBadReferencesToolStripMenuItem.Click += new System.EventHandler(this.OnMenuItemClick);
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
            this.btnShowIgnoreList.Location = new System.Drawing.Point(17, 531);
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
            this.btnClose.Location = new System.Drawing.Point(767, 531);
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
            this.statusLabel.Location = new System.Drawing.Point(98, 536);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(71, 13);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "No violations.";
            // 
            // referencesTextBox
            // 
            this.referencesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.referencesTextBox.BackColor = System.Drawing.Color.White;
            this.referencesTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.referencesTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.referencesTextBox.Location = new System.Drawing.Point(3, 3);
            this.referencesTextBox.Name = "referencesTextBox";
            this.referencesTextBox.ReadOnly = true;
            this.referencesTextBox.Size = new System.Drawing.Size(821, 222);
            this.referencesTextBox.TabIndex = 3;
            this.referencesTextBox.Text = "";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(19, 52);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.dgvCheckResults);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.referencesTextBox);
            this.splitContainerMain.Size = new System.Drawing.Size(827, 469);
            this.splitContainerMain.SplitterDistance = 234;
            this.splitContainerMain.TabIndex = 8;
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 23);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(856, 567);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.btnShowIgnoreList);
            this.Controls.Add(this.statusLabel);
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
            this.Load += new System.EventHandler(this.OnMainFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheckResults)).EndInit();
            this.contextMenu.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRunChecks;
        private System.Windows.Forms.DataGridView dgvCheckResults;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem saveResultsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.Button btnShowIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem wordListFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ignoreListFiltersMenuItem;
        private System.Windows.Forms.Button btnClose;
        private System.ComponentModel.BackgroundWorker filterSetupWorker;
        private System.Windows.Forms.ToolStripMenuItem searchLabelMenu;
        private System.Windows.Forms.ToolStripTextBox searchMenuTextBox;
        private System.Windows.Forms.ToolStripMenuItem areaMenu;
        private System.Windows.Forms.ToolStripMenuItem currentProjectAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentBookAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentChapterAreaMenuItem;
        private System.Windows.Forms.ToolStripSeparator punctuationMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem entireVerseFiltersMenuItem;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToIgnoreList;
        private System.Windows.Forms.ToolStripMenuItem removeFromIgnoreList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mainTextAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notesAndReferencesAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem biblicaTermsFiltersMenuItem;
        private System.Windows.Forms.ToolStripMenuItem introductionsAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem outlinesAreaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showIgnoredToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator referencesMenuSeparator;
        private System.Windows.Forms.ToolStripMenuItem hideLooseMatchesToolStripMenuItem;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.ToolStripMenuItem hideIncorrectNameStyleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideTagShouldntExistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMissingTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideIncorrectTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideMalformedTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideBadReferencesToolStripMenuItem;
        private System.Windows.Forms.RichTextBox referencesTextBox;
        private System.DirectoryServices.DirectoryEntry directoryEntry1;
        private System.ComponentModel.BackgroundWorker runnerSetupWorker;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.DataGridViewTextBoxColumn bcv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Verse;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Match;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.DataGridViewTextBoxColumn actionsAcceptColumn;
        private System.Windows.Forms.DataGridViewButtonColumn actionsIgnoreColumn;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
    }
}