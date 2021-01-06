namespace TvpMain.Forms
{
    partial class CheckResultsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckResultsForm));
            this.Deny = new System.Windows.Forms.Button();
            this.ShowDenied = new System.Windows.Forms.CheckBox();
            this.cancel = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LicenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.bookFilterLabel = new System.Windows.Forms.Label();
            this.clearCheckFilterButton = new System.Windows.Forms.Button();
            this.bookFilterClearButton = new System.Windows.Forms.Button();
            this.selectBooksButton = new System.Windows.Forms.Button();
            this.bookFilterTextBox = new System.Windows.Forms.TextBox();
            this.filterTextBox = new System.Windows.Forms.TextBox();
            this.checkFilterLabel = new System.Windows.Forms.Label();
            this.checksDataGridView = new System.Windows.Forms.DataGridView();
            this.SelectedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CategoryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ActionsColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.issuesDataGridView = new System.Windows.Forms.DataGridView();
            this.ReferenceColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchTextColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.matchTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fixTextBox = new System.Windows.Forms.RichTextBox();
            this.menuStrip.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.issuesDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // Deny
            // 
            this.Deny.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Deny.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Deny.Location = new System.Drawing.Point(680, 410);
            this.Deny.Name = "Deny";
            this.Deny.Size = new System.Drawing.Size(156, 50);
            this.Deny.TabIndex = 1;
            this.Deny.Text = "Deny";
            this.Deny.UseVisualStyleBackColor = true;
            this.Deny.Click += new System.EventHandler(this.Deny_Click);
            // 
            // ShowDenied
            // 
            this.ShowDenied.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowDenied.AutoSize = true;
            this.ShowDenied.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ShowDenied.Location = new System.Drawing.Point(705, 466);
            this.ShowDenied.Name = "ShowDenied";
            this.ShowDenied.Size = new System.Drawing.Size(110, 21);
            this.ShowDenied.TabIndex = 2;
            this.ShowDenied.Text = "Show Denied";
            this.ShowDenied.UseVisualStyleBackColor = true;
            this.ShowDenied.CheckedChanged += new System.EventHandler(this.ShowDenied_CheckedChanged);
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(900, 582);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.LicenseToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(987, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // LicenseToolStripMenuItem
            // 
            this.LicenseToolStripMenuItem.Name = "LicenseToolStripMenuItem";
            this.LicenseToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.LicenseToolStripMenuItem.Text = "License";
            this.LicenseToolStripMenuItem.Click += new System.EventHandler(this.LicenseToolStripMenuItem_Click);
            // 
            // topPanel
            // 
            this.topPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topPanel.Controls.Add(this.bookFilterLabel);
            this.topPanel.Controls.Add(this.clearCheckFilterButton);
            this.topPanel.Controls.Add(this.bookFilterClearButton);
            this.topPanel.Controls.Add(this.selectBooksButton);
            this.topPanel.Controls.Add(this.bookFilterTextBox);
            this.topPanel.Controls.Add(this.filterTextBox);
            this.topPanel.Controls.Add(this.checkFilterLabel);
            this.topPanel.Controls.Add(this.checksDataGridView);
            this.topPanel.Location = new System.Drawing.Point(0, 27);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(987, 251);
            this.topPanel.TabIndex = 2;
            // 
            // bookFilterLabel
            // 
            this.bookFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookFilterLabel.AutoSize = true;
            this.bookFilterLabel.Location = new System.Drawing.Point(747, 53);
            this.bookFilterLabel.Name = "bookFilterLabel";
            this.bookFilterLabel.Size = new System.Drawing.Size(57, 13);
            this.bookFilterLabel.TabIndex = 8;
            this.bookFilterLabel.Text = "Book Filter";
            // 
            // clearCheckFilterButton
            // 
            this.clearCheckFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearCheckFilterButton.Location = new System.Drawing.Point(954, 21);
            this.clearCheckFilterButton.Name = "clearCheckFilterButton";
            this.clearCheckFilterButton.Size = new System.Drawing.Size(21, 20);
            this.clearCheckFilterButton.TabIndex = 7;
            this.clearCheckFilterButton.Text = "X";
            this.clearCheckFilterButton.UseVisualStyleBackColor = true;
            this.clearCheckFilterButton.Click += new System.EventHandler(this.clearCheckFilterButton_Click);
            // 
            // bookFilterClearButton
            // 
            this.bookFilterClearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookFilterClearButton.Location = new System.Drawing.Point(954, 70);
            this.bookFilterClearButton.Name = "bookFilterClearButton";
            this.bookFilterClearButton.Size = new System.Drawing.Size(20, 20);
            this.bookFilterClearButton.TabIndex = 6;
            this.bookFilterClearButton.Text = "X";
            this.bookFilterClearButton.UseVisualStyleBackColor = true;
            this.bookFilterClearButton.Click += new System.EventHandler(this.bookFilterClearButton_Click);
            // 
            // selectBooksButton
            // 
            this.selectBooksButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectBooksButton.Location = new System.Drawing.Point(885, 96);
            this.selectBooksButton.Name = "selectBooksButton";
            this.selectBooksButton.Size = new System.Drawing.Size(89, 23);
            this.selectBooksButton.TabIndex = 5;
            this.selectBooksButton.Text = "Select Books";
            this.selectBooksButton.UseVisualStyleBackColor = true;
            this.selectBooksButton.Click += new System.EventHandler(this.selectBooksButton_Click);
            // 
            // bookFilterTextBox
            // 
            this.bookFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookFilterTextBox.Location = new System.Drawing.Point(750, 70);
            this.bookFilterTextBox.Name = "bookFilterTextBox";
            this.bookFilterTextBox.ReadOnly = true;
            this.bookFilterTextBox.Size = new System.Drawing.Size(198, 20);
            this.bookFilterTextBox.TabIndex = 4;
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(750, 21);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(198, 20);
            this.filterTextBox.TabIndex = 2;
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // checkFilterLabel
            // 
            this.checkFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkFilterLabel.AutoSize = true;
            this.checkFilterLabel.Location = new System.Drawing.Point(747, 4);
            this.checkFilterLabel.Name = "checkFilterLabel";
            this.checkFilterLabel.Size = new System.Drawing.Size(63, 13);
            this.checkFilterLabel.TabIndex = 1;
            this.checkFilterLabel.Text = "Check Filter";
            // 
            // checksDataGridView
            // 
            this.checksDataGridView.AllowUserToAddRows = false;
            this.checksDataGridView.AllowUserToDeleteRows = false;
            this.checksDataGridView.AllowUserToResizeRows = false;
            this.checksDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.checksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectedColumn,
            this.CategoryColumn,
            this.DescriptionColumn,
            this.CountColumn,
            this.ActionsColumn});
            this.checksDataGridView.Location = new System.Drawing.Point(0, 0);
            this.checksDataGridView.MultiSelect = false;
            this.checksDataGridView.Name = "checksDataGridView";
            this.checksDataGridView.ReadOnly = true;
            this.checksDataGridView.RowHeadersVisible = false;
            this.checksDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.checksDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.checksDataGridView.Size = new System.Drawing.Size(740, 251);
            this.checksDataGridView.TabIndex = 0;
            this.checksDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.checksDataGridView_CellContentClick);
            this.checksDataGridView.SelectionChanged += new System.EventHandler(this.checksDataGridView_SelectionChanged);
            // 
            // SelectedColumn
            // 
            this.SelectedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SelectedColumn.FillWeight = 50F;
            this.SelectedColumn.HeaderText = "Selected";
            this.SelectedColumn.MinimumWidth = 70;
            this.SelectedColumn.Name = "SelectedColumn";
            this.SelectedColumn.ReadOnly = true;
            this.SelectedColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SelectedColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SelectedColumn.Width = 74;
            // 
            // CategoryColumn
            // 
            this.CategoryColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CategoryColumn.HeaderText = "Category";
            this.CategoryColumn.Name = "CategoryColumn";
            this.CategoryColumn.ReadOnly = true;
            this.CategoryColumn.Width = 74;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ReadOnly = true;
            // 
            // CountColumn
            // 
            this.CountColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CountColumn.FillWeight = 15F;
            this.CountColumn.HeaderText = "Count";
            this.CountColumn.MinimumWidth = 20;
            this.CountColumn.Name = "CountColumn";
            this.CountColumn.ReadOnly = true;
            this.CountColumn.Width = 60;
            // 
            // ActionsColumn
            // 
            this.ActionsColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ActionsColumn.FillWeight = 15F;
            this.ActionsColumn.HeaderText = "Auto Fix";
            this.ActionsColumn.MinimumWidth = 20;
            this.ActionsColumn.Name = "ActionsColumn";
            this.ActionsColumn.ReadOnly = true;
            this.ActionsColumn.Width = 51;
            // 
            // issuesDataGridView
            // 
            this.issuesDataGridView.AllowUserToAddRows = false;
            this.issuesDataGridView.AllowUserToDeleteRows = false;
            this.issuesDataGridView.AllowUserToResizeRows = false;
            this.issuesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.issuesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.issuesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReferenceColumn,
            this.MatchTextColumn});
            this.issuesDataGridView.Location = new System.Drawing.Point(0, 284);
            this.issuesDataGridView.MultiSelect = false;
            this.issuesDataGridView.Name = "issuesDataGridView";
            this.issuesDataGridView.ReadOnly = true;
            this.issuesDataGridView.RowHeadersVisible = false;
            this.issuesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.issuesDataGridView.Size = new System.Drawing.Size(343, 285);
            this.issuesDataGridView.TabIndex = 3;
            this.issuesDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.issuesDataGridView_CellContentClick);
            this.issuesDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.issuesDataGridView_CellContentDoubleClick);
            this.issuesDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.issuesDataGridView_CellContentDoubleClick);
            this.issuesDataGridView.SelectionChanged += new System.EventHandler(this.issuesDataGridView_SelectionChanged);
            // 
            // ReferenceColumn
            // 
            this.ReferenceColumn.HeaderText = "Reference";
            this.ReferenceColumn.Name = "ReferenceColumn";
            this.ReferenceColumn.ReadOnly = true;
            // 
            // MatchTextColumn
            // 
            this.MatchTextColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MatchTextColumn.HeaderText = "Match Text";
            this.MatchTextColumn.Name = "MatchTextColumn";
            this.MatchTextColumn.ReadOnly = true;
            // 
            // matchTextBox
            // 
            this.matchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.matchTextBox.BackColor = System.Drawing.Color.White;
            this.matchTextBox.Location = new System.Drawing.Point(349, 300);
            this.matchTextBox.Name = "matchTextBox";
            this.matchTextBox.ReadOnly = true;
            this.matchTextBox.Size = new System.Drawing.Size(391, 125);
            this.matchTextBox.TabIndex = 4;
            this.matchTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(350, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Match in Context";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(350, 428);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Suggested Fix in Context";
            // 
            // fixTextBox
            // 
            this.fixTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fixTextBox.BackColor = System.Drawing.Color.White;
            this.fixTextBox.Location = new System.Drawing.Point(349, 444);
            this.fixTextBox.Name = "fixTextBox";
            this.fixTextBox.ReadOnly = true;
            this.fixTextBox.Size = new System.Drawing.Size(391, 125);
            this.fixTextBox.TabIndex = 7;
            this.fixTextBox.Text = "";
            // 
            // CheckResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 617);
            this.Controls.Add(this.fixTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matchTextBox);
            this.Controls.Add(this.issuesDataGridView);
            this.Controls.Add(this.ShowDenied);
            this.Controls.Add(this.Deny);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "CheckResultsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Check Results";
            this.Shown += new System.EventHandler(this.CheckResultsForm_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.issuesDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Deny;
        private System.Windows.Forms.CheckBox ShowDenied;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.ToolStripMenuItem LicenseToolStripMenuItem;
        private System.Windows.Forms.DataGridView checksDataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CategoryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountColumn;
        private System.Windows.Forms.DataGridViewButtonColumn ActionsColumn;
        private System.Windows.Forms.Label bookFilterLabel;
        private System.Windows.Forms.Button clearCheckFilterButton;
        private System.Windows.Forms.Button bookFilterClearButton;
        private System.Windows.Forms.Button selectBooksButton;
        private System.Windows.Forms.TextBox bookFilterTextBox;
        private System.Windows.Forms.TextBox filterTextBox;
        private System.Windows.Forms.Label checkFilterLabel;
        private System.Windows.Forms.DataGridView issuesDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReferenceColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MatchTextColumn;
        private System.Windows.Forms.RichTextBox matchTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox fixTextBox;
    }
}