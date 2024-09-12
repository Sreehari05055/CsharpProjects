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
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeManagementSyst
{
    public partial class CheckStatus : Form
    {
        private string serverConnection;
        public CheckStatus()
        {

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            InitiateServer();
            EmployeeStatus();

        }
        public void EmployeeStatus()
        {
            try
            {
                using (SqlConnection server = new SqlConnection(serverConnection))
                {
                    server.Open();
                    string queryCheck = "SELECT empname, id FROM hourstable;";
                    SqlCommand payExec = new SqlCommand(queryCheck, server);


                    DataTable employeeTable = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(payExec);

                    adapter.Fill(employeeTable);

                    if (employeeTable.Rows.Count == 0)
                    {
                        MessageBox.Show("No one is working at the moment.");
                        return;
                    }

                    dataGridView1.DataSource = employeeTable;
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Checking Employee Status: " + ex.Message);
            }
        }

        public void InitiateServer()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("connectionString.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();

                // Get connection string
                string connectionString = configuration.GetConnectionString("EmployeeDatabase");

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Connection string 'EmployeeDatabase' not found in configuration file.");
                }

                serverConnection = connectionString;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
