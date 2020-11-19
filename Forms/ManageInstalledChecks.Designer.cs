namespace TvpMain.Forms
{
    partial class ManageInstalledChecks
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageInstalledChecks));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.action = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Available = new System.Windows.Forms.TabPage();
            this.availableChecksGrid = new System.Windows.Forms.DataGridView();
            this.availableCheckNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.availableCheckVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updates = new System.Windows.Forms.TabPage();
            this.outdatedChecksGrid = new System.Windows.Forms.DataGridView();
            this.outdatedCheckNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Installed = new System.Windows.Forms.TabPage();
            this.installedChecksGrid = new System.Windows.Forms.DataGridView();
            this.installedCheckNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.installedCheckVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.search = new System.Windows.Forms.Button();
            this.checkDescription = new System.Windows.Forms.TextBox();
            this.copyrightText = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.Available.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableChecksGrid)).BeginInit();
            this.Updates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outdatedChecksGrid)).BeginInit();
            this.Installed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.installedChecksGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.licenseToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // licenseToolStripMenuItem
            // 
            this.licenseToolStripMenuItem.Name = "licenseToolStripMenuItem";
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.licenseToolStripMenuItem.Text = "License";
            // 
            // action
            // 
            this.action.Location = new System.Drawing.Point(699, 58);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(75, 23);
            this.action.TabIndex = 1;
            this.action.Text = "Action";
            this.action.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.Available);
            this.tabControl.Controls.Add(this.Updates);
            this.tabControl.Controls.Add(this.Installed);
            this.tabControl.Location = new System.Drawing.Point(13, 27);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(775, 342);
            this.tabControl.TabIndex = 3;
            // 
            // Available
            // 
            this.Available.Controls.Add(this.availableChecksGrid);
            this.Available.Location = new System.Drawing.Point(4, 22);
            this.Available.Name = "Available";
            this.Available.Padding = new System.Windows.Forms.Padding(3);
            this.Available.Size = new System.Drawing.Size(767, 316);
            this.Available.TabIndex = 0;
            this.Available.Text = "Available";
            this.Available.UseVisualStyleBackColor = true;
            // 
            // availableChecksGrid
            // 
            this.availableChecksGrid.AllowUserToAddRows = false;
            this.availableChecksGrid.AllowUserToDeleteRows = false;
            this.availableChecksGrid.AllowUserToResizeColumns = false;
            this.availableChecksGrid.AllowUserToResizeRows = false;
            this.availableChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.availableChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableChecksGrid.AutoGenerateColumns = false;
            this.availableChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.availableChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.availableCheckNameDataGridViewTextBoxColumn,
            this.availableCheckVersionDataGridViewTextBoxColumn});
            this.availableChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.availableChecksGrid.Name = "availableChecksGrid";
            this.availableChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.availableChecksGrid.TabIndex = 0;
            this.availableChecksGrid.MultiSelect = false;
            this.availableChecksGrid.ReadOnly = true;
            this.availableChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // availableCheckNameDataGridViewTextBoxColumn
            // 
            this.availableCheckNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.availableCheckNameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.availableCheckNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.availableCheckNameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.availableCheckNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // availableCheckVersionDataGridViewTextBoxColumn
            // 
            this.availableCheckVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.availableCheckVersionDataGridViewTextBoxColumn.DataPropertyName = "Version";
            this.availableCheckVersionDataGridViewTextBoxColumn.HeaderText = "Version";
            this.availableCheckVersionDataGridViewTextBoxColumn.Name = "versionDataGridViewTextBoxColumn";
            this.availableCheckVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Updates
            // 
            this.Updates.Controls.Add(this.outdatedChecksGrid);
            this.Updates.Location = new System.Drawing.Point(4, 22);
            this.Updates.Name = "Updates";
            this.Updates.Padding = new System.Windows.Forms.Padding(3);
            this.Updates.Size = new System.Drawing.Size(767, 316);
            this.Updates.TabIndex = 1;
            this.Updates.Text = "Updates";
            this.Updates.UseVisualStyleBackColor = true;
            // 
            // outdatedChecksGrid
            // 
            this.outdatedChecksGrid.AllowUserToAddRows = false;
            this.outdatedChecksGrid.AllowUserToDeleteRows = false;
            this.outdatedChecksGrid.AllowUserToResizeColumns = false;
            this.outdatedChecksGrid.AllowUserToResizeRows = false;
            this.outdatedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.outdatedChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.outdatedChecksGrid.AutoGenerateColumns = false;
            this.outdatedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outdatedChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.outdatedCheckNameDataGridViewTextBoxColumn,
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn,
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn});
            this.outdatedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.outdatedChecksGrid.Name = "outdatedChecksGrid";
            this.outdatedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.outdatedChecksGrid.TabIndex = 1;
            this.outdatedChecksGrid.MultiSelect = false;
            this.outdatedChecksGrid.ReadOnly = true;
            this.outdatedChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // outdatedCheckNameDataGridViewTextBoxColumn
            // 
            this.outdatedCheckNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.outdatedCheckNameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.outdatedCheckNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.outdatedCheckNameDataGridViewTextBoxColumn.Name = "outdatedCheckNameDataGridViewTextBoxColumn";
            this.outdatedCheckNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // outdatedCheckInstalledVersionDataGridViewTextBoxColumn
            // 
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn.DataPropertyName = "InstalledVersion";
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn.HeaderText = "Installed Version";
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn.Name = "outdatedCheckInstalledVersionDataGridViewTextBoxColumn";
            this.outdatedCheckInstalledVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // outdatedCheckAvailableVersionDataGridViewTextBoxColumn
            // 
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn.DataPropertyName = "AvailableVersion";
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn.HeaderText = "Available Version";
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn.Name = "outdatedCheckAvailableVersionDataGridViewTextBoxColumn";
            this.outdatedCheckAvailableVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Installed
            // 
            this.Installed.Controls.Add(this.installedChecksGrid);
            this.Installed.Location = new System.Drawing.Point(4, 22);
            this.Installed.Name = "Installed";
            this.Installed.Size = new System.Drawing.Size(767, 316);
            this.Installed.TabIndex = 2;
            this.Installed.Text = "Installed";
            this.Installed.UseVisualStyleBackColor = true;
            // 
            // installedChecksGrid
            // 
            this.installedChecksGrid.AllowUserToAddRows = false;
            this.installedChecksGrid.AllowUserToDeleteRows = false;
            this.installedChecksGrid.AllowUserToResizeColumns = false;
            this.installedChecksGrid.AllowUserToResizeRows = false;
            this.installedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.installedChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.installedChecksGrid.AutoGenerateColumns = false;
            this.installedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.installedChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.installedCheckNameDataGridViewTextBoxColumn,
            this.installedCheckVersionDataGridViewTextBoxColumn});
            this.installedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.installedChecksGrid.Name = "installedChecksGrid";
            this.installedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.installedChecksGrid.TabIndex = 2;
            this.installedChecksGrid.MultiSelect = false;
            this.installedChecksGrid.ReadOnly = true;
            this.installedChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // installedCheckNameDataGridViewTextBoxColumn
            // 
            this.installedCheckNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.installedCheckNameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.installedCheckNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.installedCheckNameDataGridViewTextBoxColumn.Name = "installedCheckNameDataGridViewTextBoxColumn";
            this.installedCheckNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // installedCheckVersionDataGridViewTextBoxColumn
            // 
            this.installedCheckVersionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.installedCheckVersionDataGridViewTextBoxColumn.DataPropertyName = "Version";
            this.installedCheckVersionDataGridViewTextBoxColumn.HeaderText = "Version";
            this.installedCheckVersionDataGridViewTextBoxColumn.Name = "installedCheckVersionDataGridViewTextBoxColumn";
            this.installedCheckVersionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(28, 58);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 4;
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(135, 58);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(75, 20);
            this.search.TabIndex = 5;
            this.search.Text = "Search";
            this.search.UseVisualStyleBackColor = true;
            // 
            // checkDescription
            // 
            this.checkDescription.Location = new System.Drawing.Point(13, 371);
            this.checkDescription.Multiline = true;
            this.checkDescription.Name = "checkDescription";
            this.checkDescription.Size = new System.Drawing.Size(771, 101);
            this.checkDescription.TabIndex = 6;
            // 
            // copyrightText
            // 
            this.copyrightText.AutoSize = true;
            this.copyrightText.Location = new System.Drawing.Point(13, 479);
            this.copyrightText.Name = "copyrightText";
            this.copyrightText.Size = new System.Drawing.Size(51, 13);
            this.copyrightText.TabIndex = 7;
            this.copyrightText.Text = "Copyright";
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.Location = new System.Drawing.Point(613, 478);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(51, 13);
            this.progressLabel.TabIndex = 8;
            this.progressLabel.Text = "Progress:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(670, 478);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(114, 14);
            this.progressBar.TabIndex = 9;
            // 
            // ManageInstalledChecks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 504);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.copyrightText);
            this.Controls.Add(this.checkDescription);
            this.Controls.Add(this.search);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.action);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ManageInstalledChecks";
            this.Text = "Manage Installed Checks";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.Available.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.availableChecksGrid)).EndInit();
            this.Updates.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outdatedChecksGrid)).EndInit();
            this.Installed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.installedChecksGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenseToolStripMenuItem;
        private System.Windows.Forms.Button action;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage Available;
        private System.Windows.Forms.TabPage Updates;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button search;
        private System.Windows.Forms.TextBox checkDescription;
        private System.Windows.Forms.Label copyrightText;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.DataGridView availableChecksGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn availableCheckNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn availableCheckVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.TabPage Installed;
        private System.Windows.Forms.DataGridView outdatedChecksGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn outdatedCheckNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn outdatedCheckInstalledVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn outdatedCheckAvailableVersionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridView installedChecksGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn installedCheckNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn installedCheckVersionDataGridViewTextBoxColumn;
    }
}