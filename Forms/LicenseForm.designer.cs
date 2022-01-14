/*
Copyright © 2021 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace TvpMain.Forms
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
            this.Copyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LicenseTextBox
            // 
            this.LicenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.DismissLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            this.AcceptLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
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
            // Copyright
            // 
            this.Copyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Copyright.AutoSize = true;
            this.Copyright.Location = new System.Drawing.Point(12, 580);
            this.Copyright.Name = "Copyright";
            this.Copyright.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.Copyright.Size = new System.Drawing.Size(101, 23);
            this.Copyright.TabIndex = 5;
            this.Copyright.Text = "© 2020 Biblica, Inc.";
            // 
            // LicenseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(598, 615);
            this.Controls.Add(this.Copyright);
            this.Controls.Add(this.LicensePrompt);
            this.Controls.Add(this.AcceptLicenseButton);
            this.Controls.Add(this.DismissLicenseButton);
            this.Controls.Add(this.LicenseTextBox);
            this.Name = "LicenseForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
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
        private System.Windows.Forms.Label Copyright;
    }
}