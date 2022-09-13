/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Windows.Forms;
using TvpMain.Util;

namespace TvpMain.Forms
{
    public partial class LicenseForm : Form
    {
        private const string DismissButtonText = "Dismiss";
        private const string CancelButtonText = "Cancel";

        /// <summary>
        /// The types of forms that can be represented by the <c>License Form</c>.
        /// </summary>
        public enum FormTypes
        {
            /// <summary>
            /// A form which prompts the user to accept or cancel.
            /// </summary>
            Prompt,
            /// <summary>
            /// A form which the user can dismiss.
            /// </summary>
            Info
        };

        /// <summary>
        /// The type of form that should be displayed.
        /// </summary>
        public FormTypes FormType
        {
            get => _formType;
            set
            {
                _formType = value;
                if (_formType.Equals(FormTypes.Prompt))
                {
                    AcceptLicenseButton.Visible = true;
                    DismissLicenseButton.Text = CancelButtonText;
                }
                else
                {
                    AcceptLicenseButton.Visible = false;
                    DismissLicenseButton.Text = DismissButtonText;
                }
            }
        }
        private FormTypes _formType = FormTypes.Prompt;

        /// <summary>
        /// A callback function to be invoked when a button is clicked.
        /// </summary>
        public delegate void ClickHandler();

        /// <summary>
        /// This method is invoked when the Accept button is clicked.
        /// </summary>
        public ClickHandler OnAccept { get; set; } = () => { };

        /// <summary>
        /// This method is invoked when the Dismiss/Cancel button is clicked.
        /// </summary>
        public ClickHandler OnDismiss { get; set; } = () => { };

        /// <summary>
        /// The title that is displayed at the top of the form.
        /// </summary>
        public string FormTitle { get => this.Text; set => this.Text = value; }

        /// <summary>
        /// The RTF license text.
        /// </summary>
        public string LicenseText
        {
            get => LicenseTextBox.Rtf;
            set
            {
                LicenseTextBox.Rtf = value;
            }
        }

        public LicenseForm()
        {
            InitializeComponent();
        }

        private void LicenseForm_Load(object sender, EventArgs e)
        {
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            OnAccept();
        }

        private void Dismiss_Click(object sender, EventArgs e)
        {
            OnDismiss();
        }
    }
}
