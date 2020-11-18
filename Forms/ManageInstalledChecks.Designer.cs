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
            this.action = new System.Windows.Forms.Button();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.Available = new System.Windows.Forms.TabPage();
            this.Updates = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.search = new System.Windows.Forms.Button();
            this.checkDescription = new System.Windows.Forms.TextBox();
            this.copyrightText = new System.Windows.Forms.Label();
            this.progressLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.availableChecksGrid = new System.Windows.Forms.DataGridView();
            this.Installed = new System.Windows.Forms.TabPage();
            this.outdatedChecksGrid = new System.Windows.Forms.DataGridView();
            this.installedChecksGrid = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.Available.SuspendLayout();
            this.Updates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableChecksGrid)).BeginInit();
            this.Installed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outdatedChecksGrid)).BeginInit();
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
            // action
            // 
            this.action.Location = new System.Drawing.Point(699, 58);
            this.action.Name = "action";
            this.action.Size = new System.Drawing.Size(75, 23);
            this.action.TabIndex = 1;
            this.action.Text = "Action";
            this.action.UseVisualStyleBackColor = true;
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
            this.licenseToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.licenseToolStripMenuItem.Text = "License";
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
            // availableChecksGrid
            // 
            this.availableChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.availableChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.availableChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.availableChecksGrid.Name = "availableChecksGrid";
            this.availableChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.availableChecksGrid.TabIndex = 0;
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
            // outdatedChecksGrid
            // 
            this.outdatedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.outdatedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.outdatedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.outdatedChecksGrid.Name = "outdatedChecksGrid";
            this.outdatedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.outdatedChecksGrid.TabIndex = 1;
            // 
            // installedChecksGrid
            // 
            this.installedChecksGrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.installedChecksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.installedChecksGrid.Location = new System.Drawing.Point(0, 38);
            this.installedChecksGrid.Name = "installedChecksGrid";
            this.installedChecksGrid.Size = new System.Drawing.Size(767, 278);
            this.installedChecksGrid.TabIndex = 1;
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
            this.Text = "ManageInstalledChecks";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.Available.ResumeLayout(false);
            this.Updates.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.availableChecksGrid)).EndInit();
            this.Installed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outdatedChecksGrid)).EndInit();
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
        private System.Windows.Forms.TabPage Installed;
        private System.Windows.Forms.DataGridView outdatedChecksGrid;
        private System.Windows.Forms.DataGridView installedChecksGrid;
    }
}