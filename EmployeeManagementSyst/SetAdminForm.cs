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
    public partial class SetAdminForm : Form
    {
        private string AdminId;


        /// <summary>
        /// Initializes the form with the provided employee ID and sets visual properties like form border style and background color.
        /// </summary>
        /// <param name="empid">The employee ID used to identify the employee.</param>
        public SetAdminForm(String empid)
        {
            this.AdminId = empid;
            InitializeComponent();

        }

        /// <summary>
        /// Handles the click event for the OK button. It retrieves the administrator information
        /// based on the provided ID and closes the form.
        /// </summary>
        private void Ok_Click(object sender, EventArgs e)
        {

            SetAdmin(AdminId);
            this.Close();

        }

        public static bool SetAdmin(string empid)
        {
            try
            {
                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                    // Check current role first
                    string checkQuery = "SELECT UserRole FROM EmployeeDetails WHERE Id = @Id";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, serverConnect))
                    {
                        checkCmd.Parameters.AddWithValue("@Id", empid);
                        object roleObj = checkCmd.ExecuteScalar();
                        if (roleObj == null || roleObj == DBNull.Value)
                        {
                            MessageBox.Show("No employee found with the provided ID.");
                            return false;
                        }

                        string currentRole = roleObj.ToString();
                        if (string.Equals(currentRole, "admin", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Employee is already an admin.");
                            return false;
                        }
                    }

                    // Update role to admin
                    string qry = "UPDATE EmployeeDetails SET UserRole = 'admin' WHERE Id = @Id;";
                    using (SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect))
                    {
                        mySqlCommand.Parameters.AddWithValue("@Id", empid);
                        int affected = mySqlCommand.ExecuteNonQuery();
                        if (affected > 0)
                        {
                            MessageBox.Show("Employee was made admin.");
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("No matching employee found to update.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Set Admin Error: " + ex.Message);
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

            var success = SetAdmin(AdminId);
            if (!success) { }

            string promotingAdminId = EmployeeHelper.GetIdByClockPin(userInput);
            string promotingAdminName = EmployeeHelper.GetNameById(promotingAdminId);

            string employeeName = EmployeeHelper.GetNameById(AdminId) ?? AdminId;
            var adminEmails = EmployeeHelper.GetAdminEmails();

            // Send notifications to admins
            if (adminEmails != null && adminEmails.Length > 0)
            {
                var subject = "Employee Role Update Notification";
                var body = $"Employee {employeeName} (ID: {AdminId}) has been promoted to admin by {promotingAdminName} (ID: {promotingAdminId}).";
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
