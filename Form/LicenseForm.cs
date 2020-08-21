using System;

namespace TvpMain.Form
{
    public partial class LicenseForm : System.Windows.Forms.Form
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
        /// The text displayed above the license.
        /// </summary>
        public string FormDescription { get => LicensePrompt.Text; set => LicensePrompt.Text = value; }

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

        private void Print_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException(nameof(Print_Click));
        }
    }
}
