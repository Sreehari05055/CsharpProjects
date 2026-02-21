using Microsoft.Data.SqlClient;
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

        // Click handler for the confirmation button
        private void Yes_Click(object sender, EventArgs e)
        {
            if (IsEmployeeWorking(_employeeId))
            {
                MessageBox.Show("Cannot delete employee: employee is currently clocked in.");
                this.Close();
                return;
            }

            var success = DeleteEmployeeCascade(_employeeId);
            if (!success)
            {
                // Message already shown from DeleteEmployeeCascade
            }

            this.Close();
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
                var deletes = new (string Sql, string ParamName)[]
                {
                    ("DELETE FROM EmployeePayInfo WHERE EmployeeId = @id;", "@id"),
                    ("DELETE FROM CardInformation WHERE EmployeeId = @id;", "@id"),
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
