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
    public partial class TerminateAdminForm : Form
    {
        private string adminCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateAdminForm"/> class.
        /// </summary>
        /// <param name="adminCode">The admin code (ID) of the admin to be removed.</param>
        public TerminateAdminForm(string adminCode)
        {
            this.adminCode = adminCode;
            InitializeComponent();

        }
        /// <summary>
        /// Event handler for the 'OK' button click. Removes the admin and closes the form.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {

            RevokeAdminPrivileges(adminCode);
            this.Close();
        }


        /// <summary>
        /// Removes an admin from the database based on the provided admin ID.
        /// </summary>
        /// <param name="id">The admin ID to be deleted from the database.</param>
        public void RevokeAdminPrivileges(string id)
        {
            try
            {
                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    // Verify the user exists and is currently an admin
                    string checkQuery = "SELECT UserRole, FullName FROM EmployeeDetails WHERE Id = @id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        using (var reader = checkCmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show("Admin not found.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            reader.Read();
                            var role = reader["UserRole"]?.ToString();
                            var fullName = reader["FullName"]?.ToString();
                            if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("The selected user is not an admin.", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // Confirm action with the user
                            var confirm = MessageBox.Show($"Revoke admin privileges for '{fullName ?? id}'?", "Confirm Revocation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (confirm != DialogResult.Yes) return;
                        }
                    }

                    // Revoke admin privileges
                    string revokeSql = "UPDATE EmployeeDetails SET UserRole = 'employee' WHERE Id = @id";
                    using (SqlCommand revokeCmd = new SqlCommand(revokeSql, conn))
                    {
                        revokeCmd.Parameters.AddWithValue("@id", id);
                        int rowsAffected = revokeCmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Admin privileges revoked.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to revoke admin privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

            }
            catch (Exception e) { Console.WriteLine("Error Removing Admin: " + e.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
