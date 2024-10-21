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
    public partial class DeleteEmpGrid : Form
    {
        private string serverConnection;
        public DeleteEmpGrid()
        {
            InitializeComponent();
            InitiateServer();
            EmployeeDetails();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
        }
        // Method to load employee details from the database
        public void EmployeeDetails()
        {
            try
            {
                DataTable dataTable = new DataTable();

                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("Id",typeof(string));


                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
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
                }
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        // Method to initiate server connection using configuration file
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
        // Method to handle cell clicks in the DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the current row
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string employeeName = row.Cells["fullname"].Value.ToString();
                string code = row.Cells["Id"].Value.ToString();

                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    string qry = "SELECT id FROM employeedetails WHERE fullname = @fname OR id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.AddWithValue("@fname", employeeName);
                    mySqlCommand.Parameters.AddWithValue("@id", code);

                    object result = mySqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string empId = result.ToString();
                        DeleteEmp deleteEmp = new DeleteEmp(empId);
                        deleteEmp.Show();
                        this.Close();
                    }
                    else { MessageBox.Show("Error Finding employee ID"); }
                }
            }
        }
        // Method to filter employee details based on user input
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
               

                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
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

                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        // Method to load all employee data into the grid
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("fullname", typeof(string));
     
       

                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
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


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }

        // Method to load employee details when the form loads
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
