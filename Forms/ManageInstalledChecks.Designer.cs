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
            this.actionButton = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Available = new System.Windows.Forms.TabPage();
            this.availableChecksGrid = new System.Windows.Forms.DataGridView();
            this.Updates = new System.Windows.Forms.TabPage();
            this.outdatedChecksGrid = new System.Windows.Forms.DataGridView();
            this.OutdatedCheckName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutdatedCheckInstalledVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutdatedCheckAvailableVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Installed = new System.Windows.Forms.TabPage();
            this.installedChecksGrid = new System.Windows.Forms.DataGridView();
            this.InstalledCheckName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InstalledCheckVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.search = new System.Windows.Forms.Button();
            this.checkDescription = new System.Windows.Forms.TextBox();
            this.copyrightText = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.AvailableCheckName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvailableCheckVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            // actionButton
            // 
            this.actionButton.Location = new System.Drawing.Point(699, 58);
            this.actionButton.Name = "actionButton";
            this.actionButton.Size = new System.Drawing.Size(75, 23);
            this.actionButton.TabIndex = 1;
            this.actionButton.Text = "Install";
            this.actionButton.UseVisualStyleBackColor = true;
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
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            this.tabControl.TabIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
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
            this.availableChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableChecksGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.availableChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.availableChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.availableChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AvailableCheckName,
            this.AvailableCheckVersion});
            this.availableChecksGrid.Enabled = false;
            this.availableChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.availableChecksGrid.MultiSelect = false;
            this.availableChecksGrid.Name = "availableChecksGrid";
            this.availableChecksGrid.ReadOnly = true;
            this.availableChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.availableChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.availableChecksGrid.TabIndex = 0;
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
            this.outdatedChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outdatedChecksGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.outdatedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.outdatedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outdatedChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OutdatedCheckName,
            this.OutdatedCheckInstalledVersion,
            this.OutdatedCheckAvailableVersion});
            this.outdatedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.outdatedChecksGrid.MultiSelect = false;
            this.outdatedChecksGrid.Name = "outdatedChecksGrid";
            this.outdatedChecksGrid.ReadOnly = true;
            this.outdatedChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.outdatedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.outdatedChecksGrid.TabIndex = 1;
            // 
            // OutdatedCheckName
            // 
            this.OutdatedCheckName.DataPropertyName = "Name";
            this.OutdatedCheckName.HeaderText = "Name";
            this.OutdatedCheckName.Name = "OutdatedCheckName";
            this.OutdatedCheckName.ReadOnly = true;
            // 
            // OutdatedCheckInstalledVersion
            // 
            this.OutdatedCheckInstalledVersion.DataPropertyName = "InstalledVersion";
            this.OutdatedCheckInstalledVersion.HeaderText = "Installed Version";
            this.OutdatedCheckInstalledVersion.Name = "OutdatedCheckInstalledVersion";
            this.OutdatedCheckInstalledVersion.ReadOnly = true;
            // 
            // OutdatedCheckAvailableVersion
            // 
            this.OutdatedCheckAvailableVersion.DataPropertyName = "AvailableVersion";
            this.OutdatedCheckAvailableVersion.HeaderText = "AvailableVersion";
            this.OutdatedCheckAvailableVersion.Name = "OutdatedCheckAvailableVersion";
            this.OutdatedCheckAvailableVersion.ReadOnly = true;
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
            this.installedChecksGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.installedChecksGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.installedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.installedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.installedChecksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.InstalledCheckName,
            this.InstalledCheckVersion});
            this.installedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.installedChecksGrid.MultiSelect = false;
            this.installedChecksGrid.Name = "installedChecksGrid";
            this.installedChecksGrid.ReadOnly = true;
            this.installedChecksGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.installedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.installedChecksGrid.TabIndex = 2;
            // 
            // InstalledCheckName
            // 
            this.InstalledCheckName.DataPropertyName = "Name";
            this.InstalledCheckName.HeaderText = "Name";
            this.InstalledCheckName.Name = "InstalledCheckName";
            this.InstalledCheckName.ReadOnly = true;
            // 
            // InstalledCheckVersion
            // 
            this.InstalledCheckVersion.DataPropertyName = "Version";
            this.InstalledCheckVersion.HeaderText = "Version";
            this.InstalledCheckVersion.Name = "InstalledCheckVersion";
            this.InstalledCheckVersion.ReadOnly = true;
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
            // AvailableCheckName
            // 
            this.AvailableCheckName.DataPropertyName = "Name";
            this.AvailableCheckName.HeaderText = "Name";
            this.AvailableCheckName.Name = "AvailableCheckName";
            this.AvailableCheckName.ReadOnly = true;
            // 
            // AvailableCheckVersion
            // 
            this.AvailableCheckVersion.DataPropertyName = "Version";
            this.AvailableCheckVersion.HeaderText = "Version";
            this.AvailableCheckVersion.Name = "AvailableCheckVersion";
            this.AvailableCheckVersion.ReadOnly = true;
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
            this.Controls.Add(this.actionButton);
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
        private System.Windows.Forms.Button actionButton;
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
        private System.Windows.Forms.TabPage Installed;
        private System.Windows.Forms.DataGridView outdatedChecksGrid;
        private System.Windows.Forms.DataGridView installedChecksGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutdatedCheckName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutdatedCheckInstalledVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutdatedCheckAvailableVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn InstalledCheckName;
        private System.Windows.Forms.DataGridViewTextBoxColumn InstalledCheckVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailableCheckName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvailableCheckVersion;
    }
}