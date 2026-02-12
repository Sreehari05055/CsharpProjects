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
    public partial class DeleteEmployeeList : Form
    {

        /// <summary>
        /// The DeleteEmpGrid class is responsible for displaying, searching, and managing employee details.
        /// It provides functionality to delete employee data by interacting with the database.
        /// </summary>
        public DeleteEmployeeList()
        {
            InitializeComponent();
          
            EmployeeDetails();
        }
        /// <summary>
        /// Loads the employee details from the database into a DataGridView.
        /// </summary>
        public void EmployeeDetails()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("Id",typeof(string));


                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
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
                            row["Id"] = reader["id"].ToString();

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
        /// Handles the cell click event of the DataGridView. When a row is clicked, the employee's ID and fullname
        /// are retrieved and passed to the DeleteEmp form for deletion.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data containing information about the clicked cell.</param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeName = row.Cells["fullname"].Value.ToString();
                string code = row.Cells["Id"].Value.ToString();

                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
         
                    string qry = "SELECT id FROM employeedetails WHERE fullname = @fname OR id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@fname", employeeName);
                    mySqlCommand.Parameters.AddWithValue("@id", code);

                    object result = mySqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string empId = result.ToString();
                        DeleteEmployeeForm deleteEmp = new DeleteEmployeeForm(empId);
                        deleteEmp.Show();
                        this.Close();
                    }
                    else { MessageBox.Show("Error Finding employee ID"); }
                serverConnect.Close();
                }
            }
        }
        /// <summary>
        /// Filters the employee details displayed in the DataGridView based on user input in the search textbox.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="s">Event data containing the user input from the text box.</param>
        private void Changing_Text(object sender, EventArgs s)
        {
            string userInput = textBox1.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(userInput))
            {

                LoadAllData();
                return;
            }
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("Id", typeof(string));
               

                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                    
                    string qry = "SELECT id,fullname FROM employeedetails WHERE surname = @surname OR id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    SqlDataReader reader = mySqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader["id"].ToString();
                            row["fullname"] = reader["fullname"].ToString();
                       
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all employee data into the DataGridView without any filters.
        /// </summary>
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("fullname", typeof(string));
     
       

                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                    
                    string query = "SELECT id,fullname FROM employeedetails";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader["id"].ToString();
                            row["fullname"] = reader["fullname"].ToString();
                  
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;

                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads all employee details when the form is loaded.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the form's load event.</param>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
