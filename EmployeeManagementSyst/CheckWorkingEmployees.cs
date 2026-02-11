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
                
                    string queryCheck = "SELECT empname, id FROM hourstable;";
                    SqlCommand payExec = new SqlCommand(queryCheck, server);
                    SqlDataReader reader = payExec.ExecuteReader();

                    if (/*employeeTable.Rows.Count != 0*/ reader.Read())
                    {
                        DataTable employeeTable = new DataTable();
                        SqlDataAdapter adapter = new SqlDataAdapter(payExec);

                        adapter.Fill(employeeTable);
                        dataGridView1.DataSource = employeeTable;
                    }
                    else
                    {
                        MessageBox.Show("No one is working at the moment.");
                        return;
                    }

                    server.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Checking Employee Status: " + ex.Message);
            }
        }
    }
}
