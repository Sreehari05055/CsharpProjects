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
    public partial class EmployeeDetailGrid : Form
    {
        private string searchParameter;
        private string serverConnection;
        public EmployeeDetailGrid()
        {          
            InitializeComponent();
            InitiateServer();
           
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            
        }
       public void Table() 
        {
            dataGridView1 = new DataGridView
            {
                AutoGenerateColumns = true,
                AllowUserToAddRows = false,
                Dock = DockStyle.Fill
            };
            Controls.Add(dataGridView1);
        
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
        private void Changing_Text(object sender,EventArgs s)
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
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("age", typeof(string));
                dataTable.Columns.Add("phonenumber", typeof(string));
                dataTable.Columns.Add("email", typeof(string));
                dataTable.Columns.Add("hourlyrate", typeof(string));

                using (SqlConnection serverConnect = new SqlConnection(serverConnection))
                {
                    serverConnect.Open();
                    string qry = "SELECT id,fullname,age,phonenumber,email,hourlyrate FROM employeedetails WHERE surname = @surname OR id = @id;";
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
                            row["age"] = reader["age"].ToString();
                            row["phonenumber"] = reader["phonenumber"].ToString();
                            row["email"] = reader["email"].ToString();
                            row["hourlyrate"] = reader["hourlyrate"].ToString();
                            dataTable.Rows.Add(row);
                        }
                        dataGridView1.DataSource = dataTable;
                    }
                    
                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        private void LoadAllData()
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("id", typeof(string));
                dataTable.Columns.Add("fullname", typeof(string));
                dataTable.Columns.Add("age", typeof(string));
                dataTable.Columns.Add("phonenumber", typeof(string));
                dataTable.Columns.Add("email", typeof(string));
                dataTable.Columns.Add("hourlyrate", typeof(string));

                using (SqlConnection connection = new SqlConnection(serverConnection))
                {
                    connection.Open();
                    string query = "SELECT id,fullname,age,phonenumber,email,hourlyrate FROM employeedetails";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            DataRow row = dataTable.NewRow();
                            row["id"] = reader["id"].ToString();
                            row["fullname"] = reader["fullname"].ToString();
                            row["age"] = reader["age"].ToString();
                            row["phonenumber"] = reader["phonenumber"].ToString();
                            row["email"] = reader["email"].ToString();
                            row["hourlyrate"] = reader["hourlyrate"].ToString();
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

        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
