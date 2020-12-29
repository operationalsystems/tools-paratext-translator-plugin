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
            this.Cancel = new System.Windows.Forms.Button();
            this.Deny = new System.Windows.Forms.Button();
            this.ShowDenied = new System.Windows.Forms.CheckBox();
            this.Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Location = new System.Drawing.Point(1179, 599);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Deny
            // 
            this.Deny.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.Deny.Location = new System.Drawing.Point(1098, 496);
            this.Deny.Name = "Deny";
            this.Deny.Size = new System.Drawing.Size(156, 50);
            this.Deny.TabIndex = 1;
            this.Deny.Text = "Deny";
            this.Deny.UseVisualStyleBackColor = true;
            this.Deny.Click += new System.EventHandler(this.Deny_Click);
            // 
            // ShowDenied
            // 
            this.ShowDenied.AutoSize = true;
            this.ShowDenied.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ShowDenied.Location = new System.Drawing.Point(1123, 552);
            this.ShowDenied.Name = "ShowDenied";
            this.ShowDenied.Size = new System.Drawing.Size(110, 21);
            this.ShowDenied.TabIndex = 2;
            this.ShowDenied.Text = "Show Denied";
            this.ShowDenied.UseVisualStyleBackColor = true;
            this.ShowDenied.CheckedChanged += new System.EventHandler(this.ShowDenied_CheckedChanged);
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(1098, 599);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 3;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // CheckResultsForm
            // 
            this.AcceptButton = this.Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(1266, 634);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.ShowDenied);
            this.Controls.Add(this.Deny);
            this.Controls.Add(this.Cancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CheckResultsForm";
            this.Text = "Check Results";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Deny;
        private System.Windows.Forms.CheckBox ShowDenied;
        private System.Windows.Forms.Button Save;
    }
}