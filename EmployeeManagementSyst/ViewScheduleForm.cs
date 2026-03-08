using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmployeeManagementSyst
{
    public partial class ViewScheduleForm : Form
    {


        public ViewScheduleForm()
        {
            InitializeComponent();

        }



        /// <summary>
        /// Populates the DataGridView with employee rota data by querying the database.
        /// Retrieves employee names and their respective shift details, such as start time, finish time, and work day.
        /// The data is organized by employee name and displayed in a grid format, where each unique day/date combination 
        /// is a separate column, and the shifts are displayed under the respective day/date columns.
        /// </summary>
        private void PopulateDataGridView(string filter = null)
        {
            try
            {
                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                    // Single query joining schedule and employee details to avoid per-employee roundtrips
                    string query = @"
                        SELECT si.EmployeeId, e.FullName, si.StartWork, si.FinishWork, si.DayOfWeek
                        FROM ScheduleInformation si
                        LEFT JOIN EmployeeDetails e ON si.EmployeeId = e.Id";

                    // Append filter clause if a filter was provided (filter by surname only)
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        query += " WHERE e.Surname = @filter";
                    }

                    query += " ORDER BY si.StartWork, e.FullName;";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        cmd.Parameters.AddWithValue("@filter", filter);
                    }
                    Dictionary<string, Dictionary<string, string>> employeeRota = new Dictionary<string, Dictionary<string, string>>();
                    // Map display key ("DayName date") -> date for ordering columns chronologically
                    Dictionary<string, DateTime> allDaysDates = new Dictionary<string, DateTime>();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string employeeName = reader["FullName"] as string;
                            if (string.IsNullOrWhiteSpace(employeeName)) continue; // skip rows without a matched employee

                            DateTime shiftStart = reader.GetDateTime(reader.GetOrdinal("StartWork"));
                            DateTime shiftEnd = reader.GetDateTime(reader.GetOrdinal("FinishWork"));
                            string dayOfWeek = reader["DayOfWeek"]?.ToString() ?? shiftStart.DayOfWeek.ToString();

                            string date = $"{shiftStart:d}";
                            string key = $"{dayOfWeek} {date}";

                            string shift = $"{shiftStart:t} - {shiftEnd:t}";

                            if (!employeeRota.ContainsKey(employeeName))
                                employeeRota[employeeName] = new Dictionary<string, string>();

                            // latest value wins if multiple entries for same employee/day
                            employeeRota[employeeName][key] = shift;

                            if (!allDaysDates.ContainsKey(key))
                                allDaysDates[key] = shiftStart.Date;
                        }
                        reader.Close();
                    }

                    DataTable rotaTable = new DataTable();
                    rotaTable.Columns.Add("Employee Name", typeof(string));

                    var orderedDayDates = allDaysDates.OrderBy(kv => kv.Value).Select(kv => kv.Key).ToList();
                    foreach (var dayDate in orderedDayDates)
                        rotaTable.Columns.Add(dayDate, typeof(string));

                    foreach (var kv in employeeRota)
                    {
                        DataRow row = rotaTable.NewRow();
                        row["Employee Name"] = kv.Key;
                        foreach (var dayDate in orderedDayDates)
                        {
                            row[dayDate] = kv.Value.ContainsKey(dayDate) ? kv.Value[dayDate] : string.Empty;
                        }
                        rotaTable.Rows.Add(row);
                    }

                    dataGridView1.DataSource = rotaTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Viewing Rota: " + ex.Message);
            }
        }

        private void ViewScheduleForm_Load(object sender, EventArgs e)
        {
            PopulateDataGridView();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Highlight rows that match the surname instead of reloading/clearing the grid
            try
            {
                string userInput = textBox1.Text.Trim();
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    // clear any highlighting
                    foreach (DataGridViewRow r in dataGridView1.Rows)
                    {
                        r.DefaultCellStyle.BackColor = Color.White;
                    }
                    return;
                }

                int firstMatchRow = -1;
                string filter = userInput;
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    var cell = r.Cells["Employee Name"];
                    if (cell == null || cell.Value == null)
                    {
                        r.DefaultCellStyle.BackColor = Color.White;
                        continue;
                    }
                    string fullName = cell.Value.ToString();
                    string surname = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
                    if (string.Equals(surname, filter, StringComparison.OrdinalIgnoreCase))
                    {
                        r.DefaultCellStyle.BackColor = Color.LightYellow;
                        if (firstMatchRow == -1) firstMatchRow = r.Index;
                    }
                    else
                    {
                        r.DefaultCellStyle.BackColor = Color.White;
                    }
                }

                // if we found a match, move selection to first match so user sees it
                if (firstMatchRow != -1 && dataGridView1.Rows.Count > firstMatchRow)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[firstMatchRow].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[firstMatchRow].Cells[0];
                }
                else
                {
                    // no match found: do not clear the grid, just leave highlighting cleared
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Filter error: " + ex.Message);
            }
        }
    }
}
