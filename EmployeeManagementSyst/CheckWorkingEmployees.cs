using Microsoft.Data.SqlClient;

using System.Data;


namespace EmployeeManagementSyst
{
    public partial class CheckWorkingEmployees : Form
    {
        public CheckWorkingEmployees()
        {

            InitializeComponent();
            EmployeeStatus();

        }

        /// <summary>
        /// Fetches the current status of employees from the database and displays it in the DataGridView.
        /// </summary>
        public void EmployeeStatus()
        {
            try
            {
                using (SqlConnection server = ServerConnection.GetOpenConnection())
                {
                    string queryCheck = "SELECT EmployeeName AS empname, EmployeeId AS id FROM TimeLogs WHERE EndTime IS NULL;";
                    using (SqlCommand payExec = new SqlCommand(queryCheck, server))
                    {
                        DataTable employeeTable = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(payExec))
                        {
                            adapter.Fill(employeeTable);
                        }

                        if (employeeTable.Rows.Count > 0)
                        {
                            dataGridView1.DataSource = employeeTable;
                        }
                        else
                        {
                            MessageBox.Show("No one is working at the moment.");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Checking Employee Status: " + ex.Message);
            }
        }
    }
}
