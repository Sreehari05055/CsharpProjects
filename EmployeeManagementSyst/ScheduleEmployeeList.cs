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
    public partial class AllEmployees : Form
    {
        public AllEmployees()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Fetches employee details from the database and displays them in the DataGridView.
        /// </summary>
        public void EmployeeDetails()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("Fullname", typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {

                    string qry = "SELECT Id,FullName FROM EmployeeDetails;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["Id"] = reader["Id"].ToString();
                            row["Fullname"] = reader["Fullname"].ToString();

                            dataTable.Rows.Add(row);
                        }
                    }
                    else { MessageBox.Show("Employee not found"); }
                    serverConnect.Close();
                }
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Event handler for cell clicks in the DataGridView.
        /// Opens the AddRota form for the selected employee.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments containing cell click details.</param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeId = row.Cells["Id"].Value.ToString();
                string employeeName = row.Cells["Fullname"].Value.ToString();

                if (employeeId != null)
                {
                    EmployeeScheduleForm addRota = new EmployeeScheduleForm(employeeId);
                    addRota.Show();
                    this.Close();
                }

            }
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string userInput = textBox1.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(userInput))
            {
                EmployeeDetails();
                return;
            }

            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("Id", typeof(string));
                dataTable.Columns.Add("FullName", typeof(string));


                using (SqlConnection conn = ServerConnection.GetOpenConnection())
                {
                    string qry = "SELECT Id, FullName FROM EmployeeDetails WHERE Surname = @surname OR Id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, conn);


                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var row = dataTable.NewRow();
                            row["Id"] = reader["Id"].ToString();
                            row["FullName"] = reader["FullName"].ToString();
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching employees: " + ex.Message);
            }

        }

        private void AllEmployees_Load(object sender, EventArgs e)
        {
            EmployeeDetails();
        }
    }
}
