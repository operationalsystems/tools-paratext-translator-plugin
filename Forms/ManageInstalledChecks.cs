using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TvpMain.CheckManagement;

namespace TvpMain.Forms
{
    public partial class ManageInstalledChecks : Form
    {
        /// <summary>
        /// The controller that will handle check operations.
        /// </summary>
        public CheckManager CheckManager { get; private set; } = new CheckManager();

        public ManageInstalledChecks()
        {
            InitializeComponent();
            RefreshBindings();
        }

        private void RefreshBindings()
        {
            availableChecksGrid.DataSource = CheckManager.GetAvailableCheckAndFixItems();
            installedChecksGrid.DataSource = CheckManager.GetInstalledCheckAndFixItems();

            var outdatedChecks = from checkKVP in CheckManager.GetOutdatedCheckAndFixItems()
                                 select new
                                 {
                                     checkKVP.Key.Name,
                                     InstalledVersion = checkKVP.Key.Version,
                                     AvailableVersion = checkKVP.Value.Version
                                 };

            outdatedChecksGrid.DataSource = outdatedChecks.ToList();
        }
    }
}
