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
            this.menuStrip.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(907, 700);
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
            this.menuStrip.Size = new System.Drawing.Size(994, 24);
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
            this.topPanel.Size = new System.Drawing.Size(994, 251);
            this.topPanel.TabIndex = 2;
            // 
            // bookFilterLabel
            // 
            this.bookFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookFilterLabel.AutoSize = true;
            this.bookFilterLabel.Location = new System.Drawing.Point(754, 53);
            this.bookFilterLabel.Name = "bookFilterLabel";
            this.bookFilterLabel.Size = new System.Drawing.Size(57, 13);
            this.bookFilterLabel.TabIndex = 8;
            this.bookFilterLabel.Text = "Book Filter";
            // 
            // clearCheckFilterButton
            // 
            this.clearCheckFilterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearCheckFilterButton.Location = new System.Drawing.Point(961, 21);
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
            this.bookFilterClearButton.Location = new System.Drawing.Point(961, 70);
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
            this.selectBooksButton.Location = new System.Drawing.Point(892, 96);
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
            this.bookFilterTextBox.Location = new System.Drawing.Point(757, 70);
            this.bookFilterTextBox.Name = "bookFilterTextBox";
            this.bookFilterTextBox.ReadOnly = true;
            this.bookFilterTextBox.Size = new System.Drawing.Size(198, 20);
            this.bookFilterTextBox.TabIndex = 4;
            // 
            // filterTextBox
            // 
            this.filterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.filterTextBox.Location = new System.Drawing.Point(757, 21);
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(198, 20);
            this.filterTextBox.TabIndex = 2;
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // checkFilterLabel
            // 
            this.checkFilterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkFilterLabel.AutoSize = true;
            this.checkFilterLabel.Location = new System.Drawing.Point(754, 4);
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
            this.checksDataGridView.Size = new System.Drawing.Size(747, 251);
            this.checksDataGridView.TabIndex = 0;
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
            // CheckResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 735);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "CheckResultsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Check Results";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.checksDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
    }
}