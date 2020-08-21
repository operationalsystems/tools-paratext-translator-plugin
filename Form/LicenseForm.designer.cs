namespace TvpMain.Form
{
    partial class LicenseForm
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
            this.LicenseTextBox = new System.Windows.Forms.RichTextBox();
            this.DismissLicenseButton = new System.Windows.Forms.Button();
            this.AcceptLicenseButton = new System.Windows.Forms.Button();
            this.LicensePrompt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LicenseTextBox
            // 
            this.LicenseTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LicenseTextBox.Location = new System.Drawing.Point(12, 47);
            this.LicenseTextBox.Name = "LicenseTextBox";
            this.LicenseTextBox.ReadOnly = true;
            this.LicenseTextBox.Size = new System.Drawing.Size(574, 520);
            this.LicenseTextBox.TabIndex = 0;
            this.LicenseTextBox.Text = "";
            // 
            // DismissLicenseButton
            // 
            this.DismissLicenseButton.Location = new System.Drawing.Point(511, 580);
            this.DismissLicenseButton.Name = "DismissLicenseButton";
            this.DismissLicenseButton.Size = new System.Drawing.Size(75, 23);
            this.DismissLicenseButton.TabIndex = 1;
            this.DismissLicenseButton.Text = "Cancel";
            this.DismissLicenseButton.UseVisualStyleBackColor = true;
            this.DismissLicenseButton.Click += new System.EventHandler(this.Dismiss_Click);
            // 
            // AcceptLicenseButton
            // 
            this.AcceptLicenseButton.Location = new System.Drawing.Point(430, 580);
            this.AcceptLicenseButton.Name = "AcceptLicenseButton";
            this.AcceptLicenseButton.Size = new System.Drawing.Size(75, 23);
            this.AcceptLicenseButton.TabIndex = 2;
            this.AcceptLicenseButton.Text = "I Agree";
            this.AcceptLicenseButton.UseVisualStyleBackColor = true;
            this.AcceptLicenseButton.Click += new System.EventHandler(this.Accept_Click);
            // 
            // LicensePrompt
            // 
            this.LicensePrompt.AutoSize = true;
            this.LicensePrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.LicensePrompt.Location = new System.Drawing.Point(12, 13);
            this.LicensePrompt.Name = "LicensePrompt";
            this.LicensePrompt.Size = new System.Drawing.Size(350, 18);
            this.LicensePrompt.TabIndex = 4;
            this.LicensePrompt.Text = "Press Page Down to read the rest of the agreement.";
            // 
            // LicenseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 615);
            this.Controls.Add(this.LicensePrompt);
            this.Controls.Add(this.AcceptLicenseButton);
            this.Controls.Add(this.DismissLicenseButton);
            this.Controls.Add(this.LicenseTextBox);
            this.Name = "LicenseForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "End User License Agreement";
            this.Load += new System.EventHandler(this.LicenseForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox LicenseTextBox;
        private System.Windows.Forms.Button DismissLicenseButton;
        private System.Windows.Forms.Button AcceptLicenseButton;
        private System.Windows.Forms.Label LicensePrompt;
    }
}