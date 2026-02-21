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
        /// Removes an admin from the database based on the provided admin ID.
        /// </summary>
        /// <param name="id">The admin ID to be deleted from the database.</param>
        public static bool RevokeAdminPrivileges(string id)
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

                            // Confirm action with the user
                            var confirm = MessageBox.Show($"Revoke admin privileges for '{fullName ?? id}'?", "Confirm Revocation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (confirm != DialogResult.Yes) return false;
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
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("Failed to revoke admin privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Removing Admin: " + e.Message);
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var tb = sender as System.Windows.Forms.TextBox;
            if (tb == null) return;
            string digits = string.Empty;
            foreach (char c in tb.Text)
            {
                if (char.IsDigit(c)) digits += c;
            }
            if (digits.Length > tb.MaxLength) digits = digits.Substring(0, tb.MaxLength);
            if (tb.Text != digits)
            {
                int selStart = tb.SelectionStart - (tb.Text.Length - digits.Length);
                if (selStart < 0) selStart = 0;
                tb.Text = digits;
                tb.SelectionStart = selStart;
            }


        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter the employee Clock PIN to confirm deletion.", "Missing PIN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!EmployeeHelper.isAdmin(userInput))
            {
                MessageBox.Show("Clock PIN incorrect.", "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Perform revocation
            var success = RevokeAdminPrivileges(adminCode);
            if (!success) return;

            string terminatingAdminId = EmployeeHelper.GetIdByClockPin(userInput);
            string terminatingAdminName = EmployeeHelper.GetNameById(terminatingAdminId);

            string employeeName = EmployeeHelper.GetNameById(adminCode) ?? adminCode;
            var adminEmails = EmployeeHelper.GetAdminEmails();
            
            
            if (adminEmails != null && adminEmails.Length > 0)
            {
                string subject = $"Admin Privileges Revoked for {employeeName}";
                string body = $"Admin privileges for {employeeName} (ID: {adminCode}) have been revoked by {terminatingAdminName} (ID: {terminatingAdminId}).";

                var emailer = new EmailConfiguration();
                foreach (var admin in adminEmails)
                {
                    try
                    {
                        emailer.SendEmail(admin, subject, body);
                    }
                    catch (Exception ex)
                    {
                        // Log or inform; do not abort for a single failing recipient
                        MessageBox.Show("Error sending admin notification to: " + admin + "\n" + ex.Message, "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            this.Close();
        }
    }
}
