/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace TvpMain.Forms
{
    partial class BookSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BookSelection));
            this.bookList = new System.Windows.Forms.ListBox();
            this.allBooksButton = new System.Windows.Forms.Button();
            this.otButton = new System.Windows.Forms.Button();
            this.ntButton = new System.Windows.Forms.Button();
            this.deutButton = new System.Windows.Forms.Button();
            this.extraButton = new System.Windows.Forms.Button();
            this.deselectButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bookList
            // 
            this.bookList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bookList.FormattingEnabled = true;
            this.bookList.Location = new System.Drawing.Point(121, 12);
            this.bookList.MultiColumn = true;
            this.bookList.Name = "bookList";
            this.bookList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.bookList.Size = new System.Drawing.Size(488, 264);
            this.bookList.TabIndex = 1;
            // 
            // allBooksButton
            // 
            this.allBooksButton.Location = new System.Drawing.Point(12, 12);
            this.allBooksButton.Name = "allBooksButton";
            this.allBooksButton.Size = new System.Drawing.Size(103, 23);
            this.allBooksButton.TabIndex = 2;
            this.allBooksButton.Text = "All Books";
            this.allBooksButton.UseVisualStyleBackColor = true;
            this.allBooksButton.Click += new System.EventHandler(this.allBooksButton_Click);
            // 
            // otButton
            // 
            this.otButton.Location = new System.Drawing.Point(13, 42);
            this.otButton.Name = "otButton";
            this.otButton.Size = new System.Drawing.Size(102, 23);
            this.otButton.TabIndex = 3;
            this.otButton.Text = "Old Testament";
            this.otButton.UseVisualStyleBackColor = true;
            this.otButton.Click += new System.EventHandler(this.otButton_Click);
            // 
            // ntButton
            // 
            this.ntButton.Location = new System.Drawing.Point(13, 72);
            this.ntButton.Name = "ntButton";
            this.ntButton.Size = new System.Drawing.Size(102, 23);
            this.ntButton.TabIndex = 4;
            this.ntButton.Text = "New Testament";
            this.ntButton.UseVisualStyleBackColor = true;
            this.ntButton.Click += new System.EventHandler(this.ntButton_Click);
            // 
            // deutButton
            // 
            this.deutButton.Enabled = false;
            this.deutButton.Location = new System.Drawing.Point(13, 102);
            this.deutButton.Name = "deutButton";
            this.deutButton.Size = new System.Drawing.Size(102, 23);
            this.deutButton.TabIndex = 5;
            this.deutButton.Text = "Deuterocanon";
            this.deutButton.UseVisualStyleBackColor = true;
            // 
            // extraButton
            // 
            this.extraButton.Location = new System.Drawing.Point(13, 132);
            this.extraButton.Name = "extraButton";
            this.extraButton.Size = new System.Drawing.Size(102, 23);
            this.extraButton.TabIndex = 6;
            this.extraButton.Text = "Extra Material";
            this.extraButton.UseVisualStyleBackColor = true;
            this.extraButton.Click += new System.EventHandler(this.extraButton_Click);
            // 
            // deselectButton
            // 
            this.deselectButton.Location = new System.Drawing.Point(13, 193);
            this.deselectButton.Name = "deselectButton";
            this.deselectButton.Size = new System.Drawing.Size(102, 23);
            this.deselectButton.TabIndex = 7;
            this.deselectButton.Text = "Deselect All";
            this.deselectButton.UseVisualStyleBackColor = true;
            this.deselectButton.Click += new System.EventHandler(this.deselectButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.textBox1.Location = new System.Drawing.Point(12, 297);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(597, 123);
            this.textBox1.TabIndex = 8;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(453, 426);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(534, 426);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // BookSelection
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(621, 461);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.deselectButton);
            this.Controls.Add(this.extraButton);
            this.Controls.Add(this.deutButton);
            this.Controls.Add(this.ntButton);
            this.Controls.Add(this.otButton);
            this.Controls.Add(this.allBooksButton);
            this.Controls.Add(this.bookList);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BookSelection";
            this.ShowIcon = false;
            this.Text = "Books to Check";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox bookList;
        private System.Windows.Forms.Button allBooksButton;
        private System.Windows.Forms.Button otButton;
        private System.Windows.Forms.Button ntButton;
        private System.Windows.Forms.Button deutButton;
        private System.Windows.Forms.Button extraButton;
        private System.Windows.Forms.Button deselectButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}