using System;
using System.Windows.Forms;
using TvpMain.Project;

namespace TvpMain.Forms
{
    public partial class ProjectCheckSettingsTestForm : Form
    {
        private string _activeProjectName { get; set; }
        private ProjectCheckSettings _settings { get; set; }
        public ProjectCheckSettingsTestForm(string projectName)
        {
            InitializeComponent();
            _activeProjectName = projectName;
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _settings.DefaultCheckIds.Add(textBox1.Text);
            Util.HostUtil.Instance.PutProjectCheckSettings(_activeProjectName, _settings);
            refresh();
        }

        private void refresh()
        {
            _settings = Util.HostUtil.Instance.GetProjectCheckSettings(_activeProjectName);

            listBox1.Items.Clear();
            foreach (string id in _settings.DefaultCheckIds)
            {
                listBox1.Items.Add((string)id);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _settings.DefaultCheckIds.Remove((string)listBox1.SelectedItem);
            Util.HostUtil.Instance.PutProjectCheckSettings(_activeProjectName, _settings);
            refresh();
        }
    }
}
