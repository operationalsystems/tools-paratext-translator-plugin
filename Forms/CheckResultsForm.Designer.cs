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
            this.CancelButton = new System.Windows.Forms.Button();
            this.DenyButton = new System.Windows.Forms.Button();
            this.ShowDeniedCheckbox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(1179, 599);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 0;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // DenyButton
            // 
            this.DenyButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.DenyButton.Location = new System.Drawing.Point(1098, 496);
            this.DenyButton.Name = "DenyButton";
            this.DenyButton.Size = new System.Drawing.Size(156, 50);
            this.DenyButton.TabIndex = 1;
            this.DenyButton.Text = "Deny";
            this.DenyButton.UseVisualStyleBackColor = true;
            this.DenyButton.Click += new System.EventHandler(this.Deny_Click);
            // 
            // ShowDeniedCheckbox
            // 
            this.ShowDeniedCheckbox.AutoSize = true;
            this.ShowDeniedCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.ShowDeniedCheckbox.Location = new System.Drawing.Point(1123, 552);
            this.ShowDeniedCheckbox.Name = "ShowDeniedCheckbox";
            this.ShowDeniedCheckbox.Size = new System.Drawing.Size(110, 21);
            this.ShowDeniedCheckbox.TabIndex = 2;
            this.ShowDeniedCheckbox.Text = "Show Denied";
            this.ShowDeniedCheckbox.UseVisualStyleBackColor = true;
            this.ShowDeniedCheckbox.CheckedChanged += new System.EventHandler(this.ShowDenied_CheckedChanged);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(1098, 599);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 3;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.Save_Click);
            // 
            // CheckResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1266, 634);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.ShowDeniedCheckbox);
            this.Controls.Add(this.DenyButton);
            this.Controls.Add(this.CancelButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CheckResultsForm";
            this.Text = "Check Results";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button DenyButton;
        private System.Windows.Forms.CheckBox ShowDeniedCheckbox;
        private System.Windows.Forms.Button SaveButton;
    }
}