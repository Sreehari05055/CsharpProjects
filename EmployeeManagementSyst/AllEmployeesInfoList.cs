using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class AllEmployeesInfoList : Form
    {
        public AllEmployeesInfoList()
        {
            InitializeComponent();
        }

        private void Changing_Text(object sender, EventArgs e)
        {
            string userInput = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                LoadAllData();
                return;
            }

            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FullName", typeof(string));
                dataTable.Columns.Add("Age", typeof(string));
                dataTable.Columns.Add("PhoneNumber", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("HourlyRate", typeof(string));

                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string qry = "SELECT Id, FullName, Age, PhoneNumber, Email, HourlyRate, ClockPin FROM EmployeeDetails WHERE LOWER(Surname) LIKE '%' + @surname + '%' OR Id = @id;";
                    using (SqlCommand cmd = new SqlCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@surname", userInput);
                        cmd.Parameters.AddWithValue("@id", userInput);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = dataTable.NewRow();
                                row["Id"] = reader["Id"].ToString();
                                row["FullName"] = reader["FullName"].ToString();
                                row["Age"] = reader["Age"]?.ToString();
                                row["PhoneNumber"] = reader["PhoneNumber"]?.ToString();
                                row["Email"] = reader["Email"]?.ToString();
                                row["HourlyRate"] = reader["HourlyRate"]?.ToString();
                                row["ClockPin"] = reader["ClockPin"]?.ToString();
                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                }

                dataGridView1.DataSource = dataTable;
                // Make Id and ClockPin columns readonly
                if (dataGridView1.Columns["Id"] != null) dataGridView1.Columns["Id"].ReadOnly = true;
                if (dataGridView1.Columns["ClockPin"] != null) dataGridView1.Columns["ClockPin"].ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching employees: " + ex.Message);
            }
        }

        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FullName", typeof(string));
                dataTable.Columns.Add("Age", typeof(string));
                dataTable.Columns.Add("PhoneNumber", typeof(string));
                dataTable.Columns.Add("Email", typeof(string));
                dataTable.Columns.Add("HourlyRate", typeof(string));
                dataTable.Columns.Add("ClockPin", typeof(string));

                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string query = "SELECT Id, FullName, Age, PhoneNumber, Email, HourlyRate, ClockPin FROM EmployeeDetails ORDER BY FullName;";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["Id"] = reader["Id"].ToString();
                            row["FullName"] = reader["FullName"].ToString();
                            row["Age"] = reader["Age"]?.ToString();
                            row["PhoneNumber"] = reader["PhoneNumber"]?.ToString();
                            row["Email"] = reader["Email"]?.ToString();
                            row["HourlyRate"] = reader["HourlyRate"]?.ToString();
                            row["ClockPin"] = reader["ClockPin"]?.ToString();
                            dataTable.Rows.Add(row);
                        }
                    }
                }

                dataGridView1.DataSource = dataTable;
                // mark id and clockpin as readonly
                if (dataGridView1.Columns["Id"] != null) dataGridView1.Columns["Id"].ReadOnly = true;
                if (dataGridView1.Columns["ClockPin"] != null) dataGridView1.Columns["ClockPin"].ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message);
            }
        }

        private void AllEmployeesInfoList_Load(object sender, EventArgs e)
        {
            // Setup grid for inline editing and hook handler
            dataGridView1.ReadOnly = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.CellEndEdit -= dataGridView1_CellEndEdit;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;

            LoadAllData();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                var grid = (DataGridView)sender;
                var id = grid.Rows[e.RowIndex].Cells["Id"].Value?.ToString();
                var columnName = grid.Columns[e.ColumnIndex].Name;
                var newValue = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();

                if (string.IsNullOrWhiteSpace(id))
                {
                    MessageBox.Show("Cannot determine employee Id for update.");
                    LoadAllData();
                    return;
                }

                // Protect Id and ClockPin from edits
                if (string.Equals(columnName, "Id", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(columnName, "ClockPin", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("This field is not editable.");
                    LoadAllData();
                    return;
                }

                var ok = UpdateEmployeeField(id, columnName, newValue);
                if (!ok)
                {
                    LoadAllData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating value: " + ex.Message);
                LoadAllData();
            }
        }

        private bool UpdateEmployeeField(string id, string column, string value)
        {
            try
            {
                // Safety: prevent updating Id/ClockPin here
                if (string.Equals(column, "Id", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(column, "ClockPin", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("This column cannot be updated.");
                    return false;
                }
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    if (string.Equals(column, "FullName", StringComparison.OrdinalIgnoreCase))
                    {
                        // When full name changes, also update surname (last token) and related TimeLogs employee name
                        string surname = string.Empty;
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 0) surname = parts[^1];
                        }

                        string sql = "UPDATE EmployeeDetails SET FullName = @val, Surname = @surname WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@val", (object)value ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@surname", (object)surname ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", id);
                            int affected = cmd.ExecuteNonQuery();
                            if (affected > 0)
                            {
                                // also update TimeLogs.EmployeeName to keep display names in sync
                                try
                                {
                                    using (var tcmd = new SqlCommand("UPDATE TimeLogs SET EmployeeName = @name WHERE EmployeeId = @id", conn))
                                    {
                                        tcmd.Parameters.AddWithValue("@name", (object)value ?? DBNull.Value);
                                        tcmd.Parameters.AddWithValue("@id", id);
                                        tcmd.ExecuteNonQuery();
                                    }
                                }
                                catch { /* non-fatal */ }

                                return true;
                            }

                            MessageBox.Show("No rows were updated.");
                            return false;
                        }
                    }

                    string sqlUpdate = $"UPDATE EmployeeDetails SET [{column}] = @val WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@val", (object)value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", id);
                        int affected = cmd.ExecuteNonQuery();
                        if (affected > 0) return true;
                        MessageBox.Show("No rows were updated.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating database: " + ex.Message);
                return false;
            }
        }

        
    }
}
