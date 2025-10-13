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
           
            EmployeeDetails();
        }

        /// <summary>
        /// Fetches employee details from the database and displays them in the DataGridView.
        /// </summary>
        public void EmployeeDetails()
        {
            try
            {
                DataTable dataTable = new DataTable();
               
                dataTable.Columns.Add("fullname", typeof(string));


                using (SqlConnection serverConnect = MainPage.ConnectionString())
                {
       
                    string qry = "SELECT id,fullname FROM employeedetails;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["fullname"] = reader["fullname"].ToString();
                           
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
                string employeeName = row.Cells["fullname"].Value.ToString();
              
                using (SqlConnection serverConnect = MainPage.ConnectionString())
                {
         
                    string qry = "SELECT id FROM employeedetails WHERE fullname = @fname;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@fname",employeeName);

                    object result = mySqlCommand.ExecuteScalar();

                    if (result != null)
                    { 
                         string empId = result.ToString();
                         AddRota addRota = new AddRota(empId);
                        addRota.Show();
                        this.Close();
                    }
                    else { MessageBox.Show("Error Finding employee ID"); }
                    serverConnect.Close();
                    }
               
                }
            }
    }
}
