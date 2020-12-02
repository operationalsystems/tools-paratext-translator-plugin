using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TvpMain.Check;
using TvpMain.CheckManagement;

namespace TvpMain.Forms
{
    public partial class CheckManagerTestInterface : Form
    {
        readonly ICheckManager checkManager = new CheckManager();
        private List<CheckAndFixItem> installed = new List<CheckAndFixItem>();
        private List<CheckAndFixItem> saved = new List<CheckAndFixItem>();
        private string _activeProjectName { get; set; }
        public CheckManagerTestInterface(string projectName)
        {
            InitializeComponent();
            _activeProjectName = projectName;
            refresh();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CheckAndFixItem check = new CheckAndFixItem
            {
                Name = CheckName.Text,
                Version = Version.Text,
                Description = Description.Text
            };

            checkManager.SaveCheckAndFixItem(check);
            refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in savedChecks.SelectedRows)
                checkManager.PublishCheckAndFixItem((CheckAndFixItem)row.DataBoundItem);

            refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in savedChecks.SelectedRows)
                checkManager.DeleteCheckAndFixItem((CheckAndFixItem)row.DataBoundItem);

            refresh();
        }

        private void refresh()
        {
            installed = checkManager.GetInstalledCheckAndFixItems();
            installedChecks.DataSource = installed;
            saved = checkManager.GetSavedCheckAndFixItems();
            savedChecks.DataSource = saved;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkManager.SynchronizeInstalledChecks();
            refresh();
        }

        private void projectSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form settingsForm = new ProjectCheckSettingsTestForm(_activeProjectName);
            settingsForm.ShowDialog();
        }
    }
}
