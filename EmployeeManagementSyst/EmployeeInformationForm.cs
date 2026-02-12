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
    public partial class EmployeeInformationForm : Form
    {
        
        public EmployeeInformationForm()
        {          
            InitializeComponent();

            
        }
        /// <summary>
        /// Event handler for text changes in the input textbox. Filters employee data based on the user's input.
        /// If the input is empty, it loads all employee data.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the text change event.</param>
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

                using (SqlConnection serverConnect = ServerConnection.GetOpenConnection())
                {
                   
                    string qry = "SELECT Id,FullName,Age,PhoneNumber,Email,HourlyRate FROM EmployeeDetails WHERE Surname = @surname OR Id = @id;";
                    SqlCommand mySqlCommand = new SqlCommand(qry, serverConnect);
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.Parameters.AddWithValue("@surname", userInput);
                    mySqlCommand.Parameters.AddWithValue("@id", userInput);
                    using (SqlDataReader reader = mySqlCommand.ExecuteReader())
                    {
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
                    serverConnect.Close();
                }

            }

            catch (Exception ex) { MessageBox.Show("Employee Details Error: " + ex.Message); }
        }
        /// <summary>
        /// Loads all employee data into the DataGridView when there are no filters applied.
        /// </summary>
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

                using (SqlConnection connection = ServerConnection.GetOpenConnection())
                {
                   
                    string query = "SELECT Id,FullName,Age,PhoneNumber,Email,HourlyRate FROM EmployeeDetails";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
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
                    
                   connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
        }
        /// <summary>
        /// Event handler for when the form loads. This is where all employee data is loaded initially.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event data for the form's load event.</param>
        private void EmployeeDetailGrid_Load(object sender, EventArgs e)
        {
            LoadAllData();
        }
    }
}
