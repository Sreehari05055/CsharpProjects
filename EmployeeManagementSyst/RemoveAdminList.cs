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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class RemoveAdminList : Form
    {
        
        public RemoveAdminList()
        {
            InitializeComponent();         
        }


        /// <summary>
        /// Handles the DataGridView cell click event.
        /// Retrieves admin information and opens the RemoveAdmin form.
        /// </summary>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeName = row.Cells["Admin Name"].Value.ToString();
                string code = row.Cells["Id"].Value.ToString();


                if (string.IsNullOrWhiteSpace(code))
                {
                    MessageBox.Show("Error: selected row does not contain an Id.");
                    return;
                }

                // Verify acting user is an admin using existing AdminVerification dialog
                var verify = new AdminVerification();
                verify.ReturnDialogResultOnSuccess = true;
                var dr = verify.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    // Verification failed or cancelled
                    return;
                }

                // Ask for final confirmation since this is a high-risk action
                string targetName = EmployeeHelper.GetNameById(code) ?? code;
                var confirmMsg = $"THIS IS A HIGH RISK ACTION:\n\nYou are about to revoke admin privileges for {targetName} (ID: {code}).\nAll existing admins will be notified of this action.\n\nDo you want to proceed?";
                var confirm = MessageBox.Show(confirmMsg, "Confirm Revocation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes)
                {
                    return;
                }

                // Perform revocation from this list
                string actingAdminId = verify.VerifiedAdminId;
                string actingAdminName = verify.VerifiedAdminName;
                var success = RevokeAdmin(code, actingAdminId, actingAdminName);
                if (success)
                {
                    this.Close();
                }
            }
        }
        /// <summary>
        /// Handles text changes in the input TextBox.
        /// Filters the admin list based on user input.
        /// </summary>
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
                dataTable.Columns.Add("Admin Name", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                   
                    // Search admins directly in EmployeeDetails. Use case-insensitive surname match.
                    string qry = "SELECT Id AS EmployeeId, FullName AS AdminName FROM EmployeeDetails " +
                                 "WHERE UserRole = 'admin' AND (LOWER(Surname) LIKE '%' + @surname + '%' OR Id = @id);";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    using (SqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Id"] = reader["EmployeeId"].ToString();
                                row["Admin Name"] = reader["AdminName"].ToString();

                                dataTable.Rows.Add(row);
                            }
                            dataGridView1.DataSource = dataTable;
                        }
                    }
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Admin Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all admin details from the database into the DataGridView.
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("Admin Name", typeof(string));



                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {

                    string query = "SELECT Id, FullName FROM EmployeeDetails WHERE UserRole = 'admin';";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                row["Id"] = reader["Id"].ToString();
                                row["Admin Name"] = reader["FullName"].ToString();

                                dataTable.Rows.Add(row);
                            }
                            dataGridView1.DataSource = dataTable;
                        }
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
        /// Event handler for the form's Load event.
        /// Loads all admin details when the form is loaded.
        /// </summary>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }

        /// <summary>
        /// Revoke admin privileges for the specified admin id and notify all admins.
        /// </summary>
        private bool RevokeAdmin(string adminId, string actingAdminId, string actingAdminName)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    // Verify the user exists and is currently an admin
                    string checkQuery = "SELECT UserRole, FullName FROM EmployeeDetails WHERE Id = @id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", adminId);
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("Admin not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                            reader.Read();
                            var role = reader["UserRole"]?.ToString();
                            var fullName = reader["FullName"]?.ToString();
                            if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("The selected user is not an admin.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }

                    // Revoke admin privileges
                    string revokeSql = "UPDATE EmployeeDetails SET UserRole = 'employee' WHERE Id = @id";
                    using (SqlCommand revokeCmd = new SqlCommand(revokeSql, conn))
                    {
                        revokeCmd.Parameters.AddWithValue("@id", adminId);
                        int rowsAffected = revokeCmd.ExecuteNonQuery();
                        if (rowsAffected <= 0)
                        {
                            MessageBox.Show("Failed to revoke admin privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }

                    // Notify admins
                    string employeeName = EmployeeHelper.GetNameById(adminId) ?? adminId;
                    var adminEmails = EmployeeHelper.GetAdminEmails();
                    if (adminEmails != null && adminEmails.Length > 0)
                    {
                        string subject = $"Admin Privileges Revoked for {employeeName}";
                        string body = $"Admin privileges for {employeeName} (ID: {adminId}) have been revoked by {actingAdminName} (ID: {actingAdminId}).";

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

                    MessageBox.Show("Admin privileges revoked.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Removing Admin: " + e.Message);
                return false;
            }
        }
    }
}
