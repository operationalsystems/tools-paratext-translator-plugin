using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TvpMain.Data;
using TvpMain.Util;
using static System.Environment;

namespace TvpMain.Form
{
    public partial class IgnoreListForm : System.Windows.Forms.Form
    {
        public IList<IgnoreListItem> IgnoreListItems { get; set; }

        public IgnoreListForm()
        {
            InitializeComponent();
            Shown += OnFormShown;
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            UpdateTableFromItems();
        }

        private void OnClickClose(object sender, EventArgs e)
        {
            UpdateItemsFromTable();
            Close();
        }

        private void OnClickCancel(object sender, EventArgs e)
        {
            Close();
        }

        private void OnClickImport(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open CSV file...";
            openFile.InitialDirectory = Environment.GetFolderPath(SpecialFolder.MyDocuments);
            openFile.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using Stream inputStream = openFile.OpenFile();
                    using StreamReader inputReader = new StreamReader(inputStream);

                    dgvIgnoreList.AllowUserToAddRows = false;
                    try
                    {
                        while (!inputReader.EndOfStream)
                        {
                            string nextLine = inputReader.ReadLine();
                            string[] lineParts = nextLine.Split(',');

                            if (lineParts.Length == 1)
                            {
                                dgvIgnoreList.Rows.Add(new object[] { lineParts[0].Trim() });
                            }
                            else
                            {
                                string lastPart = lineParts[lineParts.Length - 1].Trim();
                                string otherParts = String.Join(",", lineParts.Take(lineParts.Length - 1));

                                dgvIgnoreList.Rows.Add(new object[] { otherParts.Trim(),
                                    lastPart.ToLower().Equals("true") || lastPart.ToLower().Equals("yes")});
                            }
                        }
                    }
                    finally
                    {
                        dgvIgnoreList.AllowUserToAddRows = true;
                    }
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError($"Can't read CSV file: {openFile.FileName}", ex);
                }
            }
        }

        private void OnClickClear(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the list?",
                "Notice...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dgvIgnoreList.Rows.Clear();
            }
        }

        private void UpdateTableFromItems()
        {
            dgvIgnoreList.AllowUserToAddRows = false;
            try
            {
                dgvIgnoreList.Rows.Clear();

                foreach (IgnoreListItem listItem in IgnoreListItems)
                {
                    dgvIgnoreList.Rows.Add(new object[] { listItem.CaseSensitiveItemText, listItem.IsIgnoreCase });
                }
            }
            finally
            {
                dgvIgnoreList.AllowUserToAddRows = true;
            }
        }

        private void UpdateItemsFromTable()
        {
            dgvIgnoreList.AllowUserToAddRows = false;
            try
            {
                IgnoreListItems.Clear();

                foreach (DataGridViewRow rowItem in dgvIgnoreList.Rows)
                {
                    IgnoreListItems.Add(new IgnoreListItem(rowItem.Cells[0].Value.ToString(),
                                                  rowItem.Cells[1].Value == null ? false : (bool)rowItem.Cells[1].Value));
                }
            }
            finally
            {
                dgvIgnoreList.AllowUserToAddRows = true;
            }
        }
    }
}
