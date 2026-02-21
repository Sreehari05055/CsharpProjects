using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.NativeInterop;
using System;
using System.Windows.Forms;

namespace EmployeeManagementSyst
{
    public partial class DeleteEmployeeForm : Form
    {
        private readonly string _employeeId;

        public DeleteEmployeeForm(string id)
        {
            _employeeId = id ?? throw new ArgumentNullException(nameof(id));
            InitializeComponent();
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
                // Log or inform minimal output; treat as "working" to be safe
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

                // Delete statements for related tables. EmployeeDetails uses Id instead of EmployeeId.
                // AdminInformation table does not exist in this schema, so it is omitted.
                // Delete child records first, then the EmployeeDetails row
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DeleteEmployeeForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text?.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Please enter the employee Clock PIN to confirm deletion.", "Missing PIN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify the PIN exists
            if (!EmployeeHelper.isAdmin(userInput))
            {
                MessageBox.Show("Clock PIN incorrect.", "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string terminatingAdminId = EmployeeHelper.GetIdByClockPin(userInput);
            string terminatingAdminName = EmployeeHelper.GetNameById(terminatingAdminId);

            // Resolve employee id from the provided PIN and ensure it matches the employee being deleted
            string employeeIdFromPin = EmployeeHelper.GetIdByClockPin(_employeeId);
            if (string.IsNullOrWhiteSpace(employeeIdFromPin))
            {
                MessageBox.Show("Unable to resolve employee for the provided PIN.", "Verification Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if employee is currently clocked in
            if (IsEmployeeWorking(employeeIdFromPin))
            {
                MessageBox.Show("Cannot delete employee: employee is currently clocked in.", "Operation Aborted", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var success = DeleteEmployeeCascade(employeeIdFromPin);
            if (!success)
            {
                // DeleteEmployeeCascade shows its own error message
                return;
            }

            // Send notifications to admins
            string employeeName = EmployeeHelper.GetNameById(employeeIdFromPin) ?? employeeIdFromPin;
            var adminEmails = EmployeeHelper.GetAdminEmails();
            if (adminEmails != null && adminEmails.Length > 0)
            {
                var subject = "Employee Termination Confirmation";
                var body = $"Employee {employeeName} (ID: {employeeIdFromPin}) has been terminated from the system by Admin {terminatingAdminName} (ID: {terminatingAdminId})";
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // sanitize input in case of paste: keep only digits and limit to MaxLength
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
    }
}
