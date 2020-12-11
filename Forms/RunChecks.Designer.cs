
namespace TvpMain.Forms
{
    partial class RunChecks
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.runChecksMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectLabel = new System.Windows.Forms.Label();
            this.checksGroupBox = new System.Windows.Forms.GroupBox();
            this.setDefaultsToSelected = new System.Windows.Forms.Button();
            this.resetToProjectDefaultsButton = new System.Windows.Forms.Button();
            this.checksList = new System.Windows.Forms.DataGridView();
            this.contextGroupBox = new System.Windows.Forms.GroupBox();
            this.toChapterLabel = new System.Windows.Forms.Label();
            this.chapterLabel = new System.Windows.Forms.Label();
            this.chooseBooksButton = new System.Windows.Forms.Button();
            this.toChapterDropDown = new System.Windows.Forms.ComboBox();
            this.fromChapterDropDown = new System.Windows.Forms.ComboBox();
            this.currentBookText = new System.Windows.Forms.TextBox();
            this.chooseBooksText = new System.Windows.Forms.TextBox();
            this.chooseBooksRadioButton = new System.Windows.Forms.RadioButton();
            this.currentBookRadioButton = new System.Windows.Forms.RadioButton();
            this.Copyright = new System.Windows.Forms.Label();
            this.runChecksButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.helpTextBox = new System.Windows.Forms.TextBox();
            this.projectNameText = new System.Windows.Forms.Label();
            this.loadingWorker = new System.ComponentModel.BackgroundWorker();
            this.CFSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CFName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFLanguages = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CFTags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.runChecksMenu.SuspendLayout();
            this.checksGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksList)).BeginInit();
            this.contextGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // runChecksMenu
            // 
            this.runChecksMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editorToolStripMenuItem,
            this.licenseToolStripMenuItem});
            this.runChecksMenu.Location = new System.Drawing.Point(10, 10);
            this.runChecksMenu.Name = "runChecksMenu";
            this.runChecksMenu.Size = new System.Drawing.Size(856, 24);
            this.runChecksMenu.TabIndex = 0;
            this.runChecksMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editorToolStripMenuItem
            // 
            this.editorToolStripMenuItem.Name = "editorToolStripMenuItem";
            this.editorToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.editorToolStripMenuItem.Text = "Editor";
            this.editorToolStripMenuItem.Click += new System.EventHandler(this.editorToolStripMenuItem_Click);
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.licenseToolStripMenuItem.Text = "License";
            this.licenseToolStripMenuItem.Click += new System.EventHandler(this.licenseToolStripMenuItem_Click);
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(14, 44);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(40, 13);
            this.projectLabel.TabIndex = 2;
            this.projectLabel.Text = "Project";
            // 
            // checksGroupBox
            // 
            this.checksGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checksGroupBox.Controls.Add(this.setDefaultsToSelected);
            this.checksGroupBox.Controls.Add(this.resetToProjectDefaultsButton);
            this.checksGroupBox.Controls.Add(this.checksList);
            this.checksGroupBox.Location = new System.Drawing.Point(13, 64);
            this.checksGroupBox.Name = "checksGroupBox";
            this.checksGroupBox.Size = new System.Drawing.Size(850, 380);
            this.checksGroupBox.TabIndex = 3;
            this.checksGroupBox.TabStop = false;
            this.checksGroupBox.Text = "Checks";
            // 
            // setDefaultsToSelected
            // 
            this.setDefaultsToSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.setDefaultsToSelected.Location = new System.Drawing.Point(6, 351);
            this.setDefaultsToSelected.Name = "setDefaultsToSelected";
            this.setDefaultsToSelected.Size = new System.Drawing.Size(229, 23);
            this.setDefaultsToSelected.TabIndex = 2;
            this.setDefaultsToSelected.Text = "Set Selected Checks as the Project Defaults";
            this.setDefaultsToSelected.UseVisualStyleBackColor = true;
            this.setDefaultsToSelected.Click += new System.EventHandler(this.setDefaultsToSelected_Click);
            this.setDefaultsToSelected.MouseEnter += new System.EventHandler(this.setDefaultsToSelected_MouseEnter);
            // 
            // resetToProjectDefaultsButton
            // 
            this.resetToProjectDefaultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetToProjectDefaultsButton.Location = new System.Drawing.Point(702, 351);
            this.resetToProjectDefaultsButton.Name = "resetToProjectDefaultsButton";
            this.resetToProjectDefaultsButton.Size = new System.Drawing.Size(142, 23);
            this.resetToProjectDefaultsButton.TabIndex = 1;
            this.resetToProjectDefaultsButton.Text = "Reset to Project Defaults";
            this.resetToProjectDefaultsButton.UseVisualStyleBackColor = true;
            this.resetToProjectDefaultsButton.Click += new System.EventHandler(this.resetToProjectDefaults_Click);
            this.resetToProjectDefaultsButton.MouseEnter += new System.EventHandler(this.resetToProjectDefaults_MouseEnter);
            // 
            // checksList
            // 
            this.checksList.AllowUserToAddRows = false;
            this.checksList.AllowUserToDeleteRows = false;
            this.checksList.AllowUserToResizeRows = false;
            this.checksList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checksList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.checksList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.checksList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.checksList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CFSelected,
            this.CFName,
            this.Version,
            this.CFLanguages,
            this.CFTags,
            this.Id});
            this.checksList.Location = new System.Drawing.Point(6, 19);
            this.checksList.MultiSelect = false;
            this.checksList.Name = "checksList";
            this.checksList.ReadOnly = true;
            this.checksList.RowHeadersVisible = false;
            this.checksList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.checksList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.checksList.Size = new System.Drawing.Size(838, 326);
            this.checksList.TabIndex = 0;
            this.checksList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.checksList_CellClick);
            this.checksList.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.checksList_CellMouseEnter);
            // 
            // contextGroupBox
            // 
            this.contextGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contextGroupBox.Controls.Add(this.toChapterLabel);
            this.contextGroupBox.Controls.Add(this.chapterLabel);
            this.contextGroupBox.Controls.Add(this.chooseBooksButton);
            this.contextGroupBox.Controls.Add(this.toChapterDropDown);
            this.contextGroupBox.Controls.Add(this.fromChapterDropDown);
            this.contextGroupBox.Controls.Add(this.currentBookText);
            this.contextGroupBox.Controls.Add(this.chooseBooksText);
            this.contextGroupBox.Controls.Add(this.chooseBooksRadioButton);
            this.contextGroupBox.Controls.Add(this.currentBookRadioButton);
            this.contextGroupBox.Location = new System.Drawing.Point(13, 450);
            this.contextGroupBox.Name = "contextGroupBox";
            this.contextGroupBox.Size = new System.Drawing.Size(850, 82);
            this.contextGroupBox.TabIndex = 4;
            this.contextGroupBox.TabStop = false;
            // 
            // toChapterLabel
            // 
            this.toChapterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toChapterLabel.AutoSize = true;
            this.toChapterLabel.Location = new System.Drawing.Point(747, 21);
            this.toChapterLabel.Name = "toChapterLabel";
            this.toChapterLabel.Size = new System.Drawing.Size(16, 13);
            this.toChapterLabel.TabIndex = 8;
            this.toChapterLabel.Text = "to";
            // 
            // chapterLabel
            // 
            this.chapterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chapterLabel.AutoSize = true;
            this.chapterLabel.Location = new System.Drawing.Point(611, 21);
            this.chapterLabel.Name = "chapterLabel";
            this.chapterLabel.Size = new System.Drawing.Size(49, 13);
            this.chapterLabel.TabIndex = 7;
            this.chapterLabel.Text = "Chapters";
            // 
            // chooseBooksButton
            // 
            this.chooseBooksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseBooksButton.Location = new System.Drawing.Point(769, 46);
            this.chooseBooksButton.Name = "chooseBooksButton";
            this.chooseBooksButton.Size = new System.Drawing.Size(75, 23);
            this.chooseBooksButton.TabIndex = 6;
            this.chooseBooksButton.Text = "Choose...";
            this.chooseBooksButton.UseVisualStyleBackColor = true;
            this.chooseBooksButton.Click += new System.EventHandler(this.chooseBooksButton_Click);
            this.chooseBooksButton.MouseEnter += new System.EventHandler(this.chooseBooksRadioButton_MouseEnter);
            // 
            // toChapterDropDown
            // 
            this.toChapterDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.toChapterDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toChapterDropDown.FormattingEnabled = true;
            this.toChapterDropDown.Location = new System.Drawing.Point(769, 19);
            this.toChapterDropDown.Name = "toChapterDropDown";
            this.toChapterDropDown.Size = new System.Drawing.Size(75, 21);
            this.toChapterDropDown.TabIndex = 5;
            this.toChapterDropDown.MouseEnter += new System.EventHandler(this.toChapterDropDown_MouseEnter);
            // 
            // fromChapterDropDown
            // 
            this.fromChapterDropDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fromChapterDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fromChapterDropDown.FormattingEnabled = true;
            this.fromChapterDropDown.Location = new System.Drawing.Point(666, 18);
            this.fromChapterDropDown.Name = "fromChapterDropDown";
            this.fromChapterDropDown.Size = new System.Drawing.Size(75, 21);
            this.fromChapterDropDown.TabIndex = 4;
            this.fromChapterDropDown.MouseEnter += new System.EventHandler(this.fromChapterDropDown_MouseEnter);
            // 
            // currentBookText
            // 
            this.currentBookText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.currentBookText.Location = new System.Drawing.Point(100, 19);
            this.currentBookText.Name = "currentBookText";
            this.currentBookText.ReadOnly = true;
            this.currentBookText.Size = new System.Drawing.Size(505, 20);
            this.currentBookText.TabIndex = 3;
            this.currentBookText.MouseEnter += new System.EventHandler(this.currentBookRadioButton_MouseEnter);
            // 
            // chooseBooksText
            // 
            this.chooseBooksText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseBooksText.Location = new System.Drawing.Point(100, 48);
            this.chooseBooksText.Name = "chooseBooksText";
            this.chooseBooksText.ReadOnly = true;
            this.chooseBooksText.Size = new System.Drawing.Size(663, 20);
            this.chooseBooksText.TabIndex = 2;
            this.chooseBooksText.Text = "*none*";
            this.chooseBooksText.MouseEnter += new System.EventHandler(this.chooseBooksText_MouseEnter);
            // 
            // chooseBooksRadioButton
            // 
            this.chooseBooksRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chooseBooksRadioButton.AutoSize = true;
            this.chooseBooksRadioButton.Location = new System.Drawing.Point(6, 49);
            this.chooseBooksRadioButton.Name = "chooseBooksRadioButton";
            this.chooseBooksRadioButton.Size = new System.Drawing.Size(94, 17);
            this.chooseBooksRadioButton.TabIndex = 1;
            this.chooseBooksRadioButton.TabStop = true;
            this.chooseBooksRadioButton.Text = "Choose Books";
            this.chooseBooksRadioButton.UseVisualStyleBackColor = true;
            this.chooseBooksRadioButton.Click += new System.EventHandler(this.chooseBooksRadioButton_Click);
            this.chooseBooksRadioButton.MouseEnter += new System.EventHandler(this.chooseBooksRadioButton_MouseEnter);
            // 
            // currentBookRadioButton
            // 
            this.currentBookRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.currentBookRadioButton.AutoSize = true;
            this.currentBookRadioButton.Location = new System.Drawing.Point(7, 19);
            this.currentBookRadioButton.Name = "currentBookRadioButton";
            this.currentBookRadioButton.Size = new System.Drawing.Size(87, 17);
            this.currentBookRadioButton.TabIndex = 0;
            this.currentBookRadioButton.TabStop = true;
            this.currentBookRadioButton.Text = "Current Book";
            this.currentBookRadioButton.UseVisualStyleBackColor = true;
            this.currentBookRadioButton.Click += new System.EventHandler(this.currentBookRadioButton_Click);
            this.currentBookRadioButton.MouseEnter += new System.EventHandler(this.currentBookRadioButton_MouseEnter);
            // 
            // Copyright
            // 
            this.Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Copyright.AutoSize = true;
            this.Copyright.Location = new System.Drawing.Point(17, 653);
            this.Copyright.Name = "Copyright";
            this.Copyright.Size = new System.Drawing.Size(101, 13);
            this.Copyright.TabIndex = 10;
            this.Copyright.Text = "© 2020 Biblica, Inc.";
            // 
            // runChecksButton
            // 
            this.runChecksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runChecksButton.Location = new System.Drawing.Point(782, 648);
            this.runChecksButton.Name = "runChecksButton";
            this.runChecksButton.Size = new System.Drawing.Size(75, 23);
            this.runChecksButton.TabIndex = 11;
            this.runChecksButton.Text = "Run Checks";
            this.runChecksButton.UseVisualStyleBackColor = true;
            this.runChecksButton.Click += new System.EventHandler(this.runChecksButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(701, 648);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // helpTextBox
            // 
            this.helpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextBox.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.helpTextBox.Location = new System.Drawing.Point(13, 538);
            this.helpTextBox.Multiline = true;
            this.helpTextBox.Name = "helpTextBox";
            this.helpTextBox.ReadOnly = true;
            this.helpTextBox.Size = new System.Drawing.Size(850, 104);
            this.helpTextBox.TabIndex = 14;
            // 
            // projectNameText
            // 
            this.projectNameText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectNameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectNameText.Location = new System.Drawing.Point(60, 40);
            this.projectNameText.Name = "projectNameText";
            this.projectNameText.Size = new System.Drawing.Size(803, 21);
            this.projectNameText.TabIndex = 15;
            this.projectNameText.Text = "Project Name";
            this.projectNameText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loadingWorker
            // 
            this.loadingWorker.WorkerReportsProgress = true;
            this.loadingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadingWorker_DoWork);
            this.loadingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadingWorker_RunWorkerCompleted);
            // 
            // CFSelected
            // 
            this.CFSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CFSelected.FalseValue = "false";
            this.CFSelected.FillWeight = 20F;
            this.CFSelected.HeaderText = "Selected";
            this.CFSelected.Name = "CFSelected";
            this.CFSelected.ReadOnly = true;
            this.CFSelected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.CFSelected.TrueValue = "true";
            this.CFSelected.Width = 82;
            // 
            // CFName
            // 
            this.CFName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CFName.HeaderText = "Name";
            this.CFName.Name = "CFName";
            this.CFName.ReadOnly = true;
            // 
            // Version
            // 
            this.Version.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Version.FillWeight = 80F;
            this.Version.HeaderText = "Version";
            this.Version.Name = "Version";
            this.Version.ReadOnly = true;
            this.Version.Width = 74;
            // 
            // CFLanguages
            // 
            this.CFLanguages.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CFLanguages.FillWeight = 80F;
            this.CFLanguages.HeaderText = "Languages";
            this.CFLanguages.MinimumWidth = 70;
            this.CFLanguages.Name = "CFLanguages";
            this.CFLanguages.ReadOnly = true;
            this.CFLanguages.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CFLanguages.Width = 94;
            // 
            // CFTags
            // 
            this.CFTags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CFTags.FillWeight = 80F;
            this.CFTags.HeaderText = "Tags";
            this.CFTags.MinimumWidth = 70;
            this.CFTags.Name = "CFTags";
            this.CFTags.ReadOnly = true;
            this.CFTags.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CFTags.Width = 70;
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // RunChecks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(876, 685);
            this.Controls.Add(this.projectNameText);
            this.Controls.Add(this.helpTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.runChecksButton);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.contextGroupBox);
            this.Controls.Add(this.checksGroupBox);
            this.Controls.Add(this.projectLabel);
            this.Controls.Add(this.runChecksMenu);
            this.MainMenuStrip = this.runChecksMenu;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunChecks";
            this.Padding = new System.Windows.Forms.Padding(10, 10, 10, 25);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Run Checks";
            this.Load += new System.EventHandler(this.RunChecks_Load);
            this.runChecksMenu.ResumeLayout(false);
            this.runChecksMenu.PerformLayout();
            this.checksGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.checksList)).EndInit();
            this.contextGroupBox.ResumeLayout(false);
            this.contextGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip runChecksMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.GroupBox checksGroupBox;
        private System.Windows.Forms.DataGridView checksList;
        private System.Windows.Forms.GroupBox contextGroupBox;
        private System.Windows.Forms.Label chapterLabel;
        private System.Windows.Forms.Button chooseBooksButton;
        private System.Windows.Forms.ComboBox toChapterDropDown;
        private System.Windows.Forms.ComboBox fromChapterDropDown;
        private System.Windows.Forms.TextBox currentBookText;
        private System.Windows.Forms.TextBox chooseBooksText;
        private System.Windows.Forms.RadioButton chooseBooksRadioButton;
        private System.Windows.Forms.RadioButton currentBookRadioButton;
        private System.Windows.Forms.Button resetToProjectDefaultsButton;
        private System.Windows.Forms.Button setDefaultsToSelected;
        private System.Windows.Forms.Label Copyright;
        private System.Windows.Forms.Label toChapterLabel;
        private System.Windows.Forms.Button runChecksButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox helpTextBox;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label projectNameText;
        private System.ComponentModel.BackgroundWorker loadingWorker;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CFSelected;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Version;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFLanguages;
        private System.Windows.Forms.DataGridViewTextBoxColumn CFTags;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
    }
}