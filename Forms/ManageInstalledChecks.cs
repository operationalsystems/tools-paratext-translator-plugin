using System;
using System.Data;
using System.Linq;
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
            var availableChecks = from check in CheckManager.GetAvailableCheckAndFixItems()
                                  select new
                                  {
                                      check.Name,
                                      check.Version
                                  };
            var installedChecks = from check in CheckManager.GetInstalledCheckAndFixItems()
                                  select new
                                  {
                                      check.Name,
                                      check.Version
                                  };

            var outdatedChecks = from checkKVP in CheckManager.GetOutdatedCheckAndFixItems()
                                 select new
                                 {
                                     checkKVP.Key.Name,
                                     InstalledVersion = checkKVP.Key.Version,
                                     AvailableVersion = checkKVP.Value.Version
                                 };

            availableChecksGrid.DataSource = availableChecks.ToList();
            installedChecksGrid.DataSource = installedChecks.ToList();
            outdatedChecksGrid.DataSource = outdatedChecks.ToList();
        }

        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;

            switch (tabControl.SelectedTab.Name)
            {
                case "Available":
                    {
                        actionButton.Text = "Install";
                        break;
                    }
                case "Updates":
                    {
                        actionButton.Text = "Update";
                        break;
                    }
                case "Installed":
                    {
                        actionButton.Text = "Uninstall";
                        break;
                    }
            }
        }
    }
}
