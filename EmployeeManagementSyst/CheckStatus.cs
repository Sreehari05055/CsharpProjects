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
        public CheckStatus()
        {

            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            EmployeeStatus();

        }

        /// <summary>
        /// Fetches the current status of employees from the database and displays it in the DataGridView.
        /// </summary>
        public void EmployeeStatus()
        {
            try
            {
                using (SqlConnection server = MainPage.ConnectionString())
                {
                
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
                   server.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Checking Employee Status: " + ex.Message);
            }
        }
        /// <summary>
        /// Event handler for when a cell in the DataGridView is clicked.
        /// (Currently not implemented)
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments containing cell click details.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
