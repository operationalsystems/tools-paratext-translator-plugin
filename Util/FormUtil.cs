using TvpMain.Forms;
using TvpMain.Properties;

namespace TvpMain.Util
{
    /// <summary>
    /// This class is used for Form utility functions.
    /// </summary>
    public static class FormUtil
    {
        /// <summary>
        /// This function will set up and activate the License form.
        /// </summary>
        public static void StartLicenseForm()
        {
            string pluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string formTitle = $"{pluginName} - End User License Agreement";

            LicenseForm eulaForm = new LicenseForm
            {
                FormType = LicenseForm.FormTypes.Info,
                FormTitle = formTitle,
                LicenseText = Resources.TVP_EULA
            };
            eulaForm.OnDismiss = () => eulaForm.Close();
            eulaForm.Show();
        }
    }
}
