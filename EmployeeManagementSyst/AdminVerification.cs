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
using System.Web;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class AdminVerification : Form
    {
        // If set, after successful verification the form will open DeleteEmployeeForm for this employee id
        public string PendingDeleteEmployeeId { get; set; }
        // When used as a dialog, these are set when verification succeeds
        public string VerifiedAdminId { get; private set; }
        public string VerifiedAdminName { get; private set; }
        // If true, the dialog will return DialogResult.OK when verification succeeds
        public bool ReturnDialogResultOnSuccess { get; set; } = false;

        public AdminVerification()
        {
            InitializeComponent();

        }
        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Event handler for the OK button click. Verifies the admin code input by the user.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Ok_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text.Trim();

            AdminVerify(userInput);

        }

        /// <summary>
        /// Verifies if the provided admin code exists in the database.
        /// </summary>
        /// <param name="adminCode">The admin code to be verified.</param>
        public void AdminVerify(string adminCode)
        {
            try
            {
                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {

                    // Check that the provided clock pin belongs to a user and retrieve id/name/role
                    string querytoCheck = "SELECT Id, FullName, UserRole FROM EmployeeDetails WHERE ClockPin = @clockPin;";
                    using (SqlCommand mySqlCommand = new SqlCommand(querytoCheck, serverConnect))
                    {
                        mySqlCommand.Parameters.AddWithValue("@clockPin", adminCode);
                        using (var reader = mySqlCommand.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                // invalid code, allow user to retry
                                MessageBox.Show("Code incorrect");
                                return;
                            }

                            string role = reader["UserRole"]?.ToString() ?? string.Empty;
                            string adminId = reader["Id"]?.ToString();
                            string adminName = reader["FullName"]?.ToString();

                            if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                MessageBox.Show("Access denied: user is not an admin");
                                return;
                            }

                            // Verified admin
                            VerifiedAdminId = adminId;
                            VerifiedAdminName = adminName;

                            // If caller wants the dialog to return OK, or a pending delete id is set,
                            // return DialogResult.OK so the caller can proceed.
                            if (ReturnDialogResultOnSuccess || !string.IsNullOrEmpty(PendingDeleteEmployeeId))
                            {
                                this.DialogResult = DialogResult.OK;
                                this.Close();
                                return;
                            }

                            // No special return behavior requested: open admin page (preserve previous behavior)
                            AdminForm page = new AdminForm();
                            page.Show();
                            this.Close();
                            return;
                        }
                    }
                    serverConnect.Close();
                }

            }
            catch (Exception ex) { Console.WriteLine("Admin Verification Error: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
