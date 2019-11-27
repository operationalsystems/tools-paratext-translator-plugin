using CsvHelper;
using CsvHelper.Configuration;
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
    /// <summary>
    /// Manages ignore list for filtering.
    /// </summary>
    public partial class IgnoreListForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Working ignore list.
        /// </summary>
        public IList<IgnoreListItem> IgnoreList { get; set; }

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public IgnoreListForm()
        {
            InitializeComponent();
            Shown += OnFormShown;
        }

        /// <summary>
        /// Form shown handler that populates table from items.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnFormShown(object sender, EventArgs e)
        {
            UpdateTableFromItems();
        }

        /// <summary>
        /// Close handler that updates the ignore list from the table.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnClickClose(object sender, EventArgs e)
        {
            UpdateItemsFromTable();
            Close();
        }

        /// <summary>
        /// Cancel handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnClickCancel(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Import button handler that imports ignore list items from CSV file.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnClickImport(object sender, EventArgs e)
        {
            using OpenFileDialog openFile = new OpenFileDialog();

            openFile.Title = "Open CSV file...";
            openFile.InitialDirectory = Environment.GetFolderPath(SpecialFolder.MyDocuments);
            openFile.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using Stream inputStream = openFile.OpenFile();
                    using StreamReader streamReader = new StreamReader(inputStream);
                    using CsvReader csvReader = new CsvReader(streamReader);

                    csvReader.Configuration.HasHeaderRecord = false;
                    csvReader.Configuration.IgnoreBlankLines = true;
                    csvReader.Configuration.TrimOptions = TrimOptions.Trim;
                    csvReader.Configuration.MissingFieldFound = null;

                    dgvIgnoreList.AllowUserToAddRows = false;
                    try
                    {
                        foreach (IgnoreListItem listItem in csvReader.GetRecords<IgnoreListItem>())
                        {
                            IgnoreList.Add(listItem);
                        }
                        UpdateTableFromItems();
                    }
                    finally
                    {
                        dgvIgnoreList.AllowUserToAddRows = true;
                    }
                }
                catch (Exception ex)
                {
                    HostUtil.Instance.ReportError($"Can't read CSV file: {openFile.FileName}", false, ex);
                }
            }
        }

        /// <summary>
        /// Clear button handler.
        /// </summary>
        /// <param name="sender">Event sender (ignored).</param>
        /// <param name="e">Event args (ignored).</param>
        private void OnClickClear(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the list?",
                "Notice...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                dgvIgnoreList.Rows.Clear();
            }
        }

        /// <summary>
        /// Updates table from current ignore list items.
        /// </summary>
        private void UpdateTableFromItems()
        {
            dgvIgnoreList.AllowUserToAddRows = false;
            try
            {
                dgvIgnoreList.Rows.Clear();
                foreach (IgnoreListItem listItem in IgnoreList)
                {
                    dgvIgnoreList.Rows.Add(new object[] { listItem.CaseSensitiveText, listItem.IsIgnoreCase });
                    dgvIgnoreList.Rows[(dgvIgnoreList.Rows.Count - 1)].HeaderCell.Value =
                        $"{dgvIgnoreList.Rows.Count:N0}";
                }
            }
            finally
            {
                dgvIgnoreList.AllowUserToAddRows = true;
            }
        }

        /// <summary>
        /// Updates ignore list items from current table.
        /// </summary>
        private void UpdateItemsFromTable()
        {
            dgvIgnoreList.AllowUserToAddRows = false;
            try
            {
                IgnoreList.Clear();
                foreach (DataGridViewRow rowItem in dgvIgnoreList.Rows)
                {
                    IgnoreList.Add(new IgnoreListItem(rowItem.Cells[0].Value.ToString(),
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
