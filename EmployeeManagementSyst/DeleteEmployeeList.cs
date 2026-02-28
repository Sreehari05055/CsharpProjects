using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
    public partial class DeleteEmployeeList : Form
    {

        /// <summary>
        /// The DeleteEmpGrid class is responsible for displaying, searching, and managing employee details.
        /// It provides functionality to delete employee data by interacting with the database.
        /// </summary>
        public DeleteEmployeeList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the cell click event of the DataGridView. When a row is clicked, the employee's ID and fullname
        /// are retrieved and passed to the DeleteEmp form for deletion.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data containing information about the clicked cell.</param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeName = (dataGridView1.Columns["fullname"] != null && row.Cells["fullname"].Value != null)
                    ? row.Cells["fullname"].Value.ToString()
                    : string.Empty;

                string code = null;
                if (dataGridView1.Columns["Id"] != null && row.Cells["Id"].Value != null)
                    code = row.Cells["Id"].Value.ToString();
                else if (dataGridView1.Columns["id"] != null && row.Cells["id"].Value != null)
                    code = row.Cells["id"].Value.ToString();

                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Error: employee ID is missing");
                    return;
                }

                // Ask for admin verification as a modal dialog
                AdminVerification verify = new AdminVerification
                {
                    PendingDeleteEmployeeId = code
                };

                var result = verify.ShowDialog();
                if (result != DialogResult.OK)
                {
                    // Verification cancelled or failed; do nothing
                    return;
                }

                // Verified admin info
                string terminatingAdminId = verify.VerifiedAdminId;
                string terminatingAdminName = verify.VerifiedAdminName;

                // Ask for final confirmation before deleting (inform that notification will be sent to all admins)
                string displayNameForConfirm = !string.IsNullOrWhiteSpace(employeeName) ? employeeName : code;
                string confirmMessage = $"Are you sure you want to permanently delete employee '{displayNameForConfirm}' (ID: {code})?\n\nThis action cannot be undone and a notification will be sent to all admins.";
                var confirmResult = MessageBox.Show(confirmMessage, "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (confirmResult != DialogResult.Yes)
                {
                    // User cancelled the deletion
                    return;
                }

                // Perform deletion flow here
                try
                {
                    if (IsEmployeeWorking(code))
                    {
                        MessageBox.Show("Cannot delete employee: employee is currently clocked in.", "Operation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var success = DeleteEmployeeCascade(code);
                    if (!success)
                    {
                        return;
                    }

                    // Send notifications to admins
                    string empDisplayName = EmployeeHelper.GetNameById(code) ?? code;
                    var adminEmails = EmployeeHelper.GetAdminEmails();
                    if (adminEmails != null && adminEmails.Length > 0)
                    {
                        var subject = "Employee Termination Confirmation";
                        var bodyAdminName = string.IsNullOrEmpty(terminatingAdminName) ? "(unknown)" : terminatingAdminName;
                        var bodyAdminId = string.IsNullOrEmpty(terminatingAdminId) ? "(unknown)" : terminatingAdminId;
                        var body = $"Employee {empDisplayName} (ID: {code}) has been terminated from the system by Admin {bodyAdminName} (ID: {bodyAdminId})";
                        var emailer = new EmailConfiguration();
                        foreach (var admin in adminEmails)
                        {
                            try
                            {
                                emailer.SendEmail(admin, subject, body);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error sending admin notification to: " + admin + "\n" + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error performing delete: " + ex.Message);
                }

                this.Close();
            }
        }
        /// <summary>
        /// Filters the employee details displayed in the DataGridView based on user input in the search textbox.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="s">Event data containing the user input from the text box.</param>
        private void Changing_Text(object sender, EventArgs s)
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
                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {

                    string qry = "SELECT Id,FullName FROM EmployeeDetails WHERE Surname = @surname OR Id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader["id"].ToString();
                            row["fullname"] = reader["fullname"].ToString();

                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all employee data into the DataGridView without any filters.
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("fullname", typeof(string));



                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {

                    string query = "SELECT Id,FullName FROM EmployeeDetails WHERE UserRole = 'employee';";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader["id"].ToString();
                            row["fullname"] = reader["fullname"].ToString();

                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;

                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads all employee details when the form is loaded.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the form's load event.</param>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }

        private void DeletEmployeeList_Click(object sender, EventArgs e)
        {

        }

        // Returns true if the employee currently has an open TimeLogs entry
        private static bool IsEmployeeWorking(string id)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var cmd = new SqlCommand("SELECT COUNT(1) FROM TimeLogs WHERE EmployeeId = @id AND EndTime IS NULL;", conn);
                cmd.Parameters.AddWithValue("@id", id);
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking work status: " + ex.Message);
                return true;
            }
        }

        // Performs a transactional cascade delete across related tables.
        private static bool DeleteEmployeeCascade(string id)
        {
            try
            {
                using var conn = ServerConnection.GetOpenConnection();
                using var tx = conn.BeginTransaction();

                var deletes = new (string Sql, string ParamName)[]
                {
                    ("DELETE FROM TimeLogs WHERE EmployeeId = @id;", "@id"),
                    ("DELETE FROM EmployeePayInfo WHERE EmployeeId = @id;", "@id"),
                    ("DELETE FROM CardInformation WHERE EmployeeId = @id;", "@id"),
                    ("DELETE FROM ScheduleInformation WHERE EmployeeId = @id;", "@id"),
                    ("DELETE FROM EmployeeDetails WHERE Id = @id;", "@id")
                };

                foreach (var (sql, param) in deletes)
                {
                    using var cmd = new SqlCommand(sql, conn, tx);
                    cmd.Parameters.AddWithValue(param, id);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                MessageBox.Show("Employee and related records deleted successfully.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting employee: " + ex.Message);
                return false;
            }
        }
    }
}
